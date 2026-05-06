namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines full auditing capabilities for an entity or aggregate root.
/// </summary>
/// <remarks>
/// This contract combines creation, modification, and deletion auditing concerns into a single
/// composite abstraction.
/// </remarks>
public interface IAuditable : ICreationAuditable, IModificationAuditable, IDeletionAuditable
{
}
