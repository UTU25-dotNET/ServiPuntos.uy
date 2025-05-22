using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Web;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;


public class PKCE
{
    [SecuritySafeCritical]
    internal static string Create(int len, string option)
    {
       Console.WriteLine("Create PKCE");
        byte[] code_verifier_bytes = GetRandomBytes(len);
        string code_verifier = System.Text.Encoding.UTF8.GetString(UrlBase64.Encode(code_verifier_bytes));
        switch (option.ToUpper().Trim())
        {
            case "S256":
                byte[] digest = Hash(new Sha256Digest(), System.Text.Encoding.ASCII.GetBytes(code_verifier));
                return $"{code_verifier},{Jose.Base64Url.Encode(digest)}"; //Retorno el code_verifier y el code_challenge
            case "PLAIN":
                return $"{code_verifier},{code_verifier}";
            default:
                Console.WriteLine("Unknown PKCE option");
                return "";
        }
    }

    private static byte[] Hash(IDigest digest, byte[] inputBytes)
    {
        byte[] retValue = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(inputBytes, 0, inputBytes.Length);
        digest.DoFinal(retValue, 0);
        return retValue;
    }
    private static byte[] GetRandomBytes(int len)
    {
        byte[] data = new byte[len];

		var arraySpan = new Span<byte>(data);
		System.Security.Cryptography.RandomNumberGenerator.Fill(arraySpan);

        return data;
    }
}

