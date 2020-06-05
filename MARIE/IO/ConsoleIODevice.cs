using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE.IO
{
    /// <summary>
    /// A generic I/O implementation that uses the console
    /// </summary>
    public class ConsoleIODevice : IIODevice
    {
        public void AttachTo(MarieSimulator ms)
        {
            ms.InputRegisterRead += Ms_InputRegisterRead;
            ms.OutputRegisterWrite += Ms_OutputRegisterWrite;
        }

        public void DetachFrom(MarieSimulator ms)
        {
            ms.InputRegisterRead -= Ms_InputRegisterRead;
            ms.OutputRegisterWrite -= Ms_OutputRegisterWrite;
        }
        private void Ms_OutputRegisterWrite(object sender, IOEventArgs e)
        {
            Write(e.Simulator.OutputRegister);
            e.Handled = true;
        }

        private void Ms_InputRegisterRead(object sender, IOEventArgs e)
        {
            e.Simulator.InputRegister = Read();
            e.Handled = true;
        }

        public ushort Read()
        {
            return ushort.Parse(Console.ReadLine());
        }

        public void Write(ushort data)
        {
            Console.Write(data);
        }
    }
}
