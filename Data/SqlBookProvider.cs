using Dapper;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using static System.Net.Mime.MediaTypeNames;

namespace LibraryApp.Data
{
	public class SqlBookProvider : ISqlDataProvider<Book>
	{
		private readonly IConfiguration _config;

		public SqlBookProvider(IConfiguration config)
		{
			_config = config;
		}

		public async Task<Book> Get(int id, string connection = "Default")
		{
			string cmd = @"SELECT * FROM Books b
							WHERE b.id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				return await con.QuerySingleAsync<Book>(cmd, new { id });
			}
		}

		public async Task<IEnumerable<Book>> GetAll(string connection = "Default")
		{
			string cmd = @"SELECT * FROM Books b;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				return await con.QueryAsync<Book>(cmd);
			}
		}

		public async Task<int> Create(Book book, string connection = "Default")
		{
			string cmd = @"INSERT INTO Books (Name, Author, Genre, Description, Image, FileData) 
							VALUES (@name, @author, @genre, @description, @image, @fileData);
							SELECT @id = @@IDENTITY;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				var parameters = new DynamicParameters(new
				{
					name = book.Name,
					author = book.Author,
					genre = book.Genre,
					description = book.Description,
					image = book.Image,
					fileData = book.FileData
				});
				parameters.Add("@id", -1, DbType.Int32, ParameterDirection.Output);

				await con.ExecuteAsync(cmd, parameters);

				return parameters.Get<int>("@id");
			}
		}

		public async Task Update(int id, [FromBody] Book book, string connection = "Default")
		{
			string cmd = @"UPDATE Books (Name, Author, Genre, Description, Image, FileData) 
							SET (@name, @author, @genre, @description, @image, @fileData)
							WHERE id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				await con.ExecuteAsync(cmd, new
				{
					id = id,
					name = book.Name,
					author = book.Author,
					genre = book.Genre,
					description = book.Description,
					image = book.Image,
					fileData = book.FileData
				});
			}
		}

		public async Task Delete(int id, string connection = "Default")
		{
			string cmd = @"DELETE Books
							WHERE id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				await con.ExecuteAsync(cmd, new { id });
			}
		}
	}
}
