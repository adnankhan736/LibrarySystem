using System;

public static class LibraryWarmups
{
    /// <summary>
    /// Checks if a given book ID is a power of two.
    /// </summary>
    public static bool IsPowerOfTwo(int bookId)
    {
        return bookId > 0 && (bookId & (bookId - 1)) == 0;
    }

    /// <summary>
    /// Reverses the given book title.
    /// </summary>
    public static string ReverseTitle(string title)
    {
        if (title == null) return null;

        string reversed = "";
        for (int i = title.Length - 1; i >= 0; i--)
        {
            reversed += title[i];
        }
        return reversed;
    }

    /// <summary>
    /// Repeats the book title a specified number of times.
    /// </summary>
    public static string GenerateReplicas(string title, int count)
    {
        if (count <= 0 || title == null || title == "") return "";

        string result = "";
        for (int i = 0; i < count; i++)
        {
            result += title;
        }
        return result;
    }

    /// <summary>
    /// Prints all odd numbers between 0 and 100.
    /// </summary>
    public static void ListOddBookIds()
    {
        for (int i = 1; i <= 100; i += 2)
        {
            Console.WriteLine(i);
        }
    }
}
