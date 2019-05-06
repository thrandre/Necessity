namespace Necessity
{
    public static class StringExtensions
    {
        public static string Append(this string target, string appendable)
        {
            return target + appendable;
        }

        public static string Prepend(this string target, string prependable)
        {
            return prependable + target;
        }
    }
}