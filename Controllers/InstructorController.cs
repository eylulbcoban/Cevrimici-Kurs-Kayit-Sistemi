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

        // ================== DASHBOARD ==================
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null)
            {
                instructor = new Instructor
                {
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
            }

            var courses = await _context.Courses
                .Include(c => c.Modules)
                .Where(c => c.InstructorId == instructor.Id)
                .ToListAsync();

            ViewBag.CourseCount = courses.Count;
            ViewBag.ModuleCount = courses.Sum(c => c.Modules.Count);
            ViewBag.RecentCourses = courses.Take(3).ToList();

            return View();
        }

        // ================== MY COURSES ==================
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return View(new List<Course>());

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null)
                return View(new List<Course>());

            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructor.Id)
                .ToListAsync();

            return View(courses);
        }

        // ------------------ STUDENTS OF COURSE ------------------
        public async Task<IActionResult> CourseStudents(int courseId)
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null)
                return RedirectToAction("MyCourses");

            var students = await _context.Enrollments
                .Include(e => e.Student)
                .ThenInclude(s => s.User)
                .Include(e => e.Course)
                .Where(e =>
                    e.CourseId == courseId &&
                    e.Course.InstructorId == instructor.Id)
                .ToListAsync();

            return View(students);
        }

        // ================== CREATE COURSE ==================
        [HttpGet]
        public IActionResult CreateCourse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            if (instructor == null)
                return RedirectToAction("MyCourses");

            // ❗ Course.CreatedAt YOK → HİÇ dokunmuyoruz
            model.InstructorId = instructor.Id;

            _context.Courses.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourses");
        }

        // ================== ADD MODULE ==================
        [HttpGet]
        public IActionResult AddModule(int courseId)
        {
            ViewBag.CourseId = courseId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddModule(Module model, int courseId)
        {
            model.CourseId = courseId;

            _context.Modules.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourses");
        }

        public async Task<IActionResult> CourseDetail(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            var course = await _context.Courses
                .Include(c => c.Modules)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(i => i.UserId == user.Id);

            var course = await _context.Courses
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyCourses");
        }
    }
}
