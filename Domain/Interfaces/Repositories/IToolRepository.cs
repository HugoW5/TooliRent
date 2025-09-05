using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
	public interface IToolRepository
	{
		// Basic CRUD
		Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<IEnumerable<Tool>> GetAllAsync(CancellationToken ct);
		Task AddAsync(Tool tool, CancellationToken ct);
		Task UpdateAsync(Tool tool, CancellationToken ct);
		Task DeleteAsync(Tool tool, CancellationToken ct);

		// Domain-specific queries
		Task<IEnumerable<Tool>> GetByCategoryAsync(Guid categoryId, CancellationToken ct);
		Task<IEnumerable<Tool>> GetAvailableAsync(CancellationToken ct);
		Task<IEnumerable<Tool>> SearchByNameAsync(string name, CancellationToken ct);
	}
}
