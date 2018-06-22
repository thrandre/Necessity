using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace Necessity.Tests
{
    [TestClass]
    public class DictionaryExtensions_GetOrDefault
    {
        [TestMethod]
        public void Should_return_assoc_value_if_key_is_present_and_default_otherwise()
        {
            var dictWithRefValue = new Dictionary<int, string>
            {
                {1, "test"}
            };

            var dictWithValueValue = new Dictionary<int, int>
            {
                {1, 1}
            };

            dictWithRefValue.GetOrDefault(1).Should().Be("test");
            dictWithRefValue.GetOrDefault(2).Should().BeNull();
            dictWithValueValue.GetOrDefault(1).Should().Be(1);
            dictWithValueValue.GetOrDefault(2).Should().Be(0);
        }
    }

    [TestClass]
    public class DictionaryExtensions_GetOrAdd
    {
        [TestMethod]
        public void Should_return_assoc_value_if_key_is_present_or_add_new_item_otherwise()
        {
            var dict = new Dictionary<int, string>
            {
                {1, "test"}
            };

            dict.GetOrAdd(1, key => "foo");
            dict.GetOrAdd(2, key => "foobar");

            dict.Should().Contain(new KeyValuePair<int, string>(1, "test"), new KeyValuePair<int, string>(2, "foobar"));
        }
    }

    [TestClass]
    public class DictionaryExtensions_AddOrUpdate
    {
        [TestMethod]
        public void Should_update_assoc_value_if_key_is_present_or_add_new_item_otherwise()
        {
            var dict = new Dictionary<int, string>
            {
                {1, "test"}
            };

            dict.AddOrUpdate(1, (key, oldValue) => "foo");
            dict.AddOrUpdate(2, (key, oldValue) => "foobar");

            dict.Should().Contain(new KeyValuePair<int, string>(1, "foo"), new KeyValuePair<int, string>(2, "foobar"));
        }
    }
}
