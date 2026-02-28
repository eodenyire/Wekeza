namespace Wekeza.Core.Application.Common.Interfaces;

/// <summary>
/// Generic mapping interface for object transformations
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Maps source object to destination type
    /// </summary>
    TDestination Map<TDestination>(object source);

    /// <summary>
    /// Maps source object to existing destination object
    /// </summary>
    TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

    /// <summary>
    /// Maps collection of source objects to collection of destination type
    /// </summary>
    IEnumerable<TDestination> Map<TDestination>(IEnumerable<object> source);
}