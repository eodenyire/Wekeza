using EnhancedWekezaApi.Domain.Common;
using EnhancedWekezaApi.Application.Features.Accounts.Queries.GetAccount;

namespace EnhancedWekezaApi.Application.Features.Accounts.Queries.GetBalance;

/// <summary>
/// A bank is useless if the user can't see their money. 
/// We use a Query to fetch the current balance.
/// </summary>
public record GetBalanceQuery(string AccountNumber) : IQuery<AccountDto>;