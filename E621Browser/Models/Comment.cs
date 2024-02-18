namespace E621Browser.Models;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UserEmail { get; set; }
    
    // Foreign key for Artwork
    public int ArtworkId { get; set; }
    public Artwork Artwork { get; set; }
}