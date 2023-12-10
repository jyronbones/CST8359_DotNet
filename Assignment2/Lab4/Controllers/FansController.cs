using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Lab4.Data;
using Lab4.Models;
using Lab4.Models.ViewModels;

namespace Lab4.Controllers
{
    public class FansController : Controller
    {
        private readonly SportsDbContext _context;

        public FansController(SportsDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Index(string selectedFanId)
        {
            var fans = await _context.Fans
                                     .Include(f => f.Subscriptions)
                                         .ThenInclude(s => s.SportClub)
                                     .ToListAsync();

            var fanViewModels = fans.Select(f => new FanSubscriptionViewModel
            {
                Fan = f,
                Subscriptions = f.Subscriptions.Select(s => new SportClubSubscriptionViewModel
                {
                    SportClubId = s.SportClub.Id,
                    Title = s.SportClub.Title,
                    IsMember = true
                }),
                IsSelected = f.Id.ToString() == selectedFanId
            }).ToList();

            return View(fanViewModels);
        }

        // GET: Fans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // GET: Fans/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fan);
        }

        // GET: Fans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans.FindAsync(id);
            if (fan == null)
            {
                return NotFound();
            }
            return View(fan);
        }

        // POST: Fans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Fan fan)
        {
            if (id != fan.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FanExists(fan.Id))
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
            return View(fan);
        }

        // GET: Fans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fan == null)
            {
                return NotFound();
            }

            return View(fan);
        }

        // POST: Fans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fans == null)
            {
                return Problem("Entity set 'SportsDbContext.Fans'  is null.");
            }
            var fan = await _context.Fans.FindAsync(id);
            if (fan != null)
            {
                _context.Fans.Remove(fan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FanExists(int id)
        {
            return _context.Fans.Any(e => e.Id == id);
        }

        // POST: Fans/AddSubscriptions
        [HttpPost]
        public async Task<IActionResult> AddSubscriptions(int fanId, string sportsClubId)
        {
            if (!SubscriptionExists(fanId, sportsClubId))
            {
                var subscription = new Subscription { FanId = fanId, SportClubId = sportsClubId };
                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Fans/RemoveSubscriptions
        [HttpPost]
        public async Task<IActionResult> RemoveSubscriptions(int fanId, string sportsClubId)
        {
            var subscription = await _context.Subscriptions
                                             .FirstOrDefaultAsync(s => s.FanId == fanId && s.SportClubId == sportsClubId);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int fanId, string sportsClubId)
        {
            return _context.Subscriptions.Any(e => e.FanId == fanId && e.SportClubId == sportsClubId);
        }

        // GET: Fans/EditSubscriptions/5
        public async Task<IActionResult> EditSubscriptions(int? id)
        {
            if (id == null || _context.Fans == null)
            {
                return NotFound();
            }

            var fan = await _context.Fans
                                    .Include(f => f.Subscriptions)
                                    .ThenInclude(s => s.SportClub)
                                    .FirstOrDefaultAsync(m => m.Id == id);

            if (fan == null)
            {
                return NotFound();
            }

            var allSportClubs = await _context.SportClubs.ToListAsync();
            var subscribedClubs = fan.Subscriptions.Select(s => s.SportClub).ToList();
            var notSubscribedClubs = allSportClubs.Except(subscribedClubs).OrderBy(s => s.Title).ToList();

            var fanViewModel = new FanSubscriptionViewModel
            {
                Fan = fan,
                Subscriptions = subscribedClubs.Select(s => new SportClubSubscriptionViewModel
                {
                    SportClubId = s.Id,
                    Title = s.Title,
                    IsMember = true
                })
                .Concat(notSubscribedClubs.Select(s => new SportClubSubscriptionViewModel
                {
                    SportClubId = s.Id,
                    Title = s.Title,
                    IsMember = false
                }))
            };

            return View(fanViewModel);
        }
    }
}
