namespace E621Browser.Models;

// Represents an artwork saved by a user
public class SavedArtwork
{
    public int Id { get; set; } // Unique identifier for the saved artwork record
    public Artwork artwork { get; set; } // The artwork that was saved
    public List<string>? UserIds { get; set; } // List of user IDs that have saved this artwork
}