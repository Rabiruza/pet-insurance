using RestSharp;
using FluentAssertions;
using PetInsurance.Tests.Core;
using PetInsurance.Tests.ApiClients.RestfulBooker;
using PetInsurance.Tests.Models.RestfulBooker;

namespace PetInsurance.Tests.Tests.API.RestfulBooker
{
    /// <summary>
    /// Lesson 2: POST - Creating Resources
    /// 
    /// What you'll learn:
    /// - How to create resources with POST
    /// - How to send JSON body in requests
    /// - How to validate 201 Created response
    /// - How to extract data from response (booking ID)
    /// </summary>
    [TestFixture]
    [Category("API")]
    [Category("Lesson2")]
    public class Lesson2_CreateBookingTests : BaseApiTest
    {
        private RestfulBookerClient _apiClient = null!;

        [SetUp]
        public void SetUp()
        {
            _apiClient = new RestfulBookerClient();
        }

        [TearDown]
        public void TearDown()
        {
            _apiClient?.Dispose();
        }

        /// <summary>
        /// Test 1: Create a new booking with valid data
        /// 
        /// Learn: Basic POST request with JSON body
        /// Expected: Status 201, Booking ID returned, Data matches request
        /// </summary>
        [Test]
        [Order(1)]
        public async Task CreateBooking_WithValidData_ReturnsCreatedBooking()
        {
            // Arrange - 准备 test data
            var newBooking = new BookingRestful
            {
                Firstname = "John",
                Lastname = "Doe",
                Totalprice = 100,
                Depositpaid = true,
                Bookingdates = new BookingDates
                {
                    Checkin = "2024-01-01",
                    Checkout = "2024-01-10"
                },
                Additionalneeds = "Breakfast"
            };

            // Act - 执行 POST request
            var response = await _apiClient.CreateBookingAsync(newBooking);

            // Assert - 验证 response
            Assert.That(response, Is.Not.Null, "Response should not be null");
            Assert.That(response!.Bookingid, Is.GreaterThan(0), "Booking ID should be positive");
            Assert.That(response.Booking, Is.Not.Null, "Booking data should be returned");
            
            // Verify the returned data matches what we sent
            Assert.That(response.Booking!.Firstname, Is.EqualTo("John"), "Firstname should match");
            Assert.That(response.Booking.Lastname, Is.EqualTo("Doe"), "Lastname should match");
            Assert.That(response.Booking.Totalprice, Is.EqualTo(100), "Total price should match");
        }

        /// <summary>
        /// Test 2: Create booking - Validate with FluentAssertions
        /// 
        /// Learn: Using FluentAssertions for complex object validation
        /// </summary>
        [Test]
        [Order(2)]
        public async Task CreateBooking_ValidateResponse_WithFluentAssertions()
        {
            // Arrange
            var newBooking = new BookingRestful
            {
                Firstname = "Jane",
                Lastname = "Smith",
                Totalprice = 150,
                Depositpaid = false,
                Bookingdates = new BookingDates
                {
                    Checkin = "2024-02-01",
                    Checkout = "2024-02-15"
                },
                Additionalneeds = "Lunch"
            };

            // Act
            var response = await _apiClient.CreateBookingAsync(newBooking);

            // Assert - FluentAssertions style
            response.Should().NotBeNull("creation should return a response");
            response!.Bookingid.Should().BeGreaterThan(0, "booking ID should be positive");
            response.Booking.Should().NotBeNull("booking data should be returned");
            
            response.Booking!.Should().BeEquivalentTo(newBooking, 
                options => options.ExcludingMissingMembers(),
                "returned booking should match the request");
        }

        /// <summary>
        /// Test 3: Create booking with minimal data
        /// 
        /// Learn: What fields are required vs optional
        /// </summary>
        [Test]
        [Order(3)]
        public async Task CreateBooking_WithMinimalData_Success()
        {
            // Arrange - Only required fields
            var minimalBooking = new BookingRestful
            {
                Firstname = "Minimal",
                Lastname = "Test",
                Totalprice = 100,
                Depositpaid = true,
                Bookingdates = new BookingDates
                {
                    Checkin = "2026-04-01",
                    Checkout = "2026-04-02"
                }
            };

            // Act
            var response = await _apiClient.CreateBookingAsync(minimalBooking);

            // Assert
            response.Should().NotBeNull();
            response!.Bookingid.Should().BeGreaterThan(0);
            response.Booking!.Firstname.Should().Be("Minimal");
            response.Booking.Lastname.Should().Be("Test");
        }

        /// <summary>
        /// Test 4: Create booking with special characters
        /// 
        /// Learn: How API handles special characters and unicode
        /// </summary>
        [Test]
        [Order(4)]
        public async Task CreateBooking_WithSpecialCharacters_Success()
        {
            // Arrange
            var bookingWithSpecialChars = new BookingRestful
            {
                Firstname = "José",
                Lastname = "García",
                Totalprice = 200,
                Depositpaid = true,
                Bookingdates = new BookingDates
                {
                    Checkin = "2026-04-01",
                    Checkout = "2026-04-05"
                },
                Additionalneeds = "Vegetarian meal 🥗"
            };

            // Act
            var response = await _apiClient.CreateBookingAsync(bookingWithSpecialChars);

            // Assert
            response.Should().NotBeNull();
            response!.Booking!.Firstname.Should().Be("José", "Unicode characters should be preserved");
            response.Booking.Additionalneeds.Should().Contain("🥗", "Emoji should be preserved");
        }
    }
}
