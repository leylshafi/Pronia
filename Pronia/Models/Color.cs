using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Color
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(25, ErrorMessage = "Name's max length is 25")]
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
