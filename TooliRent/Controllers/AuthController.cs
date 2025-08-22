using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Dto.AuthDtos;
using TooliRent.Responses;
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
		public async Task<ActionResult<ApiResponse<TokenDto>>> Login([FromBody] LoginDto dto)
		{
			var result = await _authService.LoginAsync(dto);
			if (result.IsError)
			{
				return Unauthorized(result);
			}
			return Ok(result);
		}

		[HttpPost("refresh")]
		public async Task<ActionResult<ApiResponse<TokenDto>>> Refresh([FromBody] RefreshDto dto, CancellationToken ct)
		{
			var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ct);
			if (result.IsError)
			{
				return Unauthorized(result);
			}
			return Ok(result);
		}

	}
}
