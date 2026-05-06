namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines a creator identifier.
/// </summary>
public interface IHasCreatedBy
{
    /// <summary>
    /// Gets or sets the identifier of the user or actor who created the current instance.
    /// </summary>
    string? CreatedBy { get; set; }
}
