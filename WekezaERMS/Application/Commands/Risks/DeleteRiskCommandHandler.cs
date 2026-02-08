using MediatR;

namespace WekezaERMS.Application.Commands.Risks;

public class DeleteRiskCommandHandler : IRequestHandler<DeleteRiskCommand, bool>
{
    private readonly IRiskRepository _riskRepository;

    public DeleteRiskCommandHandler(IRiskRepository riskRepository)
    {
        _riskRepository = riskRepository;
    }

    public async Task<bool> Handle(DeleteRiskCommand request, CancellationToken cancellationToken)
    {
        var risk = await _riskRepository.GetByIdAsync(request.Id);
        
        if (risk == null)
        {
            return false;
        }

        // Soft delete by closing the risk
        risk.Close("Risk archived/deleted", request.DeletedBy);

        await _riskRepository.UpdateAsync(risk);
        await _riskRepository.SaveChangesAsync();

        return true;
    }
}
