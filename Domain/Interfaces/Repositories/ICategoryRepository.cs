using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories
{
	public interface ICategoryRepository
	{
		Task<Guid?> AddAsync(Category category, CancellationToken ct);
		Task DeleteAsync(Category category, CancellationToken ct);
		Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct);
		Task<Category?> GetByIdAsync(Guid id, CancellationToken ct);
		Task<IEnumerable<Category>> SearchByNameAsync(string name, CancellationToken ct);
		Task UpdateAsync(Category category, CancellationToken ct);
		Task<Category?> GetWithToolsAsync(Guid id, CancellationToken ct);
	}
}
