using System.Text.Json.Serialization;

namespace E621Browser.DTOs;

public class Tags
{
    [JsonPropertyName("general")]
    public List<String> General { get; set; } // List of "general" tags
}