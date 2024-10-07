namespace Dddify.Domain;

/// <summary>
/// Represents an interface for entities that require a concurrency stamp.
/// The concurrency stamp is used to prevent conflicts when multiple users or processes attempt to modify the same entity simultaneously.
/// </summary>
public interface IHasConcurrencyStamp
{
    /// <summary>
    /// Gets or sets a unique stamp used to manage concurrency.
    /// This property is typically updated whenever the entity is modified, allowing the system to detect conflicts during data updates.
    /// When the entity is saved, the current value of this property is compared with the stored value to determine if another process has modified the entity in the meantime.
    /// If a conflict is detected, an exception can be thrown, and the application can handle it accordingly.
    /// </summary>
    string? ConcurrencyStamp { get; set; }
}