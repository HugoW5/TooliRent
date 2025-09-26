using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure.Data
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\TooliRent");

			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(basePath)
				.AddJsonFile("appsettings.json")
				.Build();

			var connectionString = configuration.GetConnectionString("DefaultConnection");

			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			optionsBuilder.UseSqlServer(connectionString);

			return new ApplicationDbContext(optionsBuilder.Options);
		}
	}
}
