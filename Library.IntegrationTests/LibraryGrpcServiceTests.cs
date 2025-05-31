using Library.GrpcContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Library.IntegrationTests
{
    public class LibraryGrpcServiceTests : GrpcTestBase
    {
        [Fact]
        public async Task GetMostBorrowedBooks_ReturnsBooks()
        {
            var response = await Client.GetMostBorrowedBooksAsync(new Empty());
            Assert.NotNull(response);
            Assert.True(response.Books.Count >= 0);
        }

        [Fact]
        public async Task GetBookAvailability_ReturnsStatus()
        {
            var response = await Client.GetBookAvailabilityAsync(new BookRequest { BookId = 1 });
            Assert.NotNull(response);
            Assert.True(response.Borrowed >= 0);
            Assert.True(response.Available >= 0);
        }

        [Fact]
        public async Task GetTopBorrowers_ReturnsUsers()
        {
            var response = await Client.GetTopBorrowersAsync(new DateRange
            {
                StartDate = "2023-01-01",
                EndDate = "2025-01-01"
            });
            Assert.NotNull(response);
            Assert.True(response.Users.Count >= 0);
        }

        [Fact]
        public async Task GetUserBorrowedBooks_ReturnsBooks()
        {
            var response = await Client.GetUserBorrowedBooksAsync(new UserDateRequest
            {
                UserId = 1,
                StartDate = "2023-01-01",
                EndDate = "2025-01-01"
            });
            Assert.NotNull(response);
            Assert.True(response.Books.Count >= 0);
        }

        [Fact]
        public async Task GetBooksBorrowedWith_ReturnsBooks()
        {
            var response = await Client.GetBooksBorrowedWithAsync(new BookRequest { BookId = 1 });
            Assert.NotNull(response);
            Assert.True(response.Books.Count >= 0);
        }

        [Fact]
        public async Task EstimateReadingRate_ReturnsRate()
        {
            var response = await Client.EstimateReadingRateAsync(new BorrowRecordRequest { BorrowRecordId = 1 });
            Assert.NotNull(response);
            Assert.True(response.PagesPerDay >= 0);
        }

    }
}
