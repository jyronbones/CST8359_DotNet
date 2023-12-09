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
using Microsoft.AspNetCore.Http.Extensions;

public class NewsController : Controller
{
    private readonly SportsDbContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public NewsController(SportsDbContext context, IConfiguration configuration, BlobServiceClient blobServiceClient)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;
        //_containerName = configuration["AzureBlobStorageSettings:ContainerName"];
    }

    // GET: News/Create/{subscriptionId}
    public IActionResult Create(string subscriptionId)
    {
        string _containerName = GetContainerNameFromSubscriptionId(subscriptionId);
        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Title");
        return View();
    }

    // POST: News/Create/{subscriptionId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string subscriptionId, [Bind("NewsId,FileName,Url,SportClubId")] News news, IFormFile Photo)
    {
        string _containerName = GetContainerNameFromSubscriptionId(subscriptionId);
        if (ModelState.IsValid)
        {
            try
            {
                news.SportClubId = subscriptionId;

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
                    var currentUrl = HttpContext.Request.GetDisplayUrl();
                    news.SportClubId = currentUrl.Substring(Math.Max(0, currentUrl.Length - 2));
                }

                _context.News.Add(news);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return Redirect($"/SportClubs/News/{news.SportClubId}");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"An error occurred while uploading the file: {ex.Message}";
            }
        }

        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Title", news.SportClubId);
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

    private string GetContainerNameFromSubscriptionId(string subscriptionId)
    {
        var currentUrl = HttpContext.Request.GetDisplayUrl();
        var lastTwoChars = currentUrl.Substring(Math.Max(0, currentUrl.Length - 2));

        string containerName = "";

        
            if (lastTwoChars.Equals("A1", StringComparison.OrdinalIgnoreCase))
            {
                containerName = "alpha";
            }
            else if (lastTwoChars.Equals("B1", StringComparison.OrdinalIgnoreCase))
            {
                containerName = "beta";
            }
            else if (lastTwoChars.Equals("O1", StringComparison.OrdinalIgnoreCase))
            {
                containerName = "omega";
            }
        

        return containerName;
    }

}
