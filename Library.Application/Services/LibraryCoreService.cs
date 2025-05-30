using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Services;

public class LibraryCoreService : ILibraryCoreService
{
    private readonly LibraryDbContext _context;

    public LibraryCoreService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetMostBorrowedBooksAsync()
    {
        return await _context.Books
            .OrderByDescending(b => b.BorrowRecords.Count)
            .Take(10)
            .ToListAsync();
    }

    public async Task<(int Borrowed, int Available)> GetBookAvailabilityAsync(int bookId)
    {
        var book = await _context.Books
            .Include(b => b.BorrowRecords)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
            throw new ArgumentException("Book not found.");

        int borrowed = book.BorrowRecords.Count(br => br.ReturnDate == null);
        int available = book.TotalCopies - borrowed;

        return (borrowed, available);
    }

    public async Task<IEnumerable<User>> GetTopBorrowersAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Users
            .Where(u => u.BorrowRecords.Any(br => br.BorrowDate >= startDate && br.BorrowDate <= endDate))
            .OrderByDescending(u => u.BorrowRecords.Count(br => br.BorrowDate >= startDate && br.BorrowDate <= endDate))
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetUserBorrowedBooksAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.BorrowRecords
            .Where(br => br.UserId == userId && br.BorrowDate >= startDate && br.BorrowDate <= endDate)
            .Select(br => br.Book)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksBorrowedWithAsync(int bookId)
    {
        var userIds = await _context.BorrowRecords
            .Where(br => br.BookId == bookId)
            .Select(br => br.UserId)
            .Distinct()
            .ToListAsync();

        return await _context.BorrowRecords
            .Where(br => userIds.Contains(br.UserId) && br.BookId != bookId)
            .Select(br => br.Book)
            .Distinct()
            .ToListAsync();
    }

    public async Task<double> EstimateReadingRateAsync(int borrowRecordId)
    {
        var record = await _context.BorrowRecords
            .Include(br => br.Book)
            .FirstOrDefaultAsync(br => br.Id == borrowRecordId);

        if (record == null || record.ReturnDate == null)
            throw new ArgumentException("Invalid borrow record.");

        var days = (record.ReturnDate.Value - record.BorrowDate).TotalDays;
        if (days <= 0)
            throw new InvalidOperationException("Invalid borrow duration.");

        // Assuming each book has 300 pages for estimation
        const int totalPages = 300;
        return totalPages / days;
    }
}
