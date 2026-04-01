using System;
using System.Collections.Generic;
using System.Text;

namespace PetInsurance.Tests.ApiClients
{
    public interface IPetApiClient
    {
        Task<Pet?> GetPetByIdAsync(long id);
        Task<List<Pet>?> GetPetByStatusAsync(string status);
        Task<Pet> CreatePetAsync(Pet pet);
        Task<Pet> GetPetAsync(int petId);
        Task<Pet> UpdatePetAsync(Pet pet);
        Task<Pet> DeletePetAsync(int petId);
    }
}
