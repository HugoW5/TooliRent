using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.ToolDtos
{
	public class AddToolDto
	{
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
		public Guid CategoryId { get; set; }
		public ToolStatus Status { get; set; } = ToolStatus.Available;
	}
}
