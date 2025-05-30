using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Library.Common.Dtos.Request;
using Library.Common.Dtos.Response;
using LibraryProto;

namespace Library.Service.Services;

public class LibraryServiceImpl : LibraryService.LibraryServiceBase
{
    private readonly LibraryDbContext _context;

    public LibraryServiceImpl(LibraryDbContext context)
    {
        _context = context;
    }

    public override async Task<BookListResponse> GetMostBorrowedBooks(Empty request, ServerCallContext context)
    {
        var books = await _context.BorrowRecords
            .GroupBy(b => b.BookId)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Join(_context.Books, g => g.Key, b => b.Id, (g, b) => new BookResponse
            {
                Id = b.Id,
                Title = b.Title,
                BorrowCount = g.Count()
            })
            .ToListAsync();

        var response = new BookListResponse();
        response.Books.AddRange(books);
        return response;
    }

    public override async Task<AvailabilityResponse> GetBookAvailability(BookIdRequest request, ServerCallContext context)
    {
        var book = await _context.Books.FindAsync(request.BookId);
        var borrowedCount = await _context.BorrowRecords
            .CountAsync(br => br.BookId == request.BookId && br.ReturnedAt == null);

        return new AvailabilityResponse
        {
            TotalCopies = book.TotalCopies,
            BorrowedCopies = borrowedCount,
            AvailableCopies = book.TotalCopies - borrowedCount
        };
    }

    public override async Task<UserListResponse> GetTopBorrowers(DateRangeRequest request, ServerCallContext context)
    {
        var start = DateTime.Parse(request.StartDate);
        var end = DateTime.Parse(request.EndDate);

        var users = await _context.BorrowRecords
            .Where(br => br.BorrowedAt >= start && br.BorrowedAt <= end)
            .GroupBy(br => br.UserId)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Join(_context.Users, g => g.Key, u => u.Id, (g, u) => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                BorrowCount = g.Count()
            })
            .ToListAsync();

        var response = new UserListResponse();
        response.Users.AddRange(users);
        return response;
    }

    public override async Task<BookListResponse> GetUserBorrowHistory(UserPeriodRequest request, ServerCallContext context)
    {
        var start = DateTime.Parse(request.StartDate);
        var end = DateTime.Parse(request.EndDate);

        var bookIds = await _context.BorrowRecords
            .Where(br => br.UserId == request.UserId && br.BorrowedAt >= start && br.BorrowedAt <= end)
            .Select(br => br.BookId)
            .Distinct()
            .ToListAsync();

        var books = await _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .Select(b => new BookResponse { Id = b.Id, Title = b.Title })
            .ToListAsync();

        var response = new BookListResponse();
        response.Books.AddRange(books);
        return response;
    }

    public override async Task<BookListResponse> GetAlsoBorrowedBooks(BookIdRequest request, ServerCallContext context)
    {
        var users = await _context.BorrowRecords
            .Where(br => br.BookId == request.BookId)
            .Select(br => br.UserId)
            .Distinct()
            .ToListAsync();

        var bookIds = await _context.BorrowRecords
            .Where(br => users.Contains(br.UserId) && br.BookId != request.BookId)
            .GroupBy(br => br.BookId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToListAsync();

        var books = await _context.Books
            .Where(b => bookIds.Contains(b.Id))
            .Select(b => new BookResponse { Id = b.Id, Title = b.Title })
            .ToListAsync();

        var response = new BookListResponse();
        response.Books.AddRange(books);
        return response;
    }

    public override async Task<ReadingRateResponse> GetReadingRate(BookIdRequest request, ServerCallContext context)
    {
        var records = await _context.BorrowRecords
            .Where(br => br.BookId == request.BookId && br.ReturnedAt != null)
            .ToListAsync();

        var book = await _context.Books.FindAsync(request.BookId);

        if (records.Count == 0 || book == null)
            return new ReadingRateResponse { PagesPerDay = 0 };

        double totalDays = records.Sum(r => (r.ReturnedAt!.Value - r.BorrowedAt).TotalDays);
        double avgDays = totalDays / records.Count;

        return new ReadingRateResponse { PagesPerDay = (float)(book.Pages / avgDays) };
    }
}
