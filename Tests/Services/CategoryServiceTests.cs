using Application.Dto.CategoryDtos;
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
	}
}
