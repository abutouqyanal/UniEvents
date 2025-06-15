using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UniEvents.Data;
using UniEvents.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;      
using System.Security.Claims;         


namespace UniEvents.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbcontext _context;

        public HomeController(AppDbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Explore(string searchString, string place, string dateFilter) 
        {
           
            var today = DateTime.Today;
            ViewBag.TodaysEvents = await _context.Events
                .Where(e => e.StartDate.Date == today && e.Status == Status.Approved)
                .Take(5) 
                .ToListAsync();

            int? userId = null;
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int parsedUserId)) { userId = parsedUserId; }

            HashSet<int> favoriteEventIds = new HashSet<int>();
            if (userId.HasValue)
            {
                favoriteEventIds = await _context.FavoriteEvents
                                            .Where(fe => fe.UserId == userId.Value)
                                            .Select(fe => fe.EventId)
                                            .ToHashSetAsync();
            }
            ViewBag.UserFavoriteIds = favoriteEventIds;

            var eventsQuery = _context.Events.Where(e => e.Status == Status.Approved).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            { eventsQuery = eventsQuery.Where(e => e.EventName.Contains(searchString) || e.Description.Contains(searchString)); }

            if (!String.IsNullOrEmpty(place))
            { eventsQuery = eventsQuery.Where(e => e.Location != null && e.Location.ToLower().Contains(place.ToLower())); }
    
            DateTime? filterStartDate = null; DateTime? filterEndDate = null;
          
            switch (dateFilter?.ToLower()) { case "today": filterStartDate = DateTime.Today; filterEndDate = DateTime.Today.AddDays(1); break; 
                case "this_week": int diff = (7 + (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Sunday) % 7; filterStartDate = DateTime.Today.AddDays(-diff).Date; filterEndDate = filterStartDate.Value.AddDays(7); break; 
                case "this_month": filterStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); filterEndDate = filterStartDate.Value.AddMonths(1); break; }
            if (filterStartDate.HasValue && filterEndDate.HasValue)
            { eventsQuery = eventsQuery.Where(e => e.StartDate >= filterStartDate.Value && e.StartDate < filterEndDate.Value); }
 
            var filteredEvents = await eventsQuery
                                    .OrderByDescending(e => e.StartDate) 
                                    .ToListAsync(); 

        
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentPlace = place;
            ViewBag.CurrentDateFilter = dateFilter;

            return View(filteredEvents); 
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyFavorites()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) { return Challenge(); }

            var favoriteEvents = await _context.FavoriteEvents
                .Where(fe => fe.UserId == userId)
                .Include(fe => fe.Event)
                .Select(fe => fe.Event)
                .Where(e => e != null && e.Status == Status.Approved)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();

            return View(favoriteEvents);
        }
    }
}