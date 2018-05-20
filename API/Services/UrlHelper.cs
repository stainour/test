using System;

namespace API.Services
{
    internal class UrlHelper
    {
        public static readonly string Domain = "cooldomain.org";
        public static readonly string Sheme = "https";

        public static string AddShemeAndDomain(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return $"{Sheme}://{Domain}/{key}";
        }

        public static (bool isValid, string key) IsUrlValid(string url)
        {
            if (url == null) return (false, null);

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && uri.Scheme == Sheme && uri.Host == Domain)
            {
                return (true, uri.PathAndQuery.TrimStart('/'));
            }
            return (false, null);
        }
    }
}