using System;
using System.Collections.Generic;
using Grpc.Core;
using Library.Domain.Entities;

public class EstimateReadingRateTestData
{
    public static IEnumerable<object[]> GetValidData()
    {
        yield return new object[]
        {
            new List<Book>
            {
                new Book { Id = 1, Pages = 150, Title = "Book A", TotalCopies = 1 }
            },
            new List<BorrowRecord>
            {
                new BorrowRecord
                {
                    Id = 1,
                    BookId = 1,
                    BorrowDate = new DateTime(2023, 1, 1),
                    ReturnDate = new DateTime(2023, 1, 6)
                }
            },
            1, // BorrowRecordId
            25.0 // Expected reading rate
        };
    }

    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return new object[]
        {
            new List<Book>
            {
                new Book { Id = 2, Pages = 100, Title = "Book B", TotalCopies = 1 }
            },
            new List<BorrowRecord>
            {
                new BorrowRecord
                {
                    Id = 2,
                    BookId = 2,
                    BorrowDate = new DateTime(2023, 1, 2),
                    ReturnDate = new DateTime(2023, 1, 1)
                }
            },
            2,
            StatusCode.InvalidArgument,
            "Return date must be after borrow date."
        };

        yield return new object[]
        {
            new List<Book>
            {
                new Book { Id = 3, Pages = 100, Title = "Book C", TotalCopies = 1 }
            },
            new List<BorrowRecord>
            {
                new BorrowRecord
                {
                    Id = 3,
                    BookId = 3,
                    BorrowDate = new DateTime(2023, 1, 1),
                    ReturnDate = null
                }
            },
            3,
            StatusCode.InvalidArgument,
            "Return date is required."
        };
    }
}
