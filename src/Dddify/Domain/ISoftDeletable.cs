namespace Dddify.Domain;

/// <summary>
/// Represents an interface for entities that support soft-deletion.
/// Soft deletion allows entities to be marked as deleted without being removed from the database, enabling recovery and auditing of deleted entities.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Determines whether the entity has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time of when the entity was deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }
}