
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Amqp;
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

    public async Task<Pet> CreatePetAsync(Pet pet)
    {
        var request = new RestRequest("pet", Method.Post);
        request.AddJsonBody(pet);

        var response = await _client.ExecuteAsync<Pet>(request);

        
        if (!response.IsSuccessful)
        {
           throw new Exception($"Failed to create pet: {response.StatusCode} - {response.Content}");
        }

        return response.Data;
    }

    public async Task<Pet> GetPetAsync(int petId)
    {
        var request = new RestRequest($"pets/{petId}", Method.Get);
        var response = await _client.ExecuteAsync(request);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
            
        if (!response.IsSuccessful)
            throw new Exception($"Failed to get pet: {response.StatusCode} - {response.Content}");
        
        return response.Content != null 
            ? JsonSerializer.Deserialize<Pet>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
            : throw new Exception("Response content is null");
    }

    public async Task<Pet> UpdatePetAsync(Pet pet)
    {
        var request = new RestRequest($"pet/{pet.Id}", Method.Put);
        request.AddJsonBody(pet);
        var response = await _client.ExecuteAsync<Pet>(request);
        if (!response.IsSuccessful)
            throw new Exception($"Failed to update pet: {response.StatusCode} - {response.Content}");
        
        return response.Data;
    }

    public async Task<Pet> DeletePetAsync(int petId)
    {
        var request = new RestRequest($"pet/{petId}", Method.Delete);
        request.AddHeader("api_key", "special-key"); // 🔍 Якщо потрібен ключ для видалення
        var response = await _client.ExecuteAsync(request);
        if (!response.IsSuccessful)            
            throw new Exception($"Failed to delete pet: {response.StatusCode} - {response.Content}");
        return JsonSerializer.Deserialize<Pet>(response.Content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<List<Pet>> GetPetByStatusAsync(string status)
    {
        // var request = new RestRequest($"pet/findByStatus", Method.Get);
        // request.AddQueryParameter("status", status);
        // //if (!string.IsNullOrEmpty(category))
        // //    request.AddQueryParameter("category", category);    

        // var response = await _client.ExecuteAsync<List<Pet>>(request);
        // if (!response.IsSuccessful)            
        //     throw new Exception($"Failed to search pets: {response.StatusCode} - {response.Content}");       
        // return response.Data ?? new List<Pet>();

        // 🔑 Слеш на початку + AddQueryParameter
        var request = new RestRequest("/pet/findByStatus", Method.Get);
        request.AddQueryParameter("status", status);

        request.AddHeader("Accept", "application/json");
        request.AddHeader("User-Agent", "Mozilla/5.0");

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Failed to search pets: {response.StatusCode} - {response.Content}");
        }

        var pets = JsonSerializer.Deserialize<List<Pet>>(response.Content!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return pets ?? new List<Pet>();
    }
}