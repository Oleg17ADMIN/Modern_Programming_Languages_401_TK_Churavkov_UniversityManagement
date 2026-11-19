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

    public async Task<List<ExternalUser>> GetUsersAsync(IEnumerable<int> ids, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var tasks = ids.Select(id => GetUserAsync(id, token)).ToList();
        ExternalUser?[] users = await Task.WhenAll(tasks);

        return users.OfType<ExternalUser>().ToList();
    }

    public async Task<List<ExternalUser>> GetUsersSequentiallyAsync(IEnumerable<int> ids, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(ids);

        var users = new List<ExternalUser>();
        foreach (var id in ids)
        {
            var user = await GetUserAsync(id, token);
            if (user is not null) users.Add(user);
        }

        return users;
    }
}
