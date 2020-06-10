using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MARIE.Assembler
{
    public class Parser
    {
        private Instruction[] assembled;
        Dictionary<string, ushort> varAddrMap;
        ushort asm_ptr = 0;

        /// <summary>
        /// Parses the input code and returns a fully assembled program
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ushort[] AssembleProgram(string code)
        {
            assembled = new Instruction[0x0FFF];
            // init array with all empty instructions
            for (int i = 0; i < assembled.Length; i++)
                assembled[i] = new EmptyInstruction();

            varAddrMap = new Dictionary<string, ushort>();
            asm_ptr = 0;
            Parse(code);
            return assembled.Select(i => i.Encode(varAddrMap)).ToArray();
        }

        /// <summary>
        /// Place the instruction into the assembled program and increment the location pointer
        /// </summary>
        /// <param name="ins"></param>
        private void Emit(Instruction ins)
        {
            assembled[asm_ptr] = ins;
            asm_ptr++;
        }

        /// <summary>
        /// Emits an instruction with an operand that is either a literal value or a named variable
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="addr"></param>
        void EmitWithOperand(byte opcode, string addr)
        {
            if (char.IsDigit(addr[0])) // hex num literal
                Emit(new LiteralInstruction((ushort)((opcode << 12) + (ushort.Parse(addr, NumberStyles.HexNumber) & 0x0FFF))));
            else
                Emit(new VariableReferencingInstruction(opcode, addr));
        }

        /// <summary>
        /// Main parser logic
        /// </summary>
        /// <param name="code"></param>
        private void Parse(string code)
        {
            var tr = new StringReader(code);
            string line;

            while ((line = tr.ReadLine()) != null)
            {
                line = line.Trim();

                // Skip empty lines
                if (string.IsNullOrEmpty(line))
                    continue;

                // Skip comment
                if (line.StartsWith('/'))
                    continue;

                // Split line by whitespace
                var tokens = line.Split(" ").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));

                // Skip lines containing only whitespace
                if (tokens.Count() == 0)
                    continue;

                // Instructions here are hard-coded to their values
                // I don't care enough right now to implement some sort of lookup table like for the instruction set
                // Another issue is after parsing the instruction is does not verify the end of the line... 

                var first = tokens.First().ToUpper();
                if (first == "ORG")
                    asm_ptr = ushort.Parse(tokens.Skip(1).First(), NumberStyles.HexNumber);
                else if (first == "LOAD")
                    EmitWithOperand(0x1, tokens.Skip(1).First());
                else if (first == "STORE")
                    EmitWithOperand(0x2, tokens.Skip(1).First());
                else if (first == "ADD")
                    EmitWithOperand(0x3, tokens.Skip(1).First());
                else if (first == "SUBT")
                    EmitWithOperand(0x4, tokens.Skip(1).First());
                else if (first == "INPUT")
                    Emit(new LiteralInstruction(0x5000));
                else if (first == "OUTPUT")
                    Emit(new LiteralInstruction(0x6000));
                else if (first == "HALT")
                    Emit(new LiteralInstruction(0x7000));
                else if (first == "SKIPCOND")
                    EmitWithOperand(0x8, tokens.Skip(1).First());
                else if (first == "JUMP")
                    EmitWithOperand(0x9, tokens.Skip(1).First());
                else if (first.EndsWith(","))
                {
                    // parse variable
                    var name = tokens.First()[0..^1];
                    var type = tokens.Skip(1).First();
                    var val = tokens.Skip(2).First();

                    NumberStyles parseType = NumberStyles.Integer;
                    if (type.Equals("hex", StringComparison.OrdinalIgnoreCase))
                        parseType = NumberStyles.HexNumber;
                    else if (type.Equals("dec", StringComparison.OrdinalIgnoreCase))
                        parseType = NumberStyles.Integer;

                    var decoded = short.Parse(val, parseType);
                    varAddrMap[name] = asm_ptr;
                    Emit(new LiteralInstruction((ushort)decoded));
                }
                else
                    throw new Exception($"Invalid symbol {tokens.First()}");
            }
        }
    }
}
