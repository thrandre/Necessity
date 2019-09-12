using System;
using System.Threading.Tasks;

namespace Necessity
{
    public static class FuncExtensions
    {
        public static Func<TOut> Fn<TOut>(Delegate test)
        {
            return (Func<TOut>)test;
        }

        public static Func<TIn, TOut> Fn<TIn, TOut>(Func<TIn, TOut> fn)
        {
            return fn;
        }

        public static Func<TIn1, TIn2, TOut> Fn<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> fn)
        {
            return fn;
        }

        public static Func<TIn1, TIn2, TIn3, TOut> Fn<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, TOut> fn)
        {
            return fn;
        }

        public static Func<TIn1, TIn2, TIn3, TIn4, TOut> Fn<TIn1, TIn2, TIn3, TIn4, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TOut> fn)
        {
            return fn;
        }

        public static Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> Fn<TIn1, TIn2, TIn3, TIn4, TIn5, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> fn)
        {
            return fn;
        }

        public static TOut Pipe<TIn, TOut>(this TIn @in, Func<TIn, TOut> func)
        {
            return func(@in);
        }

        public static TIn Pipe<TIn>(this TIn @in, Action<TIn> act)
        {
            act(@in);
            return @in;
        }

        public static async Task<TOut> Pipe<TIn, TOut>(this Task<TIn> @in, Func<TIn, TOut> func)
        {
            return func(await @in);
        }

        public static async Task<TIn> Pipe<TIn>(this Task<TIn> @in, Action<TIn> act)
        {
            var result = await @in;
            act(result);
            return result;
        }

        public static TIn PipeIf<TIn>(this TIn target, Func<TIn, bool> predicate, Action<TIn> fn)
        {
            return predicate(target)
                ? target.Pipe(fn)
                : target;
        }

        public static TIn PipeIf<TIn>(this TIn target, Func<TIn, bool> predicate, Func<TIn, TIn> fn)
        {
            return predicate(target)
                ? target.Pipe(fn)
                : target;
        }

        public static TOut Fork<TIn, TOut>(this TIn target, Func<TIn, bool> predicate, Func<TIn, TOut> @if, Func<TIn, TOut> @else)
        {
            return predicate(target)
                ? target.Pipe(@if)
                : target.Pipe(@else);
        }

        public static Action<TIn> Compose<TIn>(this Action<TIn> act1, Action<TIn> act2)
        {
            return @in =>
            {
                act1?.Invoke(@in);
                act2?.Invoke(@in);
            };
        }

        public static Func<TIn, TOut2> Compose<TIn, TOut1, TOut2>(this Func<TIn, TOut1> fn1, Func<TOut1, TOut2> fn2)
        {
            return @in =>
                fn2(fn1(@in));
        }

        public static Func<TOut> Partial<TIn, TOut>(this Func<TIn, TOut> target, TIn val)
        {
            return () => target(val);
        }

        public static Func<TIn2, TOut> Partial<TIn1, TIn2, TOut>(this Func<TIn1, TIn2, TOut> target, TIn1 val)
        {
            return in2 => target(val, in2);
        }

        public static Func<TIn2, TIn3, TOut> Partial<TIn1, TIn2, TIn3, TOut>(this Func<TIn1, TIn2, TIn3, TOut> target, TIn1 val)
        {
            return (in2, in3) => target(val, in2, in3);
        }
    }
}
