using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab5.Models;

namespace Lab5.Pages.Predictions
{
    public class IndexModel : PageModel
    {
        private readonly Lab5.Data.PredictionDataContext _context;

        public IndexModel(Lab5.Data.PredictionDataContext context)
        {
            _context = context;
        }

        public IList<Prediction> Prediction { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Predictions != null)
            {
                Prediction = await _context.Predictions.ToListAsync();
            }
        }
    }
}
