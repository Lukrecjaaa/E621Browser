using System.Text.Json.Serialization;

namespace E621Browser.DTOs;

// Represents the response received from the E621 API containing a list of posts
public class E621ListApiResponse
{
    [JsonPropertyName("posts")]
    public List<Post> Posts { get; set; } // Holds the list of Post objects returned by the API
}