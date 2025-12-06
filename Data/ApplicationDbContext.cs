using Eylul_Webproje.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eylul_Webproje.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = default!;
        public DbSet<Instructor> Instructors { get; set; } = default!;
        public DbSet<AdminProfile> AdminProfiles { get; set; } = default!;
    }
}