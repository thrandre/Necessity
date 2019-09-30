using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

// ReSharper disable InconsistentNaming
namespace Necessity.Tests
{
    [TestClass]
    public class UrlExtensions_Append
    {
        [TestMethod]
        public void Should_append_path_to_URI()
        {
            UrlInfo.Parse("https://consul.io/v1").Append("kv").Append("foo").Append("bar")
                .ToString().Should().Be("https://consul.io/v1/kv/foo/bar");
        }
    }

    [TestClass]
    public class UrlExtensions_AppendQueryParameters
    {
        [TestMethod]
        public void Should_append_path_to_URI()
        {
            UrlInfo.Parse("https://consul.io/v1").Append("kv").Append("foo").Append("bar")
                .AppendQueryParameters(new { foobar = 42 })
                .ToString().Should().Be("https://consul.io/v1/kv/foo/bar?foobar=42");
        }

        [TestMethod]
        public void Should_append_path_to_relative_URI()
        {
            UrlInfo.Parse("users").Append("kv").Append("foo").Append("bar")
                .AppendQueryParameters(new { foobar = 42 })
                .ToString().Should().Be("users/kv/foo/bar?foobar=42");
        }

        [TestMethod]
        public void Should_append_path_to_relative_URI_while_removing_leading_and_trailing_slashes()
        {
            UrlInfo.Parse("/users").Append("kv").Append("foo").Append("bar/")
                .AppendQueryParameters(new { foobar = 42 })
                .ToString().Should().Be("users/kv/foo/bar?foobar=42");
        }
    }

    [TestClass]
    public class UrlExtensions_PracticalTests
    {
        [TestMethod]
        public void Should_append_base_path_to_relative_url()
        {
            var url = UrlInfo.Parse("users").Append("http://www.vg.no").ToString();
            url.Should().Be("http://www.vg.no/users");
        }

        [TestMethod]
        public void Should_append_base_path_to_query_string()
        {
            var url = new UrlInfo().AppendQueryParameters(new { foo = "bar", bar = "baz" }).Append("http://www.vg.no#frag").ToString();
            url.Should().Be("http://www.vg.no?foo=bar&bar=baz#frag");
        }
    }
}
