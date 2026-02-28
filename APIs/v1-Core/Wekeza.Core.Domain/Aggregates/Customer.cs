using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.ValueObjects;
///<summary>
/// ðŸ“‚ Wekeza.Core.Domain/Aggregates
/// 1. Customer.cs (The Identity & Risk Aggregate)
/// A bank is nothing without its customers. This aggregate handles the legal identity and the Risk Profileâ€”essential for the Model Risk Management we are passionate about.
///</summary>
namespace Wekeza.Core.Domain.Aggregates;

public class Customer : AggregateRoot
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string IdentificationNumber { get; private set; } // National ID or Passport
    public RiskLevel RiskRating { get; private set; }
    public bool IsActive { get; private set; }
    
    // Computed property for full name
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Private constructor for EF Core
    private Customer() : base(Guid.NewGuid()) { }

    public Customer(Guid id, string firstName, string lastName, string email, string phoneNumber, string idNumber) 
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        IdentificationNumber = idNumber;
        RiskRating = RiskLevel.Low; // Default for new customers
        IsActive = true;
    }

    public void UpdateRiskRating(RiskLevel newRating)
    {
        RiskRating = newRating;
        // Logic for triggering higher scrutiny if High
    }

    public void Deactivate() => IsActive = false;
}


