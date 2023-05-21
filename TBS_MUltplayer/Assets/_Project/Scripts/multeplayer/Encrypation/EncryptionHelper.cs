using System.IO;
using System;
using System.Security.Cryptography;
using System.Text;


public class EncryptionHelper
{
    public static byte[] EncryptString(string plainText, string EncryptionKey)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.Zeros;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    byte[] data = Encoding.UTF8.GetBytes(plainText);
                    csEncrypt.Write(data, 0, data.Length);
                }
                return msEncrypt.ToArray();
            }
        }
    }

    public static byte[] DecryptBytes(byte[] encryptedData, int length, string EncryptionKey)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.Zeros;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    csDecrypt.Write(encryptedData, 0, length);
                }
                return msDecrypt.ToArray();
            }
        }
    }

}

