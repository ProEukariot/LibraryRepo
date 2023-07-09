using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
	public class Book : BookAbstract
	{
		public byte[] Image { get; set; } = Array.Empty<byte>();

		public byte[] BookContents { get; set; } = Array.Empty<byte>();
	}
}
