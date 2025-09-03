using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VMSApplication.Data;
using VMSApplication.Models;

namespace VMSApplication.Controllers
{
    [Authorize]
    public class VisitorBlacklistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorBlacklistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VisitorBlacklists
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.visitorBlacklists.Include(v => v.visitor).ToListAsync();
            return View(await applicationDbContext);
        }
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpGet]

        public async Task<IActionResult> IndexView()
        {
            var visitorblklist = await _context.visitorBlacklists.Include(x => x.visitor).ToListAsync();
            return PartialView("_IndexView", visitorblklist);
        }

        // GET: VisitorBlacklists/Details/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorBlacklist = await _context.visitorBlacklists
                .Include(v => v.visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorBlacklist == null)
            {
                return NotFound();
            }

            return View(visitorBlacklist);
        }

        //// GET: VisitorBlacklists/Create
        //[Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        //public IActionResult Create()
        //{
        //    ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id");
        //    return View();
        //}

        // GET: VisitorBlacklists/Create
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Step 1: Get logged-in user's ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Step 2: Get user's CompanyId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    ViewData["visitorId"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    return View();
                }

                var userCompanyId = user.CompanyId;

                // Step 3: Get visitors whose CompanyId matches user's CompanyId
                var visitors = await _context.visitorsregistration
                    .Where(v => v.CompanyId == userCompanyId)
                    .ToListAsync();

                ViewData["visitorId"] = new SelectList(visitors, "Id", "Id");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["visitorId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View();
        }


        // POST: VisitorBlacklists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,visitorId,CreatedId,createdOn,ModifiedId,ModifiedOn")] VisitorBlacklist visitorBlacklist)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                visitorBlacklist.CreatedId = UserId;
                visitorBlacklist.createdOn = DateTime.Now;
                visitorBlacklist.ModifiedId = UserId;
                visitorBlacklist.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);
                _context.Add(visitorBlacklist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id", visitorBlacklist.visitorId);
            return View(visitorBlacklist);
        }

        // GET: VisitorBlacklists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorBlacklist = await _context.visitorBlacklists.FindAsync(id);
            if (visitorBlacklist == null)
            {
                return NotFound();
            }
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id", visitorBlacklist.visitorId);
            return View(visitorBlacklist);
        }

        // POST: VisitorBlacklists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,visitorId,CreatedId,createdOn,ModifiedId,ModifiedOn")] VisitorBlacklist visitorBlacklist)
        {
            if (id != visitorBlacklist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitorBlacklist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorBlacklistExists(visitorBlacklist.Id))
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
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id", visitorBlacklist.visitorId);
            return View(visitorBlacklist);
        }

        // GET: VisitorBlacklists/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorBlacklist = await _context.visitorBlacklists
                .Include(v => v.visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorBlacklist == null)
            {
                return NotFound();
            }

            return View(visitorBlacklist);
        }

        // POST: VisitorBlacklists/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitorBlacklist = await _context.visitorBlacklists.FindAsync(id);
            if (visitorBlacklist != null)
            {
                _context.visitorBlacklists.Remove(visitorBlacklist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorBlacklistExists(int id)
        {
            return _context.visitorBlacklists.Any(e => e.Id == id);
        }
    }
}
