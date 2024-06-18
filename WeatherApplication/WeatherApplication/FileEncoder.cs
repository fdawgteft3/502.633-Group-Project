using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeatherApplication
{
    //Implements the Singleton Pattern - the rationale behind this
    //is that FileEncoder only needs one instance as it accesses
    //one file(security.sys) and Singleton prevents multiple modifications
    //to the file at the same time which could cause the file to corrupt
    public class FileEncoder
    {
        private static readonly Lazy<FileEncoder> lazyInstance = new Lazy<FileEncoder>(() => new FileEncoder("security.sys"));
        private readonly string filePath;
        private readonly byte[] key = Convert.FromBase64String("C/+YjsuTzXJzop3TX46d2WATe1qZ/PiNT/mCRxrSw1o=");

        private FileEncoder(string filePath)
        {
            this.filePath = filePath;
        }

        public static FileEncoder Instance
        {
            get
            {
                return lazyInstance.Value;
            }
        }

        public void Write(string key, string value)
        {
            // Encrypt the key-value pair
            string encryptedPair = EncryptString($"{key}={value}", this.key);

            //Check if key already exists
            if (Read(key) == null)
            {
                // Write to file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(encryptedPair);

                }
            }
        }

        public string Read(string key)
        {
            // Read lines from the file
            string[] lines = File.ReadAllLines(filePath);

            // Find the line corresponding to the key
            foreach (string line in lines)
            {
                string decryptedLine = DecryptString(line, this.key);
                string[] parts = decryptedLine.Split('=');
                if (parts.Length == 2 && parts[0] == key)
                {
                    // Return the decrypted value
                    return parts[1];
                }
            }

            // Key not found
            return null;
        }
        private string EncryptString(string input, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.GenerateIV();

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    //store IV
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(input);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        private string DecryptString(string input, byte[] key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(input)))
                {
                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    msDecrypt.Read(iv, 0, iv.Length); //get IV
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
