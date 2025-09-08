using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Repositories
{
	[TestClass]
	public class ToolRepositoryTests
	{
		private ApplicationDbContext GetInMemoryDbContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
				.Options;

			return new ApplicationDbContext(options);
		}

		[TestMethod]
		public async Task AddTool_Should_Add_Tool_To_DbContext()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new ToolRepository(context);
			var uow = new UnitOfWork(context);


			var tool = new Tool
			{
				Id = Guid.NewGuid(),
				Name = "Hammer",
				Status = ToolStatus.Available,
				Category = new Category
				{
					Id = new Guid(),
					Name = "saker"
				}
			};

			// Act
			await repo.AddAsync(tool, ct);
			await uow.SaveChangesAsync(ct);

			// Assert
			var savedTool = await repo.GetByIdAsync(tool.Id, ct);
			Assert.IsNotNull(savedTool);
			Assert.AreEqual("Hammer", savedTool.Name);
		}

		[TestMethod]
		public async Task GetAvailableAsync_Should_Return_Only_Available_Tools()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new ToolRepository(context);

			var availableTool = new Tool { Id = Guid.NewGuid(), Name = "Drill", Status = ToolStatus.Available };
			var unavailableTool = new Tool { Id = Guid.NewGuid(), Name = "Saw", Status = ToolStatus.Borrowed };

			await context.Tools.AddRangeAsync(availableTool, unavailableTool);
			await context.SaveChangesAsync(ct);

			// Act
			var availableTools = await repo.GetAvailableAsync(ct);

			// Assert
			Assert.AreEqual(1, availableTools.Count());
			Assert.AreEqual("Drill", availableTools.First().Name);
		}

		[TestMethod]
		public async Task UnitOfWork_SaveChanges_Should_Persist_Changes()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var uow = new UnitOfWork(context);

			var tool = new Tool { Id = Guid.NewGuid(), Name = "Screwdriver", Status = ToolStatus.Available };

			await uow.Tools.AddAsync(tool, ct);

			// Act
			await uow.SaveChangesAsync(ct);

			// Assert
			var savedTool = await context.Tools.FindAsync(new object[] { tool.Id }, ct);
			Assert.IsNotNull(savedTool);
			Assert.AreEqual("Screwdriver", savedTool.Name);
		}
	}
}
