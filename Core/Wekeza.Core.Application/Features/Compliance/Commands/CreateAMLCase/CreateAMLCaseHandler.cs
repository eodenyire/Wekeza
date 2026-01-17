using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Exceptions;

namespace Wekeza.Core.Application.Features.Compliance.Commands.CreateAMLCase;

public class CreateAMLCaseHandler : IRequestHandler<CreateAMLCaseCommand, CreateAMLCaseResponse>
{
    private readonly IAMLCaseRepository _amlCaseRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAMLCaseHandler(
        IAMLCaseRepository amlCaseRepository,
        IPartyRepository partyRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _amlCaseRepository = amlCaseRepository;
        _partyRepository = partyRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAMLCaseResponse> Handle(CreateAMLCaseCommand request, CancellationToken cancellationToken)
    {
        // Validate case number uniqueness
        if (await _amlCaseRepository.ExistsAsync(request.CaseNumber, cancellationToken))
        {
            throw new ValidationException($"AML Case with number {request.CaseNumber} already exists");
        }

        // Validate that either PartyId or TransactionId is provided
        if (!request.PartyId.HasValue && !request.TransactionId.HasValue)
        {
            throw new ValidationException("Either PartyId or TransactionId must be provided");
        }

        // Validate party exists if provided
        if (request.PartyId.HasValue)
        {
            var party = await _partyRepository.GetByIdAsync(request.PartyId.Value, cancellationToken);
            if (party == null)
            {
                throw new NotFoundException("Party", request.PartyId.Value);
            }
        }

        // Validate transaction exists if provided
        if (request.TransactionId.HasValue)
        {
            var transaction = await _transactionRepository.GetByIdAsync(request.TransactionId.Value, cancellationToken);
            if (transaction == null)
            {
                throw new NotFoundException("Transaction", request.TransactionId.Value);
            }
        }

        // Create risk score
        var riskScore = new RiskScore(request.RiskScore, request.RiskMethodology, request.RiskFactors);

        // Create AML case
        var amlCase = AMLCase.Create(
            request.CaseNumber,
            request.AlertType,
            riskScore,
            request.PartyId,
            request.TransactionId,
            request.Description);

        // Add to repository
        await _amlCaseRepository.AddAsync(amlCase, cancellationToken);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAMLCaseResponse
        {
            CaseId = amlCase.Id,
            CaseNumber = amlCase.CaseNumber,
            Status = amlCase.Status.ToString(),
            RiskScore = amlCase.RiskScore.Score,
            RiskLevel = amlCase.RiskScore.Level.ToString(),
            CreatedDate = amlCase.CreatedDate
        };
    }
}