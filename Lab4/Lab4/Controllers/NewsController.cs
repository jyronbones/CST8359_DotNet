using Lab4.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
using Lab4.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public class NewsController : Controller
{
    private readonly SportsDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public NewsController(SportsDbContext context, IConfiguration configuration, BlobServiceClient blobServiceClient)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["AzureBlobStorageSettings:ContainerName"];
    }

    // GET: News/Create
    public IActionResult Create()
    {
        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Title"); // Changed "Name" to "Title"
        return View();
    }

    // POST: News/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NewsId,FileName,Url,SportClubId")] News news, IFormFile Photo)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (Photo != null && Photo.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Photo.FileName)}";
                    BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                    BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

                    using (var stream = Photo.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    news.FileName = uniqueFileName;
                    news.Url = blobClient.Uri.ToString();
                }

                _context.News.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred while uploading the file: {ex.Message}";
            }
        }

        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Title", news.SportClubId); // Ensure SelectList is set for re-display
        return View(news);
    }

    // GET: News
    public async Task<IActionResult> Index()
    {
        var newsList = await _context.News.Include(n => n.SportClub).ToListAsync();
        return View(newsList);
    }

    // POST: News/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news != null)
        {
            if (!string.IsNullOrEmpty(news.Url))
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                BlobClient blobClient = containerClient.GetBlobClient(Path.GetFileName(news.Url));
                await blobClient.DeleteIfExistsAsync();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
