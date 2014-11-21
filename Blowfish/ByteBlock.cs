using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish
{
    /// <summary>
    /// Internal class representing an array of bytes.
    /// </summary>
    class ByteBlock
    {
        /// <summary>
        /// Encapsulated data.
        /// </summary>
        protected byte[] Data;

        /// <summary>
        /// Returns the size of this block.
        /// </summary>
        public int Size
        {
            get
            {
                return Data.Length;
            }
        }

        /// <summary>
        /// Empty block constructor.
        /// </summary>
        /// <param name="Size"></param>
        public ByteBlock(int Size)
        {
            Data = new byte[Size];
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public ByteBlock(byte[] Data) : this(Data.Length)
        {
            Array.Copy(Data, this.Data, Data.Length);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="data"></param>
        public ByteBlock(ByteBlock other)
        {
            Data = new byte[other.Size];
            Array.Copy(other.Pointer(), Data, Data.Length);
        }

        /// <summary>
        /// Gets pointer to the encapsulated bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] Pointer()
        {
            return Data;
        }

        /// <summary>
        /// Returns byte at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public byte Get(int index)
        {
            if (index < 0 || index > Size)
            {
                throw new IndexOutOfRangeException();
            }

            return Data[index];
        }

        /// <summary>
        /// Friendly output.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String output = "";

            foreach (byte b in Data) output += String.Format("{0,2:X2}", b);

            return output;
        }
    }
}
