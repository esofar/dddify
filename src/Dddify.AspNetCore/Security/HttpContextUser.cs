using System.Security.Claims;
using Dddify.DependencyInjection;
using Dddify.Security.Identity;
using Microsoft.AspNetCore.Http;

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

    public Guid? Id => this.FindClaimValue(DefaultClaimTypes.UserId)?.To<Guid>();

    public string UserName => this.FindClaimValue(DefaultClaimTypes.UserName);

    public string Name => this.FindClaimValue(DefaultClaimTypes.Name);

    public string Email => this.FindClaimValue(DefaultClaimTypes.Email);

    public bool EmailVerified => this.FindClaimValue<bool>(DefaultClaimTypes.EmailVerified);

    public string PhoneNumber => this.FindClaimValue(DefaultClaimTypes.PhoneNumber);

    public bool PhoneNumberVerified => this.FindClaimValue<bool>(DefaultClaimTypes.PhoneNumberVerified);
}
