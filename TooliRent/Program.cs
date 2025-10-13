using Application.Mappings;
using Application.Metrics;
using Application.Metrics.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using Application.Validators;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.ServiceInterfaces;
using Domain.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using TooliRent.Middleware;
using TooliRent.Services;
using Serilog;

namespace TooliRent;

public class Program
{
	public async static Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.Enrich.FromLogContext()
			.CreateLogger();

		builder.Host.UseSerilog();

		builder.AddServiceDefaults();
		builder.Services.AddControllers();

		// Database
		var connectionString = builder.Configuration["SQLSERVER_CONNECTION"]
					   ?? builder.Configuration.GetConnectionString("DefaultConnection")
					   ?? throw new Exception("Connection string is missing!");

		builder.Services.AddSqlServer<ApplicationDbContext>(connectionString);

		// Identity
		builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		// JWT
		// Environment
		var env = builder.Environment.EnvironmentName;

		// JWT settings
		string secret, issuer, audience;
		int expiryMinutes, refreshDays;

		if (env == "Development")
		{
			secret = builder.Configuration["JWTSECRET_DEV"] ?? "dev-secret";
			issuer = builder.Configuration["JWTISSUER_DEV"] ?? "ApiDev";
			audience = builder.Configuration["JWTAUDIENCE_DEV"] ?? "ApiUsersDev";
			expiryMinutes = int.Parse(builder.Configuration["JWTEXPIRYMINUTES_DEV"] ?? "15");
			refreshDays = int.Parse(builder.Configuration["JWTREFRESHTOKENDAYS_DEV"] ?? "7");
		}
		else
		{
			secret = builder.Configuration["JWTSECRET"] ?? throw new Exception("JWT Secret missing!");
			issuer = builder.Configuration["JWTISSUER"] ?? "Api";
			audience = builder.Configuration["JWTAUDIENCE"] ?? "ApiUsers";
			expiryMinutes = int.Parse(builder.Configuration["JWTEXPIRYMINUTES"] ?? "15");
			refreshDays = int.Parse(builder.Configuration["JWTREFRESHTOKENDAYS"] ?? "7");
		}

		var key = Encoding.UTF8.GetBytes(secret);


		builder.Services.AddAuthorization();

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
				ValidIssuer = issuer,
				ValidAudience = audience,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero,
				RoleClaimType = ClaimTypes.Role,
				NameClaimType = ClaimTypes.NameIdentifier
			};
		});


		// Repositories
		builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
		builder.Services.AddScoped<IToolRepository, ToolRepository>();
		builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
		builder.Services.AddScoped<IBookingRepository, BookingRepository>();
		builder.Services.AddScoped<IBookingItemRepository, BookingItemRepository>();
		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

		// Services
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IToolService, ToolService>();
		builder.Services.AddScoped<ICategoryService, CategoryService>();
		builder.Services.AddScoped<IBookingService, BookingService>();

		// Metrics
		builder.Services.AddSingleton<IAuthMetrics, AuthMetrics>();

		// AutoMapper
		builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

		// FluentValidation
		builder.Services.AddFluentValidationAutoValidation();
		builder.Services.AddValidatorsFromAssemblyContaining<UpdateToolDtoValidator>();

		// Swagger
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo { Title = "TooliRent", Version = "v1" });
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header
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

		// Health Check
		builder.Services.AddHealthChecks();

		var app = builder.Build();

		// Seed/migrate only in dev
		if (app.Environment.IsDevelopment())
		{
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;
			var db = services.GetRequiredService<ApplicationDbContext>();
			db.Database.Migrate();
			await UserRolesSeed.SeedAdminUserAsync(services);
		}
		else
		{
			Console.WriteLine("Skipping database migration in non-dev environment.");
		}

		app.UseMiddleware<ExceptionHandlingMiddleware>();

		if (app.Environment.IsDevelopment())
		{
			app.UseHttpsRedirection();
		}

		app.UseSwaggerUI();
		app.UseSwagger();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();
		app.MapHealthChecks("/health");
		app.MapDefaultEndpoints();

		app.Run();
	}
}
