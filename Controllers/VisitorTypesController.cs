using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMSApplication.Data;
using VMSApplication.Models;

namespace VMSApplication.Controllers
{
    [Authorize(Roles = "Systemadmin,Superadmin")]
    public class VisitorTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VisitorTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.visitertypes.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Indexview()
        {
            return PartialView("_Indexview", await _context.visitertypes.ToListAsync());
        }

        // GET: VisitorTypes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: VisitorTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Visitortype,Description")] VisitorType visitors)
        {
            // Check if a visitor type with the same name already exists (instead of checking by Id)
            bool visitorExists = _context.visitertypes.Any(x => x.Visitortype == visitors.Visitortype);

            if (visitorExists)
            {
                TempData["Error"] = "Visitor Type already exists.";
                return View(visitors);
            }

            if (ModelState.IsValid)
            {
                _context.Add(visitors);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Record inserted successfully.";
                return RedirectToAction(nameof(Create));  // Redirect to Create to reload form
            }

            return View(visitors);  // If ModelState is invalid, return the same view with validation errors
        }

        // GET: VisitorTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var visitorType = await _context.visitertypes.FindAsync(id);
            if (visitorType == null)
                return NotFound();

            return View(visitorType);
        }

        // POST: VisitorTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Visitortype,Description")] VisitorType visitorType)
        {
            if (id != visitorType.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitorType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorTypeExists(visitorType.Id))
                        return NotFound();
                    else
                        throw;
                }
                TempData["Success"] = "Record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(visitorType);
        }

        // GET: VisitorTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var visitorType = await _context.visitertypes.FirstOrDefaultAsync(m => m.Id == id);
            if (visitorType == null)
                return NotFound();

            return View(visitorType);
        }


        // POST: VisitorTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitorType = await _context.visitertypes.FindAsync(id);
            if (visitorType != null)
                _context.visitertypes.Remove(visitorType);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorTypeExists(int id)
        {
            return _context.visitertypes.Any(e => e.Id == id);
        }
    }
}
