using System.Security.Claims;
using E621Browser.Data;
using E621Browser.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace E621Browser.Controllers;

[Authorize]
public class CommentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CommentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Adds comment under the artwork
    public async Task<IActionResult> Create(int artworkId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return BadRequest("Comment content cannot be empty.");
        }

        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var comment = new Comment
        {
            Content = content,
            UserEmail = userEmail,
            ArtworkId = artworkId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        
        // Redirect back to the artwork details page
        return RedirectToAction("Details", "Artwork", new { id = artworkId });
    }

    // Removes the user's comment
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.Artwork)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (comment == null)
        {
            return NotFound();
        }

        var userEmail = User.Identity.Name;

        if (comment.UserEmail != userEmail)
        {
            return Forbid();
        }

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        
        // Redirect back to the artwork details page
        return RedirectToAction("Details", "Artwork", new { id = comment.ArtworkId });
    }
}