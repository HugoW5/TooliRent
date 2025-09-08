using Application.Services;
using AutoMapper;
using Domain.Interfaces.Repositories;
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

	}
}
