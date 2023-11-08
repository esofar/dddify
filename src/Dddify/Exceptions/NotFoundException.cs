namespace Dddify.Exceptions;

/// <summary>
/// Represents an exception that occurs when a specified resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a default error message.
    /// </summary>
    public NotFoundException()
        : base("The specified resource was not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }
}