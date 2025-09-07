using Application.Dto.ToolDtos;
using Domain.Models;
using Responses;


namespace Application.Services.Interfaces
{
	public interface IToolService
	{
		Task<Guid?> AddAsync(AddToolDto addToolDto, CancellationToken ct = default);
		Task UpdateAsync(UpdateToolDto toolDto, Guid toolId, CancellationToken ct = default);
		Task DeleteAsync(Guid id, CancellationToken ct = default);
		Task<ApiResponse<ToolDto?>> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<ToolDto>>> GetAllAsync(CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<ToolDto>>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<ToolDto>>> GetAvailableAsync(CancellationToken ct = default);
		Task<ApiResponse<IEnumerable<ToolDto>>> SearchByNameAsync(string name, CancellationToken ct = default);

	}
}
