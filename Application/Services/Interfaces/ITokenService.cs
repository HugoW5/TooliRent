using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces.ServiceInterfaces
{
	public interface ITokenService
	{
		Task<string> GenerateTokenAsync(IdentityUser user);
		Task<string> GenerateRefreshTokenAsync();
	}
}
