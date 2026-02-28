using Wekeza.Core.Application.Common;
using Wekeza.Core.Application.Features.Accounts.Queries.GetAccount;
///<summary>
/// ðŸ“‚ Wekeza.Core.Application - The Final Checklist
/// To ensure the Application Layer is 100% complete and future-proofed for the 100 systems, we must finalize the "Read" side of our vertical slices.
/// 1. Features/Accounts/Queries/GetBalance
/// A bank is useless if the user can't see their money. We use a Query to fetch the current balance.
///</summary>

namespace Wekeza.Core.Application.Features.Accounts.Queries.GetBalance;

public record GetBalanceQuery(string AccountNumber) : IQuery<AccountDto>;
