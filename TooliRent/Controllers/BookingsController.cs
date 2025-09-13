using Application.Dto.BookingDtos;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Responses;

namespace TooliRent.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class BookingsController : Controller
	{
		private readonly IBookingService _bookingService;

		public BookingsController(IBookingService service)
		{
			_bookingService = service;
		}

		[HttpGet("all")]
		public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetAllBookings(CancellationToken ct)
		{
			var result = await _bookingService.GetAllAsync(ct);
			if (result.IsError)
			{
				return BadRequest(result);
			}

			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<BookingDto?>>> GetBookingById(Guid id, CancellationToken ct)
		{
			var result = await _bookingService.GetByIdAsync(id, ct);
			if (result.IsError)
			{
				return NotFound(result);
			}

			return Ok(result);
		}

		[HttpGet("{id}/items")]
		public async Task<ActionResult<ApiResponse<BookingWithItemsDto?>>> GetBookingWithItems(Guid id, CancellationToken ct)
		{
			var result = await _bookingService.GetWithItemsAsync(id, ct);
			if (result.IsError)
			{
				return NotFound(result);
			}

			return Ok(result);
		}

		[HttpGet("user/{userId}")]
		public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetBookingsByUser(string userId, CancellationToken ct)
		{
			var result = await _bookingService.GetByUserAsync(userId, ct);
			if (result.IsError)
			{
				return NotFound(result);
			}

			return Ok(result);
		}

		[HttpGet("status/{status}")]
		public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetBookingsByStatus(BookingStatus status, CancellationToken ct)
		{
			var result = await _bookingService.GetByStatusAsync(status, ct);
			if (result.IsError)
			{
				return NotFound(result);
			}

			return Ok(result);
		}

		[HttpGet("active")]
		public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetActiveBookings(CancellationToken ct)
		{
			var result = await _bookingService.GetActiveAsync(ct);
			if (result.IsError)
			{
				return NotFound(result);
			}

			return Ok(result);
		}

		[HttpPost("{id}/return")]
		public async Task<ActionResult<ApiResponse>> ReturnBooking(Guid id, CancellationToken ct)
		{
			var response = await _bookingService.ReturnBookingAsync(id, ct);
			return Ok(response);
		}


		[HttpPost("create")]
		public async Task<ActionResult<ApiResponse<Guid?>>> AddBooking([FromBody] AddBookingDto addBookingDto, CancellationToken ct)
		{
			var addedBookingId = await _bookingService.AddAsync(addBookingDto, User, ct);

			var addedBooking = await _bookingService.GetByIdAsync(addedBookingId.Value, ct);
			addedBooking.Message = "Booking created successfully";

			return CreatedAtAction(nameof(GetBookingById), new { id = addedBookingId }, addedBooking);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] UpdateBookingDto bookingDto, CancellationToken ct)
		{
			await _bookingService.UpdateAsync(bookingDto, id, ct);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteBooking(Guid id, CancellationToken ct)
		{
			await _bookingService.DeleteAsync(id, ct);
			return NoContent();
		}
	}
}
