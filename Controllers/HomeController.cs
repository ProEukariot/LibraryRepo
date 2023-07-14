using LibraryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Net.Http.Headers;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Reflection.Metadata.BlobBuilder;
using System.Web;
using System.Net.Http;
using System.Reflection;

namespace LibraryApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly SqlConnection db;

		public HomeController(SqlConnection con)
		{
			db = con;
		}

		public IActionResult Index() => View();

		[HttpPost]
		[Authorize]
		[Route("/AddFav")]
		public async Task<IActionResult> AddToFav([FromBody] BookAbstract book)
		{

			try
			{
				var DbOpenTask = db.OpenAsync();

				string query = "INSERT INTO Favorites(userId, bookId) VALUES " +
					"(@userId, @bookId); ";

				using (SqlCommand cmd = new(query, db))
				{
					cmd.Parameters.AddWithValue("@bookId", book.Id);
					cmd.Parameters.AddWithValue("@userId", HttpContext.User.FindFirst("id")!.Value);
					await cmd.ExecuteNonQueryAsync();
				}
			}
			catch (Exception e)
			{
				return BadRequest();
			}
			finally
			{
				await db.CloseAsync();
			}

			return Ok();
		}

		[HttpPost]
		[Authorize]
		[Route("/RemoveFav")]
		public async Task<IActionResult> RemoveFromFav([FromBody] BookAbstract book)
		{

			try
			{
				var DbOpenTask = db.OpenAsync();

				string query = "DELETE FROM Favorites WHERE userId = @userId AND bookId = @bookId; ";

				using (SqlCommand cmd = new(query, db))
				{
					cmd.Parameters.AddWithValue("@bookId", book.Id);
					cmd.Parameters.AddWithValue("@userId", HttpContext.User.FindFirst("id")!.Value);
					await cmd.ExecuteNonQueryAsync();
				}
			}
			catch (Exception e)
			{
				return BadRequest();
			}
			finally
			{
				await db.CloseAsync();
			}

			return Ok();
		}

		[Authorize]
		public async Task<IActionResult> Read(Guid? id, string? actionType)
		{
			string query = "SELECT * FROM Books WHERE id = @id; ";
			Book book = new();

			try
			{
				await db.OpenAsync();

				using (SqlCommand cmd = new(query, db))
				{
					cmd.Parameters.AddWithValue("@id", id);

					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						if (!reader.HasRows)
						{
							return Content("No book found by this id.");
						}

						while (await reader.ReadAsync())
						{
							book = new()
							{
								Name = reader.GetString(1),
								BookContents = (byte[])reader[6],
							};
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				await db.CloseAsync();
			}

			switch (actionType)
			{
				case "Read":
					return File(book.BookContents, "Application/pdf");
				case "Download":
					return File(book.BookContents, "Application/pdf", $"{book.Name}.pdf");
				default:
					goto case "Read";
			}
		}

		public async Task<IActionResult> Books(Guid? id = null, string pattern = "", int page = 1)
		{
			string query = "SELECT * FROM Books b LEFT JOIN Favorites f ON b.id = f.bookId; ";
			List<Book> books = new() { };

			try
			{
				await db.OpenAsync();

				using (SqlCommand cmd = new(query, db))
				{
					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							if (id.HasValue)
							{
								if (!reader.GetGuid(0).Equals(id.Value))
								{
									continue;
								}

								Book book = new()
								{
									Id = reader.GetGuid(0),
									Name = reader.GetString(1),
									Author = reader.GetString(2),
									Genre = reader.GetString(3),
									Description = reader.GetString(4),
									Image = (byte[])reader[5],
								};

								Guid? userId = null;
								Guid? bookId = null;

								if (!reader.IsDBNull("userId"))
								{
									userId = reader.GetGuid("userId");
								}
								if (!reader.IsDBNull("bookId"))
								{
									bookId = reader.GetGuid("bookId");
								}

								bool isFav = false;

								if (userId != null && bookId != null)
									if (userId.ToString() == HttpContext.User.FindFirst("id")!.Value
										&& bookId == book.Id)
									{ isFav = true; }

								ViewBag.IsFavorite = isFav;

								return View("BookDetails", book);
							}
							else
							{
								Book book = new()
								{
									Id = reader.GetGuid(0),
									Name = reader.GetString(1),
									Author = reader.GetString(2),
									Image = (byte[])reader[5],
								};

								if (Regex.IsMatch(book.Name, pattern) || Regex.IsMatch(book.Author, pattern))
									books.Add(book);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				await db.CloseAsync();
			}

			int pageSize = 2;
			int totalBooks = books.Count;

			return View(new BooksViewModel(page, pageSize, totalBooks, books));
		}
	}
}
