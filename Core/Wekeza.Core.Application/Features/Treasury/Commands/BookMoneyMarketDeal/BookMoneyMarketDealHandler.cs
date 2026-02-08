using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.Treasury.Commands.BookMoneyMarketDeal;

public class BookMoneyMarketDealHandler : IRequestHandler<BookMoneyMarketDealCommand, BookMoneyMarketDealResponse>
{
    private readonly IMoneyMarketDealRepository _dealRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookMoneyMarketDealHandler(
        IMoneyMarketDealRepository dealRepository,
        IPartyRepository partyRepository,
        IUnitOfWork unitOfWork)
    {
        _dealRepository = dealRepository;
        _partyRepository = partyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookMoneyMarketDealResponse> Handle(BookMoneyMarketDealCommand request, CancellationToken cancellationToken)
    {
        // Validate deal number uniqueness
        if (await _dealRepository.ExistsAsync(request.DealNumber, cancellationToken))
        {
            throw new ValidationException(new List<FluentValidation.Results.ValidationFailure> { new FluentValidation.Results.ValidationFailure("", $"Deal with number {request.DealNumber} already exists") });
        }

        // Validate counterparty exists
        var counterparty = await _partyRepository.GetByIdAsync(request.CounterpartyId, cancellationToken);
        if (counterparty == null)
        {
            throw new NotFoundException("Counterparty", request.CounterpartyId);
        }

        // Create value objects
        var principal = new Money(request.Principal, request.Currency);
        var interestRate = new InterestRate((decimal)request.InterestRate);

        // Book the money market deal
        var deal = MoneyMarketDeal.Book(
            request.DealNumber,
            request.CounterpartyId,
            request.DealType,
            principal,
            interestRate,
            request.ValueDate,
            request.MaturityDate,
            request.TraderId,
            request.CollateralReference,
            request.Terms);

        // Add to repository
        await _dealRepository.AddAsync(deal, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BookMoneyMarketDealResponse
        {
            DealId = deal.Id,
            DealNumber = deal.DealNumber,
            Status = deal.Status.ToString(),
            MaturityAmount = deal.MaturityAmount?.Amount ?? 0,
            BookingTime = deal.CreatedAt
        };
    }
}