using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using VMSApplication.Data;
using VMSApplication.Models;

namespace VMSApplication.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; 
        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
        {
            _context= context;
            _logger = logger;
        }



        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("~/Identity/Account/Login");
            }

            // Get system codes for all status types in a single query
            var statusCodes = _context.systemCodes
                .Where(y => y.Name == "Approved" || y.Name == "Pending" || y.Name == "Rejected")
                .ToList();

            // Get the status IDs from the fetched system codes
            var approvedStatusId = statusCodes.FirstOrDefault(y => y.Name == "Approved")?.Id;
            var pendingStatusId = statusCodes.FirstOrDefault(y => y.Name == "Pending")?.Id;
            var rejectedStatusId = statusCodes.FirstOrDefault(y => y.Name == "Rejected")?.Id;

            // Fetch visitor registrations by status using the status IDs
            var applicationDbContext = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .Where(x => x.StatusId == approvedStatusId && x.userId==userId);

            var applicationDbContext2 = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .Where(x => x.StatusId == pendingStatusId && x.userId==userId);

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var totalInVisitorsToday = _context.securityForms
                .Where(y => y.DateTime >= today && y.DateTime < tomorrow && y.status == "IN")
                .Count();


            var applicationDbContext3 = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .Where(x => x.StatusId == rejectedStatusId && x.userId == userId);

            // Get the total count of companies
            var companiesCount = _context.companys.Count();

            // Get today's total visitor count
            var totalVisitorToday = _context.visitorsregistration
                .Where(x => x.FromTime == DateTime.Today)
                .Count();

            // Pass data to the view
            

            // Fetch the CompanyId and CompanyName for the current user
            var userCompany = _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.company)  // Assuming "Company" is a navigation property
                .FirstOrDefault();

            if (userCompany != null)
            {
                ViewData["UserCompanyName"] = userCompany.CompanyName;  // Passing Company Name
                ViewData["UserCompanyId"] = userCompany.CompanyId.ToString();  // Optionally pass Company ID
            }

            //ViewData["CompId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");

            if (user != null)
            {

                if (userCompany.CompanyName== "JM BAXI GRP")
                {
                    ViewData["CompId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
                }
                else { 
                var userCompanys = _context.companys
                    .Where(c => c.CompanyId == user.CompanyId) // Filter company based on user's CompId
                    .ToList();

                ViewData["CompId"] = new SelectList(userCompanys, "CompanyId", "CompanyName");
                }
            }

            // Passing the counts to ViewBag
            ViewBag.Count = _context.visitorsregistration.Count();
            ViewBag.Count2 = _context.applicationUsers.Count();
            ViewBag.Count3 = applicationDbContext.Count();
            ViewBag.Count4 = applicationDbContext2.Count(); 
            ViewBag.Count5 = totalVisitorToday;
            ViewBag.Count6 = applicationDbContext3.Count();
            ViewBag.Count7 = companiesCount;
            ViewBag.Count8 = totalInVisitorsToday;



            HttpContext.Session.SetString("Test", "Hello Session");
            var value = HttpContext.Session.GetString("Test"); // Safe here
            ViewBag.Message = value;

            // Use the authenticated check to return the view
            return !User.Identity.IsAuthenticated ? Redirect("~/Identity/Account/Login") : View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //Group Dashboard Chart Json Result

        [HttpGet]
        public IActionResult GetVisitorStatusChartData()
        {
            // Corrected Status IDs as per your DB
            int approvedStatusId = 4;
            int pendingStatusId = 3;
            int rejectedStatusId = 5;

            var approvedData = _context.visitorsregistration
                .Where(x => x.StatusId == approvedStatusId)
                .GroupBy(x => x.FromTime.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var pendingData = _context.visitorsregistration
                .Where(x => x.StatusId == pendingStatusId)
                .GroupBy(x => x.FromTime.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var rejectedData = _context.visitorsregistration
                .Where(x => x.StatusId == rejectedStatusId)
                .GroupBy(x => x.FromTime.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToList();

            var monthNames = Enumerable.Range(1, 12)
                .Select(i => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i))
                .ToList();

            int[] approvedCounts = new int[12];
            int[] pendingCounts = new int[12];
            int[] rejectedCounts = new int[12];

            foreach (var item in approvedData)
                approvedCounts[item.Month - 1] = item.Count;

            foreach (var item in pendingData)
                pendingCounts[item.Month - 1] = item.Count;

            foreach (var item in rejectedData)
                rejectedCounts[item.Month - 1] = item.Count;

            return Json(new
            {
                labels = monthNames,
                approved = approvedCounts,
                pending = pendingCounts,
                rejected = rejectedCounts
            });
        }


        //User Wise Dashboard Chart Json Result        

        [HttpGet]
    public IActionResult GetVisitorStatusChartData2()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user's ID

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(); // Just in case
        }

        int approvedStatusId = 4;
        int pendingStatusId = 3;
        int rejectedStatusId = 5;

        var approvedData = _context.visitorsregistration
            .Where(x => x.StatusId == approvedStatusId && x.userId == userId)
            .GroupBy(x => x.FromTime.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var pendingData = _context.visitorsregistration
            .Where(x => x.StatusId == pendingStatusId && x.userId == userId)
            .GroupBy(x => x.FromTime.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var rejectedData = _context.visitorsregistration
            .Where(x => x.StatusId == rejectedStatusId && x.userId == userId)
            .GroupBy(x => x.FromTime.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() })
            .ToList();

        var monthNames = Enumerable.Range(1, 12)
            .Select(i => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i))
            .ToList();

        int[] approvedCounts = new int[12];
        int[] pendingCounts = new int[12];
        int[] rejectedCounts = new int[12];

        foreach (var item in approvedData)
            approvedCounts[item.Month - 1] = item.Count;

        foreach (var item in pendingData)
            pendingCounts[item.Month - 1] = item.Count;

        foreach (var item in rejectedData)
            rejectedCounts[item.Month - 1] = item.Count;

        return Json(new
        {
            labels = monthNames,
            approved = approvedCounts,
            pending = pendingCounts,
            rejected = rejectedCounts
        });
    }


    [HttpGet]
        public JsonResult GetChartData()
        {
            var data = new
            {
                TotalVisitor = _context.visitorsregistration.Count(),
                ApprovedVisitor = _context.applicationUsers.Count(),
                ActiveUsers = _context.Users.Count(),
                TotalVisitorToday = _context.visitorsregistration
                .Where(x => x.FromTime == DateTime.Today)
                .Count(),
                CompaniesCount = _context.companys.Count(),
            };

            return Json(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult FilterDataByCompany(int companyId)
        {

           
            var rejectedStatus = _context.systemCodes.Where(y => y.Name == "Approved").FirstOrDefault();
            var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user).Where(x => x.StatusId == rejectedStatus.Id && x.CompanyId == companyId);


            var PendingStatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var applicationDbContext2 = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user).Where(x => x.StatusId == PendingStatus.Id && x.CompanyId == companyId);


            var totalvisitor = _context.visitorsregistration.Where(x => /*x.FromTime == DateTime.Today &&*/ x.CompanyId == companyId).Count();

            var totalUsers = _context.applicationUsers
                .Where(x => x.CompanyId == companyId)
                .Count();

            return Json(new
            {
                count2 = totalUsers,
                count3 = applicationDbContext.Count(),//Total Approval
                count4 = applicationDbContext2.Count(),
                count5 = totalvisitor
            });
        }

    }
}
