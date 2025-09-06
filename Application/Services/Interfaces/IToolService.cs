using Application.Dto.ToolDtos;


namespace Application.Services.Interfaces
{
	public interface IToolService
	{
		Task AddAsync(ToolDto toolDto, CancellationToken ct = default);
		Task UpdateAsync(ToolDto toolDto, CancellationToken ct = default);
		Task DeleteAsync(Guid id, CancellationToken ct = default);
		Task<ToolDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
		Task<IEnumerable<ToolDto>> GetAllAsync(CancellationToken ct = default);
		Task<IEnumerable<ToolDto>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);
		Task<IEnumerable<ToolDto>> GetAvailableAsync(CancellationToken ct = default);
		Task<IEnumerable<ToolDto>> SearchByNameAsync(string name, CancellationToken ct = default);

	}
}
