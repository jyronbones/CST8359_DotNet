using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Lab4.Data;
using Lab4.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

public class NewsController : Controller
{
    private readonly SportsDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public NewsController(SportsDbContext context, IConfiguration configuration)
    {
        _context = context;
        _containerName = configuration.GetSection("AzureBlobStorage:ContainerName").Value;

        // Initialize BlobServiceClient with the connection string
        var connectionString = configuration.GetSection("AzureBlobStorage:ConnectionString").Value;
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    // GET: News/Create
    public IActionResult Create()
    {
        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Name");
        return View();
    }

    // POST: News/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NewsId,FileName,Url,SportClubId")] News news, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            // Check if a file was uploaded
            if (file != null && file.Length > 0)
            {
                // Generate a unique filename
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

                // Create a blob client
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

                // Upload the file to Azure Blob Storage
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                // Update the news object with the URL of the uploaded file
                news.Url = blobClient.Uri.ToString();
            }

            _context.Add(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Name", news.SportClubId);
        return View(news);
    }

    // GET: News/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var news = await _context.News
            .Include(n => n.SportClub)
            .FirstOrDefaultAsync(m => m.NewsId == id);
        if (news == null)
        {
            return NotFound();
        }

        return View(news);
    }

    // POST: News/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news != null)
        {
            // Delete the associated file from Azure Blob Storage (if applicable)
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