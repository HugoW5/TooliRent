using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
	public interface IBookingItemRepository
	{
		Task<BookingItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<IEnumerable<BookingItem>> GetByBookingAsync(Guid bookingId, CancellationToken ct = default);
		Task<Guid?> AddAsync(BookingItem item, CancellationToken ct = default);
		Task UpdateAsync(BookingItem item, CancellationToken ct = default);
		Task DeleteAsync(BookingItem item, CancellationToken ct = default);
	}
}
