namespace Dddify.Domain;

/// <summary>
/// Combines auditing interfaces for <see cref="ICreationAuditable"/>, <see cref="IModificationAuditable"/>, and <see cref="IDeletionAuditable"/>.
/// </summary>
public interface IFullAuditable : ICreationAuditable, IModificationAuditable, IDeletionAuditable
{
}