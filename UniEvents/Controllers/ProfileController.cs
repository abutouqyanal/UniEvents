using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniEvents.Data;
using UniEvents.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using UniEvents.ViewModels;
using System;

namespace UniEvents.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbcontext _context;

        public ProfileController(AppDbcontext context)
        {
            _context = context;
        }

        private async Task<Attendee> GetCurrentUserAttendeeAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return await _context.Attendees.FirstOrDefaultAsync(u => u.UserId == userId);
            }
            return null;
        }

        public async Task<IActionResult> About()
        {
            var user = await GetCurrentUserAttendeeAsync();
            if (user == null) return Challenge();
            ViewBag.UserProfile = user;
            return View(user);
        }

        public async Task<IActionResult> Edit()
        {
            var user = await GetCurrentUserAttendeeAsync();
            if (user == null) return Challenge();
            ViewBag.UserProfile = user;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Attendee updatedUserData)
        {
            var userToUpdate = await GetCurrentUserAttendeeAsync();
            if (userToUpdate == null) return Challenge();
            if (updatedUserData.UserId != userToUpdate.UserId) return Forbid();

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                userToUpdate.FirstName = updatedUserData.FirstName;
                userToUpdate.LastName = updatedUserData.LastName;
                userToUpdate.PhoneNumber = updatedUserData.PhoneNumber;
                userToUpdate.University_id = updatedUserData.University_id;
                userToUpdate.University_name = updatedUserData.University_name;
                userToUpdate.University_major = updatedUserData.University_major;

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("About");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Error saving profile: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
            ViewBag.UserProfile = userToUpdate;
            return View(updatedUserData);
        }

        public async Task<IActionResult> MyBookings()
        {
            var user = await GetCurrentUserAttendeeAsync();
            if (user == null) return Challenge();
            var bookedEvents = await _context.AttendeeEvents
                .Where(ae => ae.AttendeeId == user.UserId && ae.Event.Status == Status.Approved)
                .Include(ae => ae.Event)
                .Select(ae => ae.Event)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
            ViewBag.UserProfile = user;
            return View(bookedEvents);
        }

        [HttpGet]
        public async Task<IActionResult> MyEvents()
        {
           
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Challenge(); 
            }
       
            var currentUser = await _context.users.FindAsync(userId);
            if (currentUser == null)
            {
                return NotFound("User profile could not be found.");
            }

            var organizedEvents = await _context.Events
                                      .Where(e => e.OrganizerId == userId)
                                      .OrderByDescending(e => e.StartDate)
                                      .ToListAsync();
            ViewBag.UserProfile = currentUser;

            return View(organizedEvents);
        }

        public async Task<IActionResult> Calendar(DateTime? weekStartDateInput)
        {
            var user = await GetCurrentUserAttendeeAsync();
            if (user == null) return Challenge();
            ViewBag.UserProfile = user;
            DateTime weekStart;
            if (weekStartDateInput.HasValue)
            {
                weekStart = weekStartDateInput.Value.Date.AddDays(-(int)weekStartDateInput.Value.DayOfWeek);
            }
            else
            {
                var today = DateTime.Today;
                weekStart = today.AddDays(-(int)today.DayOfWeek);
            }
            DateTime weekEnd = weekStart.AddDays(7);
            ViewBag.WeekStartDate = weekStart;
            var registeredEventsThisWeek = await _context.AttendeeEvents
                .Where(ae => ae.AttendeeId == user.UserId && ae.Event.Status == Status.Approved && ae.Event.EndDate >= weekStart && ae.Event.StartDate < weekEnd)
                .Include(ae => ae.Event)
                .Select(ae => ae.Event)
                .OrderBy(e => e.StartDate)
                .ToListAsync();
            return View(registeredEventsThisWeek);
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents(int? year, int? month, int? day)
        {
            IQueryable<Event> eventsQuery = _context.Events.Where(e => e.Status == Status.Approved).OrderBy(e => e.StartDate);
            if (year.HasValue && month.HasValue)
            {
                try
                {
                    var startDate = new DateTime(year.Value, month.Value, 1);
                    var endDate = startDate.AddMonths(1);
                    eventsQuery = eventsQuery.Where(e => e.StartDate >= startDate && e.StartDate < endDate);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return BadRequest("Invalid year/month.");
                }
            }
            var calendarEvents = await eventsQuery
                .Select(e => new { id = e.EventId, title = e.EventName, start = e.StartDate.ToString("o"), end = e.EndDate.ToString("o"), })
                .ToListAsync();
            return Json(calendarEvents);
        }
    }
}