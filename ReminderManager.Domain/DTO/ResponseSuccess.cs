using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.DTO
{
    public class ResponseSuccess <T>
    {
        public required int Status { get; set; }
        public required string Message { get; set; }
        public required T Data { get; set; }
    }
}
