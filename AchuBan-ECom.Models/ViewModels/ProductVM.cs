using AchuBan_ECom.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace AchuBan_ECom.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        [ValidateNever]
        public required IEnumerable<SelectListItem> CategoryList { get; set; }

        // uploaded file (not persisted) — ValidateNever so model validation ignores it
        [ValidateNever]
        public IFormFile? ImageFile { get; set; }
    }
}