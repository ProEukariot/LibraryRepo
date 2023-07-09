using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
	public class Book : BookAbstract
	{
		//[Required]
		//[Display(Name = "Image")]
		public byte[] Image { get; set; } = Array.Empty<byte>();

		//[Required]
		//[Display(Name = "Book")]
		public byte[] BookContents { get; set; } = Array.Empty<byte>();
	}
}
