using LibraryApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{

	public class HomeController : Controller
	{
		private readonly ISqlDataProvider<Book> _booksProvider;
		private readonly ISqlDataProvider<SavedBooks> _savedProvider;

		public HomeController(ISqlDataProvider<Book> booksProvider, ISqlDataProvider<SavedBooks> savedProvider)
		{
			_booksProvider = booksProvider;
			_savedProvider = savedProvider;
		}

		public IActionResult Index() => View();

		[Authorize]
		public async Task<IActionResult> Read(int id, string? actionType)
		{
			Book book = new();

			try
			{
				book = await _booksProvider.Get(id);

				if (book == null)
					throw new Exception("Not found!");
			}
			catch (Exception ex)
			{
				throw;
			}

			switch (actionType)
			{
				case "Read":
					return File(book.FileData, "Application/pdf");
				case "Download":
					return File(book.FileData, "Application/pdf", $"{book.Name}.pdf");
				default:
					goto case "Read";
			}
		}

		public async Task<IActionResult> Books(PageParams pageParams)
		{
			try
			{
				var books = (await _booksProvider.GetAll())
												.Where(b => b.Name.Contains(pageParams.Pattern ?? ""));

				ViewBag.pageParams = pageParams;

				return View(books);

			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}

		[HttpGet("[controller]/[action]/{id:int}")]
		public async Task<IActionResult> Books(int id)
		{
			try
			{
				var book = await _booksProvider.Get(id);

				ViewBag.IsSaved = !true;

				if (User.Identity!.IsAuthenticated)
				{
					var listFav = await _savedProvider.GetAll();

					var recordIds = from fav in listFav
									where fav.UserId.Equals(int.Parse(User.FindFirst("Id")!.Value))
									&& fav.BookId.Equals(id)
									select fav.Id;

					if (recordIds.Any())
						ViewBag.IsSaved = true;
				}

				return View("BookDetails", book);
			}
			catch (Exception ex)
			{
				//return Problem(ex.Message);
				return RedirectToAction("Books", new { id = "" });
			}
		}

		public async Task<IActionResult> SavedBooks(PageParams pageParams)
		{
			try
			{
				var saved = await _savedProvider.GetAll();
				var books = await _booksProvider.GetAll();

				var savedBooks = from svd in saved
								 join book in books on svd.BookId equals book.Id
								 where svd.UserId.Equals(int.Parse(User.FindFirst("Id")!.Value))
								 && book.Name.Contains(pageParams.Pattern)
								 select book;

				ViewBag.pageParams = pageParams;

				return View(savedBooks);
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}
	}
}
