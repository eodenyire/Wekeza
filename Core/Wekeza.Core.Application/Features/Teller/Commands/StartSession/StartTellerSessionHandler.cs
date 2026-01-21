using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Domain.ValueObjects;

namespace Wekeza.Core.Application.Features.Teller.Commands.StartSession;

public class StartTellerSessionHandler : IRequestHandler<StartTellerSessionCommand, Result<Guid>>
{
    private readonly ITellerSessionRepository _tellerSessionRepository;
    private readonly ICashDrawerRepository _cashDrawerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public StartTellerSessionHandler(
        ITellerSessionRepository tellerSessionRepository,
        ICashDrawerRepository cashDrawerRepository,
        IBranchRepository branchRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _tellerSessionRepository = tellerSessionRepository;
        _cashDrawerRepository = cashDrawerRepository;
        _branchRepository = branchRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(StartTellerSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validate branch exists
            var branch = await _branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
            if (branch == null)
            {
                return Result<Guid>.Failure("Branch not found");
            }

            // 2. Check if teller already has an active session
            var existingSession = await _tellerSessionRepository.GetActiveSessionByUserAsync(
                _currentUserService.UserId, cancellationToken);
            
            if (existingSession != null)
            {
                return Result<Guid>.Failure("You already have an active teller session");
            }

            // 3. Create cash drawer
            var cashDrawer = CashDrawer.Create(
                tellerId: _currentUserService.UserId,
                branchId: request.BranchId,
                workstationId: request.WorkstationId,
                openingBalance: new Money(request.OpeningCashBalance, Currency.KES)
            );

            // 4. Set cash denominations
            foreach (var denomination in request.CashDenominations)
            {
                cashDrawer.SetDenomination(denomination.Key, denomination.Value);
            }

            // 5. Create teller session
            var tellerSession = TellerSession.Start(
                tellerId: _currentUserService.UserId,
                branchId: request.BranchId,
                workstationId: request.WorkstationId,
                cashDrawerId: cashDrawer.Id,
                openingBalance: new Money(request.OpeningCashBalance, Currency.KES),
                notes: request.Notes
            );

            // 6. Save entities
            await _cashDrawerRepository.AddAsync(cashDrawer, cancellationToken);
            await _tellerSessionRepository.AddAsync(tellerSession, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(tellerSession.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to start teller session: {ex.Message}");
        }
    }
}