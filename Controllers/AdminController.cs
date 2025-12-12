using Eylul_Webproje.Data;
using Eylul_Webproje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eylul_Webproje.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ================= DASHBOARD =================
        public async Task<IActionResult> Index()
        {
            ViewBag.CourseCount = await _context.Courses.CountAsync();
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.InstructorCount = await _context.Instructors.CountAsync();
            return View();
        }

        // ================= CATEGORIES =================
        public async Task<IActionResult> Categories()
        {
            return View(await _context.Categories.ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Categories.Update(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
        }

        // ================= COURSES =================
        public async Task<IActionResult> AllCourses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .ToListAsync();

            return View(courses);
        }

        // 👉 ADMIN COURSE DETAIL
        public async Task<IActionResult> CourseDetail(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // 👉 ADMIN COURSE STUDENTS
        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var students = await _context.Enrollments
                .Include(e => e.Student)
                .ThenInclude(s => s.User)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            return View(students);
        }

        // ================= USERS =================
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // ================= ROLE ASSIGN =================
        [HttpGet]
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new List<string> { "Instructor", "Student" };
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // 🔥 Eski roller silinsin
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // 🔥 Yeni rol eklensin
            await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(Users));
        }
    }
}
