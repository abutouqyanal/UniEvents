using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniEvents.Data;
using UniEvents.Models;

namespace UniEvents.Controllers
{
    [Authorize]
    public class EventtController : Controller
    {
        private readonly AppDbcontext _context;

        public EventtController(AppDbcontext context)
        {
            _context = context;
        }

        
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return BadRequest();

            var selectedEvent = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (selectedEvent == null) return NotFound();

           
            ViewBag.EventMedia = await _context.MediaFiles.Where(m => m.EventId == id).ToListAsync();
            ViewBag.EventFeedbacks = await _context.Feedbacks.Include(f => f.Attendee).Where(f => f.EventId == id).OrderByDescending(f => f.FeedbackId).ToListAsync();
            ViewBag.AttendeeCount = await _context.AttendeeEvents.CountAsync(ae => ae.EventId == id);

            var ratings = await _context.EventRatings.Where(r => r.EventId == id).ToListAsync();
            ViewBag.AverageRating = ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
            ViewBag.RatingCount = ratings.Count;

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserRating = 0; 
            if (int.TryParse(userIdStr, out int userId))
            {
                ViewBag.isAlreadyRegistered = await _context.AttendeeEvents.AnyAsync(ae => ae.EventId == id && ae.AttendeeId == userId);
                var userRating = await _context.EventRatings.FirstOrDefaultAsync(r => r.EventId == id && r.AttendeeId == userId);
                if (userRating != null)
                {
                    ViewBag.UserRating = userRating.RatingValue;
                }
            }
            else
            {
                ViewBag.isAlreadyRegistered = false;
            }

            return View(selectedEvent);
        }

        
        [HttpGet]
        public async Task<IActionResult> Create(int? id)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            if (id.HasValue && id > 0)
            {
                var existingEvent = await _context.Events.FindAsync(id.Value);
                if (existingEvent != null) return View(existingEvent);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event evt)
        {
            ModelState.Remove("Organizer");
            if (ModelState.IsValid)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();
                evt.OrganizerId = userId;
                evt.Status = Status.Pending;
                if (evt.EventId == 0) _context.Events.Add(evt);
                else _context.Events.Update(evt);
                await _context.SaveChangesAsync();
                return RedirectToAction("Upload", "MediaFiles", new { eventId = evt.EventId });
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(evt);
        }

        [HttpGet]
        public IActionResult InviteInfo(int eventId)
        {
            if (eventId == 0) return RedirectToAction("Create");
            ViewBag.EventId = eventId;
            return View(new Invitation { EventId = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InviteInfo(Invitation model)
        {
            ModelState.Remove("Event");
            if (model.EventId == 0) ModelState.AddModelError("", "Event ID is missing.");
            if (ModelState.IsValid)
            {
                _context.Invitations.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Agreement", "Eventt", new { eventId = model.EventId });
            }
            ViewBag.EventId = model.EventId;
            return View(model);
        }

        [HttpGet]
        public IActionResult Agreement(int eventId)
        {
            if (eventId == 0) return RedirectToAction("Create");
            ViewBag.EventId = eventId;
            return View(new AgreementInfo { EventId = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agreement(AgreementInfo model)
        {
            ModelState.Remove("Event");
            if (model.EventId == 0) ModelState.AddModelError("", "Event ID is missing.");
            if (ModelState.IsValid)
            {
                var eventToUpdate = await _context.Events.FindAsync(model.EventId);
                if (eventToUpdate == null)
                {
                    ModelState.AddModelError("", "The associated event could not be found.");
                    return View(model);
                }
                eventToUpdate.Status = Status.Pending;
                _context.Agreements.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your event has been submitted successfully and is now pending approval!";
                return RedirectToAction("Explore", "Home");
            }
            ViewBag.EventId = model.EventId;
            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRating(int eventId, int ratingValue)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized(new { success = false, message = "You must be logged in to rate." });
            }

            if (ratingValue < 1 || ratingValue > 5)
            {
                return BadRequest(new { success = false, message = "Invalid rating value." });
            }

            var existingRating = await _context.EventRatings.FirstOrDefaultAsync(r => r.EventId == eventId && r.AttendeeId == userId);

            string message;
            int newUserRating = 0; 

            if (existingRating != null)
            {
                if (existingRating.RatingValue == ratingValue)
                {
                    _context.EventRatings.Remove(existingRating);
                    message = "Your rating has been removed.";
                    newUserRating = 0; 
                }
                else
                {
                    
                    existingRating.RatingValue = ratingValue;
                    existingRating.DateRated = DateTime.UtcNow;
                    message = "Your rating has been updated!";
                    newUserRating = ratingValue;
                }
            }
            else
            {
         
                _context.EventRatings.Add(new EventRating
                {
                    EventId = eventId,
                    AttendeeId = userId,
                    RatingValue = ratingValue,
                    DateRated = DateTime.UtcNow
                });
                message = "Thank you for your rating!";
                newUserRating = ratingValue;
            }

            await _context.SaveChangesAsync();

           
            var allRatings = await _context.EventRatings.Where(r => r.EventId == eventId).ToListAsync();
            var newAverage = allRatings.Any() ? allRatings.Average(r => r.RatingValue) : 0;
            var newCount = allRatings.Count;

            return Ok(new
            {
                success = true,
                message = message,
                newUserRating = newUserRating, 
                newAverageRating = newAverage.ToString("0.0"),
                newRatingCount = newCount
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeedback(Feedback model)
        {
            ModelState.Remove("Attendee");
            ModelState.Remove("Event");
            if (ModelState.IsValid)
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out int attendeeId))
                {
                    return Unauthorized(new { success = false, message = "User not authenticated." });
                }
                model.AttendeeId = attendeeId;
                model.DatePosted = DateTime.UtcNow;
                _context.Feedbacks.Add(model);
                await _context.SaveChangesAsync();
                var savedFeedback = await _context.Feedbacks.Include(f => f.Attendee).FirstOrDefaultAsync(f => f.FeedbackId == model.FeedbackId);
                return savedFeedback != null ? PartialView("_FeedbackPartial", savedFeedback) : Ok(new { success = true });
            }
            var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
            return BadRequest(new { success = false, message = error?.ErrorMessage ?? "Invalid comment." });
        }

        [Authorize] 
        [HttpGet]
        public async Task<IActionResult> EventAnalytics(int id) 
        {
            
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int organizerId))
            {
                return Unauthorized();
            }

           
            var selectedEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == id && e.OrganizerId == organizerId);

            if (selectedEvent == null)
            {
               
                return Forbid(); 
            }

            
            ViewBag.AttendeeCount = await _context.AttendeeEvents
                .CountAsync(ae => ae.EventId == id);

         
            ViewBag.FeedbackCount = await _context.Feedbacks
                .CountAsync(f => f.EventId == id);

           
            var ratings = await _context.EventRatings
                .Where(r => r.EventId == id).ToListAsync();

            ViewBag.AverageRating = ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
            ViewBag.RatingCount = ratings.Count;

            
            ViewBag.AttendeesList = await _context.AttendeeEvents
                .Where(ae => ae.EventId == id)
                .Include(ae => ae.Attendee)
                .Select(ae => ae.Attendee)
                .ToListAsync();

          
            return View(selectedEvent);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int eventId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized(new { success = false, message = "User not authenticated." });
            }

           
            var favoriteEntry = await _context.FavoriteEvents
                .FirstOrDefaultAsync(fe => fe.EventId == eventId && fe.UserId == userId);

            bool isNowFavorite;

            if (favoriteEntry == null)
            {
                
                _context.FavoriteEvents.Add(new FavoriteEvent { EventId = eventId, UserId = userId });
                isNowFavorite = true;
            }
            else
            {
               
                _context.FavoriteEvents.Remove(favoriteEntry);
                isNowFavorite = false;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, isFavorite = isNowFavorite });
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFavoriteIds()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Ok(new List<int>()); 
            }

            var favoriteIds = await _context.FavoriteEvents
                .Where(fe => fe.UserId == userId)
                .Select(fe => fe.EventId)
                .ToListAsync();

            return Ok(favoriteIds);
        }
    }
}