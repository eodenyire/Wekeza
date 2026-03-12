using MediatR;
using WekezaERMS.Application.DTOs;

namespace WekezaERMS.Application.Queries.Risks;

public class GetAllRisksQuery : IRequest<List<RiskDto>>
{
}
