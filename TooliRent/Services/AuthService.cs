using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TooliRent.Dto.AuthDtos;
using TooliRent.Models;
using TooliRent.Repositories.Interfaces;
using TooliRent.Responses;
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

		public async Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto)
		{
			if (dto.Password != dto.ConfirmPassword)
			{
				return new ApiResponse<TokenDto>
				{
					IsError = true,
					Message = "Passwords do not match.",
					Error = new ProblemDetails
					{
						Type = "about:blank",
						Title = "Password Mismatch",
						Status = 400,
						Detail = "Password and Confirm Password must be the same.",
						Instance = "/auth/register"
					}
				};
			}
			var emailValidator = new EmailAddressAttribute();
			if (!emailValidator.IsValid(dto.Email))
			{
				return new ApiResponse<TokenDto>
				{
					IsError = true,
					Message = "Invalid email format.",
					Error = new ProblemDetails
					{
						Type = "about:blank",
						Title = "Invalid Email",
						Status = 400,
						Detail = "The provided email is not valid.",
						Instance = "/auth/register"
					}
				};
			}
			var user = new IdentityUser
			{
				Email = dto.Email,
				UserName = dto.Email
			};

			var result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
			{
				return new ApiResponse<TokenDto>
				{
					IsError = true,
					Message = "User registration failed.",
					Error = new ProblemDetails
					{
						Type = "about:blank",
						Title = "Registration Failed",
						Status = 400,
						Detail = string.Join("; ", result.Errors.Select(e => e.Description)),
						Instance = "/auth/register",
					}
				};
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
				Data = new TokenDto
				{
					Token = token,
					RefreshToken = refreshToken
				},
				Message = "User registered successfully."
			};
		}

		public async Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
			{
				return new ApiResponse<TokenDto>
				{
					IsError = true,
					Message = "Invalid credentials.",
					Error = new ProblemDetails
					{
						Type = "about:blank",
						Title = "Invalid Credentials",
						Status = 401,
						Detail = "Email or password is incorrect.",
						Instance = "/auth/login"
					}
				};
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
				Message = "Login successful.",
				Data = new TokenDto { Token = token, RefreshToken = refreshToken }
			};
		}

		public async Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
		{
			var storedToken = await _tokenRepo.GetRefreshTokenAsync(refreshToken, ct);
			if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
			{
				return new ApiResponse<TokenDto>
				{
					IsError = true,
					Message = "Invalid refresh token.",
					Error = new ProblemDetails
					{
						Type = "about:blank",
						Title = "Invalid Refresh Token",
						Status = 401,
						Detail = "Refresh token is invalid or expired.",
						Instance = "/auth/refresh"
					}
				};
			}

			storedToken.IsUsed = true;
			await _tokenRepo.UpdateRefreshTokenAsync(storedToken, ct);

			var user = await _userManager.FindByIdAsync(storedToken.UserId);
			if (user == null)
			{
				return new ApiResponse<TokenDto>
				{
					IsError = true,
					Message = "User not found for this token.",
					Error = new ProblemDetails
					{
						Type = "about:blank",
						Title = "User Not Found",
						Status = 404,
						Detail = "User not found.",
						Instance = "/auth/refresh"
					}
				};
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
				Message = "Token refreshed successfully.",
				Data = new TokenDto { Token = token, RefreshToken = newRefreshToken }
			};
		}
	}
}
