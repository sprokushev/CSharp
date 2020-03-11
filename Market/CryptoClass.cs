using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PSVClassLibrary
{
    
    // шифрация/дешифрация строк
    public static class CryptoClass
    {
        // Create byte array for additional entropy when using Protect method.
        static byte[] s_aditionalEntropy = { 9, 2, 3, 7, 5 };


        public static byte[] encrypt_function(string secret)
        {
            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            //Just grabbing the bytes since most crypto functions need bytes.
            byte[] bytes = Byte_Transform.GetBytes(secret);

            //Encrypt the data.
            return ProtectedData.Protect(bytes, s_aditionalEntropy, DataProtectionScope.CurrentUser);
        }


        public static string decrypt_function(byte[] crypted_bytes)
        {
            byte[] bytes = ProtectedData.Unprotect(crypted_bytes, s_aditionalEntropy, DataProtectionScope.CurrentUser);

            System.Text.UTF8Encoding Byte_Transform = new System.Text.UTF8Encoding();

            return Byte_Transform.GetString(bytes);
        }

    }

}
