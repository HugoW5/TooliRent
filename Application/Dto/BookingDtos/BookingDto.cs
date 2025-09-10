using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.BookingDtos
{
	public class BookingDto
	{
		public Guid Id { get; set; }
		public string UserId { get; set; } = null!;
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public BookingStatus Status { get; set; }
	}
}
