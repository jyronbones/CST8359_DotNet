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
        _containerName = configuration.GetConnectionString("AzureBlobStorage:ContainerName");
    }

    public IActionResult Create()
    {
        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NewsId,FileName,Url,SportClubId")] News news, IFormFile file)
    {
        if (ModelState.IsValid)
        {
            if (file != null && file.Length > 0)
            {
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                BlobClient blobClient = containerClient.GetBlobClient(uniqueFileName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                news.Url = blobClient.Uri.ToString();
            }

            _context.Add(news);
            await _context.SaveChangesAsync();
            return Redirect($"/SportClubs/News/{news.NewsId}");
        }

        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Name", news.SportClubId);
        return View(news);
    }

    // Other actions...

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
