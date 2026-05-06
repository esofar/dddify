using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dddify.Users.Internal;

/// <summary>
/// Provides access to the current user resolved for the active execution context.
/// </summary>
internal sealed class CurrentUser(ICurrentUserProvider provider, IOptions<CurrentUserOptions> optionsAccessor) : ICurrentUser
{
    private readonly ClaimsPrincipal _principal = provider.GetCurrentUser();
    private readonly CurrentUserOptions _options = optionsAccessor.Value;

    public bool IsAuthenticated => _principal.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(Id);

    public string Id => FindClaim(_options.IdClaimType) ?? string.Empty;

    public IReadOnlyCollection<Claim> Claims => [.. _principal.Claims];

    public string? FindClaim(string claimType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);

        return _principal.FindFirst(claimType)?.Value;
    }
}
