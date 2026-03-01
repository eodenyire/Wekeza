using Wekeza.Core.Domain.Common;

namespace Wekeza.Core.Domain.Aggregates;

/// <summary>
/// Feedback Aggregate - Customer feedback and ratings
/// </summary>
public class Feedback : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public string Category { get; private set; }
    public int Rating { get; private set; } // 1-5 stars
    public string? Comments { get; private set; }
    public string? Source { get; private set; } // App, Web, Branch, Call Center
    public DateTime ProvidedAt { get; private set; }
    public bool IsPublic { get; private set; }

    private Feedback() : base(Guid.NewGuid()) { }

    public static Feedback Create(Guid customerId, string category, int rating, string? comments = null, string? source = null)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Category = category,
            Rating = rating,
            Comments = comments,
            Source = source,
            ProvidedAt = DateTime.UtcNow,
            IsPublic = false
        };
        return feedback;
    }

    public void MakePublic()
    {
        IsPublic = true;
    }
}
