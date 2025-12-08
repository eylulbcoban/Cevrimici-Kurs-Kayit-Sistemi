using Eylul_Webproje.Data;
using Eylul_Webproje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eylul_Webproje.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------ DASHBOARD ------------------
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            var courseCount = await _context.Courses.CountAsync(c => c.InstructorId == instructor.Id);

            ViewBag.CourseCount = courseCount;

            return View();
        }


        // ------------------ MY COURSES ------------------
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructor.Id)
                .ToListAsync();

            return View(courses);
        }


        // ------------------ CREATE COURSE ------------------
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course model)
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            model.InstructorId = instructor.Id; // Eğitmen ilişkisi ekleniyor

            _context.Courses.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourses");
        }


        // ------------------ ADD MODULE ------------------
        [HttpGet]
        public IActionResult AddModule(int courseId)
        {
            TempData["CourseId"] = courseId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddModule(Module model)
        {
            model.CourseId = int.Parse(TempData["CourseId"].ToString());

            _context.Modules.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourses");
        }


        // ------------------ STUDENTS OF COURSE ------------------
        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var students = await _context.Enrollments
                .Include(e => e.Student)
                .ThenInclude(s => s.User)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            return View(students);
        }
    }
}
