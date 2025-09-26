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
		builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		// Add JWT Authentication
		var jwtSettings = builder.Configuration.GetSection("JwtSettings");
		var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

		// Register Repositories and Services
		#region Register Repositories
		builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
		builder.Services.AddScoped<IToolRepository, ToolRepository>();
		builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
		builder.Services.AddScoped<IBookingRepository, BookingRepository>();
		builder.Services.AddScoped<IBookingItemRepository, BookingItemRepository>();
		#endregion

		#region Register Unit of Work
		builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
		#endregion

		#region Register Services
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<ITokenService, TokenService>();
		builder.Services.AddScoped<IToolService, ToolService>();
		builder.Services.AddScoped<ICategoryService, CategoryService>();
		builder.Services.AddScoped<IBookingService, BookingService>();
		#endregion

		#region Register Aspire metrics
		builder.Services.AddSingleton<IAuthMetrics, AuthMetrics>();
		#endregion



		//Add AutoMapper
		builder.Services.AddAutoMapper(cfg =>
		{
			cfg.AddProfile<MappingProfile>();
		});

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

		// Register FluentValidation auto-validation for Web API
		builder.Services.AddFluentValidationAutoValidation();
		builder.Services.AddFluentValidationClientsideAdapters(); // optional, mainly for MVC/Razor

		// Register all validators from your assembly
		builder.Services.AddValidatorsFromAssemblyContaining<UpdateToolDtoValidator>();

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
					.WithOrigins("http://localhost:5173")
					.AllowAnyHeader()
					.AllowAnyMethod());
		});

		var app = builder.Build();
		// Apply migrations & seed admin user
		using (var scope = app.Services.CreateScope())
		{
			var services = scope.ServiceProvider;

			// Apply migrations
			var dbContext = services.GetRequiredService<ApplicationDbContext>();
			dbContext.Database.Migrate();

			// Seed admin user
			await UserRolesSeed.SeedAdminUserAsync(services);
		}

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
