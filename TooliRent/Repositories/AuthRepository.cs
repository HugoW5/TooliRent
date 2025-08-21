using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TooliRent.Data;
using TooliRent.Models;
using TooliRent.Repositories.Interfaces;

namespace TooliRent.Repositories
{
	public class AuthRepository : IAuthRepository
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ApplicationDbContext _context;

		public AuthRepository(UserManager<IdentityUser> userManager, ApplicationDbContext context, CancellationToken ct = default)
		{
			_userManager = userManager;
			_context = context;
		}

		public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
		{
			await _context.RefreshTokens.AddAsync(token);
			await _context.SaveChangesAsync(ct);
		}

		public Task<bool> CheckPasswordAsync(IdentityUser user, string password)
		{
			return _userManager.CheckPasswordAsync(user, password);
		}

		public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
		{
			return await _userManager.CreateAsync(user, password);
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken ct = default)
		{
			return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token, ct);
		}

		public Task<IdentityUser?> GetUserByEmailAsync(string email)
		{
			return _userManager.FindByEmailAsync(email);
		}

		public async Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken ct = default)
		{
			_context.RefreshTokens.Update(token);
			await _context.SaveChangesAsync(ct);
		}
	}
}
