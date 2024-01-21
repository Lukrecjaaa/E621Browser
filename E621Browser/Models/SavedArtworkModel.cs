namespace E621Browser.Models;

public class SavedArtworkModel
{
    public int Id { get; set; }
    public ArtworkModel artwork { get; set; }
    public List<string>? UserId { get; set; }
}