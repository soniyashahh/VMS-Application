using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VMSApplication.Data;
using VMSApplication.Models;
using VMSApplication.UserViewModel;

namespace VMSApplication.Controllers
{
    [Authorize(Roles = "Systemadmin,Superadmin")]
	public class RolesController : Controller
	{

		private readonly ApplicationDbContext _context;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		public RolesController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_context = context;
			_roleManager = roleManager;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<IActionResult>Index()
		{
			var roles=await _context.Roles.ToListAsync();
			return View(roles);
		}
        [HttpGet]
		public async Task<IActionResult> Create()
		{
			return View();
		}
		[HttpGet]
        public async Task<IActionResult> Indexview()
        {
            return PartialView("_Indexview",await _context.Roles.ToListAsync());
        }
        [HttpPost]
		public async Task<IActionResult> Create(RolesViewModel rolesmodel)
		{
			IdentityRole role = new IdentityRole();
			role.Name=rolesmodel.RoleName;
			var result=await _roleManager.CreateAsync(role);
			if (result.Succeeded)
			{
				return RedirectToAction("Index");
			}
			else 
			{
				return View(rolesmodel);
			}
		}

	}
}
