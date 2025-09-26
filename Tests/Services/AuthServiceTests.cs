using Application.Metrics;
using Application.Metrics.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Models;
using Dto.AuthDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TooliRent.Exceptions;
using TooliRent.Services;

namespace Tests.Services
{
	[TestClass]
	public class AuthServiceTests
	{
		private Mock<IRefreshTokenRepository> _tokenRepoMock = null!;
		private Mock<ITokenService> _tokenServiceMock = null!;
		private Mock<IConfiguration> _configMock = null!;
		private UserManager<ApplicationUser> _userManager = null!;
		private AuthService _authService = null!;
		private Mock<IAuthMetrics> _metricsMock = null!;


		[TestInitialize]
		public void Setup()
		{
			_tokenRepoMock = new Mock<IRefreshTokenRepository>();
			_tokenServiceMock = new Mock<ITokenService>();
			_configMock = new Mock<IConfiguration>();
			_metricsMock = new Mock<IAuthMetrics>();

			_configMock.Setup(c => c["JwtSettings:RefreshTokenExpiryDays"]).Returns("7");

			var store = new Mock<IUserStore<ApplicationUser>>();

			var passwordStore = store.As<IUserPasswordStore<ApplicationUser>>();
			passwordStore
				.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(IdentityResult.Success);
			passwordStore
				.Setup(x => x.SetPasswordHashAsync(It.IsAny<ApplicationUser>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			store.As<IUserEmailStore<ApplicationUser>>()
				.Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((string email, CancellationToken _) => null); // no user found

			store.As<IUserRoleStore<ApplicationUser>>()
				.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			_userManager = new UserManager<ApplicationUser>(
				store.Object,
				Options.Create(new IdentityOptions()),
				new PasswordHasher<ApplicationUser>(),
				new List<IUserValidator<ApplicationUser>>(),
				new List<IPasswordValidator<ApplicationUser>> { new PasswordValidator<ApplicationUser>() },
				new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(),
				null!,
				Mock.Of<ILogger<UserManager<ApplicationUser>>>()
			);

			_authService = new AuthService(
				_tokenRepoMock.Object,
				_tokenServiceMock.Object,
				_configMock.Object,
				_userManager,
				_metricsMock.Object
			);
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
		[DataRow("P!2a", DisplayName = "Too Short")]
		[DataRow("PASsword123", DisplayName = "Without non-alphanumerical character")]
		[DataRow("", DisplayName = "Empty password")]
		[DataRow("               ", DisplayName = "Whitespace")]
		public async Task RegisterAsync_ShouldThrow_WhenUnsafePassword(string password)
		{
			// Arrange
			var dto = new RegisterDto
			{
				Email = "test@email.com",
				UserName = "testuser",
				Password = password,
				ConfirmPassword = password // Unsafe password
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

		[TestMethod]
		[ExpectedException(typeof(UnauthorizedAccessException))]
		public async Task LoginAsync_ShouldThrow_WhenInvalidCredentials()
		{
			// Arrange
			var dto = new LoginDto
			{
				Email = "nonexistentuser",
				Password = "WrongPassword!"
			};
			// Act & Assert
			await _authService.LoginAsync(dto);
		}
	}
}