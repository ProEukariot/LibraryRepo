using System.Text;
using System.Security.Cryptography;
using System.Text.Unicode;

namespace LibraryApp.Extensions
{
    public static class Calc
    {
        public static string Hash(string str, string salt)
        {
            var sha256 = SHA256.Create();

            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str + salt));

            StringBuilder builder = new();

            foreach (var i in bytes)
            {
                builder.Append(i.ToString("x2"));
            }

            sha256.Dispose();

            return builder.ToString();
        }
    }
}
