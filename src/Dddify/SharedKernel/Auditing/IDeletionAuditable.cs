namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines deletion auditing capabilities.
/// </summary>
/// <remarks>
/// This contract combines soft deletion with deleter and deletion time information for an entity or aggregate root.
/// </remarks>
public interface IDeletionAuditable : IHasDeletedBy, IHasDeletedAt, ISoftDeletable
{
}
