using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
	public class Booking
	{
		public Guid Id { get; set; }

		public string UserId { get; set; } = null!; // FK to IdentityUser
		public ApplicationUser User { get; set; } = null!;

		public DateTime StartAt { get; set; }
		public DateTime EndAt { get; set; }
		public BookingStatus Status { get; set; } = BookingStatus.Reserved;

		public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
	}
}
