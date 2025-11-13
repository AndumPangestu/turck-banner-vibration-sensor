using FluentValidation.Results;
using ReminderManager.Domain.DTO;
using System.Net;

namespace ReminderManager.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
        public ResponseError<Dictionary<string, List<string>>> Failure { get; }

        public ValidationException(List<ValidationFailure> failures)
            :base("One or more validation error occured")
        {
            var errors = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToList()
                );

            Failure = new ResponseError<Dictionary<string, List<string>>>
            {
                Status = 422,
                Message = "Validation Error",
                Errors = errors
            };
        }
    }
}
