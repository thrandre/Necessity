using System;
using System.Net.Http;
using System.Threading.Tasks;
using Necessity.Serialization.Abstractions;

namespace Necessity.Rest
{
    public interface IRestClient
    {
        Action<HttpRequestMessage> CommonConfigure { get; set; }
        Task<T> Request<T>(string path, Action<HttpRequestMessage> configureRequest, Func<HttpResponseMessage, ISerializer, Task<T>> onSuccess);
        Task<T> Get<T>(string path, Action<HttpRequestMessage> configureRequest = null, T anonymousObjectPrototype = default);
        Task Post(string path, Action<HttpRequestMessage> configureRequest = null);
        Task Put(string path, Action<HttpRequestMessage> configureRequest = null);
        Task Delete(string path, Action<HttpRequestMessage> configureRequest = null);
    }
}