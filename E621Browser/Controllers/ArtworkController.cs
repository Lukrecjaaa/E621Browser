using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using E621Browser.Data;
using E621Browser.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E621Browser.Controllers;

public class ArtworkController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    
    public ArtworkController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public void TryAddArtwork(ArtworkModel artwork)
    {
        var dbArtwork = _context.Artworks.FirstOrDefault(a => a.Id == artwork.Id);

        if (dbArtwork != null) return;
        _context.Artworks.Add(artwork);
        _context.SaveChanges();
    }

    public List<ArtworkModel> GetArtworksFromApi(string query, int page)
    {
        List<ArtworkModel> artworks = new List<ArtworkModel>();

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("User-Agent", "E621Browser");
            
            HttpResponseMessage response = client.GetAsync($"https://e621.net/posts.json?tags=rating:safe {query}&limit=50&page={page}").Result;
            
            if (response.IsSuccessStatusCode)
            {
                string content = response.Content.ReadAsStringAsync().Result;
                
                var apiResponse = JsonSerializer.Deserialize<E621ListApiResponse>(content);
                
                if (apiResponse != null)
                {
                    foreach (var post in apiResponse.Posts)
                    {
                        var artwork = new ArtworkModel
                        {
                            Id = post.Id,
                            Url = post.Sample.Url,
                            PreviewUrl = post.Preview.Url,
                            Description = post.Description,
                        };

                        artworks.Add(artwork);
                    
                        TryAddArtwork(artwork);
                    }
                }
            }
        }

        return artworks;
    }

    // GET
    public IActionResult Index(int page = 1)
    {
        var artworks = GetArtworksFromApi("", page);
        ViewData["CurrentPage"] = page;
        return View(artworks);
    }

    [HttpGet("/Artwork/Search/{query}")]
    public IActionResult Search(string query, int page = 1)
    {
        var artworks = GetArtworksFromApi(query, page);
        ViewData["CurrentPage"] = page;
        ViewData["Query"] = query;
        return View(artworks);
    }

    public IActionResult Details(int id)
    {
        var artwork = _context.Artworks.FirstOrDefault(a => a.Id == id);
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        bool isSaved = _context.SavedArtworks.Any(sa => sa.artwork.Id == id && sa.UserId.Contains(userId));
        
        ViewBag.IsSaved = isSaved;
        
        return View(artwork);
    }

    [Authorize]
    public IActionResult Save(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var savedArtwork = _context.SavedArtworks.FirstOrDefault(a => a.artwork.Id == id);
        
        if (savedArtwork == null)
        {
            var artwork = _context.Artworks.FirstOrDefault(a => a.Id == id);
            _context.SavedArtworks.Add(new SavedArtworkModel
            {
                artwork = artwork,
                UserId = new List<string>{ userId }
            });
            _context.SaveChanges();
        }
        else
        {
            savedArtwork.UserId.Add(userId);
            _context.SaveChanges();
        }
        
        return RedirectToAction("SavedArtworks");
    }

    [Authorize]
    public IActionResult Unsave(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var savedArtwork = _context.SavedArtworks.FirstOrDefault(a => a.artwork.Id == id);

        if (savedArtwork != null)
        {
            savedArtwork.UserId.Remove(userId);
            _context.SaveChanges();
        }
        
        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult SavedArtworks()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var savedArtworks = _context.SavedArtworks
            .Where(a => a.UserId.Contains(userId)).Include(sa => sa.artwork)
            .ToList();

        var artworkList = new List<ArtworkModel>();

        foreach (var savedArtwork in savedArtworks)
        {
            var artwork = _context.Artworks.FirstOrDefault(a => a.Id == savedArtwork.artwork.Id);
            if (artwork != null) artworkList.Add(artwork);
        }
        
        return View(artworkList);
    }
}

public class Preview
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class Sample
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class Post
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("sample")]
    public Sample Sample { get; set; }
    [JsonPropertyName("preview")]
    public Preview Preview { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public class E621ListApiResponse
{
    [JsonPropertyName("posts")]
    public List<Post> Posts { get; set; }
}