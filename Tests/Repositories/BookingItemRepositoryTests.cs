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
	public class BookingItemRepositoryTests
	{
		private ApplicationDbContext GetInMemoryDbContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			return new ApplicationDbContext(options);
		}

		private async Task<(Booking, Tool)> SeedBookingAndTool(ApplicationDbContext context)
		{
			var booking = new Booking
			{
				Id = Guid.NewGuid(),
				UserId = "user1",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(1),
				Status = Domain.Enums.BookingStatus.Active
			};
			var tool = new Tool
			{
				Id = Guid.NewGuid(),
				Name = "Hammer",
				Status = Domain.Enums.ToolStatus.Available
			};

			await context.Bookings.AddAsync(booking);
			await context.Tools.AddAsync(tool);
			await context.SaveChangesAsync();

			return (booking, tool);
		}


		[TestMethod]
		public async Task AddBookingItem_Should_Add_Item_To_DbContext()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingItemRepository(context);

			var (booking, tool) = await SeedBookingAndTool(context);

			var item = new BookingItem
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				ToolId = tool.Id
			};

			await repo.AddAsync(item, ct);

			var savedItem = await repo.GetByIdAsync(item.Id, ct);
			Assert.IsNotNull(savedItem);
			Assert.AreEqual(booking.Id, savedItem.BookingId);
			Assert.AreEqual(tool.Id, savedItem.ToolId);
		}

		[TestMethod]
		public async Task GetByBookingAsync_Should_Return_Items_For_Booking()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingItemRepository(context);

			var (booking, tool) = await SeedBookingAndTool(context);

			var item1 = new BookingItem { Id = Guid.NewGuid(), BookingId = booking.Id, ToolId = tool.Id };
			var item2 = new BookingItem { Id = Guid.NewGuid(), BookingId = booking.Id, ToolId = tool.Id };

			await context.BookingItems.AddRangeAsync(item1, item2);
			await context.SaveChangesAsync(ct);

			var items = await repo.GetByBookingAsync(booking.Id, ct);
			Assert.AreEqual(2, items.Count());
		}

		[TestMethod]
		public async Task UpdateAsync_Should_Update_BookingItem()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingItemRepository(context);

			var (booking, tool) = await SeedBookingAndTool(context);

			var item = new BookingItem
			{
				Id = Guid.NewGuid(),
				BookingId = booking.Id,
				ToolId = tool.Id
			};

			await repo.AddAsync(item, ct);

			// Update ToolId (create new tool)
			var newTool = new Tool { Id = Guid.NewGuid(), Name = "Screwdriver", Status = Domain.Enums.ToolStatus.Available };
			await context.Tools.AddAsync(newTool);
			await context.SaveChangesAsync();

			item.ToolId = newTool.Id;
			await repo.UpdateAsync(item, ct);

			var updatedItem = await repo.GetByIdAsync(item.Id, ct);
			Assert.AreEqual(newTool.Id, updatedItem.ToolId);
		}

		[TestMethod]
		public async Task DeleteAsync_Should_Remove_BookingItem()
		{
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new BookingItemRepository(context);

			var (booking, tool) = await SeedBookingAndTool(context);

			var item = new BookingItem { Id = Guid.NewGuid(), BookingId = booking.Id, ToolId = tool.Id };
			await context.BookingItems.AddAsync(item);
			await context.SaveChangesAsync(ct);

			await repo.DeleteAsync(item, ct);
			await context.SaveChangesAsync(ct);
			var deletedItem = await repo.GetByIdAsync(item.Id, ct);
			Assert.IsNull(deletedItem);
		}
	}
}
