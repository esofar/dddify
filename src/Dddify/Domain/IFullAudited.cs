namespace Dddify.Domain;

/// <summary>
/// This interface can be implemented to add standard auditing properties to a class.
/// </summary>
public interface IFullAudited : ICreationAudited, IModificationAudited
{
}