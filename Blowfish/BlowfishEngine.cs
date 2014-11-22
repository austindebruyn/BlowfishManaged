using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blowfish.KeyGeneration;

namespace Blowfish
{
    internal class BlowfishEngine
    {
        /// <summary>
        /// Encrypts a single block, running the 64 bits of data through all 16
        /// rounds of the Blowfish algorithm. Returns the encrypted 64 bits.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public static UInt64 EncryptBlock(UInt64 Data, KeySchedule Schedule)
        {
            UInt32 xl_par = ByteOperations.Right(Data);
            UInt32 xr_par = ByteOperations.Left(Data);

            xl_par ^= Schedule.Get(0);
            for (int i = 0; i < 16; i += 2)
            {
                xr_par = round(xr_par, xl_par, Schedule.Get(i + 1));
                xl_par = round(xl_par, xr_par, Schedule.Get(i + 2));
            }
            xr_par = xr_par ^ Schedule.Get(17);

            //swap the blocks
            uint swap = xl_par;
            xl_par = xr_par;
            xr_par = swap;

            return ByteOperations.Combine(xl_par, xr_par);
        }

        /// <summary>
        /// The function that powers the Feistel network.
        /// </summary>
        /// <param name="rightHalf"></param>
        /// <returns></returns>
        //UInt32 F(UInt32 value)
        //{


        //    UInt32 sum1 = BlowfishConstants.sbox[0, a] + BlowfishConstants.sbox[1, b];
        //    UInt32 sum2 = sum1 ^ BlowfishConstants.sbox[2, c];
        //    UInt32 sum3 = sum2 + BlowfishConstants.sbox[3, d];

        //    return sum3;
        //}

        private static UInt32 round(UInt32 leftHalf, UInt32 rightHalf, UInt32 Key)
        {
            uint a = (0xFF000000 & rightHalf) >> 24;
            uint b = (0x00FF0000 & rightHalf) >> 16;
            uint c = (0x0000FF00 & rightHalf) >> 8;
            uint d = 0x000000FF & rightHalf;

            uint x1 = (BlowfishConstants.sbox[0, a] + BlowfishConstants.sbox[1, b]) ^ BlowfishConstants.sbox[2,c];
            uint x2 = x1 + BlowfishConstants.sbox[3, d];
            uint x3 = x2 ^ Key;

            return x3 ^ leftHalf;
        }

        //gets the first byte in a uint
        private static byte wordByte0(uint w)
        {
            return (byte)(w / 256 / 256 / 256 % 256);
        }

        //gets the second byte in a uint
        private static byte wordByte1(uint w)
        {
            return (byte)(w / 256 / 256 % 256);
        }

        //gets the third byte in a uint
        private static byte wordByte2(uint w)
        {
            return (byte)(w / 256 % 256);
        }

        //gets the fourth byte in a uint
        private static byte wordByte3(uint w)
        {
            return (byte)(w % 256);
        }
    }
}
