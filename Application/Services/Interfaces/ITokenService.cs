using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces.ServiceInterfaces
{
	public interface ITokenService
	{
		Task<string> GenerateTokenAsync(ApplicationUser user);
		Task<string> GenerateRefreshTokenAsync();
	}
}
