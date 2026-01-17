using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.Treasury.Commands.ExecuteFXDeal;

public class ExecuteFXDealHandler : IRequestHandler<ExecuteFXDealCommand, ExecuteFXDealResponse>
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

    public async Task<ExecuteFXDealResponse> Handle(ExecuteFXDealCommand request, CancellationToken cancellationToken)
    {
        // Validate deal number uniqueness
        if (await _fxDealRepository.ExistsAsync(request.DealNumber, cancellationToken))
        {
            throw new ValidationException($"FX Deal with number {request.DealNumber} already exists");
        }

        // Validate counterparty exists
        var counterparty = await _partyRepository.GetByIdAsync(request.CounterpartyId, cancellationToken);
        if (counterparty == null)
        {
            throw new NotFoundException("Counterparty", request.CounterpartyId);
        }

        // Create value objects
        var baseAmount = new Money(request.BaseAmount, request.BaseCurrency);
        var exchangeRate = new ExchangeRate(
            request.BaseCurrency, 
            request.QuoteCurrency, 
            request.ExchangeRate, 
            request.Spread, 
            request.RateSource);

        // Execute the FX deal
        var fxDeal = FXDeal.Execute(
            request.DealNumber,
            request.CounterpartyId,
            request.DealType,
            request.BaseCurrency,
            request.QuoteCurrency,
            baseAmount,
            exchangeRate,
            request.ValueDate,
            request.TraderId,
            request.MaturityDate);

        // Add to repository
        await _fxDealRepository.AddAsync(fxDeal, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ExecuteFXDealResponse
        {
            DealId = fxDeal.Id,
            DealNumber = fxDeal.DealNumber,
            Status = fxDeal.Status.ToString(),
            BaseAmount = fxDeal.BaseAmount.Amount,
            QuoteAmount = fxDeal.QuoteAmount.Amount,
            ExchangeRate = fxDeal.Rate.Rate,
            ExecutionTime = fxDeal.CreatedAt
        };
    }
}