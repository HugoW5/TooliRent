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

		/// <summary>
		/// Creates a new booking for the speficied user. If the users role is member they do not need to speficy 
		/// a user id in the add booking dto because the methods gets the userid form the Claims.
		/// If the requesters role is Admin they need to speficy a user id of a member
		/// </summary>
		/// <param name="addBookingDto">The booking dto</param>
		/// <param name="user">Whoever is loggedin</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns>The id of the created booking</returns>
		/// <exception cref="UnauthorizedAccessException">User is unauthorized</exception>
		/// <exception cref="ArgumentException">Invalid data</exception>
		/// <exception cref="KeyNotFoundException">Tool with the speficied id is not found</exception>
		/// <exception cref="InvalidOperationException">The tool is already booked</exception>
		/// <exception cref="Exception">Generic exception</exception>
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
				{
					throw new KeyNotFoundException($"Tool with ID {toolId} not found.");
				}

				if (tool.Status != ToolStatus.Available)
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

		/// <summary>
		/// Updates a booking, should only be used by Admins
		/// </summary>
		/// <param name="bookingDto">New data for the booking we are updating</param>
		/// <param name="bookingId">Id of the booking that we are updating</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns>Nothing</returns>
		/// <exception cref="KeyNotFoundException">Booking does not exist</exception>
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

		/// <summary>
		/// Deletes a booking, should only be used by Admins 
 		/// </summary>
		/// <param name="bookingId">The id of the booking we are deleting</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
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

		/// <summary>
		/// Gets a booking by id, should only be used by Admins
		/// </summary>
		/// <param name="id">The </param>
		/// <param name="ct"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
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
		
		/// <summary>
		/// Returns a booking and marks all tools as available again,
		/// should be used by Members when they returns their tools
		/// </summary>
		/// <param name="bookingId"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
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

		/// <summary>
		/// Gets all bookings, should only be used by Admins
		/// </summary>
		/// <param name="ct"></param>
		/// <returns>Returns all bookings</returns>
		/// <exception cref="KeyNotFoundException">No bookings found</exception>
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
		
		/// <summary>
		/// Retrieves a collection of bookings associated with the specified user.
		/// </summary>
		/// <remarks>Use this method to retrieve all bookings associated with a specific user. If no bookings are
		/// found,  a <see cref="KeyNotFoundException"/> is thrown. Ensure that the <paramref name="userId"/> is valid  and
		/// not null or empty before calling this method.</remarks>
		/// <param name="userId">The unique identifier of the user whose bookings are to be retrieved. Cannot be null or empty.</param>
		/// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
		/// <returns>An <see cref="ApiResponse{T}"/> containing an enumerable collection of <see cref="BookingDto"/> objects 
		/// representing the user's bookings. The response includes a success message if bookings are found.</returns>
		/// <exception cref="KeyNotFoundException">Thrown if no bookings are found for the specified user.</exception>
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

		/// <summary>
		/// Retrieves a collection of bookings that match the specified status.
		/// </summary>
		/// <param name="status">The status of the bookings to retrieve.</param>
		/// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
		/// <returns>An <see cref="ApiResponse{T}"/> containing an enumerable collection of <see cref="BookingDto"/> objects  that
		/// represent the bookings with the specified status.</returns>
		/// <exception cref="KeyNotFoundException">Thrown if no bookings are found with the specified <paramref name="status"/>.</exception>
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

		/// <summary>
		/// Gets all active bookings, should be used by Admins
		/// </summary>
		/// <param name="ct"></param>
		/// <returns>Rerturns the an Ienumarble of bookingdtos</returns>
		/// <exception cref="KeyNotFoundException"></exception>
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

		/// <summary>
		/// Retrieves a booking along with its associated items by the specified identifier.
		/// </summary>
		/// <remarks>The returned <see cref="ApiResponse{T}"/> will have <see cref="ApiResponse{T}.IsError"/> set to
		/// <see langword="false"/>  and <see cref="ApiResponse{T}.Message"/> set to "Booking with items retrieved
		/// successfully" if the operation succeeds.</remarks>
		/// <param name="id">The unique identifier of the booking to retrieve.</param>
		/// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
		/// <returns>An <see cref="ApiResponse{T}"/> containing the booking and its associated items as a <see
		/// cref="BookingWithItemsDto"/>.  If no booking is found, the method throws a <see cref="KeyNotFoundException"/>.</returns>
		/// <exception cref="KeyNotFoundException">Thrown if a booking with the specified <paramref name="id"/> is not found.</exception>
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

		/// <summary>
		/// Picks up the booking by marking the tools as borrowed and the booking as active
		/// </summary>
		/// <param name="bookingId">The booking id</param>
		/// <param name="ct"></param>
		/// <returns>A Success message</returns>
		/// <exception cref="KeyNotFoundException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
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
