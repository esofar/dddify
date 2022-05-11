namespace Dddify.Auditing;

/// <summary>
/// This interface can be implemented to store deletion information (who and when deleted).
/// </summary>
public interface IDeletionAudited
{
    /// <summary>
    /// The deleted for this entity.
    /// </summary>
    Guid? DeletedBy { get; set; }

    /// <summary>
    /// The deleted time for this entity.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }
}
