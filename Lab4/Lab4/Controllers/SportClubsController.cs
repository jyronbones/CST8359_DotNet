using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;

namespace Lab4.Controllers
{
    public class SportClubsController : Controller
    {
        private readonly SportsDbContext _context;

        public SportClubsController(SportsDbContext context)
        {
            _context = context;
        }

        // GET: SportClubs
        public async Task<IActionResult> Index(string id)
        {
            if (id != null)
            {
                var _viewModel = new NewsViewModel
                {
                    SportClubs = await _context.SportClubs.ToListAsync(),
                    Fans = await _context.Fans.ToListAsync(),
                    Subscriptions = await _context.Subscriptions.ToListAsync(),
                    currentId = id
                };
                return View(_viewModel);
            }

            var viewModel = new NewsViewModel
            {
                SportClubs = await _context.SportClubs.ToListAsync(),
                Fans = await _context.Fans.ToListAsync(),
                Subscriptions = await _context.Subscriptions.ToListAsync(),
                currentId = string.Empty
            };
            return View(viewModel);
            

        }

        // GET: SportClubs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.SportClubs == null)
            {
                return NotFound();
            }

            var sportClub = await _context.SportClubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportClub == null)
            {
                return NotFound();
            }

            return View(sportClub);
        }

        // GET: SportClubs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SportClubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Fee")] SportClub sportClub)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sportClub);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sportClub);
        }

        // GET: SportClubs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SportClubs == null)
            {
                return NotFound();
            }

            var sportClub = await _context.SportClubs.FindAsync(id);
            if (sportClub == null)
            {
                return NotFound();
            }
            return View(sportClub);
        }

        // POST: SportClubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Fee")] SportClub sportClub)
        {
            if (id != sportClub.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportClub);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportClubExists(sportClub.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sportClub);
        }

        // GET: SportClubs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.SportClubs == null)
            {
                return NotFound();
            }

            var sportClub = await _context.SportClubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportClub == null)
            {
                return NotFound();
            }

            return View(sportClub);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var newsCount = await _context.News.CountAsync(news => news.SportClubId == id);

            if (newsCount > 0)
            {
                TempData["AlertMessage"] = "Cannot delete SportsClub as it has associated news.";
                return RedirectToAction(nameof(Index)); // Redirect to Index without deleting
            }

            var sportClub = await _context.SportClubs.FindAsync(id);
            if (sportClub == null)
            {
                return NotFound();
            }

            _context.SportClubs.Remove(sportClub);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: SportClubs/News/5
        public async Task<IActionResult> News(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Assuming there's a relationship between SportClubs and News in your model
            var sportClubNews = await _context.News
                .Where(n => n.SportClubId == id) // Replace 'SportClubId' with the actual foreign key property
                .ToListAsync();

            if (sportClubNews == null)
            {
                return NotFound();
            }

            return View(sportClubNews);
        }

        private bool SportClubExists(string id)
        {
          return _context.SportClubs.Any(e => e.Id == id);
        }
    }
}
