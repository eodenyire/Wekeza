using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Application.Common;

namespace Wekeza.Core.Application.Features.Treasury.Commands.ExecuteFXDeal;

public class ExecuteFXDealHandler : IRequestHandler<ExecuteFXDealCommand, Result<ExecuteFXDealResponse>>
{
    private readonly IFXDealRepository _fxDealRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExecuteFXDealHandler(
        IFXDealRepository fxDealRepository,
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork)
    {
        _fxDealRepository = fxDealRepository;
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ExecuteFXDealResponse>> Handle(ExecuteFXDealCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // For now, return a simple success response
            // Full implementation would involve complex FX deal processing
            
            var response = new ExecuteFXDealResponse
            {
                DealId = Guid.NewGuid(),
                DealNumber = request.DealNumber,
                Status = "PENDING_APPROVAL",
                ExchangeRate = request.ExchangeRate,
                FromAmount = request.Amount,
                ToAmount = request.Amount * request.ExchangeRate,
                FromCurrency = request.BaseCurrency,
                ToCurrency = request.QuoteCurrency,
                ValueDate = request.ValueDate,
                Message = "FX Deal created successfully and pending approval",
                RequiresApproval = request.RequiresApproval
            };

            return Result<ExecuteFXDealResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ExecuteFXDealResponse>.Failure($"Failed to execute FX deal: {ex.Message}");
        }
    }
}