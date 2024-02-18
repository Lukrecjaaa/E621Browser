using System.Net.Http.Headers;
using System.Text.Json;
using E621Browser.Data;
using E621Browser.DTOs;
using E621Browser.Models;

namespace E621Browser.Services;

public class ArtworkService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public ArtworkService(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        // Using IHttpClientFactory to manage HttpClient instances efficiently
        _httpClient = httpClientFactory.CreateClient("E621Client");
    }

    // Tries to add a new artwork to the database if it doesn't already exist
    public async Task TryAddArtworkAsync(Artwork artwork)
    {
        var dbArtwork = await _context.Artworks.FindAsync(artwork.Id);

        if (dbArtwork != null) return; // Exit if artwork already exists

        _context.Artworks.Add(artwork);
        await _context.SaveChangesAsync(); // Asynchronously save changes to the database
    }

    // Asynchronously retrieves artworks from the API based on a query and page number
    public async Task<List<Artwork>> GetArtworksFromApiAsync(string query, int page)
    {
        List<Artwork> artworks = new List<Artwork>();

        // Set up the request to the E621 API
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "E621Browser");

        // Asynchronously get the response from the API
        var response = await _httpClient.GetAsync($"https://e621.net/posts.json?tags=rating:safe {query}&limit=50&page={page}");

        if (response.IsSuccessStatusCode)
        {
            // Asynchronously read the response content
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<E621ListApiResponse>(content);

            if (apiResponse != null)
            {
                foreach (var post in apiResponse.Posts)
                {
                    var artwork = new Artwork
                    {
                        Id = post.Id,
                        Url = post.Sample.Url,
                        PreviewUrl = post.Preview.Url,
                        Description = post.Description,
                        Tags = post.Tags.General
                    };

                    artworks.Add(artwork);

                    // Attempt to add each fetched artwork to the database
                    await TryAddArtworkAsync(artwork);
                }
            }
        }

        return artworks;
    }
    
    // Asynchronously retrieves artwork from the API based on ID
    public async Task<Artwork> GetArtworkFromApiAsync(int id)
    {
        Artwork artwork = new Artwork();

        // Set up the request to the E621 API
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "E621Browser");

        // Asynchronously get the response from the API
        var response = await _httpClient.GetAsync($"https://e621.net/posts/{id}.json");

        if (response.IsSuccessStatusCode)
        {
            // Asynchronously read the response content
            var content = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<E621PostApiResponse>(content);

            if (apiResponse != null)
            {
                artwork = new Artwork
                {
                    Id = apiResponse.Post.Id,
                    Url = apiResponse.Post.Sample.Url,
                    PreviewUrl = apiResponse.Post.Preview.Url,
                    Description = apiResponse.Post.Description,
                    Tags = apiResponse.Post.Tags.General
                };
                
                await TryAddArtworkAsync(artwork);
            }
        }

        return artwork;
    }
}