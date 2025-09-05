using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
	public class Tool
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string? Description { get; set; }

		public Guid CategoryId { get; set; }
		public Category Category { get; set; } = null!;

		public ToolStatus Status { get; set; } = ToolStatus.Available;
		public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
	}
}
