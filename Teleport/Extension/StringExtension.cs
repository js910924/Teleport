using System;
using System.Text;

namespace Teleport.Extension
{
    public static class StringExtension
    {
        public static string ToBase64String(this string x)
        {
            var bytes = Encoding.UTF8.GetBytes(x);

            return Convert.ToBase64String(bytes);
        }
    }
}