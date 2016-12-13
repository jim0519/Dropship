using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DropshipBusiness.Security
{
    public class EncryptionService : IEncryptionService
    {
        public EncryptionService()
        {
            
        }

        /// <summary>
        /// Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public virtual string CreateSaltKey(int size) 
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA1")
        {
            if (String.IsNullOrEmpty(passwordFormat))
                passwordFormat = "SHA1";
            string saltAndPassword = String.Concat(password, saltkey);

            //return FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, passwordFormat);
            var algorithm = HashAlgorithm.Create(passwordFormat);
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        

        #region Utilities

        private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv) 
        {
            using (var ms = new MemoryStream()) {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write)) {
                    byte[] toEncrypt = new UnicodeEncoding().GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }

                return ms.ToArray();
            }
        }

        private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv) 
        {
            using (var ms = new MemoryStream(data)) {
                using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    var sr = new StreamReader(cs, new UnicodeEncoding());
                    return sr.ReadLine();
                }
            }
        }

        #endregion
    }
}
