using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using VMSApplication.Data;
using VMSApplication.Hubs;
using VMSApplication.Models;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Immutable;



namespace VMSApplication.Controllers
{

    [Authorize]
    public class VisitorRegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<NotificationHub> _hubContext;

        public VisitorRegistrationsController(ApplicationDbContext context, IWebHostEnvironment webHost, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _webHostEnvironment = webHost;
            _hubContext = hubContext;
        }

        // GET: VisitorRegistrations
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user);
            //return View(await applicationDbContext.ToListAsync());
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user).Include(z => z.company).OrderByDescending(v => v.Id).Where(x => x.user.Id == CurrentUserId && x.StatusId == pendingStatus.Id);

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> IndividualPass()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get current user object from database to fetch CompanyId
            var currentUser = await _context.Users
                                            .Where(u => u.Id == currentUserId)
                                            .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                return Unauthorized();
            }

            var approvedStatus = await _context.systemCodes
                                               .FirstOrDefaultAsync(y => y.Name == "Approved");

            if (approvedStatus == null)
            {
                return NotFound("Approved status not found.");
            }

            // Filter visitorsregistration based on Status, CompanyId, and UserId
            var applicationDbContext = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .Where(x => x.StatusId == approvedStatus.Id &&
                            x.CompanyId == currentUser.CompanyId &&
                            x.userId == currentUserId) // This line is for filtering by user
                .OrderByDescending(v => v.Id);

            return View(await applicationDbContext.ToListAsync());
        }




        //[Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        //public async Task<IActionResult> IndividualPass()
        //{
        //    var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var approvedStatus = _context.systemCodes.Where(y => y.Name == "Approved").FirstOrDefault();
        //    var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user).OrderByDescending(v => v.Id).Where(x => x.StatusId == approvedStatus.Id);

        //    return View(await applicationDbContext.ToListAsync());
        //}

        [AllowAnonymous]
        public async Task<IActionResult> EmailApproval()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user).OrderByDescending(v => v.Id).Where(x => x.user.Id == CurrentUserId && x.StatusId == pendingStatus.Id);

            return View(await applicationDbContext.ToListAsync());
        }
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> IndexAll()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the logged-in user's Company Name
            var userCompanyName = await _context.Users
                                     .Where(u => u.Id == userId)
                                     .OrderByDescending(v => v.Id)
                                     .Select(u => u.company.CompanyName)
                                     .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userCompanyName))
            {
                return Forbid(); // Restrict access if Company Name is not found
            }

            // Fetch visitors based on company name
            var applicationDbContext = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .Where(v => userCompanyName == "JM BAXI GRP" || v.user.company.CompanyName == userCompanyName); // Show all records for JM BAXI GRP, else filter by Company Name

            return View(await applicationDbContext.ToListAsync());

            //var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).Include(v => v.user);
            //return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> IndexView()
        {
            var applicationDbContext = _context.visitorsregistration.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.VisitorType).OrderByDescending(v => v.Id).Include(v => v.user);
            return PartialView("_IndexView", await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> ApprovalView()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the logged-in user's Company Name
            var userCompanyName = await _context.Users
                                     .Where(u => u.Id == userId)
                                     .OrderByDescending(v => v.Id)
                                     .Select(u => u.company.CompanyName)
                                     .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userCompanyName))
            {
                return Forbid(); // Restrict access if Company Name is not found
            }


            // Retrieve Approved status
            var approvedStatus = await _context.systemCodes
                                     .Where(y => y.Name == "Approved")
                                     .Select(y => y.Id)
                                     .FirstOrDefaultAsync();

            if (approvedStatus == 0)
            {
                return NotFound("Approved status not found.");
            }

            // Fetch approved visitors based on company name
            var applicationDbContext = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .OrderByDescending(v => v.Id)
                .Where(v => v.StatusId == approvedStatus &&
                            (userCompanyName == "JM BAXI GRP" || v.user.company.CompanyName == userCompanyName));

            return View(await applicationDbContext.ToListAsync());
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


        [AllowAnonymous]

        [HttpGet]
        public IActionResult GetQuestionByCompany(int companyId)
        {
            var safetyVideo = _context.SafetyVideo
                .Where(sv => sv.CompanyId == companyId)
                .FirstOrDefault();

            if (safetyVideo != null)
            {
                // Return all 5 questions
                return Json(new
                {
                    success = true,
                    question1 = safetyVideo.Q1,
                    question2 = safetyVideo.Q2,
                    question3 = safetyVideo.Q3,
                    question4 = safetyVideo.Q4,
                    question5 = safetyVideo.Q5,
                    ans1 = safetyVideo.Q1Answer,
                    ans2 = safetyVideo.Q2Answer,
                    ans3 = safetyVideo.Q3Answer,
                    ans4 = safetyVideo.Q4Answer,
                    ans5 = safetyVideo.Q5Answer
                   

                });
            }

            return Json(new { success = false, message = "No question found for this company." });
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> RejectedView()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the logged-in user's Company Name
            var userCompanyName = await _context.Users
                                     .Where(u => u.Id == userId)
                                     .OrderByDescending(v => v.Id)
                                     .Select(u => u.company.CompanyName)
                                     .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userCompanyName))
            {
                return Forbid(); // Restrict access if Company Name is not found 
            }

            // Retrieve Rejected status
            var rejectedStatus = await _context.systemCodes
                                     .Where(y => y.Name == "Rejected")
                                     .Select(y => y.Id)
                                     .FirstOrDefaultAsync();

            if (rejectedStatus == 0)
            {
                return NotFound("Rejected status not found.");
            }

            // Fetch rejected visitors based on company name
            var applicationDbContext = _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .OrderByDescending(v => v.Id)
                .Where(v => v.StatusId == rejectedStatus &&
                            (userCompanyName == "JM BAXI GRP" || v.user.company.CompanyName == userCompanyName));

            return View(await applicationDbContext.ToListAsync());
        }


        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> PrintVisitorCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorRegistration = await _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visitorRegistration == null)
            {
                return NotFound();
            }

            return View(visitorRegistration);
        }

        // GET: VisitorRegistrations/Details/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorRegistration = await _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorRegistration == null)
            {
                return NotFound();
            }

            return View(visitorRegistration);
        }

        // GET: VisitorRegistrations/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            //ViewData["StatusId"] = new SelectList(_context.systemCodes, "Id", "Id");
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName");
            ViewData["VisitortypeId"] = new SelectList(_context.visitertypes, "Id", "Visitortype");
            //ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "FullName");
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult RRequest()
        {
            //ViewData["StatusId"] = new SelectList(_context.systemCodes, "Id", "Id");
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName");
            ViewData["VisitortypeId"] = new SelectList(_context.visitertypes, "Id", "Visitortype");
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "FirstName");
            return View();
        }

        // POST: VisitorRegistrations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        private string UploadFile(VisitorRegistration visitor)
        {

            string uniqFileName = null!;
            if (visitor.Aadharupload != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                uniqFileName = Guid.NewGuid().ToString() + "_" + visitor.Aadharupload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    visitor.Aadharupload.CopyTo(fileStream);
                }
            }
            return uniqFileName;
        }

        private string UploadFile2(VisitorRegistration visitor)
        {

            string uniqFileName2 = null!;
            if (visitor.VisitorImgFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                uniqFileName2 = Guid.NewGuid().ToString() + "_" + visitor.VisitorImgFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqFileName2);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    visitor.VisitorImgFile.CopyTo(fileStream);
                }
            }
            return uniqFileName2;
        }



        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitorRegistration visitorRegistration)
        {
            try 
            { 
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = _context.systemCodes.FirstOrDefault(y => y.Name == "Pending");
            var alreadyexistvisitor = await _context.visitorsregistration.AnyAsync(y => y.IDNumber == visitorRegistration.IDNumber);
            if (alreadyexistvisitor)
            {
                TempData["Error"] = "Visitor already Exist so please choose Re-Request Option";
            }
            else
            {
            if (!ModelState.IsValid)
            {
                   
                // Upload Aadhar and Visitor Image
                string aadhar = UploadFile(visitorRegistration);
                string visitorImg = UploadFile2(visitorRegistration);

                visitorRegistration.CreatedId = userId;
                visitorRegistration.createdOn = DateTime.Now;
                visitorRegistration.ModifiedId = userId;
                visitorRegistration.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);
                visitorRegistration.StatusId = pendingStatus.Id;
                visitorRegistration.UploadId = aadhar;
                visitorRegistration.VisitorImg = visitorImg;

                // Handle CHA-specific file upload
                if (visitorRegistration.VisitortypeId == 7 && visitorRegistration.CHADocument != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + visitorRegistration.CHADocument.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await visitorRegistration.CHADocument.CopyToAsync(fileStream);
                    }

                    visitorRegistration.CHAFilePath = uniqueFileName;
                }

                _context.Add(visitorRegistration);
                await _context.SaveChangesAsync();

                // 🔹 Send Notification to Only the Selected Host
                var hostUserId = visitorRegistration.userId; // Host User ID (who should receive the notification)

                var notification = new Notification
                {
                    UserId = hostUserId, // Send notification only to this user
                    Message = $"New Visitor Registration: {visitorRegistration.FullName}. Pending Approval.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.notifications.Add(notification);
                await _context.SaveChangesAsync();

                // 🔹 Send Real-time Notification via SignalR to Only the Selected Host
                var isNotificationSaved = await _context.notifications.AnyAsync(n => n.UserId == hostUserId && n.Message == notification.Message);
                if (isNotificationSaved)
                {
                    await _hubContext.Clients.User(hostUserId).SendAsync("ReceiveNotification", notification.Message);
                }
                else
                {
                    TempData["Error"] = "Notification not stored!";
                }

                // 🔹 Send Email to the Host
                try
                {
                    using (MailMessage mail = new MailMessage("vmsjmbaxi@ict.in", visitorRegistration.EmailId))
                    {

                        mail.Subject = $@"Registration:- {_context.companys.FirstOrDefault(y => y.CompanyId == visitorRegistration.CompanyId)?.CompanyName}VMS";

                        string content = $@"
                                <h5 style='margin-top: 0;'>Dear {visitorRegistration.FullName},</h5>
                                <h6>Thank you for registering as a visitor at {_context.companys.FirstOrDefault(y => y.CompanyId == visitorRegistration.CompanyId)?.CompanyName}.</h6>
                                <p>Your request has been successfully submitted and is currently pending approval from the host. Please be patient as we process your request.</p>";

                        mail.Body = content;
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient())
                        {
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;

                            // App Password is required for the application if 2FA is enabled
                            NetworkCredential creds = new NetworkCredential("vmsjmbaxi@ict.in", "hjso jmlp utxg wdvg");
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = creds;
                            smtp.Port = 587;
                            smtp.Send(mail);
                        }

                        //Host Mail Details inside mail massage first one is from email and second one is to email

                        using (MailMessage mail2 = new MailMessage("vmsjmbaxi@ict.in", visitorRegistration.HostEmail))
                        {

                            mail2.Subject = "VMS:- New Request For Your Approval";

                            mail2.Body = $@" <h4 style='margin-top: 0;'>Dear {_context.Users.FirstOrDefault(x => x.Id == visitorRegistration.userId)?.FullName},</h4><p>A new Registration has been submitted for your approval in the VMS.</p> 
                <p>Kindly review and take the necessary action at your earliest convenience. </p>
                <div style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f9;'>
                    <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1); overflow: hidden;'>
                        <!-- Header Section -->
                        <div style='background: linear-gradient(45deg, #007BFF, #0056b3); color: #ffffff; padding: 15px; text-align: center;'>
                            <h1 style='margin: 0; font-size: 24px; font-weight: bold;'>Visitor Registration Details</h1>
                        </div>
                        <!-- Visitor Details -->
                        <div style='padding: 20px;'>
                            <div style='background: #f9f9f9; padding: 15px; border-1radius: 5px; border: 1px solid #ddd;'>
                                <ul style='list-style-type: none; padding: 0; margin: 0;'>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Pass ID: </strong> {visitorRegistration.Id}</li> 
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Name: </strong> {visitorRegistration.FullName}</li>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Email: </strong> {visitorRegistration.EmailId}</li>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Phone Number: </strong> {visitorRegistration.ContactNumber}</li>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Adhar Number: </strong> {visitorRegistration.IDNumber}</li>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Driving License Number: </strong> {visitorRegistration.DrivingLicenceNo}</li>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Vehicle Reg Number: </strong> {visitorRegistration.VehicleRegistrationNo}</li>
                                    <li style='margin-bottom: 10px; color: #333;'><strong>Visit Purpose: </strong> {_context.visitorPurposes.FirstOrDefault(p => p.Id == visitorRegistration.visitorpurposeId)?.PurposeName}</li>
                                    <li style='margin-bottom: 10pxz; color: #333;'><strong>Visit Timing: </strong> {visitorRegistration.FromTime} - {visitorRegistration.ToTime}</li>

                </ul>
                            </div>
                        </div>
                        <!-- Action Buttons -->
                        <div style='text-align: center;'>
                       <a href='https://vms.jmbaxi.com/Identity/Account/Login' 
                       style='display: inline-block; background: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Check and Verify</a>

                </div>
                        <!-- Footer -->
                        <div style='background: #007BFF; color: #ffffff; padding: 10px; text-align: center;'>
                            <p style='margin: 0; font-size: 14px;'>Thank you for visiting {_context.companys.FirstOrDefault(y => y.CompanyId == visitorRegistration.CompanyId)?.CompanyName}</p>
                        </div>
                    </div> 
                </div>";
                            mail2.IsBodyHtml = true;

                            using (SmtpClient smtp2 = new SmtpClient())
                            {
                                smtp2.Host = "smtp.gmail.com";
                                smtp2.EnableSsl = true;

                                // App Password is required for the application if 2FA is enabled
                                NetworkCredential creds2 = new NetworkCredential("vmsjmbaxi@ict.in", "hjso jmlp utxg wdvg");
                                smtp2.UseDefaultCredentials = false;
                                smtp2.Credentials = creds2;
                                smtp2.Port = 587;
                                smtp2.Send(mail2);
                                TempData["Success"] = "Your Registration successful... Visitor-ID " + visitorRegistration.Id;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error sending email: " + ex.Message;
                }

                TempData["Success"] = "Visitor Registered Successfully... Visitor-ID " + visitorRegistration.Id;
                return RedirectToAction(nameof(Create));
            }
            }
            // Repopulate ViewData for dropdowns
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName", visitorRegistration.visitorpurposeId);
            ViewData["VisitortypeId"] = new SelectList(_context.visitertypes, "Id", "Id", visitorRegistration.VisitortypeId);
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "FullName", visitorRegistration.userId);
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
            }
            catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Create));
        }



        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorRegistration = await _context.visitorsregistration.FindAsync(id);
            if (visitorRegistration == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.systemCodes, "Id", "Id", visitorRegistration.StatusId);
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName", visitorRegistration.visitorpurposeId);
            ViewData["VisitortypeId"] = new SelectList(_context.visitertypes, "Id", "Id", visitorRegistration.VisitortypeId);
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "Id", visitorRegistration.userId);
            return View(visitorRegistration);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> NotifyUsers()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", "New visitor registered!");
            return Ok();
        }
        // POST: VisitorRegistrations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Gender,EmailId,ContactNumber,VisitorImg,FromTime,ToTime,VisitortypeId,visitorpurposeId,UploadId,IDNumber,ItemsDetails,VisitorLocation,VehicleRegistrationNo,DrivingLicenceNo,userId,StatusId,CreatedId,createdOn,ModifiedId,ModifiedOn")] VisitorRegistration visitorRegistration)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var PendingSatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            string aadhar = UploadFile(visitorRegistration);
            string visitorimg = UploadFile2(visitorRegistration);
            if (id != visitorRegistration.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    visitorRegistration.CreatedId = UserId;
                    visitorRegistration.createdOn = DateTime.Now;
                    visitorRegistration.ModifiedId = UserId;
                    visitorRegistration.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);

                    visitorRegistration.StatusId = PendingSatus.Id;

                    visitorRegistration.UploadId = aadhar;
                    visitorRegistration.VisitorImg = visitorimg;
                    _context.Update(visitorRegistration);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Visitor Details Updated: {visitorRegistration.FullName}");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorRegistrationExists(visitorRegistration.Id))
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
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName", visitorRegistration.visitorpurposeId);
            ViewData["VisitortypeId"] = new SelectList(_context.visitertypes, "Id", "Id", visitorRegistration.VisitortypeId);
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "Id", visitorRegistration.userId);
            return View(visitorRegistration);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Json(notifications);
        }

        [AllowAnonymous]
        public JsonResult GetUserIdsByCompany(int companyId)
        {
            var users = _context.Users.Where(u => u.CompanyId == companyId)
                                        .Select(u => new { u.Id, u.FullName })
                                        .ToList();

            return Json(users);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetUserDetails(string userId)
        {
            var user = _context.applicationUsers
                        .Where(u => u.Id == userId)
                        .Select(u => new
                        {
                            FirstName = u.FullName,
                            Email = u.Email,
                            PhoneNumber = u.PhoneNumber,
                            Department = u.department.DepqartmentName,
                            Designation = u.designation.DesignationName
                            /*u.LastName*/ // Add other fields if needed
                        })
                        .FirstOrDefault();
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            return Json(new { success = true, data = user });
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetCompanyDetails(int userId)
        {
            var user1 = _context.companys
                        .Where(u => u.CompanyId == userId)
                        .Select(u => new
                        {
                            Location = u.Location
                        })
                        .FirstOrDefault();

            if (user1 == null)
            {
                return Json(new { success = false, message = "User not found" });
            }
            return Json(new { success = true, data = user1 });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Approval(int id, VisitorRegistration visitorRegistration)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Fetch "Approved" status from the database
            var approvedStatus = await _context.systemCodes.FirstOrDefaultAsync(y => y.Name == "Approved");
            if (approvedStatus == null)
            {
                TempData["Error"] = "Approval status not found.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the visitor from the database
            var visitor = await _context.visitorsregistration.FirstOrDefaultAsync(m => m.Id == id);
            if (visitor == null)
            {
                TempData["Error"] = "Visitor not found.";
                return NotFound();
            }
            // Update the visitor's status
            visitor.StatusId = approvedStatus.Id;
            visitor.ModifiedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            visitor.ModifiedOn = DateOnly.MaxValue;

            _context.Update(visitor);
            await _context.SaveChangesAsync();
            var notification = new Notification
            {
                UserId = userId,
                Message = $"Visitor {visitorRegistration.FullName} Approved successfully.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Send Real-time Notification via SignalR
            var isNotificationSaved = await _context.notifications.AnyAsync(n => n.UserId == userId && n.Message == notification.Message);
            if (isNotificationSaved)
            {
                if (userId == "8216f84d-e96d-47f7-9fba-7e33dbd33093") // 🔹 Match correct user
                {
                    //Console.WriteLine($"🔔 Sending notification to user: {userId}");
                    await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notification.Message);
                }
                // Send Real-time Notification
                //await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification.Message);
            }
            else
            {
                TempData["Error"] = "Notification not stored!";
            }
            //TempData["Success"] = "Visitor Regsiter Sucessfully..";

            // Send email notification if the email ID is not null or empty
            if (!string.IsNullOrWhiteSpace(visitor.EmailId))
            {
                try
                {
                    using (MailMessage mail = new MailMessage("vmsjmbaxi@ict.in", visitor.EmailId))
                    {
                        // ✅ Add dynamic CC from MailConfig table
                        var ccList = _context.mailconfigs
                                             .Where(m => m.CompanyId == visitor.CompanyId)
                                             .Select(m => m.mailId)
                                             .ToList();

                        foreach (var cc in ccList)
                        {
                            if (!string.IsNullOrWhiteSpace(cc))
                                mail.CC.Add(cc);
                        }
                        mail.Subject = "VMS Approval Notification";

                        string content = $@" <p style='margin: 0; font-size: 16px;'>Dear {visitor.FullName},</p>
            <p style='font-size: 14px; color: #555;'>Your request has been approved by <b>{_context.Users.FirstOrDefault(x => x.Id == visitor.userId)?.FullName}</b>. <b><I>Please carry your Document as per the details mention for verification purpose</I></b>. Below are your registration details:</p>
<div style='font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f9;'>
    <div style='max-width: 600px; margin: auto; background: #ffffff; border-radius: 8px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); overflow: hidden;'>
        <div style='background: #007BFF; color: #ffffff; padding: 15px; text-align: center;'>
            <div style='background: #007BFF; color: #ffffff; padding: 15px; text-align: center; display: flex; align-items: center;'>
    <img src='~/AdminLTE/assets/img/kaiadmin/jmbaxilogo.png' alt='Logo' style='height: 40px; margin-right: 10px;'>
    <h1 style='margin: 0; font-size: 24px;'>Visitor Pass</h1>
</div>
        </div>
        <div style='padding: 20px;'>
            <div style='display: flex; align-items: flex-start;'>
                <div style='flex: 2; padding-right: 15px;'>
                    <ul style='list-style-type: none; padding: 0; margin: 0;'>
                        <li style='margin-bottom: 10px;'><strong>Pass ID:</strong> {visitor.Id}</li>
                        <li style='margin-bottom: 10px;'><strong>Name:</strong> {visitor.FullName}</li>
                        <li style='margin-bottom: 10px;'><strong>Email:</strong> {visitor.EmailId}</li>
                        <li style='margin-bottom: 10px;'><strong>Phone Number:</strong> {visitor.ContactNumber}</li>
                        <li style='margin-bottom: 10px;'><strong>Aadhar Number:</strong> {visitor.IDNumber}</li>
                        <li style='margin-bottom: 10px;'><strong>Driving License Number:</strong> {visitor.DrivingLicenceNo}</li>
                        <li style='margin-bottom: 10px;'><strong>Vehicle Reg Number:</strong> {visitor.VehicleRegistrationNo}</li>
                        <li style='margin-bottom: 10px;'><strong>Visit Purpose:</strong> {_context.visitorPurposes.FirstOrDefault(p => p.Id == visitor.visitorpurposeId)?.PurposeName}</li>
                        <li style='margin-bottom: 10px;'><strong>Visit Timing:</strong> {visitor.FromTime} - {visitor.ToTime}</li>
                        <li style='margin-bottom: 10pxz; color: #333;'><strong>Host Name:</strong> {_context.Users.FirstOrDefault(x => x.Id == visitor.userId)?.FullName}</li>
                        <li style='margin-bottom: 10pxz; color: #333;'><strong>Host Email:</strong> {_context.Users.FirstOrDefault(x => x.Id == visitor.userId)?.Email}</li>
                        
                 </ul>
                </div>
            </div>
        </div>
        <div style='background: #007BFF; color: #ffffff; padding: 10px; text-align: center;'>
            <p style='margin: 0; font-size: 14px;'>Thank you for visiting {_context.companys.FirstOrDefault(y => y.CompanyId == visitor.CompanyId)?.CompanyName}</p>
        </div>
    </div>
</div>";
                        mail.Body = content;

                        //    mail.Body = $@"
                        //<p>Dear {visitor.FullName},</p>
                        //<p>Thank you for registering as a visitor at KICT.Your registration has been approved.</p>
                        //<p>Best regards,<br>The KICT Team</p>";
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.EnableSsl = true;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential("vmsjmbaxi@ict.in", "hjso jmlp utxg wdvg");
                            smtp.Send(mail);
                        }
                    }

                    TempData["Success"] = "Visitor approved and email sent successfully.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Visitor approved but failed to send email: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "Visitor approved, but email ID is missing. Email not sent.";
            }

            return RedirectToAction(nameof(EmailApproval));
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Rejected(int id)
        {
            var rejectedStatus = await _context.systemCodes.FirstOrDefaultAsync(y => y.Name == "Rejected");
            if (rejectedStatus == null)
            {
                TempData["Error"] = "Approval status not found.";
                return RedirectToAction(nameof(Index));
            }

            var visitor = await _context.visitorsregistration.FirstOrDefaultAsync(m => m.Id == id);
            if (visitor == null) return NotFound();

            visitor.StatusId = rejectedStatus.Id;
            visitor.ModifiedId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //visitor.ModifiedOn = DateTime.Now;

            _context.Update(visitor);
            await _context.SaveChangesAsync();
            TempData["NotSuccess"] = "Visitor Rejected.";

            return RedirectToAction(nameof(Index));
        }


       


        // GET: VisitorRegistrations/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitorRegistration = await _context.visitorsregistration
                .Include(v => v.Status)
                .Include(v => v.VisitorPurpose)
                .Include(v => v.VisitorType)
                .Include(v => v.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitorRegistration == null)
            {
                return NotFound();
            }

            return View(visitorRegistration);
        }
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [Route("VisitorRegistration/Approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {

            var visitor = await _context.visitorsregistration.FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null || string.IsNullOrEmpty(visitor.HostEmail))
            {
                return BadRequest("Invalid visitor or host email is not set.");
            }

            var approvedStatus = _context.systemCodes.FirstOrDefault(s => s.Name == "Approved");
            if (approvedStatus != null)
            {
                visitor.StatusId = approvedStatus.Id;
                visitor.ModifiedOn = DateOnly.MinValue;
                _context.Update(visitor);
                await _context.SaveChangesAsync();

                // Send confirmation email to the visitor
                await SendConfirmationEmail(visitor.EmailId, "Your visit has been approved.");
                return Ok("Visitor approved successfully.");
            }
            return BadRequest("Approved status not found.");
        }
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [Route("VisitorRegistration/Reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var visitor = await _context.visitorsregistration.FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null || string.IsNullOrEmpty(visitor.HostEmail))
            {
                return BadRequest("Invalid visitor or host email is not set.");
            }

            var rejectedStatus = _context.systemCodes.FirstOrDefault(s => s.Name == "Rejected");
            if (rejectedStatus != null)
            {
                visitor.StatusId = rejectedStatus.Id;
                visitor.ModifiedOn = DateOnly.MinValue;
                _context.Update(visitor);
                await _context.SaveChangesAsync();

                // Send rejection email to the visitor
                await SendConfirmationEmail(visitor.EmailId, "Your visit has been rejected.");
                return Ok("Visitor rejected successfully.");
            }
            return BadRequest("Rejected status not found.");
        }

        private async Task SendConfirmationEmail(string toEmail, string message)
        {
            try
            {
                using (MailMessage mail = new MailMessage("vmsjmbaxi@ict.in", toEmail))
                {
                    mail.Subject = "Visitor Status Update";
                    mail.Body = message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("vmsjmbaxi@ict.in", "hjso jmlp utxg wdvg");
                        smtp.Port = 587;
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }


        // POST: VisitorRegistrations/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitorRegistration = await _context.visitorsregistration.FindAsync(id);
            if (visitorRegistration != null)
            {
                _context.visitorsregistration.Remove(visitorRegistration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorRegistrationExists(int id)
        {
            return _context.visitorsregistration.Any(e => e.Id == id);
        }
    }
}
