using MediatR;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Application.Common.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.StartTellerSession;

/// <summary>
/// Start Teller Session Handler - Processes teller session initiation
/// Creates teller session and opens cash drawer with initial cash balance
/// </summary>
public class StartTellerSessionHandler : IRequestHandler<StartTellerSessionCommand, StartTellerSessionResult>
{
    private readonly ITellerSessionRepository _tellerSessionRepository;
    private readonly ICashDrawerRepository _cashDrawerRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public StartTellerSessionHandler(
        ITellerSessionRepository tellerSessionRepository,
        ICashDrawerRepository cashDrawerRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _tellerSessionRepository = tellerSessionRepository;
        _cashDrawerRepository = cashDrawerRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<StartTellerSessionResult> Handle(StartTellerSessionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Check if teller already has an active session
            var existingSession = await _tellerSessionRepository.GetActiveSessionByTellerIdAsync(request.TellerId, cancellationToken);
            if (existingSession != null)
            {
                return StartTellerSessionResult.Failed($"Teller {request.TellerCode} already has an active session: {existingSession.SessionId}");
            }

            // 2. Get or create cash drawer for teller
            var cashDrawer = await _cashDrawerRepository.GetByTellerIdAsync(request.TellerId, cancellationToken);
            if (cashDrawer == null)
            {
                // Create new cash drawer
                var drawerId = GenerateDrawerId(request.BranchCode, request.TellerCode);
                var maxCashLimit = new Money(5000000, new Currency(request.Currency)); // 5M limit
                var minCashLimit = new Money(10000, new Currency(request.Currency)); // 10K minimum
                
                cashDrawer = CashDrawer.Create(
                    drawerId,
                    request.TellerId,
                    request.TellerCode,
                    request.BranchId,
                    request.BranchCode,
                    maxCashLimit,
                    minCashLimit,
                    requiresDualControl: false,
                    _currentUserService.UserId?.ToString() ?? "System");

                await _cashDrawerRepository.AddAsync(cashDrawer, cancellationToken);
            }

            // 3. Validate cash drawer is not already open
            if (cashDrawer.Status == CashDrawerStatus.Open)
            {
                return StartTellerSessionResult.Failed($"Cash drawer {cashDrawer.DrawerId} is already open");
            }

            // 4. Create opening cash balance
            var openingCash = new Money(request.OpeningCashAmount, new Currency(request.Currency));

            // 5. Create teller limits
            var limits = new TellerLimits(
                new Money(request.DailyTransactionLimit, new Currency(request.Currency)),
                new Money(request.SingleTransactionLimit, new Currency(request.Currency)),
                new Money(request.CashWithdrawalLimit, new Currency(request.Currency)));

            // 6. Start teller session
            var currentUser = _currentUserService.UserId?.ToString() ?? "System";
            var session = TellerSession.Start(
                request.TellerId,
                request.TellerCode,
                request.TellerName,
                request.BranchId,
                request.BranchCode,
                openingCash,
                limits,
                currentUser);

            // 7. Open cash drawer with initial cash
            cashDrawer.Open(session.SessionId, openingCash, currentUser);

            // 8. Save session and drawer
            await _tellerSessionRepository.AddAsync(session, cancellationToken);
            _cashDrawerRepository.Update(cashDrawer);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return StartTellerSessionResult.Success(
                session.Id,
                session.SessionId,
                cashDrawer.DrawerId,
                openingCash.Amount,
                session.SessionStartTime,
                $"Teller session started for {request.TellerName} with opening cash {openingCash.Amount} {openingCash.Currency.Code}");
        }
        catch (Exception ex)
        {
            return StartTellerSessionResult.Failed($"Error starting teller session: {ex.Message}");
        }
    }

    private string GenerateDrawerId(string branchCode, string tellerCode)
    {
        return $"DRAWER-{branchCode}-{tellerCode}";
    }
}