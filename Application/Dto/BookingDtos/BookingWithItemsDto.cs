using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.BookingDtos
{
	public class BookingWithItemsDto : BookingDto
	{
		public IEnumerable<BookingItemDto> BookingItems { get; set; } = new List<BookingItemDto>();
	}
}
