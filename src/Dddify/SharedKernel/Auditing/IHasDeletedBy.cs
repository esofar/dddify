namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines a deleter identifier.
/// </summary>
public interface IHasDeletedBy
{
    /// <summary>
    /// Gets or sets the identifier of the user or actor who soft deleted the current instance.
    /// </summary>
    string? DeletedBy { get; set; }
}
