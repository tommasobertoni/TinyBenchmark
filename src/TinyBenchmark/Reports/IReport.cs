using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// Defines a report that can be exported.
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// Accepts an exporter.
        /// </summary>
        /// <param name="exporter">The exporter.</param>
        void Accept(IExporter exporter);
    }
}
