namespace Dddify.Domain;

/// <summary>
/// This interface can be implemented to add standard logical deletion property to a class.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Determines whether the entity has been deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}