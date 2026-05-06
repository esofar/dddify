using Dddify.Users.Internal;
using System.Security.Claims;

namespace Dddify.Users;

/// <summary>
/// Configures Dddify current-user services.
/// </summary>
public sealed class CurrentUserOptions
{
    /// <summary>
    /// The default claim type used to resolve the current user identifier.
    /// </summary>
    public const string DefaultIdClaimType = ClaimTypes.NameIdentifier;

    private readonly Dictionary<Type, Type> _extendedCurrentUserTypes = [];

    /// <summary>
    /// Gets the concrete type registered for <see cref="ICurrentUserProvider"/>.
    /// </summary>
    internal Type ProviderType { get; private set; } = typeof(DefaultCurrentUserProvider);

    /// <summary>
    /// Gets the configured enhanced current-user service mappings.
    /// </summary>
    internal IReadOnlyDictionary<Type, Type> ExtendedCurrentUserTypes => _extendedCurrentUserTypes;

    /// <summary>
    /// Gets the claim type used to resolve the current user identifier.
    /// </summary>
    internal string IdClaimType { get; private set; } = DefaultIdClaimType;

    /// <summary>
    /// Registers the specified implementation type for <see cref="ICurrentUserProvider"/>.
    /// </summary>
    public CurrentUserOptions UseProvider<TCurrentUserProvider>()
        where TCurrentUserProvider : class, ICurrentUserProvider
    {
        ProviderType = typeof(TCurrentUserProvider);
        return this;
    }

    /// <summary>
    /// Sets the claim type used to resolve the current user identifier.
    /// </summary>
    public CurrentUserOptions UseIdClaim(string claimType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);

        IdClaimType = claimType;
        return this;
    }

    /// <summary>
    /// Registers an additional current-user service alongside <see cref="ICurrentUser"/>.
    /// </summary>
    /// <typeparam name="TEnhancedCurrentUser">
    /// The additional current-user abstraction to expose.
    /// </typeparam>
    /// <typeparam name="TEnhancedCurrentUserImplementation">
    /// The concrete implementation registered for <typeparamref name="TEnhancedCurrentUser"/>.
    /// </typeparam>
    public CurrentUserOptions Enhanced<TEnhancedCurrentUser, TEnhancedCurrentUserImplementation>()
        where TEnhancedCurrentUser : class, ICurrentUser
        where TEnhancedCurrentUserImplementation : class, TEnhancedCurrentUser
    {
        _extendedCurrentUserTypes[typeof(TEnhancedCurrentUser)] = typeof(TEnhancedCurrentUserImplementation);
        return this;
    }
}
