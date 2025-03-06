namespace Dddify.Exceptions;

/// <summary>
/// Represents exceptions that occur at the application level, usually related to application logic and user interactions.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="KnownException"/>.
/// </remarks>
public abstract class ApplicationException : KnownException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ApplicationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationException"/> class.
    /// </summary>
    public ApplicationException()
    {
    }
}