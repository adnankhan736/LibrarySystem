using Grpc.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Application.IntegrationTests;
public class LibraryCoreServiceIntegrationTests : LibraryCoreServiceTestBase
{
    [Fact]
    public async Task GetMostBorrowedBooksAsync_ReturnsTop10Books()
    {
        var books = await Service.GetMostBorrowedBooksAsync();

        Assert.NotNull(books);
        Assert.True(books.Any());
        Assert.True(books.Count() <= 10);

        // Book with Id=1 has 2 borrow records, so it should be first
        Assert.Equal(1, books.First().Id);
    }

    [Fact]
    public async Task GetBookAvailabilityAsync_ReturnsCorrectAvailability()
    {
        var (borrowed, available) = await Service.GetBookAvailabilityAsync(1);

        // Book 1 has 10 copies, 2 borrowed not returned
        Assert.Equal(0, borrowed); // both records returned
        Assert.Equal(10, available);

        // Check book 2 (has 1 borrowed not returned)
        var (borrowed2, available2) = await Service.GetBookAvailabilityAsync(2);
        Assert.Equal(1, borrowed2);
        Assert.Equal(4, available2);
    }

    [Fact]
    public async Task GetBookAvailabilityAsync_ThrowsRpcException_WhenBookNotFound()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.GetBookAvailabilityAsync(999));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetTopBorrowersAsync_ReturnsTopBorrowers()
    {
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 1, 31);

        var users = await Service.GetTopBorrowersAsync(startDate, endDate);

        Assert.NotNull(users);
        Assert.Contains(users, u => u.Id == 1 || u.Id == 2);
    }

    [Fact]
    public async Task GetTopBorrowersAsync_ThrowsRpcException_WhenNoBorrowers()
    {
        var startDate = new DateTime(2030, 1, 1);
        var endDate = new DateTime(2030, 12, 31);

        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.GetTopBorrowersAsync(startDate, endDate));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowedBooksAsync_ReturnsBooks()
    {
        var startDate = new DateTime(2025, 1, 1);
        var endDate = new DateTime(2025, 12, 31);

        var books = await Service.GetUserBorrowedBooksAsync(2, startDate, endDate);

        Assert.NotNull(books);
        Assert.Contains(books, b => b.Id == 1 || b.Id == 2);
    }

    [Fact]
    public async Task GetUserBorrowedBooksAsync_ThrowsRpcException_WhenUserNotFound()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.GetUserBorrowedBooksAsync(999, DateTime.Now, DateTime.Now));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowedBooksAsync_ThrowsRpcException_WhenNoBooks()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.GetUserBorrowedBooksAsync(1, new DateTime(2030, 1, 1), new DateTime(2030, 12, 31)));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetBooksBorrowedWithAsync_ReturnsAssociatedBooks()
    {
        var books = await Service.GetBooksBorrowedWithAsync(1);

        Assert.NotNull(books);
        Assert.Contains(books, b => b.Id == 2);
    }

    [Fact]
    public async Task GetBooksBorrowedWithAsync_ThrowsRpcException_WhenBookNotFound()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.GetBooksBorrowedWithAsync(999));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task GetBooksBorrowedWithAsync_ThrowsRpcException_WhenNoBorrowRecords()
    {
        // Add a book with no borrow records
        Context.Books.Add(new Library.Domain.Entities.Book { Id = 99, Title = "No Borrows", TotalCopies = 1, Pages = 100 });
        Context.SaveChanges();

        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.GetBooksBorrowedWithAsync(99));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task EstimateReadingRateAsync_ReturnsCorrectRate()
    {
        var rate = await Service.EstimateReadingRateAsync(1);

        // BorrowDate 2025-01-01 to 2025-01-10 inclusive = 10 days
        // Pages = 900, rate = 900 / 10 = 90
        Assert.Equal(90, rate);
    }

    [Fact]
    public async Task EstimateReadingRateAsync_ThrowsRpcException_WhenRecordNotFound()
    {
        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.EstimateReadingRateAsync(999));
        Assert.Equal(StatusCode.NotFound, ex.StatusCode);
    }

    [Fact]
    public async Task EstimateReadingRateAsync_ThrowsRpcException_WhenReturnDateNull()
    {
        // BorrowRecord Id 3 has null ReturnDate
        var ex = await Assert.ThrowsAsync<RpcException>(() => Service.EstimateReadingRateAsync(3));
        Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
    }
}
