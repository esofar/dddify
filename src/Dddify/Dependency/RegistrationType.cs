﻿namespace Dddify.Dependency;

/// <summary>
/// Defines the registration types for services.
/// </summary>
public enum RegistrationType
{
    /// <summary>
    /// Registers the specified service as itself.
    /// </summary>
    AsSelf,

    /// <summary>
    /// Registers the specified service with the first found matching interface name.
    /// </summary>
    /// <remarks>
    /// (e.g. ClassName is matched to IClassName)
    /// </remarks>
    AsMatchingInterface,

    /// <summary>
    /// Registers the specified service as all of its implemented interfaces.
    /// </summary>
    AsImplementedInterfaces,
}
