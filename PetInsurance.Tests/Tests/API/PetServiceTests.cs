using FluentAssertions;

using PetInsurance.Tests.ApiClients;
using PetInsurance.Tests.Config;

using RestSharp;

[TestFixture]
public class PetServiceTests
{
    private IPetApiClient _apiClient;


    [SetUp]
    public void SetUp()
    {
        // 🔍 Перемикач: реальний API або мок
        var useMock = false; // Зміни на false для реальних тестів

        _apiClient = useMock
            ? new MockPetApiClient()
            : new RealPetApiClient(TestConfiguration.Instance.ApiUrl);
    }

    //[Test]
    //public async Task ApiEndpoints_AreAvailable()
    //{
    //    // Перевіряємо, що базовий URL працює
    //    var healthRequest = new RestRequest("/pet/findByStatus?status=available", Method.Get);
    //    var healthResponse = await _client.ExecuteAsync(healthRequest);
        
    //    healthResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
    //    // Логуємо базовий URL для довідки
    //    TestContext.WriteLine($"API Base URL: {_client.Options.BaseUrl}");
    //}

    [Test]
    public async Task GetPet_ExistingId_ReturnsPet()
    {
        var response = await _apiClient.GetPetByIdAsync(1);  // ID=1 зазвичай існує в демо
        response.Should().NotBeNull();
    }

    [Test]
    public async Task GetPet_Exists_ReturnsPet()
    {
        // Arrange & Act
        var pet = await _apiClient.GetPetByIdAsync(1);

        // Assert
        pet.Should().NotBeNull();  
        pet!.Name.Should().Be("doggie"); // Працює з реальним API!
        //pet!.Name.Should().Be("Buddy"); // Працює з моком!
    }

    [Test]
    public async Task GetPet_NotExists_ReturnsNull()
    {
        // Arrange & Act
        var pet = await _apiClient.GetPetByIdAsync(999);
        // Assert
        pet.Should().BeNull();
    }

    [Test]
    public async Task CreatePet_ValidPet_ReturnsCreatedPet()
    {
        // Arrange
        var newPet = new Pet
        {
            Name = $"Charlie_{Guid.NewGuid().ToString().Substring(0, 8)}", // Унікальне ім'я
            Status = "available",
            Category = new Category { Id = 1, Name = "Dogs" }  // 🔍 Деякі версії API вимагають category
        };

        // Act
        var createdPet = await _apiClient.CreatePetAsync(newPet);

        // Assert
        createdPet.Should().NotBeNull();
        createdPet.Id.Should().BeGreaterThan(0);
        createdPet.Name.Should().Be(newPet.Name);
        createdPet.Status.Should().Be(newPet.Status);

        //await _apiClient.DeletePetAsync(newPet);
    }
}