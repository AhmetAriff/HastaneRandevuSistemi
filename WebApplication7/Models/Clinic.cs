using System.ComponentModel.DataAnnotations;

namespace WebApplication7.Models
{
    public class Clinic
    {
        [Key]
        public int clinicId { get; set; }

        [Required(ErrorMessage = "Klinik ismi boş olamaz")]
        [Display(Name = "Klinik")]
        public string clinicName { get; set; }

        public ICollection<Doctor>? doctors { get; set; }
    }
}
