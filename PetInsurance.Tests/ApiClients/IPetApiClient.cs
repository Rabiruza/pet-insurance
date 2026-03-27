using System;
using System.Collections.Generic;
using System.Text;

namespace PetInsurance.Tests.ApiClients
{
    public interface IPetApiClient
    {
        Task<Pet?> GetPetByIdAsync(long id);
        Task<List<Pet>?> GetPetsByStatusAsync(string status);
    }
}
