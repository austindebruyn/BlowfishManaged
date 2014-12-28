using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices; 
using System.Text;
using System.Threading.Tasks;

namespace BlowfishManaged
{
    internal class ByteOperations
    {
        /// <summary>
        /// Convert a hex string into an array of bytes.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        internal static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
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
        internal static UInt64 PackBytesIntoUInt64(Byte[] Data, int offset = 0)
        {
            UInt64 newData = 0;

            for (int j = 0; j < 8; j++)
            {
                newData |= ((UInt64)Data[offset + j] << ((7 - j) * 8));
            }

            return newData;
        }

        /// <summary>
        /// Unpacks a 64-bit integer into an array of 8 bytes.
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        internal static byte[] UnpackUInt64IntoBytes(UInt64 Data)
        {
            byte[] Bytes = new byte[8];

            for (int i = 0; i < 8; i++)
            {
                Bytes[i] = (byte)(0xFF & (Data >> ((7 - i) * 8)));
            }

            return Bytes;
        }

        /// <summary>
        /// Combines two 32-bit integers into a 64-bit result.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static UInt64 Combine(UInt32 left, UInt32 right)
        {
            return (UInt64)left << 32 | right;
        }

        /// <summary>
        /// Returns the leftmost 32-bits of a 64-bit integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static UInt32 Left(UInt64 value)
        {
            return (UInt32)(0xFFFFFFFF & (value >> 32));
        }

        /// <summary>
        /// Returns the rightmost 32-bits of a 64-bit integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static UInt32 Right(UInt64 value)
        {
            return (UInt32)(0xFFFFFFFF & value);
        }

        /// <summary>
        /// Swaps the leftmost 32-bits and rightmost in a 64-bit integer and
        /// returns the modified result.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static UInt64 Swap(UInt64 value)
        {
            return Combine(Right(value), Left(value));
        }

        /// <summary>
        /// Formats the 64-bit integer as a hex string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal String UInt64ToByteString(UInt64 value)
        {
            return String.Format("0x{0:X16}", value);
        }

        /// <summary>
        /// Formats the 32-bit integer as a hex string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal String UInt32ToByteString(UInt32 value)
        {
            return String.Format("0x{0:X8}", value);
        }

        /// <summary>
        /// Returns whether or not the two byte arrays are identical.
        /// </summary>
        /// <param name="rgbKey"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        internal static bool ArraysEqual(byte[] array1, byte[] array2)
        {
            if (array1 == null || array2 == null) throw new ArgumentNullException();

            if (array1.Length != array2.Length) return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i]) return false;
            }

            return true;
        }
    }
}
