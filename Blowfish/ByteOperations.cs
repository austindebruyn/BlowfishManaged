using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustinXYZ
{
    internal class ByteOperations
    {
        /// <summary>
        /// Convert a hex string into an array of bytes.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(System.Globalization.CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        /// <summary>
        /// Packs an array of bytes into an array of 64-bit integers.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static UInt64 PackBytesIntoUInt64(Byte[] Data, int offset = 0)
        {
            UInt64 newData = 0;

            for (int j = 0; j < 8; j++)
            {
                newData |= ((UInt64)Data[offset + j] << ((7 - j) * 8));
            }

            return newData;
        }

        /// <summary>
        /// Combines two 32-bit integers into a 64-bit result.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static UInt64 Combine(UInt32 left, UInt32 right)
        {
            return (UInt64)left << 32 | right;
        }

        /// <summary>
        /// Returns the leftmost 32-bits of a 64-bit integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt32 Left(UInt64 value)
        {
            return (UInt32)(0xFFFFFFFF & (value >> 32));
        }

        /// <summary>
        /// Returns the rightmost 32-bits of a 64-bit integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt32 Right(UInt64 value)
        {
            return (UInt32)(0xFFFFFFFF & value);
        }

        /// <summary>
        /// Swaps the leftmost 32-bits and rightmost in a 64-bit integer and
        /// returns the modified result.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UInt64 Swap(UInt64 value)
        {
            return Combine(Right(value), Left(value));
        }

        /// <summary>
        /// Formats the 64-bit integer as a hex string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public String UInt64ToByteString(UInt64 value)
        {
            return String.Format("0x{0:X16}", value);
        }

        /// <summary>
        /// Formats the 32-bit integer as a hex string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public String UInt32ToByteString(UInt32 value)
        {
            return String.Format("0x{0:X8}", value);
        }
    }
}
