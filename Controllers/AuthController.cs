using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using LibraryApp.Data;
using LibraryApp.Extensions;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace LibraryApp.Controllers
{
	public class AuthController : Controller
	{
		private readonly ISqlDataProvider<User> _db;

		public AuthController(ISqlDataProvider<User> db)
		{
			_db = db;
		}

		public IActionResult Login() => View();

		public IActionResult Regist() => View();

		[Authorize]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return Redirect("/");
		}

		[HttpPost]
		public async Task<IActionResult> Login(UserIdentity model, string? returnUrl = null)
		{
			User user;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
				string salt = config.GetValue<string>("Security:Salt") ?? throw new InvalidOperationException("No security value.");

				user = (await _db.GetAll())
						.First(u => u.Username.Equals(model.Username)
						&& u.Password.Equals(Calc.Hash(model.Password, salt)));
			}
			catch (InvalidOperationException invEx)
			{
				return Problem(invEx.Message);

			}
			catch (Exception ex)
			{
				//throw;
				ModelState.AddModelError("", "Wrong credentials.");
				return View(model);
			}

			var claimsIdentity = CreateBasicIdentity(user, CookieAuthenticationDefaults.AuthenticationScheme);

			await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

			return Redirect(returnUrl ?? "/");
		}

		[HttpPost]
		public async Task<IActionResult> Regist(UserIdentityReg model, string? returnUrl = null)
		{
			if (!ModelState.IsValid)
				return View(model);

			int newRecordId;

			try
			{
				var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
				string salt = config.GetValue<string>("Security:Salt") ?? throw new InvalidOperationException("No security value.");

				model.Password = Calc.Hash(model.Password, salt);
				model.Role = "Default";
				newRecordId = await _db.Create(model);
			}
			catch (Exception ex)
			{
				//throw;
				//return Problem(ex.Message);
				ModelState.AddModelError("", "User already exists.");
				return View(model);
			}

			User user = new()
			{
				Id = newRecordId,
				Username = model.Username,
				Email = model.Email,
				Role = model.Role
			};

			var claimsIdentity = CreateBasicIdentity(user, CookieAuthenticationDefaults.AuthenticationScheme);

			await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

			return Redirect(returnUrl ?? "/");
		}

		[NonAction]
		private ClaimsIdentity CreateBasicIdentity(User user, string authScheme)
		{
			var claims = new List<Claim>() {
				new Claim("Id", user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role),
			};

			return new ClaimsIdentity(claims, authScheme);
		}
	}
}
