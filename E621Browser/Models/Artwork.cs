namespace E621Browser.Models;

// Represents an artwork within the application, derived from a post in the E621 API
public class Artwork
{
    public int Id { get; set; } // Unique identifier for the artwork
    public string? PreviewUrl { get; set; } // URL of the artwork's preview image
    public string? Url { get; set; } // URL of the artwork's full image
    public string? Description { get; set; } // Description of the artwork
    public List<string>? Tags { get; set; } = new List<string>(); // Tags associated with the artwork
}