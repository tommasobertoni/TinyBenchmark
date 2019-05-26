using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// Defines which kind of logs should be displayed during execution.
    /// </summary>
    public enum OutputLevel
    {
        /// <summary>
        /// No logs.
        /// </summary>
        Silent = 0,

        /// <summary>
        /// Only logs indicating errors.
        /// </summary>
        ErrorsOnly = 5,

        /// <summary>
        /// Logs with minimal information about the execution.
        /// </summary>
        Minimal = 10,

        /// <summary>
        /// Informative logs with information about the steps of the execution.
        /// </summary>
        Normal = 20,

        /// <summary>
        /// Rich logs with many details about the steps of the execution.
        /// </summary>
        Verbose = 30
    }
}
