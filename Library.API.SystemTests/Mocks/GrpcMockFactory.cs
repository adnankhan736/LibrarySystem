using Grpc.Core;
using Library.GrpcContracts;
using Moq;
using static Library.GrpcContracts.LibraryService;

namespace Library.API.SystemTests.Mocks;

public static class GrpcMockFactory
{
    public static Mock<LibraryServiceClient> CreateMockClient()
    {
        var mock = new Mock<LibraryServiceClient>();

        // Success responses
        mock.Setup(x => x.GetMostBorrowedBooksAsync(It.IsAny<Empty>(), null, null, default))
            .Returns(CreateAsyncUnaryCall(new BookList
            {
                Books =
                {
                    new Book { Id = 1, Title = "Mock Book 1" },
                    new Book { Id = 2, Title = "Mock Book 2" }
                }
            }));

        mock.Setup(x => x.GetBookAvailabilityAsync(It.IsAny<BookRequest>(), null, null, default))
            .Returns(CreateAsyncUnaryCall(new BookAvailability
            {
                Borrowed = 10,
                Available = 5
            }));

        mock.Setup(x => x.GetTopBorrowersAsync(It.IsAny<DateRange>(), null, null, default))
            .Returns(CreateAsyncUnaryCall(new UserList
            {
                Users =
                {
                    new User { Id = 1, Name = "Top User" }
                }
            }));

        mock.Setup(x => x.GetUserBorrowedBooksAsync(It.IsAny<UserDateRequest>(), null, null, default))
            .Returns(CreateAsyncUnaryCall(new BookList
            {
                Books =
                {
                    new Book { Id = 3, Title = "User Book" }
                }
            }));

        mock.Setup(x => x.GetBooksBorrowedWithAsync(It.IsAny<BookRequest>(), null, null, default))
            .Returns(CreateAsyncUnaryCall(new BookList
            {
                Books =
                {
                    new Book { Id = 4, Title = "With Book" }
                }
            }));

        mock.Setup(x => x.EstimateReadingRateAsync(
        It.Is<BorrowRecordRequest>(r => r.BorrowRecordId == 1),
        null, null, default))
        .Returns(CreateAsyncUnaryCall(new ReadingRate
        {
        PagesPerDay = 12.5
        }));

        return mock;
    }

    // Error mocks

    public static void SetupBookAvailabilityNotFound(Mock<LibraryServiceClient> mock)
    {
        mock.Setup(x => x.GetBookAvailabilityAsync(It.Is<BookRequest>(r => r.BookId == 999), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "Book not found")));
    }

    public static void SetupTopBorrowersInvalid(Mock<LibraryServiceClient> mock)
    {
        mock.Setup(x => x.GetTopBorrowersAsync(It.Is<DateRange>(r => r.StartDate == "invalid"), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format")));
    }

    public static void SetupEstimateReadingRateInvalid(Mock<LibraryServiceClient> mock)
    {
        mock.Setup(x => x.EstimateReadingRateAsync(It.Is<BorrowRecordRequest>(r => r.BorrowRecordId == -1), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.InvalidArgument, "Invalid borrowRecordId")));
    }

    public static void SetupEstimateReadingRateNotFound(Mock<LibraryServiceClient> mock)
    {
        mock.Setup(x => x.EstimateReadingRateAsync(It.Is<BorrowRecordRequest>(r => r.BorrowRecordId == 999), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.NotFound, "Borrow record not found")));
    }

    // Helper to create AsyncUnaryCall from result
    private static AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T result) where T : class
    {
        return new AsyncUnaryCall<T>(
            Task.FromResult(result),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
    }
}
