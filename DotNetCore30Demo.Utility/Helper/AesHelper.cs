using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DotNetCore30Demo.Utility.Helper
{
    /// <summary>
    /// 正确使用AES对称加密
    /// </summary>
    public class AesHelper
    {
        public static string Encrypt(string password, string purpose, byte[] plainBytes)
        {
            byte[] key = PasswordToKey(password, purpose);
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                    byte[] packedBytes = Pack(
                        version: 1,
                        iv: aes.IV,
                        cipherBytes: cipherBytes);
                    return Base64UrlEncode(packedBytes);
                }
            }
        }
        public static byte[] Decrypt(string packedString, string password, string purpose)
        {
            byte[] key = PasswordToKey(password, purpose);
            byte[] packedBytes = Base64UrlDecode(packedString);
            (byte version, byte[] iv, byte[] cipherBytes) = Unpack(packedBytes);
            using (var aes = Aes.Create())
            {
                using (ICryptoTransform decryptor = aes.CreateDecryptor(key, iv))
                {
                    return decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                }
            }
        }


        static byte[] PasswordToKey(string password, string purpose)
        {
            using (var hmac = new HMACMD5(Encoding.UTF8.GetBytes(purpose)))
            {
                return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");
        }

        static byte[] Base64UrlDecode(string base64Url)
        {
            return Convert.FromBase64String(base64Url
                .Replace("_", "/")
                .Replace("-", "+"));
        }

        static (byte version, byte[] iv, byte[] cipherBytes) Unpack(byte[] packedBytes)
        {
            if (packedBytes[0] == 1)
            {
                // version 1
                return (1, packedBytes[1..17], packedBytes[17..]);
            }
            else
            {
                throw new NotImplementedException("unknown version");
            }
        }

        static byte[] Pack(byte version, byte[] iv, byte[] cipherBytes)
        {
            return new[] { version }.Concat(iv).Concat(cipherBytes).ToArray();
        }
    }
}