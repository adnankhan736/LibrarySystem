using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Contracts.Dtos.Response
{
    public class BookListResponse
    {
        public List<BookResponse> Books { get; set; } = new();
    }
}
