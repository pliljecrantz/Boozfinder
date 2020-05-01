using System;
using System.Text;

namespace Boozfinder.Helpers
{
    public static class TokenGenerator
    {
        public static string Token()
        {
            var builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(10, 9999));
            builder.Append(RandomString(6, false));
            builder.Append(RandomNumber(100, 9999));
            builder.Append(RandomString(4, false));
            builder.Append(RandomNumber(1000, 9999));
            return builder.ToString();
        }

        private static int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        private static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
            {
                builder.ToString().ToLower();
            }
            return builder.ToString();
        }
    }
}
