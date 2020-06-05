using MARIE.IO;
using System;
using System.Collections.Generic;
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
            List<byte> program = new List<byte>();

            Console.WriteLine("An empty line will commence execution of the program");
            while (true)
            {
                Console.Write("Instruction (hex): ");
                var insRaw = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(insRaw))
                    break;
                ushort data = ushort.Parse(insRaw, System.Globalization.NumberStyles.HexNumber);
                program.AddRange(new[] { (byte)(data >> 8), (byte)(data & 0xFF) });
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
                var mem = ms.GetMemory().Take(8 * 4).ToArray();
                for (int i = 0; i < mem.Length; i += 8)
                {
                    for (int j = 0; j < 8; j += 2)
                    {
                        var fmt = string.Format("{0:X02}{1:X02} ", mem[i + j], mem[i + j + 1]);
                        Console.Write(fmt);
                    }
                    Console.WriteLine();
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
