using MARIE.IO;
using System;

namespace MARIE
{
    public class MarieSimulator
    {
        public event EventHandler<IOEventArgs> InputRegisterRead;
        public event EventHandler<IOEventArgs> OutputRegisterWrite;

        private byte[] memory = new byte[0x0FFF]; // 4K words of main memory (this implies 12 bits per address)

        /// <summary>
        /// A 16-bit accumulator(AC)
        /// </summary>
        public ushort Accumulator { get; set; }

        /// <summary>
        /// A 16-bit instruction register(IR)
        /// </summary>
        public ushort InstructionRegister { get; set; }

        /// <summary>
        /// Gets the current decoded opcode
        /// </summary>
        public byte Opcode { get; protected set; }

        /// <summary>
        /// A 16-bit memory buffer register(MBR)
        /// </summary>
        public ushort MemoryBufferRegister { get; set; }

        /// <summary>
        /// The program counter, which holds the address of the next instruction to be executed in the program.
        /// </summary>
        public ushort ProgramCounter
        {
            get => (ushort)(program_counter & 0xFFF);
            set => program_counter = (ushort)(value & 0xFFF);
        }
        ushort program_counter; // A 12-bit program counter(PC)

        /// <summary>
        /// The memory address register, which holds the memory address of the data being referenced.
        /// </summary>
        public ushort MemoryAddressRegister
        {
            get => (ushort)(memory_address_register & 0xFFF);
            set => memory_address_register = (ushort)(value & 0xFFF);
        }
        ushort memory_address_register; // A 12-bit memory address register(MAR)

        /// <summary>
        /// An 8-bit input register
        /// </summary>
        public ushort InputRegister
        {
            get
            {
                var ea = new IOEventArgs(this);

                // Allow registered events to handle retrieving input
                InputRegisterRead?.Invoke(this, ea);

                // If they did not, manually read from the IO device
                if (!ea.Handled)
                    InputRegister = IODevice.Read();

                return input_register;
            }
            set => input_register = value;
        }
        ushort input_register;

        /// <summary>
        /// An 8-bit output register
        /// </summary>
        public ushort OutputRegister
        {
            get => output_register;
            set
            {
                output_register = value;

                var ea = new IOEventArgs(this);

                // Allow events to handle output
                OutputRegisterWrite?.Invoke(this, ea);

                // If not, call it
                if (!ea.Handled)
                    IODevice.Write(OutputRegister);
            }
        }
        ushort output_register;

        /// <summary>
        /// The current I/O device attached to the simulator
        /// </summary>
        public IIODevice IODevice
        {
            get => ioDevice;
            set
            {
                if (ioDevice != null)
                    ioDevice.DetachFrom(this);
                ioDevice = value;
                if (ioDevice != null)
                    ioDevice.AttachTo(this);
            }
        }
        private IIODevice ioDevice;

        /// <summary>
        /// The current execution state of the simulator
        /// </summary>
        public SimulatorExecutionState CurrentState { get; set; }

        /// <summary>
        /// The instruction set for this instance of the MARIE simulator
        /// </summary>
        public InstructionSet InstructionSet { get; set; } = new InstructionSet();

        public MarieSimulator()
        {
            IODevice = new ConsoleIODevice();
        }

        /// <summary>
        /// Retrives the word stored stored at the specified address
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public ushort GetMemory(ushort addr)
        {
            // Word addressable, so multiply by the size of the word
            addr *= sizeof(ushort);

            if (addr > 0x0FFF)
                throw new ArgumentOutOfRangeException(nameof(addr), "Memory address cannot be greater than 0x0FFF");

            // Ensure the address is masked to 12 bytes as per the MARIE spec
            ushort masked = (ushort)(addr & 0x0FFF);

            // Retrieve the first byte of the word
            byte first = memory[masked];

            // Retrieve the second byte of the word
            byte second = memory[masked + 1];

            // Return the combined bytes into a single word
            return (ushort)((first << 8) + second);
        }

        /// <summary>
        /// Sets the word stored at the specified address
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="word"></param>
        public void SetMemory(ushort addr, ushort word)
        {
            // Word addressable, so multiply by the size of the word
            addr *= sizeof(ushort);

            if (addr > 0x0FFF)
                throw new ArgumentOutOfRangeException(nameof(addr), "Memory address cannot be greater than 0x0FFF");

            // Ensure the address is masked to 12 bytes as per the MARIE spec
            ushort masked = (ushort)(addr & 0x0FFF);

            memory[masked] = (byte)(word >> 8);
            memory[masked + 1] = (byte)(word & 0x00FF);
        }

        /// <summary>
        /// Returns the memory value at the MAR (Memory Address Register)
        /// </summary>
        /// <returns></returns>
        public ushort GetMemoryAtMAR() => GetMemory(MemoryAddressRegister);

        /// <summary>
        /// Returns a copy of the entire memory of the MARIE simulator
        /// </summary>
        /// <returns></returns>
        public byte[] GetMemory()
        {
            byte[] copy = new byte[memory.Length];
            memory.CopyTo(copy, 0);
            return copy;
        }

        /// <summary>
        /// Set the memory to the specified program and set (or reset) the simulator for execution
        /// </summary>
        /// <param name="program"></param>
        public void LoadProgram(byte[] program)
        {
            if (program.Length > 0x0FFF)
                throw new ArgumentException("Program length should less than 0x0FFF", nameof(program));
            memory = new byte[0x0FFF];
            program.CopyTo(memory, 0);

            // Reset counters to 0
            ProgramCounter = 0;
            Accumulator = 0;
            InstructionRegister = 0;
            Opcode = 0;
            MemoryBufferRegister = 0;
            MemoryAddressRegister = 0;
            CurrentState = SimulatorExecutionState.Executing;
        }

        /// <summary>
        /// Performs the Fetch portion of the FDE cycle, as per the MARIE spec
        /// </summary>
        /// <returns></returns>
        public MarieSimulator Fetch()
        {
            // Copy the contents of the PC to the MAR. 
            // Go to main memory and fetch the instruction found at the address in the MAR, placing this instruction in the IR; 
            // increment PC by 1(PC now points to the next instruction in the program):  
            // MAR <- PC
            // IR <- M[MAR]
            // PC <- PC + 1

            MemoryAddressRegister = ProgramCounter;
            InstructionRegister = GetMemory(MemoryAddressRegister);
            ProgramCounter++;
            return this;
        }

        /// <summary>
        /// Performs the Decode portion of the FDE cycle, as per the MARIE spec
        /// </summary>
        /// <returns></returns>
        public MarieSimulator Decode()
        {
            // Copy the rightmost 12 bits of the IR into the MAR; 
            // decode the leftmost four bits to determine the opcode
            // MAR <- IR[11–0]
            // (opcode) <- IR[15–12].

            MemoryAddressRegister = InstructionRegister.DecodeAddress();
            Opcode = InstructionRegister.DecodeOpcode();

            return this;
        }

        /// <summary>
        /// Performs the Execute portion of the FDE cycle, almost as per the MARIE spec
        /// </summary>
        /// <returns></returns>
        public MarieSimulator Execute()
        {
            // If necessary, use the address in the MAR to go to memory to get data, 
            // placing the data in the MBR (and possibly the AC), 
            // and then execute the instruction MBR <- M[MAR] and execute the actual instruction.

            var ins = InstructionSet.FetchInstruction(Opcode);
            ins(this);

            return this;
        }
    }
}
