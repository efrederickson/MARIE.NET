using System;
using System.Collections.Generic;
using System.Text;

namespace MARIE
{
    /// <summary>
    /// The current execution state of the simulator
    /// </summary>
    public enum SimulatorExecutionState
    {
        /// <summary>
        /// The simulator has halted
        /// </summary>
        Halted = 0,

        /// <summary>
        /// The simulator should continue execution
        /// </summary>
        Executing = 1
    }
}
