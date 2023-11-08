namespace Dddify.Domain;

/// <summary>
/// This interface can be implemented to add standard concurrency stamp to a class.
/// </summary>
public interface IHasConcurrencyStamp
{
    /// <summary>
    /// The concurrency stamp for this entity.
    /// </summary>
    string? ConcurrencyStamp { get; set; }
}