using Domain.Enums;
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
	public class BookingRepository
	{
		private readonly ApplicationDbContext _context;

		public BookingRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Guid?> AddAsync(Booking booking, CancellationToken ct = default)
		{
			await _context.Bookings.AddAsync(booking, ct);
			await _context.SaveChangesAsync(ct);
			return booking.Id;
		}

		public async Task DeleteAsync(Booking booking, CancellationToken ct = default)
		{
			_context.Bookings.Remove(booking);
			await _context.SaveChangesAsync(ct);
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

	}
}
