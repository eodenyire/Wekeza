using Wekeza.Core.Application.DTOs;

namespace Wekeza.Core.Application.Common.Interfaces;

/// <summary>
/// Service interface for M-Pesa integration with Safaricom Daraja API
/// </summary>
public interface IMpesaService
{
    /// <summary>
    /// Initiates an STK push to the customer's phone
    /// </summary>
    Task<string> InitiateStkPush(decimal amount, string phoneNumber, string accountRef);
    
    /// <summary>
    /// Processes callback from Safaricom after payment completion
    /// </summary>
    Task ProcessCallback(MpesaCallbackDto callbackData);
}
