using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly ApplicationDbContext _context;

		public CategoryRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<Guid?> AddAsync(Category category, CancellationToken ct)
		{
			var addedCategory = await _context.Categories.AddAsync(category, ct);
			return addedCategory.Entity.Id;
		}

		public Task DeleteAsync(Category category, CancellationToken ct)
		{
			_context.Categories.Remove(category);
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct)
		{
			return await _context.Categories.ToListAsync(ct);
		}
		public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			return await _context.Categories
				.Include(c => c.Tools) // eager load tools
				.FirstOrDefaultAsync(c => c.Id == id, ct);
		}

	}
}
