using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace _12306ByXX.Common
{
    /// <summary>
    /// DES加密帮助类
    /// </summary>
    public static class DesEncryptHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="unEncryptString">明文</param>
        /// <param name="key">密钥(长度必须8位以上)</param>
        /// <returns>密文</returns>
        public static string EncryptString(string unEncryptString, string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length < 8)
            {
                throw new ArgumentException("min length is 8.", "key");
            }

            var cryptoServiceProvider = new DESCryptoServiceProvider();
            var inputByteArray = Encoding.UTF8.GetBytes(unEncryptString);
            cryptoServiceProvider.Key = Encoding.UTF8.GetBytes(key);
            cryptoServiceProvider.IV = Encoding.UTF8.GetBytes(key);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, cryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
            cryptoStream.FlushFinalBlock();
            var stringBuilder = new StringBuilder();

            foreach (var b in memoryStream.ToArray())
            {
                stringBuilder.AppendFormat("{0:X2}", b);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptString">密文</param>
        /// <param name="key">//密钥(长度必须8位以上)</param>
        /// <returns>明文</returns>
        public static string DecryptString(string encryptString, string key)
        {
            if (string.IsNullOrEmpty(key) || key.Length < 8)
            {
                throw new ArgumentException("min length is 8.", "key");
            }

            var cryptoServiceProvider = new DESCryptoServiceProvider();
            var inputByteArray = new byte[encryptString.Length / 2];

            for (var x = 0; x < encryptString.Length / 2; x++)
            {
                var i = (Convert.ToInt32(encryptString.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            cryptoServiceProvider.Key = Encoding.UTF8.GetBytes(key);
            cryptoServiceProvider.IV = Encoding.UTF8.GetBytes(key);
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(memoryStream, cryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
            cryptoStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
    }
}
