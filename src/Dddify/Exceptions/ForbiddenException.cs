namespace Dddify.Exceptions;

/// <summary>
/// Represents an exception that occurs when access to a specified resource is forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with a default error message.
    /// </summary>
    public ForbiddenException()
        : base("The specified resource was forbidden.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }
}