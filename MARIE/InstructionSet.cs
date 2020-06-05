using MARIE.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE
{
    /// <summary>
    /// This class contains a map between given the byte/opcode and the executable instruction
    /// </summary>
    public sealed class InstructionSet
    {
        /// <summary>
        /// The definition for an instruction to execute
        /// </summary>
        /// <param name="sim">The instance of the MARIE simulator the instruction is executing upon</param>
        public delegate void Instruction(MarieSimulator sim);

        private readonly Dictionary<byte, Instruction> instructions = new Dictionary<byte, Instruction>();

        /// <summary>
        /// Register an unused opcode to the specified instruction
        /// </summary>
        /// <param name="opcode">The unused bytecode</param>
        /// <param name="action">The instruction implementation</param>
        public void RegisterInstruction(byte opcode, Instruction action)
        {
            if (instructions.ContainsKey(opcode))
                throw new InvalidOperationException($"The opcode {opcode} is already registered");
            instructions[opcode] = action;
        }

        /// <summary>
        /// Retrieves the instruction for the opcode, if it does not exist it throws an exception
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        /// <exception cref="InvalidInstructionException">Thrown the opcode is not mapped to an instruction</exception>
        public Instruction FetchInstruction(byte opcode)
        {
            if (!instructions.ContainsKey(opcode))
                throw new InvalidInstructionException(opcode);
            return instructions[opcode];
        }

        public InstructionSet()
        {
            DefaultInstructions.Register(this);
        }

        /// <summary>
        /// Gets or the sets the instruction for the opcode
        /// </summary>
        /// <param name="opcode"></param>
        /// <returns></returns>
        public Instruction this[byte opcode] 
        {
            get => FetchInstruction(opcode);
            set => RegisterInstruction(opcode, value);
        }
    }
}
