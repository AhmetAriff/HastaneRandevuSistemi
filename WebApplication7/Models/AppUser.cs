using Microsoft.AspNetCore.Identity;

namespace WebApplication7.Models
{
    public class AppUser :IdentityUser
    {
        public ICollection<Appointment>? appointments { get; set; }
    }
}
