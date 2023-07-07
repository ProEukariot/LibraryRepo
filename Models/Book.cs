using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
	public class Book
	{
		[Required]
		[Display(Name = "Name")]
		public string Name { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Author")]
		public string Author { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Genre")]
		public string Genre { get; set; } = string.Empty;

		[Display(Name = "Description")]
		public string Description { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Image")]
		public IFormFile Image { get; set; } = null!;

		[Required]
		[Display(Name = "Book")]
		public IFormFile BookContents { get; set; } = null!;
	}
}
