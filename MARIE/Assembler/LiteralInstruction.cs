using System.Collections.Generic;

namespace MARIE.Assembler
{
    /// <summary>
    /// Provides a literal parsed instruction or value with no dependency on a named variable
    /// </summary>
    public class LiteralInstruction : Instruction
    {
        /// <summary>
        /// The full 16-bit instruction value
        /// </summary>
        public ushort Value { get; set; }
        public override ushort Encode(Dictionary<string, ushort> varAddrMap) => Value;

        public LiteralInstruction() { }
        public LiteralInstruction(ushort ins) => Value = ins;
    }
}
