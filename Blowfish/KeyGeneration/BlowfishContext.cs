using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish.KeyGeneration
{
    /// <summary>
    /// The context encapsulates a number of runtime data structures used during encryption.
    /// Blowfish has a large memory footprint, with nearly 4K of memory needed to encrypt a single
    /// 64-bit block. It takes 521 iterations to complete the key schedule and s-boxes.
    /// 
    /// This context is computed once per key. To change key, discard this instance and construct
    /// a new key. The Blowfish algorithm was not designed with rapidly changing keys in mind.
    /// </summary>
    class BlowfishContext
    {
        /// <summary>
        /// The keys for each round of the algorithm.
        /// </summary>
        public KeySchedule Schedule;

        /// <summary>
        /// The s-boxes.
        /// </summary>
        public UInt32[,] sbox;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlowfishContext()
        {
        }

        /// <summary>
        /// Constructor via Key.
        /// </summary>
        public BlowfishContext(BlowfishKey Key) : base()
        {
            sbox = new UInt32[4, 256];
            Array.Copy(BlowfishConstants.sbox, sbox, BlowfishConstants.sbox.LongLength);

            Schedule = new KeySchedule(Key);
            Setup();
        }

        /// <summary>
        /// After being initialized with a key, the schedule (p-array) and s-boxes needs some
        /// work from the Blowfish engine to finish setting up.
        /// </summary>
        void Setup()
        {
            // The setup process starts with the all zero string.
            UInt64 bitScape = 0x0000000000000000;

            // To set up the schedule (p-arrays), 
            for (int i = 0; i < 9; i++)
            {
                bitScape = BlowfishEngine.EncryptBlock(bitScape, Schedule);
                Schedule.Set(i * 2, ByteOperations.Left(bitScape));
                Schedule.Set(i * 2 + 1, ByteOperations.Right(bitScape));
                bitScape = ByteOperations.Swap(bitScape);
            }

            // To finish the s-boxes
            for (int i = 0; i < 1024; )
            {
                bitScape = BlowfishEngine.EncryptBlock(bitScape, Schedule);

                BlowfishConstants.sbox[i / 256, i % 256] = ByteOperations.Left(bitScape);
                i++;
                BlowfishConstants.sbox[i / 256, i % 256] = ByteOperations.Right(bitScape);
                i++;

                bitScape = ByteOperations.Swap(bitScape);
            }
        }
    }
}
