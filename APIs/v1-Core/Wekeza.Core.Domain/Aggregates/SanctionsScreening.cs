using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class SanctionsScreening : AggregateRoot
{
    public EntityType EntityType { get; private set; }
    public Guid EntityId { get; private set; }
    public DateTime ScreeningDate { get; private set; }
    public List<WatchlistMatch> Matches { get; private set; }
    public decimal HighestMatchScore { get; private set; }
    public ScreeningStatus Status { get; private set; }
    public ScreeningDecision? Decision { get; private set; }
    public string? ReviewedBy { get; private set; }
    public DateTime? ReviewedDate { get; private set; }
    public string? ReviewNotes { get; private set; }
    public List<string> ScreenedWatchlists { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private SanctionsScreening() : base(Guid.NewGuid()) {
        Matches = new List<WatchlistMatch>();
        ScreenedWatchlists = new List<string>();
    }

    public static SanctionsScreening Create(
        EntityType entityType,
        Guid entityId,
        List<string> watchlistsToScreen)
    {
        if (entityId == Guid.Empty)
            throw new ArgumentException("Entity ID cannot be empty", nameof(entityId));

        if (watchlistsToScreen == null || !watchlistsToScreen.Any())
            throw new ArgumentException("At least one watchlist must be specified", nameof(watchlistsToScreen));

        var screening = new SanctionsScreening
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            ScreeningDate = DateTime.UtcNow,
            ScreenedWatchlists = watchlistsToScreen.ToList(),
            Status = ScreeningStatus.InProgress,
            UpdatedAt = DateTime.UtcNow
        };

        screening.AddDomainEvent(new SanctionsScreeningInitiatedDomainEvent(
            screening.Id, entityType, entityId, watchlistsToScreen));

        return screening;
    }

    public void AddMatch(
        string watchlistName,
        string matchedName,
        decimal matchScore,
        string matchType,
        Dictionary<string, string>? additionalData = null)
    {
        if (Status != ScreeningStatus.InProgress)
            throw new InvalidOperationException($"Cannot add matches to screening in {Status} status");

        if (matchScore < 0 || matchScore > 100)
            throw new ArgumentException("Match score must be between 0 and 100", nameof(matchScore));

        var match = new WatchlistMatch
        {
            Id = Guid.NewGuid(),
            SanctionsScreeningId = Id,
            WatchlistName = watchlistName,
            MatchedName = matchedName,
            MatchScore = matchScore,
            MatchType = matchType,
            AdditionalData = additionalData ?? new Dictionary<string, string>(),
            MatchedAt = DateTime.UtcNow
        };

        Matches.Add(match);

        // Update highest match score
        if (matchScore > HighestMatchScore)
        {
            HighestMatchScore = matchScore;
        }

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SanctionsMatchFoundDomainEvent(
            Id, EntityType, EntityId, watchlistName, matchedName, matchScore));
    }

    public void CompleteScreening()
    {
        if (Status != ScreeningStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete screening in {Status} status");

        Status = DetermineScreeningStatus();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SanctionsScreeningCompletedDomainEvent(
            Id, EntityType, EntityId, Status, HighestMatchScore, Matches.Count));
    }

    public void Review(string reviewedBy, ScreeningDecision decision, string reviewNotes = "")
    {
        if (Status == ScreeningStatus.Clear)
            throw new InvalidOperationException("Cannot review clear screening result");

        if (string.IsNullOrWhiteSpace(reviewedBy))
            throw new ArgumentException("Reviewer cannot be empty", nameof(reviewedBy));

        ReviewedBy = reviewedBy;
        Decision = decision;
        ReviewNotes = reviewNotes;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Update status based on decision
        Status = decision switch
        {
            ScreeningDecision.FalsePositive => ScreeningStatus.Clear,
            ScreeningDecision.TruePositive => ScreeningStatus.Blocked,
            ScreeningDecision.RequiresEscalation => ScreeningStatus.Escalated,
            ScreeningDecision.PendingInvestigation => ScreeningStatus.UnderReview,
            _ => Status
        };

        AddDomainEvent(new SanctionsScreeningReviewedDomainEvent(
            Id, EntityType, EntityId, decision, reviewedBy));

        // Create AML case if true positive or escalated
        if (decision == ScreeningDecision.TruePositive || decision == ScreeningDecision.RequiresEscalation)
        {
            var riskScore = RiskScore.ForAMLAlert(AMLAlertType.SanctionsMatch);
            var partyId = EntityType == EntityType.Party ? EntityId : (Guid?)null;
            var transactionId = EntityType == EntityType.Transaction ? EntityId : (Guid?)null;

            AddDomainEvent(new AMLCaseRequiredDomainEvent(
                transactionId, partyId, AMLAlertType.SanctionsMatch, riskScore));
        }
    }

    public void AddToWhitelist(string addedBy, string reason)
    {
        if (Status != ScreeningStatus.UnderReview)
            throw new InvalidOperationException("Can only whitelist items under review");

        Status = ScreeningStatus.Whitelisted;
        Decision = ScreeningDecision.FalsePositive;
        ReviewedBy = addedBy;
        ReviewNotes = $"Added to whitelist: {reason}";
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EntityWhitelistedDomainEvent(
            EntityType, EntityId, addedBy, reason));
    }

    public void Block(string blockedBy, string reason)
    {
        Status = ScreeningStatus.Blocked;
        Decision = ScreeningDecision.TruePositive;
        ReviewedBy = blockedBy;
        ReviewNotes = reason;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new EntityBlockedDomainEvent(
            EntityType, EntityId, blockedBy, reason));
    }

    public void Escalate(string escalatedBy, string reason)
    {
        Status = ScreeningStatus.Escalated;
        Decision = ScreeningDecision.RequiresEscalation;
        ReviewedBy = escalatedBy;
        ReviewNotes = reason;
        ReviewedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SanctionsScreeningEscalatedDomainEvent(
            Id, EntityType, EntityId, reason));
    }

    private ScreeningStatus DetermineScreeningStatus()
    {
        if (!Matches.Any())
            return ScreeningStatus.Clear;

        if (HighestMatchScore >= 95)
            return ScreeningStatus.Blocked;

        if (HighestMatchScore >= 80)
            return ScreeningStatus.UnderReview;

        if (HighestMatchScore >= 60)
            return ScreeningStatus.Alert;

        return ScreeningStatus.Clear;
    }

    public bool IsClear => Status == ScreeningStatus.Clear;
    public bool IsBlocked => Status == ScreeningStatus.Blocked;
    public bool RequiresReview => Status == ScreeningStatus.UnderReview || Status == ScreeningStatus.Alert;
    public bool IsEscalated => Status == ScreeningStatus.Escalated;
    public bool IsWhitelisted => Status == ScreeningStatus.Whitelisted;
    public bool HasMatches => Matches.Any();
    public bool HasHighConfidenceMatch => HighestMatchScore >= 80;
    public int MatchCount => Matches.Count;
    public int DaysSinceScreening => (DateTime.UtcNow - ScreeningDate).Days;
}

public class WatchlistMatch
{
    public Guid Id { get; set; }
    public Guid SanctionsScreeningId { get; set; }
    public string WatchlistName { get; set; } = string.Empty;
    public string MatchedName { get; set; } = string.Empty;
    public decimal MatchScore { get; set; }
    public string MatchType { get; set; } = string.Empty;
    public Dictionary<string, string> AdditionalData { get; set; } = new();
    public DateTime MatchedAt { get; set; }
}








