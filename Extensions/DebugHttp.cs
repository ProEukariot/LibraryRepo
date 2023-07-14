
namespace LibraryApp.Extensions
{
	public static class DebugHttp
	{
		public static async Task<string> ReadBody(Stream body)
		{
			string result = "";

			using (StreamReader reader = new StreamReader(body))
			{
				result = await reader.ReadToEndAsync();
			}

			return result;
		}
	}
}
