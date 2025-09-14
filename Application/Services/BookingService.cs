using Application.Dto.BookingDtos;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class BookingService : IBookingService
	{
		private readonly IBookingRepository _repo;
		private readonly IToolRepository _toolRepo;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public BookingService(
			IBookingRepository repo,
			IToolRepository toolRepo,
			IMapper mapper,
			IUnitOfWork unitOfWork)
		{
			_repo = repo;
			_toolRepo = toolRepo;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public async Task<Guid?> AddAsync(AddBookingDto addBookingDto, ClaimsPrincipal user, CancellationToken ct = default)
		{
			var role = user.FindFirstValue(ClaimTypes.Role);
			var claimUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);

			if (role == "Member")
			{
				if (string.IsNullOrEmpty(claimUserId))
				{
					throw new UnauthorizedAccessException("User ID claim is missing.");
				}

				addBookingDto.UserId = claimUserId;
			}
			else if (role == "Admin")
			{
				if (string.IsNullOrWhiteSpace(addBookingDto.UserId))
				{
					throw new ArgumentException("Admin must provide UserId when booking for a member.");
				}
			}
			else
			{
				throw new UnauthorizedAccessException("Invalid role for booking.");
			}

			var startAtUtc = DateTime.SpecifyKind(addBookingDto.StartAt, DateTimeKind.Utc);
			var endAtUtc = DateTime.SpecifyKind(addBookingDto.EndAt, DateTimeKind.Utc);

			if (startAtUtc >= endAtUtc)
			{
				throw new ArgumentException("Start date must be earlier than end date.");
			}

			if (startAtUtc < DateTime.UtcNow)
			{
				throw new ArgumentException("Start date cannot be in the past.");
			}

			// Validate tool IDs
			foreach (var toolId in addBookingDto.ToolIds)
			{
				var tool = await _toolRepo.GetByIdAsync(toolId, ct);
				if (tool == null)
					throw new KeyNotFoundException($"Tool with ID {toolId} not found.");

				if (tool.Status == ToolStatus.Maintenance || tool.Status == ToolStatus.Retired)
				{
					throw new InvalidOperationException($"Tool with ID {toolId} is not available for booking.");
				}

				var hasConflict = await _repo.HasBookingConflictAsync(toolId, addBookingDto.StartAt, addBookingDto.EndAt, ct);
				if (hasConflict)
				{
					throw new InvalidOperationException($"Tool with ID {toolId} is already booked in the requested time window.");
				}
			}

			var booking = new Booking
			{
				UserId = addBookingDto.UserId,
				StartAt = addBookingDto.StartAt,
				EndAt = addBookingDto.EndAt,
				Status = BookingStatus.Reserved,
				BookingItems = addBookingDto.ToolIds
					.Select(tid => new BookingItem { ToolId = tid })
					.ToList()
			};

			var bookingId = await _repo.AddAsync(booking, ct);
			if (bookingId == null)
				throw new Exception("Failed to create booking.");

			await _unitOfWork.SaveChangesAsync(ct);
			return bookingId;
		}


		public async Task UpdateAsync(UpdateBookingDto bookingDto, Guid bookingId, CancellationToken ct = default)
		{
			var existing = await _repo.GetByIdAsync(bookingId, ct);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
			}

			_mapper.Map(bookingDto, existing);
			await _unitOfWork.SaveChangesAsync(ct);
		}

		public async Task DeleteAsync(Guid bookingId, CancellationToken ct = default)
		{
			var existing = await _repo.GetByIdAsync(bookingId, ct);
			if (existing == null)
			{
				throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
			}

			await _repo.DeleteAsync(existing, ct);
			await _unitOfWork.SaveChangesAsync(ct);
		}

		public async Task<ApiResponse<BookingDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			var booking = await _repo.GetByIdAsync(id, ct);
			if (booking == null)
			{
				throw new KeyNotFoundException($"Booking with ID {id} not found.");
			}

			var dto = _mapper.Map<BookingDto>(booking);
			return new ApiResponse<BookingDto?>
			{
				IsError = false,
				Data = dto,
				Message = "Booking retrieved successfully"
			};
		}
		public async Task<ApiResponse> ReturnBookingAsync(Guid bookingId, CancellationToken ct = default)
		{
			string responseMessage = "Booking returned successfully";

			var booking = await _repo.GetWithItemsAsync(bookingId, ct);
			if (booking == null)
			{
				throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
			}

			if(booking.EndAt < DateTime.UtcNow)
			{
				responseMessage = "Overdue Booking returned successfully";
			}

			// Mark booking as returned
			booking.Status = BookingStatus.Returned;

			// Mark all tools as available
			foreach (var item in booking.BookingItems)
			{
				var tool = await _toolRepo.GetByIdAsync(item.ToolId, ct);
				if (tool != null)
				{
					tool.Status = ToolStatus.Available;
				}
			}

			await _unitOfWork.SaveChangesAsync(ct);
			return new ApiResponse
			{
				IsError = false,
				Message = responseMessage
			};
		}

		public async Task<ApiResponse<IEnumerable<BookingDto>>> GetAllAsync(CancellationToken ct = default)
		{
			var bookings = await _repo.GetAllAsync(ct);
			if (!bookings.Any())
			{
				throw new KeyNotFoundException("No bookings found.");
			}

			var dtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
			return new ApiResponse<IEnumerable<BookingDto>>
			{
				IsError = false,
				Data = dtos,
				Message = "Bookings retrieved successfully"
			};
		}
		public async Task<ApiResponse<IEnumerable<BookingDto>>> GetByUserAsync(string userId, CancellationToken ct = default)
		{
			var bookings = await _repo.GetByUserAsync(userId, ct);
			if (!bookings.Any())
				throw new KeyNotFoundException($"No bookings found for user {userId}.");

			var dtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
			return new ApiResponse<IEnumerable<BookingDto>>
			{
				IsError = false,
				Data = dtos,
				Message = "Bookings retrieved successfully"
			};
		}

		public async Task<ApiResponse<IEnumerable<BookingDto>>> GetByStatusAsync(BookingStatus status, CancellationToken ct = default)
		{
			var bookings = await _repo.GetByStatusAsync(status, ct);
			if (!bookings.Any())
				throw new KeyNotFoundException($"No bookings found with status {status}.");

			var dtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
			return new ApiResponse<IEnumerable<BookingDto>>
			{
				IsError = false,
				Data = dtos,
				Message = "Bookings retrieved successfully"
			};
		}

		public async Task<ApiResponse<IEnumerable<BookingDto>>> GetActiveAsync(CancellationToken ct = default)
		{
			var bookings = await _repo.GetActiveAsync(ct);
			if (!bookings.Any())
				throw new KeyNotFoundException("No active bookings found.");

			var dtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
			return new ApiResponse<IEnumerable<BookingDto>>
			{
				IsError = false,
				Data = dtos,
				Message = "Active bookings retrieved successfully"
			};
		}

		public async Task<ApiResponse<BookingWithItemsDto?>> GetWithItemsAsync(Guid id, CancellationToken ct = default)
		{
			var booking = await _repo.GetWithItemsAsync(id, ct);
			if (booking == null)
				throw new KeyNotFoundException($"Booking with ID {id} not found.");

			var dto = _mapper.Map<BookingWithItemsDto>(booking);
			return new ApiResponse<BookingWithItemsDto?>
			{
				IsError = false,
				Data = dto,
				Message = "Booking with items retrieved successfully"
			};
		}

		public async Task<ApiResponse> PickupBookingAsync(Guid bookingId, CancellationToken ct = default)
		{
			var booking = await _repo.GetWithItemsAsync(bookingId, ct);
			if (booking == null)
			{
				throw new KeyNotFoundException($"Booking with ID {bookingId} not found.");
			}

			if (booking.Status != BookingStatus.Reserved)
			{
				throw new InvalidOperationException("Only reserved bookings can be picked up.");
			}

			// Normalize DB value to UTC
			var startAtUtc = DateTime.SpecifyKind(booking.StartAt, DateTimeKind.Utc);
			if (startAtUtc > DateTime.UtcNow)
			{
				throw new InvalidOperationException("Cannot pick up before the booking start time.");
			}

			booking.Status = BookingStatus.Active;

			foreach (var item in booking.BookingItems)
			{
				var tool = await _toolRepo.GetByIdAsync(item.ToolId, ct);
				if (tool != null)
					tool.Status = ToolStatus.Borrowed;
			}

			await _unitOfWork.SaveChangesAsync(ct);

			return new ApiResponse
			{
				IsError = false,
				Message = "Booking picked up successfully"
			};
		}


	}
}
