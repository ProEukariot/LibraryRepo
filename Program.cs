using LibraryApp.Data;
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

			#region SynchronousIO
			//builder.Services.Configure<KestrelServerOptions>(opt =>
			//{
			//	//opt.AllowSynchronousIO = true;
			//});
			#endregion

			builder.Services.AddControllersWithViews();

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(opt =>
				{
					opt.AccessDeniedPath = "/Auth/Denied";
					opt.LoginPath = "/Auth/Login";
					opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				});

			builder.Services.AddTransient<SqlConnection>(opt => new SqlConnection(builder.Configuration.GetConnectionString("Default"))); ;
			builder.Services.AddSingleton<ISqlDataProvider<Book>, SqlBookProvider>();
			builder.Services.AddSingleton<ISqlDataProvider<User>, SqlUserProvider>();
			builder.Services.AddSingleton<ISqlDataProvider<SavedBooks>, SqlSavedBooksProvider>();

			var app = builder.Build();

			app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "Default",
				pattern: "{Controller=Home}/{Action=Index}/{id:int?}"
			);

			app.Run();
		}
	}
}