using System.Security.Cryptography;
using System.Text;

namespace Dddify.Security.Cryptography;

/// <summary>
/// This class is used to provide related methods for DES.
/// </summary>
public class Des
{
    /// <summary>
    /// The default secret key.
    /// </summary>
    private const string _defaultSecretKey = "$@^&*#%!";

    /// <summary>
    /// Encrypt with the specified key.
    /// </summary>
    /// <param name="text">Clear text.</param>
    /// <param name="secretKey">The specified key, Can include any character, but the total length must be equal to 8.</param>
    /// <returns>Cipher text.</returns>
    public static string Encrypt(string text, string secretKey)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        using (var ms = new MemoryStream())
        {
            var provider = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(secretKey),
                IV = Encoding.ASCII.GetBytes(secretKey)
            };

            var bytes = Encoding.GetEncoding("UTF-8").GetBytes(text);

            var cs = new CryptoStream(ms, provider.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            var result = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                result.AppendFormat("{0:X2}", b);
            }
            return result.ToString();
        }
    }

    /// <summary>
    /// Decrypt with the specified key.
    /// </summary>
    /// <param name="cipherText">Cipher text.</param>
    /// <param name="secretKey">The specified key.</param>
    /// <returns>Clear text.</returns>
    public static string Decrypt(string cipherText, string secretKey)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return null;
        }

        using (var ms = new MemoryStream())
        {
            var bytes = new byte[cipherText.Length / 2];

            for (int x = 0; x < cipherText.Length / 2; x++)
            {
                int i = Convert.ToInt32(cipherText.Substring(x * 2, 2), 16);
                bytes[x] = (byte)i;
            }

            var provider = new DESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(secretKey),
                IV = Encoding.ASCII.GetBytes(secretKey)
            };

            var cs = new CryptoStream(ms, provider.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();

            return Encoding.GetEncoding("UTF-8").GetString(ms.ToArray());
        }
    }

    /// <summary>
    /// Encrypt with the default key.
    /// </summary>
    /// <param name="text">Clear text.</param>
    /// <returns>Cipher text.</returns>
    public static string Encrypt(string text)
    {
        return Encrypt(text, _defaultSecretKey);
    }

    /// <summary>
    /// Decrypt with the default key.
    /// </summary>
    /// <param name="cipherText">Cipher text.</param>
    /// <returns>Clear text.</returns>
    public static string Decrypt(string cipherText)
    {
        return Decrypt(cipherText, _defaultSecretKey);
    }
}
