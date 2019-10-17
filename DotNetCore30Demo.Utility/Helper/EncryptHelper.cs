using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DotNetCore30Demo.Utility.Helper
{
    public class EncryptHelper
    {
        /// <summary>
        /// 对字符串进行SHA1加密
        /// </summary>
        /// <param name="sourceString">需要加密的字符串</param>
        /// <returns>密文</returns>
        public  string SHA1_Encrypt(string sourceString)
        {
            byte[] strRes = Encoding.Default.GetBytes(sourceString);
            HashAlgorithm iSha = new SHA1CryptoServiceProvider();
            strRes = iSha.ComputeHash(strRes);
            StringBuilder enText = new StringBuilder();
            foreach (byte iByte in strRes)
            {
                enText.AppendFormat("{0:x2}", iByte);
            }
            return enText.ToString();
        }

        /// <summary>
        /// SHA256加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public  string Sha256Encrypt(string str)
        {
            SHA256 s256 = new SHA256Managed();
            byte[] bytes;
            bytes = s256.ComputeHash(Encoding.Default.GetBytes(str));
            s256.Clear();
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// SHA384加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public  string Sha384Encrypt(string str)
        {
            SHA384 s384 = new SHA384Managed();
            byte[] bytes;
            bytes = s384.ComputeHash(Encoding.Default.GetBytes(str));
            s384.Clear();
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }


        /// <summary>
        /// SHA512加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public  string Sha512Encrypt(string str)
        {
            SHA512 s512 = new SHA512Managed();
            byte[] bytes;
            bytes = s512.ComputeHash(Encoding.Default.GetBytes(str));
            s512.Clear();
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// aes加密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public  string AESEncrypt(string content, string password)
        {
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(password);
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(content);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// aes解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public  string AESDecrypt(string content, string password)
        {
            try
            {
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(password);
                byte[] toEncryptArray =Convert.FromBase64String(content);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="content">被加密的明文</param>
        /// <param name="key">密钥</param>
        /// <param name="vector">向量</param>
        /// <returns>密文</returns>
        public  string AESEncrypt(string content, string key, string vector)
        {
            string str = "";
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(content);
                byte[] bKey = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
                byte[] bVector = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);
                byte[] Cryptograph = null; // 加密后的密文
                Rijndael Aes = Rijndael.Create();

                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(data, 0, data.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                        str = Convert.ToBase64String(Cryptograph);
                    }
                }
            }
            catch
            {
                str = null;
            }
            return str;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="content">被解密的密文</param>
        /// <param name="vector">向量</param>
        /// <param name="key"></param>
        /// <returns>明文</returns>
        public  string AESDecrypt(string content, string key, string vector)
        {
            string str = "";
            try
            {
                byte[] data = Convert.FromBase64String(content);
                byte[] bKey = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
                byte[] bVector = new byte[16];
                Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);
                Rijndael Aes = Rijndael.Create();

                // 开辟一块内存流，存储密文
                using (MemoryStream Memory = new MemoryStream())
                {

                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Decryptor = new CryptoStream(Memory, Aes.CreateDecryptor(bKey, bVector), CryptoStreamMode.Write))
                    {
                        Decryptor.Write(data, 0, data.Length);
                        Decryptor.FlushFinalBlock();
                        str = Encoding.UTF8.GetString(Memory.ToArray());
                    }
                }
            }
            catch
            {
                str = null;
            }
            return str;
        }


        public  string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
