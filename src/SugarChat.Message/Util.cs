using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message
{
    public static class Util
    {
        public static string CreateId(string info)
        {
            return info.ToMd5();
        }

        private static string ToMd5(this string strToEncrypt)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(strToEncrypt));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}
