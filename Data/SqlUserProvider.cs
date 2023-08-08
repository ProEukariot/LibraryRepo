using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;
using LibraryApp.Models;

namespace LibraryApp.Data
{
	public class SqlUserProvider : ISqlDataProvider<User>
	{
		private readonly IConfiguration _config;

		public SqlUserProvider(IConfiguration config)
		{
			_config = config;
		}

		public async Task<User> Get(int id, string connection = "Default")
		{
			string cmd = @"SELECT * FROM Users u
							WHERE u.id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				return await con.QuerySingleAsync<User>(cmd, new { id });
			}
		}

		public async Task<IEnumerable<User>> GetAll(string connection = "Default")
		{
			string cmd = @"SELECT * FROM Users u;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				return await con.QueryAsync<User>(cmd);
			}
		}

		public async Task<int> Create(User user, string connection = "Default")
		{
			string cmd = @"INSERT INTO Users (Username, Password, Email, Role) 
							VALUES (@username, @password, @email, @role);
							SELECT @id = @@IDENTITY;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				var parameters = new DynamicParameters(new
				{
					username = user.Username,
					password = user.Password,
					email = user.Email,
					role = user.Role
				});
				parameters.Add("@id", -1, DbType.Int32, ParameterDirection.Output);

				await con.ExecuteAsync(cmd, parameters);

				return parameters.Get<int>("@id");
			}
		}

		public async Task Update(int id, [FromBody] User user, string connection = "Default")
		{
			string cmd = @"UPDATE Users (Username, Password, Email, Role) 
							SET (@username, @password, @email, @role)
							WHERE id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				await con.ExecuteAsync(cmd, new
				{
					id = id,
					username = user.Username,
					password = user.Password,
					email = user.Email,
					role = user.Role
				});
			}
		}

		public async Task Delete(int id, string connection = "Default")
		{
			string cmd = @"DELETE Users
							WHERE id = @id;";

			using (IDbConnection con = new SqlConnection(_config.GetConnectionString(connection)))
			{
				await con.ExecuteAsync(cmd, new { id });
			}
		}

		public async Task<IEnumerable<dynamic>> GetJoin<T>(string query, dynamic parameters, string connection = "Default")
		{
			using (IDbConnection con = new SqlConnection(connection))
			{
				var res = await con.QueryAsync<User, T, dynamic>(query, (book, obj) =>
				{
					return new { book, obj };
				});

				return res;
			}
		}
	}
}
