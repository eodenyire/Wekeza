using Microsoft.OpenApi.Models;

namespace Wekeza.Core.Api.Extensions;

/// <summary>
/// Swagger/OpenAPI configuration for Wekeza Bank API
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddWekezaSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Wekeza Bank Core API",
                Version = "v1.0.2026",
                Description = @"
# Wekeza Bank - Core Banking API

![Wekeza Bank Logo](https://raw.githubusercontent.com/wekeza-bank/assets/main/logo.png)

## Overview
Principal-Grade Financial Core for the Nairobi Fintech Market.

This API provides comprehensive banking services including:
- **Account Management**: Open, close, freeze, and manage accounts
- **Transactions**: Deposits, withdrawals, transfers, and M-Pesa integration
- **Loans**: Apply, approve, disburse, and repay loans
- **Cards**: Issue, cancel, and manage debit/credit cards
- **Business Banking**: Corporate accounts with multi-signatory support

## Authentication
All endpoints (except login) require JWT Bearer token authentication.

1. Call `/api/authentication/login` with credentials
2. Copy the returned token
3. Click 'Authorize' button above
4. Enter: `Bearer {your-token}`

## Security & Compliance
- PCI-DSS compliant card processing
- AML/KYC verification workflows
- Audit trail for all transactions
- Role-based access control (RBAC)

## Support
For API support, contact: dev@wekeza.com
",
                Contact = new OpenApiContact 
                { 
                    Name = "Wekeza Engineering Team", 
                    Email = "dev@wekeza.com",
                    Url = new Uri("https://wekeza.com")
                },
                License = new OpenApiLicense
                {
                    Name = "Proprietary",
                    Url = new Uri("https://wekeza.com/license")
                }
            });

            // JWT Bearer Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"JWT Authorization header using the Bearer scheme.
                
Enter your JWT token in the text input below.
                
Example: '12345abcdef'"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            // Include XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Group endpoints by feature
            options.TagActionsBy(api =>
            {
                if (api.GroupName != null)
                    return new[] { api.GroupName };

                var controllerName = api.ActionDescriptor.RouteValues["controller"];
                return new[] { controllerName ?? "Unknown" };
            });

            options.DocInclusionPredicate((name, api) => true);

            // Use full names for schema IDs to avoid conflicts with duplicate type names
            options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
        });

        return services;
    }

    public static IApplicationBuilder UseWekezaSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wekeza Bank API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "Wekeza Bank API Documentation";
            
            // Custom CSS for branding
            options.InjectStylesheet("/swagger-ui/custom.css");
            
            // Display request duration
            options.DisplayRequestDuration();
            
            // Enable deep linking
            options.EnableDeepLinking();
            
            // Enable filter
            options.EnableFilter();
            
            // Show extensions
            options.ShowExtensions();
        });

        return app;
    }
}
