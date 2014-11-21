using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Blowfish.BlowfishEngine engine = new Blowfish.BlowfishEngine();
            engine.Go();
            Console.ReadLine();
        }
    }
}
