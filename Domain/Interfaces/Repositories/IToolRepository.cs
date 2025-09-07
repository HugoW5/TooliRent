using Domain.Models;

namespace Domain.Interfaces.Repositories
{
	public interface IToolRepository
	{
		// Basic CRUD
		Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<IEnumerable<Tool>> GetAllAsync(CancellationToken ct = default);
		Task AddAsync(Tool tool, CancellationToken ct = default);
		Task UpdateAsync(Tool tool, CancellationToken ct = default);
		Task DeleteAsync(Tool tool, CancellationToken ct = default);

		// Domain-specific queries
		Task<IEnumerable<Tool>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
		Task<IEnumerable<Tool>> GetAvailableAsync(CancellationToken ct = default);
		Task<IEnumerable<Tool>> SearchByNameAsync(string name, CancellationToken ct = default);
	}
}
