using System.Collections.Generic;
using Library.Domain.Entities;

public static class BooksBorrowedWithTestData
{
    // Valid test cases: (books, borrowRecords, bookId, expectedCount)
    public static IEnumerable<object[]> GetValidData()
    {
        yield return new object[]
        {
            // books
            new List<Book>
            {
                new Book { Id = 1, Title = "Book A" },
                new Book { Id = 2, Title = "Book B" },
                new Book { Id = 3, Title = "Book C" }
            },
            // borrowRecords
            new List<BorrowRecord>
            {
                new BorrowRecord { UserId = 1, BookId = 1 },
                new BorrowRecord { UserId = 1, BookId = 2 },
                new BorrowRecord { UserId = 2, BookId = 1 },
                new BorrowRecord { UserId = 2, BookId = 3 }
            },
            // bookId to test
            1,
            // expected associated books count (2 and 3)
            2
        };
    }

    // Invalid test cases: (books, borrowRecords, bookId, expectedExceptionMessage)
    public static IEnumerable<object[]> GetInvalidData()
    {
        yield return new object[]
        {
            // books - empty, so bookId not found
            new List<Book>(),
            // borrowRecords
            new List<BorrowRecord>(),
            // bookId to test
            1,
            // expected exception message
            "Book with ID 1 not found."
        };

        yield return new object[]
        {
            // books contains the book, but no borrow records
            new List<Book> { new Book { Id = 1, Title = "Book A" } },
            new List<BorrowRecord>(),
            1,
            "No borrow records found for book ID 1."
        };

        yield return new object[]
        {
            // books with one book
            new List<Book> { new Book { Id = 1, Title = "Book A" }, new Book { Id = 2, Title = "Book B" } },
            // borrowRecords where users only borrowed bookId, no other books
            new List<BorrowRecord>
            {
                new BorrowRecord { UserId = 1, BookId = 1 }
            },
            1,
            "No associated books found with book ID 1."
        };
    }
}
