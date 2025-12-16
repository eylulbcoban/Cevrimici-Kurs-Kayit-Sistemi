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

        
        public async Task<IActionResult> Index(string search)
        {
            var coursesQuery = _context.Courses
                .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(search))
            {
                coursesQuery = coursesQuery.Where(c =>
                    c.Title.Contains(search) ||
                    c.Category != null && c.Category.Contains(search));
            }

            var courses = await coursesQuery.ToListAsync();

            ViewBag.Search = search;
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
