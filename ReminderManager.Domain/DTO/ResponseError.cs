

namespace ReminderManager.Domain.DTO
{
    public class ResponseError <T>
    {
        public required int Status { get; set; }
        public required string Message { get; set; }
        public T? Errors { get; set; }
    }
}
