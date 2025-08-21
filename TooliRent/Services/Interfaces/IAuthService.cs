using TooliRent.Dto.AuthDtos;
using TooliRent.Repositories;

namespace TooliRent.Services.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto);
		Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto);
		Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
	}
}
