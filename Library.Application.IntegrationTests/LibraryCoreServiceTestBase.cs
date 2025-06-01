using Library.Application.Services;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;
using Xunit;

public abstract class LibraryCoreServiceTestBase : IDisposable
{
    protected readonly LibraryDbContext Context;
    protected readonly LibraryCoreService Service;

    public LibraryCoreServiceTestBase()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: $"LibraryIntegrationTestDb_{Guid.NewGuid()}")
            .Options;

        Context = new LibraryDbContext(options);

        // Apply seeding
        Context.Database.EnsureCreated();
        var modelBuilder = new ModelBuilder();
        DataSeeder.Seed(modelBuilder);
        Context.SaveChanges();

        Service = new LibraryCoreService(Context);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
