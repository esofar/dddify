using Dddify.Exceptions;

namespace Dddify.ResultWrapping;

/// <summary>
/// Resolves the localization resource type used for a business exception.
/// </summary>
public interface IBusinessExceptionResourceTypeResolver
{
    /// <summary>
    /// Resolves the resource type used to localize the specified business exception.
    /// </summary>
    /// <param name="exception">The business exception to resolve.</param>
    Type Resolve(BusinessException exception);
}
