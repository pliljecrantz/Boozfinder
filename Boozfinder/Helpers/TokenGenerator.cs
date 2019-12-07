using System.Text;

namespace Boozfinder.Helpers
{
    public static class TokenGenerator
    {
        public static string Token()
        {
            var builder = new StringBuilder();
            builder.Append(RandomGenerator.RandomString(4, true));
            builder.Append(RandomGenerator.RandomNumber(10, 9999));
            builder.Append(RandomGenerator.RandomString(6, false));
            builder.Append(RandomGenerator.RandomNumber(100, 9999));
            builder.Append(RandomGenerator.RandomString(4, false));
            builder.Append(RandomGenerator.RandomNumber(1000, 9999));
            return builder.ToString();
        }
    }
}
