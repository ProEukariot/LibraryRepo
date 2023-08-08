using Dapper;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using static System.Net.Mime.MediaTypeNames;

namespace LibraryApp.Data
{
	public class SqlSavedBooksProvider : ISqlDataProvider<SavedBooks>
	{
		private readonly IConfiguration _config;

		public SqlSavedBooksProvider(IConfiguration config)
		{
			_config = config;
		}

		public async Task<SavedBooks> Get(int id, string connection = "Default")
		{
			string cmd = @"SELECT * FROM SavedBooks sb
							WHERE sb.id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				return await con.QuerySingleAsync<SavedBooks>(cmd, new { id });
			}
		}

		public async Task<IEnumerable<SavedBooks>> GetAll(string connection = "Default")
		{
			string cmd = @"SELECT * FROM SavedBooks sb;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				return await con.QueryAsync<SavedBooks>(cmd);
			}
		}

		public async Task<int> Create(SavedBooks favorites, string connection = "Default")
		{
			string cmd = @"INSERT INTO SavedBooks (BookId, UserId) 
							VALUES (@bookId, @userId);
							SELECT @id = @@IDENTITY;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				var parameters = new DynamicParameters(new
				{
					bookId = favorites.BookId,
					userId = favorites.UserId
				});
				parameters.Add("@id", -1, DbType.Int32, ParameterDirection.Output);

				await con.ExecuteAsync(cmd, parameters);

				return parameters.Get<int>("@id");
			}
		}

		public async Task Update(int id, [FromBody] SavedBooks favorites, string connection = "Default")
		{
			string cmd = @"UPDATE SavedBooks (UserId, BookId) 
							SET (@bookId, @userId)
							WHERE id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				await con.ExecuteAsync(cmd, new
				{
					id = favorites.Id,
					bookId = favorites.BookId,
					userId = favorites.UserId
				});
			}
		}

		public async Task Delete(int id, string connection = "Default")
		{
			string cmd = @"DELETE SavedBooks
							WHERE id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				await con.ExecuteAsync(cmd, new { id });
			}
		}
	}
}
