using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawCare.Models;

namespace PawCare.Controllers
{
    public class PetController : Controller
    {
        private readonly AppDbContext _context;

        public PetController(AppDbContext context)
        {
            _context = context;
        }

        // عرض كل الحيوانات مع البحث
        public IActionResult Index(string search)
        {
            var pets = _context.Pets.Include(p => p.User).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                pets = pets.Where(p => p.Name.Contains(search) || p.Species.Contains(search) || p.Breed.Contains(search));
            ViewBag.Search = search;
            return View(pets.ToList());
        }

        // عرض الحيوانات المتاحة للتبني مع البحث
        public IActionResult Adopt(string search)
        {
            var pets = _context.Pets.Where(p => p.IsAvailableForAdoption).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                pets = pets.Where(p => p.Name.Contains(search) || p.Species.Contains(search));
            ViewBag.Search = search;
            return View(pets.ToList());
        }

        // طلب تبني
        [HttpPost]
        public IActionResult RequestAdoption(int petId, string message)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var existing = _context.AdoptionRequests.FirstOrDefault(r => r.PetId == petId && r.UserId == userId);
            if (existing != null)
            {
                TempData["Error"] = "You already requested to adopt this pet!";
                return RedirectToAction("Adopt");
            }
            var request = new AdoptionRequest
            {
                PetId = petId,
                UserId = (int)userId,
                Message = message ?? "",
                Status = "Pending"
            };
            _context.AdoptionRequests.Add(request);
            _context.SaveChanges();
            TempData["Success"] = "Adoption request sent successfully!";
            return RedirectToAction("Adopt");
        }

        // إضافة حيوان
        public IActionResult AddPet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public IActionResult AddPet(Pet pet)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            pet.UserId = (int)userId;
            _context.Pets.Add(pet);
            _context.SaveChanges();
            TempData["Success"] = "Pet added successfully!";
            return RedirectToAction("MyPets");
        }

        // حيواناتي
        public IActionResult MyPets()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var pets = _context.Pets.Where(p => p.UserId == userId).ToList();
            return View(pets);
        }

        // تعديل حيوان
        public IActionResult EditPet(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var pet = _context.Pets.Find(id);
            if (pet == null || pet.UserId != userId) return RedirectToAction("MyPets");
            return View(pet);
        }

        [HttpPost]
        public IActionResult EditPet(Pet pet)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            pet.UserId = (int)userId;
            _context.Pets.Update(pet);
            _context.SaveChanges();
            TempData["Success"] = "Pet updated successfully!";
            return RedirectToAction("MyPets");
        }

        // حذف حيوان
        public IActionResult DeletePet(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var pet = _context.Pets.Find(id);
            if (pet != null && pet.UserId == userId)
            {
                _context.Pets.Remove(pet);
                _context.SaveChanges();
                TempData["Success"] = "Pet deleted successfully!";
            }
            return RedirectToAction("MyPets");
        }

        // طلبات التبني
        public IActionResult MyRequests()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");
            var requests = _context.AdoptionRequests
                .Include(r => r.Pet)
                .Where(r => r.UserId == userId)
                .ToList();
            return View(requests);
        }
    }
}