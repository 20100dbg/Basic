using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Basic
{
    static class Crypt
    {

        public static Boolean AESencryptFile(String inputFile, String outputFile, String skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    Byte[] b = new Guid().ToByteArray();
                    var key = new Rfc2898DeriveBytes(skey, b);

                    aes.Mode = CipherMode.CBC;
                    aes.KeySize = Basic.AES_KEY_LENGTH;
                    aes.BlockSize = 128;

                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);


                    using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write))
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    {
                        int data;
                        while ((data = fsIn.ReadByte()) != -1)
                        {
                            cs.WriteByte((byte)data);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        public static Boolean AESdecryptFile(String inputFile, String outputFile, String skey)
        {
            try
            {
                using (RijndaelManaged aes = new RijndaelManaged())
                {
                    Byte[] b = new Guid().ToByteArray();
                    var key = new Rfc2898DeriveBytes(skey, b);

                    aes.Mode = CipherMode.CBC;
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                    using (ICryptoTransform decryptor = aes.CreateDecryptor())
                    using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor, CryptoStreamMode.Read))
                    {
                        int data;
                        while ((data = cs.ReadByte()) != -1)
                        {
                            fsOut.WriteByte((byte)data);
                        }
                    }
                }

            }
            catch
            {
                return false;
            }

            return true;
        }


        public static String RsaEncrypt(String str, String pubKey)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.FromXmlString(pubKey);

            Byte[] bPlain = Encoding.Unicode.GetBytes(str);
            Byte[] bCrypt = csp.Encrypt(bPlain, true);

            return Convert.ToBase64String(bCrypt);
        }


        public static String RsaDecrypt(String str, String privKey)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            csp.FromXmlString(privKey);

            Byte[] bCrypt = Convert.FromBase64String(str);
            Byte[] bPlain = csp.Decrypt(bCrypt, true);

            return Encoding.Unicode.GetString(bPlain);
        }


        public static void GenRsaKeys(out String pubKey, out String privKey)
        {
            RSACryptoServiceProvider c = new RSACryptoServiceProvider();
            pubKey = c.ToXmlString(false);
            privKey = c.ToXmlString(true);
            c.Clear();
        }

        /*
        public static Byte[] GenAesKey()
        {
            Byte[] b = new Guid().ToByteArray();

            for (int i = 0; i < Basic.NB_HASH_ROUNDS; i++) b = SHA512(b);

            return b;
        }
        */

        public static String GenAesKey()
        {
            String s = new Guid().ToString();

            for (int i = 0; i < Basic.NB_HASH_ROUNDS; i++) s = SHA512(s);

            return s;
        }


        public static String SHA512(String str)
        {
            StringBuilder sb = new StringBuilder();
            Byte[] b = ASCIIEncoding.ASCII.GetBytes(str);
            Byte[] h = new Byte[64];

            using (SHA512Managed sha = new SHA512Managed())
                h = sha.ComputeHash(b);

            foreach (Byte byt in h) sb.Append(byt.ToString("X2"));
            return sb.ToString();
        }


        public static Byte[] SHA512(Byte[] b)
        {
            Byte[] h = new Byte[64];

            using (SHA512Managed sha = new SHA512Managed())
                h = sha.ComputeHash(b);

            return h;
        }
    }
}
