namespace Dddify.Exceptions;

/// <summary>
/// Represents a base class for application-specific exceptions.
/// </summary>
public abstract class AppException : BussinessException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public AppException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    public AppException()
    {
    }
}