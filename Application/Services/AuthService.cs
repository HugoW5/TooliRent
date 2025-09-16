using Application.Metrics;
using Application.Metrics.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Models;
using Dto.AuthDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Responses;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using TooliRent.Exceptions;

namespace TooliRent.Services
{
	public class AuthService : IAuthService
	{
		private readonly IRefreshTokenRepository _tokenRepo;
		private readonly ITokenService _tokenService;
		private readonly IConfiguration _config;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IAuthMetrics _metrics;

		public AuthService(
			IRefreshTokenRepository tokenRepo,
			ITokenService tokenService,
			IConfiguration config,
			UserManager<IdentityUser> userManager,
			IAuthMetrics metrics)
		{
			_tokenRepo = tokenRepo;
			_tokenService = tokenService;
			_config = config;
			_userManager = userManager;
			_metrics = metrics;
		}

		/// <summary>
		/// Registers and adds a Member role to a new user
		/// </summary>
		/// <param name="dto">Registration dto</param>
		/// <returns>returns an apiresponse with a tokendto</returns>
		/// <exception cref="ArgumentException">Invalid validaiton</exception>
		/// <exception cref="IdentityException">Falied to create user</exception>
		public async Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto)
		{
			if (dto.Password != dto.ConfirmPassword)
			{
				throw new ArgumentException("Password and Confirm Password must be the same.");
			}
			var emailValidator = new EmailAddressAttribute();
			if (!emailValidator.IsValid(dto.Email))
			{
				throw new ArgumentException("Invalid email format.");
			}
			var user = new IdentityUser
			{
				Email = dto.Email,
				UserName = dto.UserName
			};

			var result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
			{
				throw new IdentityException(result.Errors.Select(e => e.Description));
			}

			// Assign default role
			await _userManager.AddToRoleAsync(user, "Member");

			var token = await _tokenService.GenerateTokenAsync(user);
			var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _tokenRepo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = refreshToken,
				UserId = user.Id,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!))
			});

			return new ApiResponse<TokenDto>
			{
				IsError = false,
				Message = "User registered successfully.",
				Data = new TokenDto
				{
					Token = token,
					RefreshToken = refreshToken
				},
			};
		}

		/// <summary>
		/// Logs in a user and returns a token dto with refresh and token.
		/// </summary>
		/// <param name="dto">The login credentials</param>
		/// <returns>An ApiResponse with a token and a refresh token</returns>
		/// <exception cref="UnauthorizedAccessException"></exception>
		public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto)
		{
			var sw = Stopwatch.StartNew();
			_metrics.RecordAttempt("login");
			try
			{

				var user = await _userManager.FindByEmailAsync(dto.Email);
				if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
				{
					_metrics.RecordFailure("login", "invalid_credentials");
					throw new UnauthorizedAccessException("Invalid email or password.");
				}

				var token = await _tokenService.GenerateTokenAsync(user);
				var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

				await _tokenRepo.AddRefreshTokenAsync(new RefreshToken
				{
					Token = refreshToken,
					UserId = user.Id,
					ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!))
				});

				return new ApiResponse<TokenDto>
				{
					IsError = false,
					Message = "User logged in successfully.",
					Data = new TokenDto
					{
						Token = token,
						RefreshToken = refreshToken
					},
				};

			}
			finally
			{
				sw.Stop();
				_metrics.RecordDuration("login", sw.ElapsedMilliseconds);
			}
		}
		
		/// <summary>
		/// Refreshes the JWT token and returns a new token and refresh token, updates the used refresh tokenes status to used.
		/// </summary>
		/// <param name="refreshToken">JWT refresh token</param>
		/// <param name="ct">Cancellation token</param>
		/// <returns>A tokenDto with JWT token and refesh token</returns>
		/// <exception cref="UnauthorizedAccessException">Invalid or expired refresh token</exception>
		/// <exception cref="InvalidOperationException">User not matching token</exception>
		public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
		{
			var storedToken = await _tokenRepo.GetRefreshTokenAsync(refreshToken, ct);
			if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
			{
				throw new UnauthorizedAccessException("Invalid or expired refresh token.");
			}

			storedToken.IsUsed = true;
			await _tokenRepo.UpdateRefreshTokenAsync(storedToken, ct);

			var user = await _userManager.FindByIdAsync(storedToken.UserId);
			if (user == null)
			{
				throw new InvalidOperationException("User not found for this token.");
			}

			var token = await _tokenService.GenerateTokenAsync(user);
			var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

			await _tokenRepo.AddRefreshTokenAsync(new RefreshToken
			{
				Token = newRefreshToken,
				UserId = storedToken.UserId,
				ExpiryDate = DateTime.UtcNow.AddDays(double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!))
			});

			return new ApiResponse<TokenDto>
			{
				IsError = false,
				Message = "Successfully refreshed token.",
				Data = new TokenDto
				{
					Token = token,
					RefreshToken = newRefreshToken
				},
			};
		}
	}
}
