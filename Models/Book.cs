using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApp.Models
{
	public class Book : BookData
	{
		public byte[] Image { get; set; } = Array.Empty<byte>();

		public byte[] FileData { get; set; } = Array.Empty<byte>();
	}
}
