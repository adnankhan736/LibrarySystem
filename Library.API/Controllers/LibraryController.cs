using Grpc.Net.Client;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Library.GrpcContracts;

namespace Library.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly LibraryService.LibraryServiceClient _grpcClient;

    public LibraryController(LibraryService.LibraryServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    /// <summary>
    /// Retrieves the most borrowed books across all users.
    /// </summary>
    /// <returns>A list of the most borrowed books.</returns>
    [HttpGet("most-borrowed-books")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMostBorrowedBooks()
    {
        try
        {
            var response = await _grpcClient.GetMostBorrowedBooksAsync(new Empty());
            return Ok(response.Books);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Retrieves the availability status of a specific book.
    /// </summary>
    /// <param name="bookId">The ID of the book to check availability for.</param>
    /// <returns>The availability status of the specified book.</returns>
    [HttpGet("book-availability/{bookId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBookAvailability(int bookId)
    {
        try
        {
            var response = await _grpcClient.GetBookAvailabilityAsync(new BookRequest { BookId = bookId });
            if (response == null)
                return NotFound();

            return Ok(response);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Retrieves the top borrowers within a specific date range.
    /// </summary>
    /// <param name="startDate">Start date in yyyy-MM-dd format.</param>
    /// <param name="endDate">End date in yyyy-MM-dd format.</param>
    /// <returns>A list of top borrowers within the specified date range.</returns>
    [HttpGet("top-borrowers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTopBorrowers([FromQuery] string startDate, [FromQuery] string endDate)
    {
        if (!DateTime.TryParse(startDate, out _) || !DateTime.TryParse(endDate, out _))
            return BadRequest("Invalid date format.");

        try
        {
            var response = await _grpcClient.GetTopBorrowersAsync(new DateRange
            {
                StartDate = startDate,
                EndDate = endDate
            });
            return Ok(response.Users);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Retrieves books borrowed by a specific user within a date range.
    /// </summary>
    /// <param name="userId">User ID to search borrow records for.</param>
    /// <param name="startDate">Start date in yyyy-MM-dd format.</param>
    /// <param name="endDate">End date in yyyy-MM-dd format.</param>
    /// <returns>A list of books borrowed by the user during the date range.</returns>
    [HttpGet("user-borrowed-books")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserBorrowedBooks([FromQuery] int userId, [FromQuery] string startDate, [FromQuery] string endDate)
    {
        if (userId <= 0)
            return BadRequest("Invalid userId.");

        if (!DateTime.TryParse(startDate, out _) || !DateTime.TryParse(endDate, out _))
            return BadRequest("Invalid date format.");

        try
        {
            var response = await _grpcClient.GetUserBorrowedBooksAsync(new UserDateRequest
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            });
            return Ok(response.Books);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Retrieves books frequently borrowed along with a specific book.
    /// </summary>
    /// <param name="bookId">The ID of the reference book.</param>
    /// <returns>A list of books that are frequently borrowed with the specified book.</returns>
    [HttpGet("books-borrowed-with/{bookId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBooksBorrowedWith(int bookId)
    {
        try
        {
            var response = await _grpcClient.GetBooksBorrowedWithAsync(new BookRequest { BookId = bookId });
            return Ok(response.Books);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }

    /// <summary>
    /// Estimates the reading rate for a borrow record by analyzing borrow and return dates.
    /// </summary>
    /// <param name="borrowRecordId">The ID of the borrow record.</param>
    /// <returns>The estimated reading rate in pages per day.</returns>
    [HttpGet("estimate-reading-rate/{borrowRecordId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EstimateReadingRate(int borrowRecordId)
    {
        try
        {
            var response = await _grpcClient.EstimateReadingRateAsync(
                new BorrowRecordRequest { BorrowRecordId = borrowRecordId });

            if (response == null)
                return NotFound();

            return Ok(response);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.FailedPrecondition)
        {
            return BadRequest(ex.Status.Detail);
        }
        catch (Exception ex)
        {
            return Problem(detail: ex.Message, statusCode: 500);
        }
    }
}
