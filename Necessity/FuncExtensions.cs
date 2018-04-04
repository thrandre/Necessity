using System;

namespace Necessity
{
    public static class FuncExtensions
    {
        public static TOut Pipe<TIn, TOut>(this TIn @in, Func<TIn, TOut> func)
        {
            return func(@in);
        }

        public static TIn Pipe<TIn>(this TIn @in, Action<TIn> act)
        {
            act(@in);
            return @in;
        }
    }
}
