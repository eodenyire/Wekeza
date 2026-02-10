using Wekeza.Core.Application.Common.Interfaces;
using System.Text.Json;

namespace Wekeza.Core.Infrastructure.Services;

/// <summary>
/// Simple mapper implementation using JSON serialization for object transformation
/// Note: This is a basic implementation. For production, consider using AutoMapper.
/// </summary>
public class SimpleMapper : IMapper
{
    public TDestination Map<TDestination>(object source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        // Simple JSON-based mapping
        var json = JsonSerializer.Serialize(source);
        var result = JsonSerializer.Deserialize<TDestination>(json);
        
        if (result == null)
            throw new InvalidOperationException($"Failed to map {source.GetType().Name} to {typeof(TDestination).Name}");
            
        return result;
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        // Simple property copying via JSON
        var json = JsonSerializer.Serialize(source);
        var temp = JsonSerializer.Deserialize<TDestination>(json);
        
        if (temp == null)
            throw new InvalidOperationException($"Failed to map {typeof(TSource).Name} to {typeof(TDestination).Name}");
            
        return temp;
    }

    public IEnumerable<TDestination> Map<TDestination>(IEnumerable<object> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Select(item => Map<TDestination>(item)).ToList();
    }
}
