using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE
{
    /// <summary>
    /// EventArgs used for MARIE I/O events, this allows IIODevice's to handle I/O requests without explicitly being invoked. 
    /// </summary>
    public class IOEventArgs : EventArgs
    {
        public MarieSimulator Simulator { get; protected set; }

        /// <summary>
        /// Whether the I/O event has been handled to an I/O device. 
        /// If set the true, the I/O device on the MARIE simulator will not be explicitly invoked
        /// </summary>
        public bool Handled { get; set; }

        public IOEventArgs(MarieSimulator ms)
        {
            Simulator = ms;
            Handled = false;
        }
    }
}
