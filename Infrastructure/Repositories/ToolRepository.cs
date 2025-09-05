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

		public async Task AddAsync(Tool tool)
		{
			await _context.Tools.AddAsync(tool);
		}

		public Task DeleteAsync(Tool tool)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> GetAvailableAsync()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> GetByCategoryAsync(Guid categoryId)
		{
			throw new NotImplementedException();
		}

		public Task<Tool?> GetByIdAsync(Guid id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Tool>> SearchByNameAsync(string name)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(Tool tool)
		{
			throw new NotImplementedException();
		}
	}
}
