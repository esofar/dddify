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

    /// <summary>
    /// Creates a new <see cref="Guid"/> and returns its string representation using the specified format.
    /// </summary>
    /// <param name="format">The format specifier. Default value is "N".</param>
    /// <returns>The string representation of the created <see cref="Guid"/>.</returns>
    string CreateAsString(string format = "N");
}