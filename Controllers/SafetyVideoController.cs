using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VMSApplication.Data;
using VMSApplication.Models;

namespace VMSApplication.Controllers
{
    [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
    public class SafetyVideoController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public SafetyVideoController(ApplicationDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHostEnvironment = webHost;
        }
        public IActionResult Index()
        {
            var data = _context.securityForms
                .Include(s => s.Visitor) // Include Visitor if you're accessing item.Visitor.Id
                .ToList(); // Ensure it's a list, not null

            return View(data); // data must not be null
        }

        public IActionResult Create()
        {
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetSafetyVideoByCompany(int companyId)
        {
            var video = await _context.SafetyVideo
                .Where(v => v.CompanyId == companyId)
                .OrderByDescending(v => v.Id)
                .FirstOrDefaultAsync();

            if (video == null)
                return Json(null);

            return Json(new { videoPath = video.FilePath, title = video.Title });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SafetyVideo model)
        {
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
            try
            {


            if (!ModelState.IsValid)
            {
                if (model.VideoFile != null && model.VideoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "videos");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.VideoFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.VideoFile.CopyToAsync(stream);
                    }

                    model.FilePath = "/videos/" + uniqueFileName;
                }

                model.CreatedId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.SafetyVideo.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Safety video uploaded successfully!";

                return RedirectToAction("Create");
            }
                else
                {
                    TempData["Error"] = "Data Not Saved";
                }

            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            // Debugging
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }

            return View(model);
        }



    }
}
