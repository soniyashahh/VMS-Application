using System;
using System.Collections.Generic;
using System.Linq;
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

    public class HelpandSupportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HelpandSupportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HelpandSupports
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.helpandSupports.ToListAsync());
        }

        // GET: HelpandSupports/Details/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helpandSupport = await _context.helpandSupports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (helpandSupport == null)
            {
                return NotFound();
            }

            return View(helpandSupport);
        }

        // GET: HelpandSupports/Create
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: HelpandSupports/Create
        // To protect from overposting attacks, enable the specific properties you want
        // [Authorize(Roles = "Systemadmin,Superadmin,Security,User,
        // ,")]to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Facingissues,CreatedId,createdOn,ModifiedId,ModifiedOn")] HelpandSupport helpandSupport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(helpandSupport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(helpandSupport);
        }

        // GET: HelpandSupports/Edit/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helpandSupport = await _context.helpandSupports.FindAsync(id);
            if (helpandSupport == null)
            {
                return NotFound();
            }
            return View(helpandSupport);
        }

        // POST: HelpandSupports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Facingissues,CreatedId,createdOn,ModifiedId,ModifiedOn")] HelpandSupport helpandSupport)
        {
            if (id != helpandSupport.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(helpandSupport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HelpandSupportExists(helpandSupport.Id))
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
            return View(helpandSupport);
        }

        // GET: HelpandSupports/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helpandSupport = await _context.helpandSupports
                .FirstOrDefaultAsync(m => m.Id == id);
            if (helpandSupport == null)
            {
                return NotFound();
            }

            return View(helpandSupport);
        }

        // POST: HelpandSupports/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var helpandSupport = await _context.helpandSupports.FindAsync(id);
            if (helpandSupport != null)
            {
                _context.helpandSupports.Remove(helpandSupport);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HelpandSupportExists(int id)
        {
            return _context.helpandSupports.Any(e => e.Id == id);
        }
    }
}
