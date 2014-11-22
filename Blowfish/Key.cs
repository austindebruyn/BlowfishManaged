using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish
{
    public class BlowfishKey
    {
        /// <summary>
        /// An array of bytes that make up the key.
        /// </summary>
        public byte[] keyBytes;

        /// <summary>
        /// Read-only property for key length.
        /// </summary>
        public int Length
        {
            get
            {
                return keyBytes.Length;
            }
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public BlowfishKey(byte[] Data)
        {
            ValidateLength(Data.Length);

            keyBytes = new byte[Data.Length];
            Array.Copy(Data, keyBytes, Length);
        }

        /// <summary>
        /// Constructor via hex string.
        /// </summary>
        /// <param name="hexString"></param>
        public BlowfishKey(String hexString)
        {
            ValidateLength(hexString.Length / 2);
            keyBytes = ByteOperations.ConvertHexStringToByteArray(hexString);
        }

        /// <summary>
        /// Throw exception if the provided key length is not valid for Blowfish. Valid
        /// keys must be between 32 and 448 bits.
        /// </summary>
        /// <param name="length">Size in bytes</param>
        private void ValidateLength(int length)
        {
            if (length < 4) throw new ArgumentException("Keys can be no smaller than 32 bits in length.");
            if (length > 56) throw new ArgumentException("Keys can be no greater than 448 bits in length.");
            if (length % 4 != 0) throw new ArgumentException("Keys must be of a length divisible by 32 bits.");
        }
    }
}
