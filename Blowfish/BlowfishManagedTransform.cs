using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using BlowfishManaged.KeyGeneration;

namespace BlowfishManaged
{
    /// <summary>
    /// An enumeratd
    /// </summary>
    internal enum BlowfishManagedTransformMode { Encrypt, Decrypt }; 

    /// <summary>
    /// Performs a cryptographic transformation of data using the Blowfish algorithm. This class cannot be inherited.
    /// </summary>
    public sealed class BlowfishManagedTransform : ICryptoTransform, IDisposable
    {
        /// <summary>
        /// The context of precomputed s-boxes and schedule for each key used. The engine
        /// should support multiple keys, but it is not designed for the key to be changed
        /// often. The context will need to be re-assigned when the key changes.
        /// </summary>
        BlowfishContext Context;

        /// <summary>
        /// Gets whether or not the transform is an encryption or a decryption.
        /// </summary>
        BlowfishManagedTransformMode Mode;

        /// <summary>
        /// Gets or sets the block cipher mode to use for encryption.
        /// </summary>
        CipherMode CipherMode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        internal BlowfishManagedTransform(BlowfishContext Context, CipherMode CipherMode, BlowfishManagedTransformMode Mode)
        {
            this.Context = Context;
            this.Mode = Mode;
            this.CipherMode = CipherMode;

            if (CipherMode != CipherMode.ECB) throw new CryptographicException("Invalid cipher mode.");
        }

        /// <summary>
        /// Gets a value indicating whether the current transform can be reused.
        /// </summary>
        public bool CanReuseTransform
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether multiple blocks can be transformed.
        /// </summary>
        public bool CanTransformMultipleBlocks
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the input block size.
        /// </summary>
        public int InputBlockSize
        {
            get { return 8; }
        }

        /// <summary>
        /// Gets the output block size.
        /// </summary>
        public int OutputBlockSize
        {
            get { return 8; }
        }

        /// <summary>
        /// Transforms the specified region of the input byte array and copies the resulting transform to the specified
        /// region of the output byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the input byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the input byte array to use as data.</param>
        /// <param name="outputBuffer">The output to which to write the transform.</param>
        /// <param name="outputOffset">The offset into the output byte array from which to begin writing data.</param>
        /// <returns>The number of bytes written.</returns>
        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            UInt64 block = ByteOperations.PackBytesIntoUInt64(inputBuffer, inputOffset);

            // Call a function from the blowfish engine function.
            if (Mode == BlowfishManagedTransformMode.Encrypt) block = BlowfishEngine.Encrypt(block, Context);
            if (Mode == BlowfishManagedTransformMode.Decrypt) block = BlowfishEngine.Decrypt(block, Context);

            byte[] TransformedBytes = ByteOperations.UnpackUInt64IntoBytes(block);

            Array.Copy(TransformedBytes, 0, outputBuffer, outputOffset, TransformedBytes.Length);
            return TransformedBytes.Length;
        }

        /// <summary>
        /// Transforms the specified region of the specified byte array.
        /// </summary>
        /// <param name="inputBuffer">The input for which to compute the transform.</param>
        /// <param name="inputOffset">The offset into the byte array from which to begin using data.</param>
        /// <param name="inputCount">The number of bytes in the byte array to use as data.</param>
        /// <returns>The computed transform.</returns>
        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            UInt64 block = ByteOperations.PackBytesIntoUInt64(inputBuffer, inputOffset);

            // Call a function from the blowfish engine function.
            if (Mode == BlowfishManagedTransformMode.Encrypt) block = BlowfishEngine.Encrypt(block, Context);
            if (Mode == BlowfishManagedTransformMode.Decrypt) block = BlowfishEngine.Decrypt(block, Context);

            return ByteOperations.UnpackUInt64IntoBytes(block);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //
        }
    }
}
