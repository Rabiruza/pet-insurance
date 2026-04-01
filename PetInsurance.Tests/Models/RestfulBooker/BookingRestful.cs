using System;
using System.Text.Json.Serialization;

namespace PetInsurance.Tests.Models.RestfulBooker
{
    /// <summary>
    /// Represents a booking response from the API
    /// </summary>
    public class BookingRestful
    {
        [JsonPropertyName("firstname")]
        public string? Firstname { get; set; }
        [JsonPropertyName("lastname")]
        public string? Lastname { get; set; }
        [JsonPropertyName("totalprice")]
        public int? Totalprice { get; set; }
        [JsonPropertyName("depositpaid")]   
        public bool Depositpaid { get; set; }
        [JsonPropertyName("bookingdates")]
        public BookingDates? Bookingdates { get; set; }
        [JsonPropertyName("additionalneeds")]                           
        public string? Additionalneeds { get; set; }
    }

    /// <summary>
    /// Represents booking dates (check-in and check-out)
    /// </summary>
    public class BookingDates
    {
        [JsonPropertyName("checkin")]
        public string? Checkin { get; set; }
        [JsonPropertyName("checkout")]
        public string? Checkout { get; set; }
    }

    /// <summary>
    /// Represents the response when creating a booking
    /// </summary>
    public class BookingResponse
    {   
        [JsonPropertyName("bookingid")]
        public int Bookingid { get; set; }
        [JsonPropertyName("booking")]
        public BookingRestful? Booking { get; set; }
    }

    public class BookingSummary
    {
        [JsonPropertyName("bookingid")]
        public int Bookingid { get; set; }
    }

    /// <summary>
    /// Represents authentication token response
    /// </summary>
    public class Token
    {
        [JsonPropertyName("token")]
        public string? token { get; set; }
    }

    /// <summary>
    /// Represents credentials for authentication
    /// </summary>
    public class TokenRequest
    {
        [JsonPropertyName("username")]
        public string? Username { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }

    /// <summary>
    /// Represents a partial update for a booking
    /// </summary>
    public class PartialUpdate
    {
        [JsonPropertyName("firstname")]
        public string? Firstname { get; set; }
        [JsonPropertyName("lastname")]
        public string? Lastname { get; set; }
        [JsonPropertyName("totalprice")]
        public int? Totalprice { get; set; }
        [JsonPropertyName("depositpaid")]   
        public bool? Depositpaid { get; set; }
        [JsonPropertyName("bookingdates")]
        public BookingDates? Bookingdates { get; set; }
        [JsonPropertyName("additionalneeds")]                           
        public string? Additionalneeds { get; set; }
    }
}
