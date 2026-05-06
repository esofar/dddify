namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines creation auditing capabilities.
/// </summary>
/// <remarks>
/// This contract combines the creator identifier and creation time for an entity or aggregate root.
/// </remarks>
public interface ICreationAuditable : IHasCreatedBy, IHasCreatedAt
{
}
