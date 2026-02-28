using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Wekeza.Core.Api.Authentication;

namespace Wekeza.Core.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddWekezaAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];

        // Configure JWT Settings as options
        services.Configure<JwtSettings>(jwtSettings);

        // Register JWT Token Generator
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(options =>
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager"));
            options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("RequireTellerRole", policy => policy.RequireRole("Teller"));
            options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
        });

        return services;
    }
}