namespace LokyPet.Client.Services;

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using LokyPet.Application.Interfaces;
using LokyPet.Domain.Entities;
using LokyPet.Infrastructure.Configuration;

/// <summary>
/// Implementación de IContactService usando PostgREST de Supabase.
/// Solo el método SendMessageAsync es público para clientes (RLS permite insert anónimo).
/// GetAllMessagesAsync y UpdateMessageStatusAsync son para admin (Fase 7).
/// </summary>
public sealed class ContactService : IContactService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    public ContactService(SupabaseConfig config)
    {
        _http = new HttpClient();
        _baseUrl = $"{config.Url}/rest/v1";
        _http.DefaultRequestHeaders.TryAddWithoutValidation("apikey", config.AnonKey);
        _http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {config.AnonKey}");
    }

    public async Task<bool> SendMessageAsync(ContactMessage message)
    {
        try
        {
            var payload = new
            {
                name = message.Name.Trim(),
                email = message.Email.Trim(),
                phone = string.IsNullOrWhiteSpace(message.Phone) ? null : message.Phone.Trim(),
                subject = message.Subject,
                message = message.Message.Trim(),
                status = "nuevo"
            };

            var url = $"{_baseUrl}/contact_messages";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload, options: JsonOpts)
            };
            request.Headers.TryAddWithoutValidation("Prefer", "return=minimal");

            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<ContactMessage>> GetAllMessagesAsync()
    {
        // Requiere rol admin (RLS). Se implementa completo en Fase 7.
        var url = $"{_baseUrl}/contact_messages?order=created_at.desc";
        var rows = await _http.GetFromJsonAsync<List<ContactMessageRow>>(url, JsonOpts);
        return rows?.Select(r => new ContactMessage
        {
            Id = r.Id,
            Name = r.Name,
            Email = r.Email,
            Phone = r.Phone,
            Subject = r.Subject,
            Message = r.Message,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList() ?? new();
    }

    public async Task UpdateMessageStatusAsync(Guid id, string status)
    {
        // Requiere rol admin (RLS). Se implementa completo en Fase 7.
        var url = $"{_baseUrl}/contact_messages?id=eq.{id}";
        var payload = new { status };
        var request = new HttpRequestMessage(HttpMethod.Patch, url)
        {
            Content = JsonContent.Create(payload, options: JsonOpts)
        };
        request.Headers.TryAddWithoutValidation("Prefer", "return=minimal");
        await _http.SendAsync(request);
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private sealed class ContactMessageRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
        public string Status { get; set; } = "nuevo";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
