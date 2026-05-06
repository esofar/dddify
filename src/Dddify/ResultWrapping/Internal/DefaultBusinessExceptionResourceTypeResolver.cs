using Dddify.Exceptions;
using Dddify.Localization;

namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Resolves business exception messages from the shared localization resource set.
/// </summary>
public class DefaultBusinessExceptionResourceTypeResolver : IBusinessExceptionResourceTypeResolver
{
    /// <summary>
    /// Resolves the default resource type for business exceptions.
    /// </summary>
    /// <param name="exception">The business exception to resolve.</param>
    public Type Resolve(BusinessException exception) => typeof(SharedResource);
}
