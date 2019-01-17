using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Necessity.Rest
{
    public interface IRestClient
    {
        Task<T> Request<T>(Action<HttpRequestMessage> configureRequest, Func<HttpResponseMessage, Task<T>> onSuccess);
        Task<T> Get<T>(string path, Action<HttpRequestMessage> configureRequest = null);
        Task Post(string path, Action<HttpRequestMessage> configureRequest = null);
        Task Put(string path, Action<HttpRequestMessage> configureRequest = null);
        Task Delete(string path, Action<HttpRequestMessage> configureRequest = null);
    }
}