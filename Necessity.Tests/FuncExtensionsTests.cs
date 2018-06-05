using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Necessity.FuncExtensions;

// ReSharper disable InconsistentNaming
namespace Necessity.Tests
{
    public class Test
    {
        public string TestProperty { get; set; }
    }

    [TestClass]
    public class FuncExtensions_Pipe
    {
        [TestMethod]
        public void Should_apply_action_to_the_current_instance_and_return_the_original()
        {
            var testInstance = new Test();

            var pipedInstance = testInstance.Pipe(x => { x.TestProperty = "foo"; });

            pipedInstance.Should().BeSameAs(testInstance);
            pipedInstance.TestProperty.Should().Be("foo");
        }

        [TestMethod]
        public void Should_apply_function_to_the_current_instance_and_return_the_result()
        {
            var testInstance = new Test {TestProperty = "test1"};

            var pipedInstance = testInstance.Pipe(x => new Test { TestProperty = "test2" });

            pipedInstance.Should().NotBeSameAs(testInstance);
            pipedInstance.TestProperty.Should().Be("test2");
        }
    }

    [TestClass]
    public class FuncExtensions_Compose
    {
        [TestMethod]
        public void Should_compose_functions()
        {
            var fn1 = Fn((string x) => x + "1");
            var fn2 = Fn((string x) => x + "2");
            var fn3 = fn1.Compose(fn2);

            var res = fn3("hello");

            res.Should().Be("hello12");
        }
    }

    [TestClass]
    public class FuncExtensions_Partial
    {
        public static string Concat(string x1, string x2, string x3)
        {
            return x1 + " " + x2 + x3;
        }

        [TestMethod]
        public void Should_partially_apply_functions()
        {
            var fn1 = Fn((string x1, string x2) => x1 + " " + x2);
            var fn2 = Fn<string, string, string, string>(Concat);

            var partialfn1 = fn1.Partial("hello");
            var partialfn2 = fn2.Partial("hello").Partial("world").Partial("!");

            var res1 = partialfn1("world");
            var res2 = partialfn2();

            res1.Should().Be("hello world");
            res2.Should().Be("hello world!");
        }
    }
}
