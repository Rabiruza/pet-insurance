
using PetInsurance.Tests.ApiClients;

public class MockPetApiClient : IPetApiClient
{
    private readonly List<Pet> _mockPets = new()
    {
        new Pet { Id = 1, Name = "Buddy", Status = "available" },
        new Pet { Id = 2, Name = "Luna", Status = "pending" }
    };

    public Task<Pet?> GetPetByIdAsync(long id)
    {
        return Task.FromResult(_mockPets.FirstOrDefault(p => p.Id == id));
    }

    public Task<List<Pet>?> GetPetsByStatusAsync(string status)
    {
        var result = _mockPets.Where(p => p.Status == status).ToList();
        return Task.FromResult<List<Pet>?>(result); //  Явно вказуємо тип
    }
}