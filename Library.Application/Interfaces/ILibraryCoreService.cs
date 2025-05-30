using Library.Domain.Entities;

namespace Library.Application.Interfaces;

public interface ILibraryCoreService
{
    Task<IEnumerable<Book>> GetMostBorrowedBooksAsync();
    Task<(int Borrowed, int Available)> GetBookAvailabilityAsync(int bookId);
    Task<IEnumerable<User>> GetTopBorrowersAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Book>> GetUserBorrowedBooksAsync(int userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Book>> GetBooksBorrowedWithAsync(int bookId);
    Task<double> EstimateReadingRateAsync(int borrowRecordId);
}
