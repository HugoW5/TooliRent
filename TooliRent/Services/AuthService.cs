using Microsoft.AspNetCore.Identity;
using TooliRent.Dto.AuthDtos;
using TooliRent.Models;
using TooliRent.Repositories.Interfaces;
using TooliRent.Services.Interfaces;

namespace TooliRent.Services
{
	public class AuthService : IAuthService
	{
		private readonly IRefreshTokenRepository _tokenRepo;
		private readonly ITokenService _tokenService;
		private readonly IConfiguration _config;
		private readonly UserManager<IdentityUser> _userManager;

		public AuthService(
			IRefreshTokenRepository tokenRepo,
			ITokenService tokenService,
			IConfiguration config,
			UserManager<IdentityUser> userManager)
		{
			_tokenRepo = tokenRepo;
			_tokenService = tokenService;
			_config = config;
			_userManager = userManager;
		}

		public async Task<(string Token, string RefreshToken)> RegisterAsync(RegisterDto dto)
		{
			if (dto.Password != dto.ConfirmPassword)
				throw new Exception("Passwords do not match.");

			var user = new IdentityUser
			{
				Email = dto.Email,
				UserName = dto.Email
			};

			var result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				throw new Exception(errors);
			}

			var token = await _tokenService.GenerateTokenAsync(user);
			var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _tokenRepo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = user.Id,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]))
			});

			return (token, refreshToken);
		}

		public async Task<(string Token, string RefreshToken)> LoginAsync(LoginDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
				throw new Exception("Invalid credentials.");

			var token = await _tokenService.GenerateTokenAsync(user);
			var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _tokenRepo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = user.Id,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]))
			});

			return (token, refreshToken);
		}

		public async Task<(string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
		{
			var storedToken = await _tokenRepo.GetRefreshTokenAsync(refreshToken, ct);
			if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
				throw new Exception("Invalid refresh token.");

			storedToken.IsUsed = true;
			await _tokenRepo.UpdateRefreshTokenAsync(storedToken, ct);

			var user = await _userManager.FindByIdAsync(storedToken.UserId);
			if (user == null)
				throw new Exception("User not found for this token.");

			var token = await _tokenService.GenerateTokenAsync(user);
			var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _tokenRepo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = newRefreshToken,
				UserId = storedToken.UserId,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]))
			});

			return (token, newRefreshToken);
		}
	}
}
