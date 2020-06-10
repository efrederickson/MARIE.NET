using System;
using System.IO;
using System.Linq;

namespace MARIE.Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            var inFile = args.FirstOrDefault();
            if (string.IsNullOrEmpty(inFile))
            {
                Console.WriteLine("Please specify input filename");
            }

            var outFile = args.Skip(1).FirstOrDefault();
            if (string.IsNullOrEmpty(outFile))
            {
                Console.WriteLine("Please specify output filename");
            }

            Parser parser = new Parser();
            ushort[] prog = parser.AssembleProgram(File.ReadAllText(inFile));

            Console.WriteLine("Assembled Program:");
            Console.WriteLine("        +0   +1   +2   +3   +4   +5   +6   +7   +8   +9   +A   +B   +C   +D   +E   +F");
            var mem = prog;
            for (int i = 0; i < mem.Length - 0xF; i += 0x10)
            {
                Console.Write(string.Format("{0:X04}   ", i));
                for (int j = 0; j <= 0xf; j += 1)
                {
                    var fmt = string.Format(" {0:X04}", mem[i + j]);
                    Console.Write(fmt);
                }
                Console.WriteLine();
            }

            File.WriteAllBytes(outFile, prog.SelectMany(word => new byte[2] { (byte)(word >> 8), (byte)(word & 0x00ff) }).ToArray());
        }
    }
}
