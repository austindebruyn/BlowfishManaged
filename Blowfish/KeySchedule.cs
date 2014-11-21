using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish
{
    /// <summary>
    /// Internal class to represent a full round of keys.
    /// </summary>
    class KeySchedule
    {
        /// <summary>
        /// The original key that generated the schedule.
        /// </summary>
        Key Original;

        /// <summary>
        /// List of subkeys in the schedule.
        /// </summary>
        ByteBlock[] Subkeys;

        /// <summary>
        /// There are four 32-bit S-boxes with 256 entries each.
        /// </summary>
        UInt32[,] sBox;

        /// <summary>
        /// Constructor.
        /// </summary>
        public KeySchedule(Key Key)
        {
            Original = new Key(Key);

            // S-boxes are initialized with the values of pi.
            sBox = new UInt32[4, 256];
            Array.Copy(BlowfishConstants.sbox, sBox, sBox.LongLength);

            // P-array is initialized with the continued values of pi, wherever
            // the s-box init vectors left off.
            Subkeys = new ByteBlock[18];


        }
    }
}
