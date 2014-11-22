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
        /// <param name="block64"></param>
        /// <param name="Schedule"></param>
        /// <returns></returns>
        public static UInt64 EncryptBlock(UInt64 block64, BlowfishContext Context)
        {
            UInt32 left32 = ByteOperations.Right(block64);
            UInt32 right32 = ByteOperations.Left(block64);

            // We unrolled the Feistel loop, so the very first key XOR happens
            // outside the loop here.
            left32 ^= Context.Schedule.Get(0);

            // 16 iterations of the Round function.
            for (int i = 0; i < 16; i += 2)
            {
                right32 = Round(right32, left32, Context.Schedule.Get(i + 1), Context);
                left32 = Round(left32, right32, Context.Schedule.Get(i + 2), Context);
            }

            // Finalize the loop unrolling.
            right32 = right32 ^ Context.Schedule.Get(17);

            return ByteOperations.Combine(right32, left32);
        }

        /// <summary>
        /// This performs Schneier's round function for Blowfish, which consists of swapping
        /// the left and right halves, and swapping bytes from the s-box.
        /// </summary>
        /// <param name="leftHalf"></param>
        /// <param name="rightHalf"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        private static UInt32 Round(UInt32 leftHalf, UInt32 rightHalf, UInt32 Key, BlowfishContext Context)
        {
            uint a = (0xFF000000 & rightHalf) >> 24;
            uint b = (0x00FF0000 & rightHalf) >> 16;
            uint c = (0x0000FF00 & rightHalf) >> 8;
            uint d = 0x000000FF & rightHalf;

            uint x1 = (Context.sbox[0, a] + Context.sbox[1, b]) ^ Context.sbox[2, c];
            uint x2 = x1 + Context.sbox[3, d];
            uint x3 = x2 ^ Key;

            return x3 ^ leftHalf;
        }
    }
}
