using Domain.Interfaces.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _context;
		private readonly IToolRepository _toolRepository;

		public UnitOfWork(ApplicationDbContext context)
		{
			_context = context;
			_toolRepository = new ToolRepository(_context);
		}

		public IToolRepository Tools => _toolRepository;

		public async Task<int> SaveChangesAsync(CancellationToken ct)
		{
			return await _context.SaveChangesAsync(ct);
		}
	}
}
