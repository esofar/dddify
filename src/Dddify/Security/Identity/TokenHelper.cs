using Dddify.DependencyInjection;
using Dddify.Timing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Dddify.Security.Identity;

public class TokenHelper : ITokenHelper, ISingletonDependency
{
    private readonly IConfiguration _configuration;
    private readonly IClock _clock;

    public TokenHelper(IConfiguration configuration, IClock clock)
    {
        _configuration = configuration;
        _clock = clock;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var jwtBearer = _configuration.GetSection("Authentication:JwtBearer");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtBearer["SecurityKey"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            notBefore: _clock.Now,
            expires: _clock.Now.AddMinutes(jwtBearer["AccessTokenExpireMinutes"].To<int>()),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
