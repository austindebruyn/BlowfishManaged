using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish
{
    class Key : ByteBlock
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Size"></param>
        public Key(int Size) : base(Size)
        {
            if (Size < 32) throw new ArgumentException("Keys can be no smaller than 32 bits in length.");
            if (Size > 448) throw new ArgumentException("Keys can be no greater than 448 bits in length.");
            if (Size % 32 != 0) throw new ArgumentException("Keys must be of a length divisible by 32.");
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data"></param>
        public Key(byte[] Data)
            : this(Data.Length)
        {
            Array.Copy(Data, this.Data, Data.Length);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        public Key(Key other)
            : base(other)
        {

        }
    }
}
