using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// A benchmark-aware facade used to handle benchmark's logs.
    /// </summary>
    public interface IBenchmarkOutput
    {
        /// <summary>
        /// Writes a new line with the log message, if the configuration enables it.
        /// </summary>
        /// <param name="message">The log message.</param>
        void WriteLine(string message);
    }
}
