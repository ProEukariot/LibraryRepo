using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryApp.Models
{
	public class BooksViewModel : IPaginable<Book>
	{
		public int CurrPage { get; set; }

		public int MaxPages { get; set; }

		public int PageSize { get; set; }

		public IEnumerable<Book> Values { get; set; } = null!;

		public BooksViewModel(int currPage, int pageSize, int totalItems, IEnumerable<Book> books)
		{
			bool lastPageNOTEmpty = totalItems % pageSize > 0;
			MaxPages = totalItems / pageSize + (lastPageNOTEmpty ? 1 : 0);

			CurrPage = currPage > MaxPages ? MaxPages : currPage;
			CurrPage = currPage < 1 ? 1 : CurrPage;

			PageSize = pageSize;

			Values = books = books.Skip((CurrPage - 1) * PageSize).Take(PageSize);
		}

		public bool HasNextPage() => CurrPage < MaxPages;

		public bool HasPrevPage() => CurrPage > 1;
	}
}
