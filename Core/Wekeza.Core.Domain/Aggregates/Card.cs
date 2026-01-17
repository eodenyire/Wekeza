using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Card Aggregate - Represents a debit/credit card linked to an account
/// </summary>
public class Card : AggregateRoot
{
    public Guid AccountId { get; private set; }
    public string CardNumber { get; private set; }
    public string CardType { get; private set; } // Debit, Credit, Prepaid
    public string NameOnCard { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }
    public decimal DailyWithdrawalLimit { get; private set; }
    public decimal DailyWithdrawnToday { get; private set; }
    public DateTime LastWithdrawalDate { get; private set; }

    private Card() : base(Guid.NewGuid()) { }

    public Card(
        Guid id,
        Guid accountId,
        string cardType,
        string nameOnCard,
        decimal dailyWithdrawalLimit) : base(id)
    {
        AccountId = accountId;
        CardNumber = GenerateCardNumber();
        CardType = cardType;
        NameOnCard = nameOnCard;
        ExpiryDate = DateTime.UtcNow.AddYears(3);
        IsCancelled = false;
        DailyWithdrawalLimit = dailyWithdrawalLimit;
        DailyWithdrawnToday = 0;
        LastWithdrawalDate = DateTime.MinValue;
    }

    public bool CanWithdraw(decimal amount)
    {
        if (IsCancelled)
            return false;

        // Reset daily counter if it's a new day
        if (LastWithdrawalDate.Date < DateTime.UtcNow.Date)
        {
            DailyWithdrawnToday = 0;
        }

        return (DailyWithdrawnToday + amount) <= DailyWithdrawalLimit;
    }

    public void RecordWithdrawal(decimal amount)
    {
        if (!CanWithdraw(amount))
            throw new DomainException("Withdrawal would exceed daily limit.");

        // Reset if new day
        if (LastWithdrawalDate.Date < DateTime.UtcNow.Date)
        {
            DailyWithdrawnToday = 0;
        }

        DailyWithdrawnToday += amount;
        LastWithdrawalDate = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (IsCancelled)
            throw new DomainException("Card is already cancelled.");

        IsCancelled = true;
        CancellationReason = reason;
    }

    private static string GenerateCardNumber()
    {
        // Simple card number generation (in production, use proper BIN ranges)
        var random = new Random();
        var cardNumber = "5399"; // Wekeza Bank BIN
        for (int i = 0; i < 12; i++)
        {
            cardNumber += random.Next(0, 10).ToString();
        }
        return cardNumber;
    }
}
