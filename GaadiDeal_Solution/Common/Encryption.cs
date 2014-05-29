using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;

namespace Common
{

    public class Encryption
    {
        class StandardParameter
        {
            //Password:  This is this password for encryption, like a key. Eg: “Welcome123”
            public static string PrivatePassword = "Syc9hNjfgS4Ux45sGFFK";
            //Salt:string, just like a second password.
            public static string PrivateSalt = "sk4a0ij966s4x5c";
            //HashAlgorithm: string, can be SHA1 or MD5.
            public static string PrivateHashAlgorithm = "SHA1";
            //PasswordIterations: int is the number of times the algorithm is run on the text.
            public static int PrivatePasswordIterations = 2;
            //InitialVector: should be a string of 16 ASCII characters. Eg: “*1B2c3D4e5F6g7H8”
            public static string PrivateInitialVector = "@1B2c3D4e5F6g7H8";
            //KeySize: int,can be 128, 192, or 256.
            public static int PrivateKeySize = 128;
        }




        public static string Decrypt16(string Text)
        {
            return Decrypt(HexadecimalToString(Text), false);
        }
        public static string Encrypt16(string Text)
        {
            return StringToHexadecimal(Encrypt(Text, false));
        }
        public static string Decrypt16(string EncryptKey, string Text)
        {
            return Decrypt(EncryptKey, HexadecimalToString(Text), false);
        }
        public static string Encrypt16(string EncryptKey, string Text)
        {
            return StringToHexadecimal(Encrypt(EncryptKey, Text, false));
        }
        static string Encrypt(string Text, bool UrlEncode = true)
        {
            String EncryptText = Encrypt(Text, StandardParameter.PrivatePassword, StandardParameter.PrivateSalt, StandardParameter.PrivateHashAlgorithm, StandardParameter.PrivatePasswordIterations, StandardParameter.PrivateInitialVector, StandardParameter.PrivateKeySize);
            if (UrlEncode) EncryptText = HttpUtility.UrlEncode(EncryptText);
            return EncryptText;
        }
        static string Decrypt(string Text, bool UrlDecode = true)
        {
            if (UrlDecode) Text = HttpUtility.UrlDecode(Text);
            String DecryptText = Decrypt(Text, StandardParameter.PrivatePassword, StandardParameter.PrivateSalt, StandardParameter.PrivateHashAlgorithm, StandardParameter.PrivatePasswordIterations, StandardParameter.PrivateInitialVector, StandardParameter.PrivateKeySize);
            return DecryptText;
        }
        static string Encrypt(string EncryptKey, string Text, bool UrlEncode = true)
        {
            String EncryptText = Encrypt(Text, EncryptKey, StandardParameter.PrivateSalt, StandardParameter.PrivateHashAlgorithm, StandardParameter.PrivatePasswordIterations, StandardParameter.PrivateInitialVector, StandardParameter.PrivateKeySize);
            if (UrlEncode) EncryptText = HttpUtility.UrlEncode(EncryptText);
            return EncryptText;
        }
        static string Decrypt(string EncryptKey, string Text, bool UrlDecode = true)
        {
            if (UrlDecode) Text = HttpUtility.UrlDecode(Text);
            String DecryptText = Decrypt(Text, EncryptKey, StandardParameter.PrivateSalt, StandardParameter.PrivateHashAlgorithm, StandardParameter.PrivatePasswordIterations, StandardParameter.PrivateInitialVector, StandardParameter.PrivateKeySize);
            return DecryptText;
        }


        public static string Encrypt(string Text, String Password, string Salt, string HashAlgorithm, int PasswordIterations, string InitialVector, int KeySize)
        {
            if (string.IsNullOrEmpty(Text))
                return "";
            String EncryptText = "";
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] PlainTextBytes = Encoding.UTF8.GetBytes(Text);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            byte[] CipherTextBytes = null;
            using (ICryptoTransform Encryptor = SymmetricKey.CreateEncryptor(KeyBytes, InitialVectorBytes))
            {
                using (MemoryStream MemStream = new MemoryStream())
                {
                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Encryptor, CryptoStreamMode.Write))
                    {
                        CryptoStream.Write(PlainTextBytes, 0, PlainTextBytes.Length);
                        CryptoStream.FlushFinalBlock();
                        CipherTextBytes = MemStream.ToArray();
                        MemStream.Close();
                        CryptoStream.Close();
                    }
                }
            }
            SymmetricKey.Clear();
            EncryptText = Convert.ToBase64String(CipherTextBytes);
            return EncryptText;
        }
        public static string Decrypt(string Text, String Password, string Salt, string HashAlgorithm, int PasswordIterations, string InitialVector, int KeySize)
        {
            if (string.IsNullOrEmpty(Text))
                return "";
            String DecryptText = "";
            byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(InitialVector);
            byte[] SaltValueBytes = Encoding.ASCII.GetBytes(Salt);
            byte[] CipherTextBytes = Convert.FromBase64String(Text);
            PasswordDeriveBytes DerivedPassword = new PasswordDeriveBytes(Password, SaltValueBytes, HashAlgorithm, PasswordIterations);
            byte[] KeyBytes = DerivedPassword.GetBytes(KeySize / 8);
            RijndaelManaged SymmetricKey = new RijndaelManaged();
            SymmetricKey.Mode = CipherMode.CBC;
            byte[] PlainTextBytes = new byte[CipherTextBytes.Length];
            int ByteCount = 0;
            using (ICryptoTransform Decryptor = SymmetricKey.CreateDecryptor(KeyBytes, InitialVectorBytes))
            {
                using (MemoryStream MemStream = new MemoryStream(CipherTextBytes))
                {
                    using (CryptoStream CryptoStream = new CryptoStream(MemStream, Decryptor, CryptoStreamMode.Read))
                    {

                        ByteCount = CryptoStream.Read(PlainTextBytes, 0, PlainTextBytes.Length);
                        MemStream.Close();
                        CryptoStream.Close();
                    }
                }
            }
            SymmetricKey.Clear();
            DecryptText = Encoding.UTF8.GetString(PlainTextBytes, 0, ByteCount);

            return DecryptText;
        }


        static string StringToHexadecimal(string str)
        {
            string strTemp = "";
            if (str.Trim() == "")
                return "";
            byte[] bTemp = System.Text.Encoding.Default.GetBytes(str);

            foreach (byte bt in bTemp)
            {
                strTemp += bt.ToString("X");
            }
            return strTemp;
        }
        static string HexadecimalToString(string hex)
        {
            if (hex.Trim() == "")
                return "";

            string str = "";
            string strTemp = "";
            byte[] b = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length / 2; i++)
            {
                strTemp = hex.Substring(i * 2, 2);
                b[i] = Convert.ToByte(strTemp, 16);
            }
            str = System.Text.Encoding.Default.GetString(b);

            return str;
        }

    }
}
