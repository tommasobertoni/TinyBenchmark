using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    /// <summary>
    /// The comparison data against the baseline benchmark's results.
    /// </summary>
    public class BaselineStats
    {
        /// <summary>
        /// duration / baseline duration
        /// </summary>
        public decimal Ratio { get; }

        /// <summary>
        /// baseline duration / duration
        /// </summary>
        public decimal Efficiency { get; }

        /// <summary>
        /// baseline duration - duration
        /// </summary>
        public TimeSpan AvgTimeDifference { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ratio">duration / baseline duration</param>
        /// <param name="efficiency">baseline duration / duration</param>
        /// <param name="avgTimeDifference">baseline duration - duration</param>
        public BaselineStats(
            decimal ratio,
            decimal efficiency,
            TimeSpan avgTimeDifference)
        {
            this.Ratio = ratio;
            this.Efficiency = efficiency;
            this.AvgTimeDifference = avgTimeDifference;
        }
    }
}
