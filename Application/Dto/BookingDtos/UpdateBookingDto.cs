using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.BookingDtos
{

	public class UpdateBookingDto
	{
		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public BookingStatus Status { get; set; }
	}
}
