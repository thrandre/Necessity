using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

// ReSharper disable InconsistentNaming
namespace Necessity.Tests
{
    [TestClass]
    public class UriExtensions_AppendPath
    {
        [TestMethod]
        public void Should_append_path_to_URI()
        {
            new Uri("https://consul.io/v1").AppendPath("kv").AppendPath("foo").AppendPath("bar")
                .ToString().Should().Be("https://consul.io/v1/kv/foo/bar");
        }
    }

    [TestClass]
    public class UriExtensions_AppendQueryStringParameters
    {
        [TestMethod]
        public void Should_append_path_to_URI()
        {
            new Uri("https://consul.io/v1").AppendPath("kv").AppendPath("foo").AppendPath("bar")
                .AppendQueryStringParameters(new { foobar = 42 })
                .ToString().Should().Be("https://consul.io/v1/kv/foo/bar?foobar=42");
        }
    }
}
