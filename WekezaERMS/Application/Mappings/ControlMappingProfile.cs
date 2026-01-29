using AutoMapper;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Mappings;

public class ControlMappingProfile : Profile
{
    public ControlMappingProfile()
    {
        // RiskControl to ControlDto
        CreateMap<RiskControl, ControlDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.RiskId, opt => opt.MapFrom(src => src.RiskId))
            .ForMember(dest => dest.ControlName, opt => opt.MapFrom(src => src.ControlName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ControlType, opt => opt.MapFrom(src => src.ControlType))
            .ForMember(dest => dest.Effectiveness, opt => opt.MapFrom(src => src.Effectiveness))
            .ForMember(dest => dest.LastTestedDate, opt => opt.MapFrom(src => src.LastTestedDate))
            .ForMember(dest => dest.NextTestDate, opt => opt.MapFrom(src => src.NextTestDate))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.TestingFrequency, opt => opt.MapFrom(src => src.TestingFrequency))
            .ForMember(dest => dest.TestingEvidence, opt => opt.MapFrom(src => src.TestingEvidence))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
