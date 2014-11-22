using Blowfish.KeyGeneration;
using System;

namespace Blowfish.Exceptions
{
    public class InvalidKeyException : Exception
    {
        /// <summary>
        /// Contains the bad key.
        /// </summary>
        BlowfishKey BadKey;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Message"></param>
        public InvalidKeyException(String Message, BlowfishKey BadKey)
            : base(Message)
        {
            this.BadKey = BadKey;
        }
    }
}
