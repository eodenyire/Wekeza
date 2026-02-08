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
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
            {
                return Result<Guid>.Failure("Branch not found");
            }

            // 2. Check if teller already has an active session
            var tellerId = _currentUserService.UserId ?? Guid.Empty;
            var existingSession = await _tellerSessionRepository.GetActiveSessionByUserAsync(
                tellerId, cancellationToken);
            
            if (existingSession != null)
            {
                return Result<Guid>.Failure("You already have an active teller session");
            }

            // 3. Create cash drawer
            var cashDrawer = CashDrawer.Create(
                drawerId: $"DRAWER-{request.BranchId}-{DateTime.UtcNow:yyyyMMdd}",
                tellerId: tellerId,
                tellerCode: _currentUserService.Username ?? "TELLER",
                branchId: request.BranchId,
                branchCode: branch?.BranchCode ?? "HQ",
                maxCashLimit: new Money(1_000_000, Currency.KES),
                minCashLimit: new Money(10_000, Currency.KES),
                requiresDualControl: false,
                createdBy: _currentUserService.Username ?? "System"
            );

            // 4. Cash denominations are handled internally by CashDrawer

            // 5. Create teller session
            var tellerLimits = new TellerLimits(
                DailyTransactionLimit: new Money(100_000, Currency.KES),
                SingleTransactionLimit: new Money(50_000, Currency.KES),
                CashWithdrawalLimit: new Money(50_000, Currency.KES)
            );
            
            var tellerSession = TellerSession.Start(
                tellerId: tellerId,
                tellerCode: _currentUserService.Username ?? "TELLER",
                tellerName: _currentUserService.Username ?? "Teller",
                branchId: request.BranchId,
                branchCode: branch?.BranchCode ?? "HQ",
                openingCashBalance: new Money(request.OpeningCashBalance, Currency.KES),
                limits: tellerLimits,
                createdBy: _currentUserService.Username ?? "System"
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