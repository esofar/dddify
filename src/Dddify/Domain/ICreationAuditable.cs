namespace Dddify.Domain;

/// <summary>
/// Represents an interface for entities that require creation auditing.
/// </summary>
/// <remarks>
/// This interface includes properties to track who created the entity and when it was created.
/// </remarks>
public interface ICreationAuditable
{
    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    Guid? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time of when the entity was created.
    /// </summary>
    DateTime? CreatedAt { get; set; }
}