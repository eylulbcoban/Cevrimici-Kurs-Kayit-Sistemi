using System.Diagnostics;
using Eylul_Webproje.Data;
using Eylul_Webproje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eylul_Webproje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index(
            string search,
            string category,
            string instructor)
        {
            var coursesQuery = _context.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.User)
                .AsQueryable();

            //  SEARCH (Baþlýk + Açýklama + Kategori string)
            if (!string.IsNullOrWhiteSpace(search))
            {
                coursesQuery = coursesQuery.Where(c =>
                    c.Title.Contains(search) ||
                    c.Description.Contains(search) ||
                    c.Category.Contains(search));
            }

            //  CATEGORY FILTER (string Category)
            if (!string.IsNullOrWhiteSpace(category))
            {
                coursesQuery = coursesQuery.Where(c =>
                    c.Category == category);
            }

            //  INSTRUCTOR FILTER
            if (!string.IsNullOrWhiteSpace(instructor))
            {
                coursesQuery = coursesQuery.Where(c =>
                    c.Instructor != null &&
                    c.Instructor.User.UserName == instructor);
            }

            var courses = await coursesQuery.ToListAsync();

            //  KATEGORÝ DROPDOWN 
            ViewBag.Categories = await _context.Courses
                .Where(c => c.Category != null)
                .Select(c => c.Category)
                .Distinct()
                .ToListAsync();

            //  EÐÝTMEN DROPDOWN
            ViewBag.Instructors = await _context.Instructors
                .Include(i => i.User)
                .Select(i => i.User.UserName)
                .Distinct()
                .ToListAsync();

            //  SEÇÝLÝ DEÐERLER
            ViewBag.Search = search;
            ViewBag.SelectedCategory = category;
            ViewBag.SelectedInstructor = instructor;

            return View(courses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
