using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.Teller.Commands.PrintStatement;

public record PrintStatementCommand : ICommand<Result<byte[]>>
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public string AccountNumber { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class PrintStatementHandler : IRequestHandler<PrintStatementCommand, Result<byte[]>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PrintStatementHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<byte[]>> Handle(PrintStatementCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var statementData = new byte[] { 0x25, 0x50, 0x44, 0x46 }; // PDF header stub
            await Task.CompletedTask;
            return Result<byte[]>.Success(statementData);
        }
        catch (Exception ex)
        {
            return Result<byte[]>.Failure($"Failed to print statement: {ex.Message}");
        }
    }
}
