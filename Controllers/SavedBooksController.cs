using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LibraryApp.Data;

namespace LibraryApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SavedBooksController : ControllerBase
	{
		private readonly ISqlDataProvider<SavedBooks> _provider;

		public SavedBooksController(ISqlDataProvider<SavedBooks> provider)
		{
			_provider = provider;
		}

		[Authorize]
		[HttpPost("Add")]
		public async Task<IResult> AddToFav([FromBody] IDictionary<string, string> json)
		{
			try
			{
				int id = int.Parse(json["Id"]);

				int recordId = await _provider.Create(new SavedBooks()
				{
					UserId = int.Parse(User.FindFirst("Id")!.Value),
					BookId = id
				});

				return Results.Ok(id);
			}
			catch (Exception ex)
			{
				return Results.BadRequest(ex.Message);
			}
		}

		[Authorize]
		[HttpPost("Remove")]
		public async Task<IResult> RemoveFromFav([FromBody] IDictionary<string, string> json)
		{
			try
			{
				int id = int.Parse(json["Id"]);

				var listSaved = await _provider.GetAll();

				var idRecords = from record in listSaved
								where record.UserId == int.Parse(User.FindFirst("Id")!.Value)
								&& record.BookId == id
								select record.Id;

				if (idRecords.Any())
					await _provider.Delete(idRecords.First());

				return Results.Ok(id);
			}
			catch (Exception ex)
			{
				return Results.BadRequest(ex.Message);
			}
		}
	}
}
