using MediatR;
using Wekeza.Core.Application.Common;
using Wekeza.Core.Domain.Aggregates;
using Wekeza.Core.Domain.Interfaces;
using System.Text.Json;

namespace Wekeza.Core.Application.Features.Reporting.Commands.GenerateRegulatoryReport;

/// <summary>
/// Handler for generating regulatory reports
/// </summary>
public class GenerateRegulatoryReportHandler : IRequestHandler<GenerateRegulatoryReportCommand, Result<Guid>>
{
    private readonly IRegulatoryReportRepository _regulatoryReportRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateRegulatoryReportHandler(
        IRegulatoryReportRepository regulatoryReportRepository,
        IAccountRepository accountRepository,
        ILoanRepository loanRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _regulatoryReportRepository = regulatoryReportRepository;
        _accountRepository = accountRepository;
        _loanRepository = loanRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(GenerateRegulatoryReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if report already exists for the period
            var existingReport = await _regulatoryReportRepository.GetByCodeAndPeriodAsync(
                request.ReportCode, 
                request.ReportingPeriodStart, 
                request.ReportingPeriodEnd);

            if (existingReport != null)
                return Result<Guid>.Failure("Report already exists for this period");

            // Create regulatory report
            var report = new RegulatoryReport(
                request.ReportId,
                request.ReportCode,
                request.ReportName,
                request.Authority,
                request.ReportType,
                request.Frequency,
                request.ReportingPeriodStart,
                request.ReportingPeriodEnd,
                request.DueDate,
                request.GeneratedBy);

            // Generate report data based on type and authority
            await GenerateReportData(report, request, cancellationToken);

            // Generate the report
            var reportData = await BuildReportData(report, request, cancellationToken);
            report.GenerateReport(reportData);

            // Validate the report
            report.ValidateReport();

            // Auto-submit if requested and validation passed
            if (request.AutoSubmit && report.Status == Domain.Enums.ReportStatus.Validated)
            {
                report.ApproveReport(request.GeneratedBy, "Auto-approved for submission");
                report.SubmitReport(request.GeneratedBy, $"AUTO_{DateTime.UtcNow:yyyyMMddHHmmss}");
            }

            // Save the report
            await _regulatoryReportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(report.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to generate regulatory report: {ex.Message}");
        }
    }

    private async Task GenerateReportData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        switch (request.Authority)
        {
            case Domain.Enums.RegulatoryAuthority.CBK:
                await GenerateCBKReportData(report, request, cancellationToken);
                break;
            case Domain.Enums.RegulatoryAuthority.KRA:
                await GenerateKRAReportData(report, request, cancellationToken);
                break;
            case Domain.Enums.RegulatoryAuthority.CMA:
                await GenerateCMAReportData(report, request, cancellationToken);
                break;
            default:
                await GenerateGenericReportData(report, request, cancellationToken);
                break;
        }
    }

    private async Task GenerateCBKReportData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // CBK-specific report data generation
        switch (request.ReportType)
        {
            case Domain.Enums.ReportType.PrudentialReturn:
                await GeneratePrudentialReturnData(report, request, cancellationToken);
                break;
            case Domain.Enums.ReportType.StatutoryReturn:
                await GenerateStatutoryReturnData(report, request, cancellationToken);
                break;
            default:
                await GenerateGenericCBKData(report, request, cancellationToken);
                break;
        }
    }

    private async Task GeneratePrudentialReturnData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // Get account balances
        var accounts = await _accountRepository.GetAccountsByDateRangeAsync(
            request.ReportingPeriodStart, 
            request.ReportingPeriodEnd);

        var totalDeposits = accounts
            .Where(a => a.AccountType == Domain.Enums.AccountType.Savings || 
                       a.AccountType == Domain.Enums.AccountType.Current)
            .Sum(a => a.Balance.Amount);

        report.AddDataItem("DEPOSITS", "Total Customer Deposits", totalDeposits);

        // Get loan balances
        var loans = await _loanRepository.GetLoansByDateRangeAsync(
            request.ReportingPeriodStart, 
            request.ReportingPeriodEnd);

        var totalLoans = loans.Sum(l => l.OutstandingBalance.Amount);
        report.AddDataItem("LOANS", "Total Outstanding Loans", totalLoans);

        // Calculate capital adequacy ratio (simplified)
        var capitalAdequacyRatio = totalDeposits > 0 ? (totalLoans / totalDeposits) * 100 : 0;
        report.AddDataItem("CAR", "Capital Adequacy Ratio", capitalAdequacyRatio);
    }

    private async Task GenerateStatutoryReturnData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // Get transaction volumes
        var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(
            request.ReportingPeriodStart, 
            request.ReportingPeriodEnd);

        var totalTransactionValue = transactions.Sum(t => t.Amount.Amount);
        var totalTransactionCount = transactions.Count();

        report.AddDataItem("TRANSACTION_VALUE", "Total Transaction Value", totalTransactionValue);
        report.AddDataItem("TRANSACTION_COUNT", "Total Transaction Count", totalTransactionCount);

        // Average transaction value
        var avgTransactionValue = totalTransactionCount > 0 ? totalTransactionValue / totalTransactionCount : 0;
        report.AddDataItem("AVG_TRANSACTION", "Average Transaction Value", avgTransactionValue);
    }

    private async Task GenerateGenericCBKData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // Generic CBK data
        report.AddDataItem("REPORTING_PERIOD", "Reporting Period Days", 
            (request.ReportingPeriodEnd - request.ReportingPeriodStart).Days);
    }

    private async Task GenerateKRAReportData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // KRA tax-related data
        var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(
            request.ReportingPeriodStart, 
            request.ReportingPeriodEnd);

        // Calculate withholding tax (simplified)
        var interestTransactions = transactions.Where(t => t.Description.Contains("interest", StringComparison.OrdinalIgnoreCase));
        var totalInterest = interestTransactions.Sum(t => t.Amount.Amount);
        var withholdingTax = totalInterest * 0.15m; // 15% WHT

        report.AddDataItem("INTEREST_PAID", "Total Interest Paid", totalInterest);
        report.AddDataItem("WITHHOLDING_TAX", "Withholding Tax", withholdingTax);
    }

    private async Task GenerateCMAReportData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // CMA compliance-related data
        report.AddDataItem("COMPLIANCE_STATUS", "Compliance Status", 1); // 1 = Compliant
    }

    private async Task GenerateGenericReportData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        // Generic report data
        report.AddDataItem("REPORT_PERIOD", "Reporting Period", 
            (request.ReportingPeriodEnd - request.ReportingPeriodStart).Days);
    }

    private async Task<string> BuildReportData(
        RegulatoryReport report, 
        GenerateRegulatoryReportCommand request, 
        CancellationToken cancellationToken)
    {
        var reportData = new
        {
            ReportHeader = new
            {
                ReportCode = request.ReportCode,
                ReportName = request.ReportName,
                Authority = request.Authority.ToString(),
                ReportType = request.ReportType.ToString(),
                PeriodStart = request.ReportingPeriodStart,
                PeriodEnd = request.ReportingPeriodEnd,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = request.GeneratedBy
            },
            DataItems = report.DataItems.Select(d => new
            {
                Category = d.Category,
                Description = d.Description,
                Amount = d.Amount.Amount,
                Currency = d.Amount.Currency.Code
            }),
            Summary = new
            {
                TotalRecords = report.RecordCount,
                TotalAmount = report.TotalAmount?.Amount ?? 0,
                Currency = report.TotalAmount?.Currency.Code ?? "KES"
            }
        };

        return JsonSerializer.Serialize(reportData, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
    }
}