using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
namespace Infrastructure.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}
		public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			var adminRoleId = "8d12c03f-8b7d-4b11-9d39-12ab3b45d3c1";
			var userRoleId = "7a7b5c20-1234-4c99-a9a1-8e1b51a7a111";
			var adminUserId = "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc";

			// Roles
			builder.Entity<IdentityRole>().HasData(
				new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
				new IdentityRole { Id = userRoleId, Name = "Member", NormalizedName = "MEMBER" }
			);

			// Admin user
			var adminUser = new IdentityUser
			{
				Id = "5c77e6e4-4321-4e9e-8a19-f9b1c7c4fabc",
				UserName = "admin",
				NormalizedUserName = "ADMIN",
				Email = "admin@site.com",
				NormalizedEmail = "ADMIN@SITE.COM",
				EmailConfirmed = true,
				SecurityStamp = "STATIC-SECURITY-STAMP",
				ConcurrencyStamp = "STATIC-CONCURRENCY-STAMP",
				PasswordHash = "AQAAAAIAAYagAAAAEMQjUOb4dV0YwutubztJCQ0zAgbv35afKy5XJIInX8AQ9BBhLaV3QSucs/0LoWiHEw==" // Password123!
			};

			builder.Entity<IdentityUser>().HasData(adminUser);

			// User ↔ Role relationship
			builder.Entity<IdentityUserRole<string>>().HasData(
				new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId }
			);
		}

	}
}
