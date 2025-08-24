using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Dto.AuthDtos;
using TooliRent.Repositories.Interfaces;
using TooliRent.Services;
using TooliRent.Services.Interfaces;

namespace Tests
{
	[TestClass]
	public class AuthServiceTests
	{
		private Mock<IRefreshTokenRepository> _tokenRepoMock = null!;
		private Mock<ITokenService> _tokenServiceMock = null!;
		private Mock<IConfiguration> _configMock = null!;
		private UserManager<IdentityUser> _userManager = null!;
		private AuthService _authService = null!;

		[TestInitialize]
		public void Setup()
		{
			_tokenRepoMock = new Mock<IRefreshTokenRepository>();
			_tokenServiceMock = new Mock<ITokenService>();
			_configMock = new Mock<IConfiguration>();

			// Mock IConfiguration["JwtSettings:RefreshTokenExpiryDays"]
			_configMock.Setup(c => c["JwtSettings:RefreshTokenExpiryDays"]).Returns("7");

			// Setup UserManager with in-memory store
			var store = new Mock<IUserStore<IdentityUser>>();
			_userManager = new UserManager<IdentityUser>(
				store.Object, null!, null!, null!, null!, null!, null!, null!, null!
			);

			// Initialize AuthService
			_authService = new AuthService(
				_tokenRepoMock.Object,
				_tokenServiceMock.Object,
				_configMock.Object,
				_userManager
			); ;
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task RegisterAsync_ShouldThrow_WhenPasswordsDoNotMatch()
		{
			// Arrange
			var dto = new RegisterDto
			{
				Email = "test@test.com",
				UserName = "testuser",
				Password = "Password123!",
				ConfirmPassword = "Password1234!" // NOT matching
			};
			// Act & Assert
			await _authService.RegisterAsync(dto);


		}
	}
}
