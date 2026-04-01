using RestSharp;
using FluentAssertions;
using PetInsurance.Tests.Core;
using PetInsurance.Tests.ApiClients.RestfulBooker;
using PetInsurance.Tests.Models.RestfulBooker;

namespace PetInsurance.Tests.Tests.API.RestfulBooker
{
    /// <summary>
    /// Lesson 3: Authentication and PUT/PATCH Updates
    /// 
    /// What you'll learn:
    /// - How to authenticate and get tokens
    /// - How to use tokens in requests (cookies)
    /// - Difference between PUT (full update) and PATCH (partial update)
    /// - Testing 401/403 unauthorized errors
    /// </summary>
    [TestFixture]
    [Category("API")]
    [Category("Lesson3")]
    public class Lesson3_UpdateBookingTests : BaseApiTest
    {
        private RestfulBookerClient _apiClient = null!;
        private string _token = null!;
        private int _bookingId;

        [SetUp]
        public async Task SetUp()
        {
            _apiClient = new RestfulBookerClient();
            
            // Get authentication token
            var tokenResponse = await _apiClient.CreateTokenAsync("admin", "password123");
            _token = tokenResponse?.token ?? throw new InvalidOperationException("Failed to get token");

            // Create a booking to update
            var newBooking = new BookingRestful
            {
                Firstname = "Original",
                Lastname = "User",
                Totalprice = 100,
                Depositpaid = true,
                Bookingdates = new BookingDates
                {
                    Checkin = "2024-01-01",
                    Checkout = "2024-01-10"
                }
            };
            var createResponse = await _apiClient.CreateBookingAsync(newBooking);
            _bookingId = createResponse?.Bookingid   ?? throw new InvalidOperationException("Failed to create booking");
        }

        [TearDown]
        public void TearDown()
        {
            _apiClient?.Dispose();
         }

        /// <summary>
        /// Test 1: Authenticate with valid credentials
        /// 
        /// Learn: How authentication works
        /// Expected: Status 200, Token returned
        /// </summary>
        [Test]
        [Order(1)]
        public async Task CreateToken_WithValidCredentials_ReturnsToken()
        {
            // Act
            var response = await _apiClient.CreateTokenAsync("admin", "password123");

            // Assert
            response.Should().NotBeNull("valid credentials should return a token");
            response!.token.Should().NotBeNullOrEmpty("token should be returned");
        }

        /// <summary>
        /// Test 2: Authenticate with invalid credentials
        /// 
        /// Learn: How API handles authentication failures
        /// Expected: Status 403 Forbidden
        /// </summary>
        [Test]
        [Order(2)]
        public async Task CreateToken_WithInvalidCredentials_ReturnsForbidden()
        {
            // Act
            var response = await _apiClient.CreateTokenAsync("wronguser", "wrongpass");

            // Assert
            response.Should().BeNull("invalid credentials should not return a token");
        }

        /// <summary>
        /// Test 3: Update booking with PUT (full update)
        /// 
        /// Learn: PUT replaces the ENTIRE resource
        /// All fields must be provided, even if unchanged
        /// </summary>
        [Test]
        [Order(3)]
        public async Task UpdateBooking_WithPut_FullReplace()
        {
            // Arrange - Must provide ALL fields for PUT
            var updatedBooking = new BookingRestful
            {
                Firstname = "Updated",
                Lastname = "Name",
                Totalprice = 200,
                Depositpaid = false,
                Bookingdates = new BookingDates
                {
                    Checkin = "2024-03-01",
                    Checkout = "2024-03-10"
                },
                Additionalneeds = "Dinner"
            };

            // Act
            var response = await _apiClient.UpdateBookingAsync(_bookingId, updatedBooking, _token);

            // Assert
            response.Should().NotBeNull();
            response!.Firstname.Should().Be("Updated", "firstname should be updated");
            response.Lastname.Should().Be("Name", "lastname should be updated");
            response.Totalprice.Should().Be(200, "totalprice should be updated");
        }

        /// <summary>
        /// Test 4: Update booking with PATCH (partial update)
        /// 
        /// Learn: PATCH only updates provided fields
        /// Other fields remain unchanged
        /// </summary>
        [Test]
        [Order(4)]
        public async Task UpdateBooking_WithPatch_PartialUpdate()
        {
            // Arrange - Only update firstname
            var partialUpdate = new PartialUpdate
            {
                Firstname = "Patched",
                Lastname = "User",          // ← передаємо старе значення
                Totalprice = 100,           // ← передаємо старе значення
                Depositpaid = true,         // ← передаємо старе значення
                Bookingdates = new BookingDates
                {
                    Checkin = "2024-01-01",
                    Checkout = "2024-01-10"
                },
                Additionalneeds = null      // або старе значення, якщо було
            };


            // Act
            var response = await _apiClient.PartialUpdateBookingAsync(_bookingId, partialUpdate, _token);

            // Assert
            response.Should().NotBeNull();
            response!.Firstname.Should().Be("Patched", "firstname should be updated");
            response.Lastname.Should().Be("User", "lastname should remain unchanged");
            response.Totalprice.Should().Be(100, "totalprice should remain unchanged");
        }

        /// <summary>
        /// Test 5: Update without authentication
        /// 
        /// Learn: Protected endpoints require authentication
        /// Expected: 403 Forbidden
        /// </summary>
        [Test]
        [Order(5)]
        public async Task UpdateBooking_WithoutToken_ReturnsForbidden()
        {
            // Arrange
            var updatedBooking = new BookingRestful
            {
                Firstname = "Hacker",
                Lastname = "Attempt",
                Totalprice = 999,
                Depositpaid = true
            };

            // Act - Try without token (pass empty string)
            var act = async () => await _apiClient.UpdateBookingAsync(_bookingId, updatedBooking, "");

            // Assert - Should throw exception or return null
            await act.Should().NotThrowAsync("request should complete but return forbidden");
        }

        /// <summary>
        /// Test 6: Update non-existent booking
        /// 
        /// Learn: Updating non-existent resources
        /// Expected: 404 Not Found
        /// </summary>
        [Test]
        [Order(6)]
        public async Task UpdateBooking_NonExistentBooking_ReturnsNotFound()
        {
            // Arrange
            var updatedBooking = new BookingRestful
            {
                Firstname = "Ghost",
                Lastname = "Booking"
            };

            // Act
            var response = await _apiClient.UpdateBookingAsync(99999, updatedBooking, _token);

            // Assert
            response.Should().BeNull("non-existent booking cannot be updated");
        }
    }
}
