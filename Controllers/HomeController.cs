using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Net.Http.Headers;
using System.Data;

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
		//"application/octet-stream"
		public async Task<IActionResult> Books(Guid? id = null)
		{
			string query = "SELECT * FROM Books; ";
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
									//BookContents = (byte[])reader[6],
								};

								return View("BookDetails", book);
							}
							else
							{
								Book book = new()
								{
									Id = reader.GetGuid(0),
									Name = reader.GetString(1),
									Image = (byte[])reader[5],
								};

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

			return View(books);
		}
	}
}
