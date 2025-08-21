using Microsoft.AspNetCore.Identity;

namespace TooliRent.Services.Interfaces
{
	public interface ITokenService
	{
		Task<string> GenerateTokenAsync(IdentityUser user);
		Task<string> GenerateRefreshTokenAsync();
	}
}
