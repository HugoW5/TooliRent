using TooliRent.Dto.AuthDtos;
using TooliRent.Repositories;

namespace TooliRent.Services.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto);
		Task<(string Token, string RefreshToken)> LoginAsync(LoginDto dto);
		Task<(string Token, string RefreshToken)> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
	}

}
