namespace Dddify.Exceptions;

/// <summary>
/// Represents an exception that occurs when access to a specified resource is unauthorized.
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with a default error message.
    /// </summary>
    public UnauthorizedException()
       : base("The specified resource was not authorized.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}