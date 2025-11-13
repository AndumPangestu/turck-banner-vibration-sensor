using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderManager.Domain.DTO
{
    public class Pageable<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();

        public Pagination? Pagination { get; set; }
    }

    public class Pagination
    {
        public int CurrPage { get; set; }
        public int TotalPage { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
    }

}
