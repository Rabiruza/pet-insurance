using PetInsurance.Tests.Models.RestfulBooker;

namespace PetInsurance.Tests.ApiClients.RestfulBooker
{
    /// <summary>
    /// Interface for RESTful Booker API client
    /// Used for booking management operations
    /// </summary>
    public interface IRestfulBookerClient
    {
        // Health Check
        Task<string> PingAsync();

        // Authentication
        Task<Token?> CreateTokenAsync(string username, string password);

        // Bookings - CRUD Operations
        Task<List<Booking>?> GetAllBookingsAsync();
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<BookingResponse?> CreateBookingAsync(Booking booking);
        Task<Booking?> UpdateBookingAsync(int bookingId, Booking booking, string token);
        Task<Booking?> PartialUpdateBookingAsync(int bookingId, PartialUpdate update, string token);
        Task<bool> DeleteBookingAsync(int bookingId, string token);
    }
}
