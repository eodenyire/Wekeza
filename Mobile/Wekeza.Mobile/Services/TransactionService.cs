using Wekeza.Mobile.Models;

namespace Wekeza.Mobile.Services;

public interface ITransactionService
{
    Task<TransferResponse?> TransferFundsAsync(TransferRequest request);
    Task<TransactionHistory?> GetTransactionHistoryAsync(string accountNumber, int pageNumber = 1, int pageSize = 20);
}

public class TransactionService : ITransactionService
{
    private readonly IApiService _apiService;

    public TransactionService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<TransferResponse?> TransferFundsAsync(TransferRequest request)
    {
        try
        {
            return await _apiService.PostAsync<TransferRequest, TransferResponse>(
                "/api/transactions/transfer",
                request
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Transfer error: {ex.Message}");
            return new TransferResponse { Success = false, Message = $"Error: {ex.Message}" };
        }
    }

    public async Task<TransactionHistory?> GetTransactionHistoryAsync(string accountNumber, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            return await _apiService.GetAsync<TransactionHistory>(
                $"/api/transactions/statement/{accountNumber}?pageNumber={pageNumber}&pageSize={pageSize}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get transaction history error: {ex.Message}");
            return null;
        }
    }
}
