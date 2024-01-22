namespace E621Browser.Models;

public class ArtworkModel
{
    public int Id { get; set; }
    public string? PreviewUrl { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public List<string>? Tags { get; set; } = new List<string>();
}