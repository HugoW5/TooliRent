using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Dto;
using TooliRent.Dto.AuthDtos;
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
		public async Task<IActionResult> Register([FromBody] RegisterDto dto)
		{
			try
			{
				var (token, refreshToken) = await _authService.RegisterAsync(dto);
				return Ok(new { token, refreshToken });
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
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

	}
}
