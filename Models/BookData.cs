using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
	public abstract class BookData
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Display(Name = "Name")]
		[MaxLength(20)]
		public string Name { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Author")]
		[MaxLength(20)]
		public string Author { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Genre")]
		[MaxLength(20)]
		public string Genre { get; set; } = string.Empty;

		[Display(Name = "Description")]
		[MaxLength(50)]
		public string Description { get; set; } = string.Empty;
    }
}
