using System.Security.Cryptography;
using System.Text;

namespace DotNetCore30Demo.Utility.Helper
{
    public  class HashAlgorithmHelper
    {
        static readonly char[] Digitals = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

         string ToString(byte[] bytes)
        {
            //unsafe
            //{
            //    const int byte_len = 2; // 表示一个 byte 的字符长度。

            //    var str = new string('\0', byte_len * bytes.Length);

            //    fixed (char* pStr = str)
            //    {
            //        var pStr2 = pStr; // fixed pStr 是只读的，所以我们定义一个变量。

            //        foreach (var item in bytes)
            //        {
            //            *pStr2 = Digitals[item >> 4/* byte high */]; ++pStr2;
            //            *pStr2 = Digitals[item & 15/* byte high */]; ++pStr2;
            //        }
            //    }

            //    return str;
            //}
            const int byte_len = 2; // 表示一个 byte 的字符长度。

            var chars = new char[byte_len * bytes.Length];

            var index = 0;

            foreach (var item in bytes)
            {
                chars[index] = Digitals[item >> 4/* byte high */]; ++index;
                chars[index] = Digitals[item & 15/* byte high */]; ++index;
            }

            return new string(chars);
        }

        public  string ComputeHash<THashAlgorithm>(string input) where THashAlgorithm : HashAlgorithm
        {
            var bytes = Encoding.UTF8.GetBytes(input);

            return ToString(THashAlgorithmInstances<THashAlgorithm>.Instance.ComputeHash(bytes));
        }
    }
}