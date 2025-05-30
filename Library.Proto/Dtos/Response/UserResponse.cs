using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Contracts.Dtos.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BorrowCount { get; set; }
    }

}
