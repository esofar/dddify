﻿using FluentValidation.Results;

namespace Dddify.Exceptions;

/// <summary>
/// Represents an exception that occurs when one or more validation failures have occurred.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BadRequestException"/> class with the specified validation failures.
/// </remarks>
/// <param name="failures">validation failures.</param>
public class BadRequestException(IEnumerable<ValidationFailure> failures) 
    : Exception("One or more validation failures have occurred.")
{
    /// <summary>
    /// Gets the dictionary containing the errors.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; } = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
}