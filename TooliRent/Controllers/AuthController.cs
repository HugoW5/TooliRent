using Domain.Interfaces.ServiceInterfaces;
using Dto.AuthDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Responses;


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

		/// <summary>
		/// Registers a new user and returns a token if successful.
		/// </summary>
		[HttpPost("register")]
		[ProducesResponseType(typeof(ApiResponse<TokenDto>), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ApiResponse<TokenDto>>> Register([FromBody] RegisterDto dto)
		{
			var result = await _authService.RegisterAsync(dto);
			return CreatedAtAction(nameof(Register), result);
		}

		/// <summary>
		/// Logs in a user and returns an authentication token.
		/// </summary>
		[HttpPost("login")]
		[ProducesResponseType(typeof(ApiResponse<TokenDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ApiResponse<TokenDto>>> Login([FromBody] LoginDto dto)
		{
			var result = await _authService.LoginAsync(dto);
			return Ok(result);
		}

		/// <summary>
		/// Refreshes an authentication token using a refresh token.
		/// </summary>
		[HttpPost("refresh")]
		[ProducesResponseType(typeof(ApiResponse<TokenDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<ApiResponse<TokenDto>>> Refresh([FromBody] RefreshTokenDto dto, CancellationToken ct) 
		{
			var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ct);
			return Ok(result);
		}

		[Authorize]
		[HttpGet("getUserInfo")]
		public IActionResult GetUserInfo()
		{
			return Ok(User.Claims.ToDictionary(c => c.Type, c => c.Value));
		}
	}
}
