//using Grpc.Net.Client;
//using Microsoft.AspNetCore.Mvc;
//using Library.GrpcContracts;

//namespace Library.API.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class LibraryController : ControllerBase
//{
//    private readonly LibraryService.LibraryServiceClient _grpcClient;

//    public LibraryController(LibraryService.LibraryServiceClient grpcClient)
//    {
//        _grpcClient = grpcClient;
//    }

//    [HttpGet("most-borrowed-books")]
//    public async Task<IActionResult> GetMostBorrowedBooks()
//    {
//        var response = await _grpcClient.GetMostBorrowedBooksAsync(new Empty());
//        return Ok(response.Books);
//    }

//    [HttpGet("book-availability/{bookId}")]
//    public async Task<IActionResult> GetBookAvailability(int bookId)
//    {
//        var response = await _grpcClient.GetBookAvailabilityAsync(new BookRequest { BookId = bookId });
//        return Ok(response);
//    }

//    [HttpGet("top-borrowers")]
//    public async Task<IActionResult> GetTopBorrowers([FromQuery] string startDate, [FromQuery] string endDate)
//    {
//        var response = await _grpcClient.GetTopBorrowersAsync(new DateRange
//        {
//            StartDate = startDate,
//            EndDate = endDate
//        });
//        return Ok(response.Users);
//    }

//    [HttpGet("user-borrowed-books")]
//    public async Task<IActionResult> GetUserBorrowedBooks([FromQuery] int userId, [FromQuery] string startDate, [FromQuery] string endDate)
//    {
//        var response = await _grpcClient.GetUserBorrowedBooksAsync(new UserDateRequest
//        {
//            UserId = userId,
//            StartDate = startDate,
//            EndDate = endDate
//        });
//        return Ok(response.Books);
//    }

//    [HttpGet("books-borrowed-with/{bookId}")]
//    public async Task<IActionResult> GetBooksBorrowedWith(int bookId)
//    {
//        var response = await _grpcClient.GetBooksBorrowedWithAsync(new BookRequest { BookId = bookId });
//        return Ok(response.Books);
//    }

//    [HttpGet("estimate-reading-rate/{borrowRecordId}")]
//    public async Task<IActionResult> EstimateReadingRate(int borrowRecordId)
//    {
//        var response = await _grpcClient.EstimateReadingRateAsync(new BorrowRecordRequest { BorrowRecordId = borrowRecordId });
//        return Ok(response);
//    }
//}
