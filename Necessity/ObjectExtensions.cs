using System.Collections.Generic;

namespace Necessity
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this T @in)
        {
            return new[] { @in };
        }

        public static T Is<T>(this object @in)
        {
            return (T)@in;
        }

        public static T As<T>(this object @in) where T : class
        {
            return @in as T;
        }
    }
}
