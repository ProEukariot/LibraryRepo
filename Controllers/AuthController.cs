using LibraryApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Security.Claims;
using LibraryApp.Extensions;

namespace LibraryApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly SqlConnection db;

        public AuthController(SqlConnection con)
        {
            db = con;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserIdentity model, string? returnUrl = null)
        {
            User user = new();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await db.OpenAsync();

                string qry = "SELECT * FROM Users " +
                    "WHERE Username = @username AND PasswordHash = @passwordHash; ";

                using (SqlCommand cmd = new(qry, db))
                {
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@passwordHash", Calc.Hash(model.Password));

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!reader.HasRows)
                            throw new Exception("No such a user.");

                        while (await reader.ReadAsync())
                        {
                            user.Id = reader.GetGuid(0);
                            user.Username = reader.GetString(1);
                            user.Email = reader.GetString(3);
                        }

                        Console.WriteLine(user.Id);
                    }
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
            finally
            {
                await db.CloseAsync();
            }

            var claim = new List<Claim>() {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Redirect(returnUrl ?? "/Home");
        }

        public IActionResult Regist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Regist(UserIdentityReg model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await db.OpenAsync();

                string qry = "INSERT INTO Users(Id, Username, PasswordHash, Email) " +
                    "VALUES (@id, @username, @passwordHash, @email); ";

                using (SqlCommand cmd = new(qry, db))
                {
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@passwordHash", Calc.Hash(model.Password));
                    cmd.Parameters.AddWithValue("@email", model.Email);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }
            finally
            {
                await db.CloseAsync();
            }

            User user = new() { Username = model.Username, Email = model.Email };

            var claim = new List<Claim>() {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Redirect(returnUrl ?? "/");
        }
    }
}
