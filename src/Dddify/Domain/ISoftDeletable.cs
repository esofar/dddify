namespace Dddify.Domain;

/// <summary>
/// Soft deletion allows entities to be marked as deleted without being removed from the database, enabling recovery and auditing of deleted entities.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Determines whether the entity has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}