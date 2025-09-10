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
	public class BookingItemRepository : IBookingItemRepository
	{
		private readonly ApplicationDbContext _context;

		public BookingItemRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Guid?> AddAsync(BookingItem item, CancellationToken ct = default)
		{
			await _context.BookingItems.AddAsync(item, ct);
			await _context.SaveChangesAsync(ct);
			return item.Id;
		}

		public Task DeleteAsync(BookingItem item, CancellationToken ct = default)
		{
			_context.BookingItems.Remove(item);
			return Task.CompletedTask;
		}

		public async Task<BookingItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			return await _context.BookingItems
				.Include(bi => bi.Tool)
				.Include(bi => bi.Booking)
				.FirstOrDefaultAsync(bi => bi.Id == id, ct);
		}

		public async Task<IEnumerable<BookingItem>> GetByBookingAsync(Guid bookingId, CancellationToken ct = default)
		{
			return await _context.BookingItems
				.Where(bi => bi.BookingId == bookingId)
				.Include(bi => bi.Tool)
				.AsNoTracking()
				.ToListAsync(ct);
		}

		public Task UpdateAsync(BookingItem item, CancellationToken ct = default)
		{
			_context.BookingItems.Update(item);
			return Task.CompletedTask;
		}
	}
}
