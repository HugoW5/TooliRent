using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static Domain.Enums.ToolStatus;

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

		public async Task<IEnumerable<Tool>> GetAllAsync(CancellationToken ct)
		{
			return await _context.Tools.ToListAsync(ct);
		}

		public async Task<IEnumerable<Tool>> GetAvailableAsync(CancellationToken ct)
		{
			return _context.Tools.Where(t => t.Status == ToolStatus.Available);
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
