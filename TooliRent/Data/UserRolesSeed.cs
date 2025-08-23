using Microsoft.AspNetCore.Identity;

namespace TooliRent.Data;

public static class UserRolesSeed
{
	public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

		// Roles
		var roles = new[] { "Admin", "User" };
		foreach (var roleName in roles)
		{
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}

		// Admin user
		var adminEmail = "admin@site.com";
		var adminExists = await userManager.FindByEmailAsync(adminEmail);
		if (adminExists == null)
		{
			var adminUser = new IdentityUser
			{
				UserName = "admin",
				Email = adminEmail,
				EmailConfirmed = true
			};

			var result = await userManager.CreateAsync(adminUser, "Password123!");
			if (!result.Succeeded)
			{
				throw new Exception("Failed to create admin user: " +
					string.Join(", ", result.Errors.Select(e => e.Description)));
			}

			// Assign admin role
			await userManager.AddToRoleAsync(adminUser, "Admin");
		}
	}
}
