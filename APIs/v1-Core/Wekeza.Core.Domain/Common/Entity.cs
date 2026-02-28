namespace Wekeza.Core.Domain.Common;

/// <summary>
/// 2. Entity.cs
/// In a bank, an Entity is something that has a unique identity that persists over time (like an Account). Two accounts with the same balance are not the same account.
/// </summary>

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        Id = id;
    }

    // Standard equality checks for Entities (Identity-based)
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override int GetHashCode() => Id.GetHashCode();
}
