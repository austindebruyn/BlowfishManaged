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
        /// The key schedule the engine is currently using. This is expensive to compute,
        /// so try to re-use where possible.
        /// </summary>
        KeySchedule Schedule;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BlowfishEngine()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlowfishEngine(BlowfishKey newKey)
        {
            SetKey(newKey);
        }

        public UInt64 Encrypt(String Plaintext)
        {
            return Encrypt(ByteOperations.ConvertHexStringToByteArray(Plaintext));
        }

        /// <summary>
        /// Encrypt block of text.
        /// </summary>
        /// <param name="Plaintext"></param>
        public UInt64 Encrypt(byte[] Plaintext)
        {
            UInt64[] plain64 = PackBytesIntoUInt64(Plaintext);
            UInt64 Final = EncryptBlock(Swap(plain64[0]), Schedule);

            return Final;
        }

        /// <summary>
        /// Assigns the given key to the engine and computes the schedule.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(BlowfishKey key)
        {
            Schedule = new KeySchedule(key);
            UInt64 bitScape = 0x0000000000000000;

            for (int i = 0; i < 9; i++)
            {
                bitScape = EncryptBlock(bitScape, Schedule);
                Schedule.Set(i * 2, Left(bitScape));
                Schedule.Set(i * 2 + 1, Right(bitScape));
                bitScape = Swap(bitScape);
            }

            for (int i = 0; i < 1024; )
            {
                bitScape = EncryptBlock(bitScape, Schedule);

                BlowfishConstants.sbox[i / 256, i % 256] = Left(bitScape);
                i++;
                BlowfishConstants.sbox[i / 256, i % 256] = Right(bitScape);
                i++;

                bitScape = Swap(bitScape);
            }
        }

        UInt64 EncryptBlock(UInt64 Data, KeySchedule Schedule)
        {
            UInt32 xl_par = Right(Data);
            UInt32 xr_par = Left(Data);

            xl_par ^= Schedule.Get(0);
            for (int i = 0; i < 16; i += 2)
            {
                xr_par = round(xr_par, xl_par, Schedule.Get(i+1));
                xl_par = round(xl_par, xr_par, Schedule.Get(i+2));
            }
            xr_par = xr_par ^ Schedule.Get(17);

            //swap the blocks
            uint swap = xl_par;
            xl_par = xr_par;
            xr_par = swap;

            return Combine(xl_par, xr_par);
        }

        private uint round(uint a, uint b, UInt32 Key)
        {
            uint x1 = (BlowfishConstants.sbox[0, wordByte0(b)] + BlowfishConstants.sbox[1, wordByte1(b)]) ^ BlowfishConstants.sbox[2,wordByte2(b)];
            uint x2 = x1 + BlowfishConstants.sbox[3, this.wordByte3(b)];
            uint x3 = x2 ^ Key;

            return x3 ^ a;
        }

        //gets the first byte in a uint
        private byte wordByte0(uint w)
        {
            return (byte)(w / 256 / 256 / 256 % 256);
        }

        //gets the second byte in a uint
        private byte wordByte1(uint w)
        {
            return (byte)(w / 256 / 256 % 256);
        }

        //gets the third byte in a uint
        private byte wordByte2(uint w)
        {
            return (byte)(w / 256 % 256);
        }

        //gets the fourth byte in a uint
        private byte wordByte3(uint w)
        {
            return (byte)(w % 256);
        }

        UInt64 DecryptBlock(UInt64 Data, KeySchedule Schedule)
        {
            for (int i = 17; i > 1; i--)
            {
                Data = Round(Data, Schedule.Get(i));
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

            UInt32 sum1 = BlowfishConstants.sbox[0, a] + BlowfishConstants.sbox[1, b];
            UInt32 sum2 = sum1 ^ BlowfishConstants.sbox[2, c];
            UInt32 sum3 = sum2 + BlowfishConstants.sbox[3, d];

            return sum3;
        }

        /// <summary>
        /// The round 
        /// </summary>
        /// <param name="data"></param>
        UInt64 Round(UInt64 Data, UInt32 RoundKey)
        {
            UInt32 xL = Left(Data);
            UInt32 xR = Right(Data);

            xL ^= RoundKey;
            xR ^= F(xL);

            UInt64 output = Combine(xR, xL);

            return output;
        }
    }
}
