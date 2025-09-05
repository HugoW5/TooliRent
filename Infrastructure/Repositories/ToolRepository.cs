using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
	public class ToolRepository : IToolRepository
	{
		private readonly ApplicationDbContext _context;
		public ToolRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Tool tool, CancellationToken ct)
		{
			await _context.Tools.AddAsync(tool);
		}

		public Task DeleteAsync(Tool tool, CancellationToken ct)
		{
			_context.Tools.Remove(tool);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<Tool>> GetAllAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> GetAvailableAsync(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> GetByCategoryAsync(Guid categoryId, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> SearchByNameAsync(string name, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(Tool tool, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}
