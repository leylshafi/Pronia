using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pronia.Models;

public class Slide
{
    public int Id { get; set; }
    [Required(ErrorMessage ="Title is required")]
    [MaxLength(25, ErrorMessage = "Max length is 25")]
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public int Order { get; set; }
    [NotMapped]
    public IFormFile? Photo { get; set; }
}
