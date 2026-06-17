using Microsoft.AspNetCore.Mvc;
using PawCare.Models;

namespace PawCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ViewBag.Error = "Email already exists";
                return View(user);
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = _context.Users.Find(userId);
            return View(user);
        }

        // تعديل بيانات الحساب
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = _context.Users.Find(userId);
            return View(user);
        }

        [HttpPost]
        public IActionResult EditProfile(User updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = _context.Users.Find(userId);
            if (user == null) return RedirectToAction("Login");
            user.FullName = updatedUser.FullName;
            user.Phone = updatedUser.Phone;
            _context.SaveChanges();
            HttpContext.Session.SetString("UserName", user.FullName);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = _context.Users.Find(userId);
            if (user == null) return RedirectToAction("Login");
            if (user.Password != oldPassword)
            {
                ViewBag.Error = "Old password is incorrect";
                return View();
            }
            user.Password = newPassword;
            _context.SaveChanges();
            TempData["Success"] = "Password changed successfully!";
            return RedirectToAction("Profile");
        }
    }
}