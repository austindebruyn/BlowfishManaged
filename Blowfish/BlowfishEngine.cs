using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blowfish
{
    /// <summary>
    /// An engine object that will drive all encryption activities. This is the public
    /// API for the assembly.
    /// </summary>
    public class BlowfishEngine
    {
        public static String hello = "Hello world";

        /// <summary>
        /// Constructor.
        /// </summary>
        public BlowfishEngine()
        {

        }

        public void Go()
        {
            var Key = new Key(Encoding.UTF8.GetBytes("helloworldhelloworldhelloworldaa"));
            var Plain = new ByteBlock(Encoding.UTF8.GetBytes("aaaabbbbccccddddaaaabbbbccccddddaaaabbbbccccddddaaaabbbbccccdddd"));

            Console.WriteLine("Key: \n" + Key);
            Console.WriteLine("Plaintext: \n" + Plain);

            var Schedule = new KeySchedule(Key);
        }
    }
}
