using TooliRent.Dto.AuthDtos;

namespace TooliRent.Services.Interfaces
{
	public interface IAuthService
	{
		Task<(string Token, string RefreshToken)> RegisterAsync(RegisterDto dto);
		Task<(string Token, string RefreshToken)> LoginAsync(LoginDto dto);
		Task<(string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
	}

}
