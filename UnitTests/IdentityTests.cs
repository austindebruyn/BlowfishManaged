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
        public void TestZeroStringStream()
        {
            byte[] zeroString = new byte[8];
            byte[] plaintext = new byte[8];
            byte[] encrypted = new byte[8];
            byte[] decrypted = new byte[8];
            Array.Copy(zeroString, plaintext, plaintext.Length);

            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();
            blowfish.Key = new byte[] {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 }; //System.Text.Encoding.UTF8.GetBytes("abcdefghijklmnop");
            blowfish.GenerateIV();
            
            ICryptoTransform encryptor = blowfish.CreateEncryptor();
            encryptor.TransformBlock(plaintext, 0, 8, encrypted, 0);

            ICryptoTransform decryptor = blowfish.CreateDecryptor();
            decryptor.TransformBlock(encrypted, 0, 8, decrypted, 0);

            CollectionAssert.AreEqual(plaintext, decrypted);
        }
    }
}
