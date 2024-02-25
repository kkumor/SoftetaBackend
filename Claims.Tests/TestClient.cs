using System.Net.Http.Json;
using Claims.Controllers.Model;
using Claims.Model;
using Newtonsoft.Json;

namespace Claims.Tests;

public class TestClient(HttpClient httpClient)
{
    public async Task<IEnumerable<Claim>?> GetClaims()
    {
        var response = await httpClient.GetAsync("/Claims");
        response.EnsureSuccessStatusCode();
        var responseObject = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Claim[]>(responseObject);
    }

    public async Task<Claim?> GetClaim(string id)
    {
        var response = await httpClient.GetAsync($"/Claims/{id}");
        response.EnsureSuccessStatusCode();
        var responseObject = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Claim>(responseObject);
    }

    public async Task<Claim?> CreateClaim(AddClaimDto dto)
    {
        var response = await httpClient.PostAsJsonAsync("/Claims", dto);
        response.EnsureSuccessStatusCode();
        var responseObject = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Claim>(responseObject);
    }

    public async Task DeleteClaim(string id)
    {
        var response = await httpClient.DeleteAsync($"/Claims/{id}");
        response.EnsureSuccessStatusCode();
    }
}