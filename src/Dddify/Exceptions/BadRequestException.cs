namespace Dddify.Exceptions;

/// <summary>
/// Represents an exception that occurs when one or more validation failures have occurred.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with the specified validation errors.
    /// </summary>
    /// <param name="error">A dictionary containing the validation errors.</param>
    public BadRequestException(Dictionary<string, string[]> error)
        : base("One or more validation failures have occurred.")
    {
        Error = error;
    }

    /// <summary>
    /// Gets the dictionary containing the validation errors.
    /// </summary>
    public IDictionary<string, string[]> Error { get; }
}