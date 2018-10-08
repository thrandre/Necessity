using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Collections.Generic;

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

    [TestClass]
    public class UriExtensions_Parse
    {
        [TestMethod]
        public void Should_parse_uri_correctly()
        {
            var res = new Uri("https://cg2.dev.service.esmartapi.com/landing/esmart_dev/1?redirectUrl=https://www.vg.no?redirectUrl=https://www.db.no")
                .Parse("/landing/{tenantKey}/{participantKey}");

            res.Base.Should().Be("https://cg2.dev.service.esmartapi.com");
            res.Path.Should().Be("/landing/esmart_dev/1");

            res.UriParams.Should().Contain(
                new KeyValuePair<string, string>("tenantKey", "esmart_dev"),
                new KeyValuePair<string, string>("participantKey", "1"));

            res.QueryParams.Should().Contain(
                new KeyValuePair<string, string>("redirectUrl", "https://www.vg.no"));

            res.QueryParams.Should().Contain(
                new KeyValuePair<string, string>("redirectUrl_1", "https://www.db.no"));
        }
    }
}
