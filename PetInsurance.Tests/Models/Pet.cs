
using System.Text.Json.Serialization;

public class Pet
{
    [JsonPropertyName("id")]
    public long Id { get; set; } // 🔍 long замість int

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("category")]
    public Category? Category { get; set; }

    [JsonPropertyName("photoUrls")]
    public List<string>? PhotoUrls { get; set; }

    [JsonPropertyName("tags")]
    public List<Tag>? Tags { get; set; }
}

public class Category
{
    [JsonPropertyName("id")]
    public long? Id { get; set; } // 🔍 nullable + long

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class Tag
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}