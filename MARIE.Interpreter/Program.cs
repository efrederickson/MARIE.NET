using MARIE.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MARIE.Interpreter
{
    /// <summary>
    /// Basic impl of the IIODevice to keep track of output and wrap the console (for input)
    /// </summary>
    class IODev : IIODevice
    {
        public List<ushort> DataWritten = new List<ushort>();

        public void AttachTo(MarieSimulator ms)
        {
            ms.OutputRegisterWrite += Ms_OutputRegisterWrite;
            ms.InputRegisterRead += Ms_InputRegisterRead;
        }

        private void Ms_InputRegisterRead(object sender, IOEventArgs e)
        {
            e.Simulator.InputRegister = Read();
            e.Handled = true;
        }

        private void Ms_OutputRegisterWrite(object sender, IOEventArgs e)
        {
            Write(e.Simulator.OutputRegister);
            e.Handled = true;
        }

        public void DetachFrom(MarieSimulator ms)
        {
            ms.InputRegisterRead -= Ms_InputRegisterRead;
            ms.OutputRegisterWrite -= Ms_OutputRegisterWrite;
        }

        public ushort Read() => ushort.Parse(Console.ReadLine());

        public void Write(ushort data)
        {
            DataWritten.Add(data);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var ms = new MarieSimulator();
            List<ushort> program = new List<ushort>();

            Console.WriteLine("An empty line will commence execution of the program");
            Console.WriteLine("Alternatively, enter a filename to execute a precompiled binary file");
            while (true)
            {
                Console.Write("Instruction(s) (hex): ");
                var insRaw = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(insRaw))
                    break;

                if (File.Exists(insRaw))
                {
                    var bytes = File.ReadAllBytes(insRaw);
                    for (int i = 0; i < bytes.Length; i += 2)
                        program.Add((ushort)((bytes[i] << 8) + bytes[i+1]));
                    break;
                }

                foreach (var insRawSplit in insRaw.Split(" "))
                {
                    ushort data = ushort.Parse(insRawSplit.Trim(), System.Globalization.NumberStyles.HexNumber);
                    program.Add(data);
                }
            }

            var ioDev = new IODev();

            ms.IODevice = ioDev;
            ms.LoadProgram(program.ToArray());

            do
            {
                // Dump a bunch of information about the MARIE simulator (just enough to get without while also being lazy)
                Console.WriteLine(string.Format("PC: {0:X02}, AC: {1:X02}, IR: {2:X04}, MAR: {3:X04}",
                    ms.ProgramCounter,
                    ms.Accumulator,
                    ms.InstructionRegister,
                    ms.MemoryAddressRegister));

                Console.WriteLine("Memory:");
                Console.WriteLine("        +0   +1   +2   +3   +4   +5   +6   +7   +8   +9   +A   +B   +C   +D   +E   +F");
                var mem = ms.GetMemory().ToArray();
                for (int i = 0; i < mem.Length - 0xF; i += 0x10)
                {
                    Console.Write(string.Format("{0:X04}   ", i));
                    for (int j = 0; j <= 0xf; j += 1)
                    {
                        var fmt = string.Format(" {0:X04}", mem[i + j]);
                        Console.Write(fmt);
                    }
                    Console.WriteLine();

                    // If all the rest is zero mem, skip it
                    if (mem.Skip(i).Where(word => word > 0).Count() == 0)
                    {
                        Console.WriteLine(string.Format("... {0:X04} to 0FFF is all zeroed memory ...", i + 0x10));
                        break;
                    }
                }

                Console.WriteLine("Output:");
                foreach (var b in ioDev.DataWritten)
                {
                    Console.Write(string.Format("{0:X04} ", b));
                }
                Console.WriteLine();

                ms.Fetch().Decode().Execute();
            } while (ms.CurrentState == SimulatorExecutionState.Executing);
        }
    }
}
