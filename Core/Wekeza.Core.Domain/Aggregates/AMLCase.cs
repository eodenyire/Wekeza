using Wekeza.Core.Domain.Common;
using Wekeza.Core.Domain.Events;
using Wekeza.Core.Domain.ValueObjects;
using Wekeza.Core.Domain.Enums;

namespace Wekeza.Core.Domain.Aggregates;

public class AMLCase : AggregateRoot
{
    public string CaseNumber { get; private set; }
    public Guid? PartyId { get; private set; }
    public Guid? TransactionId { get; private set; }
    public AMLAlertType AlertType { get; private set; }
    public RiskScore RiskScore { get; private set; }
    public AMLCaseStatus Status { get; private set; }
    public string? InvestigatorId { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? AssignedDate { get; private set; }
    public DateTime? ClosedDate { get; private set; }
    public AMLResolution? Resolution { get; private set; }
    public bool SARFiled { get; private set; }
    public string? SARReference { get; private set; }
    public List<AMLEvidence> Evidence { get; private set; }
    public List<AMLNote> Notes { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private AMLCase() : base(Guid.NewGuid())
    {
        Evidence = new List<AMLEvidence>();
        Notes = new List<AMLNote>();
    }

    public static AMLCase Create(
        string caseNumber,
        AMLAlertType alertType,
        RiskScore riskScore,
        Guid? partyId = null,
        Guid? transactionId = null,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(caseNumber))
            throw new ArgumentException("Case number cannot be empty", nameof(caseNumber));

        if (partyId == null && transactionId == null)
            throw new ArgumentException("Either PartyId or TransactionId must be provided");

        var amlCase = new AMLCase
        {
            Id = Guid.NewGuid(),
            CaseNumber = caseNumber,
            PartyId = partyId,
            TransactionId = transactionId,
            AlertType = alertType,
            RiskScore = riskScore,
            Status = AMLCaseStatus.Open,
            CreatedDate = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrWhiteSpace(description))
        {
            amlCase.AddNote("SYSTEM", "Case Created", description);
        }

        amlCase.AddDomainEvent(new AMLCaseCreatedDomainEvent(
            amlCase.Id, amlCase.CaseNumber, amlCase.AlertType, amlCase.RiskScore));

        return amlCase;
    }

    public void AssignInvestigator(string investigatorId, string assignedBy)
    {
        if (Status != AMLCaseStatus.Open)
            throw new InvalidOperationException($"Cannot assign investigator to case in {Status} status");

        if (string.IsNullOrWhiteSpace(investigatorId))
            throw new ArgumentException("Investigator ID cannot be empty", nameof(investigatorId));

        InvestigatorId = investigatorId;
        AssignedDate = DateTime.UtcNow;
        Status = AMLCaseStatus.UnderReview;
        UpdatedAt = DateTime.UtcNow;

        AddNote(assignedBy, "Case Assigned", $"Case assigned to investigator: {investigatorId}");

        AddDomainEvent(new AMLCaseAssignedDomainEvent(Id, CaseNumber, investigatorId));
    }

    public void AddEvidence(string evidenceType, string description, string? filePath = null, string addedBy = "SYSTEM")
    {
        if (Status == AMLCaseStatus.Closed)
            throw new InvalidOperationException("Cannot add evidence to closed case");

        var evidence = new AMLEvidence
        {
            Id = Guid.NewGuid(),
            AMLCaseId = Id,
            EvidenceType = evidenceType,
            Description = description,
            FilePath = filePath,
            AddedBy = addedBy,
            AddedDate = DateTime.UtcNow
        };

        Evidence.Add(evidence);
        UpdatedAt = DateTime.UtcNow;

        AddNote(addedBy, "Evidence Added", $"Evidence type: {evidenceType} - {description}");

        AddDomainEvent(new AMLEvidenceAddedDomainEvent(Id, CaseNumber, evidenceType, description));
    }

    public void AddNote(string author, string noteType, string content)
    {
        if (Status == AMLCaseStatus.Closed)
            throw new InvalidOperationException("Cannot add notes to closed case");

        var note = new AMLNote
        {
            Id = Guid.NewGuid(),
            AMLCaseId = Id,
            Author = author,
            NoteType = noteType,
            Content = content,
            CreatedDate = DateTime.UtcNow
        };

        Notes.Add(note);
        UpdatedAt = DateTime.UtcNow;
    }

    public void EscalateCase(string escalatedBy, string reason)
    {
        if (Status == AMLCaseStatus.Closed)
            throw new InvalidOperationException("Cannot escalate closed case");

        Status = AMLCaseStatus.Escalated;
        UpdatedAt = DateTime.UtcNow;

        AddNote(escalatedBy, "Case Escalated", reason);

        AddDomainEvent(new AMLCaseEscalatedDomainEvent(Id, CaseNumber, reason));
    }

    public void FileSAR(string sarReference, string filedBy, string reason)
    {
        if (Status == AMLCaseStatus.Closed)
            throw new InvalidOperationException("Cannot file SAR for closed case");

        if (string.IsNullOrWhiteSpace(sarReference))
            throw new ArgumentException("SAR reference cannot be empty", nameof(sarReference));

        SARFiled = true;
        SARReference = sarReference;
        UpdatedAt = DateTime.UtcNow;

        AddNote(filedBy, "SAR Filed", $"SAR Reference: {sarReference} - Reason: {reason}");

        AddDomainEvent(new SARFiledDomainEvent(Id, CaseNumber, sarReference, reason));
    }

    public void CloseCase(AMLResolution resolution, string closedBy, string reason)
    {
        if (Status == AMLCaseStatus.Closed)
            throw new InvalidOperationException("Case is already closed");

        if (string.IsNullOrWhiteSpace(closedBy))
            throw new ArgumentException("Closed by cannot be empty", nameof(closedBy));

        Status = AMLCaseStatus.Closed;
        Resolution = resolution;
        ClosedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddNote(closedBy, "Case Closed", $"Resolution: {resolution} - Reason: {reason}");

        AddDomainEvent(new AMLCaseClosedDomainEvent(Id, CaseNumber, resolution, reason));
    }

    public void ReopenCase(string reopenedBy, string reason)
    {
        if (Status != AMLCaseStatus.Closed)
            throw new InvalidOperationException("Only closed cases can be reopened");

        Status = AMLCaseStatus.UnderReview;
        Resolution = null;
        ClosedDate = null;
        UpdatedAt = DateTime.UtcNow;

        AddNote(reopenedBy, "Case Reopened", reason);

        AddDomainEvent(new AMLCaseReopenedDomainEvent(Id, CaseNumber, reason));
    }

    public void UpdateRiskScore(RiskScore newRiskScore, string updatedBy, string reason)
    {
        if (Status == AMLCaseStatus.Closed)
            throw new InvalidOperationException("Cannot update risk score for closed case");

        var oldRiskScore = RiskScore;
        RiskScore = newRiskScore;
        UpdatedAt = DateTime.UtcNow;

        AddNote(updatedBy, "Risk Score Updated", 
            $"Risk score changed from {oldRiskScore.Score} to {newRiskScore.Score} - Reason: {reason}");

        AddDomainEvent(new AMLRiskScoreUpdatedDomainEvent(Id, CaseNumber, oldRiskScore, newRiskScore));
    }

    public bool IsOpen => Status == AMLCaseStatus.Open;
    public bool IsUnderReview => Status == AMLCaseStatus.UnderReview;
    public bool IsEscalated => Status == AMLCaseStatus.Escalated;
    public bool IsClosed => Status == AMLCaseStatus.Closed;
    public bool IsHighRisk => RiskScore.IsHighRisk;
    public int DaysOpen => (DateTime.UtcNow - CreatedDate).Days;
    public int EvidenceCount => Evidence.Count;
    public int NotesCount => Notes.Count;
}

public class AMLEvidence
{
    public Guid Id { get; set; }
    public Guid AMLCaseId { get; set; }
    public string EvidenceType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string AddedBy { get; set; } = string.Empty;
    public DateTime AddedDate { get; set; }
}

public class AMLNote
{
    public Guid Id { get; set; }
    public Guid AMLCaseId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string NoteType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}



public enum AMLCaseStatus
{
    Open,
    UnderReview,
    Escalated,
    Closed
}



