namespace Wekeza.Core.Infrastructure.Caching;

public class RedisCacheOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public int Database { get; set; } = 0;
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromHours(1);
    public string KeyPrefix { get; set; } = "wekeza:";
}