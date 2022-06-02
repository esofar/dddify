using System;
using System.Collections.Generic;

namespace Dddify.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException()
        : base("One or more validation failures have occurred.")
    {
    }

    public BadRequestException(Dictionary<string, string[]> errors)
        : this()
    {
        Errors = errors;
    }

    public BadRequestException(string message)
        : base(message)
    {
    }

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();

}
