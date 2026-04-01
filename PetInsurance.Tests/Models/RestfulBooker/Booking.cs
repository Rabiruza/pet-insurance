using System;

namespace PetInsurance.Tests.Models.RestfulBooker
{
    /// <summary>
    /// Represents a booking response from the API
    /// </summary>
    public class Booking
    {
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int totalprice { get; set; }
        public bool depositpaid { get; set; }
        public BookingDates? bookingdates { get; set; }
        public string? additionalneeds { get; set; }
    }

    /// <summary>
    /// Represents booking dates (check-in and check-out)
    /// </summary>
    public class BookingDates
    {
        public string? checkin { get; set; }
        public string? checkout { get; set; }
    }

    /// <summary>
    /// Represents the response when creating a booking
    /// </summary>
    public class BookingResponse
    {
        public int bookingid { get; set; }
        public Booking? booking { get; set; }
    }

    /// <summary>
    /// Represents authentication token response
    /// </summary>
    public class Token
    {
        public string? token { get; set; }
    }

    /// <summary>
    /// Represents credentials for authentication
    /// </summary>
    public class TokenRequest
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    /// <summary>
    /// Represents a partial update for a booking
    /// </summary>
    public class PartialUpdate
    {
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int? totalprice { get; set; }
        public bool? depositpaid { get; set; }
        public BookingDates? bookingdates { get; set; }
        public string? additionalneeds { get; set; }
    }
}
