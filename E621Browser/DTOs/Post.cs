using System.Text.Json.Serialization;

namespace E621Browser.DTOs;

// Defines the structure of a post as returned by the E621 API
public class Post
{
    [JsonPropertyName("id")]
    public int Id { get; set; } // Unique ID of the post
    [JsonPropertyName("sample")]
    public Sample Sample { get; set; } // Contains url to the full version of the post
    [JsonPropertyName("tags")]
    public Tags Tags { get; set; } // Holds tags associated with the post
    [JsonPropertyName("preview")]
    public Preview Preview { get; set; } // Contains url to a scaled down thumbnail of the post
    [JsonPropertyName("description")]
    public string Description { get; set; } // Description of the post
}