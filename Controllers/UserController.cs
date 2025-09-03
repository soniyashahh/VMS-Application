using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Data;
using System.Security.Claims;
using VMSApplication.Data;
using VMSApplication.Data.Migrations;
using VMSApplication.Models;
using VMSApplication.UserViewModel;

namespace VMSApplication.Controllers
{
    [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
    public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public UserController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_context = context;
			_roleManager = roleManager;
			_userManager = userManager;
			_signInManager = signInManager;
		}
		[HttpGet]
        public async Task<ActionResult> Index()
        {
            var users = await _context.Users.Include(x => x.Role).ToListAsync();
            return View(users);
        }



        //[HttpGet]
        //public async Task<ActionResult> Create()
        //{
        //    ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
        //    ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
        //    ViewData["DepId"] = new SelectList(_context.departments, "DepartmentID", "DepqartmentName");
        //    ViewData["DesId"] = new SelectList(_context.designations, "Id", "DesignationName");

        //    return View();
        //}

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            // Get the currently logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // or User.Identity.GetUserId() if using old Identity
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            // Load dropdowns
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            ViewData["DepId"] = new SelectList(_context.departments, "DepartmentID", "DepqartmentName");
            ViewData["DesId"] = new SelectList(_context.designations, "Id", "DesignationName");

            // Company dropdown (only the user's company)
            var userCompany = await _context.companys
                .Where(c => c.CompanyId == user.CompanyId)
                .ToListAsync();
            ViewData["ComId"] = new SelectList(userCompany, "CompanyId", "CompanyName", user.CompanyId);

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Indexview()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userCompanyName = await _context.Users
                                     .Where(u => u.Id == UserId)
                                     .Select(u => u.company.CompanyName)
                                     .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userCompanyName))
            {
                return Forbid(); // Restrict access if Company Name is not found
            }

            var users = await _context.Users.Include(x => x.Role)
                .Include(y=>y.company)
                .Include(z=>z.designation)
                .Include(v=>v.department)
            .Where(v => userCompanyName == "JM BAXI GRP" || v.company.CompanyName == userCompanyName)
            .ToListAsync();


            return PartialView("_Indexview", (users));
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserModel model)
        {
            if (_context.Users.Any(x => x.UserName == model.UserName))
            {
                TempData["Error"] = "User already exists.";
                return RedirectToAction("Create"); // Redirect to display error alert
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                NationId = model.NationalId,
                NormalizedUserName = model.UserName.ToUpper(),
                Email = model.Email,
                EmailConfirmed = true,
                PhoneNumber = model.PhoneNumber,
                PhoneNumberConfirmed = true,
                RoleId = model.RoleId,
                CompanyId = model.CompanyId,
                DepartmentId = model.DepartmentId,
                DesignationId = model.DesignationId
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign role to user
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                if (role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }

                TempData["Success"] = "User created successfully!";
                return RedirectToAction("Create"); // Redirect to show alert
            }

            TempData["Error"] = "User creation failed. Please check the details.";
            return RedirectToAction("Create");
        }





        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Invalid user ID.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            UserModel model = new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                CompanyId = user.CompanyId,
                DepartmentId = user.DepartmentId,
                DesignationId = user.DesignationId
            };

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName", user.CompanyId);
            ViewData["DepId"] = new SelectList(_context.departments, "DepartmentID", "DepqartmentName", user.DepartmentId);
            ViewData["DesId"] = new SelectList(_context.designations, "Id", "DesignationName", user.DesignationId);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                TempData["Error"] = "Invalid user ID.";
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Update user details
            user.FirstName = model.FirstName;
            user.MiddleName = model.MiddleName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.RoleId = model.RoleId;
            user.CompanyId = model.CompanyId;
            user.DepartmentId = model.DepartmentId;
            user.DesignationId = model.DesignationId;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                TempData["Error"] = "User update failed.";
                return View(model);
            }

            // **Update User Role**
            var existingRoles = await _userManager.GetRolesAsync(user); // Get current roles
            var newRole = await _roleManager.FindByIdAsync(model.RoleId); // Get the new role name

            if (newRole != null)
            {
                // Remove the existing roles
                if (existingRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, existingRoles);
                }

                // Assign the new role
                await _userManager.AddToRoleAsync(user, newRole.Name);
            }

            // **Update Password if provided**
            if (!string.IsNullOrEmpty(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                if (!passwordChangeResult.Succeeded)
                {
                    TempData["Error"] = "Password update failed.";
                    return View(model);
                }
            }

            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("Index");
        }

    }
}