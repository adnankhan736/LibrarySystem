using System;
using System.Collections.Generic;
using Library.Domain.Entities;

public class UserBorrowedBooksTestData
{
    // Test data for valid users with expected counts
    public static IEnumerable<object[]> GetValidUserData()
    {
        var users = new List<User>
        {
            new User { Id = 1, Name = "Alice" },
            new User { Id = 2, Name = "Bob" }
        };

        var books = new List<Book>
        {
            new Book { Id = 1, Title = "Book A" },
            new Book { Id = 2, Title = "Book B" }
        };

        var borrowRecords = new List<BorrowRecord>
        {
            new BorrowRecord
            {
                Id = 1,
                UserId = 1,
                BookId = 1,
                BorrowDate = new DateTime(2023, 2, 1),
                ReturnDate = new DateTime(2023, 2, 15),
                Book = books[0]
            },
            new BorrowRecord
            {
                Id = 2,
                UserId = 1,
                BookId = 2,
                BorrowDate = new DateTime(2023, 3, 1),
                ReturnDate = new DateTime(2023, 3, 10),
                Book = books[1]
            }
        };

        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        yield return new object[]
        {
            users,
            borrowRecords,
            1,
            startDate,
            endDate,
            2
        };

        yield return new object[]
        {
            users,
            borrowRecords,
            2,
            startDate,
            endDate,
            0
        };
    }

    // Test data for invalid users expecting exceptions
    public static IEnumerable<object[]> GetInvalidUserData()
    {
        var users = new List<User>
        {
            new User { Id = 1, Name = "Alice" }
        };

        var borrowRecords = new List<BorrowRecord>(); // no borrow records needed

        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);

        yield return new object[]
        {
            users,
            borrowRecords,
            999,  // invalid user ID
            startDate,
            endDate,
            "User with ID 999 not found."
        };
    }
}
