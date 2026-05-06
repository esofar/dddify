namespace Dddify.SharedKernel;

/// <summary>
/// Defines a concurrency token for optimistic concurrency control.
/// </summary>
/// <remarks>
/// A concurrency stamp helps detect conflicting updates when multiple writers attempt to modify the
/// same entity or aggregate root.
/// </remarks>
public interface IHasConcurrencyStamp
{
    /// <summary>
    /// Gets or sets the concurrency stamp used to validate concurrent updates.
    /// </summary>
    string? ConcurrencyStamp { get; set; }
}
