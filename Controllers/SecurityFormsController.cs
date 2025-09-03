using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VMSApplication.Data;
using VMSApplication.Models;
using System.Security.Claims;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.AspNetCore.SignalR;
using VMSApplication.Hubs;
using NuGet.Protocol;

namespace VMSApplication.Controllers
{
    [Authorize]
    public class SecurityFormsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHubContext<NotificationHub> _hubContext;
        public SecurityFormsController(ApplicationDbContext context, IWebHostEnvironment webHost, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _webHostEnvironment = webHost;
            _hubContext = hubContext;

        }

        //// GET: SecurityForms
        //[Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        //public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        //{

        //    var query = _context.securityForms.AsQueryable();

        //    // Apply date filtering if both fromDate and toDate are provided
        //    if (fromDate.HasValue && toDate.HasValue)
        //    {
        //        query = query.Where(s => s.DateTime >= fromDate.Value && s.DateTime <= toDate.Value);
        //    }
        //    // Fetch the filtered data
        //    var filteredData = await query
        //                            .Include(s => s.Visitor)
        //                            .OrderByDescending(v => v.Id)
        //                            .ToListAsync();

        //    return View(filteredData);
        //}

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                // Step 1: Get logged-in user's ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Step 2: Fetch user's CompanyId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return View(new List<SecurityForm>());
                }

                // Step 3: Query SecurityForms
                var query = _context.securityForms
                    .Include(s => s.Visitor)
                    .Where(s => user.CompanyId == 5 || s.Visitor.CompanyId == user.CompanyId)
                    .AsQueryable();

                // Step 4: Apply date filtering if provided
                if (fromDate.HasValue && toDate.HasValue)
                {
                    query = query.Where(s => s.DateTime >= fromDate.Value && s.DateTime <= toDate.Value);
                }

                var filteredData = await query
                    .OrderByDescending(s => s.Id)
                    .ToListAsync();

                return View(filteredData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<SecurityForm>());
            }
        }


        //[Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        //public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        //{
        //    try
        //    {
        //        // Step 1: Get logged-in user's ID from claims
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //        // Step 2: Fetch user's CompanyId
        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //        if (user == null)
        //        {
        //            TempData["Error"] = "User not found.";
        //            return View(new List<SecurityForm>());
        //        }

        //        var userCompanyId = user.CompanyId;

        //        // Step 3: Query SecurityForms with visitor from same CompanyId
        //        var query = _context.securityForms
        //            .Include(s => s.Visitor)
        //            .Where(s => s.Visitor.CompanyId == userCompanyId)
        //            .AsQueryable();

        //        // Step 4: Apply date filtering if provided
        //        if (fromDate.HasValue && toDate.HasValue)
        //        {
        //            query = query.Where(s => s.DateTime >= fromDate.Value && s.DateTime <= toDate.Value);
        //        }

        //        var filteredData = await query
        //            .OrderByDescending(s => s.Id)
        //            .ToListAsync();

        //        return View(filteredData);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = ex.Message;
        //        return View(new List<SecurityForm>());
        //    }
        //}


        //[Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        //[HttpGet]
        //public async Task<IActionResult> IndexView()
        //{
        //    var today = DateTime.Today;
        //    var applicationDbContext = _context.securityForms.Where(x => x.DateTime.Date == today).OrderByDescending(v => v.Id).Include(s => s.Visitor);
        //    return PartialView("_IndexView", await applicationDbContext.ToListAsync());
        //}

        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpGet]
        public async Task<IActionResult> IndexView()
        {
            try
            {
                var today = DateTime.Today;

                // Step 1: Get logged-in user's ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Step 2: Fetch user's CompanyId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return PartialView("_IndexView", new List<SecurityForm>());
                }

                var userCompanyId = user.CompanyId;

                // Step 3: Get today's SecurityForms with matching Visitor's CompanyId
                var applicationDbContext = await _context.securityForms
                    .Include(s => s.Visitor)
                    .Where(s => s.DateTime.Date == today && s.Visitor.CompanyId == userCompanyId)
                    .OrderByDescending(v => v.Id)
                    .ToListAsync();

                return PartialView("_IndexView", applicationDbContext);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return PartialView("_IndexView", new List<SecurityForm>());
            }
        }



        // GET: SecurityForms/Details/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var securityForm = await _context.securityForms
                .Include(s => s.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (securityForm == null)
            {
                return NotFound();
            }

            return View(securityForm);
        }

        // GET: SecurityForms/Create
        [Authorize(Roles = "Systemadmin,Superadmin,Security,Admin")]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Step 1: Get logged-in user's ID from claims
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Step 2: Get user's CompanyId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    ViewData["VisitorID"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    return View();
                }

                var userCompanyId = user.CompanyId;

                // Step 3: Get visitors whose CompanyId matches user's CompanyId
                var visitors = await _context.visitorsregistration
                    .Where(v => v.CompanyId == userCompanyId)
                    .ToListAsync();

                ViewData["VisitorID"] = new SelectList(visitors, "Id", "Id");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                ViewData["VisitorID"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View();
        }


        [Authorize(Roles = "Systemadmin,Superadmin,Security,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkIn(SecurityForm securityForm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get visitor registration
            var visitor = await _context.visitorsregistration
                .Include(v => v.user)
                .FirstOrDefaultAsync(v => v.Id == securityForm.VisitorID);

            if (visitor == null)
            {
                TempData["Error"] = "Visitor not found.";
                return RedirectToAction(nameof(Create));
            }

            // Get system status IDs
            var approvedStatusId = await _context.systemCodes
                .Where(x => x.Name == "Approved")
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var pendingStatusId = await _context.systemCodes
                .Where(x => x.Name == "Pending")
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var rejectedStatusId = await _context.systemCodes
                .Where(x => x.Name == "Rejected")
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            // Step 1: Check VisitorRegistration Status
            if (visitor.StatusId == pendingStatusId)
            {
                TempData["Error"] = "Visitor request is still pending.";
                return RedirectToAction(nameof(Create));
            }

            if (visitor.StatusId == rejectedStatusId)
            {
                TempData["Error"] = "Visitor request was rejected.";
                return RedirectToAction(nameof(Create));
            }

            // Step 2: Check any related Re-Request record (latest)
            var reRequest = await _context.requests
                .Where(r => r.visitorId == visitor.Id)
                .OrderByDescending(r => r.visitorId)
                .FirstOrDefaultAsync();

            if (reRequest != null)
            {
                if (reRequest.StatusId == pendingStatusId)
                {
                    TempData["Error"] = "Re-Request is still pending. Cannot mark IN.";
                    return RedirectToAction(nameof(Create));
                }

                if (reRequest.StatusId == rejectedStatusId)
                {
                    TempData["Error"] = "Re-Request was rejected. Cannot mark IN.";
                    return RedirectToAction(nameof(Create));
                }

                if (reRequest.StatusId == approvedStatusId && reRequest.Todate < DateTime.Now)
                {
                    TempData["Error"] = "Re-Request has expired. Cannot mark IN.";
                    return RedirectToAction(nameof(Create));
                }
            }
            else
            {
                // Step 3: If no Re-Request exists, check VisitorRegistration ToTime
                if (visitor.ToTime < DateTime.Now)
                {
                    TempData["Error"] = "Visitor gate pass expired. Kindly Re-Request Again.";
                    return RedirectToAction(nameof(Create));
                }
            }

            // Step 4: Check if already marked IN
            var lastStatus = await _context.securityForms
                .Where(s => s.VisitorID == visitor.Id)
                .OrderByDescending(s => s.DateTime)
                .Select(s => s.status)
                .FirstOrDefaultAsync();

            if (lastStatus == "IN")
            {
                TempData["Error"] = "Visitor already marked IN. Please mark OUT first.";
                return RedirectToAction(nameof(Create));
            }

            // Step 5: Check blacklist
            bool isBlacklisted = await _context.visitorBlacklists
                .AnyAsync(b => b.visitorId == visitor.Id);

            if (isBlacklisted)
            {
                TempData["Error"] = "This visitor is blacklisted.";
                return RedirectToAction(nameof(Create));
            }

            // Step 6: Mark IN
            securityForm.status = "IN";
            securityForm.DateTime = DateTime.Now;
            securityForm.CreatedId = userId;
            securityForm.createdOn = DateTime.Now;
            securityForm.ModifiedId = userId;
            securityForm.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);

            _context.Add(securityForm);
            await _context.SaveChangesAsync();

            // Step 7: Notifications
            string securityMsg = $"Visitor {securityForm.visitorName} marked IN successfully.";
            await AddNotificationAsync(userId, securityMsg);
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", securityMsg);

            var hostUserId = visitor.user?.Id;
            if (!string.IsNullOrEmpty(hostUserId) && hostUserId != userId)
            {
                string hostMsg = $"Your visitor {securityForm.visitorName} has marked IN.";
                await AddNotificationAsync(hostUserId, hostMsg);
                await _hubContext.Clients.User(hostUserId).SendAsync("ReceiveNotification", hostMsg);
            }

            try
            {
                var hostEmail = visitor.user?.Email;
                if (!string.IsNullOrEmpty(hostEmail))
                {
                    SendEmailToHost(hostEmail, securityForm.visitorName, visitor.Id);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to send email: " + ex.Message;
            }

            TempData["Success"] = "Visitor marked IN successfully!";
            return RedirectToAction(nameof(Create));
        }


        //[Authorize(Roles = "Systemadmin,Superadmin,Security")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MarkIn(SecurityForm securityForm)
        //{
        //    var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        //    //var isVisitorExpired = _context.visitorsregistration
        //    //.Any(x => x.ToTime < DateTime.Now);
        //    //var isRerequestExpired = _context.requests.Any(x => x.Todate < DateTime.Now);
        //    //if (isVisitorExpired && isRerequestExpired)
        //    //{
        //    //    TempData["Error"] = "Visitor Gate pass has been Expire Kindly Re-Request Again.";
        //    //    return RedirectToAction(nameof(Create));
        //    //}

        //    // Fetch the current visitor by ID
        //    var currentVisitor = await _context.visitorsregistration
        //        .FirstOrDefaultAsync(x => x.Id == securityForm.VisitorID);

        //    if (currentVisitor == null)
        //    {
        //        TempData["Error"] = "Visitor not found.";
        //        return RedirectToAction(nameof(Create));
        //    }

        //    // Check if visitor's ToTime is expired
        //    bool isVisitorExpired = currentVisitor.ToTime < DateTime.Now;

        //    // Fetch the related request if applicable
        //    var currentRequest = await _context.requests
        //        .FirstOrDefaultAsync(x => x.visitorId == securityForm.VisitorID); // Adjust if your relation is different

        //    bool isRequestExpired = currentRequest != null && currentRequest.Todate < DateTime.Now;

        //    if (isVisitorExpired || isRequestExpired)
        //    {
        //        TempData["Error"] = "Visitor Gate pass has expired. Kindly Re-Request Again.";
        //        return RedirectToAction(nameof(Create));
        //    }

        //    // Check if the visitor is blacklisted
        //    var isBlacklisted = await _context.visitorBlacklists
        //        .AnyAsync(blacklist => blacklist.visitorId == securityForm.VisitorID);

        //    if (isBlacklisted)
        //    {
        //        TempData["Error"] = "This visitor is blacklisted and cannot Mark IN.";
        //        return RedirectToAction(nameof(Create));
        //    }

        //    // Get "Approved" status ID
        //    var approvedStatusId = await _context.systemCodes
        //        .Where(sc => sc.Name == "Approved")
        //        .Select(sc => sc.Id)
        //        .FirstOrDefaultAsync();

        //    // Check if the visitor is approved
        //    var isApproved = await _context.visitorsregistration
        //        .AnyAsync(v => v.Id == securityForm.VisitorID && v.StatusId == approvedStatusId);

        //    if (!isApproved)
        //    {
        //        TempData["Error"] = "This visitor is not approved and cannot Mark IN.";
        //        return RedirectToAction(nameof(Create));
        //    }

        //    // Check last status
        //    var lastStatus = await _context.securityForms
        //        .Where(sf => sf.VisitorID == securityForm.VisitorID)
        //        .OrderByDescending(sf => sf.DateTime)
        //        .Select(sf => sf.status)
        //        .FirstOrDefaultAsync();

        //    if (lastStatus == "IN")
        //    {
        //        TempData["Error"] = "This visitor is already marked IN and must Mark OUT first.";
        //        return RedirectToAction(nameof(Create));
        //    }

        //    // Mark visitor IN
        //    securityForm.status = "IN";
        //    securityForm.DateTime = DateTime.Now;
        //    securityForm.CreatedId = UserId;
        //    securityForm.createdOn = DateTime.Now;
        //    securityForm.ModifiedId = UserId;
        //    securityForm.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);

        //    _context.Add(securityForm);
        //    await _context.SaveChangesAsync();

        //    // Notify Security User
        //    string securityMessage = $"Visitor {securityForm.visitorName} marked IN successfully.";
        //    await AddNotificationAsync(UserId, securityMessage);
        //    await _hubContext.Clients.User(UserId).SendAsync("ReceiveNotification", securityMessage);

        //    // Notify Host (if assigned)
        //    var hostUserId = await _context.visitorsregistration
        //        .Where(v => v.Id == securityForm.VisitorID)
        //        .Select(v => v.user.Id)
        //        .FirstOrDefaultAsync();

        //    if (!string.IsNullOrEmpty(hostUserId) && hostUserId != UserId)
        //    {
        //        string hostMessage = $"Your visitor {securityForm.visitorName} has marked IN.";
        //        await AddNotificationAsync(hostUserId, hostMessage);
        //        await _hubContext.Clients.User(hostUserId).SendAsync("ReceiveNotification", hostMessage);
        //    }

        //    try
        //    {
        //        // Retrieve Host Email
        //        var hostEmail = await _context.visitorsregistration
        //            .Where(y => y.Id == securityForm.VisitorID)
        //            .Select(y => y.user.Email)
        //            .FirstOrDefaultAsync();

        //        if (!string.IsNullOrEmpty(hostEmail))
        //        {
        //            SendEmailToHost(hostEmail, securityForm.visitorName, securityForm.VisitorID);
        //        }
        //        else
        //        {
        //            TempData["Error"] = "Host email not found!";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Error sending email: " + ex.Message;
        //    }

        //    TempData["Success"] = "Visitor Marked IN Successfully!";
        //    return RedirectToAction(nameof(Create));
        //}

        private async Task AddNotificationAsync(string userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            // Check if the notification already exists
            var isNotificationSaved = await _context.notifications
                .AnyAsync(n => n.UserId == userId && n.Message == message);

            if (!isNotificationSaved)
            {
                _context.notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }




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



        //[HttpGet]
        //public JsonResult GetVisitorDetails(int visitorId)
        //{
        //    var visitor = _context.visitorsregistration
        //                          .Where(v => v.Id == visitorId)
        //                          .Select(v => new
        //                          {
        //                              v.Id,
        //                              v.FullName,
        //                              v.company.CompanyName,
        //                              v.Gender,
        //                              v.user,
        //                              v.EmailId,
        //                              v.VisitorPurpose.PurposeName,
        //                              v.VisitorType.Visitortype,
        //                              v.CHACompany,
        //                              v.CHALicense,
        //                              v.CHAFilePath,
        //                              v.ContactNumber,
        //                              v.FromTime,
        //                              v.ToTime,
        //                              v.IDNumber,
        //                              v.VehicleRegistrationNo,
        //                              v.DrivingLicenceNo,
        //                              v.ItemsDetails,
        //                              v.UploadId,
        //                              v.VisitorImg,
        //                              v.Status
        //                          })
        //                          .FirstOrDefault();

        //    if (visitor == null)
        //    {
        //        return Json(new { success = false, message = "Visitor not found!" });
        //    }

        //    return Json(new { success = true, data = visitor });
        //}

        [HttpGet]
        public JsonResult GetVisitorDetails(int visitorId)
        {
            var visitor = _context.visitorsregistration
                                  .Include(v => v.company)
                                  .Include(v => v.user)
                                  .Include(v => v.VisitorPurpose)
                                  .Include(v => v.VisitorType)
                                  .Include(v => v.Status)
                                  .Where(v => v.Id == visitorId)
                                  .Select(v => new
                                  {
                                      v.Id,
                                      v.FullName,
                                      v.company.CompanyName,
                                      v.Gender,
                                      user = new
                                      {
                                          v.user.FirstName,
                                          v.user.LastName
                                      },
                                      v.EmailId,
                                      PurposeName = v.VisitorPurpose.PurposeName,
                                      Visitortype = v.VisitorType.Visitortype,
                                      v.CHACompany,
                                      v.CHALicense,
                                      v.CHAFilePath,
                                      v.ContactNumber,
                                      v.FromTime,
                                      v.ToTime,
                                      v.IDNumber,
                                      v.VehicleRegistrationNo,
                                      v.DrivingLicenceNo,
                                      v.ItemsDetails,
                                      v.UploadId,
                                      v.VisitorImg,
                                      status = new
                                      {
                                          v.Status.Name
                                      }
                                  })
                                  .FirstOrDefault();

            if (visitor == null)
            {
                return Json(new { success = false, message = "Visitor not found!" });
            }

            // Check if original visit is expired
            if (visitor.ToTime < DateTime.Now)
            {
                // Look for non-expired re-request based on OriginalVisitorId
                var reRequest = _context.requests
                    .Include(r => r.company)
                    .Include(r => r.user)
                    .Include(r => r.VisitorPurpose)
                    .Include(r => r.visitor.VisitorType)
                    .Include(r => r.Status)
                    .Where(r => r.visitorId == visitorId && r.Todate >= DateTime.Now)
                    .OrderByDescending(r => r.Todate)
                    .Select(r => new
                    {
                        r.id,
                        r.visitor.FullName,
                        r.company.CompanyName,
                        r.visitor.Gender,
                        user = new
                        {
                            r.user.FirstName,
                            r.user.LastName
                        },
                        r.visitor.EmailId,
                        PurposeName = r.VisitorPurpose.PurposeName,
                        Visitortype = r.visitor.VisitorType.Visitortype,
                        r.visitor.CHACompany,
                        r.visitor.CHALicense,
                        r.visitor.CHAFilePath,
                        r.visitor.ContactNumber,
                        r.Fromdate,
                        r.Todate,
                        r.visitor.IDNumber,
                        r.VehicleNo,
                        r.visitor.DrivingLicenceNo,
                        r.ItemsDetails,
                        r.visitor.UploadId,
                        r.visitor.VisitorImg,
                        status = new
                        {
                            r.Status.Name
                        }
                    })
                    .FirstOrDefault();

                if (reRequest != null)
                {
                    return Json(new { success = true, data = reRequest, message = "Showing re-request details." });
                }
                else
                {
                    return Json(new { success = false, message = "Visitor registration expired. No valid re-request found." });
                }
            }

            // Visitor not expired
            return Json(new { success = true, data = visitor, message = "Showing original visitor details." });
        }



        [AllowAnonymous]
        private void SendEmailToHost(string hostEmail, string visitorName, int visitorId)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("vmsjmbaxi@ict.in");
                    mail.To.Add(hostEmail);
                    mail.Subject = "Visitor Marked IN - Notification";

                    string content = $@"
    <h4>Dear,{_context.Users.FirstOrDefault(x => x.Email == hostEmail)?.FullName}</h4>
    <p>We would like to inform you that <strong></strong> has successfully checked in at <strong>KICT</strong>.</p>
    <p>Once the meeting is completed, please confirm to Security by clicking the Confirm button.</p>
    <br/>
    <p><strong>Visitor Details:</strong></p>
    <ul>
        <li><strong>Name:</strong> {_context.visitorsregistration.FirstOrDefault(x => x.Id == visitorId)?.FullName}</li>
        <li><strong>Check-in Time:</strong> {DateTime.Now.ToString("f")}</li>  
    </ul>
    <br/>
    <p>
       <a href='https://vms.jmbaxi.com/SecurityForms/ConfirmMeeting?visitorId={visitorId}'
       style='display: inline-block; padding: 10px 20px; color: #fff; background-color: #28a745;
              text-decoration: none; border-radius: 5px;'>Confirm Meeting Completed</a>
    </p>
    <p>If you have any questions or require assistance, please contact the security team.</p>
    <br/>
    <p>Best Regards,<br/>VMS Team</p>";

                 

                    mail.Body = content;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("vmsjmbaxi@ict.in", "hjso jmlp utxg wdvg"); // Update with correct credentials
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email sending failed: " + ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ConfirmMeeting(int visitorId)
        {
            try
            {
                if (visitorId == 0)
                {
                    Console.WriteLine("Visitor ID is missing or invalid.");
                    return BadRequest("Invalid visitor ID.");
                }

                var visitor = _context.visitorsregistration.FirstOrDefault(v => v.Id == visitorId);
                if (visitor == null)
                {
                    Console.WriteLine($"Visitor not found for ID: {visitorId}");
                    return NotFound("Visitor not found.");
                }

                // Send confirmation email to the security team
                SendEmailToSecurityAsync(visitor);

                return Content("<h2>Meeting Confirmation Successful</h2><p>The security team has been notified.</p>", "text/html");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ConfirmMeeting: " + ex.Message);
                return StatusCode(500, "Error confirming the meeting.");
            }
        }

        [AllowAnonymous]
        private async Task SendEmailToSecurityAsync(VisitorRegistration visitor)
        {
            try
            {
                
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("vmsjmbaxi@ict.in");

                    // ✅ Add dynamic CC from MailConfig table
                    var ccList = _context.mailconfigs
                                         .Where(m => m.CompanyId == visitor.CompanyId)
                                         .Select(m => m.mailId)
                                         .ToList();

                    foreach (var cc in ccList)
                    {
                        if (!string.IsNullOrWhiteSpace(cc))
                            mail.To.Add(cc);
                    }

                    mail.Subject = "Meeting Completed - Visitor Left Premises";

                    string content = $@"
<h4>Dear Security Team,</h4>
<p>The meeting with <strong>{visitor.FullName}</strong> has been completed.</p>
<p>The visitor has left the premises.</p> 
<br/> 
<p><strong>Details:</strong></p>
<ul>
    <li><strong>Name:</strong> {visitor.FullName}</li>
    <li><strong>Check-out Time:</strong> {DateTime.Now.ToString("f")}</li>
</ul>
<br/>
<p>Thank you for ensuring the security of our premises.</p>
<br/>
<p>Best Regards,<br/>VMS Team</p>";

                    mail.Body = content;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("vmsjmbaxi@ict.in", "hjso jmlp utxg wdvg");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Security Email sending failed: " + ex.Message);
            }
        }


        [Authorize(Roles = "Systemadmin,Superadmin,Security,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkOut(SecurityForm securityForm)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the visitor is blacklisted
            var isBlacklisted = await _context.visitorBlacklists
                .AnyAsync(blacklist => blacklist.visitorId == securityForm.VisitorID);

            if (isBlacklisted)
            {
                TempData["Error"] = "This visitor is blacklisted.";
                return RedirectToAction(nameof(Create));
            }

            // Ensure the visitor has an "IN" status before allowing "OUT"
            var lastStatus = await _context.securityForms
                .Where(sf => sf.VisitorID == securityForm.VisitorID)
                .OrderByDescending(sf => sf.DateTime)
                .Select(sf => sf.status)
                .FirstOrDefaultAsync();

            if (lastStatus != "IN")
            {
                TempData["Error"] = "The visitor cannot Mark OUT without being Marked IN.";
                return RedirectToAction(nameof(Create));
            }

            // Mark visitor OUT
            securityForm.status = "OUT";
            securityForm.DateTime = DateTime.Now;
            securityForm.CreatedId = UserId;
            securityForm.createdOn = DateTime.Now;
            securityForm.ModifiedId = UserId;
            securityForm.ModifiedOn = DateOnly.FromDateTime(DateTime.Now);

            _context.Add(securityForm);
            await _context.SaveChangesAsync();

            // Notify Security User
            string securityMessage = $"Visitor {securityForm.visitorName} marked OUT successfully.";
            await AddNotificationAsync(UserId, securityMessage);
            await _hubContext.Clients.User(UserId).SendAsync("ReceiveNotification", securityMessage);

            // Notify Host (if assigned)
            var hostUserId = await _context.visitorsregistration
                .Where(v => v.Id == securityForm.VisitorID)
                .Select(v => v.user.Id)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(hostUserId) && hostUserId != UserId)
            {
                string hostMessage = $"Your visitor {securityForm.visitorName} has marked OUT.";
                await AddNotificationAsync(hostUserId, hostMessage);
                await _hubContext.Clients.User(hostUserId).SendAsync("ReceiveNotification", hostMessage);
            }

            try
            {
                // Retrieve Host Email
                var hostEmail = await _context.visitorsregistration
                    .Where(y => y.Id == securityForm.VisitorID)
                    .Select(y => y.user.Email)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(hostEmail))
                {
                    SendEmailToHost(hostEmail, securityForm.visitorName, securityForm.VisitorID);
                }
                else
                {
                    TempData["Error"] = "Host email not found!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error sending email: " + ex.Message;
            }

            TempData["Success"] = "Visitor Marked OUT Successfully!";
            return RedirectToAction(nameof(Create));
        }


        [Authorize(Roles = "Systemadmin,Superadmin,Security,User,Admin")]
        [HttpGet]
        public async Task<JsonResult> CheckVisitorStatus(int visitorId)
        {
            // Fetch the latest status for the given VisitorID
            var lastStatus = await _context.securityForms
                .Where(s => s.VisitorID == visitorId)
                .OrderByDescending(s => s.DateTime)
                .Select(s => s.status)
                .FirstOrDefaultAsync();

            // Return the status as a JSON response
            return Json(new { status = lastStatus });
        }


        [Authorize(Roles = "Systemadmin,Superadmin,Security,Admin")]

        public async Task<bool> CanMarkOut(int visitorId)
        {
            // Check if the latest status for the visitor is "IN"
            var lastStatus = await _context.securityForms
                .Where(s => s.VisitorID == visitorId)
                .OrderByDescending(s => s.DateTime)
                .Select(s => s.status)
                .FirstOrDefaultAsync();

            return lastStatus == "IN"; // Only allow "OUT" if the last status is "IN"
        }

        // POST: SecurityForms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,VisitorID,status,CreatedId,createdOn,ModifiedId,ModifiedOn,KPassId")] SecurityForm securityForm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _context.Add(securityForm);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["VisitorID"] = new SelectList(_context.visitorsregistration, "Id", "Id", securityForm.VisitorID);
            }
            catch(Exception ex)
            {
                TempData["Error"]=ex.Message;
            }
            return View(securityForm);
        }

        // GET: SecurityForms/Edit/5
        [Authorize(Roles = "Systemadmin,Superadmin,Security")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var securityForm = await _context.securityForms.FindAsync(id);
            if (securityForm == null)
            {
                return NotFound();
            }
            ViewData["VisitorID"] = new SelectList(_context.visitorsregistration, "Id", "Id", securityForm.VisitorID);
            return View(securityForm);
        }

        // POST: SecurityForms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Systemadmin,Superadmin,Security,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,VisitorID,status,CreatedId,createdOn,ModifiedId,ModifiedOn,KPassId")] SecurityForm securityForm)
        {
            if (id != securityForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(securityForm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SecurityFormExists(securityForm.Id))
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
            ViewData["VisitorID"] = new SelectList(_context.visitorsregistration, "Id", "Id", securityForm.VisitorID);
            return View(securityForm);
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        // GET: SecurityForms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var securityForm = await _context.securityForms
                .Include(s => s.Visitor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (securityForm == null)
            {
                return NotFound();
            }

            return View(securityForm);
        }

        [Authorize(Roles = "Systemadmin,Superadmin,Admin")]
        // POST: SecurityForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var securityForm = await _context.securityForms.FindAsync(id);
            if (securityForm != null)
            {
                _context.securityForms.Remove(securityForm);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SecurityFormExists(int id)
        {
            return _context.securityForms.Any(e => e.Id == id);
        }
    }
}
