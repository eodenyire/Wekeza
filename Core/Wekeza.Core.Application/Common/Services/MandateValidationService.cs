using Wekeza.Core.Domain.Aggregates;

///2. 📂 Common/Services/MandateValidationService.cs
///This service is used by the TransferFundsHandler. Before moving money from a business account, it checks if the "Mandate" (the required number of signatures) has been met.
///

namespace Wekeza.Core.Application.Common.Services;

public class MandateValidationService
{
    public bool IsMandateSatisfied(Account account, decimal amount, int providedSignaturesCount)
    {
        // 1. If Individual account, 1 signature is enough
        if (account.AccountType == "Individual") return true;

        // 2. If Corporate, check rules:
        // Rule: "Any Two to Sign" for amounts > 500,000 KES
        if (amount > 500000 && providedSignaturesCount < 2)
            return false;

        return true;
    }
}
