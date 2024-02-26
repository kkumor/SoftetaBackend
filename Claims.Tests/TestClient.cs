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

    public async Task<IEnumerable<Cover>?> GetCovers()
    {
        var response = await httpClient.GetAsync("/Covers");
        response.EnsureSuccessStatusCode();
        var responseObject = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Cover[]>(responseObject);
    }

    public async Task<Cover?> GetCover(string id)
    {
        var response = await httpClient.GetAsync($"/Covers/{id}");
        response.EnsureSuccessStatusCode();
        var responseObject = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Cover>(responseObject);
    }

    public async Task<Cover?> CreateCover(AddCoverDto dto)
    {
        var response = await httpClient.PostAsJsonAsync("/Covers", dto);
        response.EnsureSuccessStatusCode();
        var responseObject = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Cover>(responseObject);
    }

    public async Task DeleteCover(string id)
    {
        var response = await httpClient.DeleteAsync($"/Covers/{id}");
        response.EnsureSuccessStatusCode();
    }
}