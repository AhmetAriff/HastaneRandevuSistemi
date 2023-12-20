using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication7.Models
{
    public class Appointment
    {
        [Key]
        public int appointmentID { get; set; }

        [ForeignKey("user")]
        public string? userID { get; set; }

        [ForeignKey("doctor")]
        public int doctorID { get; set; }

        public AppUser? user { get; set; }

        public Doctor? doctor { get; set; }

        //public int clinicId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime appointmentDate { get; set; }

        public TimeSpan appointmentTime { get; set; }

        public bool? isBooked {  get; set; }
    }
}
