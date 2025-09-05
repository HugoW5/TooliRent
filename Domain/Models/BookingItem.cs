using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
	public class BookingItem
	{
		public Guid Id { get; set; }

		public Guid BookingId { get; set; }
		public Booking Booking { get; set; } = null!;

		public Guid ToolId { get; set; }
		public Tool Tool { get; set; } = null!;
	}
}
