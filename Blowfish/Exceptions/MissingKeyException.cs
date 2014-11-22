using System;

namespace Blowfish.Exceptions
{
    public class MissingKeyException : Exception
    {
        public MissingKeyException(String Message)
            : base(Message)
        {
            //
        }
    }
}
