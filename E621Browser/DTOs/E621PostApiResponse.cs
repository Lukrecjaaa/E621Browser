using System.Text.Json.Serialization;

namespace E621Browser.DTOs;

public class E621PostApiResponse
{
    [JsonPropertyName("post")]
    public Post Post { get; set; } // Holds the Post objects returned by the API
}