using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Risks;

public class GetRiskByIdQuery : IRequest<RiskDto?>
{
    public Guid Id { get; set; }
}
