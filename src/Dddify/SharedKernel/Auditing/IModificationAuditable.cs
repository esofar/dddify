namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines modification auditing capabilities.
/// </summary>
/// <remarks>
/// This contract combines the last modifier identifier and last modification time for an entity or aggregate root.
/// </remarks>
public interface IModificationAuditable : IHasModifiedBy, IHasModifiedAt
{
}
