using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wekeza.Core.Infrastructure.Persistence;
using Wekeza.Core.Infrastructure.Persistence.Repositories;
using Wekeza.Core.Infrastructure.Services;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IApprovalMatrixRepository, ApprovalMatrixRepository>();
        services.AddScoped<IGLAccountRepository, GLAccountRepository>();
        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();
        services.AddScoped<IPaymentOrderRepository, PaymentOrderRepository>();

        // Domain Services
        services.AddScoped<PaymentProcessingService>();
        services.AddScoped<CreditScoringService>();
        services.AddScoped<LoanServicingService>();

        // Application Services
        services.AddScoped<IAMLScreeningService, AMLScreeningService>();
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
