namespace Dddify.Exceptions;

/// <summary>
/// Represents exceptions that occur within the domain layer, typically related to domain rules and invariants.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="KnownException"/>.
/// </remarks>
public abstract class DomainException : KnownException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    public DomainException()
    {
    }
}