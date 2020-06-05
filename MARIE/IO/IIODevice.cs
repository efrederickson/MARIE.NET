using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE.IO
{
    public interface IIODevice
    {
        /// <summary>
        /// The I/O wrapper reads unsigned short (or byte) from its input
        /// </summary>
        /// <returns></returns>
        public ushort Read();

        /// <summary>
        /// The I/O wrapper writes the unsigned short (or byte) to its output
        /// </summary>
        /// <param name="data"></param>
        public void Write(ushort data);

        /// <summary>
        /// Register events for the MARIE simulator to this IO device
        /// </summary>
        /// <param name="ms"></param>
        public void AttachTo(MarieSimulator ms);

        /// <summary>
        /// Deregister events for the MARIE simulator to this IO device
        /// </summary>
        /// <param name="ms"></param>
        public void DetachFrom(MarieSimulator ms);
    }
}
