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
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "SuperAdmin")]
        // GET: Departments
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.departments.ToListAsync());
        }


        [HttpGet]

        public async Task<IActionResult> Indexview() 
        
        {
            return PartialView("_Indexview", await _context.departments.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,DepqartmentName,CreatedId,createdOn,ModifiedId,ModifiedOn")] Department department)
        {
            try
            {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool departmentExists = _context.departments.Any(x => x.DepqartmentName == department.DepqartmentName);
            if(departmentExists){
                TempData["Error"] = "Department Type already exists.";
                return View(department);

            }

            else
            {
                if (ModelState.IsValid)
                {
                    department.CreatedId = UserId;
                    department.createdOn = DateTime.Now;
                    department.ModifiedId = UserId;
                    department.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);
                    _context.Add(department);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Record inserted Successfully";
                    return RedirectToAction(nameof(Create));
                }

            }
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
            }


            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,DepqartmentName,CreatedId,createdOn,ModifiedId,ModifiedOn")] Department department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentID))
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
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.departments.FindAsync(id);
            if (department != null)
            {
                _context.departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.departments.Any(e => e.DepartmentID == id);
        }
    }
}
