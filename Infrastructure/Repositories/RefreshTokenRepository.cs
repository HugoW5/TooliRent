using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.RepoInterfaces;
using Infrastructure.Data;
using Domain.Models;

namespace Infrastructure.Repositories
{
	public class RefreshTokenRepository : IRefreshTokenRepository
	{
		private readonly ApplicationDbContext _context;

		public RefreshTokenRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
		{
			await _context.RefreshTokens.AddAsync(token, ct);
			await _context.SaveChangesAsync(ct);
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
		{
			return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);
		}

		public async Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
		{
			_context.RefreshTokens.Update(token);
			await _context.SaveChangesAsync(ct);
		}

	}
}
