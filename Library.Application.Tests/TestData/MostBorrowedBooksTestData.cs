using System.Collections.Generic;
using Library.Domain.Entities;

public class MostBorrowedBooksTestData
{
    public static IEnumerable<object[]> GetData()
    {
        yield return new object[]
        {
            new List<Book>
            {
                new Book { Id = 1, Title = "Book A", BorrowRecords = new List<BorrowRecord> { new(), new() } },
                new Book { Id = 2, Title = "Book B", BorrowRecords = new List<BorrowRecord> { new() } },
                new Book { Id = 3, Title = "Book C", BorrowRecords = new List<BorrowRecord> { new(), new(), new() } },
            }
        };
    }
}
