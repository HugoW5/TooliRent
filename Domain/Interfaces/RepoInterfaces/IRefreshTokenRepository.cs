using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces.RepoInterfaces
{
	public interface IRefreshTokenRepository
	{
		Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
		Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
		Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
	}
}
