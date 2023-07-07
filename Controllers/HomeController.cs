using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LibraryApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly SqlConnection db;

		public HomeController(SqlConnection con)
		{
			db = con;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Books()
		{
			try
			{
				await db.OpenAsync();
			}
			catch (Exception e)
			{

			}
			finally
			{
				await db.CloseAsync();
			}

			return View();
		}
	}
}
