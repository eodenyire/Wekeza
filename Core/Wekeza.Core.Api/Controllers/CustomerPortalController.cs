using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.SelfOnboard;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.UpdateProfile;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.ChangePassword;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.RequestCard;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.TransferFunds;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.PayBill;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollMobileBanking;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollInternetBanking;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.EnrollUSSD;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.BlockCard;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.ApplyForLoan;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.RepayLoan;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.BuyAirtime;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.DownloadStatement;
using Wekeza.Core.Application.Features.CustomerPortal.Commands.RequestVirtualCard;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetProfile;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetAccounts;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetOnboardingStatus;
using Wekeza.Core.Application.Features.Dashboard.Queries.GetCustomerProfile;
using Wekeza.Core.Application.Features.Teller.Queries.GetCustomerAccounts;
using Wekeza.Core.Application.Features.Teller.Queries.GetAccountBalance;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetAccountTransactions;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetTransactions;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetCards;
using Wekeza.Core.Application.Features.CustomerPortal.Queries.GetLoans;

namespace Wekeza.Core.Api.Controllers;

/// <summary>
/// Customer Portal Controller - Self-service banking portal
/// Enables customers to onboard themselves and perform banking operations
/// </summary>
[ApiController]
[Route("api/customer-portal")]
public class CustomerPortalController : BaseApiController
{
    public CustomerPortalController(IMediator mediator) : base(mediator) { }

    #region Self-Onboarding

    /// <summary>
    /// Customer self-onboarding - Step 1: Basic Information
    /// </summary>
    [HttpPost("onboard/basic-info")]
    [AllowAnonymous]
    public async Task<IActionResult> OnboardBasicInfo([FromBody] SelfOnboardBasicInfoCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                OnboardingId = result.Value.OnboardingId,
                NextStep = "document-upload",
                Message = "Basic information saved. Please upload your identification documents."
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Customer self-onboarding - Step 2: Document Upload
    /// </summary>
    [HttpPost("onboard/documents")]
    [AllowAnonymous]
    public async Task<IActionResult> OnboardDocuments([FromBody] SelfOnboardDocumentsCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                OnboardingId = command.OnboardingId,
                NextStep = "verification",
                Message = "Documents uploaded successfully. Your application is under review."
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Customer self-onboarding - Step 3: Account Setup
    /// </summary>
    [HttpPost("onboard/account-setup")]
    [AllowAnonymous]
    public async Task<IActionResult> OnboardAccountSetup([FromBody] SelfOnboardAccountSetupCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                CustomerId = result.Value.CustomerId,
                CIFNumber = result.Value.CIFNumber,
                AccountNumber = result.Value.AccountNumber,
                TempPassword = result.Value.TempPassword,
                Message = "Account created successfully. Please change your password on first login."
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Check onboarding status
    /// </summary>
    [HttpGet("onboard/status/{onboardingId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOnboardingStatus(Guid onboardingId)
    {
        var query = new GetOnboardingStatusQuery { OnboardingId = onboardingId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    #endregion

    #region Profile Management

    /// <summary>
    /// Get customer profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetProfile()
    {
        var query = new GetCustomerProfileQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update customer profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("profile/change-password")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Account Services

    /// <summary>
    /// Get customer accounts
    /// </summary>
    [HttpGet("accounts")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAccounts()
    {
        var query = new GetCustomerAccountsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get account balance
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/balance")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAccountBalance(Guid accountId)
    {
        var query = new GetAccountBalanceQuery { AccountId = accountId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get account transactions
    /// </summary>
    [HttpGet("accounts/{accountId:guid}/transactions")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetTransactions(Guid accountId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
    {
        var query = new GetAccountTransactionsQuery
        {
            AccountId = accountId,
            PageSize = pageSize,
            PageNumber = pageNumber
        };
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Download account statement
    /// </summary>
    [HttpPost("accounts/{accountId:guid}/statement")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> DownloadStatement(Guid accountId, [FromBody] DownloadStatementCommand command)
    {
        command.AccountId = accountId;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Transactions

    /// <summary>
    /// Transfer funds between accounts
    /// </summary>
    [HttpPost("transactions/transfer")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> TransferFunds([FromBody] TransferFundsCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                TransactionId = result.Value.TransactionId,
                TransactionReference = result.Value.TransactionReference,
                Status = result.Value.Status,
                Message = "Transfer initiated successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Pay bill
    /// </summary>
    [HttpPost("transactions/pay-bill")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> PayBill([FromBody] PayBillCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Buy airtime
    /// </summary>
    [HttpPost("transactions/buy-airtime")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> BuyAirtime([FromBody] BuyAirtimeCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Card Services

    /// <summary>
    /// Get customer cards
    /// </summary>
    [HttpGet("cards")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCards()
    {
        var query = new GetCustomerCardsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Request new card
    /// </summary>
    [HttpPost("cards/request")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RequestCard([FromBody] RequestCardCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Request virtual card
    /// </summary>
    [HttpPost("cards/request-virtual")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RequestVirtualCard([FromBody] RequestVirtualCardCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.IsSuccess)
        {
            return Ok(new
            {
                CardId = result.Value.CardId,
                CardNumber = result.Value.MaskedCardNumber,
                ExpiryDate = result.Value.ExpiryDate,
                CVV = result.Value.CVV,
                Message = "Virtual card created successfully"
            });
        }
        return BadRequest(result);
    }

    /// <summary>
    /// Block/Unblock card
    /// </summary>
    [HttpPost("cards/block")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> BlockCard([FromBody] BlockCardCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Loan Services

    /// <summary>
    /// Get customer loans
    /// </summary>
    [HttpGet("loans")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetLoans()
    {
        var query = new GetLoansQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Apply for loan
    /// </summary>
    [HttpPost("loans/apply")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ApplyForLoan([FromBody] ApplyForLoanCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Make loan repayment
    /// </summary>
    [HttpPost("loans/repay")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> RepayLoan([FromBody] RepayLoanCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion

    #region Digital Channel Enrollment

    /// <summary>
    /// Enroll in mobile banking
    /// </summary>
    [HttpPost("channels/mobile/enroll")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> EnrollMobileBanking([FromBody] EnrollMobileBankingCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Enroll in internet banking
    /// </summary>
    [HttpPost("channels/internet/enroll")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> EnrollInternetBanking([FromBody] EnrollInternetBankingCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Enroll in USSD banking
    /// </summary>
    [HttpPost("channels/ussd/enroll")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> EnrollUSSD([FromBody] EnrollUSSDCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    #endregion
}