using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.ProniaAdmin.ViewModels
{
	public class CreateSlideVm
	{
		[Required(ErrorMessage = "Title is required")]
		[MaxLength(25, ErrorMessage = "Max length is 25")]
		public string Title { get; set; }
		public string Subtitle { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public IFormFile? Photo { get; set; }
	}
}
