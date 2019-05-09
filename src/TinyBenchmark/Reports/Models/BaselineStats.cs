using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TinyBenchmark
{
    public class BaselineStats
    {
        public decimal Ratio { get; }

        public decimal Efficiency { get; }

        public TimeSpan AvgTimeDifference { get; }

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
