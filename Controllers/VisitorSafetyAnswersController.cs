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
    public class VisitorSafetyAnswersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorSafetyAnswersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VisitorSafetyAnswers
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.visitorSafetyAnswers.ToListAsync());
        }

        // GET: VisitorSafetyAnswers/Details/5
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorSafetyAnswer = await _context.visitorSafetyAnswers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorSafetyAnswer == null)
            {
                return NotFound();
            }

            return View(visitorSafetyAnswer);
        }

        // GET: VisitorSafetyAnswers/Create
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: VisitorSafetyAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Q1,Q2,Q3,Q4,Q5")] VisitorSafetyAnswer visitorSafetyAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visitorSafetyAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(visitorSafetyAnswer);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitAnswers([FromBody] VisitorSafetyAnswer model)
        {
            if (ModelState.IsValid)
            {
                _context.visitorSafetyAnswers.Add(model);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Answers saved successfully." });
            }

            return Json(new { success = false, message = "Invalid data." });
        }

        // GET: VisitorSafetyAnswers/Edit/5
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorSafetyAnswer = await _context.visitorSafetyAnswers.FindAsync(id);
            if (visitorSafetyAnswer == null)
            {
                return NotFound();
            }
            return View(visitorSafetyAnswer);
        }

        // POST: VisitorSafetyAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Q1,Q2,Q3,Q4,Q5")] VisitorSafetyAnswer visitorSafetyAnswer)
        {
            if (id != visitorSafetyAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitorSafetyAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorSafetyAnswerExists(visitorSafetyAnswer.Id))
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
            return View(visitorSafetyAnswer);
        }

        // GET: VisitorSafetyAnswers/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorSafetyAnswer = await _context.visitorSafetyAnswers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorSafetyAnswer == null)
            {
                return NotFound();
            }

            return View(visitorSafetyAnswer);
        }

        // POST: VisitorSafetyAnswers/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitorSafetyAnswer = await _context.visitorSafetyAnswers.FindAsync(id);
            if (visitorSafetyAnswer != null)
            {
                _context.visitorSafetyAnswers.Remove(visitorSafetyAnswer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorSafetyAnswerExists(int id)
        {
            return _context.visitorSafetyAnswers.Any(e => e.Id == id);
        }
    }
}
