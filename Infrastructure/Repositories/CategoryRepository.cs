using Domain.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Data;
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

	}
}
