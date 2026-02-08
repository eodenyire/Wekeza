using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Commands.Risks;

public class UpdateRiskCommand : IRequest<RiskDto?>
{
    public Guid Id { get; set; }
    public UpdateRiskDto RiskData { get; set; } = null!;
    public Guid UpdatedBy { get; set; }
}
