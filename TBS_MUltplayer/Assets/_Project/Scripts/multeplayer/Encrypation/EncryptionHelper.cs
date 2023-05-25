
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


public class EncryptionHelper
{

    public static byte[] Encrypt(string data, string publicKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(publicKeyXml);
            byte[] plainBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = rsa.Encrypt(plainBytes, false);
            return encryptedBytes;
        }
    }

    public static string Decrypt(byte[] encryptedData, string privateKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKeyXml);
            byte[] decryptedBytes = rsa.Decrypt(encryptedData, false);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedText;
        }
    }
}


