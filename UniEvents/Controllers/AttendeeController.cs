using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniEvents.Data;
using UniEvents.Models;

namespace UniEvents.Controllers
{
    public class AttendeeController : Controller
    {
        private readonly AppDbcontext _context;

        public AttendeeController(AppDbcontext dbcontext)
        {
            _context = dbcontext;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAttendeeInEvent(int eventId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int attendeeId))
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "User not authenticated." });
                else
                    TempData["ErrorMessage"] = "User not authenticated.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var eventToRegister = await _context.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
            var attendee = await _context.users.OfType<Attendee>().FirstOrDefaultAsync(u => u.UserId == attendeeId);

            if (eventToRegister == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Event not found." });
                else
                    TempData["ErrorMessage"] = "Event not found.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            if (attendee == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Attendee not found." });
                else
                    TempData["ErrorMessage"] = "Attendee not found.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var alreadyRegistered = await _context.Set<AttendeeEvent>()
                .AnyAsync(ae => ae.EventId == eventId && ae.AttendeeId == attendeeId);

            if (alreadyRegistered)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "You are already registered for this event." });
                else
                    TempData["ErrorMessage"] = "You are already registered for this event.";
                return RedirectToAction("Details", "Event", new { id = eventId });
            }

            var attendeeEvent = new AttendeeEvent
            {
                EventId = eventId,
                AttendeeId = attendeeId
            };

            _context.Set<AttendeeEvent>().Add(attendeeEvent);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "You have successfully registered for the event." });
            else
                TempData["SuccessMessage"] = "You have successfully registered for the event.";
            return RedirectToAction("Details", "Event", new { id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> CancelRegistration(int eventId)
        {   
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int currentUserId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated." });
            }

            var registrationToRemove = await _context.AttendeeEvents
                .FirstOrDefaultAsync(ae => ae.EventId == eventId && ae.AttendeeId == currentUserId);

            if (registrationToRemove == null)
            {
                return NotFound(new { success = false, message = "You are not registered for this event." });
            }

            _context.AttendeeEvents.Remove(registrationToRemove);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Registration cancelled successfully." });
        }

    }
}
