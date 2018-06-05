using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming
namespace Necessity.Tests
{
    [TestClass]
    public class EnumerableExtensions_Materialize
    {
        public IEnumerable<int> ProduceInts()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        [TestMethod]
        public void Should_return_reference_when_possible_and_copy_otherwise()
        {
            IEnumerable<int> list1 = new List<int> {1, 2, 3};
            IEnumerable<int> list2 = ProduceInts();

            var materializedList1 = list1.Materialize();
            var materializedList2 = list2.Materialize();

            materializedList1.Should().BeSameAs(list1);
            materializedList2.Should().NotBeSameAs(list2);
        }
    }
}
