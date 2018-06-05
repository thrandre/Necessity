using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming
namespace Necessity.Tests
{
    [TestClass]
    public class AsyncExtensions_MaterializeAsync
    {
        public Task<IEnumerable<int>> ProduceInts()
        {
            return Task.FromResult(Enumerable.Range(1, 10));
        }

        [TestMethod]
        public async Task Should_return_reference_when_possible_and_copy_otherwise()
        {
            var list1 = Task.FromResult<IEnumerable<int>>(new List<int> {1, 2, 3});
            var list2 = ProduceInts();

            var materialized1 = await list1.MaterializeAsync();
            var materialized2 = await list2.MaterializeAsync();

            (await list1).Should().BeSameAs(materialized1);
            (await list2).Should().NotBeSameAs(materialized2);
        }
    }
}
