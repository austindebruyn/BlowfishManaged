using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Blowfish.Exceptions;
using Blowfish.KeyGeneration;

namespace Blowfish
{
    /// <summary>
    /// An engine object that will drive all encryption activities. This is the public
    /// API for the assembly.
    /// </summary>
    public class Blowfish
    {
        /// <summary>
        /// The context of precomputed s-boxes and schedule for each key used. The engine
        /// should support multiple keys, but it is not designed for the key to be changed
        /// often. The context will need to be re-assigned when the key changes.
        /// </summary>
        BlowfishContext Context;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Blowfish()
        {
            //
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Blowfish(BlowfishKey newKey)
        {
            Context = new BlowfishContext(newKey);
        }

        /// <summary>
        /// Encrypt block of text.
        /// </summary>
        /// <param name="Plaintext"></param>
        public UInt64 Encrypt(byte[] Plaintext)
        {
            if (Context == null)
            {
                throw new MissingKeyException("No key was set for the engine. Set a key before encrypting.");
            }

            UInt64[] plain64 = ByteOperations.PackBytesIntoUInt64(Plaintext);
            UInt64 Final = BlowfishEngine.EncryptBlock(ByteOperations.Swap(plain64[0]), Context.Schedule);

            return Final;
        }

        /// <summary>
        /// Assigns the given key to the engine and computes the schedule.
        /// </summary>
        /// <param name="key"></param>
        public void SetKey(BlowfishKey key)
        {
            Context = new BlowfishContext(key);
        }

        //UInt64 DecryptBlock(UInt64 Data, KeySchedule Schedule)
        //{
        //    for (int i = 17; i > 1; i--)
        //    {
        //        Data = Round(Data, Schedule.Get(i));
        //    }

        //    // Undo the last swap.
        //    UInt32 leftHalf = ByteOperations.Right(Data) ^ Schedule.Get(1);
        //    UInt32 rightHalf = ByteOperations.Left(Data) ^ Schedule.Get(0);

        //    return ByteOperations.Combine(leftHalf, rightHalf);
        //}

        String UInt64ToByteString(UInt64 value)
        {
            return String.Format("0x{0:X16}", value);
        }

        String UInt32ToByteString(UInt32 value)
        {
            return String.Format("0x{0:X8}", value);
        }


    }
}
