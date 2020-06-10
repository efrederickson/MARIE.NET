using System.Collections.Generic;

namespace MARIE.Assembler
{
    /// <summary>
    /// Provides an empty (all zero) instruction, primarily a placeholder
    /// </summary>
    public class EmptyInstruction : Instruction
    {
        public override ushort Encode(Dictionary<string, ushort> varAddrMap) => 0x0000;
    }
}
