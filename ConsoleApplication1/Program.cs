using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using System.Diagnostics;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] plain = new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 };


            Console.WriteLine("My implementation: ");
            Stopwatch sw = new Stopwatch();
            sw.Start();
                        Blowfish.BlowfishEngine engine = new Blowfish.BlowfishEngine();
                        Blowfish.BlowfishKey Key = new Blowfish.BlowfishKey("49793EBC79B3258F");
                        engine.SetKey(Key);
                        for (int i = 0; i < 1048576; i++)
                        {
                            engine.Encrypt(plain);
                        }
            sw.Stop();
            long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
            Console.WriteLine("Encrypted 64MB in " + microseconds + "micros.\n");


            Console.WriteLine("FireXware's implementation: ");
            sw = new Stopwatch();
            sw.Start();
                        BlowFishCS.BlowFish b = new BlowFishCS.BlowFish("3000000000000000");
                        for (int i = 0; i < 1048576; i++)
                        {
                            b.Encrypt_ECB(plain);
                        }
            sw.Stop();
            microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
            Console.WriteLine("Encrypted 64MB in " + microseconds + "micros.");

            Console.ReadLine();
        }
    }
}
