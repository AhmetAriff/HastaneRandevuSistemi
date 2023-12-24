using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebApplication7.Models
{
    public class Clinic
    {
        [Key]
        public int clinicId { get; set; }

        [Required(ErrorMessage = "Klinik ismi boş olamaz")]
        [Display(Name = "Klinik")]
        public string clinicName { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Doctor>? doctors { get; set; }
    }
}
