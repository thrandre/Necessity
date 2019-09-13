namespace Necessity
{
    public static class ValueExtensions
    {
        public static T? NullWhenDefault<T>(this T? target) where T : struct
        {
            return !target.Equals(default(T))
                ? target
                : (T?)null;
        }

        public static T? NullWhenDefault<T>(this T target) where T : struct
        {
            return !target.Equals(default(T))
                ? target
                : (T?)null;
        }
    }
}
