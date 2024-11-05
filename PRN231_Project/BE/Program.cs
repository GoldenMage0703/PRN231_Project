using BE.Service;
using Lib.Models;
using Lib.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

// Add JWT Authentication
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
		ValidIssuer = builder.Configuration["Jwt:Issuer"], // Đảm bảo đây là URL của Google
		ValidAudience = builder.Configuration["Google:ClientId"], // Đây là Client ID của bạn
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
	};
})
.AddGoogle(options =>
{
	options.ClientId = builder.Configuration["Google:ClientId"];
	options.ClientSecret = builder.Configuration["Google:ClientSecret"];
	options.Events = new OAuthEvents
	{
		OnCreatingTicket = context =>
		{
			var handler = new JwtSecurityTokenHandler();
			var jsonToken = handler.ReadToken(context.AccessToken) as JwtSecurityToken;

			// Lấy giá trị của trường "aud"
			var audience = jsonToken?.Claims.First(claim => claim.Type == "aud").Value;

			// Kiểm tra xem giá trị "aud" có khớp với Client ID không
			if (audience != builder.Configuration["Google:ClientId"])
			{
				throw new SecurityTokenInvalidAudienceException("JWT contains untrusted 'aud' claim.");
			}

			return Task.CompletedTask;
		}
	};
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
	options.TokenLifespan = TimeSpan.FromHours(1);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
	option.SwaggerDoc("v1", new OpenApiInfo { Title = "Book API", Version = "v1" });
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
			new string[] {}
		}
	});
});

builder.Services.AddDbContext<PRN231_ProjectContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Test") ?? throw new InvalidOperationException("Connection string not found.")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IVnPayService, VnPayService>();

builder.Services.AddCors(options =>
{
	options.AddPolicy(MyAllowSpecificOrigins,
	builder =>
	{
		builder.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials();
	});
});

builder.Services.AddScoped<Lib.DTO.Password.IEmailService, Lib.DTO.Password.EmailService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
