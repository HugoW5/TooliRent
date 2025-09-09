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
	public class CategoryRepositoryTests
	{
		private ApplicationDbContext GetInMemoryDbContext()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
				.Options;

			return new ApplicationDbContext(options);
		}

		[TestMethod]
		public async Task AddCategory_Should_Add_Category_To_DbContext()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new CategoryRepository(context);

			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Hardware",
				Description = "Hardware tools"
			};

			// Act
			await repo.AddAsync(category, ct);
			await context.SaveChangesAsync(ct);

			// Assert
			var savedCategory = await repo.GetByIdAsync(category.Id, ct);
			Assert.IsNotNull(savedCategory);
			Assert.AreEqual("Hardware", savedCategory.Name);
			Assert.AreEqual("Hardware tools", savedCategory.Description);
		}
		[TestMethod]
		public async Task GetAllAsync_Should_Return_All_Categories()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new CategoryRepository(context);

			var category1 = new Category { Id = Guid.NewGuid(), Name = "Hardware" };
			var category2 = new Category { Id = Guid.NewGuid(), Name = "Software" };

			await context.Categories.AddRangeAsync(category1, category2);
			await context.SaveChangesAsync(ct);

			// Act
			var categories = await repo.GetAllAsync(ct);

			// Assert
			Assert.AreEqual(2, categories.Count());
			CollectionAssert.Contains(categories.Select(c => c.Name).ToList(), "Hardware");
			CollectionAssert.Contains(categories.Select(c => c.Name).ToList(), "Software");
		}

		[TestMethod]
		public async Task SearchByNameAsync_Should_Return_Matching_Categories()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new CategoryRepository(context);

			var category = new Category { Id = Guid.NewGuid(), Name = "Hardware" };
			await context.Categories.AddAsync(category);
			await context.SaveChangesAsync(ct);

			// Act
			var result = await repo.SearchByNameAsync("Hard", ct);

			// Assert
			Assert.AreEqual(1, result.Count());
			Assert.AreEqual("Hardware", result.First().Name);
		}

		[TestMethod]
		public async Task UpdateAsync_Should_Update_Category()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new CategoryRepository(context);

			var category = new Category { Id = Guid.NewGuid(), Name = "Hardware" };
			await context.Categories.AddAsync(category);
			await context.SaveChangesAsync(ct);

			// Act
			category.Name = "UpdatedHardware";
			await repo.UpdateAsync(category, ct);
			await context.SaveChangesAsync(ct);

			// Assert
			var updatedCategory = await repo.GetByIdAsync(category.Id, ct);
			Assert.AreEqual("UpdatedHardware", updatedCategory.Name);
		}

		[TestMethod]
		public async Task DeleteAsync_Should_Remove_Category()
		{
			// Arrange
			var ct = CancellationToken.None;
			var context = GetInMemoryDbContext();
			var repo = new CategoryRepository(context);

			var category = new Category { Id = Guid.NewGuid(), Name = "Hardware" };
			await context.Categories.AddAsync(category);
			await context.SaveChangesAsync(ct);

			// Act
			await repo.DeleteAsync(category, ct);
			await context.SaveChangesAsync(ct);

			// Assert
			var deletedCategory = await repo.GetByIdAsync(category.Id, ct);
			Assert.IsNull(deletedCategory);
		}
	}
}
