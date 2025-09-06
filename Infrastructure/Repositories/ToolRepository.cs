using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
			await _context.Tools.AddAsync(tool, ct);
		}

		public Task DeleteAsync(Tool tool, CancellationToken ct)
		{
			_context.Tools.Remove(tool);
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<Tool>> GetAllAsync(CancellationToken ct)
		{
			return await _context.Tools
					.Include(t => t.Category)
					.ToListAsync(ct);
		}

		public async Task<IEnumerable<Tool>> GetAvailableAsync(CancellationToken ct)
		{
			return await _context.Tools.Where(t => t.Status == ToolStatus.Available).ToListAsync(ct);
		}

		public async Task<IEnumerable<Tool>> GetByCategoryAsync(Guid categoryId, CancellationToken ct)
		{
			return await _context.Tools
				.Where(t => t.CategoryId == categoryId)
				.ToListAsync(ct);
		}

		public async Task<Tool?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			return await _context.Tools.FindAsync(new object[] {id}, ct);
		}

		public async Task<IEnumerable<Tool>> SearchByNameAsync(string name, CancellationToken ct)
		{
			return await _context.Tools
				.Where(t => EF.Functions.Like(t.Name, $"%{name}%"))
				.ToListAsync(ct);
		}

		public  Task UpdateAsync(Tool tool, CancellationToken ct)
		{
			_context.Tools.Update(tool);
			return Task.CompletedTask;
		}
	}
}
