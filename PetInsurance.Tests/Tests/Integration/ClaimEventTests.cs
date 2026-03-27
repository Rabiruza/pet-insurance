
using FluentAssertions;

[TestFixture]
public class ClaimEventTests
{
    private MockServiceBusClient _mockServiceBus = null!;

    [SetUp]
    public void SetUp()
    {
        _mockServiceBus = new MockServiceBusClient();
    }

    [Test]
    public async Task WhenClaimCreated_EventSentToQueue()
    {
        // Arrange
        var claimEvent = new ClaimEvent
        {
            ClaimId = 123,
            EventType = "created",
            Data = new ClaimData
            {
                PetName = "Buddy",
                OwnerName = "Anna",
                Amount = 250.50m,
                Status = "pending"
            }
        };

        // Act
        var sent = await _mockServiceBus.SendMessageAsync("claims-queue", claimEvent);

        // Assert
        sent.Should().BeTrue();
        _mockServiceBus.SentMessages.Should().HaveCount(1);
        
        // 🔍 Перевіряємо, що повідомлення валідне
        var received = await _mockServiceBus.ReceiveMessageAsync<ClaimEvent>("claims-queue");
        received.Should().NotBeNull();
        received!.ClaimId.Should().Be(123);
        received.EventType.Should().Be("created");
        received.Data.PetName.Should().Be("Buddy");
    }

    [Test]
    public async Task WhenQueueEmpty_ReceiveReturnsNull()
    {
        // Act
        var result = await _mockServiceBus.ReceiveMessageAsync<ClaimEvent>("empty-queue");

        // Assert
        result.Should().BeNull();
    }
}