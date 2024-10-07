namespace Dddify.Domain;

/// <summary>
/// Represents an interface for entities that require modification auditing.
/// This interface includes properties to track who updated the entity and when it was last updated.
/// </summary>
public interface IModificationAuditable
{
    /// <summary>
    /// Gets or sets the identifier of the user who last updated the entity.
    /// </summary>
    Guid? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time of when the entity was last updated.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}