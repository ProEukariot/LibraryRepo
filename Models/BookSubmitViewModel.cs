using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
	public class BookSubmitViewModel : BookAbstract
	{
		[Required]
		[Display(Name = "Image")]
		public IFormFile Image { get; set; } = null!;

		[Required]
		[Display(Name = "Book")]
		public IFormFile BookContents { get; set; } = null!;
	}
}
