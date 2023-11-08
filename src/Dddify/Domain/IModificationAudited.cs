namespace Dddify.Domain;

/// <summary>
/// This interface can be implemented to store modification information (who and when modified lastly).
/// </summary>
public interface IModificationAudited
{
    /// <summary>
    /// The last updater for this entity.
    /// </summary>
    Guid? UpdatedBy { get; set; }

    /// <summary>
    /// The last updated time for this entity.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}