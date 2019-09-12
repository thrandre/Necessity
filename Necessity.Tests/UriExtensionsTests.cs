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
                .AppendQueryParameters(new { foobar = 42 })
                .ToString().Should().Be("https://consul.io/v1/kv/foo/bar?foobar=42");
        }

        [TestMethod]
        public void Should_append_path_to_relative_URI()
        {
            new Uri("users", UriKind.Relative).AppendPath("kv").AppendPath("foo").AppendPath("bar")
                .AppendQueryParameters(new { foobar = 42 })
                .ToString().Should().Be("/users/kv/foo/bar?foobar=42");
        }

        [TestMethod]
        [Ignore]
        public void Should_handle_nested_querystrings()
        {
            new Uri("https://portal.dev.service.esmartapi.com")
                .AppendQueryParameters(new { redirectUrl = UriExtensions.EncodeUrlFragment("http://localhost:44301/landing/{{tenantKey}}/{{participantKey}}?redirectUrl=/") })
                .ToString().Should().Be($"https://portal.dev.service.esmartapi.com/?redirectUrl={UriExtensions.EncodeUrlFragment("http://localhost:44301/landing/{{tenantKey}}/{{participantKey}}?redirectUrl=/")}");
        }
    }

    [TestClass]
    public class UriExtensions_Parse
    {
        [TestMethod]
        [Ignore]
        public void Should_parse_uri_correctly()
        {
            var res = new Uri($"https://cg2.dev.service.esmartapi.com/landing/esmart_dev/1?redirectUrl={UriExtensions.EncodeUrlFragment("https://www.vg.no?redirectUrl=https://www.db.no")}")
                .Parse("/landing/{tenantKey}/{participantKey}");

            res.BasePath.Should().Be("https://cg2.dev.service.esmartapi.com");
            res.RelativePath.Should().Be("/landing/esmart_dev/1");

            res.UriParams.Should().Contain(
                new KeyValuePair<string, string>("tenantKey", "esmart_dev"),
                new KeyValuePair<string, string>("participantKey", "1"));

            res.QueryParams.Should().Contain(
                new KeyValuePair<string, string>("redirectUrl", UriExtensions.EncodeUrlFragment("https://www.vg.no?redirectUrl=https://www.db.no")));
        }
    }
}
