using Application.Dto.BookingDtos;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public async Task<Guid?> AddAsync(AddBookingDto addBookingDto, CancellationToken ct = default)
		{
			// Validate tool IDs exist
			foreach (var toolId in addBookingDto.ToolIds)
			{
				var tool = await _toolRepo.GetByIdAsync(toolId, ct);
				if (tool == null)
					throw new KeyNotFoundException($"Tool with ID {toolId} not found.");
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
	}
}
