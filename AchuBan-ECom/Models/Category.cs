using System.ComponentModel.DataAnnotations;

namespace AchuBan_ECom.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [MaxLength(2000, ErrorMessage = "Max Description allowed is 2000.")]
        public string? Description { get; set; }

        [Display(Name = "Display Order")]
        [Range(1, 300, ErrorMessage = "Only 300 categories the system supports.")]
        public int displayOrder { get; set; } = 1;
    }
}
