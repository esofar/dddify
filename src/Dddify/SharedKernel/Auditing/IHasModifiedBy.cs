namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines a last modifier identifier.
/// </summary>
public interface IHasModifiedBy
{
    /// <summary>
    /// Gets or sets the identifier of the user or actor who last modified the current instance.
    /// </summary>
    string? ModifiedBy { get; set; }
}
