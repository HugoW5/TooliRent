using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
	public class BookingRepository : IBookingRepository
	{
		private readonly ApplicationDbContext _context;

		public BookingRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Guid?> AddAsync(Booking booking, CancellationToken ct = default)
		{
			await _context.Bookings.AddAsync(booking, ct);
			return booking.Id;
		}

		public  Task DeleteAsync(Booking booking, CancellationToken ct = default)
		{
			_context.Bookings.Remove(booking);
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken ct = default)
		{
			return await _context.Bookings
				.Include(b => b.User)
				.Include(b => b.BookingItems)
					.ThenInclude(bi => bi.Tool)
				.AsNoTracking()
				.ToListAsync(ct);
		}

		public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _context.Bookings
				.Include(b => b.User)
				.Include(b => b.BookingItems)
					.ThenInclude(bi => bi.Tool)
				.FirstOrDefaultAsync(b => b.Id == id, ct);
		}

		public async Task<IEnumerable<Booking>> GetByUserAsync(string userId, CancellationToken ct = default)
		{
			return await _context.Bookings
				.Where(b => b.UserId == userId)
				.Include(b => b.BookingItems)
				.ThenInclude(bi => bi.Tool)
				.AsNoTracking()
				.ToListAsync(ct);
		}

		public async Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status, CancellationToken ct = default)
		{
			return await _context.Bookings
				.Where(b => b.Status == status)
				.Include(b => b.BookingItems)
				.ThenInclude(bi => bi.Tool)
				.AsNoTracking()
				.ToListAsync(ct);
		}

		public async Task<IEnumerable<Booking>> GetActiveAsync(CancellationToken ct = default)
		{
			var now = DateTime.UtcNow;
			return await _context.Bookings
				.Where(b => b.StartAt <= now && b.EndAt >= now)
				.Include(b => b.BookingItems)
				.ThenInclude(bi => bi.Tool)
				.AsNoTracking()
				.ToListAsync(ct);
		}

		public async Task<Booking?> GetWithItemsAsync(Guid id, CancellationToken ct = default)
		{
			return await _context.Bookings
				.Include(b => b.BookingItems)
				.ThenInclude(bi => bi.Tool)
				.FirstOrDefaultAsync(b => b.Id == id, ct);
		}

		public Task UpdateAsync(Booking booking, CancellationToken ct = default)
		{
			_context.Bookings.Update(booking);
			return Task.CompletedTask;
		}

	}
}
