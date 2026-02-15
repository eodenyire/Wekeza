using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Wekeza.MVP4._0.Data;
using Wekeza.MVP4._0.Services;
using Xunit;

namespace Wekeza.MVP4._0.Tests;

/// <summary>
/// Unit tests for Background Task Service functionality
/// Tests the background processing of workflow deadlines and notifications
/// </summary>
public class BackgroundTaskServiceTests : IDisposable
{
    private readonly MVP4DbContext _context;
    private readonly IServiceProvider _serviceProvider;
    private readonly Mock<ILogger<BackgroundTaskService>> _mockLogger;
    private readonly BackgroundTaskService _backgroundTaskService;

    public BackgroundTaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<MVP4DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MVP4DbContext(options);
        _mockLogger = new Mock<ILogger<BackgroundTaskService>>();

        // Setup service provider with required services
        var services = new ServiceCollection();
        services.AddSingleton(_context);
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IMakerCheckerService, MakerCheckerService>();
        services.AddScoped<IRBACService, RBACService>();
        services.AddLogging();

        // Add configuration mock
        var mockConfig = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        mockConfig.Setup(c => c["Jwt:Key"]).Returns("WekeezaMVP4SecretKeyThatIsAtLeast32CharactersLong123456");
        mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("WekeezaMVP4");
        mockConfig.Setup(c => c["Jwt:Audience"]).Returns("WekeezaMVP4Users");
        services.AddSingleton(mockConfig.Object);

        _serviceProvider = services.BuildServiceProvider();
        _backgroundTaskService = new BackgroundTaskService(_serviceProvider, _mockLogger.Object);
    }

    [Fact]
    public async Task ProcessWorkflowDeadlines_ShouldExecuteSuccessfully()
    {
        // Act
        await _backgroundTaskService.ProcessWorkflowDeadlinesAsync();

        // Assert - Should complete without throwing exceptions
        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting workflow deadline processing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed workflow deadline processing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessExpiredWorkflows_ShouldExecuteSuccessfully()
    {
        // Act
        await _backgroundTaskService.ProcessExpiredWorkflowsAsync();

        // Assert - Should complete without throwing exceptions
        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting expired workflow processing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed expired workflow processing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessPendingNotifications_ShouldExecuteSuccessfully()
    {
        // Act
        await _backgroundTaskService.ProcessPendingNotificationsAsync();

        // Assert - Should complete without throwing exceptions
        // Verify logging occurred
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Starting pending notification processing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Completed pending notification processing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task IsTaskRunning_WithNonRunningTask_ShouldReturnFalse()
    {
        // Act
        var isRunning = await _backgroundTaskService.IsTaskRunningAsync("NonExistentTask");

        // Assert
        Assert.False(isRunning);
    }

    [Fact]
    public async Task ExecuteScheduledTasks_ShouldCompleteSuccessfully()
    {
        // Act
        await _backgroundTaskService.ExecuteScheduledTasksAsync();

        // Assert - Should complete without throwing exceptions
        // This test verifies that the scheduled task execution doesn't crash
        Assert.True(true); // If we reach here, the method completed successfully
    }

    public void Dispose()
    {
        _context.Dispose();
        _serviceProvider.GetService<IServiceScope>()?.Dispose();
    }
}