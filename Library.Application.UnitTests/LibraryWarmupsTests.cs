using System;
using System.IO;
using Xunit;

namespace Library.Application.Tests;
public class LibraryWarmupsTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(0, false)]
    [InlineData(-2, false)]
    [InlineData(16, true)]
    [InlineData(18, false)]
    public void IsPowerOfTwo_ReturnsExpected(int bookId, bool expected)
    {
        // Arrange
        // (No setup needed)

        // Act
        bool result = LibraryWarmups.IsPowerOfTwo(bookId);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("abc", "cba")]
    [InlineData("Book", "kooB")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ReverseTitle_ReturnsReversedString(string input, string expected)
    {
        // Arrange
        // (No setup needed)

        // Act
        string result = LibraryWarmups.ReverseTitle(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Book", 3, "BookBookBook")]
    [InlineData("A", 5, "AAAAA")]
    [InlineData("", 10, "")]
    [InlineData(null, 4, "")]
    [InlineData("Test", 0, "")]
    [InlineData("Test", -1, "")]
    public void GenerateReplicas_ReturnsRepeatedString(string title, int count, string expected)
    {
        // Arrange
        // (No setup needed)

        // Act
        string result = LibraryWarmups.GenerateReplicas(title, count);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ListOddBookIds_PrintsOddNumbersBetween1And100()
    {
        // Arrange
        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        LibraryWarmups.ListOddBookIds();
        var output = sw.ToString().Trim().Split(Environment.NewLine);

        // Assert
        Assert.Equal(50, output.Length); // There are 50 odd numbers between 1 and 100 inclusive

        for (int i = 0; i < output.Length; i++)
        {
            int number = int.Parse(output[i]);
            Assert.True(number % 2 == 1);
            Assert.InRange(number, 1, 100);
            Assert.Equal(2 * i + 1, number);
        }
    }
}
