using Grpc.Core;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Application.Tests.Services
{
    public class LibraryCoreServiceTests
    {
        [Theory]
        [MemberData(nameof(MostBorrowedBooksTestData.GetData), MemberType = typeof(MostBorrowedBooksTestData))]
        public async Task GetMostBorrowedBooksAsync_ReturnsTop10OrderedByBorrowCount(List<Book> books)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(books).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            // Act
            var result = (await service.GetMostBorrowedBooksAsync().ConfigureAwait(false)).ToList();

            // Assert
            int expectedCount = Math.Min(10, books.Count);
            Assert.Equal(expectedCount, result.Count);

            for (int i = 0; i < result.Count - 1; i++)
            {
                Assert.True(result[i].BorrowRecords.Count >= result[i + 1].BorrowRecords.Count);
            }

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(BookAvailabilityTestData.GetData), MemberType = typeof(BookAvailabilityTestData))]
        public async Task GetBookAvailabilityAsync_ValidBook_ReturnsBorrowedAndAvailable(
            Book book, int expectedBorrowed, int expectedAvailable)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(new List<Book> { book }).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            // Act
            var (borrowed, available) = await service.GetBookAvailabilityAsync(book.Id).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedBorrowed, borrowed);
            Assert.Equal(expectedAvailable, available);

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(999), InlineData(-1)]
        public async Task GetBookAvailabilityAsync_InvalidBookId_ThrowsRpcException(int bookId)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(new List<Book>()).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RpcException>(() => service.GetBookAvailabilityAsync(bookId)).ConfigureAwait(false);
            Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            Assert.Equal("Book not found.", ex.Status.Detail);

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TopBorrowersTestData.GetValidData), MemberType = typeof(TopBorrowersTestData))]
        public async Task GetTopBorrowersAsync_ValidRange_ReturnsTopBorrowers(
            List<User> users, DateTime start, DateTime end, int expectedCount)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(users: users).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            // Act
            var result = (await service.GetTopBorrowersAsync(start, end).ConfigureAwait(false)).ToList();

            // Assert
            Assert.Equal(expectedCount, result.Count);

            for (int i = 0; i < result.Count - 1; i++)
            {
                int currentCount = result[i].BorrowRecords.Count(br => br.BorrowDate >= start && br.BorrowDate <= end);
                int nextCount = result[i + 1].BorrowRecords.Count(br => br.BorrowDate >= start && br.BorrowDate <= end);
                Assert.True(currentCount >= nextCount);
            }

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(TopBorrowersTestData.GetEmptyData), MemberType = typeof(TopBorrowersTestData))]
        public async Task GetTopBorrowersAsync_NoBorrowers_ThrowsRpcException(DateTime start, DateTime end)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(users: new List<User>()).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RpcException>(() => service.GetTopBorrowersAsync(start, end)).ConfigureAwait(false);
            Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            Assert.Equal("No top borrowers found in the given date range.", ex.Status.Detail);

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }


        [Theory]
        [MemberData(nameof(UserBorrowedBooksTestData.GetValidUserData), MemberType = typeof(UserBorrowedBooksTestData))]
        public async Task GetUserBorrowedBooksAsync_ValidUser_ReturnsBooksOrThrows(
            List<User> users, List<BorrowRecord> borrowRecords, int userId, DateTime start, DateTime end, int expectedCount)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(users: users, borrowRecords: borrowRecords).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            if (expectedCount == 0)
            {
                var ex = await Assert.ThrowsAsync<RpcException>(() => service.GetUserBorrowedBooksAsync(userId, start, end)).ConfigureAwait(false);
                Assert.Equal(StatusCode.NotFound, ex.StatusCode);
                Assert.Equal($"No books found for user {userId} in the given date range.", ex.Status.Detail);
            }
            else
            {
                var result = (await service.GetUserBorrowedBooksAsync(userId, start, end).ConfigureAwait(false)).ToList();

                Assert.Equal(expectedCount, result.Count);

                // Optional: further assertions (e.g., all books belong to user, dates within range)
                Assert.All(result, book => Assert.NotNull(book));
            }

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }


        [Theory]
        [MemberData(nameof(UserBorrowedBooksTestData.GetInvalidUserData), MemberType = typeof(UserBorrowedBooksTestData))]
        public async Task GetUserBorrowedBooksAsync_InvalidUser_ThrowsRpcException(
            List<User> users, List<BorrowRecord> borrowRecords, int userId, DateTime start, DateTime end, string expectedMessage)
        {
            // Arrange
            var context = await LibraryDbContextMockFactory.CreateAsync(users: users, borrowRecords: borrowRecords).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<RpcException>(() => service.GetUserBorrowedBooksAsync(userId, start, end)).ConfigureAwait(false);
            Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            Assert.Equal(expectedMessage, ex.Status.Detail);

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(BooksBorrowedWithTestData.GetValidData), MemberType = typeof(BooksBorrowedWithTestData))]
        public async Task GetBooksBorrowedWithAsync_ValidBook_ReturnsAssociatedBooks(
            List<Book> books, List<BorrowRecord> borrowRecords, int bookId, int expectedCount)
        {
            var context = await LibraryDbContextMockFactory.CreateAsync(books: books, borrowRecords: borrowRecords).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            var result = (await service.GetBooksBorrowedWithAsync(bookId).ConfigureAwait(false)).ToList();

            Assert.Equal(expectedCount, result.Count);

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(BooksBorrowedWithTestData.GetInvalidData), MemberType = typeof(BooksBorrowedWithTestData))]
        public async Task GetBooksBorrowedWithAsync_InvalidBook_ThrowsRpcException(
            List<Book> books, List<BorrowRecord> borrowRecords, int bookId, string expectedMessage)
        {
            var context = await LibraryDbContextMockFactory.CreateAsync(books: books, borrowRecords: borrowRecords).ConfigureAwait(false);
            var service = new LibraryCoreService(context);

            var ex = await Assert.ThrowsAsync<RpcException>(() => service.GetBooksBorrowedWithAsync(bookId)).ConfigureAwait(false);
            Assert.Equal(StatusCode.NotFound, ex.StatusCode);
            Assert.Equal(expectedMessage, ex.Status.Detail);

            await LibraryDbContextMockFactory.DestroyAsync(context).ConfigureAwait(false);
        }


        [Theory]
        [MemberData(nameof(EstimateReadingRateTestData.GetValidData), MemberType = typeof(EstimateReadingRateTestData))]
        public async Task EstimateReadingRateAsync_ValidRecord_ReturnsReadingRate(
            List<Book> books, List<BorrowRecord> borrowRecords, int borrowRecordId, double expectedRate)
        {
            var context = await LibraryDbContextMockFactory.CreateAsync(books: books, borrowRecords: borrowRecords);
            var service = new LibraryCoreService(context);

            var rate = await service.EstimateReadingRateAsync(borrowRecordId);

            Assert.Equal(expectedRate, rate, precision: 2);
            await LibraryDbContextMockFactory.DestroyAsync(context);
        }

        [Theory]
        [MemberData(nameof(EstimateReadingRateTestData.GetInvalidData), MemberType = typeof(EstimateReadingRateTestData))]
        public async Task EstimateReadingRateAsync_InvalidRecord_ThrowsRpcException(
            List<Book> books, List<BorrowRecord> borrowRecords, int borrowRecordId, StatusCode expectedCode, string expectedMessage)
        {
            var context = await LibraryDbContextMockFactory.CreateAsync(books: books, borrowRecords: borrowRecords);
            var service = new LibraryCoreService(context);

            var ex = await Assert.ThrowsAsync<RpcException>(() => service.EstimateReadingRateAsync(borrowRecordId));
            Assert.Equal(expectedCode, ex.StatusCode);
            Assert.Equal(expectedMessage, ex.Status.Detail);

            await LibraryDbContextMockFactory.DestroyAsync(context);
        }

    }
}
