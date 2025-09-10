using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
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
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
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

			string uuid = "user1";
			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				UserId = uuid.ToString(),
				User = new Microsoft.AspNetCore.Identity.IdentityUser { Id = uuid, UserName = "user1" },
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
		[TestMethod]
		public async Task GetAllAsync_Should_Return_All_Bookings()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingRepository(context);

			var user1 = new IdentityUser { Id = "u1", UserName = "user1" };
			var user2 = new IdentityUser { Id = "u2", UserName = "user2" };

			await context.Users.AddRangeAsync(user1, user2);
			await context.SaveChangesAsync(ct);

			var booking1 = new Booking { Id = Guid.NewGuid(), UserId = user1.Id, StartAt = DateTime.UtcNow, EndAt = DateTime.UtcNow.AddDays(1) };
			var booking2 = new Booking { Id = Guid.NewGuid(), UserId = user2.Id, StartAt = DateTime.UtcNow, EndAt = DateTime.UtcNow.AddDays(2) };

			await repo.AddAsync(booking1, ct);
			await repo.AddAsync(booking2, ct);
			await context.SaveChangesAsync(ct);

			// Act
			var bookings = await repo.GetAllAsync(ct);

			// Assert
			Assert.AreEqual(2, bookings.Count());
		}

		[TestMethod]
		public async Task GetByUserAsync_Should_Return_Bookings_For_User()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingRepository(context);

			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				UserId = "user1",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(1)
			};

			await context.Bookings.AddAsync(booking);
			await context.SaveChangesAsync(ct);

			var result = await repo.GetByUserAsync("user1", ct);

			Assert.AreEqual(1, result.Count());
			Assert.AreEqual("user1", result.First().UserId);
		}

		[TestMethod]
		public async Task GetByStatusAsync_Should_Return_Bookings_With_Status()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingRepository(context);

			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				UserId = "user1",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(1),
				Status = BookingStatus.Active
			};

			await context.Bookings.AddAsync(booking);
			await context.SaveChangesAsync(ct);

			var result = await repo.GetByStatusAsync(BookingStatus.Active, ct);

			Assert.AreEqual(1, result.Count());
			Assert.AreEqual(BookingStatus.Active, result.First().Status);
		}

		[TestMethod]
		public async Task GetActiveAsync_Should_Return_Active_Bookings()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingRepository(context);

			var now = DateTime.UtcNow;
			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				UserId = "user1",
				StartAt = now.AddHours(-1),
				EndAt = now.AddHours(1),
				Status = BookingStatus.Active
			};

			await context.Bookings.AddAsync(booking);
			await context.SaveChangesAsync(ct);

			var result = await repo.GetActiveAsync(ct);

			Assert.AreEqual(1, result.Count());
			Assert.AreEqual("user1", result.First().UserId);
		}
	}
}
