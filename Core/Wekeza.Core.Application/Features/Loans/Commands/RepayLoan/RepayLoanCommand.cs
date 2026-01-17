using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;
using MediatR;

namespace Wekeza.Core.Application.Features.Loans.Commands.RepayLoan;

public record RepayLoanCommand : ICommand<decimal>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid LoanId { get; init; }
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = "Cash"; // Cash, Transfer, MobileMoney
}

public class RepayLoanHandler : IRequestHandler<RepayLoanCommand, decimal>
{
    private readonly ILoanRepository _loanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RepayLoanHandler(ILoanRepository loanRepository, IUnitOfWork unitOfWork)
    {
        _loanRepository = loanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<decimal> Handle(RepayLoanCommand request, CancellationToken ct)
    {
        var loan = await _loanRepository.GetByIdAsync(request.LoanId, ct)
            ?? throw new NotFoundException("Loan", request.LoanId);
        
        // Use Domain Logic to apply the payment
        loan.ApplyPayment(request.Amount); 
        
        _loanRepository.Update(loan);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return loan.RemainingBalance;
    }
}
