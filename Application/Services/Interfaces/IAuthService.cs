using Dto.AuthDtos;
using Responses;

namespace Domain.Interfaces.ServiceInterfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<TokenDto>> RegisterAsync(RegisterDto dto);
		Task<ApiResponse<TokenDto>> LoginAsync(LoginDto dto);
		Task<ApiResponse<TokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
		Task<ApiResponse<string>> ToggleActivateAccount(string userId, CancellationToken ct = default);
	}
}