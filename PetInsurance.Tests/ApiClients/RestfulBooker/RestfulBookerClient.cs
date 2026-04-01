using RestSharp;
using PetInsurance.Tests.Models.RestfulBooker;
using PetInsurance.Tests.Config;

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

        /// <summary>
        /// Health check endpoint - verifies API is running
        /// GET /ping
        /// </summary>
        public async Task<string> PingAsync()
        {
            var request = new RestRequest("/ping", Method.Get);
            var response = await _client.ExecuteAsync(request);
            return response.Content ?? string.Empty;
        }

        /// <summary>
        /// Create authentication token
        /// POST /auth
        /// </summary>
        public async Task<Token?> CreateTokenAsync(string username, string password)
        {
            var request = new RestRequest("/auth", Method.Post);
            request.AddJsonBody(new { username, password });

            var response = await _client.ExecuteAsync<Token>(request);
            return response.Data;
        }

        /// <summary>
        /// Get all bookings (summary view)
        /// GET /booking
        /// </summary>
        public async Task<List<Booking>?> GetAllBookingsAsync()
        {
            var request = new RestRequest("/booking", Method.Get);
            var response = await _client.ExecuteAsync<List<Booking>>(request);
            return response.Data;
        }

        /// <summary>
        /// Get a single booking by ID
        /// GET /booking/{id}
        /// </summary>
        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Get);
            var response = await _client.ExecuteAsync<Booking>(request);
            return response.Data;
        }

        /// <summary>
        /// Create a new booking
        /// POST /booking
        /// </summary>
        public async Task<BookingResponse?> CreateBookingAsync(Booking booking)
        {
            var request = new RestRequest("/booking", Method.Post);
            request.AddJsonBody(booking);

            var response = await _client.ExecuteAsync<BookingResponse>(request);
            return response.Data;
        }

        /// <summary>
        /// Update an existing booking (full update)
        /// PUT /booking/{id}
        /// Requires authentication token
        /// </summary>
        public async Task<Booking?> UpdateBookingAsync(int bookingId, Booking booking, string token)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Put);
            request.AddHeader("Cookie", $"token={token}");
            request.AddJsonBody(booking);

            var response = await _client.ExecuteAsync<Booking>(request);
            return response.Data;
        }

        /// <summary>
        /// Partially update a booking
        /// PATCH /booking/{id}
        /// Requires authentication token
        /// </summary>
        public async Task<Booking?> PartialUpdateBookingAsync(int bookingId, PartialUpdate update, string token)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Patch);
            request.AddHeader("Cookie", $"token={token}");
            request.AddJsonBody(update);

            var response = await _client.ExecuteAsync<Booking>(request);
            return response.Data;
        }

        /// <summary>
        /// Delete a booking
        /// DELETE /booking/{id}
        /// Requires authentication token
        /// </summary>
        public async Task<bool> DeleteBookingAsync(int bookingId, string token)
        {
            var request = new RestRequest($"/booking/{bookingId}", Method.Delete);
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
