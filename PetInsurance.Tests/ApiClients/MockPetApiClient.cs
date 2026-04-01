
using PetInsurance.Tests.ApiClients;

public class MockPetApiClient : IPetApiClient
{
    private readonly List<Pet> _mockPets = new()
    {
        new Pet { Id = 1, Name = "Buddy", Status = "available" },
        new Pet { Id = 2, Name = "Luna", Status = "pending" },
        new Pet { Id = 3, Name = "Ella", Status = "pending" }
    };

    public Task<Pet> CreatePetAsync(Pet pet)
    {
        pet.Id = _mockPets.Max(p => p.Id) + 1; // Автоінкремент ID
        _mockPets.Add(pet);
        return Task.FromResult(pet);
    }

    public Task<Pet> DeletePetAsync(int petId)
    {
        var pet = _mockPets.FirstOrDefault(p => p.Id == petId);
        if (pet != null)
        {
            _mockPets.Remove(pet);
            return Task.FromResult(pet);
        }
        throw new Exception($"Pet with ID {petId} not found");
    }

    public Task<Pet> GetPetAsync(int petId)
    {
        var pet = _mockPets.FirstOrDefault(p => p.Id == petId);
        if (pet != null)
            return Task.FromResult(pet);
        throw new Exception($"Pet with ID {petId} not found");
    }

    public Task<Pet?> GetPetByIdAsync(long id)
    {
        var pet = _mockPets.FirstOrDefault(p => p.Id == id);
        return Task.FromResult<Pet?>(pet);
    }

    public Task<List<Pet>?> GetPetsByStatusAsync(string status)
    {
        var result = _mockPets.Where(p => p.Status == status).ToList();
        return Task.FromResult<List<Pet>?>(result); //  Явно вказуємо тип
    }

    public Task<List<Pet>?> GetPetByStatusAsync(string status)
    {
        var result = _mockPets.Where(p => p.Status == status).ToList();
        return Task.FromResult<List<Pet>?>(result); //  Явно вказуємо тип
    }

    public Task<Pet> UpdatePetAsync(Pet pet)
    {
        var existingPet = _mockPets.FirstOrDefault(p => p.Id == pet.Id);
        if (existingPet != null)
        {
            existingPet.Name = pet.Name;
            existingPet.Status = pet.Status;
            return Task.FromResult(existingPet);
        }
        throw new Exception($"Pet with ID {pet.Id} not found");
    }
}