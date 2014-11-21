using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish
{
    /// <summary>
    /// An engine object that will drive all encryption activities. This is the public
    /// API for the assembly.
    /// </summary>
    public class BlowfishEngine
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BlowfishEngine()
        {
            byte[] key = new byte[] { 0xfe, 0xdc, 0xba, 0x98, 0x76, 0x54, 0x32, 0x10 };
            byte[] plain = new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef };

            key = ConvertHexStringToByteArray("0000000000000000");
            plain = ConvertHexStringToByteArray("0000000000000000");

            Console.WriteLine("Key: \n" + BitConverter.ToString(key));
            Console.WriteLine("Plaintext: \n" + BitConverter.ToString(plain));

            var Schedule = new KeySchedule(key);

            UInt64 bitScape = 0x0000000000000000;

            for (int i = 0; i < 9; i++)
            {
                //Console.WriteLine("Replacing keys " + (i + 1) + " and " + (i + 2) + ".");
                bitScape = EncryptBlock(bitScape, Schedule);
                //Console.WriteLine("Bitscape is " + UInt64ToByteString(bitScape) + ".");
                //Console.WriteLine("New keys are " + UInt32ToByteString(Left(bitScape)) + " and " + UInt32ToByteString(Right(bitScape)) + ".");
                Schedule.Set(i * 2, Left(bitScape));
                Schedule.Set(i * 2 + 1, Right(bitScape));
            }

            for (int i = 0; i < 1024;)
            {
                bitScape = EncryptBlock(bitScape, Schedule);

                BlowfishConstants.sbox[i / 256, i % 256] = Left(bitScape);
                i++;
                BlowfishConstants.sbox[i / 256, i % 256] = Right(bitScape);
                i++;
            }

            UInt64[] plain64 = PackBytesIntoUInt64(plain);

            UInt64 Final = EncryptBlock(plain64[0], Schedule);
            Console.WriteLine("Final ciphertext: " + UInt64ToByteString(Final));

            //UInt64 Decrypted = DecryptBlock(Final, Schedule);
            //Console.WriteLine("Result of decryption: " + UInt64ToByteString(Decrypted));
        }

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

        UInt64 EncryptBlock(UInt64 Data, KeySchedule Schedule)
        {
            for (int i = 0; i < 16; i++)
            {
                Data = Round(Data, Schedule.Get(i));
                //Console.WriteLine("After round " + (i + 1) + ": \n" + UInt64ToByteString(Data));
            }

            // Undo the last swap.
            UInt32 leftHalf = Right(Data) ^ Schedule.Get(16);
            UInt32 rightHalf = Left(Data) ^ Schedule.Get(17);

            return Combine(leftHalf, rightHalf);
        }

        UInt64 DecryptBlock(UInt64 Data, KeySchedule Schedule)
        {
            for (int i = 17; i > 1; i--)
            {
                Data = Round(Data, Schedule.Get(i));
                //Console.WriteLine("After round " + (i + 1) + ": \n" + UInt64ToByteString(Data));
            }

            // Undo the last swap.
            UInt32 leftHalf = Right(Data) ^ Schedule.Get(1);
            UInt32 rightHalf = Left(Data) ^ Schedule.Get(0);

            return Combine(leftHalf, rightHalf);
        }

        public static T[] SubArray<T>(T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Packs an array of bytes into an array of 64-bit integers.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        UInt64[] PackBytesIntoUInt64(Byte[] Data)
        {
            UInt64[] newData = new UInt64[Data.Length / 8];

            for (int i = 0; i < newData.Length; i++)
            {
                newData[i] = 0;
                for (int j = 0; j < 8; j++)
                {
                    newData[i] |= ((UInt64)Data[i * 8 + j] << ((7 - j) * 8));
                    //Console.WriteLine("newData[" + i + "]: " + UInt64ToByteString(newData[i]));
                }
            }

            return newData;
        }

        String UInt64ToByteString(UInt64 value)
        {
            return String.Format("0x{0:X16}", value);
        }

        String UInt32ToByteString(UInt32 value)
        {
            return String.Format("0x{0:X8}", value);
        }

        UInt64 Combine(UInt32 left, UInt32 right)
        {
            return (UInt64)left << 32 | right;
        }

        UInt32 Left(UInt64 value)
        {
            return (UInt32)(0xFFFFFFFF & (value >> 32));
        }

        UInt32 Right(UInt64 value)
        {
            return (UInt32)(0xFFFFFFFF & value);
        }

        UInt64 Swap(UInt64 value)
        {
            return Combine(Right(value), Left(value));
        }

        UInt32 F(UInt32 value)
        {
            uint a = (0xFF000000 & value) >> 24;
            uint b = (0x00FF0000 & value) >> 16;
            uint c = (0x0000FF00 & value) >> 8;
            uint d = 0x000000FF & value;

            //Console.WriteLine("S1," + a + " = " + UInt32ToByteString(BlowfishConstants.sbox[0, a]));
            //Console.WriteLine("S2," + b + " = " + UInt32ToByteString(BlowfishConstants.sbox[1, b]));
            //Console.WriteLine("S3," + c + " = " + UInt32ToByteString(BlowfishConstants.sbox[2, c]));
            //Console.WriteLine("S4," + d + " = " + UInt32ToByteString(BlowfishConstants.sbox[3, d]));

            UInt32 sum1 = BlowfishConstants.sbox[0, a] + BlowfishConstants.sbox[1, b];
            UInt32 sum2 = sum1 ^ BlowfishConstants.sbox[2, c];
            UInt32 sum3 = sum2 + BlowfishConstants.sbox[3, d];

            return sum3;
        }

        /// <summary>
        /// The round 
        /// </summary>
        /// <param name="data"></param>
        UInt64 Round(UInt64 data, UInt32 RoundKey)
        {
            String Sep = "=============================";
            //Console.WriteLine(Sep + "\nBeginning round on input: " + UInt64ToByteString(data));
            //Console.WriteLine("Key: " + UInt32ToByteString(RoundKey));
            UInt32 leftHalf = Left(data);
            UInt32 rightHalf = Right(data);

            //Console.WriteLine("xL = " + UInt32ToByteString(leftHalf));
            //Console.WriteLine("xR = " + UInt32ToByteString(rightHalf));

            leftHalf ^= RoundKey;

            //Console.WriteLine("xL = xL XOR Pi = " + UInt32ToByteString(leftHalf));
            //Console.WriteLine("F(xL) = " + UInt32ToByteString(F(leftHalf)));

            rightHalf ^= F(leftHalf);

            //Console.WriteLine("xR = F(xL) ^ xR = " + UInt32ToByteString(rightHalf));

            UInt64 output = Combine(rightHalf, leftHalf);

            //Console.WriteLine("OUTPUT: " + UInt64ToByteString(output) + ".\n" + Sep);

            return output;
        }
    }
}
