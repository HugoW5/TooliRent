using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}
		public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<Tool> Tools { get; set; } = null!;
		public DbSet<Booking> Bookings { get; set; } = null!;
		public DbSet<BookingItem> BookingItems { get; set; } = null!;


		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Seed only non-user entities
			var cat1 = new Category { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Elverktyg", Description = "Borrmaskiner, sågar osv." };
			var cat2 = new Category { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Handverktyg", Description = "Skruvmejslar, hammare osv." };
			builder.Entity<Category>().HasData(cat1, cat2);

			var tool1 = new Tool { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Borrmaskin Bosch", CategoryId = cat1.Id, Status = ToolStatus.Available };
			var tool2 = new Tool { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Hammare", CategoryId = cat2.Id, Status = ToolStatus.Available };
			builder.Entity<Tool>().HasData(tool1, tool2);

			// Seed roles only (no ApplicationUser here!)
			var adminRoleId = "8d12c03f-8b7d-4b11-9d39-12ab3b45d3c1";
			var userRoleId = "7a7b5c20-1234-4c99-a9a1-8e1b51a7a111";

			builder.Entity<IdentityRole>().HasData(
				new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
				new IdentityRole { Id = userRoleId, Name = "Member", NormalizedName = "MEMBER" }
			);
		}



	}
}
