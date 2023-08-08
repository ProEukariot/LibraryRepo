using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Data
{
	public interface ISqlDataProvider<T>
	{
		Task<int> Create(T target, string connection = "Default");
		Task Delete(int id, string connection = "Default");
		Task<T> Get(int id, string connection = "Default");
		Task<IEnumerable<T>> GetAll(string connection = "Default");
		Task Update(int id, [FromBody] T target, string connection = "Default");
		//Task<IEnumerable<T>> GetJoin(string query, dynamic parameters, string connection = "Default");	
	}
}