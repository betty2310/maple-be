namespace Maple.Infrastructure.DbContext;

public class SupabaseClient
{
    private readonly Supabase.Client _client;

    public SupabaseClient(string supabaseUrl, string supabaseKey)
    {
        var options = new Supabase.SupabaseOptions
        {
            AutoConnectRealtime = true
        };

        _client = new Supabase.Client(supabaseUrl, supabaseKey, options);
    }

    public async Task InitializeAsync()
    {
        await _client.InitializeAsync();
    }

    public Supabase.Client GetClient()
    {
        return _client;
    }
}