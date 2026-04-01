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
            var newBooking = new Booking
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = 100,
                depositpaid = true,
                bookingdates = new BookingDates
                {
                    checkin = "2024-01-01",
                    checkout = "2024-01-10"
                },
                additionalneeds = "Breakfast"
            };

            // Act - 执行 POST request
            var response = await _apiClient.CreateBookingAsync(newBooking);

            // Assert - 验证 response
            Assert.That(response, Is.Not.Null, "Response should not be null");
            Assert.That(response!.bookingid, Is.GreaterThan(0), "Booking ID should be positive");
            Assert.That(response.booking, Is.Not.Null, "Booking data should be returned");
            
            // Verify the returned data matches what we sent
            Assert.That(response.booking!.firstname, Is.EqualTo("John"), "Firstname should match");
            Assert.That(response.booking.lastname, Is.EqualTo("Doe"), "Lastname should match");
            Assert.That(response.booking.totalprice, Is.EqualTo(100), "Total price should match");
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
            var newBooking = new Booking
            {
                firstname = "Jane",
                lastname = "Smith",
                totalprice = 150,
                depositpaid = false,
                bookingdates = new BookingDates
                {
                    checkin = "2024-02-01",
                    checkout = "2024-02-15"
                },
                additionalneeds = "Lunch"
            };

            // Act
            var response = await _apiClient.CreateBookingAsync(newBooking);

            // Assert - FluentAssertions style
            response.Should().NotBeNull("creation should return a response");
            response!.bookingid.Should().BeGreaterThan(0, "booking ID should be positive");
            response.booking.Should().NotBeNull("booking data should be returned");
            
            response.booking!.Should().BeEquivalentTo(newBooking, 
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
            var minimalBooking = new Booking
            {
                firstname = "Minimal",
                lastname = "Test"
            };

            // Act
            var response = await _apiClient.CreateBookingAsync(minimalBooking);

            // Assert
            response.Should().NotBeNull();
            response!.bookingid.Should().BeGreaterThan(0);
            response.booking!.firstname.Should().Be("Minimal");
            response.booking.lastname.Should().Be("Test");
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
            var bookingWithSpecialChars = new Booking
            {
                firstname = "José",
                lastname = "García",
                totalprice = 200,
                depositpaid = true,
                additionalneeds = "Vegetarian meal 🥗"
            };

            // Act
            var response = await _apiClient.CreateBookingAsync(bookingWithSpecialChars);

            // Assert
            response.Should().NotBeNull();
            response!.booking!.firstname.Should().Be("José", "Unicode characters should be preserved");
            response.booking.additionalneeds.Should().Contain("🥗", "Emoji should be preserved");
        }
    }
}
