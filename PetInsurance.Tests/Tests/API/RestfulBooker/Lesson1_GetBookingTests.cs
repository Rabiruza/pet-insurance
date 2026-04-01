using RestSharp;
using FluentAssertions;
using PetInsurance.Tests.Core;
using PetInsurance.Tests.ApiClients.RestfulBooker;
using PetInsurance.Tests.Models.RestfulBooker;

namespace PetInsurance.Tests.Tests.API.RestfulBooker
{
    /// <summary>
    /// Lesson 1: Basic GET requests and HTTP fundamentals
    /// 
    /// What you'll learn:
    /// - How to make GET requests
    /// - How to check HTTP status codes
    /// - How to validate response body
    /// - Understanding 200 OK and 404 Not Found
    /// </summary>
    [TestFixture]
    [Category("API")]
    [Category("Lesson1")]
    public class Lesson1_GetBookingTests : BaseApiTest
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
        /// Test 1: Health Check
        /// 
        /// The simplest API test - verify the API is running
        /// Expected: Status 200, Response contains "Created"
        /// </summary>
        [Test]
        [Order(1)]
        public async Task PingApi_WhenCalled_ReturnsCreated()
        {
            // Arrange -准备 (no setup needed for ping)

            // Act - 执行
            var response = await _apiClient.PingAsync();

            // Assert - 验证
            Assert.That(response, Is.EqualTo("Created"), "API should return 'Created' status");
        }

        /// <summary>
        /// Test 2: Get existing booking by ID
        /// 
        /// Learn: How to fetch a resource and validate its properties
        /// Expected: Status 200, Booking has firstname and lastname
        /// </summary>
        [Test]
        [Order(2)]
        public async Task GetBookingById_WhenBookingExists_ReturnsBooking()
        {
            // Arrange - 准备
            int bookingId = 1; // Known existing booking

            // Act - 执行
            var booking = await _apiClient.GetBookingByIdAsync(bookingId);

            // Assert - 验证
            Assert.That(booking, Is.Not.Null, "Booking should exist");
            Assert.That(booking!.firstname, Is.Not.Null.And.Not.Empty, "Firstname should not be empty");
            Assert.That(booking.lastname, Is.Not.Null.And.Not.Empty, "Lastname should not be empty");
        }

        /// <summary>
        /// Test 3: Get non-existent booking
        /// 
        /// Learn: How API handles errors - 404 Not Found
        /// Expected: Status 404, Null or empty response
        /// </summary>
        [Test]
        [Order(3)]
        public async Task GetBookingById_WhenBookingDoesNotExist_ReturnsNotFound()
        {
            // Arrange - 准备
            int bookingId = 99999; // Non-existent booking

            // Act - 执行
            var booking = await _apiClient.GetBookingByIdAsync(bookingId);

            // Assert - 验证
            Assert.That(booking, Is.Null, "Non-existent booking should return null");
        }

        /// <summary>
        /// Test 4: Get all bookings
        /// 
        /// Learn: Fetching collections from API
        /// Expected: Status 200, List of bookings (can be empty)
        /// </summary>
        [Test]
        [Order(4)]
        public async Task GetAllBookings_WhenCalled_ReturnsListOfBookings()
        {
            // Arrange - 准备 (no setup needed)

            // Act - 执行
            var bookings = await _apiClient.GetAllBookingsAsync();

            // Assert - 验证
            Assert.That(bookings, Is.Not.Null, "Response should not be null");
            // Note: List can be empty if no bookings exist
        }

        /// <summary>
        /// Test 5: Validate booking structure with FluentAssertions
        /// 
        /// Learn: Using FluentAssertions for better test readability
        /// Expected: Booking has all required properties
        /// </summary>
        [Test]
        [Order(5)]
        public async Task GetBookingById_ValidateBookingStructure_WithFluentAssertions()
        {
            // Arrange
            int bookingId = 1;

            // Act
            var booking = await _apiClient.GetBookingByIdAsync(bookingId);

            // Assert - FluentAssertions style (more readable)
            booking.Should().NotBeNull("booking with ID {0} should exist", bookingId);
            booking!.Should().Match<Booking>(b => 
                b.firstname != null && 
                b.lastname != null, 
                "booking should have firstname and lastname");
        }
    }
}
