using System.Reflection;
using AutoMapper;

namespace Wekeza.Core.Application.Common.Mappings;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Mappings/
/// We will use AutoMapper as the engine, but we will implement it in a way that is Type-Safe and Discovery-Driven. Instead of a giant, messy mapping file, we create a system where each DTO "knows" how to map itself.
/// 2. MappingProfile.cs (The Assembly Scanner)
/// This is the "Engine Room." When Wekeza Bank starts up, this profile scans the entire Application assembly for any class that implements IMapFrom<T> and registers the mapping automatically.
/// Automated AutoMapper profile that finds all classes implementing IMapFrom 
/// within the Application layer and registers them.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            .ToList();

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            // Look for the "Mapping" method defined in the interface or overridden in the DTO
            var methodInfo = type.GetMethod("Mapping") 
                ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

            methodInfo?.Invoke(instance, new object[] { this });
        }
    }
}
