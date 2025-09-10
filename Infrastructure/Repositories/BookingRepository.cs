using Domain.Models;
using Infrastructure.Data;
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
	}
}
