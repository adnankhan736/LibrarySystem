using Grpc.Core;
using Library.Application.Interfaces;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Application.Services
{
    /// <summary>
    /// Core service implementation handling library business logic and data operations.
    /// </summary>
    public class LibraryCoreService : ILibraryCoreService
    {
        private readonly LibraryDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryCoreService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public LibraryCoreService(LibraryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the top 10 most borrowed books.
        /// </summary>
        /// <returns>A collection of most borrowed books.</returns>
        public async Task<IEnumerable<Book>> GetMostBorrowedBooksAsync()
        {
            return await _context.Books
                .OrderByDescending(b => b.BorrowRecords.Count)
                .Take(10)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the availability of a specific book including borrowed and available copies.
        /// </summary>
        /// <param name="bookId">The book ID.</param>
        /// <returns>A tuple with the number of borrowed and available copies.</returns>
        /// <exception cref="RpcException">Thrown if the book is not found.</exception>
        public async Task<(int Borrowed, int Available)> GetBookAvailabilityAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.BorrowRecords)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Book not found."));

            int borrowed = book.BorrowRecords.Count(br => br.ReturnDate == null);
            int available = book.TotalCopies - borrowed;

            return (borrowed, available);
        }

        /// <summary>
        /// Gets the top 10 borrowers within the specified date range.
        /// </summary>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A collection of top borrowers.</returns>
        /// <exception cref="RpcException">Thrown if no borrowers are found in the date range.</exception>
        public async Task<IEnumerable<User>> GetTopBorrowersAsync(DateTime startDate, DateTime endDate)
        {
            var users = await _context.Users
                .Where(u => u.BorrowRecords.Any(br => br.BorrowDate >= startDate && br.BorrowDate <= endDate))
                .OrderByDescending(u => u.BorrowRecords.Count(br => br.BorrowDate >= startDate && br.BorrowDate <= endDate))
                .Take(10)
                .ToListAsync();

            if (!users.Any())
                throw new RpcException(new Status(StatusCode.NotFound, "No top borrowers found in the given date range."));

            return users;
        }

        /// <summary>
        /// Gets books borrowed by a specific user within a date range.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="startDate">The start date of the range.</param>
        /// <param name="endDate">The end date of the range.</param>
        /// <returns>A collection of books borrowed by the user.</returns>
        /// <exception cref="RpcException">Thrown if user or books are not found.</exception>
        public async Task<IEnumerable<Book>> GetUserBorrowedBooksAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                throw new RpcException(new Status(StatusCode.NotFound, $"User with ID {userId} not found."));

            var books = await _context.BorrowRecords
                .Where(br => br.UserId == userId && br.BorrowDate >= startDate && br.BorrowDate <= endDate)
                .Select(br => br.Book)
                .Distinct()
                .ToListAsync();

            if (!books.Any())
                throw new RpcException(new Status(StatusCode.NotFound, $"No books found for user {userId} in the given date range."));

            return books;
        }

        /// <summary>
        /// Gets books frequently borrowed together with a specified book.
        /// </summary>
        /// <param name="bookId">The book ID.</param>
        /// <returns>A collection of books borrowed with the specified book.</returns>
        /// <exception cref="RpcException">Thrown if the book or associated borrow records are not found.</exception>
        public async Task<IEnumerable<Book>> GetBooksBorrowedWithAsync(int bookId)
        {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                throw new RpcException(new Status(StatusCode.NotFound, $"Book with ID {bookId} not found."));

            var userIds = await _context.BorrowRecords
                .Where(br => br.BookId == bookId)
                .Select(br => br.UserId)
                .Distinct()
                .ToListAsync();

            if (!userIds.Any())
                throw new RpcException(new Status(StatusCode.NotFound, $"No borrow records found for book ID {bookId}."));

            var books = await _context.BorrowRecords
                .Where(br => userIds.Contains(br.UserId) && br.BookId != bookId)
                .Select(br => br.Book)
                .Distinct()
                .ToListAsync();

            if (!books.Any())
                throw new RpcException(new Status(StatusCode.NotFound, $"No associated books found with book ID {bookId}."));

            return books;
        }

        /// <summary>
        /// Estimates the reading rate (pages per day) for a borrow record, based on borrow and return dates.
        /// </summary>
        /// <param name="borrowRecordId">The borrow record ID.</param>
        /// <returns>The estimated pages read per day.</returns>
        /// <exception cref="RpcException">Thrown if the borrow record is not found, incomplete, or has invalid data.</exception>
        public async Task<double> EstimateReadingRateAsync(int borrowRecordId)
        {
            var record = await _context.BorrowRecords
                .Include(br => br.Book)
                .FirstOrDefaultAsync(br => br.Id == borrowRecordId);

            if (record == null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Borrow record with ID {borrowRecordId} not found."));

            if (record.ReturnDate == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Return date is required."));

            if (record.Book == null || record.Book.Pages <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Book information is missing or invalid."));

            var days = (record.ReturnDate.Value - record.BorrowDate).TotalDays + 1;

            if (days <= 0)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Return date must be after borrow date."));

            return record.Book.Pages / days;
        }
    }
}
