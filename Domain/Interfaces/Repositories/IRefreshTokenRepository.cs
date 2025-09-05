using Domain.Models;

namespace Domain.Repositories.Interfaces
{
	public interface IRefreshTokenRepository
	{
		Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
		Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default);
		Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
	}
}
