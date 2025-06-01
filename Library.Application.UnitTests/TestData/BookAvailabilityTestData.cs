using System.Collections.Generic;
using Library.Domain.Entities;

public class BookAvailabilityTestData
{
    public static IEnumerable<object[]> GetData()
    {
        yield return new object[]
        {
            new Book
            {
                Id = 1,
                Title = "Book A",
                TotalCopies = 5,
                BorrowRecords = new List<BorrowRecord>
                {
                    new BorrowRecord { ReturnDate = null },  // borrowed
                    new BorrowRecord { ReturnDate = DateTime.UtcNow }, // returned
                    new BorrowRecord { ReturnDate = null },  // borrowed
                }
            },
            2, // expected borrowed count
            3  // expected available count
        };

        yield return new object[]
        {
            new Book
            {
                Id = 2,
                Title = "Book B",
                TotalCopies = 3,
                BorrowRecords = new List<BorrowRecord>()
            },
            0,
            3
        };
    }
}
