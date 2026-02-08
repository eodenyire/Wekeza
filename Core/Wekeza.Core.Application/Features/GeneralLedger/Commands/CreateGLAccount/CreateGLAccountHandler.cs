using MediatR;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.GeneralLedger.Commands.CreateGLAccount;

public class CreateGLAccountHandler : IRequestHandler<CreateGLAccountCommand, string>
{
    private readonly IGLAccountRepository _glAccountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateGLAccountHandler(
        IGLAccountRepository glAccountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _glAccountRepository = glAccountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(CreateGLAccountCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate GL code
        var exists = await _glAccountRepository.ExistsAsync(request.GLCode);
        if (exists)
        {
            throw new InvalidOperationException($"GL account with code {request.GLCode} already exists.");
        }

        // Validate parent if specified
        if (!string.IsNullOrEmpty(request.ParentGLCode))
        {
            var parent = await _glAccountRepository.GetByGLCodeAsync(request.ParentGLCode);
            if (parent == null)
            {
                throw new InvalidOperationException($"Parent GL account {request.ParentGLCode} not found.");
            }
        }

        // Create GL account
        var glAccount = GLAccount.Create(
            request.GLCode,
            request.GLName,
            request.AccountType,
            request.Category,
            request.Currency,
            request.Level,
            request.IsLeaf,
            _currentUserService.UserId?.ToString() ?? "System",
            request.ParentGLCode);

        // Update control flags
        glAccount.UpdateControlFlags(
            request.AllowManualPosting,
            request.RequiresCostCenter,
            request.RequiresProfitCenter,
            _currentUserService.UserId?.ToString() ?? "System");

        // Save
        _glAccountRepository.Add(glAccount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return glAccount.GLCode;
    }
}
