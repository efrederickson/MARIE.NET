using System;
using System.Collections.Generic;
namespace MARIE
{
    // The documentation here for the instructions is largely copied or derived from  (MLA):
    // Null, Linda, and Julia Lobur. 
    // "Chapter 4 - MARIE — An Introduction to a Simple Computer". 
    // The Essentials of Computer Organization and Architecture. 
    // Jones and Bartlett Learning, © 2003. 
    // Books24x7. Web. Jun. 4, 2020. 
    // <http://library.books24x7.com.ezproxy.umgc.edu/toc.aspx?bookid=5893>

    /// <summary>
    /// This class contains the default MARIE instruction set and can register them to an instruction set
    /// </summary>
    public static class DefaultInstructions
    {
        public static void Register(InstructionSet instructionSet)
        {
            // 0001 = 1 = Load X
            instructionSet[0b0001] = Load;

            // 0010 = 2 = Store X
            instructionSet[0b0010] = Store;

            // 0011 = 3 = Add X
            instructionSet[0b0011] = Add;

            // 0100 = 4 = Subtract X
            instructionSet[0b0100] = Subtract;

            // 0101 = 5 = Input
            instructionSet[0b0101] = Input;

            // 0110 = 6 = Output
            instructionSet[0b0110] = Output;

            // 0111 = 7 = Halt
            instructionSet[0b0111] = Halt;

            // 1000 = 8 = Skipcond
            instructionSet[0b1000] = Skipcond;

            // 1001 = 9 = Jump X
            instructionSet[0b1001] = Jump;
        }

        public static void Load(MarieSimulator ms)
        {
            // Load the contents of address X into AC
            // MAR <- X
            // MBR <- M[MAR]
            // AC <- MBR

            ms.MemoryAddressRegister = ms.InstructionRegister.DecodeAddress();
            ms.MemoryBufferRegister = ms.GetMemoryAtMAR();
            ms.Accumulator = ms.MemoryBufferRegister;
        }

        public static void Store(MarieSimulator ms)
        {
            // Store the contents of AC at address X
            // MAR <- X, MBR <- AC
            // M[MAR] <- MBR

            ms.MemoryAddressRegister = ms.InstructionRegister.DecodeAddress();
            ms.MemoryBufferRegister = ms.Accumulator;
            ms.SetMemory(ms.MemoryAddressRegister, ms.MemoryBufferRegister);
        }

        public static void Add(MarieSimulator ms)
        {
            // Add the contents of address X to AC and store the result in AC
            // MAR <- X
            // MBR <- M[MAR]
            // AC <- AC + MBR

            ms.MemoryAddressRegister = ms.InstructionRegister.DecodeAddress();
            ms.MemoryBufferRegister = ms.GetMemoryAtMAR();
            ms.Accumulator += ms.MemoryBufferRegister;
        }

        public static void Subtract(MarieSimulator ms)
        {
            // Subtract the contents of address X from AC and store the result in AC
            // MAR <- X
            // MBR <- M[MAR]
            // AC <- AC – MBR

            ms.MemoryAddressRegister = ms.InstructionRegister.DecodeAddress();
            ms.MemoryBufferRegister = ms.GetMemoryAtMAR();
            ms.Accumulator -= ms.MemoryBufferRegister;
        }

        public static void Input(MarieSimulator ms)
        {
            // Input a value from the keyboard into AC
            // Any input from the input device is first routed into the InREG. Then the ms.InstructionRegister is transferred into the AC.
            // AC <- InREG
            ms.Accumulator = ms.InputRegister;
        }

        public static void Output(MarieSimulator ms)
        {
            // Output the value in AC to the display
            // This instruction causes the contents of the AC to be placed into the OutREG, where it is eventually sent to the output device.
            // OutREG <- AC
            ms.OutputRegister = ms.Accumulator;
        }

        public static void Halt(MarieSimulator ms)
        {
            // Terminate the program
            // No operations are performed on registers; the machine simply ceases execution.
            ms.CurrentState = SimulatorExecutionState.Halted;
        }

        public static void Skipcond(MarieSimulator ms)
        {
            // Skip the next instruction on condition
            // Recall that this instruction uses the bits in positions 10 and 11 in the address field to determine what comparison to perform on the AC. 
            // Depending on this bit combination, the AC is checked to see whether it is negative, equal to zero, or greater than zero. 
            // If the given condition is true, then the next instruction is skipped. This is performed by incrementing the PC register by 1.

            // if IR[11–10] = 00 then           { if bits 10 and 11 in the IR are both 0}
            // If AC< 0 then PC <- PC + 1
            // else If IR[11–10] = 01 then      { if bit 11 = 0 and bit 10 = 1}
            // If AC = 0 then PC <- PC + 1
            // else If IR[11–10] = 10 then      { if bit 11 = 1 and bit 10 = 0}
            // If AC > 0 then PC <- PC + 1
            // If the bits in positions ten and eleven are both ones, an error condition results. 
            // However, an additional condition could also be defined using these bit values.

            var addr = ms.InstructionRegister.DecodeAddress();
            if ((addr & 0b1100_0000_0000) == 0b00)
            {
                if (ms.Accumulator < 0)
                    ms.ProgramCounter++;
            }
            else if ((addr & 0b1100_0000_0000) == 0b01)
            {
                if (ms.Accumulator == 0)
                    ms.ProgramCounter++;
            }
            else if ((addr & 0b1100_0000_0000) == 0b10)
            {
                if (ms.Accumulator > 0)
                    ms.ProgramCounter++;
            }

        }

        public static void Jump(MarieSimulator ms)
        {
            // Load the value of X into PC
            // This instruction causes an unconditional branch to the given address, X. Therefore, to execute this instruction, X must be loaded into the PC.

            // PC <- X
            // In reality, the lower or least significant 12 bits of the instruction register(or IR[11–0]) reflect the value of X.
            // So this transfer is more accurately depicted as:

            // PC <- IR[11–0]
            // However, we feel that the notation PC <- X is easier to understand and relate to the actual instructions, so we use this instead.

            ms.ProgramCounter = ms.InstructionRegister.DecodeAddress();
        }
    }
}
