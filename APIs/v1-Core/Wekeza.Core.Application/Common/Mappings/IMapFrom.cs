using AutoMapper;

namespace Wekeza.Core.Application.Common.Mappings;

/// <summary>
/// 📂 Wekeza.Core.Application/Common/Mappings/
/// We will use AutoMapper as the engine, but we will implement it in a way that is Type-Safe and Discovery-Driven. Instead of a giant, messy mapping file, we create a system where each DTO "knows" how to map itself.
/// 1. IMapFrom.cs (The Discovery Interface)
/// This is a "marker" interface. Any DTO that implements this will be automatically picked up by our Mapping Profile. This is the definition of future-proofing.
/// A generic interface to allow DTOs to define their own mapping rules.
/// This promotes a "Vertical Slice" architecture where DTOs are self-contained.
/// </summary>
public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
