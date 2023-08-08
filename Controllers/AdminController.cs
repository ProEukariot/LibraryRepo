using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.Http;

namespace LibraryApp.Controllers
{
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller
	{
		private readonly ISqlDataProvider<Book> _db;

		public AdminController(ISqlDataProvider<Book> db)
		{
			_db = db;
		}

		public IActionResult Index() => View();

		public IActionResult AddBook() => View();

		[HttpPost]
		public async Task<IActionResult> AddBook(BookSubmitViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			using (var imgStream = new MemoryStream())
			using (var fileStream = new MemoryStream())
			{
				var imgTask = model.Image.CopyToAsync(imgStream);
				var fileTask = model.BookContents.CopyToAsync(fileStream);

				await Task.WhenAll(imgTask, fileTask);

				var imgBytes = imgStream.ToArray();
				var fileBytes = fileStream.ToArray();

				Book book = new() 
				{
					Name = model.Name,
					Author = model.Author,
					Genre = model.Genre,
					Description = model.Description,
					Image = imgBytes,
					FileData = fileBytes
				};
#pragma warning disable CS0168
				try
				{
					await _db.Create(book);
				}
				catch (Exception ex)
				{
					//throw;
					//return Problem(ex.Message);
					Response.StatusCode = 201;
					return View(model);
				}
#pragma warning restore CS0168
			}

			Response.StatusCode = 201;

			return View(model);
			//return RedirectToAction("AddBook", "Admin");
		}
	}
}
