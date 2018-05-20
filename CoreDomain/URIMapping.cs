using System;
using System.Text;

namespace CoreDomain
{
    public class UriMapping
    {
        private const string Alphabet = "i8kXgbjRKvEh6M7UsVaSdAJpw59cuZnBLrPNoDzmfHxIG1lYCyFQ23qWOe0T4t";
        private static readonly long Base = 62;

        public UriMapping(Uri uri, long uniqueNumber)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("URI should be absolute!", nameof(uri));
            }
            if (uniqueNumber <= 0) throw new ArgumentOutOfRangeException(nameof(uniqueNumber));

            ShortenedKey = ConvertToBase62(uniqueNumber);
            HitCount = 0;
            Uri = uri.ToString();
        }

        private UriMapping()
        {
        }

        public long HitCount { get; private set; }

        public string ShortenedKey { get; private set; }

        public string Uri { get; private set; }

        private string ConvertToBase62(long value)
        {
            var stringBuilder = new StringBuilder();
            while (value > 0)
            {
                stringBuilder.Insert(0, Alphabet[(int)(value % Base)]);
                value /= Base;
            }
            return stringBuilder.ToString();
        }
    }
}