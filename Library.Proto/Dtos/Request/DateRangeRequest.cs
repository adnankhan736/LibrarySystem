using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Contracts.Dtos.Request
{
    public class DateRangeRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
