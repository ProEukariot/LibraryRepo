using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Books()
		{
			return View();
		}
	}
}
