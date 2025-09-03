using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VMSApplication.Data;
using VMSApplication.Models;
using System.Security.Claims;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VMSApplication.Hubs;
using Microsoft.AspNetCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace VMSApplication.Controllers
{
    [Authorize]
    public class RRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<NotificationHub> _hubContext;

        public RRequestsController(ApplicationDbContext context, IWebHostEnvironment webHost, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _webHostEnvironment = webHost;
            _hubContext = hubContext;
        }

        // GET: RRequests
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Index()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var applicationDbContext = _context.requests.Include(r => r.Status).Include(r => r.VisitorPurpose).Include(r => r.company).Include(r => r.user).Include(r => r.visitor).Where(x=>x.StatusId == pendingStatus.Id && x.userId==CurrentUserId);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> RIndex()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var applicationDbContext = _context.requests.Include(r => r.Status).Include(r => r.VisitorPurpose).Include(r => r.company).Include(r => r.user).Include(r => r.visitor).Where(x => x.StatusId == pendingStatus.Id && x.userId == CurrentUserId);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> REmailApproval()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pendingStatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var applicationDbContext = _context.requests.Include(v => v.Status).Include(v => v.VisitorPurpose).Include(v => v.visitor).Include(v => v.user).Where(x => x.StatusId == pendingStatus.Id && x.userId==CurrentUserId);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RRequests/Details/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rRequest = await _context.requests
                .Include(r => r.Status)
                .Include(r => r.VisitorPurpose)
                .Include(r => r.company)
                .Include(r => r.user)
                .Include(r => r.visitor)
                .FirstOrDefaultAsync(m => m.id == id);
            if (rRequest == null)
            {
                return NotFound();
            }

            return View(rRequest);
        }


        [HttpGet]
        [AllowAnonymous]
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





        // GET: RRequests/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            //ViewData["StatusId"] = new SelectList(_context.systemCodes, "Id", "Id");
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName");
            ViewData["ComId"] = new SelectList(_context.companys, "CompanyId", "CompanyName");
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "FullName");
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id");
            return View();
        }

        // POST: RRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RRequest rRequest)
        {

           
            var PendingSatus = _context.systemCodes.Where(y => y.Name == "Pending").FirstOrDefault();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var visitor = await _context.visitorsregistration
             .FirstOrDefaultAsync(v => v.Id == rRequest.visitorId); // assuming FK is VisitorID

            if (visitor == null)
            {
                TempData["Error"] = "Invalid visitor details.";
                return View(rRequest);
            }

            var isBlacklisted = await _context.visitorBlacklists
                .Include(b => b.visitor)
                .AnyAsync(b => b.visitor != null && b.visitor.IDNumber == visitor.IDNumber);



            if (!ModelState.IsValid)
            {

                if (isBlacklisted)
                {
                    TempData["Error"] = "You are blacklisted.";
                    return View(rRequest);
                }
                else
                {
                    rRequest.StatusId = PendingSatus.Id;
                    _context.Add(rRequest);
                    System.Threading.Thread.Sleep(2000);
                    await _context.SaveChangesAsync();
                }

                var hostUserId = rRequest.userId;
                var notification = new Notification
                {
                    UserId = hostUserId,
                    Message = $"Visitor {rRequest.visitor} Visitor Registerd successfully.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send Real-time Notification via SignalR
                var isNotificationSaved = await _context.notifications.AnyAsync(n => n.UserId == hostUserId && n.Message == notification.Message);
                if (isNotificationSaved)
                {
                    // Send Real-time Notification
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification.Message);
                }
                else
                {
                    TempData["Error"] = "Notification not stored!";
                }
                //TempData["Success"] = "Visitor Regsiter Sucessfully..";


                try
                {
                    using (MailMessage mail = new MailMessage("vmsjmbaxi@ict.in", rRequest.VisitorEmail))
                    {

                        mail.Subject = $@"Registration:- {_context.companys.FirstOrDefault(y => y.CompanyId == rRequest.CompanyId)?.CompanyName}VMS";

                        string content = $@"
                <h2 style='margin-top: 0;'>Dear {_context.visitorsregistration.FirstOrDefault(p => p.Id == rRequest.visitorId)?.FullName},</h2>
                <h4>Thank you for Re-registering as a visitor at {_context.companys.FirstOrDefault(y => y.CompanyId == rRequest.CompanyId)?.CompanyName}.</h4>
                <p>Your Re-Request has been successfully submitted and is currently pending approval from the host. Please be patient as we process your request.</p>";

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

                        using (MailMessage mail2 = new MailMessage("vmsjmbaxi@ict.in", rRequest.HostEmail))
                        {

                            mail2.Subject = "VMS:- New Re-Request For Your Approval";

                            mail2.Body = $@"
           <h4 style='margin-top: 0;'>Dear {_context.Users.FirstOrDefault(x => x.Id == rRequest.userId)?.FullName},</h4><p>A new Re-request has been submitted for your approval in the VMS.</p> 
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
                    <li style='margin-bottom: 10px; color: #333;'><strong>Pass ID: </strong> {rRequest.visitorId} 
                    <li style='margin-bottom: 10px; color: #333;'><strong>Name: </strong>  {_context.visitorsregistration.FirstOrDefault(p => p.Id == rRequest.visitorId)?.FullName}</li>
                    <li style='margin-bottom: 10px; color: #333;'><strong>Email: </strong> {rRequest.visitor.EmailId}</li>
                    <li style='margin-bottom: 10px; color: #333;'><strong>Phone Number: </strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == rRequest.visitorId)?.ContactNumber}</li>
                    <li style='margin-bottom: 10px; color: #333;'><strong>Adhar Number: </strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == rRequest.visitorId)?.IDNumber}</</li>
                    <li style='margin-bottom: 10px; color: #333;'><strong>Driving License Number: </strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == rRequest.visitorId)?.DrivingLicenceNo}</</li>
                    <li style='margin-bottom: 10px; color: #333;'><strong>Vehicle Reg Number: </strong> {rRequest.visitor.VehicleRegistrationNo}</li>
                    <li style='margin-bottom: 10px; color: #333;'><strong>Visit Purpose: </strong> {_context.visitorPurposes.FirstOrDefault(p => p.Id == rRequest.visitorpurposeId)?.PurposeName}</li>
                    <li style='margin-bottom: 10pxz; color: #333;'><strong>Visit Timing: </strong> {rRequest.Fromdate} - {rRequest.Todate}</li>
                    
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
            <p style='margin: 0; font-size: 14px;'>Thank you for visiting {_context.companys.FirstOrDefault(y => y.CompanyId == rRequest.CompanyId)?.CompanyName}</p>
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
                                TempData["Success"] = "Your Re-registration successful... Visitor-ID "  +  rRequest.visitorId;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }

                return RedirectToAction(nameof(Create));
            }
         
           
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName", rRequest.visitorpurposeId);
            ViewData["CompanyId"] = new SelectList(_context.companys, "CompanyId", "CompanyId", rRequest.CompanyId);
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "FullName", rRequest.userId);
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id", rRequest.visitorId);
            
           
            return View(rRequest);
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rRequest = await _context.requests.FindAsync(id);
            if (rRequest == null)
            {
                return NotFound();
            }
            ViewData["StatusId"] = new SelectList(_context.systemCodes, "Id", "Id", rRequest.StatusId);
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName", rRequest.visitorpurposeId);
            ViewData["CompanyId"] = new SelectList(_context.companys, "CompanyId", "CompanyId", rRequest.CompanyId);
            ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "Id", rRequest.userId);
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id", rRequest.visitorId);
            return View(rRequest);
        }

        // POST: RRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Fromdate,Todate,visitorId,VehicleNo,ItemsDetails,userId,StatusId,visitorpurposeId,CompanyId")] RRequest rRequest)
        {
            if (id != rRequest.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RRequestExists(rRequest.id))
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
            ViewData["StatusId"] = new SelectList(_context.systemCodes, "Id", "Id", rRequest.StatusId);
            ViewData["visitorpurposeId"] = new SelectList(_context.visitorPurposes, "Id", "PurposeName", rRequest.visitorpurposeId);
            ViewData["CompanyId"] = new SelectList(_context.companys, "CompanyId", "CompanyId", rRequest.CompanyId);
            //ViewData["userId"] = new SelectList(_context.applicationUsers, "Id", "Id", rRequest.userId);
            ViewData["visitorId"] = new SelectList(_context.visitorsregistration, "Id", "Id", rRequest.visitorId);
            return View(rRequest);
        }
        [AllowAnonymous]
        [HttpGet]
        public JsonResult GetVisitorDetails(string Adhar)
        {
            var user = _context.visitorsregistration
                        .Where(u => u.IDNumber == Adhar)
                        .Select(u => new
                        {
                            FirstName = u.FullName,
                            Email = u.EmailId,
                            PhoneNumber = u.ContactNumber,
                            Location = u.VisitorLocation,
                            Gender = u.Gender,
                            visitorId = u.Id,
                            VisitortypeId = u.VisitorType.Visitortype,
                            DrivingLicense = u.DrivingLicenceNo,
                            Adharimage = u.UploadId,
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


        // GET: RRequests/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rRequest = await _context.requests
                .Include(r => r.Status)
                .Include(r => r.VisitorPurpose)
                .Include(r => r.company)
                .Include(r => r.user)
                .Include(r => r.visitor)
                .FirstOrDefaultAsync(m => m.id == id);
            if (rRequest == null)
            {
                return NotFound();
            }

            return View(rRequest);
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
        [HttpPost]
        public async Task<IActionResult> Approval(int id, RRequest rRequest)
        {
            // Fetch "Approved" status from the database
            var approvedStatus = await _context.systemCodes.FirstOrDefaultAsync(y => y.Name == "Approved");
            if (approvedStatus == null)
            { 
                TempData["Error"] = "Approval status not found.";
                return RedirectToAction(nameof(Index));
            }

            // Retrieve the visitor from the database
            var visitor = await _context.requests.FirstOrDefaultAsync(m => m.id == id);
            if (visitor == null)
            {
                TempData["Error"] = "Visitor not found.";
                return NotFound();
            }
            // Update the visitor's status
            visitor.StatusId = approvedStatus.Id;


            _context.Update(visitor);
            await _context.SaveChangesAsync();

            var visitorDetails = await _context.visitorsregistration.FirstOrDefaultAsync(p => p.Id == visitor.visitorId);
            if (visitorDetails == null)
            {
                TempData["Error"] = "Visitor details not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get visitor email from database if rRequest.VisitorEmail is null
            string visitorEmail = !string.IsNullOrWhiteSpace(rRequest.VisitorEmail) ?
                                    rRequest.VisitorEmail :
                                    visitorDetails.EmailId;


            // Send email notification if the email ID is not null or empty
            if (!string.IsNullOrWhiteSpace(visitorEmail))
            {
                try
                {
                    using (MailMessage mail = new MailMessage("vmsjmbaxi@ict.in", visitorEmail))
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

                        string content = $@" <p style='margin: 0; font-size: 16px;'>Dear { _context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.FullName},</p>
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
                        <li style='margin-bottom: 10px;'><strong>Pass ID:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.Id} </li>
                        <li style='margin-bottom: 10px;'><strong>Name:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.FullName}</li>
                        <li style='margin-bottom: 10px;'><strong>Email:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.EmailId}</li>
                        <li style='margin-bottom: 10px;'><strong>Phone Number:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.ContactNumber}</li>
                        <li style='margin-bottom: 10px;'><strong>Aadhar Number:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.IDNumber}</li>
                        <li style='margin-bottom: 10px;'><strong>Driving License Number:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == rRequest.visitorId)?.DrivingLicenceNo}</li>
                        <li style='margin-bottom: 10px;'><strong>Vehicle Reg Number:</strong> {_context.visitorsregistration.FirstOrDefault(p => p.Id == visitor.visitorId)?.VehicleRegistrationNo}</li>
                        <li style='margin-bottom: 10px;'><strong>Visit Purpose:</strong> {_context.visitorPurposes.FirstOrDefault(p => p.Id == visitor.visitorpurposeId)?.PurposeName}</li>
                        <li style='margin-bottom: 10px;'><strong>Visit Timing:</strong> {rRequest.Fromdate} - {rRequest.Todate}</li>
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
                        //<p>Thank you for registering as a visitor at KICT. Your registration has been approved.</p>
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
            return RedirectToAction(nameof(REmailApproval));
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
        [AllowAnonymous]
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

        // POST: RRequests/Delete/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rRequest = await _context.requests.FindAsync(id);
            if (rRequest != null)
            {
                _context.requests.Remove(rRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RRequestExists(int id)
        {
            return _context.requests.Any(e => e.id == id);
        }
    }
}
