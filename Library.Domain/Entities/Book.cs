using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TotalCopies { get; set; }
        public int Pages { get; set; }
        public ICollection<BorrowRecord> BorrowRecords { get; set; }
    }
}
