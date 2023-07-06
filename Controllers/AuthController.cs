using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Controllers
{
	public class AuthController : Controller
	{
		public IActionResult Login()
		{
			return View();
		}

		public IActionResult Regist()
		{
			return View();
		}
	}
}
