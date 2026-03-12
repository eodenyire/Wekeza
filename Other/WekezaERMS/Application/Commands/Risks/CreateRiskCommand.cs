using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Risks;

public class CreateRiskCommand : IRequest<RiskDto>
{
    public CreateRiskDto RiskData { get; set; } = null!;
    public Guid CreatedBy { get; set; }
}
