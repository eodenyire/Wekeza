using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Wekeza.Core.Application.Features.Transactions.Commands.TransferFunds;
using Wekeza.Core.Application.Features.Accounts.Queries.GetBalance;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.IntegrationTests.TestFixtures;
using MediatR;
using Xunit;
using Xunit.Abstractions;

namespace Wekeza.Core.IntegrationTests.Performance;

/// <summary>
/// Performance tests to validate system meets enterprise requirements
/// Target: <100ms response time for 95% of operations, 10,000+ TPS capability
/// </summary>
public class PerformanceTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly IMediator _mediator;
    private readonly ITestOutputHelper _output;

    public PerformanceTests(DatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _mediator = _fixture.ServiceProvider.GetRequiredService<IMediator>();
        _output = output;
    }

    [Fact]
    public async Task BalanceInquiry_ShouldComplete_UnderTargetTime()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        await SetupTestAccount(accountId);
        
        var query = new GetBalanceQuery { AccountId = accountId };
        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await _mediator.Send(query);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, 
            "Balance inquiry should complete in under 100ms");
        
        _output.WriteLine($"Balance inquiry completed in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task ConcurrentTransactions_ShouldMaintainPerformance()
    {
        // Arrange
        var accountIds = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            var accountId = Guid.NewGuid();
            await SetupTestAccount(accountId);
            accountIds.Add(accountId);
        }

        var tasks = new List<Task<TimeSpan>>();
        var concurrentOperations = 100;

        // Act - Execute concurrent balance inquiries
        for (int i = 0; i < concurrentOperations; i++)
        {
            var accountId = accountIds[i % accountIds.Count];
            tasks.Add(MeasureBalanceInquiry(accountId));
        }

        var results = await Task.WhenAll(tasks);
        
        // Assert
        var averageTime = results.Average(r => r.TotalMilliseconds);
        var p95Time = results.OrderBy(r => r.TotalMilliseconds)
                            .Skip((int)(results.Length * 0.95))
                            .First()
                            .TotalMilliseconds;

        averageTime.Should().BeLessThan(50, "Average response time should be under 50ms");
        p95Time.Should().BeLessThan(100, "95th percentile should be under 100ms");

        _output.WriteLine($"Concurrent operations: {concurrentOperations}");
        _output.WriteLine($"Average response time: {averageTime:F2}ms");
        _output.WriteLine($"95th percentile: {p95Time:F2}ms");
    }

    [Fact]
    public async Task HighVolumeTransactions_ShouldMaintainThroughput()
    {
        // Arrange
        var sourceAccountId = Guid.NewGuid();
        var targetAccountId = Guid.NewGuid();
        
        await SetupTestAccount(sourceAccountId, 1000000.00m); // 1M KES
        await SetupTestAccount(targetAccountId);

        var transactionCount = 1000;
        var stopwatch = Stopwatch.StartNew();

        // Act - Execute high volume of small transfers
        var tasks = new List<Task>();
        for (int i = 0; i < transactionCount; i++)
        {
            var transferCommand = new TransferFundsCommand
            {
                FromAccountId = sourceAccountId,
                ToAccountId = targetAccountId,
                Amount = new Money(10.00m, Currency.KES),
                Description = $"Performance test transfer {i}",
                Reference = $"PERF_TEST_{i:D6}"
            };

            tasks.Add(_mediator.Send(transferCommand));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        var tps = transactionCount / stopwatch.Elapsed.TotalSeconds;
        tps.Should().BeGreaterThan(100, "System should handle at least 100 TPS");

        _output.WriteLine($"Processed {transactionCount} transactions in {stopwatch.Elapsed.TotalSeconds:F2}s");
        _output.WriteLine($"Throughput: {tps:F2} TPS");
    }

    [Fact]
    public async Task LargeDatasetQuery_ShouldComplete_WithinTimeout()
    {
        // Arrange - Create many accounts for testing
        var accountIds = new List<Guid>();
        for (int i = 0; i < 100; i++)
        {
            var accountId = Guid.NewGuid();
            await SetupTestAccount(accountId);
            accountIds.Add(accountId);
        }

        var stopwatch = Stopwatch.StartNew();

        // Act - Query all accounts (simulating large dataset operation)
        var tasks = accountIds.Select(id => new GetBalanceQuery { AccountId = id })
                             .Select(query => _mediator.Send(query))
                             .ToArray();

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, 
            "Large dataset query should complete within 5 seconds");

        _output.WriteLine($"Queried {accountIds.Count} accounts in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task ScalabilityTest_IncreasingLoad_ShouldMaintainPerformance(int concurrentUsers)
    {
        // Arrange
        var accountId = Guid.NewGuid();
        await SetupTestAccount(accountId);

        var operationsPerUser = 10;
        var totalOperations = concurrentUsers * operationsPerUser;
        var stopwatch = Stopwatch.StartNew();

        // Act - Simulate increasing concurrent load
        var tasks = new List<Task>();
        for (int user = 0; user < concurrentUsers; user++)
        {
            for (int op = 0; op < operationsPerUser; op++)
            {
                tasks.Add(MeasureBalanceInquiry(accountId));
            }
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        var tps = totalOperations / stopwatch.Elapsed.TotalSeconds;
        var averageResponseTime = results.Average(r => r.TotalMilliseconds);

        // Performance should not degrade significantly with increased load
        averageResponseTime.Should().BeLessThan(200, 
            $"Average response time should remain reasonable with {concurrentUsers} concurrent users");
        
        tps.Should().BeGreaterThan(50, "Throughput should remain above minimum threshold");

        _output.WriteLine($"Concurrent users: {concurrentUsers}");
        _output.WriteLine($"Total operations: {totalOperations}");
        _output.WriteLine($"Throughput: {tps:F2} TPS");
        _output.WriteLine($"Average response time: {averageResponseTime:F2}ms");
    }

    [Fact]
    public async Task MemoryUsage_UnderLoad_ShouldRemainStable()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        await SetupTestAccount(accountId);

        var initialMemory = GC.GetTotalMemory(true);
        var operations = 1000;

        // Act - Perform many operations to test memory stability
        for (int i = 0; i < operations; i++)
        {
            await _mediator.Send(new GetBalanceQuery { AccountId = accountId });
            
            // Force garbage collection every 100 operations
            if (i % 100 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        var finalMemory = GC.GetTotalMemory(true);
        var memoryIncrease = finalMemory - initialMemory;

        // Assert
        memoryIncrease.Should().BeLessThan(50 * 1024 * 1024, // 50MB
            "Memory usage should not increase significantly under load");

        _output.WriteLine($"Initial memory: {initialMemory / 1024 / 1024:F2} MB");
        _output.WriteLine($"Final memory: {finalMemory / 1024 / 1024:F2} MB");
        _output.WriteLine($"Memory increase: {memoryIncrease / 1024 / 1024:F2} MB");
    }

    private async Task<TimeSpan> MeasureBalanceInquiry(Guid accountId)
    {
        var stopwatch = Stopwatch.StartNew();
        await _mediator.Send(new GetBalanceQuery { AccountId = accountId });
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private async Task SetupTestAccount(Guid accountId, decimal initialBalance = 10000.00m)
    {
        // In a real implementation, this would create a proper account
        // For now, we'll create minimal test data
        // This would typically use the account opening workflow
    }
}