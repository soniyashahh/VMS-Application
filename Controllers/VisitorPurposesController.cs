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
    [Authorize(Roles = "Systemadmin,Superadmin")]
    public class VisitorPurposesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorPurposesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VisitorPurposes
        public async Task<IActionResult> Index()
        {
            return View(await _context.visitorPurposes.ToListAsync());
        }

        [HttpGet]

        public async Task<IActionResult> Indexview()

        {
            return PartialView("_Indexview", await _context.visitorPurposes.ToListAsync());
        }

        // GET: VisitorPurposes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorPurpose = await _context.visitorPurposes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorPurpose == null)
            {
                return NotFound();
            }

            return View(visitorPurpose);
        }
        public IActionResult Create()
        {
            return View();
        }

        // POST: VisitorPurposes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PurposeName,Description,CreatedId,createdOn,ModifiedId,ModifiedOn")] VisitorPurpose visitorPurpose)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                visitorPurpose.CreatedId = UserId;
                visitorPurpose.createdOn = DateTime.Now;
                visitorPurpose.ModifiedId = UserId;
                visitorPurpose.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);
                _context.Add(visitorPurpose);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(visitorPurpose);
        }

        // GET: VisitorPurposes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorPurpose = await _context.visitorPurposes.FindAsync(id);
            if (visitorPurpose == null)
            {
                return NotFound();
            }
            return View(visitorPurpose);
        }

        // POST: VisitorPurposes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PurposeName,Description,CreatedId,createdOn,ModifiedId,ModifiedOn")] VisitorPurpose visitorPurpose)
        {
            if (id != visitorPurpose.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitorPurpose);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorPurposeExists(visitorPurpose.Id))
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
            return View(visitorPurpose);
        }

        // GET: VisitorPurposes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorPurpose = await _context.visitorPurposes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorPurpose == null)
            {
                return NotFound();
            }

            return View(visitorPurpose);
        }

        // POST: VisitorPurposes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitorPurpose = await _context.visitorPurposes.FindAsync(id);
            if (visitorPurpose != null)
            {
                _context.visitorPurposes.Remove(visitorPurpose);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorPurposeExists(int id)
        {
            return _context.visitorPurposes.Any(e => e.Id == id);
        }
    }
}
