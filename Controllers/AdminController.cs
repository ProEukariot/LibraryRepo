using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net.Http;

namespace LibraryApp.Controllers
{
	public class AdminController : Controller
	{
		private readonly SqlConnection db;

		public AdminController(SqlConnection con)
		{
			db = con;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult AddBook() => View();

		[HttpPost]
		public async Task<IActionResult> AddBook(Book model)
		{
			if (model.Image == null)
				ModelState.AddModelError("", "Image is required.");

			if (model.BookContents == null)
				ModelState.AddModelError("", "Image is required.");

			if (!ModelState.IsValid)
				return View(model);

			byte[] imgBuffer = new byte[model.Image!.Length];
			byte[] dataBuffer = new byte[model.BookContents!.Length];

			Stream streamImage = model.Image.OpenReadStream(); ;
			Stream streamContent = model.BookContents.OpenReadStream(); ;

			try
			{
				var DbOpenTask = db.OpenAsync();
				var ReadImgTask = streamImage.ReadAsync(imgBuffer, 0, imgBuffer.Length);
				var ReadContentTask = streamContent.ReadAsync(dataBuffer, 0, dataBuffer.Length);

				await Task.WhenAll(DbOpenTask, ReadImgTask, ReadContentTask);

				string query = "INSERT INTO Books(Id, Name, Author, Genre, Description, Image, FileData) " +
					"VALUES (@id, @name, @athor, @genre, @desc, @image, @file); ";

				using (SqlCommand cmd = new(query, db))
				{
					cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
					cmd.Parameters.AddWithValue("@name", model.Name);
					cmd.Parameters.AddWithValue("@athor", model.Author);
					cmd.Parameters.AddWithValue("@genre", model.Genre);
					cmd.Parameters.AddWithValue("@desc", model.Description);
					cmd.Parameters.AddWithValue("@image", imgBuffer);
					cmd.Parameters.AddWithValue("@file", dataBuffer);

					await cmd.ExecuteNonQueryAsync();
				}

				//Console.WriteLine(imageBuffer + "[--]" + contentBuffer + "[--]");
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(model);
			}
			finally
			{
				await db.CloseAsync();
				streamImage.Close();
				streamContent.Close();
			}

			return View();
		}
	}
}
