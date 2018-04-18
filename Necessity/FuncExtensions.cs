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

        public static Func<TIn, TOut2> Compose<TIn, TOut1, TOut2>(this Func<TIn, TOut1> fn1, Func<TOut1, TOut2> fn2)
        {
            return @in =>
                fn2(fn1(@in));
        }

        public static Func<TIn2, TOut> Partial<TIn1, TIn2, TOut>(this Func<TIn1, TIn2, TOut> target, TIn1 val)
        {
            return in2 => target(val, in2);
        }

        public static Action<TIn2> Partial<TIn1, TIn2>(this Action<TIn1, TIn2> target, TIn1 val)
        {
            return in2 => target(val, in2);
        }
    }
}
