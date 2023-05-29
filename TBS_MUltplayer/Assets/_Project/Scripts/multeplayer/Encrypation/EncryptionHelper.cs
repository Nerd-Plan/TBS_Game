
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

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

    public static string Decrypt(byte[] encryptedData, int chunkSize, string privateKeyXml)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKeyXml);

            List<byte[]> encryptedChunks = ChunkData(encryptedData, chunkSize);
            List<byte[]> decryptedChunks = new List<byte[]>();

            foreach (byte[] encryptedChunk in encryptedChunks)
            {
                byte[] decryptedChunk = rsa.Decrypt(encryptedChunk, false);
                decryptedChunks.Add(decryptedChunk);
            }

            byte[] decryptedBytes = CombineChunks(decryptedChunks);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedText;
        }
    }

    private static List<byte[]> ChunkData(byte[] data, int chunkSize)
    {
        List<byte[]> chunks = new List<byte[]>();

        for (int i = 0; i < data.Length; i += chunkSize)
        {
            int length = Math.Min(chunkSize, data.Length - i);
            byte[] chunk = new byte[length];
            Array.Copy(data, i, chunk, 0, length);
            chunks.Add(chunk);
        }

        return chunks;
    }

    private static byte[] CombineChunks(List<byte[]> chunks)
    {
        int totalLength = chunks.Sum(chunk => chunk.Length);
        byte[] combinedBytes = new byte[totalLength];
        int offset = 0;

        foreach (byte[] chunk in chunks)
        {
            Buffer.BlockCopy(chunk, 0, combinedBytes, offset, chunk.Length);
            offset += chunk.Length;
        }

        return combinedBytes;
    }
}


