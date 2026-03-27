using RestSharp;

using PetInsurance.Tests.Core;

using PetInsurance.Tests.Models;
using FluentAssertions;

using System.Text.Json;

namespace PetInsurance.Tests.Tests.API
{
    [TestFixture]
    public class BookingApiTests : BaseApiTest
    {
        [Test]
        public async Task GetAllBookings_Success()
        {
            // Arrange
            var request = new RestRequest("/booking", Method.Get);

            // Act
            var response = await _client.ExecuteAsync(request);
            TestContext.Out.WriteLine($"Status: {response.StatusCode}, Content: {response.Content}");

            // Assert - базові перевірки
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.ContentType.Should().Contain("application/json");

            // Спробуємо десеріалізувати
            var bookings = JsonSerializer.Deserialize<List<BookingSummary>>(response.Content!, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // 🔍 Важливо: ігноруємо регістр імен полів
            });

            bookings.Should().NotBeNull(); // Список має існувати (навіть якщо порожній)

            // 🔍 Якщо є дані - перевіримо структуру першого елементу
            if (bookings!.Any())
            {
                var first = bookings.First();
                first.FirstName.Should().NotBeNull(); // Поле має існувати
            }
        }

        [Test]
        public async Task CreateBooking_WithBuilder_Success()
        {
            // Arrange
            var newBooking = new BookingBuilder()
                .WithFirstName("Anna")
                .WithLastName("Shevchenko")
                .WithTotalPrice(250)
                .WithDepositPaid(true)
                .WithDates("2024-06-01", "2024-06-05")
                .WithAdditionalNeeds("Breakfast")
                .Build();

            // 🔍 Логуємо, що відправляємо
            var jsonBody = JsonSerializer.Serialize(newBooking);
            TestContext.Out.WriteLine($"Request body: {jsonBody}");

            // Отримуємо токен
            var tokenRequest = new RestRequest("/auth", Method.Post);
            tokenRequest.AddJsonBody(new { username = "admin", password = "password123" });
            var tokenResponse = await _client.ExecuteAsync(tokenRequest);

            // 🔍 Перевіряємо, чи отримали токен
            if (tokenResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                TestContext.Out.WriteLine($"Auth failed: {tokenResponse.Content}");
                Assert.Fail("Failed to get auth token");
            }

            var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponse.Content!)?.Token;
            TestContext.Out.WriteLine($"Token received: {!string.IsNullOrEmpty(token)}");

            // Створюємо бронювання
            var createRequest = new RestRequest("/booking", Method.Post);
            createRequest.AddHeader("Content-Type", "application/json");
            createRequest.AddHeader("Accept", "application/json"); // 🔍 Додаємо Accept
            createRequest.AddJsonBody(newBooking);

            var createResponse = await _client.ExecuteAsync(createRequest);

            // 🔍 Логуємо відповідь для дебагу
            TestContext.Out.WriteLine($"Response status: {createResponse.StatusCode}");
            TestContext.Out.WriteLine($"Response body: {createResponse.Content}");

            // Assert
            createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Test]
        public async Task CreateBooking_WithRandomData_Success()
        {
            // Arrange
            var randomBooking = new BookingBuilder()
                .WithRandomData()
                .Build();

            var tokenRequest = new RestRequest("/auth", Method.Post);
            tokenRequest.AddJsonBody(new
            {
                username = "admin",
                password = "password123"
            });
            var tokenResponse = await _client.ExecuteAsync(tokenRequest);
            var token = JsonSerializer.Deserialize<TokenResponse>(tokenResponse.Content!)?.Token;

            // Створюємо бронювання
            var createRequest = new RestRequest("/booking", Method.Post);
            createRequest.AddHeader("Content-Type", "application/json");
            createRequest.AddJsonBody(randomBooking);

            var createResponse = await _client.ExecuteAsync<Booking>(createRequest);
            createResponse.Data.Should().BeSameAs(randomBooking);
        }
    }
}