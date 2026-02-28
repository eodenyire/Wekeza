using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wekeza.Core.Infrastructure.Persistence;
using Xunit;

namespace Wekeza.Core.IntegrationTests.TestFixtures;

/// <summary>
/// Database fixture for integration tests - provides clean database for each test
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    public ApplicationDbContext Context { get; private set; } = null!;
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        
        // Use in-memory database for testing
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
                   .EnableSensitiveDataLogging()
                   .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())));

        ServiceProvider = services.BuildServiceProvider();
        Context = ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        await Context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (Context != null)
        {
            await Context.Database.EnsureDeletedAsync();
            await Context.DisposeAsync();
        }
        
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}