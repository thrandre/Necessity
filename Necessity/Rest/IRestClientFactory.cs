using System;
using System.Net.Http;

namespace Necessity.Rest
{
    public interface IRestClientFactory
    {
        IRestClient Create(Action<HttpRequestMessage> commonConfigure = null);
    }
}