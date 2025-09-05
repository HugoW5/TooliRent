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
		Task<Tool?> GetByIdAsync(Guid id);
		Task<IEnumerable<Tool>> GetAllAsync();
		Task AddAsync(Tool tool);
		Task UpdateAsync(Tool tool);
		Task DeleteAsync(Tool tool);

		// Domain-specific queries
		Task<IEnumerable<Tool>> GetByCategoryAsync(Guid categoryId);
		Task<IEnumerable<Tool>> GetAvailableAsync();
		Task<IEnumerable<Tool>> SearchByNameAsync(string name);
	}
}
