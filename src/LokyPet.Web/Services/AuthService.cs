namespace LokyPet.Client.Services;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;
using LokyPet.Domain.Enums;
using LokyPet.Infrastructure.Configuration;

/// <summary>
/// Implementación de IAuthService usando Supabase GoTrue (Auth) + PostgREST (user_profiles).
/// Toda la comunicación es via HTTP desde el navegador (WASM).
/// </summary>
public sealed class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly string _authUrl;
    private readonly string _restUrl;
    private readonly string _anonKey;

    private string? _accessToken;
    private UserProfile? _cachedUser;

    public event Action? OnAuthStateChanged;

    /// <summary>Last error message from a failed auth operation (for UI debugging).</summary>
    public string? LastError { get; private set; }

    /// <summary>Current access token (null if not logged in).</summary>
    public string? AccessToken => _accessToken;

    public AuthService(SupabaseConfig config)
    {
        _http = new HttpClient();
        _authUrl = $"{config.Url}/auth/v1";
        _restUrl = $"{config.Url}/rest/v1";
        _anonKey = config.AnonKey;
    }

    // ── Auth operations ──

    public async Task<bool> SignUpAsync(string email, string password, string fullName)
    {
        LastError = null;
        try
        {
            // Send full_name in user metadata so the DB trigger can pick it up
            // and auto-create the user_profiles row (bypasses RLS issues).
            var request = CreateAuthRequest(HttpMethod.Post, $"{_authUrl}/signup");
            request.Content = JsonContent.Create(new
            {
                email,
                password,
                data = new { full_name = fullName.Trim() }
            }, options: JsonOpts);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                LastError = $"Signup HTTP {(int)response.StatusCode}: {errorBody}";
                return false;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            AuthResponse? result;
            try
            {
                result = JsonSerializer.Deserialize<AuthResponse>(responseBody, JsonOpts);
            }
            catch
            {
                LastError = $"Could not parse signup response: {responseBody[..Math.Min(200, responseBody.Length)]}";
                return false;
            }

            var userId = result?.User?.Id;
            if (userId is null)
            {
                // Some Supabase setups return user data at the top level (no nested "user" object)
                try
                {
                    var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("id", out var idProp))
                        userId = idProp.GetString();
                }
                catch { /* ignore */ }

                if (userId is null)
                {
                    LastError = $"No user ID in signup response: {responseBody[..Math.Min(200, responseBody.Length)]}";
                    return false;
                }
            }

            // When email confirmation is DISABLED, Supabase returns an access_token
            // and we can auto-login immediately. When ENABLED, there is no token and
            // the user must confirm their email first — the DB trigger already created
            // the profile row so we just return success.
            if (result?.AccessToken is not null)
            {
                _accessToken = result.AccessToken;
                _cachedUser = null;
                OnAuthStateChanged?.Invoke();
            }

            return true;
        }
        catch (Exception ex)
        {
            LastError = $"Exception: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        LastError = null;
        try
        {
            var request = CreateAuthRequest(HttpMethod.Post, $"{_authUrl}/token?grant_type=password");
            request.Content = JsonContent.Create(new { email, password }, options: JsonOpts);

            var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                LastError = $"Login HTTP {(int)response.StatusCode}: {errorBody}";
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOpts);
            if (result?.AccessToken is null)
            {
                LastError = "No access_token in login response";
                return false;
            }

            _accessToken = result.AccessToken;
            _cachedUser = null;
            OnAuthStateChanged?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            LastError = $"Exception: {ex.Message}";
            return false;
        }
    }

    public Task SignOutAsync()
    {
        _accessToken = null;
        _cachedUser = null;
        OnAuthStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public async Task<bool> ResetPasswordAsync(string email)
    {
        try
        {
            var request = CreateAuthRequest(HttpMethod.Post, $"{_authUrl}/recover");
            request.Content = JsonContent.Create(new { email }, options: JsonOpts);
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    // ── Profile operations ──

    public async Task<UserProfile?> GetCurrentUserAsync()
    {
        if (_accessToken is null) return null;
        if (_cachedUser is not null) return _cachedUser;

        try
        {
            // Get user from GoTrue
            var userReq = CreateAuthRequest(HttpMethod.Get, $"{_authUrl}/user");
            userReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            var userResp = await _http.SendAsync(userReq);
            if (!userResp.IsSuccessStatusCode)
            {
                _accessToken = null;
                return null;
            }

            var authUser = await userResp.Content.ReadFromJsonAsync<AuthUser>(JsonOpts);
            if (authUser?.Id is null) return null;

            // Get profile from user_profiles
            var profileReq = CreateRestRequest(HttpMethod.Get,
                $"{_restUrl}/user_profiles?id=eq.{authUser.Id}&select=*");
            var profiles = await _http.SendAsync(profileReq);

            if (profiles.IsSuccessStatusCode)
            {
                var rows = await profiles.Content.ReadFromJsonAsync<List<UserProfileRow>>(JsonOpts);
                var row = rows?.FirstOrDefault();
                if (row is not null)
                {
                    _cachedUser = new UserProfile
                    {
                        Id = Guid.Parse(authUser.Id),
                        Email = authUser.Email,
                        FullName = row.FullName ?? "",
                        Phone = row.Phone,
                        DefaultAddress = row.DefaultAddress is not null ? JsonSerializer.Serialize(row.DefaultAddress) : null,
                        AvatarKey = row.AvatarKey,
                        Role = row.Role == "admin" ? UserRole.Admin : UserRole.Customer,
                        CreatedAt = row.CreatedAt,
                        UpdatedAt = row.UpdatedAt
                    };
                    return _cachedUser;
                }
            }

            // Profile row doesn't exist yet — return basic user
            _cachedUser = new UserProfile
            {
                Id = Guid.Parse(authUser.Id),
                Email = authUser.Email,
                FullName = authUser.Email ?? "",
                Role = UserRole.Customer
            };
            return _cachedUser;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> IsAdminAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Role == UserRole.Admin;
    }

    public async Task<UserProfile?> UpdateProfileAsync(UserProfile profile)
    {
        if (_accessToken is null)
        {
            LastError = "No access token — user is not logged in.";
            return null;
        }

        try
        {
            // default_address is stored as JSONB in Supabase.
            // If the value is a plain string (not valid JSON), wrap it as a JSON string.
            object? addressValue = null;
            if (profile.DefaultAddress is not null)
            {
                var trimmed = profile.DefaultAddress.Trim();
                if (trimmed.Length > 0)
                {
                    try
                    {
                        // Try parsing as JSON (object or string)
                        addressValue = JsonSerializer.Deserialize<object>(trimmed);
                    }
                    catch
                    {
                        // Plain text — send as a JSON string value
                        addressValue = trimmed;
                    }
                }
            }

            var url = $"{_restUrl}/user_profiles?id=eq.{profile.Id}";
            var request = CreateRestRequest(HttpMethod.Patch, url);
            request.Content = JsonContent.Create(new
            {
                full_name = profile.FullName?.Trim(),
                phone = profile.Phone?.Trim(),
                default_address = addressValue,
                avatar_key = profile.AvatarKey
            }, options: JsonOpts);
            request.Headers.TryAddWithoutValidation("Prefer", "return=minimal");

            var response = await _http.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                _cachedUser = profile;
                OnAuthStateChanged?.Invoke();
                return profile;
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            LastError = $"UpdateProfile HTTP {(int)response.StatusCode}: {errorBody}";
            return null;
        }
        catch (Exception ex)
        {
            LastError = $"UpdateProfile exception: {ex.Message}";
            return null;
        }
    }

    // ── Helpers ──

    private HttpRequestMessage CreateAuthRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.TryAddWithoutValidation("apikey", _anonKey);
        return req;
    }

    private HttpRequestMessage CreateRestRequest(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.TryAddWithoutValidation("apikey", _anonKey);
        if (_accessToken is not null)
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        else
            req.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_anonKey}");
        return req;
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // ── Response DTOs ──

    private sealed class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int? ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }
        public AuthUser? User { get; set; }
    }

    private sealed class AuthUser
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
    }

    private sealed class UserProfileRow
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public JsonElement? DefaultAddress { get; set; }
        public string? AvatarKey { get; set; }
        public string Role { get; set; } = "customer";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
