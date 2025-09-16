using Application.Dto.CategoryDtos;
using Application.Dto.ToolDtos;
using Application.Services;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Services
{
	[TestClass]
	public class CategoryServiceTests
	{
		private Mock<ICategoryRepository> _repoMock = null!;
		private Mock<IMapper> _mapperMock = null!;
		private Mock<IUnitOfWork> _uowMock = null!;
		private CategoryService _service = null!;

		[TestInitialize]
		public void Setup()
		{
			_repoMock = new Mock<ICategoryRepository>();
			_mapperMock = new Mock<IMapper>();
			_uowMock = new Mock<IUnitOfWork>();

			_service = new CategoryService(_repoMock.Object, _mapperMock.Object, _uowMock.Object);
		}

		[TestMethod]
		public async Task AddAsync_ShouldReturnId_WhenCategoryAdded()
		{
			// Arrange
			var dto = new AddCategoryDto { Name = "Hardware", Description = "Hardware tools" };
			var category = new Category { Id = Guid.NewGuid(), Name = "Hardware" };
			var categoryId = category.Id;

			_mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);
			_repoMock.Setup(r => r.AddAsync(category, It.IsAny<CancellationToken>()))
					 .ReturnsAsync(categoryId);

			// Act
			var result = await _service.AddAsync(dto);

			// Assert
			Assert.AreEqual(categoryId, result);
			_uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public async Task AddAsync_ShouldThrow_WhenRepositoryReturnsNull()
		{
			// Arrange
			var dto = new AddCategoryDto { Name = "Hardware" };
			var category = new Category { Name = "Hardware" };

			_mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);
			_repoMock.Setup(r => r.AddAsync(category, It.IsAny<CancellationToken>()))
					 .ReturnsAsync((Guid?)null);

			// Act
			await _service.AddAsync(dto);
		}

		[TestMethod]
		public async Task GetAllAsync_ShouldReturnCategories_WhenCategoriesExist()
		{
			// Arrange
			var categories = new List<Category> { new Category { Id = Guid.NewGuid(), Name = "Hardware" } };
			var categoryDtos = new List<CategoryDto> { new CategoryDto { Id = categories[0].Id, Name = "Hardware" } };

			_repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(categories);
			_mapperMock.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories)).Returns(categoryDtos);

			// Act
			var response = await _service.GetAllAsync();

			// Assert
			Assert.IsFalse(response.IsError);
			Assert.AreEqual(1, response.Data.Count());
			Assert.AreEqual("Hardware", response.Data.First().Name);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public async Task GetAllAsync_ShouldThrow_WhenNoCategoriesExist()
		{
			// Arrange
			_repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
					 .ReturnsAsync(new List<Category>());

			// Act
			await _service.GetAllAsync();
		}

		[TestMethod]
		public async Task DeleteAsync_ShouldCallRepositoryAndSaveChanges()
		{
			// Arrange
			var categoryId = Guid.NewGuid();
			_repoMock.Setup(r => r.DeleteAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
					 .Returns(Task.CompletedTask);

			// Act
			await _service.DeleteAsync(categoryId);

			// Assert
			_repoMock.Verify(r => r.DeleteAsync(It.Is<Category>(c => c.Id == categoryId), It.IsAny<CancellationToken>()), Times.Once);
			_uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
		{
			// Arrange
			var category = new Category { Id = Guid.NewGuid(), Name = "Hardware" };
			var categoryDto = new CategoryDto { Id = category.Id, Name = "Hardware" };

			_repoMock.Setup(r => r.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
					 .ReturnsAsync(category);
			_mapperMock.Setup(m => m.Map<CategoryDto>(category)).Returns(categoryDto);

			// Act
			var response = await _service.GetByIdAsync(category.Id);

			// Assert
			Assert.IsFalse(response.IsError);
			Assert.AreEqual(category.Id, response.Data!.Id);
			Assert.AreEqual("Hardware", response.Data.Name);
		}
		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public async Task GetByIdAsync_ShouldThrow_WhenCategoryDoesNotExist()
		{
			// Arrange
			_repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
					 .ReturnsAsync((Category)null!);

			// Act
			await _service.GetByIdAsync(Guid.NewGuid());
		}
		[TestMethod]
		public async Task GetWithToolsAsync_ShouldReturnCategoryWithTools_WhenCategoryExists()
		{
			// Arrange
			var category = new Category
			{
				Id = Guid.NewGuid(),
				Name = "Hardware",
				Tools = new List<Tool> { new Tool { Id = Guid.NewGuid(), Name = "Hammer" } }
			};
			var categoryWithToolsDto = new CategoryWithToolsDto
			{
				Id = category.Id,
				Name = "Hardware",
				Tools = new List<ToolDto> { new ToolDto { Id = category.Tools.First().Id, Name = "Hammer" } }
			};

			_repoMock.Setup(r => r.GetWithToolsAsync(category.Id, It.IsAny<CancellationToken>()))
					 .ReturnsAsync(category);
			_mapperMock.Setup(m => m.Map<CategoryWithToolsDto>(category)).Returns(categoryWithToolsDto);

			// Act
			var response = await _service.GetWithToolsAsync(category.Id);

			// Assert
			Assert.IsFalse(response.IsError);
			Assert.AreEqual(1, response.Data!.Tools.Count());
			Assert.AreEqual("Hammer", response.Data.Tools.First().Name);
		}
	}
}
