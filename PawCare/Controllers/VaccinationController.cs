using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCare.Models;

namespace PawCare.Controllers
{
    public class VaccinationController : Controller
    {
        private readonly AppDbContext _context;

        public VaccinationController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int petId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var pet = _context.Pets.Find(petId);
            if (pet == null || pet.UserId != userId) return RedirectToAction("MyPets", "Pet");
            ViewBag.Pet = pet;
            var vaccinations = _context.Vaccinations.Where(v => v.PetId == petId).ToList();
            return View(vaccinations);
        }

        public IActionResult Add(int petId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            ViewBag.PetId = petId;
            return View();
        }

        [HttpPost]
        public IActionResult Add(Vaccination vaccination)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            _context.Vaccinations.Add(vaccination);
            _context.SaveChanges();
            TempData["Success"] = "Vaccination added successfully!";
            return RedirectToAction("Index", new { petId = vaccination.PetId });
        }

        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var vaccination = _context.Vaccinations.Find(id);
            if (vaccination != null)
            {
                int petId = vaccination.PetId;
                _context.Vaccinations.Remove(vaccination);
                _context.SaveChanges();
                return RedirectToAction("Index", new { petId });
            }
            return RedirectToAction("MyPets", "Pet");
        }
    }
}