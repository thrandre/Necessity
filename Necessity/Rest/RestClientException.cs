using System;
using System.Net;

namespace Necessity.Rest
{
    public class RestClientException : Exception
    {
        public RestClientException(HttpStatusCode statusCode, string errorMessage) : base($"{statusCode}: {errorMessage}")
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public HttpStatusCode StatusCode { get; }
        public string ErrorMessage { get; }
    }
}