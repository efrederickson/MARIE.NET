using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE
{
    public static class DecodeExtensions
    {
        /// <summary>
        /// Decodes the MARIE opcode (bytes 15-12)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static byte DecodeOpcode(this ushort word)
        {
            // By shifting it 12 places, we eliminate the address portion of the word, leaving only the opcode
            return (byte)(word >> 12);
        }

        /// <summary>
        /// Decodes the MARIE address (bytes 11-0)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static ushort DecodeAddress(this ushort word)
        {
            // By ANDing 0x0FFF, we eliminate the opcode portion of the word, leaving just the address
            return (ushort)(word & 0x0FFF);
        }
    }
}
