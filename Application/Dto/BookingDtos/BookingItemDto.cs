using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.BookingDtos
{

	public class BookingItemDto
	{
		public Guid Id { get; set; }
		public Guid ToolId { get; set; }
		public string ToolName { get; set; } = string.Empty;
	}
}
