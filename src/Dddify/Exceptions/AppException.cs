namespace Dddify.Exceptions;

/// <summary>
/// Represents application layer exception, usually related to application flow, orchestration, or use case execution.
/// </summary>
/// <remarks>
/// This exception indicates an error during the execution of an application command or query, such as missing data,
/// invalid operation requests, or external service failures. It should be used to signal predictable, user-facing
/// application errors, rather than technical infrastructure issues.
/// </remarks>
public abstract class AppException : BusinessException
{
    public override string Category => "Application";
}
