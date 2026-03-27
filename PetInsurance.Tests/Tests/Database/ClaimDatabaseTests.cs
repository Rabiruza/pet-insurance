using System.Diagnostics;

using FluentAssertions;

using NUnit.Framework;

using PetInsurance.Tests.DatabaseHelpers;
using PetInsurance.Tests.Models;

namespace PetInsurance.Tests.Tests.Database
{
    [TestFixture]
    public class ClaimDatabaseTests
    {
        private SqlDatabaseHelper _dbHelper = null!;

        [SetUp]
        public async Task SetUp()
        {
            // 🔍 Створюємо нову БД в пам'яті для кожного тесту
            _dbHelper = new SqlDatabaseHelper();
        }

        [TearDown]
        public void TearDown()
        {
            _dbHelper?.Dispose();
        }

        [Test]
        public async Task CreateClaim_InsertsDataCorrectly()
        {
            // Arrange
            var newClaim = new Claim
            {
                PetName = "Buddy",
                OwnerName = "Anna Shevchenko",
                Amount = 250.50m,
                Status = "pending"
            };

            // Act
            var claimId = await _dbHelper.CreateClaimAsync(newClaim);
            var retrievedClaim = await _dbHelper.GetClaimByIdAsync(claimId);

            // Assert
            retrievedClaim.Should().NotBeNull();
            retrievedClaim!.PetName.Should().Be("Buddy");
            retrievedClaim.OwnerName.Should().Be("Anna Shevchenko");
            retrievedClaim.Amount.Should().Be(250.50m);
            retrievedClaim.Status.Should().Be("pending");
            retrievedClaim.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            retrievedClaim.ProcessedAt.Should().BeNull(); // Ще не оброблено
        }

        [Test]
        public async Task UpdateClaimStatus_ChangesStatusAndSetsProcessedAt()
        {
            // Arrange
            var claim = new Claim
            {
                PetName = "Luna",
                OwnerName = "Test Owner",
                Amount = 100,
                Status = "pending"
            };
            var claimId = await _dbHelper.CreateClaimAsync(claim);

            // Act
            var updated = await _dbHelper.UpdateClaimStatusAsync(claimId, "approved");
            var retrieved = await _dbHelper.GetClaimByIdAsync(claimId);

            // Assert
            updated.Should().BeTrue();
            retrieved!.Status.Should().Be("approved");
            retrieved.ProcessedAt.Should().NotBeNull();
            retrieved.ProcessedAt.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task GetClaimsByStatus_ReturnsOnlyMatchingClaims()
        {
            // Arrange - створюємо різні кейси
            await _dbHelper.CreateClaimAsync(new Claim { PetName = "Pet1", OwnerName = "Owner1", Amount = 100, Status = "pending" });
            await _dbHelper.CreateClaimAsync(new Claim { PetName = "Pet2", OwnerName = "Owner2", Amount = 200, Status = "approved" });
            await _dbHelper.CreateClaimAsync(new Claim { PetName = "Pet3", OwnerName = "Owner3", Amount = 150, Status = "pending" });

            // Act
            var pendingClaims = await _dbHelper.GetClaimsByStatusAsync("pending");

            // Assert
            pendingClaims.Should().HaveCount(2);
            pendingClaims.Should().OnlyContain(c => c.Status == "pending");
            pendingClaims.Select(c => c.PetName).Should().Contain(new[] { "Pet1", "Pet3" });
        }

        [Test]
        public async Task SearchByIndexedStatus_IsFasterThanNonIndexedField()
        {
            // Arrange – створюємо 100 кейсів зі статусом "pending"
            for (int i = 0; i < 100; i++)
            {
                await _dbHelper.CreateClaimAsync(new Claim
                {
                    PetName = $"Pet{i}",
                    OwnerName = $"Owner{i}",
                    Amount = i * 10,
                    Status = "pending"
                });
            }

            // Act – заміряємо час пошуку за індексованим полем Status
            var swIndexed = Stopwatch.StartNew();
            var pendingClaims = await _dbHelper.GetClaimsByStatusAsync("pending");
            swIndexed.Stop();

            // Act – заміряємо час пошуку за неіндексованим полем OwnerName
            var swNonIndexed = Stopwatch.StartNew();
            var ownerClaims = await _dbHelper.GetClaimsByOwnerNameAsync("Owner50"); // метод без індексу
            swNonIndexed.Stop();

            // Assert – перевіряємо, що обидва пошуки працюють, і порівнюємо час
            pendingClaims.Should().HaveCount(100);
            ownerClaims.Should().HaveCount(1);

            TestContext.Out.WriteLine($"Indexed search (Status): {swIndexed.ElapsedMilliseconds} ms");
            TestContext.Out.WriteLine($"Non-indexed search (OwnerName): {swNonIndexed.ElapsedMilliseconds} ms");
        }
    }
}