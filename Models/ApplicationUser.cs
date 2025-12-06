// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace Eylul_Webproje.Models
{
    // IdentityUser'dan türeyen kendi kullanıcı tipimiz
    public class ApplicationUser : IdentityUser
    {
        // İstersen buraya ortak alanlar ekleyebilirsin (FullName vb.)
        // public string? FullName { get; set; }
    }
}
