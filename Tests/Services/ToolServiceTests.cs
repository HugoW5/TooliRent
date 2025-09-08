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
	public class ToolServiceTests
	{
		private Mock<IToolRepository> _repoMock = null!;
		private Mock<IMapper> _mapperMock = null!;
		private Mock<IUnitOfWork> _uowMock = null!;
		private ToolService _service = null!;

		[TestInitialize]
		public void Setup()
		{
			_repoMock = new Mock<IToolRepository>();
			_mapperMock = new Mock<IMapper>();
			_uowMock = new Mock<IUnitOfWork>();

			_service = new ToolService(_repoMock.Object, _mapperMock.Object, _uowMock.Object);
		}

		[TestMethod]
		public async Task AddAsync_ShouldReturnId_WhenCategoryExistsAndToolAdded()
		{
			// Arrange
			var dto = new AddToolDto { CategoryId = Guid.NewGuid(), Name = "Hammer" };
			var tool = new Tool { Id = Guid.NewGuid(), Name = "Hammer" };
			var toolId = tool.Id;

			_uowMock.Setup(u => u.CategoryExistsAsync(dto.CategoryId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(true); // Category does not exist but return true anyway

			_mapperMock.Setup(m => m.Map<Tool>(dto)).Returns(tool);
			_repoMock.Setup(r => r.AddAsync(tool, It.IsAny<CancellationToken>()))
				.ReturnsAsync(toolId);

			// Act
			var result = await _service.AddAsync(dto);

			// Assert
			Assert.AreEqual(toolId, result);
			_uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public async Task AddAsync_ShouldThrow_WhenCategoryDoesNotExist()
		{
			// Arrange
			var dto = new AddToolDto { CategoryId = Guid.NewGuid(), Name = "Hammer" };

			_uowMock.Setup(u => u.CategoryExistsAsync(dto.CategoryId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(false);

			// Act
			await _service.AddAsync(dto);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public async Task AddAsync_ShouldThrow_WhenRepositoryReturnsNull()
		{
			// Arrange
			var dto = new AddToolDto { CategoryId = Guid.NewGuid(), Name = "Hammer" };
			var tool = new Tool { Name = "Hammer" };

			_uowMock.Setup(u => u.CategoryExistsAsync(dto.CategoryId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(true);
			_mapperMock.Setup(m => m.Map<Tool>(dto)).Returns(tool);
			_repoMock.Setup(r => r.AddAsync(tool, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Guid?)null);

			// Act
			await _service.AddAsync(dto);
		}
	}
}
