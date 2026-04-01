using RestSharp;

using PetInsurance.Tests.Models.RestfulBooker;
using PetInsurance.Tests.Config;

using System.Text.Json;
using System.Net;

namespace PetInsurance.Tests.ApiClients.RestfulBooker
{
    /// <summary>
    /// RESTful Booker API Client implementation using RestSharp
    /// Base URL: https://restful-booker.herokuapp.com
    /// </summary>
    public class RestfulBookerClient : IRestfulBookerClient
    {
        private readonly RestClient _client;
        private const string BaseUrl = "https://restful-booker.herokuapp.com";

        public RestfulBookerClient()
        {
            var options = new RestClientOptions(BaseUrl)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _client = new RestClient(options);
        }

        public RestfulBookerClient(RestClient client)
        {
            _client = client;
        }

        /// <summary>Health check endpoint - verifies API is running GET /ping </summary>
        public async Task<string> PingAsync()
        {
            var request = new RestRequest("/ping", Method.Get);
            var response = await _client.ExecuteAsync(request);
            return response.Content ?? string.Empty;
        }

        /// <summary> Create authentication token POST /auth </summary>
        public async Task<Token?> CreateTokenAsync(string username, string password)
        {
            var request = new RestRequest("/auth", Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(new { username, password });

            var response = await _client.ExecuteAsync<Token>(request);
            if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(response.Data?.token))
            {
                return null;
            }
            return response.Data;
        }

        /// <summary>
        /// Get all bookings (summary view)
        /// GET /booking
        /// </summary>
        public async Task<List<BookingSummary>?> GetAllBookingsAsync()
        {
            var request = new RestRequest("/booking", Method.Get);
            var response = await _client.ExecuteAsync<List<BookingSummary>>(request);
            return response.Data;
        }

        /// <summary> Get a single booking by ID GET /booking/{id} </summary>
        public async Task<BookingRestful?> GetBookingByIdAsync(int bookingId)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Get);
            request.AddHeader("Accept", "application/json"); // важливо!

            var response = await _client.ExecuteAsync<BookingRestful>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Error: {response.StatusCode}, Content: {response.Content}");
                return null;
            }

            return response.Data;
        }

        /// <summary>
        /// Create a new booking
        /// POST /booking
        /// </summary>
        public async Task<BookingResponse?> CreateBookingAsync(BookingRestful booking)
        {
            var request = new RestRequest("/booking", Method.Post);
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(booking);

            var response = await _client.ExecuteAsync<BookingResponse>(request);
            return response.Data;
        }

        /// <summary>
        /// Update an existing booking (full update)
        /// PUT /booking/{id}
        /// Requires authentication token
        /// </summary>
        public async Task<BookingRestful?> UpdateBookingAsync(int bookingId, BookingRestful booking, string token)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Put);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Cookie", $"token={token}");
            request.AddJsonBody(booking);

            var response = await _client.ExecuteAsync<BookingRestful>(request);
            return response.Data;
        }

        /// <summary>
        /// Partially update a booking
        /// PATCH /booking/{id}
        /// Requires authentication token
        /// </summary>
        public async Task<BookingRestful?> PartialUpdateBookingAsync(int bookingId, PartialUpdate update, string token)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Patch);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Cookie", $"token={token}");
            request.AddJsonBody(update);

            var response = await _client.ExecuteAsync(request);

            // Логування статусу та контенту
            Console.WriteLine($"PATCH Status: {response.StatusCode}");
            Console.WriteLine($"PATCH Content: {response.Content}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Якщо не 200 OK — повертаємо null
                return null;
            }

            try
            {
                // Ручна десеріалізація
                var booking = JsonSerializer.Deserialize<BookingRestful>(
                    response.Content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                return booking;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
              return null;
            }
        }


        /// <summary>
        /// Delete a booking
        /// DELETE /booking/{id}
        /// Requires authentication token
        /// </summary>
        public async Task<bool> DeleteBookingAsync(int bookingId, string token)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Delete);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Cookie", $"token={token}");

            var response = await _client.ExecuteAsync(request);
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
