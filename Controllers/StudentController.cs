using Eylul_Webproje.Data;
using Eylul_Webproje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eylul_Webproje.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ------------------ DASHBOARD ------------------
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            // Kayıtlı kurslar
            var enrollments = await _context.Enrollments
                .Where(e => e.StudentId == student.Id)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Modules)
                .ToListAsync();

            int enrolledCount = enrollments.Count;

            // Toplam modül sayısı
            int totalModules = enrollments
                .Sum(e => e.Course.Modules.Count);

            // 🟡 ŞİMDİLİK DEMO İLERLEME
            // (ileride CompletedModule tablosu ile değişecek)
            int completedModules = (int)(totalModules * 0.12);

            double progressPercent = totalModules == 0
                ? 0
                : (completedModules * 100.0) / totalModules;

            ViewBag.EnrolledCount = enrolledCount;
            ViewBag.WeeklyTarget = 20;
            ViewBag.ProgressPercent = Math.Round(progressPercent);

            return View();
        }


        // ------------------ LIST ALL COURSES ------------------
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .ToListAsync();

            return View(courses);
        }


        // ------------------ COURSE DETAIL ------------------
        public async Task<IActionResult> CourseDetail(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Modules)
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            return View(course);
        }


        // ------------------ ENROLL TO A COURSE ------------------
        public async Task<IActionResult> Enroll(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            var alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == student.Id && e.CourseId == id);

            if (!alreadyEnrolled)
            {
                var enroll = new Enrollment
                {
                    CourseId = id,
                    StudentId = student.Id,
                    EnrollDate = DateTime.Now
                };

                _context.Enrollments.Add(enroll);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyCourses");
        }


        // ------------------ MY COURSES ------------------
        public async Task<IActionResult> MyCourses()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            var myCourses = await _context.Enrollments
                .Include(e => e.Course)
                .ThenInclude(c => c.Instructor)
                .ThenInclude(i => i.User)
                .Where(e => e.StudentId == student.Id)
                .ToListAsync();

            return View(myCourses);
        }
    }
}
