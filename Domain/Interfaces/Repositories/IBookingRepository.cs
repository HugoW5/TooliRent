using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
	public interface IBookingRepository
	{
		// CRUD
		Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<IEnumerable<Booking>> GetAllAsync(CancellationToken ct = default);
		Task<Guid?> AddAsync(Booking booking, CancellationToken ct = default);
		Task UpdateAsync(Booking booking, CancellationToken ct = default);
		Task DeleteAsync(Booking booking, CancellationToken ct = default);

		// Domain-specific queries
		Task<IEnumerable<Booking>> GetByUserAsync(string userId, CancellationToken ct = default);
		Task<IEnumerable<Booking>> GetByStatusAsync(BookingStatus status, CancellationToken ct = default);
		Task<IEnumerable<Booking>> GetActiveAsync(CancellationToken ct = default); // StartAt <= Now <= EndAt
		Task<Booking?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
		Task<bool> HasBookingConflictAsync(Guid toolId, DateTime startAt, DateTime endAt, CancellationToken ct = default);
	}
}
