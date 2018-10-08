using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Necessity.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<TResult> RequestAsync<TResult>(
            this HttpClient httpClient,
            Action<HttpRequestMessage> configureRequest,
            Func<HttpResponseMessage, Task<TResult>> onSuccess,
            Func<HttpResponseMessage, Task> onError)
        {
            var result = await httpClient
                .SendAsync(
                    new HttpRequestMessage().Pipe(configureRequest),
                    HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            if (!result.IsSuccessStatusCode)
            {
                await onError(result);
                return default;
            }

            using (result)
            {
                return await onSuccess(result);
            }
        }
    }
}
