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
        _containerName = configuration["AzureBlobStorageSettings:ContainerName"];
    }

    // GET: News/Create/{subscriptionId}
    public IActionResult Create(string subscriptionId)
    {
      
        ViewData["SportClubId"] = new SelectList(_context.SportClubs, "Id", "Title");
        return View();
    }

    // POST: News/Create/{subscriptionId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string subscriptionId, [Bind("NewsId,FileName,Url,SportClubId")] News news, IFormFile Photo)
    {
      
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

    // GET: News/Delete/{news id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var news = await _context.News
            .Include(n => n.SportClub)
            .FirstOrDefaultAsync(n => n.NewsId == id);
        
        if (news == null)
        {
            return NotFound();
        }
       
        return View(news);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var news = await _context.News.FindAsync(id);

        if (news == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(news.Url))
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(Path.GetFileName(news.Url));
            await blobClient.DeleteIfExistsAsync();
        }

        var sportClubId = news.SportClubId;

        _context.News.Remove(news);
        await _context.SaveChangesAsync();
        return Redirect($"/SportClubs/News/{news.SportClubId}"); 
    }


}
