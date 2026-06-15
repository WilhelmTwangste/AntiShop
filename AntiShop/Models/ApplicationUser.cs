using Microsoft.AspNetCore.Identity;

namespace AntiShop.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; }
    }
}
