using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Dto.AuthDtos;
using TooliRent.Exceptions;
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
			var passwordStore = store.As<IUserPasswordStore<IdentityUser>>();
			passwordStore
				.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(IdentityResult.Success);
			passwordStore
				.Setup(x => x.SetPasswordHashAsync(It.IsAny<IdentityUser>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
			var roleStore = store.As<IUserRoleStore<IdentityUser>>();
			// Mock a working UserManager
			_userManager = new UserManager<IdentityUser>(
				store.Object,
				Options.Create(new IdentityOptions()),
				new PasswordHasher<IdentityUser>(),
				new List<IUserValidator<IdentityUser>>(),
				new List<IPasswordValidator<IdentityUser>> { new PasswordValidator<IdentityUser>() },
				new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(),
				null!,
				Mock.Of<ILogger<UserManager<IdentityUser>>>()
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

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public async Task RegisterAsync_ShouldThrow_WhenInvalidEmailFormat()
		{
			// Arrange
			var dto = new RegisterDto
			{
				Email = "email", // Invalid email
				UserName = "testuser",
				Password = "Password123!",
				ConfirmPassword = "Password123!"
			};
			// Act & Assert
			await _authService.RegisterAsync(dto);
		}

		[TestMethod]
		[ExpectedException(typeof(IdentityException))]
		public async Task RegisterAsync_ShouldThrow_WhenUnsafePassword()
		{
			// Arrange
			var dto = new RegisterDto
			{
				Email = "test@email.com",
				UserName = "testuser",
				Password = "123",
				ConfirmPassword = "123" // Unsafe password
			};
			// Act & Assert
			await _authService.RegisterAsync(dto);
		}

		[TestMethod]
		public async Task RegisterAsync_ShouldSucceed_WhenValidInput()
		{
			// Arrange
			var dto = new RegisterDto
			{
				Email = "test@test.com",
				UserName = "testuser",
				Password = "Password123!",
				ConfirmPassword = "Password123!"
			};
			// Act
			var result = await _authService.RegisterAsync(dto);
			// Assert
			Assert.IsFalse(result.IsError);
		}
	}
}
