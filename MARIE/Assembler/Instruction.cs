using System.Collections.Generic;

namespace MARIE.Assembler
{
    /// <summary>
    /// Provides a base class for parsed instructions
    /// </summary>
    public abstract class Instruction
    {
        /// <summary>
        /// Converts the parsed instruction, which may rely upon a dynamic variable placement, into the executable 16-bit value
        /// </summary>
        /// <param name="varAddrMap"></param>
        /// <returns></returns>
        public abstract ushort Encode(Dictionary<string, ushort> varAddrMap);
    }
}
