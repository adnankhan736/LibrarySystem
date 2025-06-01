using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class LibraryDbContextMockFactory
{
    public static async Task<LibraryDbContext> CreateAsync(
        List<Book> books = null,
        List<User> users = null,
        List<BorrowRecord> borrowRecords = null)
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: $"LibraryTestDb_{Guid.NewGuid()}")
            .Options;

        var context = new LibraryDbContext(options);

        if (books != null && books.Count > 0)
            context.Books.AddRange(books);

        if (users != null && users.Count > 0)
            context.Users.AddRange(users);

        if (borrowRecords != null && borrowRecords.Count > 0)
            context.BorrowRecords.AddRange(borrowRecords);

        await context.SaveChangesAsync();

        return context;
    }

    public static async Task DestroyAsync(LibraryDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }
}
