using AutoMapper;
using WekezaERMS.Application.DTOs;
using WekezaERMS.Domain.Entities;

namespace WekezaERMS.Application.Mappings;

public class RiskMappingProfile : Profile
{
    public RiskMappingProfile()
    {
        // Risk to RiskDto
        CreateMap<Risk, RiskDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.RiskCode, opt => opt.MapFrom(src => src.RiskCode))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.InherentLikelihood, opt => opt.MapFrom(src => src.InherentLikelihood))
            .ForMember(dest => dest.InherentImpact, opt => opt.MapFrom(src => src.InherentImpact))
            .ForMember(dest => dest.InherentRiskScore, opt => opt.MapFrom(src => src.InherentRiskScore))
            .ForMember(dest => dest.InherentRiskLevel, opt => opt.MapFrom(src => src.InherentRiskLevel))
            .ForMember(dest => dest.ResidualLikelihood, opt => opt.MapFrom(src => src.ResidualLikelihood))
            .ForMember(dest => dest.ResidualImpact, opt => opt.MapFrom(src => src.ResidualImpact))
            .ForMember(dest => dest.ResidualRiskScore, opt => opt.MapFrom(src => src.ResidualRiskScore))
            .ForMember(dest => dest.ResidualRiskLevel, opt => opt.MapFrom(src => src.ResidualRiskLevel))
            .ForMember(dest => dest.TreatmentStrategy, opt => opt.MapFrom(src => src.TreatmentStrategy))
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department))
            .ForMember(dest => dest.IdentifiedDate, opt => opt.MapFrom(src => src.IdentifiedDate))
            .ForMember(dest => dest.LastAssessmentDate, opt => opt.MapFrom(src => src.LastAssessmentDate))
            .ForMember(dest => dest.NextReviewDate, opt => opt.MapFrom(src => src.NextReviewDate))
            .ForMember(dest => dest.RiskAppetite, opt => opt.MapFrom(src => src.RiskAppetite))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
