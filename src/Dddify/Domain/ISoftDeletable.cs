namespace Dddify.Domain;

/// <summary>
/// This interface can be implemented to add standard logical deletion property to a class.
/// This interface will add a IsDeleted property that marks whether the entity is logically deleted.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Determines whether the entity has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// The deletor for this entity.
    /// </summary>
    Guid? DeletedBy { get; set; }

    /// <summary>
    /// The deleted time for this entity.
    /// </summary>
    DateTime? DeletedAt { get; set; }
}