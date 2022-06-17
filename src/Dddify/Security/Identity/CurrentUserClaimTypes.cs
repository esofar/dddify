namespace Dddify.Security.Identity;

/// <summary>
/// Used to get <see cref="ICurrentUser"/> claim type names.
/// </summary>
public static class CurrentUserClaimTypes
{
    /// <summary>
    /// Default: "name".
    /// </summary>
    public static string Name { get; set; } = "name";

    /// <summary>
    /// Default: "user_id".
    /// </summary>
    public static string UserId { get; set; } = "user_id";

    /// <summary>
    /// Default: "user_name".
    /// </summary>
    public static string UserName { get; set; } = "user_name";

    /// <summary>
    /// Default: "role".
    /// </summary>
    public static string Role { get; set; } = "role";

    /// <summary>
    /// Default: "email".
    /// </summary>
    public static string Email { get; set; } = "email";

    /// <summary>
    /// Default: "email_verified".
    /// </summary>
    public static string EmailVerified { get; set; } = "email_verified";

    /// <summary>
    /// Default: "phone_number".
    /// </summary>
    public static string PhoneNumber { get; set; } = "phone_number";

    /// <summary>
    /// Default: "phone_number_verified".
    /// </summary>
    public static string PhoneNumberVerified { get; set; } = "phone_number_verified";

    /// <summary>
    /// Default: "tenantid".
    /// </summary>
    public static string TenantId { get; set; } = "tenant_id";
}
