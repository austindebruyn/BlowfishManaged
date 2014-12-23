using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustinXYZ.KeyGeneration
{
    /// <summary>
    /// Internal class to represent a full round of keys.
    /// </summary>
    public class KeySchedule
    {
        /// <summary>
        /// The original key that generated the schedule.
        /// </summary>
        byte[] Original;

        /// <summary>
        /// List of subkeys in the schedule.
        /// </summary>
        UInt32[] Subkeys;

        /// <summary>
        /// Constructor.
        /// </summary>
        public KeySchedule(byte[] Key)
        {
            // Deep copy the reference.
            Original = new byte[Key.Length];
            Array.Copy(Key, Original, Original.Length);
            
            Generate();
        }

        /// <summary>
        /// Returns the given subkey.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UInt32 Get(int index)
        {
            return Subkeys[index];
        }

        /// <summary>
        /// Generate the key schedule. This is an expensive process.
        /// </summary>
        void Generate()
        {
            // P-array is initialized with the continued values of pi, wherever
            // the s-box init vectors left off.
            Subkeys = new UInt32[18];
            Array.Copy(BlowfishConstants.parray, Subkeys, 18);

            // Cycle through the key, xor-ing each byte with the byte in the subkey array.
            // When the original key is exhausted, just loop back to the beginning, until
            // all subkeys are finished.
            int index = 0;
            for (int i = 0; i < Subkeys.Length; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    UInt32 d = (UInt32)(Original[index++]);
                    Subkeys[i] ^= (d  << ((3-j) * 8));
                    if (index >= Original.Length) index = 0;
                }
            }
        }

        /// <summary>
        /// Replace the given key.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="encryptedZeroString"></param>
        internal void Set(int index, UInt32 encryptedZeroString)
        {
            Subkeys[index] = encryptedZeroString;
        }
    }
}
