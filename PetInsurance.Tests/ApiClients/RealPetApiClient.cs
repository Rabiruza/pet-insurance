
using System.Text.Json;

using PetInsurance.Tests.ApiClients;

using RestSharp;

public class RealPetApiClient : IPetApiClient
{
    private readonly RestClient _client;

    public RealPetApiClient(string baseUrl)
    {
        _client = new RestClient(baseUrl);
    }

    public async Task<Pet?> GetPetByIdAsync(long id)
    {
        var request = new RestRequest($"/pet/{id}");
        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessStatusCode) return null;

        return JsonSerializer.Deserialize<Pet>(response.Content!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public Task<List<Pet>?> GetPetsByStatusAsync(string status)
    {
        throw new NotImplementedException();
    }

    // ... інші методи
}