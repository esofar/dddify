namespace Dddify.Auditing;

/// <summary>
/// This interface can be implemented to store modification information (who and when modified lastly).
/// </summary>
public interface IModificationAudited
{
    /// <summary>
    /// The last modifier for this entity.
    /// </summary>
    Guid? LastModifiedBy { get; set; }

    /// <summary>
    /// The last modified time for this entity.
    /// </summary>
    DateTimeOffset? LastModifiedAt { get; set; }
}
