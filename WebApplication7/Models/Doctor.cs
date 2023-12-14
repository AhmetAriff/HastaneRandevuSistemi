using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Doctor
    {
        [Key]
        public int doctorId { get; set; }

        [Required]
        [Display(Name = "Ad")]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Soyad")]
        public string lastName { get; set; }

        public ICollection<Appointment>? appointments { get; set; }

        [Display(Name = "Klinik")]
        public Clinic? clinic { get; set; }

        [ForeignKey("clinic")]
        [Display(Name = "Klinik")]
        public int clinicId { get; set; }
    }
}
