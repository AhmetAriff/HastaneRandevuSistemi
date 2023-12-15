using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class AppUser :IdentityUser
    {

        public AppUser()
        {
            appointments = new List<Appointment>();
        }
        public ICollection<Appointment>? appointments;

        [Display(Name ="Ad")]
        public string? firstName { get; set; }

        [Display(Name = "Soyad")]
        public string? lastName { get; set; }
    }
}
