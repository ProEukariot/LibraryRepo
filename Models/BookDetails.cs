namespace LibraryApp.Models
{
	public class BookDetails
	{
		public Book Book { get; set; }

		public bool IsFavorite { get; set; }

		public BookDetails() { }

		public BookDetails(Book book)
		{
			Book = book;
		}

		public BookDetails(Book book, bool isFav) : this(book)
		{
			IsFavorite = isFav;
			//Book = book;
		}
	}
}
