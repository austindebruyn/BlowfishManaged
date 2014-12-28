using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

using BlowfishManaged.KeyGeneration;

namespace BlowfishManaged
{
    /// <summary>
    /// An engine object that will drive all encryption activities. This is the public
    /// API for the assembly.
    /// </summary>
    public class BlowfishManaged : SymmetricAlgorithm
    {
        /// <summary>
        /// The context of precomputed s-boxes and schedule for each key used. The engine
        /// should support multiple keys, but it is not designed for the key to be changed
        /// often. The context will need to be re-assigned when the key changes.
        /// </summary>
        BlowfishContext Context;

        /// <summary>
        /// Block size.
        /// </summary>
        public override int BlockSize
        {
            get
            {
                return BlockSizeValue;
            }
            set
            {
                if (value != 64) throw new CryptographicException("Block size must be 64 bits.");

                BlockSizeValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode for operation of the symmetric algorithm.
        /// </summary>
        public override CipherMode Mode
        {
            get
            {
                return base.Mode;
            }
            set
            {
                if (value != CipherMode.ECB) throw new CryptographicException("Invalid cipher mode.");
                base.Mode = value;
            }
        }

        /// <summary>
        /// Gets or sets the secret key used for the symmetric algorithm.
        /// </summary>
        public new byte[] Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
                Context = null;
            }
        }

        /// <summary>
        /// Gets or sets the initialization vector used for the symmetric algorithm.
        /// </summary>
        public override byte[] IV
        {
            get
            {
                return base.IVValue;
            }
            set
            {
                if (value.Length != BlockSize / 8) throw new CryptographicException("IV must be equal to the block size!");
                base.IVValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the size, in bits, of the secret key used by the symmetric algorithm.
        /// </summary>
        public override int KeySize
        {
            get
            {
                if (Key == null) return 0;
                return Key.Length;
            }
        }

        /// <summary>
        /// Gets the key sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalKeySizes
        {
            get
            {
                return new KeySizes[] { new KeySizes(8, 448, 1) };
            }
        }

        /// <summary>
        /// Gets the block sizes, in bits, that are supported by the symmetric algorithm.
        /// </summary>
        public override KeySizes[] LegalBlockSizes
        {
            get
            {
                return new KeySizes[] { new KeySizes(64, 64, 0) };
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlowfishManaged() : base()
        {
            BlockSize = 64;
            Mode = CipherMode.ECB;
        }

        /// <summary>
        /// Constructor via byte array for key.
        /// </summary>
        /// <param name="key"></param>
        public BlowfishManaged(byte[] key) : this()
        {
            Key = key;
            Context = new BlowfishContext(Key);
        }

        /// <summary>
        /// Creates a symmetric encryptor object with the specified Key property and initialization vector (IV).
        /// </summary>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        /// <returns></returns>
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            BlowfishContext encryptionContext = null;

            if (rgbKey.Equals(Key))
            {
                if (Context == null) Context = new BlowfishContext(Key);
                encryptionContext = Context;
            }
            else
            {
                encryptionContext = new BlowfishContext(rgbKey);
            }

            return new BlowfishManagedTransform(encryptionContext, Mode, BlowfishManagedTransformMode.Encrypt);
        }

        /// <summary>
        /// Creates a symmetric decryptor object with the specified Key property and initialization vector (IV).
        /// </summary>
        /// <param name="rgbKey"></param>
        /// <param name="rgbIV"></param>
        /// <returns></returns>
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            BlowfishContext encryptionContext = null;

            if (rgbKey.Equals(Key))
            {
                if (Context == null) Context = new BlowfishContext(Key);
                encryptionContext = Context;
            }
            else
            {
                encryptionContext = new BlowfishContext(rgbKey);
            }

            return new BlowfishManagedTransform(encryptionContext, Mode, BlowfishManagedTransformMode.Decrypt);
        }

        /// <summary>
        /// Encrypts a single block, running the 64 bits of data through all 16
        /// rounds of the Blowfish algorithm. Returns the encrypted 64 bits.
        /// </summary>
        /// <param name="block64"></param>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public UInt64 EncryptSingleBlock(UInt64 block64)
        {
            if (Context == null) Context = new BlowfishContext(Key);
            return BlowfishEngine.Encrypt(block64, Context);
        }

        /// <summary>
        /// Decrypts a single block, running the 64 bits of data through all 16
        /// rounds of the Blowfish algorithm. Returns the decrypted 64 bits.
        /// </summary>
        /// <param name="block64"></param>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public UInt64 DecryptSingleBlock(UInt64 block64)
        {
            if (Context == null) Context = new BlowfishContext(Key);
            return BlowfishEngine.Decrypt(block64, Context);
        }

        /// <summary>
        /// When overridden in a derived class, generates a random initialization vector (IV) to use for the algorithm.
        /// </summary>
        public override void GenerateIV()
        {
            IV = new byte[BlockSize / 8];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) rng.GetBytes(IV);
        }

        /// <summary>
        /// When overridden in a derived class, generates a random key (Key) to use for the algorithm.
        /// </summary>
        public override void GenerateKey()
        {
            int keySize = new Random().Next(1, 56);
            Key = new byte[keySize];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) rng.GetBytes(Key);
        }
    }
}
