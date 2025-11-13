using ReminderManager.Domain.DTO;
using System.Net;

namespace ReminderManager.Application.Exceptions
{
    public class ResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public ResponseError<Dictionary<string, string>> Failure { get; }

        public ResponseException(HttpStatusCode statusCode, string message, Dictionary<string, string>? errorDetails = null)
           : base(message)
        {
            StatusCode = statusCode;

            Failure = new ResponseError<Dictionary<string, string>>
            {
                Status = (int)statusCode,
                Message = message,
                Errors = errorDetails
            };
        }
    }
}
