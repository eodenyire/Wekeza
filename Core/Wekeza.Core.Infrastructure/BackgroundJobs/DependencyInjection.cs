///🏛️ Wekeza.Core.Infrastructure/DependencyInjection.cs
/// The Final Wiring: This is the "Glue" that makes all our hard work accessible to the Web API. It registers the Database, the Repositories, the Services, and the Background Jobs we just built.
///
///
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wekeza.Core.Infrastructure.Persistence;
using Wekeza.Core.Infrastructure.Persistence.Repositories;
using Wekeza.Core.Infrastructure.Services;
using Wekeza.Core.Infrastructure.BackgroundJobs;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Application.Common.Interfaces;
using System.Data;
using Npgsql;

namespace Wekeza.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 1. Persistence: EF Core for Writes
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // 2. Persistence: Dapper for High-Speed Reads
        services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(connectionString));

        // 3. Repositories & Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // 4. External Services
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddHttpClient<IMpesaService, MpesaIntegrationService>();

        // 5. Hosted Background Jobs
        services.AddHostedService<ChequeClearanceJob>();
        services.AddHostedService<InterestAccrualJob>();

        return services;
    }
}
