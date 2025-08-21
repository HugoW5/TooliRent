using Microsoft.AspNetCore.Identity;
using TooliRent.Models;

namespace TooliRent.Repositories.Interfaces
{
	public interface IAuthRepository
	{
		Task<IdentityUser?> GetUserByEmailAsync(string email);
		Task<IdentityResult> CreateUserAsync(IdentityUser user, string password);
		Task<bool> CheckPasswordAsync(IdentityUser user, string password);

		Task AddRefreshTokenAsync(RefreshToken token);
		Task<RefreshToken?> GetRefreshTokenAsync(string token);
		Task UpdateRefreshTokenAsync(RefreshToken token);
	}
}
