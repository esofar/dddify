namespace Dddify.Domain;

/// <summary>
/// Represents an interface for entities that require deletion auditing.
/// </summary>
public interface IDeletionAuditable : ISoftDeletable
{
    /// <summary>
    /// Gets or sets the identifier of the user who deleted the entity.
    /// </summary>
    Guid? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time of when the entity was deleted.
    /// </summary>
    DateTime? DeletedAt { get; set; }
}