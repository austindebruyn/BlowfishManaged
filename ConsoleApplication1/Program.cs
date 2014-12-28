using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using BlowfishManaged;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();

            double iterations = 1000;

            long total1 = 0;
            for (int i = 0; i < iterations; i++)
            {
                TestBlowfish(watch);
                total1 += watch.ElapsedMilliseconds;
            }

            long total2 = 0;
            for (int i = 0; i < iterations; i++)
            {
                TestFireX(watch);
                total2 += watch.ElapsedMilliseconds;
            }

            Console.WriteLine((double)total1 / iterations);
            Console.WriteLine((double)total2 / iterations);
            Console.ReadLine();
        }

        static void TestBlowfish(Stopwatch Watch)
        {
            byte[] cipherKey = new byte[32];
            Random r = new Random();
            r.NextBytes(cipherKey);
            BlowfishManaged.BlowfishManaged bfm = new BlowfishManaged.BlowfishManaged(cipherKey);

            int iterations = 1048576 / 8;
            UInt64[] data = new UInt64[iterations];
            UInt64[] encrypted = new UInt64[iterations];

            Watch.Reset();
            Watch.Start();

            for (int i = 0; i < iterations; i++)
            {
                encrypted[i] = bfm.DecryptSingleBlock(data[i]);
            }

            Watch.Stop();
        }

        static void TestFireX(Stopwatch Watch)
        {
            byte[] cipherKey = new byte[32];
            Random r = new Random();
            r.NextBytes(cipherKey);
            BlowFishCS.BlowFish blowfish = new BlowFishCS.BlowFish(cipherKey);
            int iterations = 1048576;
            byte[] data = new byte[iterations];
            byte[] encrypted = new byte[iterations];

            Watch.Reset();
            Watch.Start();

                encrypted = blowfish.Decrypt_ECB(data);// .EncryptSingleBlock(data[i]);


            Watch.Stop();
        }
    }
}
