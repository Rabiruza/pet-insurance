using RestSharp;
using FluentAssertions;
using PetInsurance.Tests.Core;
using PetInsurance.Tests.ApiClients.RestfulBooker;
using PetInsurance.Tests.Models.RestfulBooker;

namespace PetInsurance.Tests.Tests.API.RestfulBooker
{
    /// <summary>
    /// Lesson 4: DELETE Operations and Complete CRUD Workflows
    /// 
    /// What you'll learn:
    /// - How to delete resources
    /// - Understanding 204 No Content response
    /// - Complete CRUD workflow tests
    /// - Test cleanup patterns
    /// </summary>
    [TestFixture]
    [Category("API")]
    [Category("Lesson4")]
    public class Lesson4_DeleteBookingTests : BaseApiTest
    {
        private RestfulBookerClient _apiClient = null!;
        private string _token = null!;

        [SetUp]
        public async Task SetUp()
        {
            _apiClient = new RestfulBookerClient();
            
            // Get authentication token
            var tokenResponse = await _apiClient.CreateTokenAsync("admin", "password123");
            _token = tokenResponse?.token ?? throw new InvalidOperationException("Failed to get token");
        }

        [TearDown]
        public void TearDown()
        {
            _apiClient?.Dispose();
        }

        /// <summary>
        /// Test 1: Create and Delete booking (Complete workflow)
        /// 
        /// Learn: Full CRUD lifecycle - Create, Read, Update, Delete
        /// </summary>
        [Test]
        [Order(1)]
        public async Task CrudWorkflow_CreateReadUpdateDelete_Success()
        {
            // CREATE - 创建
            var newBooking = new Booking
            {
                firstname = "CRUD",
                lastname = "Test",
                totalprice = 50,
                depositpaid = true
            };
            var createResponse = await _apiClient.CreateBookingAsync(newBooking);
            createResponse.Should().NotBeNull("create should succeed");
            int bookingId = createResponse!.bookingid;

            try
            {
                // READ - 读取
                var getBooking = await _apiClient.GetBookingByIdAsync(bookingId);
                getBooking.Should().NotBeNull("booking should exist after creation");
                getBooking!.firstname.Should().Be("CRUD");

                // UPDATE - 更新
                var update = new PartialUpdate { totalprice = 75 };
                var updatedBooking = await _apiClient.PartialUpdateBookingAsync(bookingId, update, _token);
                updatedBooking.Should().NotBeNull();
                updatedBooking!.totalprice.Should().Be(75, "price should be updated");

                // DELETE - 删除
                var deleteResult = await _apiClient.DeleteBookingAsync(bookingId, _token);
                deleteResult.Should().BeTrue("delete should succeed");

                // Verify deletion
                var deletedBooking = await _apiClient.GetBookingByIdAsync(bookingId);
                deletedBooking.Should().BeNull("booking should not exist after deletion");
            }
            finally
            {
                // Cleanup: Ensure booking is deleted even if test fails
                try
                {
                    await _apiClient.DeleteBookingAsync(bookingId, _token);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Test 2: Delete existing booking
        /// 
        /// Learn: DELETE returns 204 No Content (no response body)
        /// </summary>
        [Test]
        [Order(2)]
        public async Task DeleteBooking_ExistingBooking_ReturnsNoContent()
        {
            // Arrange - Create booking first
            var booking = new Booking { firstname = "ToDelete", lastname = "User" };
            var createResponse = await _apiClient.CreateBookingAsync(booking);
            int bookingId = createResponse!.bookingid;

            try
            {
                // Act - Delete the booking
                var deleteResult = await _apiClient.DeleteBookingAsync(bookingId, _token);

                // Assert
                deleteResult.Should().BeTrue("delete should return success");
                
                // Verify it's gone
                var getDeleted = await _apiClient.GetBookingByIdAsync(bookingId);
                getDeleted.Should().BeNull("booking should be deleted");
            }
            finally
            {
                // Cleanup
                try { await _apiClient.DeleteBookingAsync(bookingId, _token); } catch { }
            }
        }

        /// <summary>
        /// Test 3: Delete non-existent booking
        /// 
        /// Learn: Deleting non-existent resources
        /// Expected: 404 Not Found or 204 (idempotent operation)
        /// </summary>
        [Test]
        [Order(3)]
        public async Task DeleteBooking_NonExistentBooking_ReturnsNotFound()
        {
            // Act
            var deleteResult = await _apiClient.DeleteBookingAsync(99999, _token);

            // Assert
            deleteResult.Should().BeFalse("deleting non-existent booking should fail");
        }

        /// <summary>
        /// Test 4: Delete without authentication
        /// 
        /// Learn: Protected DELETE endpoint
        /// </summary>
        [Test]
        [Order(4)]
        public async Task DeleteBooking_WithoutToken_ReturnsForbidden()
        {
            // Arrange - Create booking
            var booking = new Booking { firstname = "NoAuth", lastname = "Test" };
            var createResponse = await _apiClient.CreateBookingAsync(booking);
            int bookingId = createResponse!.bookingid;

            try
            {
                // Act - Try to delete without valid token
                var deleteResult = await _apiClient.DeleteBookingAsync(bookingId, "");

                // Assert
                deleteResult.Should().BeFalse("delete without auth should fail");
            }
            finally
            {
                // Cleanup with valid token
                try { await _apiClient.DeleteBookingAsync(bookingId, _token); } catch { }
            }
        }

        /// <summary>
        /// Test 5: Idempotent DELETE (delete twice)
        /// 
        /// Learn: DELETE is idempotent - deleting twice has same effect as once
        /// </summary>
        [Test]
        [Order(5)]
        public async Task DeleteBooking_Twice_IdempotentBehavior()
        {
            // Arrange - Create booking
            var booking = new Booking { firstname = "Idempotent", lastname = "Test" };
            var createResponse = await _apiClient.CreateBookingAsync(booking);
            int bookingId = createResponse!.bookingid;

            // First delete - should succeed
            var firstDelete = await _apiClient.DeleteBookingAsync(bookingId, _token);
            firstDelete.Should().BeTrue("first delete should succeed");

            // Second delete - booking already gone
            var secondDelete = await _apiClient.DeleteBookingAsync(bookingId, _token);
            // Note: Second delete may return false (404) - this is expected
            // Idempotency means the STATE is the same, not the response
        }
    }
}
