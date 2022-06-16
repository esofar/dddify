using Dddify.DependencyInjection;
using Dddify.Security.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dddify.AspNetCore.Security;

public class HttpContextUser : ICurrentUser, ITransientDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Id.HasValue;

    public Guid? Id => this.FindClaimValue(CurrentUserClaimTypes.UserId)?.To<Guid>();

    public string? UserName => this.FindClaimValue(CurrentUserClaimTypes.UserName);

    public string? Name => this.FindClaimValue(CurrentUserClaimTypes.Name);

    public string? Email => this.FindClaimValue(CurrentUserClaimTypes.Email);

    public bool EmailVerified => this.FindClaimValue<bool>(CurrentUserClaimTypes.EmailVerified);

    public string? PhoneNumber => this.FindClaimValue(CurrentUserClaimTypes.PhoneNumber);

    public bool PhoneNumberVerified => this.FindClaimValue<bool>(CurrentUserClaimTypes.PhoneNumberVerified);
}
