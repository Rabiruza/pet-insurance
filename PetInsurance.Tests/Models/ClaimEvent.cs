public class ClaimEvent
{
    public int ClaimId { get; set; }
    public string EventType { get; set; } = string.Empty; // "created", "approved", "rejected"
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ClaimData Data { get; set; } = new();
}

public class ClaimData
{
    public string PetName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}