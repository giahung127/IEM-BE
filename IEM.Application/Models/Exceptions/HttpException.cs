using System.Net;

namespace IEM.Application.Models.Exceptions
{
    public class HttpException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpException(HttpStatusCode statusCode) : base()
        {
            StatusCode = statusCode;
        }
    }
}
