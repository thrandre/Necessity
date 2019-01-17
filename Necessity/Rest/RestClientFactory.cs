using System;
using System.Net.Http;
using Necessity.Serialization.Abstractions;

namespace Necessity.Rest
{
    public class RestClientFactory : IRestClientFactory
    {
        public RestClientFactory(Func<HttpClient> getHttpClient, ISerializer serializer)
        {
            GetHttpClient = getHttpClient;
            Serializer = serializer;
        }

        public Func<HttpClient> GetHttpClient { get; }
        public ISerializer Serializer { get; }

        public RestClient Create(Action<HttpRequestMessage> commonConfigure = null)
        {
            return new RestClient(GetHttpClient, Serializer)
            {
                CommonConfigure = commonConfigure
            };
        }
    }
}