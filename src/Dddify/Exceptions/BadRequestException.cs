using FluentValidation.Results;

namespace Dddify.Exceptions;

/// <summary>
/// Represents an exception that occurs when one or more validation failures have occurred.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with the specified validation failures.
    /// </summary>
    /// <param name="failures">validation failures.</param>
    public BadRequestException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    /// <summary>
    /// Gets the dictionary containing the errors.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }
}