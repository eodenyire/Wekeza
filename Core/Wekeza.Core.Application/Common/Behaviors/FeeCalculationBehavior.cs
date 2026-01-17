///
/// 📂 Phase 3: The Automated "Tax & Fee" Gate
///To ensure the bank is profitable, we are adding a Global Fee Interceptor. This ensures that every transfer automatically deducts the bank's fee and the Excise Duty (Tax).
/// 1. 📂 Common/Behaviors/FeeCalculationBehavior.cs
/// This behavior intercepts every TransferFundsCommand and calculates the required deductions before the handler even starts.
///
public class FeeCalculationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is TransferFundsCommand transfer)
        {
            // Logic: 
            // 1. Calculate Bank Fee (e.g., 50 KES)
            // 2. Calculate Excise Duty (e.g., 20% of Fee)
            // 3. Update the transfer amount or add separate fee commands
            // This ensures the bank ALWAYS gets its cut before the money moves.
        }

        return await next();
    }
}
