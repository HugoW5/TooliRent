using Application.Dto.BookingDtos;
using Application.Services;
using AutoMapper;
using Domain.Enums;
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
	public class BookingServiceTests
	{
		private Mock<IBookingRepository> _repoMock = null!;
		private Mock<IToolRepository> _toolRepoMock = null!;
		private Mock<IMapper> _mapperMock = null!;
		private Mock<IUnitOfWork> _uowMock = null!;
		private BookingService _service = null!;

		[TestInitialize]
		public void Setup()
		{
			_repoMock = new Mock<IBookingRepository>();
			_toolRepoMock = new Mock<IToolRepository>();
			_mapperMock = new Mock<IMapper>();
			_uowMock = new Mock<IUnitOfWork>();

			// Setup default mapping behavior (optional, can override in individual tests)
			_mapperMock.Setup(m => m.Map<BookingDto>(It.IsAny<Domain.Models.Booking>()))
					   .Returns((Domain.Models.Booking b) => new BookingDto { Id = b.Id, UserId = b.UserId });

			_mapperMock.Setup(m => m.Map<BookingWithItemsDto>(It.IsAny<Domain.Models.Booking>()))
					   .Returns((Domain.Models.Booking b) => new BookingWithItemsDto { Id = b.Id, UserId = b.UserId });

			_service = new BookingService(
				_repoMock.Object,
				_toolRepoMock.Object,
				_mapperMock.Object,
				_uowMock.Object
			);
		}

		[TestMethod]
		public async Task AddAsync_ShouldReturnBookingId_WhenAllToolsAvailable()
		{
			// Arrange
			var bookingDto = new AddBookingDto
			{
				UserId = "user1",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(1),
				ToolIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
			};

			_toolRepoMock.Setup(tr => tr.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Guid id, CancellationToken ct) => new Tool { Id = id, Status = ToolStatus.Available });

			var bookingId = Guid.NewGuid();
			_repoMock.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(bookingId);

			// Act
			var result = await _service.AddAsync(bookingDto);

			// Assert
			Assert.AreEqual(bookingId, result);
			_uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		[ExpectedException(typeof(KeyNotFoundException))]
		public async Task AddAsync_ShouldThrow_WhenToolDoesNotExist()
		{
			// Arrange
			var bookingDto = new AddBookingDto
			{
				UserId = "user1",
				StartAt = DateTime.UtcNow,
				EndAt = DateTime.UtcNow.AddDays(1),
				ToolIds = new List<Guid> { Guid.NewGuid() }
			};

			_toolRepoMock.Setup(tr => tr.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Tool)null!);

			// Act
			await _service.AddAsync(bookingDto);
		}
	}
}
