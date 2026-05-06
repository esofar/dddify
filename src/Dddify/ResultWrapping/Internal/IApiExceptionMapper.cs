namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Maps exceptions into API-facing error payload descriptions.
/// </summary>
public interface IApiExceptionMapper
{
    /// <summary>
    /// Maps an exception to an <see cref="ApiExceptionMapping"/>.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    ApiExceptionMapping Map(Exception exception);
}
