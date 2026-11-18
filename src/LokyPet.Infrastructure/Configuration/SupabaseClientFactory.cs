namespace LokyPet.Infrastructure.Configuration;
using Supabase;

public sealed class SupabaseClientFactory
{
    private readonly SupabaseConfig _config;
    private Client? _client;

    public SupabaseClientFactory(SupabaseConfig config)
    {
        _config = config;
    }

    public async Task<Client> GetClientAsync()
    {
        if (_client is not null) return _client;

        var options = new SupabaseOptions { AutoRefreshToken = true };
        _client = new Client(_config.Url, _config.AnonKey, options);
        await _client.InitializeAsync();
        return _client;
    }
}
