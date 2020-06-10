using System.Collections.Generic;

namespace MARIE.Assembler
{
    /// <summary>
    /// A parsed instruction that relies on a named variable
    /// </summary>
    public class VariableReferencingInstruction : Instruction
    {
        /// <summary>
        /// The 4-bit opcode
        /// </summary>
        public byte Opcode { get; set; }

        /// <summary>
        /// The named variable to lookup and convert into an address
        /// </summary>
        public string Varname { get; set; }

        public override ushort Encode(Dictionary<string, ushort> varAddrMap)
        {
            return (ushort)((Opcode << 12) + varAddrMap[Varname]);
        }

        public VariableReferencingInstruction() { }
        public VariableReferencingInstruction(byte opcode, string varname)
        {
            Opcode = opcode;
            Varname = varname;
        }
    }
}
