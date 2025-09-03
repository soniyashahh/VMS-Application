using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using VMSApplication.Data;
using VMSApplication.Models;

namespace VMSApplication.Controllers
{
    [Authorize(Roles = "Systemadmin,Superadmin")]
    public class MailconfigsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MailconfigsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mailconfigs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.mailconfigs.Include(m => m.company);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Indexview()
        {
            var applicationDbContext = _context.mailconfigs.Include(m => m.company);
            return PartialView("_Indexview", await applicationDbContext.ToListAsync());
        }

        // GET: Mailconfigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mailconfig = await _context.mailconfigs
                .Include(m => m.company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mailconfig == null)
            {
                return NotFound();
            }

            return View(mailconfig);
        }

        // GET: Mailconfigs/Create
        public IActionResult Create()
        {
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
            return View();
        }

        // POST: Mailconfigs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,mailId,CompanyId,CreatedId,createdOn,ModifiedId,ModifiedOn")] Mailconfig mailconfig)
        {
            try { 
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool mailExists = _context.mailconfigs.Any(x => x.mailId == mailconfig.mailId);
            if (mailExists)
            {
                TempData["Error"] = "Email Type already exists or Wrong Data.";
                return View(mailconfig);
            }
                else
                {
                    if (!ModelState.IsValid)
                 {
                mailconfig.CreatedId = UserId;
                mailconfig.createdOn = DateTime.Now;
                mailconfig.ModifiedId = UserId;
                mailconfig.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);
                _context.Add(mailconfig);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record inserted Successfully";
                return RedirectToAction(nameof(Create));
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            ViewData["CompanyId"] = new SelectList(_context.companys, "CompanyId", "CompanyId", mailconfig.CompanyId);
            return View(mailconfig);
        }

        // GET: Mailconfigs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mailconfig = await _context.mailconfigs.FindAsync(id);
            if (mailconfig == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.companys, "CompanyId", "CompanyId", mailconfig.CompanyId);
            return View(mailconfig);
        }

        // POST: Mailconfigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,mailId,CompanyId,CreatedId,createdOn,ModifiedId,ModifiedOn")] Mailconfig mailconfig)
        {
            if (id != mailconfig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mailconfig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MailconfigExists(mailconfig.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.companys, "CompanyId", "CompanyId", mailconfig.CompanyId);
            return View(mailconfig);
        }

        // GET: Mailconfigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mailconfig = await _context.mailconfigs
                .Include(m => m.company)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mailconfig == null)
            {
                return NotFound();
            }

            return View(mailconfig);
        }

        // POST: Mailconfigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mailconfig = await _context.mailconfigs.FindAsync(id);
            if (mailconfig != null)
            {
                _context.mailconfigs.Remove(mailconfig);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MailconfigExists(int id)
        {
            return _context.mailconfigs.Any(e => e.Id == id);
        }
    }
}
