using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AchuBan_ECom.Models.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        [Display(Name = "Price for 1-50")]
        public decimal ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 50+")]
        public decimal Price50 { get; set; }

        [Required]
        [Display(Name = "Price for 100+")]
        public decimal Price100 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public required Category category { get; set; }
    }
}
