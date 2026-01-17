using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.GeneralLedger.Commands.CreateGLAccount;

/// <summary>
/// Command to create a new GL account in Chart of Accounts
/// </summary>
[Authorize(UserRole.Administrator)]
public record CreateGLAccountCommand : ICommand<string>
{
    public string GLCode { get; init; } = string.Empty;
    public string GLName { get; init; } = string.Empty;
    public GLAccountType AccountType { get; init; }
    public GLAccountCategory Category { get; init; }
    public string Currency { get; init; } = "KES";
    public int Level { get; init; } = 1;
    public bool IsLeaf { get; init; } = true;
    public string? ParentGLCode { get; init; }
    public bool AllowManualPosting { get; init; } = true;
    public bool RequiresCostCenter { get; init; } = false;
    public bool RequiresProfitCenter { get; init; } = false;
}
