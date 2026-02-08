using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Controls;

public class GetControlsByRiskIdQuery : IRequest<List<ControlDto>>
{
    public Guid RiskId { get; set; }
}
