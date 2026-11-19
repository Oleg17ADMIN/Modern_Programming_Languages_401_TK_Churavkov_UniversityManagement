using System.Net.Http.Json;

namespace UniversityManagement.Domain.External;

public class ExternalUserClient
{
    private readonly HttpClient _http;

    public ExternalUserClient(HttpClient http)
    {
        ArgumentNullException.ThrowIfNull(http);
        _http = http;
    }

    public async Task<ExternalUser?> GetUserAsync(int id, CancellationToken token)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        return await _http.GetFromJsonAsync<ExternalUser>($"users/{id}", token);
    }
}
