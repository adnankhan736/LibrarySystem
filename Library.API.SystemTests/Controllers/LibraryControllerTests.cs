using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Library.API.SystemTests.Mocks;
using Library.GrpcContracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Library.API.SystemTests;

public class LibraryControllerSystemTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly Mock<LibraryService.LibraryServiceClient> _mockGrpcClient;

    public LibraryControllerSystemTests(TestWebApplicationFactory factory)
    {
        _mockGrpcClient = factory.GrpcClientMock;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMostBorrowedBooks_ReturnsBooks()
    {
        var response = await _client.GetAsync("api/library/most-borrowed-books");

        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<Book[]>();

        Assert.NotNull(books);
        Assert.Contains(books, b => b.Title == "Mock Book 1");
        Assert.Contains(books, b => b.Title == "Mock Book 2");
    }

    [Fact]
    public async Task GetBookAvailability_ReturnsAvailability()
    {
        var response = await _client.GetAsync("api/library/book-availability/1");

        response.EnsureSuccessStatusCode();
        var availability = await response.Content.ReadFromJsonAsync<BookAvailability>();

        Assert.NotNull(availability);
        Assert.Equal(10, availability.Borrowed);
        Assert.Equal(5, availability.Available);
    }

    [Fact]
    public async Task GetBookAvailability_NotFound_Returns404()
    {
        GrpcMockFactory.SetupBookAvailabilityNotFound(_mockGrpcClient);

        var response = await _client.GetAsync("api/library/book-availability/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTopBorrowers_ValidDates_ReturnsUsers()
    {
        var response = await _client.GetAsync("api/library/top-borrowers?startDate=2023-01-01&endDate=2023-12-31");

        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<User[]>();

        Assert.NotNull(users);
        Assert.Contains(users, u => u.Name == "Top User");
    }

    [Fact]
    public async Task GetTopBorrowers_InvalidDate_Returns400()
    {
        var response = await _client.GetAsync("api/library/top-borrowers?startDate=invalid&endDate=2023-12-31");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowedBooks_ValidRequest_ReturnsBooks()
    {
        var response = await _client.GetAsync("api/library/user-borrowed-books?userId=1&startDate=2023-01-01&endDate=2023-12-31");

        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<Book[]>();

        Assert.NotNull(books);
        Assert.Contains(books, b => b.Title == "User Book");
    }

    [Fact]
    public async Task GetUserBorrowedBooks_InvalidUserId_Returns400()
    {
        var response = await _client.GetAsync("api/library/user-borrowed-books?userId=0&startDate=2023-01-01&endDate=2023-12-31");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetUserBorrowedBooks_InvalidDate_Returns400()
    {
        var response = await _client.GetAsync("api/library/user-borrowed-books?userId=1&startDate=invalid&endDate=2023-12-31");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBooksBorrowedWith_ReturnsBooks()
    {
        var response = await _client.GetAsync("api/library/books-borrowed-with/1");

        response.EnsureSuccessStatusCode();
        var books = await response.Content.ReadFromJsonAsync<Book[]>();

        Assert.NotNull(books);
        Assert.Contains(books, b => b.Title == "With Book");
    }

    [Fact]
    public async Task EstimateReadingRate_ValidRequest_ReturnsRate()
    {
        var response = await _client.GetAsync("api/library/estimate-reading-rate/1");

        response.EnsureSuccessStatusCode();
        var rate = await response.Content.ReadFromJsonAsync<ReadingRate>();

        Assert.NotNull(rate);
        Assert.Equal(12.5, rate.PagesPerDay);
    }

    [Fact]
    public async Task EstimateReadingRate_InvalidBorrowRecordId_Returns400()
    {
        GrpcMockFactory.SetupEstimateReadingRateInvalid(_mockGrpcClient);

        var response = await _client.GetAsync("api/library/estimate-reading-rate/-1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EstimateReadingRate_NotFound_Returns404()
    {
        GrpcMockFactory.SetupEstimateReadingRateNotFound(_mockGrpcClient);

        var response = await _client.GetAsync("api/library/estimate-reading-rate/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
