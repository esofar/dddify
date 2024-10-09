namespace Dddify.Guids;

/// <summary>
/// Represents an interface for generating sequential GUIDs.
/// </summary>
public interface IGuidGenerator
{
    /// <summary>
    /// Creates a new <see cref="Guid"/>.
    /// </summary>
    /// <returns>A new <see cref="Guid"/> value.</returns>
    Guid Create();
}