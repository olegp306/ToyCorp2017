//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using AdvantShop.Diagnostics;

namespace AdvantShop.Helpers
{
    public class SecurityHelper
    {
        private static byte[] SymmetricKey
        {
            get { return Encoding.UTF8.GetBytes("1B2c3D4e5F6g7H81"); }
        }

        public static byte[] EncryptString(string data)
        {
            using (var target = new MemoryStream())
            {
                var clearData = Encoding.UTF8.GetBytes(data);
                using (var algorithm = SymmetricAlgorithm.Create())
                {
                    algorithm.Key = SymmetricKey;
                    algorithm.GenerateIV();
                    target.Write(algorithm.IV, 0, algorithm.IV.Length);
                    using (var cs = new CryptoStream(target, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearData, 0, clearData.Length);
                        cs.FlushFinalBlock();
                        return target.ToArray();
                    }
                }
            }
        }

        public static string DecryptString(byte[] data)
        {
            using (var target = new MemoryStream())
            {
                using (var algorithm = SymmetricAlgorithm.Create())
                {
                    algorithm.Key = SymmetricKey;
                    var readPos = 0;
                    var iv = new byte[algorithm.IV.Length ];
                    Array.Copy(data, iv, iv.Length);
                    algorithm.IV = iv;
                    readPos += algorithm.IV.Length;
                    using (var cs = new CryptoStream(target, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(data, readPos, data.Length - readPos);
                        cs.FlushFinalBlock();
                        return Encoding.UTF8.GetString(target.ToArray());
                    }
                }
            }
        }

        public static string GetPasswordHash(string password)
        {
            try
            {
                byte[] byteRepresentation = Encoding.UTF8.GetBytes(password);
                using (var myMd5 = new MD5CryptoServiceProvider())
                {
                    return Convert.ToBase64String(myMd5.ComputeHash(byteRepresentation));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, password);
                return string.Empty;
            }
        }
    }
}