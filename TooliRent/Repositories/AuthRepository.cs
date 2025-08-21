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

		public AuthRepository(UserManager<IdentityUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		public Task AddRefreshTokenAsync(RefreshToken token)
		{
			throw new NotImplementedException();
		}

		public Task<bool> CheckPasswordAsync(IdentityUser user, string password)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityUser> CreateUserAsync(IdentityUser user, string password)
		{
			throw new NotImplementedException();
		}

		public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
		{
			return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
		}

		public Task<IdentityUser?> GetUserByEmailAsync(string email)
		{
			return _userManager.FindByEmailAsync(email);
		}

		public Task UpdateRefreshTokenAsync(RefreshToken token)
		{
			throw new NotImplementedException();
		}
	}
}
