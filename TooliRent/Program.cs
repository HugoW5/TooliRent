using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TooliRent.Data;
using TooliRent.Repositories.Interfaces;
using TooliRent.Services;
using TooliRent.Services.Interfaces;
using TooliRent.Middleware;

namespace TooliRent;

public class Program
{
	public async static Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.AddServiceDefaults();

		// Add services to the container.
		builder.Services.AddSqlServer<ApplicationDbContext>(
			builder.Configuration.GetConnectionString("DefaultConnection"));

		// Add Identity 
		builder.Services.AddIdentity<IdentityUser, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		// Add JWT Authentication
		var jwtSettings = builder.Configuration.GetSection("JwtSettings");
		var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

		// Register Repositories and Services
		#region Register Repositories
		builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
		#endregion

		#region Register Services
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		#endregion


		builder.Services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer(options =>
		{
			options.TokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = jwtSettings["Issuer"],
				ValidAudience = jwtSettings["Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero
			};
		});


		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();


		var app = builder.Build();
		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;
			await UserRolesSeed.SeedAdminUserAsync(services);
		}

		// .NET ASPIRE
		app.MapDefaultEndpoints();

		// Global Exception Middleware
		app.UseMiddleware<ExceptionHandlingMiddleware>();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();


		app.MapControllers();

		app.Run();
	}
}
