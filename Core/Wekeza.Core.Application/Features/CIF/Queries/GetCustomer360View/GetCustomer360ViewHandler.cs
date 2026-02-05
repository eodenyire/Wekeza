using MediatR;
using Wekeza.Core.Application.Common.Exceptions;
using Wekeza.Core.Domain.Interfaces;

namespace Wekeza.Core.Application.Features.CIF.Queries.GetCustomer360View;

public class GetCustomer360ViewHandler : IRequestHandler<GetCustomer360ViewQuery, Customer360ViewDto>
{
    private readonly IPartyRepository _partyRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly ICardRepository _cardRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetCustomer360ViewHandler(
        IPartyRepository partyRepository,
        IAccountRepository accountRepository,
        ILoanRepository loanRepository,
        ICardRepository cardRepository,
        ITransactionRepository transactionRepository)
    {
        _partyRepository = partyRepository;
        _accountRepository = accountRepository;
        _loanRepository = loanRepository;
        _cardRepository = cardRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Customer360ViewDto> Handle(GetCustomer360ViewQuery request, CancellationToken cancellationToken)
    {
        // Get party information
        var party = await _partyRepository.GetByPartyNumberAsync(request.PartyNumber, cancellationToken);
        
        if (party == null)
        {
            throw new NotFoundException("Party", request.PartyNumber, request.PartyNumber);
        }

        // Get accounts
        var accounts = await _accountRepository.GetByCustomerIdAsync(party.Id, cancellationToken);
        var accountSummaries = accounts.Select(a => new AccountSummary
        {
            AccountNumber = a.AccountNumber,
            AccountType = a.AccountType.ToString(),
            Currency = a.Currency,
            Balance = a.Balance,
            Status = a.Status.ToString()
        }).ToList();

        var totalBalance = accountSummaries.Sum(a => a.Balance);

        // Get loans
        var loans = await _loanRepository.GetByCustomerIdAsync(party.Id, cancellationToken);
        var loanSummaries = loans.Select(l => new LoanSummary
        {
            LoanId = l.Id,
            LoanType = l.LoanType.ToString(),
            Principal = l.PrincipalAmount,
            Outstanding = l.OutstandingBalance,
            Status = l.Status.ToString(),
            NextPaymentDate = l.NextPaymentDate
        }).ToList();

        var totalLoanOutstanding = loanSummaries.Sum(l => l.Outstanding);

        // Get cards
        var cards = await _cardRepository.GetByCustomerIdAsync(party.Id, cancellationToken);
        var cardSummaries = cards.Select(c => new CardSummary
        {
            CardNumber = c.MaskedCardNumber,
            CardType = c.CardType.ToString(),
            Status = c.Status.ToString(),
            ExpiryDate = c.ExpiryDate
        }).ToList();

        // Get recent transactions (last 10)
        var recentTransactions = await _transactionRepository.GetRecentByCustomerIdAsync(party.Id, 10, cancellationToken);
        var transactionSummaries = recentTransactions.Select(t => new TransactionSummary
        {
            Date = t.TransactionDate,
            Type = t.TransactionType.ToString(),
            Amount = t.Amount,
            Description = t.Description ?? string.Empty
        }).ToList();

        // Get relationships
        var relationshipInfos = party.Relationships.Select(r => new RelationshipInfo
        {
            RelatedPartyNumber = r.RelatedPartyId.ToString(),
            RelatedPartyName = "Related Party", // Would need to fetch actual name
            RelationshipType = r.RelationshipType
        }).ToList();

        // Build alerts
        var alerts = new List<string>();
        if (party.IsPEP)
            alerts.Add("⚠️ Politically Exposed Person (PEP)");
        if (party.IsSanctioned)
            alerts.Add("🚫 Sanctioned Party");
        if (party.KYCStatus == Domain.Enums.KYCStatus.Expired)
            alerts.Add("⏰ KYC Expired - Renewal Required");
        if (party.RiskRating == Domain.Enums.RiskRating.High)
            alerts.Add("⚠️ High Risk Rating");

        // Build full name
        var fullName = party.PartyType == Domain.Enums.PartyType.Individual
            ? $"{party.FirstName} {party.LastName}"
            : party.CompanyName ?? string.Empty;

        // Build address information
        var addressInfos = party.Addresses.Select(a => new AddressInfo
        {
            AddressType = a.AddressType,
            FullAddress = $"{a.AddressLine1}, {a.City}, {a.Country}",
            IsPrimary = a.IsPrimary
        }).ToList();

        return new Customer360ViewDto
        {
            PartyNumber = party.PartyNumber,
            PartyType = party.PartyType.ToString(),
            Status = party.Status.ToString(),
            FullName = fullName,
            CreatedDate = party.CreatedDate,
            
            // Individual details
            DateOfBirth = party.DateOfBirth?.ToString("yyyy-MM-dd"),
            Gender = party.Gender,
            Nationality = party.Nationality,
            
            // Corporate details
            CompanyName = party.CompanyName,
            RegistrationNumber = party.RegistrationNumber,
            Industry = party.Industry,
            
            // Contact
            PrimaryEmail = party.PrimaryEmail,
            PrimaryPhone = party.PrimaryPhone,
            Addresses = addressInfos,
            
            // KYC & Risk
            KYCStatus = party.KYCStatus.ToString(),
            KYCCompletedDate = party.KYCCompletedDate,
            KYCExpiryDate = party.KYCExpiryDate,
            RiskRating = party.RiskRating.ToString(),
            IsPEP = party.IsPEP,
            IsSanctioned = party.IsSanctioned,
            
            // Segmentation
            Segment = party.Segment.ToString(),
            SubSegment = party.SubSegment,
            
            // Financial summary
            Accounts = accountSummaries,
            TotalBalance = totalBalance,
            Loans = loanSummaries,
            TotalLoanOutstanding = totalLoanOutstanding,
            Cards = cardSummaries,
            RecentTransactions = transactionSummaries,
            Relationships = relationshipInfos,
            Alerts = alerts
        };
    }
}
