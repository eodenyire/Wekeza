using MediatR;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Services;
using Wekeza.Core.Domain.Enums;
using Wekeza.Core.Application.Common.Interfaces;
using Wekeza.Core.Application.Features.Workflows.Commands.InitiateWorkflow;

namespace Wekeza.Core.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentOrderRepository _paymentOrderRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly PaymentProcessingService _paymentProcessingService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public ProcessPaymentHandler(
        IUnitOfWork unitOfWork,
        IPaymentOrderRepository paymentOrderRepository,
        IAccountRepository accountRepository,
        PaymentProcessingService paymentProcessingService,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _paymentOrderRepository = paymentOrderRepository;
        _accountRepository = accountRepository;
        _paymentProcessingService = paymentProcessingService;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<ProcessPaymentResult> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Create payment order based on type
            var paymentOrder = await CreatePaymentOrderAsync(request);
            _paymentOrderRepository.Add(paymentOrder);

            // 2. Check if payment requires approval
            if (paymentOrder.RequiresApproval)
            {
                // Initiate workflow for approval
                var workflowCommand = new InitiateWorkflowCommand
                {
                    WorkflowType = Domain.Enums.Domain.Aggregates.WorkflowType.Transaction,
                    EntityId = paymentOrder.Id,
                    EntityType = "PaymentOrder",
                    Amount = request.Amount,
                    Currency = request.Currency,
                    Description = $"Payment approval required: {request.Description}",
                    Priority = MapPriorityToWorkflow(request.Priority),
                    RequestedBy = _currentUserService.UserId
                };

                var workflowResult = await _mediator.Send(workflowCommand, cancellationToken);
                paymentOrder.SetWorkflowInstance(workflowResult.Value);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ProcessPaymentResult
                {
                    IsSuccess = true,
                    PaymentOrderId = paymentOrder.Id,
                    PaymentReference = paymentOrder.PaymentReference,
                    Status = PaymentStatus.Pending,
                    RequiresApproval = true,
                    WorkflowInstanceId = workflowResult.Value
                };
            }

            // 3. Process payment immediately if no approval required
            if (request.ProcessImmediately)
            {
                var processingResult = await ProcessPaymentOrderAsync(paymentOrder);
                
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ProcessPaymentResult
                {
                    IsSuccess = processingResult.IsSuccess,
                    ErrorMessage = processingResult.ErrorMessage,
                    PaymentOrderId = paymentOrder.Id,
                    PaymentReference = paymentOrder.PaymentReference,
                    JournalNumber = processingResult.JournalNumber,
                    ExternalReference = processingResult.ExternalReference,
                    Status = paymentOrder.Status,
                    FeeAmount = paymentOrder.FeeAmount?.Amount,
                    RequiresApproval = false
                };
            }

            // 4. Schedule for later processing
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ProcessPaymentResult
            {
                IsSuccess = true,
                PaymentOrderId = paymentOrder.Id,
                PaymentReference = paymentOrder.PaymentReference,
                Status = PaymentStatus.Pending,
                RequiresApproval = false
            };
        }
        catch (Exception ex)
        {
            return new ProcessPaymentResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                PaymentOrderId = Guid.Empty,
                Status = PaymentStatus.Failed
            };
        }
    }

    private async Task<PaymentOrder> CreatePaymentOrderAsync(ProcessPaymentCommand request)
    {
        var amount = new Money(request.Amount, Currency.FromCode(request.Currency));

        return request.Type switch
        {
            PaymentType.InternalTransfer => await CreateInternalTransferAsync(request, amount),
            PaymentType.ExternalTransfer => await CreateExternalPaymentAsync(request, amount),
            _ => throw new ArgumentException($"Unsupported payment type: {request.Type}")
        };
    }

    private async Task<PaymentOrder> CreateInternalTransferAsync(ProcessPaymentCommand request, Money amount)
    {
        // Resolve account IDs if account numbers provided
        var fromAccountId = request.FromAccountId;
        var toAccountId = request.ToAccountId;

        if (!fromAccountId.HasValue && !string.IsNullOrEmpty(request.FromAccountNumber))
        {
            var fromAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.FromAccountNumber));
            fromAccountId = fromAccount?.Id ?? throw new ArgumentException("From account not found");
        }

        if (!toAccountId.HasValue && !string.IsNullOrEmpty(request.ToAccountNumber))
        {
            var toAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.ToAccountNumber));
            toAccountId = toAccount?.Id ?? throw new ArgumentException("To account not found");
        }

        if (!fromAccountId.HasValue || !toAccountId.HasValue)
        {
            throw new ArgumentException("Both from and to accounts must be specified for internal transfer");
        }

        return PaymentOrder.CreateInternalTransfer(
            fromAccountId.Value,
            toAccountId.Value,
            amount,
            request.Description,
            _currentUserService.UserId?.ToString() ?? "",
            request.CustomerReference,
            request.Priority);
    }

    private async Task<PaymentOrder> CreateExternalPaymentAsync(ProcessPaymentCommand request, Money amount)
    {
        // Resolve from account ID if account number provided
        var fromAccountId = request.FromAccountId;

        if (!fromAccountId.HasValue && !string.IsNullOrEmpty(request.FromAccountNumber))
        {
            var fromAccount = await _accountRepository.GetByAccountNumberAsync(new AccountNumber(request.FromAccountNumber));
            fromAccountId = fromAccount?.Id ?? throw new ArgumentException("From account not found");
        }

        if (!fromAccountId.HasValue)
        {
            throw new ArgumentException("From account must be specified for external payment");
        }

        if (string.IsNullOrEmpty(request.BeneficiaryName) || 
            string.IsNullOrEmpty(request.BeneficiaryAccountNumber) ||
            string.IsNullOrEmpty(request.BeneficiaryBank))
        {
            throw new ArgumentException("Beneficiary details are required for external payment");
        }

        return PaymentOrder.CreateExternalPayment(
            fromAccountId.Value,
            request.BeneficiaryAccountNumber,
            request.BeneficiaryName,
            request.BeneficiaryBank,
            request.BeneficiaryBankCode ?? string.Empty,
            amount,
            request.Description,
            request.Channel,
            _currentUserService.UserId?.ToString() ?? "",
            request.CustomerReference,
            request.Priority);
    }

    private async Task<PaymentProcessingResult> ProcessPaymentOrderAsync(PaymentOrder paymentOrder)
    {
        return paymentOrder.Type switch
        {
            PaymentType.InternalTransfer => await _paymentProcessingService.ProcessInternalTransferAsync(
                paymentOrder, _currentUserService.UserId?.ToString() ?? ""),
            PaymentType.ExternalTransfer => await _paymentProcessingService.ProcessExternalPaymentAsync(
                paymentOrder, _currentUserService.UserId?.ToString() ?? ""),
            _ => PaymentProcessingResult.Failed($"Unsupported payment type: {paymentOrder.Type}")
        };
    }

    private static WorkflowPriority MapPriorityToWorkflow(PaymentPriority paymentPriority)
    {
        return paymentPriority switch
        {
            PaymentPriority.Low => WorkflowPriority.Low,
            PaymentPriority.Normal => WorkflowPriority.Normal,
            PaymentPriority.High => WorkflowPriority.High,
            PaymentPriority.Urgent => WorkflowPriority.Urgent,
            PaymentPriority.Emergency => WorkflowPriority.Critical,
            _ => WorkflowPriority.Normal
        };
    }
}