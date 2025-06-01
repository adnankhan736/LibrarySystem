using System;
using System.Linq;
using System.Threading.Tasks;
using Library.GrpcContracts;
using Xunit;

namespace Library.Service.Tests;
public class GrpcFunctionalTests : GrpcTestBase
{
    [Fact]
    public async Task GetMostBorrowedBooks_Returns_CorrectBooks()
    {
        // Act
        var response = await Client.GetMostBorrowedBooksAsync(new Empty());

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Books.Count >= 1);
        Assert.Contains(response.Books, b => b.Title == "C# in Depth");
    }

    [Fact]
    public async Task GetBookAvailability_Returns_CorrectValues()
    {
        // For BookId 1: TotalCopies = 10, Borrowed = 0 (all returned)
        var response1 = await Client.GetBookAvailabilityAsync(new BookRequest { BookId = 1 });
        Assert.Equal(0, response1.Borrowed);
        Assert.Equal(10, response1.Available);

        // For BookId 2: TotalCopies = 5, Borrowed = 1 (UserId 2 still borrowed)
        var response2 = await Client.GetBookAvailabilityAsync(new BookRequest { BookId = 2 });
        Assert.Equal(1, response2.Borrowed);
        Assert.Equal(4, response2.Available);
    }

    [Fact]
    public async Task GetTopBorrowers_Returns_UsersWithinDateRange()
    {
        var request = new DateRange
        {
            StartDate = "2025-01-01",
            EndDate = "2025-01-31"
        };

        var response = await Client.GetTopBorrowersAsync(request);

        Assert.NotEmpty(response.Users);
        Assert.Contains(response.Users, u => u.Name == "Alice");
        Assert.Contains(response.Users, u => u.Name == "Bob");
    }

    [Fact]
    public async Task GetUserBorrowedBooks_Returns_CorrectBooks()
    {
        var request = new UserDateRequest
        {
            UserId = 2,
            StartDate = "2025-01-01",
            EndDate = "2025-02-28"
        };

        var response = await Client.GetUserBorrowedBooksAsync(request);

        Assert.Equal(2, response.Books.Count);
        Assert.Contains(response.Books, b => b.Title == "C# in Depth");
        Assert.Contains(response.Books, b => b.Title == "ASP.NET Core Fundamentals");
    }

    [Fact]
    public async Task GetBooksBorrowedWith_Returns_OtherBooks()
    {
        // BookId 1 was borrowed by Alice and Bob. Bob also borrowed BookId 2.
        var response = await Client.GetBooksBorrowedWithAsync(new BookRequest { BookId = 1 });

        Assert.NotEmpty(response.Books);
        Assert.Contains(response.Books, b => b.Title == "ASP.NET Core Fundamentals");
    }

    [Fact]
    public async Task EstimateReadingRate_Returns_CorrectRate()
    {
        // BookId 1 has 900 pages, borrowed by Alice from 2025-01-01 to 2025-01-10 => 10 days
        var response = await Client.EstimateReadingRateAsync(new BorrowRecordRequest { BorrowRecordId = 1 });

        Assert.Equal(90, response.PagesPerDay); // 900 pages / 10 days
    }
}
