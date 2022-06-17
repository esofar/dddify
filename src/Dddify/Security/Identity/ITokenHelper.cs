using System.Security.Claims;

namespace Dddify.Security.Identity;

public interface ITokenHelper
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();
}