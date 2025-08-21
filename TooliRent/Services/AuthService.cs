using Microsoft.AspNetCore.Identity;
using TooliRent.Dto;
using TooliRent.Dto.AuthDtos;
using TooliRent.Models;
using TooliRent.Repositories.Interfaces;
using TooliRent.Services.Interfaces;

namespace TooliRent.Services
{
	public class AuthService : IAuthService
	{
		private readonly IAuthRepository _repo;
		private readonly ITokenService _tokenService;
		private readonly IConfiguration _config;

		public AuthService(IAuthRepository repo, ITokenService tokenService, IConfiguration config)
		{
			_repo = repo;
			_tokenService = tokenService;
			_config = config;
		}

		public async Task<(string Token, string RefreshToken)> RegisterAsync(RegisterDto dto)
		{
			if (dto.Password != dto.ConfirmPassword)
				throw new Exception("Passwords do not match.");

			var existingUser = await _repo.GetUserByEmailAsync(dto.Email);
			if (existingUser != null)
				throw new Exception("User already exists.");

			var user = new IdentityUser
			{
				Email = dto.Email,
				UserName = dto.Email
			};

			await _repo.CreateUserAsync(user, dto.Password);

			var token = await _tokenService.GenerateTokenAsync(user);
			var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _repo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = user.Id,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]))
			});

			return (token, refreshToken);
		}

		public async Task<(string Token, string RefreshToken)> LoginAsync(LoginDto dto)
		{
			var user = await _repo.GetUserByEmailAsync(dto.Email);
			if (user == null || !await _repo.CheckPasswordAsync(user, dto.Password))
				throw new Exception("Invalid credentials.");

			var token = await _tokenService.GenerateTokenAsync(user);
			var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _repo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = user.Id,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]))
			});

			return (token, refreshToken);
		}

		public async Task<(string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
		{
			var storedToken = await _repo.GetRefreshTokenAsync(refreshToken, ct);
			if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
			{
				//Revoked or expired tokens should not be used
				throw new Exception("Invalid refresh token.");
			}

			storedToken.IsUsed = true;
			await _repo.UpdateRefreshTokenAsync(storedToken, ct);

			var user = await _repo.GetUserByEmailAsync(storedToken.UserId);
			var token = await _tokenService.GenerateTokenAsync(user!);
			var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _repo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = newRefreshToken,
				UserId = storedToken.UserId,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]))
			});

			return (token, newRefreshToken);
		}
	}
}
