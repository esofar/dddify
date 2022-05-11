using System.Text;
using System.Security.Cryptography;

namespace Dddify.Security.Cryptography;

/// <summary>
/// This class is used to provide related methods for <see cref="MD5"/>.
/// </summary>
public class Md5
{
    public static string Compute(string input)
    {
        using var md5 = MD5.Create();
        var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return HashToCompactString(result);
    }

    public static string Compute(byte[] buffer)
    {
        using var md5 = MD5.Create();
        var result = md5.ComputeHash(buffer);
        return HashToCompactString(result);
    }

    public static string Compute(Stream stream)
    {
        using var md5 = MD5.Create();
        var result = md5.ComputeHash(stream);
        return HashToCompactString(result);
    }

    private static string HashToCompactString(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
    }
}
