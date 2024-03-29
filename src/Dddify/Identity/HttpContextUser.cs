﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Dddify.Identity;

public class HttpContextUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Id.HasValue;

    public Guid? Id => this.FindClaimValue(DefaultClaimTypes.UserId)?.ToGuid();

    public string? Name => this.FindClaimValue(DefaultClaimTypes.FullName);

    public string? Email => this.FindClaimValue(DefaultClaimTypes.Email);

    public bool EmailVerified => this.FindClaimValue<bool>(DefaultClaimTypes.EmailVerified);

    public string? PhoneNumber => this.FindClaimValue(DefaultClaimTypes.PhoneNumber);

    public bool PhoneNumberVerified => this.FindClaimValue<bool>(DefaultClaimTypes.PhoneNumberVerified);
}