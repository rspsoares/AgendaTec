using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace AgendaTec.Business.Helpers
{
    public static class SecurityHelper
    {
        private const string _aesIV = "5CHEysozgKcIWvrS/y5ppQ==";

        public static byte[] Encrypt(SecureString secureText)
        {
            byte[] encrypted = null;

            if (secureText.Length.Equals(0))
                return null;

            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = Convert.FromBase64String(Properties.Resources.AesKey);
                aesAlg.IV = Convert.FromBase64String(_aesIV);

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(secureText.ToUnsecureString());

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        public static SecureString Decrypt(byte[] cipherText)
        {
            char[] decripted;

            if (cipherText.Length.Equals(0))
                return new SecureString();

            using (var aesAlg = new AesManaged())
            {
                aesAlg.Key = Convert.FromBase64String(Properties.Resources.AesKey);
                aesAlg.IV = Convert.FromBase64String(_aesIV);

                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            using (var ms = new MemoryStream())
                            {
                                srDecrypt.BaseStream.CopyTo(ms);
                                decripted = Encoding.UTF8.GetString(ms.ToArray()).ToCharArray();
                            }
                        }
                    }
                }
            }

            return decripted.ToSecureString();
        }      
    }
}
