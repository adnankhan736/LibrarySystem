using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Contracts.Dtos.Response
{
    public class AvailabilityResponse
    {
        public int TotalCopies { get; set; }
        public int BorrowedCopies { get; set; }
        public int AvailableCopies { get; set; }
    }
}
