namespace Dddify.Security.Identity;

public interface IPasswordHelper
{
    void GeneratePassword(string password, out byte[] passwordHash, out byte[] passwordSalt);

    void GeneratePassword(string password, out string passwordHash, out string passwordSalt);

    bool IsValid(string password, byte[] passwordHash, byte[] passwordSalt);

    bool IsValid(string password, string passwordHash, string passwordSalt);
}