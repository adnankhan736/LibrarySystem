using Grpc.Core;
using Library.GrpcContracts;
using Library.Application.Interfaces;

namespace Library.Service.Services
{
    /// <summary>
    /// gRPC service implementation for library-related operations.
    /// Delegates calls to the core library service.
    /// </summary>
    public class LibraryGrpcService : LibraryService.LibraryServiceBase
    {
        private readonly ILibraryCoreService _libraryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryGrpcService"/> class.
        /// </summary>
        /// <param name="libraryService">The core library service instance.</param>
        public LibraryGrpcService(ILibraryCoreService libraryService)
        {
            _libraryService = libraryService;
        }

        /// <summary>
        /// Retrieves the most borrowed books.
        /// </summary>
        /// <param name="request">An empty request.</param>
        /// <param name="context">Server call context.</param>
        /// <returns>A list of the most borrowed books.</returns>
        public override async Task<BookList> GetMostBorrowedBooks(Empty request, ServerCallContext context)
        {
            var books = await _libraryService.GetMostBorrowedBooksAsync();
            return new BookList
            {
                Books = { books.Select(b => new Book { Id = b.Id, Title = b.Title }) }
            };
        }

        /// <summary>
        /// Retrieves the availability status of a specific book.
        /// </summary>
        /// <param name="request">The book request containing the book ID.</param>
        /// <param name="context">Server call context.</param>
        /// <returns>The availability status including borrowed and available count.</returns>
        public override async Task<BookAvailability> GetBookAvailability(BookRequest request, ServerCallContext context)
        {
            var (borrowed, available) = await _libraryService.GetBookAvailabilityAsync(request.BookId);
            return new BookAvailability
            {
                Borrowed = borrowed,
                Available = available
            };
        }

        /// <summary>
        /// Retrieves top borrowers within a specified date range.
        /// </summary>
        /// <param name="request">The date range request containing start and end dates.</param>
        /// <param name="context">Server call context.</param>
        /// <returns>A list of top borrowers.</returns>
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

        /// <summary>
        /// Retrieves books borrowed by a specific user within a date range.
        /// </summary>
        /// <param name="request">The user date request containing user ID and date range.</param>
        /// <param name="context">Server call context.</param>
        /// <returns>A list of books borrowed by the user.</returns>
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

        /// <summary>
        /// Retrieves books frequently borrowed together with a specific book.
        /// </summary>
        /// <param name="request">The book request containing the book ID.</param>
        /// <param name="context">Server call context.</param>
        /// <returns>A list of books borrowed with the specified book.</returns>
        public override async Task<BookList> GetBooksBorrowedWith(BookRequest request, ServerCallContext context)
        {
            var books = await _libraryService.GetBooksBorrowedWithAsync(request.BookId);
            return new BookList
            {
                Books = { books.Select(b => new Book { Id = b.Id, Title = b.Title }) }
            };
        }

        /// <summary>
        /// Estimates the reading rate (pages per day) for a given borrow record.
        /// </summary>
        /// <param name="request">The borrow record request containing the borrow record ID.</param>
        /// <param name="context">Server call context.</param>
        /// <returns>The estimated reading rate as pages per day.</returns>
        public override async Task<ReadingRate> EstimateReadingRate(BorrowRecordRequest request, ServerCallContext context)
        {
            var rate = await _libraryService.EstimateReadingRateAsync(request.BorrowRecordId);
            return new ReadingRate
            {
                PagesPerDay = rate
            };
        }
    }
}
