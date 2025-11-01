using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AchuBan_ECom.Models.Models
{
    public class ApplicationUser:IdentityUser 
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}
