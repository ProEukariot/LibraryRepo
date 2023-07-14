using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;

namespace LibraryApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.Configure<KestrelServerOptions>(opt => {
				opt.AllowSynchronousIO = true;
			});
			builder.Services.AddControllersWithViews();
			builder.Services.AddTransient<SqlConnection>(opt => new SqlConnection(builder.Configuration.GetConnectionString("Default"))); ;

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt => {
				opt.AccessDeniedPath = "/Auth/Denied";
				opt.LoginPath = "/Auth/Login";
				opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
			});

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseRouting();


			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "Default",
				pattern: "{Controller=Home}/{Action=Index}/{id?}"
			);

			app.Run();
		}
	}
}