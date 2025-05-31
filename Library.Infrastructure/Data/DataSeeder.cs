using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public static class DataSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Alice" },
            new User { Id = 2, Name = "Bob" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "C# in Depth", TotalCopies = 10, Pages = 900 },
            new Book { Id = 2, Title = "ASP.NET Core Fundamentals", TotalCopies = 5, Pages = 600 }
        );

        modelBuilder.Entity<BorrowRecord>().HasData(
            new BorrowRecord
            {
                Id = 1,
                BookId = 1,
                UserId = 1,
                BorrowDate = new DateTime(2025, 1, 1),
                ReturnDate = new DateTime(2025, 1, 10)
            },
            new BorrowRecord
            {
                Id = 2,
                BookId = 1,
                UserId = 2,
                BorrowDate = new DateTime(2025, 1, 5),
                ReturnDate = new DateTime(2025, 1, 11)
            },
            new BorrowRecord
            {
                Id = 3,
                BookId = 2,
                UserId = 2,
                BorrowDate = new DateTime(2025, 2, 1),
                ReturnDate = null
            }
        );
    }
}
