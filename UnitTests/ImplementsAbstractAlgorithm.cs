using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;

namespace UnitTests
{
    [TestClass]
    public class ImplementsAbstractAlgorithm
    {
        [TestMethod]
        public void it_validates_key_sizes()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            for (int i = 32; i <= 448; i++)
            {
                Assert.IsTrue(blowfish.ValidKeySize(i), "Key size {0} failed.", i);
            }
        }

        [TestMethod]
        public void it_accepts_well_formed_key()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            blowfish.Key = new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };
        }

        [TestMethod]
        public void it_rejects_well_formed_short_key()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            blowfish.Key = new byte[] { 0xAB };
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void it_rejects_malformed_long_key()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            blowfish.Key = new byte[60];
        }

        [TestMethod]
        public void it_returns_proper_block_sizes()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            foreach (var blockSize in blowfish.LegalBlockSizes) Assert.AreEqual(blockSize.MinSize, 64);
            foreach (var blockSize in blowfish.LegalBlockSizes) Assert.AreEqual(blockSize.MaxSize, 64);
        }

        [TestMethod]
        public void it_accepts_well_formed_iv()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();
            
            byte[] iv = System.Text.Encoding.UTF8.GetBytes("UsAMkD7N");

            blowfish.BlockSize = 64;
            blowfish.IV = iv;
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void it_rejects_malformed_short_iv()
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            blowfish.BlockSize = 64;
            blowfish.IV = new byte[] { 0xAB };
        }
    }
}
