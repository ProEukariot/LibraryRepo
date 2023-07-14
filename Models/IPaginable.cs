namespace LibraryApp.Models
{
	public interface IPaginable<T>
	{
		int CurrPage { get; set; }

		int MaxPages { get; set; }

		int PageSize { get; set; }

		IEnumerable<T> Values { get; set; }

		bool HasNextPage();

		bool HasPrevPage();
	}
}
