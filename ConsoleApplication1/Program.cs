using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using AustinXYZ;
using System.Security.Cryptography;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            SymmetricAlgorithm blowfish = new AustinXYZ.BlowfishManaged();

            byte[] plain = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] kk = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] enc = new byte[8];
            byte[] dec = new byte[8];

            blowfish.Key = kk;
            ICryptoTransform transform = blowfish.CreateEncryptor();

            transform.TransformBlock(plain, 0, 8, enc, 0);

            Console.WriteLine(String.Format("Encrypted: {0}", BitConverter.ToString(enc)));


            ICryptoTransform dtransform = blowfish.CreateDecryptor();
            dtransform.TransformBlock(enc, 0, 8, dec, 0);

            Console.WriteLine(String.Format("Decrypted: {0}", BitConverter.ToString(dec)));
            Console.ReadLine();
        }
    }
}
