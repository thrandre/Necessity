using System;
using System.Net.Http;

namespace Necessity.Rest
{
    public interface IRestClientFactory
    {
        RestClient Create(Action<HttpRequestMessage> commonConfigure = null);
    }
}