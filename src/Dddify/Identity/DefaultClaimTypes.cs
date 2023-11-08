namespace Dddify.Identity;

public static class DefaultClaimTypes
{
    /// <summary>
    /// Default: "userid".
    /// </summary>
    public static string UserId { get; set; } = "userid";

    /// <summary>
    /// Default: "fullname".
    /// </summary>
    public static string FullName { get; set; } = "fullname";

    /// <summary>
    /// Default: "role".
    /// </summary>
    public static string Role { get; set; } = "role";

    /// <summary>
    /// Default: "email".
    /// </summary>
    public static string Email { get; set; } = "email";

    /// <summary>
    /// Default: "emailverified".
    /// </summary>
    public static string EmailVerified { get; set; } = "emailverified";

    /// <summary>
    /// Default: "phonenumber".
    /// </summary>
    public static string PhoneNumber { get; set; } = "phonenumber";

    /// <summary>
    /// Default: "phonenumberverified".
    /// </summary>
    public static string PhoneNumberVerified { get; set; } = "phonenumberverified";

    /// <summary>
    /// Default: "tenantid".
    /// </summary>
    public static string TenantId { get; set; } = "tenantid";
}