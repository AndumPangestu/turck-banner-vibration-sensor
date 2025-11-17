using ReminderManager.Domain.DTO;


namespace ReminderManager.Application.Common.Helpers
{
    public static class ResponseHelper
    {
        public static ResponseSuccess<T> SendSuccess<T>(T data, int status = 200, string message = "Success", Pagination? pagination = null)
        {
            return new ResponseSuccess<T>
            {
                Status = status,
                Message = message,
                Data = data,
                Pagination = pagination
            };
        }

        public static ResponseError<T> SendError<T>(T errors, int status = 500, string message = "Internal Server Error")
        {
            return new ResponseError<T>
            {
                Status = status,
                Message = message,
                Errors = errors
            };
        }
    }
}

