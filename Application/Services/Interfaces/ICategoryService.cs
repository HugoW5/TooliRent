using Application.Dto.CategoryDtos;
using Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
	public interface ICategoryService
	{
		Task<Guid?> AddAsync(AddCategoryDto addCategoryDto, CancellationToken ct = default);
		Task UpdateAsync(UpdateCategoryDto categoryDto, Guid categoryId, CancellationToken ct = default);
		Task DeleteAsync(Guid id, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken ct = default);
		Task<ApiResponse<CategoryDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<CategoryDto>>> SearchByNameAsync(string name, CancellationToken ct = default);
		Task<ApiResponse<CategoryWithToolsDto?>> GetWithToolsAsync(Guid id, CancellationToken ct = default);

		}
}
