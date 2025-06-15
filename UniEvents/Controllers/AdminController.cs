using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniEvents.Data;
using UniEvents.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ClosedXML.Excel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 

namespace UniEvents.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly AppDbcontext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(AppDbcontext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

       
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> CreateEvent(Event evt, IFormFile EventImage)
        {
            
            ModelState.Remove("Organizer"); 

            if (!ModelState.IsValid)
                return View(evt);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized("Admin user not found. Please log in again.");
            }
            evt.OrganizerId = int.Parse(userIdStr);

            evt.Status = Status.Approved;

            if (EventImage != null && EventImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(EventImage.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await EventImage.CopyToAsync(stream);
                }
                evt.ImagePath = "/uploads/images/" + uniqueFileName;
            }
            else
            {
                ModelState.AddModelError("EventImage", "An event image is required.");
                return View(evt);
            }

            try
            {
                _context.Events.Add(evt);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event has been created and approved successfully!";
                return RedirectToAction("ApprovedEvents"); 
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "An error occurred while saving to the database.");
               
                return View(evt);
            }
        }

        public async Task<IActionResult> PendingEvents()
        {
            var pendingEvents = await _context.Events
                .Where(e => e.Status == Status.Pending)
                .Include(e => e.Organizer) 
                .OrderBy(e => e.EventDate)
                .ToListAsync();
            return View(pendingEvents);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                ev.Status = Status.Approved;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Event '{ev.EventName}' has been approved.";
            }
            else
            {
                TempData["ErrorMessage"] = "Event not found.";
            }
            return RedirectToAction("PendingEvents");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectEvent(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                ev.Status = Status.Rejected;
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = $"Event '{ev.EventName}' has been rejected.";
            }
            else
            {
                TempData["ErrorMessage"] = "Event not found.";
            }
            return RedirectToAction("PendingEvents");
        }
        public async Task<IActionResult> ApprovedEvents()
        {
            var approvedEvents = await _context.Events
                .Where(e => e.Status == Status.Approved)
                .Include(e => e.Organizer)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
            return View(approvedEvents);
        }

        public async Task<IActionResult> RejectedEvents()
        {
            var rejectedEvents = await _context.Events
                .Where(e => e.Status == Status.Rejected)
                .Include(e => e.Organizer)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();
            return View(rejectedEvents);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAttendeesToExcel(int eventId)
        {
            var eventDetails = await _context.Events.FindAsync(eventId);
            if (eventDetails == null)
            {
                TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("ApprovedEvents");
            }

            var registeredAttendees = await _context.AttendeeEvents
                .Where(ae => ae.EventId == eventId)
                .Include(ae => ae.Attendee)
                .Select(ae => ae.Attendee)
                .ToListAsync();

            if (!registeredAttendees.Any())
            {
                TempData["InfoMessage"] = $"No attendees registered for '{eventDetails.EventName}'.";
                return RedirectToAction("ApprovedEvents");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendees");
                worksheet.Cell("A1").Value = "Student Name";
                worksheet.Cell("B1").Value = "University ID";
                worksheet.Range("A1:B1").Style.Font.Bold = true;

                int currentRow = 2;
                foreach (var attendee in registeredAttendees)
                {
                    worksheet.Cell(currentRow, 1).Value = $"{attendee.FirstName} {attendee.LastName}";
                    worksheet.Cell(currentRow, 2).Value = attendee.University_id;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"Attendees_{eventDetails.EventName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.xlsx";
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
    }
}