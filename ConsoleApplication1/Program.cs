using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using AustinXYZ;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var AAA = new AesManaged();
            var aesenc = AAA.CreateEncryptor();
            var aesdec = AAA.CreateDecryptor();
            byte[] plaintext = new byte[16];
            byte[] encrypted;
            byte[] decrypted = new byte[16];

            for (int i = 0; i < plaintext.Length; i++) plaintext[i] = 10;
            Console.WriteLine("Plaintext bytes: {0}.", BitConverter.ToString(plaintext));

            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();
            blowfish.GenerateKey();
            blowfish.GenerateIV();
            blowfish.Mode = CipherMode.CBC;

            Console.WriteLine("Key: {0}.", BitConverter.ToString(blowfish.Key));
            Console.WriteLine("IV: {0}.", BitConverter.ToString(blowfish.IV));
            Console.WriteLine("Cipher Mode: {0}.", blowfish.Mode);
            Console.WriteLine("");

            // Stream all plaintext bytes through the blowfish encryptor.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, blowfish.CreateEncryptor(), CryptoStreamMode.Write))
                    cryptoStream.Write(plaintext, 0, plaintext.Length);
                encrypted = memoryStream.GetBuffer();
            }

            // Make sure that CBC is properly subtituting. If CBC used the IV correctly, the second block should
            // be much different from the first, even though the source input block was the same for both.
            int BlockSize = blowfish.BlockSize / 8;
            byte[] FirstBlock = new byte[BlockSize];
            byte[] SecondBlock = new byte[BlockSize];
            Array.Copy(encrypted, 0, FirstBlock, 0, BlockSize);
            Array.Copy(encrypted, BlockSize, SecondBlock, 0, BlockSize);

            if (FirstBlock.SequenceEqual<byte>(SecondBlock))
                Console.WriteLine("First and second block are EQUAL.");
            else
                Console.WriteLine("First and second blocks are NOT EQUAL - CBC worked.");

            // Stream all ciphertext bytes through the blowfish decryptor.
            using (MemoryStream memoryStream = new MemoryStream(encrypted))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, blowfish.CreateDecryptor(), CryptoStreamMode.Read))
            {
                int bytesRead = cryptoStream.Read(decrypted, 0, decrypted.Length);
            }

            Console.WriteLine("\nDecrypted bytes: {0}.", BitConverter.ToString(decrypted));

            Console.ReadLine();
        }
    }
}
