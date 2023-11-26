using Microsoft.Build.Framework;
using Pronia.Models;

namespace Pronia.Areas.ProniaAdmin.ViewModels
{
    public class CreateProductVM
    {
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }

        public string SKU { get; set; }

        public string Description { get; set; }

        [Required]
        public int? CategoryId { get; set; }

    }
}
