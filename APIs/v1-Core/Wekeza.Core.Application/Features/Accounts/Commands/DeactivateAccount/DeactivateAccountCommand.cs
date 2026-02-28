using MediatR;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Accounts.Commands.DeactivateAccount;

/// <summary>
/// Command to deactivate an account.
/// Requires RiskOfficer or Administrator role.
/// </summary>
[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record DeactivateAccountCommand(string AccountNumber, string Reason) : IRequest<bool>;
