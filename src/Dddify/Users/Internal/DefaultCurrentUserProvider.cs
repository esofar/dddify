using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dddify.Users.Internal;

/// <summary>
/// Resolves the current user from the active HTTP request.
/// </summary>
internal sealed class DefaultCurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public ClaimsPrincipal GetCurrentUser()
        => httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
}
