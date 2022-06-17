using Dddify.DependencyInjection;
using System.Security.Cryptography;
using System.Text;

namespace Dddify.Security.Identity;

public class PasswordHelper : IPasswordHelper, ISingletonDependency
{
    public void GeneratePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public void GeneratePassword(string password, out string passwordHash, out string passwordSalt)
    {
        GeneratePassword(password, out byte[] passwordHashBytes, out byte[] passwordSaltBytes);

        passwordHash = Convert.ToBase64String(passwordHashBytes);
        passwordSalt = Convert.ToBase64String(passwordSaltBytes);
    }

    public bool IsValid(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }

    public bool IsValid(string password, string passwordHash, string passwordSalt)
    {
        var passwordHashBytes = Convert.FromBase64String(passwordHash);
        var passwordSaltBytes = Convert.FromBase64String(passwordSalt);

        return IsValid(password, passwordHashBytes, passwordSaltBytes);
    }
}