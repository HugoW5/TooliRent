using Application.Dto.BookingDtos;
using Application.Services.Interfaces;
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
				return NotFound(result);

			return Ok(result);
		}

	}
}
