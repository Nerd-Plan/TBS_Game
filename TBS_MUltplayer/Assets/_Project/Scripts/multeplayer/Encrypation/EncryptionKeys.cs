using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class EncryptionKeys
{
    public string private_key ;
    public string public_key;

    public EncryptionKeys(string private_key, string public_key)
    {
        this.private_key = private_key;
        this.public_key = public_key;
    }
    public EncryptionKeys()
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(512))
        {
            // Export private key
            private_key = rsa.ToXmlString(true);

            // Export public key
            public_key = rsa.ToXmlString(false);
        }
    }
}
