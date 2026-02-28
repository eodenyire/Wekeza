using MediatR;
using Wekeza.Core.Application.Common.Authorization;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Application.Features.Accounts.Commands.VerifyBusinessAccount;

/// <summary>
/// Command to verify a business account.
/// Requires RiskOfficer or Administrator role.
/// </summary>
[Authorize(UserRole.RiskOfficer, UserRole.Administrator)]
public record VerifyBusinessAccountCommand(Guid AccountId, string VerifiedBy, decimal DailyLimit) : IRequest<bool>;
