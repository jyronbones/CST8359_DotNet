using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.Predictions
{
    public class CreateModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";

        private readonly Lab5.Data.PredictionDataContext _context;

        public CreateModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Prediction Prediction { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Upload != null)
            {
                BlobContainerClient containerClient;

                string containerName = Prediction.Question == Question.Earth ? earthContainerName : computerContainerName;
                try
                {
                    containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                }
                catch (RequestFailedException)
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                }

                Prediction.FileName = Upload.FileName;

                var blobClient = containerClient.GetBlobClient(Upload.FileName);

                using (var stream = Upload.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, true);
                }

                Prediction.Url = blobClient.Uri.AbsoluteUri;
            }
            else
            {
                ModelState.AddModelError("Upload", "File upload is required.");
                return Page();
            }

            _context.Predictions.Add(Prediction);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
