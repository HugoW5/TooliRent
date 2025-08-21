
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using TooliRent.Data;
using Microsoft.IdentityModel.Tokens;

namespace TooliRent;

public class Program
{
	public static void Main(string[] args)
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
		var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

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


		// .NET ASPIRE
		app.MapDefaultEndpoints();

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
