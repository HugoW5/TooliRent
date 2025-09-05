using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TooliRent.Middleware;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Infrastructure.Data;
using Domain.Interfaces.ServiceInterfaces;
using Infrastructure.Repositories;
using TooliRent.Services;
using Application.Services;
using Domain.Interfaces.Repositories;

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
				ClockSkew = TimeSpan.Zero,

				RoleClaimType = ClaimTypes.Role,
				NameClaimType = ClaimTypes.NameIdentifier
			};
		});

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();

		// Swagger with JWT Auth support
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "TooliRent", Version = "v1" });

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Description = "Enter JWT Token:"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		});

		builder.Services.AddCors(options =>
		{
			options.AddPolicy("AllowFrontend",
				policy => policy
					.WithOrigins("http://localhost:5173") // your frontend URL
					.AllowAnyHeader()
					.AllowAnyMethod());
		});

		var app = builder.Build();
		app.UseCors("AllowFrontend");

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

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}
