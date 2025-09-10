using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Repositories
{
	[TestClass]
	public class BookingRepositoryTests
	{
		private ApplicationDbContext GetInMemoryDbContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
				.Options;

			return new ApplicationDbContext(options);
		}

		[TestMethod]
		public async Task AddBooking_Should_Add_Booking_To_DbContext()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingRepository(context);

			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				UserId = "user1",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(1),
				Status = BookingStatus.Reserved
			};

			// Act
			await repo.AddAsync(booking, ct);
			await context.SaveChangesAsync(ct);

			// Assert
			var savedBooking = await repo.GetByIdAsync(booking.Id, ct);
			Assert.IsNotNull(savedBooking);
			Assert.AreEqual("user1", savedBooking.UserId);
			Assert.AreEqual(BookingStatus.Reserved, savedBooking.Status);
		}
	}
}
