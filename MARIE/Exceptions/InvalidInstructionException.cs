using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE.Exceptions
{
    /// <summary>
    /// This execution is used to identify that an invalid opcode/instruction has been executed
    /// </summary>
    public class InvalidInstructionException : Exception
    {
        /// <summary>
        /// The opcode byte that is invalid
        /// </summary>
        public byte Opcode { get; }

        public InvalidInstructionException(byte opcode) : base() => Opcode = opcode;
    }
}
