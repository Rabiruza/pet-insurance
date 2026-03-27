using FluentAssertions;

using PetInsurance.Tests.ApiClients;
using PetInsurance.Tests.Config;

[TestFixture]
public class PetServiceTests
{
    private IPetApiClient _apiClient;

    [SetUp]
    public void SetUp()
    {
        // 🔍 Перемикач: реальний API або мок
        var useMock = true; // Зміни на false для реальних тестів

        _apiClient = useMock
            ? new MockPetApiClient()
            : new RealPetApiClient(TestConfiguration.Instance.ApiUrl);
    }

    [Test]
    public async Task GetPet_Exists_ReturnsPet()
    {
        // Arrange & Act
        var pet = await _apiClient.GetPetByIdAsync(1);

        // Assert
        pet.Should().NotBeNull();
        pet!.Name.Should().Be("Buddy"); // Працює з моком!
    }
}