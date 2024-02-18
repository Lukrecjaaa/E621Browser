using System.Security.Claims;
using E621Browser.Data;
using E621Browser.Models;
using E621Browser.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E621Browser.Controllers;

public class ArtworkController : Controller
{
    private readonly ArtworkService _artworkService;
    private readonly ApplicationDbContext _context;

    // Constructor for dependency injection, including ArtworkService, which is used for retrieving artworks from API
    public ArtworkController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ArtworkService artworkService)
    {
        _context = context;
        _artworkService = artworkService;
    }

    // Attempts to add unique artwork data to the database for later retrieval, to ensure the API isn't called too often
    public void TryAddArtwork(Artwork artwork)
    {
        var dbArtwork = _context.Artworks.FirstOrDefault(a => a.Id == artwork.Id);

        if (dbArtwork != null) return;
        _context.Artworks.Add(artwork);
        _context.SaveChanges();
    }

    // Displays a paginated list of artworks from the API
    public async Task<IActionResult> Index(int page = 1)
    {
        var artworks = await _artworkService.GetArtworksFromApiAsync("", page);
        ViewData["CurrentPage"] = page;
        return View(artworks);
    }

    // Displays a paginated list of artworks for a given search query
    [HttpGet("/Artwork/Search/{query}/{page?}")]
    public async Task<IActionResult> Search(string query, int page = 1)
    {
        var artworks = await _artworkService.GetArtworksFromApiAsync(query, page);
        ViewData["CurrentPage"] = page;
        ViewData["Query"] = query;
        return View(artworks); // TODO: Reuse the Index view if it's suitable for displaying search results
    }

    // Displays details for a single artwork
    public async Task<IActionResult> Details(int id)
    {
        var artwork = _context.Artworks
            .Include(a => a.Comments)
            .FirstOrDefault(a => a.Id == id);

        if (artwork == null)
        {
            artwork = await _artworkService.GetArtworkFromApiAsync(id);
        }
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        bool isSaved = _context.SavedArtworks.Any(sa => sa.artwork.Id == id && sa.UserIds.Contains(userId));
        
        ViewBag.IsSaved = isSaved;
        
        return View(artwork);
    }

    // Saves an artwork, this page is only available for the logged-in user
    [Authorize]
    public IActionResult Save(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var savedArtwork = _context.SavedArtworks.FirstOrDefault(a => a.artwork.Id == id);
        
        if (savedArtwork == null)
        {
            var artwork = _context.Artworks.FirstOrDefault(a => a.Id == id);
            _context.SavedArtworks.Add(new SavedArtwork
            {
                artwork = artwork,
                UserIds = new List<string>{ userId }
            });
            _context.SaveChanges();
        }
        else
        {
            savedArtwork.UserIds.Add(userId);
            _context.SaveChanges();
        }
        
        return RedirectToRefererOrFallback("SavedArtworks");
    }

    // Removes the saved artwork, this page is only available for the logged-in user
    [Authorize]
    public IActionResult Unsave(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var savedArtwork = _context.SavedArtworks.FirstOrDefault(a => a.artwork.Id == id);

        if (savedArtwork != null)
        {
            savedArtwork.UserIds.Remove(userId);
            _context.SaveChanges();
        }
        
        return RedirectToRefererOrFallback("SavedArtworks");
    }

    // Displays a list of saved artworks, this page is only available for the logged-in user
    [Authorize]
    public IActionResult SavedArtworks()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var savedArtworks = _context.SavedArtworks
            .Where(sa => sa.UserIds.Contains(userId)).Include(sa => sa.artwork)
            .ToList();

        return View(savedArtworks.Select(sa => sa.artwork).ToList());
    }
    
    // Helper method to redirect to the Referer header or a fallback action
    private IActionResult RedirectToRefererOrFallback(string fallbackAction)
    {
        var referer = Request.Headers["Referer"].ToString();
        return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction(fallbackAction);
    }
}