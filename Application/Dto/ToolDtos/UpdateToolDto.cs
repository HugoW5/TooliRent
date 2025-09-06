using Domain.Enums;

namespace Application.Dto.ToolDtos
{
	public class UpdateToolDto
	{
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public Guid CategoryId { get; set; }
		public ToolStatus Status { get; set; }

	}
}
