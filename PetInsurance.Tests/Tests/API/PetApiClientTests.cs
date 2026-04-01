using System.Text.Json;

using FluentAssertions;

using NUnit.Framework;

using PetInsurance.Tests.ApiClients;
using PetInsurance.Tests.Config;
using PetInsurance.Tests.Core;
using PetInsurance.Tests.Models;

using RestSharp;

namespace PetInsurance.Tests.Tests.API
{
    [TestFixture]
    public class PetApiClientTests : BaseApiTest
    {
        private IPetApiClient _apiClient;

        [SetUp]
        public void SetUp()
        {
            _apiClient = new RealPetApiClient(TestConfiguration.Instance.PetStoreUrl);
        }

        [Test]
        public async Task GetAnyPetById_Success()
        {
            // 🔍 Крок 1: Знайти будь-якого доступного пета
            var searchRequest = new RestRequest("/pet/findByStatus?status=available", Method.Get);
            var searchResponse = await _client.ExecuteAsync(searchRequest);

            var pets = JsonSerializer.Deserialize<List<Pet>>(searchResponse.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (pets == null || !pets.Any())
            {
                Assert.Inconclusive("No available pets found in the system");
            }

            var petId = pets.First().Id;
            TestContext.Out.WriteLine($"Testing with pet ID: {petId}");

            // 🔍 Крок 2: Отримати конкретного пета
            var request = new RestRequest($"/pet/{petId}", Method.Get);
            var response = await _client.ExecuteAsync(request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var pet = JsonSerializer.Deserialize<Pet>(response.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            pet.Should().NotBeNull();
            pet!.Id.Should().Be(petId);
        }

        [Test]
        public async Task GetAnyPetById_SuccessTest()
        {
            // Act 1: Отримуємо список через виправлений метод
            var pets = await _apiClient.GetPetByStatusAsync("available");

            if (pets == null || !pets.Any())
            {
                Assert.Inconclusive("No available pets found");
            }

            var petId = pets.First().Id;

            // Act 2: Отримуємо пета по ID
            var pet = await _apiClient.GetPetByIdAsync(petId);

            // Assert
            pet.Should().NotBeNull();
            pet!.Id.Should().Be(petId);
        }

        [Test]
        public async Task GetPetsByStatus_Success()
        {
            // Arrange
            var request = new RestRequest("/pet/findByStatus?status=available", Method.Get);

            // Act
            var response = await _client.ExecuteAsync(request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var pets = JsonSerializer.Deserialize<List<Pet>>(response.Content!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            pets.Should().NotBeNull();
            // Не вимагаємо, щоб список не був порожнім - стан API може змінюватися
        }

        [Test]
        public async Task CreateNewPet_AndUpdatePetId_Success()
        {
            // Arrange - створюємо пета
            var newPet = new Pet
            {
                Name = $"TestPet_{Guid.NewGuid().ToString().Substring(0, 8)}", // Унікальне ім'я
                Status = "available",
                Category = new Category { Id = 1, Name = "Dogs" } // 🔍 Деякі API вимагають category
            };

            var createRequest = new RestRequest("/pet", Method.Post);
            createRequest.AddJsonBody(newPet);

            // Act 1 - Створюємо
            var createResponse = await _client.ExecuteAsync(createRequest);
            TestContext.Out.WriteLine($"Create response: {createResponse.StatusCode} - {createResponse.Content}");

            // Assert 1
            createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var createdPet = JsonSerializer.Deserialize<Pet>(createResponse.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            createdPet.Should().NotBeNull();
            var petId = createdPet!.Id;
            TestContext.Out.WriteLine($"Created pet with ID: {petId}");

            // Act 2 - Оновлюємо
            var updatedPet = new Pet
            {
                Id = petId,  // ✅ Використовуємо ID з попередньої відповіді
                Name = $"Updated_{newPet.Name}",
                Status = "pending", // Змінюємо статус
                Category = newPet.Category
            };

            var updateRequest = new RestRequest("/pet", Method.Put);
            updateRequest.AddJsonBody(updatedPet);

            var updateResponse = await _client.ExecuteAsync(updateRequest);
            TestContext.Out.WriteLine($"Update response: {updateResponse.StatusCode} - {updateResponse.Content}");

            // Assert 2
            updateResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            // 🔍 Перевіряємо, що оновлення спрацювало - отримуємо пета знову
            var getRequest = new RestRequest($"/pet/{petId}");
            var getResponse = await _client.ExecuteAsync(getRequest);

            var retrievedPet = JsonSerializer.Deserialize<Pet>(getResponse.Content!,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            retrievedPet!.Name.Should().Be(updatedPet.Name);
            retrievedPet.Status.Should().Be("pending");
        }
    }
}