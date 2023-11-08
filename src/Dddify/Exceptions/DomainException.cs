namespace Dddify.Exceptions;

/// <summary>
/// Represents a base class for domain-specific exceptions.
/// </summary>
public abstract class DomainException : BussinessException
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