using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

using Blowfish.Exceptions;
using Blowfish.KeyGeneration;

namespace Blowfish
{
    /// <summary>
    /// An engine object that will drive all encryption activities. This is the public
    /// API for the assembly.
    /// </summary>
    public class Blowfish : SymmetricAlgorithm
    {
        /// <summary>
        /// The context of precomputed s-boxes and schedule for each key used. The engine
        /// should support multiple keys, but it is not designed for the key to be changed
        /// often. The context will need to be re-assigned when the key changes.
        /// </summary>
        BlowfishContext Context;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Blowfish()
        {
            //
        }

        /// <summary>
        /// Constructor via byte array for key.
        /// </summary>
        /// <param name="key"></param>
        public Blowfish(byte[] key)
        {
            var Key = new BlowfishKey(key);
            Context = new BlowfishContext(Key);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Blowfish(BlowfishKey key)
        {
            Context = new BlowfishContext(key);
        }

        /// <summary>
        /// Encrypt block of text.
        /// </summary>
        /// <param name="Plaintext"></param>
        public UInt64 Encrypt(byte[] Plaintext)
        {
            if (Context == null)
            {
                throw new MissingKeyException("No key was set for the engine. Set a key before encrypting.");
            }

            UInt64[] plain64 = ByteOperations.PackBytesIntoUInt64(Plaintext);

            return Encrypt(plain64[0]);
        }

        /// <summary>
        /// Encrypt a single 64-bit piece of data with no chaining.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public UInt64 Encrypt(UInt64 data)
        {
            return BlowfishEngine.EncryptBlock(ByteOperations.Swap(data), Context);
        }

        /// <summary>
        /// Assigns the given key to the engine and computes the schedule.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(BlowfishKey key)
        {
            Context = new BlowfishContext(key);
        }

        /// <summary>
        /// Assigns the given key to the engine and computes the schedule.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(byte[] key)
        {
            BlowfishKey Key = new BlowfishKey(key);
            Context = new BlowfishContext(Key);
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            throw new NotImplementedException();
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            throw new NotImplementedException();
        }

        public override void GenerateIV()
        {
            throw new NotImplementedException();
        }

        public override void GenerateKey()
        {
            throw new NotImplementedException();
        }
    }
}
