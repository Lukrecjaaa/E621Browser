using System.Text.Json.Serialization;

namespace E621Browser.DTOs;

public class Sample
{
    [JsonPropertyName("url")]
    public string Url { get; set; } // Url of the image
}