using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Wekeza.Core.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for EF Core tooling (migrations, scaffolding).
/// Used by `dotnet ef database update` when the startup project cannot start.
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Allow override via environment variable (CI/CD, Docker) or fallback to local dev default
        var connectionString = Environment.GetEnvironmentVariable("EF_CONNECTION_STRING")
            ?? "Host=localhost;Port=5432;Database=WekezaCoreDB;Username=wekeza_app;Password=WekeZa2026!SecurePass";

            optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
