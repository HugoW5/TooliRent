using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TooliRent.Services.Interfaces;

namespace TooliRent.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _config;
		private readonly UserManager<IdentityUser> _userManager;

		public TokenService(IConfiguration configuration)
		{
			_config = configuration;
		}

		public Task<string> GenerateRefreshTokenAsync()
		{
			var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
			return Task.FromResult(refreshToken);
		}

		public async Task<string> GenerateTokenAsync(IdentityUser user)
		{

			var jwtSettings = _config.GetSection("JwtSettings");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var roles = await _userManager.GetRolesAsync(user);
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
