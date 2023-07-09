using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
	public class BookAbstract
	{
		[Key]
		public Guid Id { get; set; }

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
	}
}
