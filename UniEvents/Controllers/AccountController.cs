using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniEvents.Data;
using UniEvents.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UniEvents.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbcontext _context;

        public AccountController(AppDbcontext context)
        {
            _context = context;
        }



        [HttpPost]
                 
        public async Task<IActionResult> Login(User user)
        {
            var chkuser = _context.users
                .Where(usr => usr.Email == user.Email && usr.Password == user.Password)
                .FirstOrDefault();

            if (chkuser != null)
            {
                // Create the user claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, chkuser.UserId.ToString()), 
            new Claim(ClaimTypes.Name, chkuser.FirstName + " " + chkuser.LastName), 
            new Claim(ClaimTypes.Email, chkuser.Email),
            new Claim(ClaimTypes.Role, chkuser is Admin ? "Admin" : "Attendee") 
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

               
                if (chkuser is Admin)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Explore", "Home");
                }
            }

            ViewBag.error = "Wrong user/password";
            return View(user);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Attendee user)
        {
            if (ModelState.IsValid)
            {
                
                if (user.Password != user.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The passwords do not match.");
                    return View(user);
                }

                try
                {
                  
                  
                    _context.users.Add(user);  
                    _context.SaveChanges();

                    return RedirectToAction("Login", "Account"); 
                }
                catch (Exception ex)
                {
                   
                    var inner = ex.InnerException?.Message;
                    ModelState.AddModelError("", "An error occurred while saving the user: " + ex.Message + " | Inner: " + inner);
                    return View(user);
                }
            }

            return View(user);
        }




        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account"); 
        }

    }
}
