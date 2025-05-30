using Grpc.Core;
using Library.GrpcContracts;
using Library.Application.Interfaces;

namespace Library.Service.Services;

public class LibraryGrpcService : LibraryService.LibraryServiceBase
{
    private readonly ILibraryCoreService _libraryService;

    public LibraryGrpcService(ILibraryCoreService libraryService)
    {
        _libraryService = libraryService;
    }

    public override async Task<BookList> GetMostBorrowedBooks(Empty request, ServerCallContext context)
    {
        var books = await _libraryService.GetMostBorrowedBooksAsync();
        return new BookList
        {
            Books = { books.Select(b => new Book { Id = b.Id, Title = b.Title }) }
        };
    }

    public override async Task<BookAvailability> GetBookAvailability(BookRequest request, ServerCallContext context)
    {
        var (borrowed, available) = await _libraryService.GetBookAvailabilityAsync(request.BookId);
        return new BookAvailability
        {
            Borrowed = borrowed,
            Available = available
        };
    }

    public override async Task<UserList> GetTopBorrowers(DateRange request, ServerCallContext context)
    {
        var users = await _libraryService.GetTopBorrowersAsync(
            DateTime.Parse(request.StartDate),
            DateTime.Parse(request.EndDate));

        return new UserList
        {
            Users = { users.Select(u => new User { Id = u.Id, Name = u.Name }) }
        };
    }

    public override async Task<BookList> GetUserBorrowedBooks(UserDateRequest request, ServerCallContext context)
    {
        var books = await _libraryService.GetUserBorrowedBooksAsync(
            request.UserId,
            DateTime.Parse(request.StartDate),
            DateTime.Parse(request.EndDate));

        return new BookList
        {
            Books = { books.Select(b => new Book { Id = b.Id, Title = b.Title }) }
        };
    }

    public override async Task<BookList> GetBooksBorrowedWith(BookRequest request, ServerCallContext context)
    {
        var books = await _libraryService.GetBooksBorrowedWithAsync(request.BookId);
        return new BookList
        {
            Books = { books.Select(b => new Book { Id = b.Id, Title = b.Title }) }
        };
    }

    public override async Task<ReadingRate> EstimateReadingRate(BorrowRecordRequest request, ServerCallContext context)
    {
        var rate = await _libraryService.EstimateReadingRateAsync(request.BorrowRecordId);
        return new ReadingRate
        {
            PagesPerDay = rate
        };
    }
}
