using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Dto.AuthDtos;
using TooliRent.Repositories;
using TooliRent.Services.Interfaces;

namespace TooliRent.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("register")]
		public async Task<ActionResult<ApiResponse<TokenDto>>> Register([FromBody] RegisterDto dto)
		{
			var result = await _authService.RegisterAsync(dto);
			if (result.IsError)
			{
				return BadRequest(result);
			}
			return CreatedAtAction(nameof(Register), result);
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto)
		{
			try
			{
				var (token, refreshToken) = await _authService.LoginAsync(dto);
				return Ok(new { token, refreshToken });
			}
			catch (Exception ex)
			{
				return Unauthorized(ex.Message);
			}
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshDto dto, CancellationToken ct)
		{
			try
			{
				var (token, refreshToken) = await _authService.RefreshTokenAsync(dto.RefreshToken, ct);
				return Ok(new { token, refreshToken });
			}
			catch (Exception ex)
			{
				return Unauthorized(ex.Message);
			}
		}

	}
}
