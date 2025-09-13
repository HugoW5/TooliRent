using Application.Dto.BookingDtos;
using Domain.Enums;
using Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
	public interface IBookingService
	{
		Task<Guid?> AddAsync(AddBookingDto addBookingDto, ClaimsPrincipal user, CancellationToken ct = default);
		Task UpdateAsync(UpdateBookingDto bookingDto, Guid bookingId, CancellationToken ct = default);
		Task DeleteAsync(Guid bookingId, CancellationToken ct = default);

		Task<ApiResponse<BookingDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<BookingDto>>> GetAllAsync(CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<BookingDto>>> GetByUserAsync(string userId, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<BookingDto>>> GetByStatusAsync(BookingStatus status, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<BookingDto>>> GetActiveAsync(CancellationToken ct = default);
		Task<ApiResponse<BookingWithItemsDto?>> GetWithItemsAsync(Guid id, CancellationToken ct = default);
		Task<ApiResponse> ReturnBookingAsync(Guid bookingId, CancellationToken ct = default);
	}
}
