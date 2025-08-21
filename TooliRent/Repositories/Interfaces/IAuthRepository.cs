using Microsoft.AspNetCore.Identity;
using TooliRent.Models;

namespace TooliRent.Repositories.Interfaces
{
	public interface IAuthRepository
	{
		Task<IdentityUser?> GetUserByEmailAsync(string email, CancellationToken ct = default);
		Task<IdentityResult> CreateUserAsync(IdentityUser user, string password, CancellationToken ct = default);
		Task<bool> CheckPasswordAsync(IdentityUser user, string password, CancellationToken ct = default);
		Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
		Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
		Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
	}
}
