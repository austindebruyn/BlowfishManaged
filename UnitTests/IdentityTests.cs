using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class IdentityTests
    {
        [TestMethod]
        public void decryption_matches_plaintext()
        {
            byte[] plaintext = new byte[8];
            byte[] encrypted = new byte[8];
            byte[] decrypted = new byte[8];

            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();
            blowfish.Key = System.Text.Encoding.UTF8.GetBytes("abcdefghijklmnop");
            
            ICryptoTransform encryptor = blowfish.CreateEncryptor();
            encryptor.TransformBlock(plaintext, 0, 8, encrypted, 0);

            ICryptoTransform decryptor = blowfish.CreateDecryptor();
            decryptor.TransformBlock(encrypted, 0, 8, decrypted, 0);

            CollectionAssert.AreEqual(plaintext, decrypted);
        }

        [TestMethod]
        public void decryption_matches_plaintext_cbc()
        {
            byte[] plaintext = new byte[512];
            byte[] encrypted = new byte[512];
            byte[] decrypted = new byte[512];

            for (int i = 0; i < 512; i++) plaintext[i] = 10;

            AustinXYZ.BlowfishManaged blowfish = new AustinXYZ.BlowfishManaged();
            blowfish.Key = System.Text.Encoding.UTF8.GetBytes("zzzzxcvmnopoabjk");
            blowfish.GenerateIV();

            // Stream all plaintext bytes through the blowfish encryptor.
            int count = 0;
            UInt64 PreviousBlock = ByteOperations.PackBytesIntoUInt64(blowfish.IV);
            while (count < 512)
            {
                UInt64 Plaintext = ByteOperations.PackBytesIntoUInt64(plaintext, count);
                Plaintext ^= PreviousBlock;

                UInt64 Transformed = blowfish.EncryptSingleBlock(Plaintext);
                PreviousBlock = Transformed;

                byte[] TransformedBytes = ByteOperations.UnpackUInt64IntoBytes(Transformed);
                Array.Copy(TransformedBytes, 0, encrypted, count, 8);

                count += 8;
            }

            // Make sure that CBC is properly subtituting. If CBC used the IV correctly, the second block should
            // be much different from the first, even though the source input block was the same for both.
            int BlockSize = blowfish.BlockSize / 8;
            byte[] FirstBlock = new byte[BlockSize];
            byte[] SecondBlock = new byte[BlockSize];
            Array.Copy(encrypted, 0, FirstBlock, 0, BlockSize);
            Array.Copy(encrypted, BlockSize, SecondBlock, 0, BlockSize);
            CollectionAssert.AreNotEqual(FirstBlock, SecondBlock);


            // Steam all encrypted bytes through the blowfish decryptor.
            count = 512;
            byte[] PreviousBlockBytes = new byte[8];
            while (count > 0)
            {
                count -= 8;

                UInt64 Ciphertext = ByteOperations.PackBytesIntoUInt64(encrypted, count);
                UInt64 Transformed = blowfish.DecryptSingleBlock(Ciphertext);

                if (count < 8)
                    PreviousBlockBytes = blowfish.IV;
                else
                    Array.Copy(encrypted, count - 8, PreviousBlockBytes, 0, 8);

                PreviousBlock = ByteOperations.PackBytesIntoUInt64(PreviousBlockBytes);

                Transformed ^= PreviousBlock;

                byte[] TransformedBytes = ByteOperations.UnpackUInt64IntoBytes(Transformed);
                Array.Copy(TransformedBytes, 0, decrypted, count, 8);
            }

            CollectionAssert.AreEqual(plaintext, decrypted);
        }
    }
}
