using Library.Domain.Entities;

public class TopBorrowersTestData
{
    public static IEnumerable<object[]> GetValidData()
    {
        var start = new DateTime(2023, 1, 1);
        var end = new DateTime(2023, 12, 31);

        yield return new object[]
        {
            new List<User>
            {
                new User
                {
                    Id = 1,
                    Name = "Alice",
                    BorrowRecords = new List<BorrowRecord>
                    {
                        new BorrowRecord { BorrowDate = new DateTime(2023, 6, 1) },
                        new BorrowRecord { BorrowDate = new DateTime(2023, 7, 1) }
                    }
                },
                new User
                {
                    Id = 2,
                    Name = "Bob",
                    BorrowRecords = new List<BorrowRecord>
                    {
                        new BorrowRecord { BorrowDate = new DateTime(2023, 4, 1) }
                    }
                }
            },
            start,
            end,
            2  // expectedCount
        };
    }

    public static IEnumerable<object[]> GetEmptyData()
    {
        var start = new DateTime(2023, 1, 1);
        var end = new DateTime(2023, 12, 31);

        yield return new object[] { start, end };
    }
}
