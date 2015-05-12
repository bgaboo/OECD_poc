using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class CpuPerfCounter : PerfCounterBase
    {
        private const string prefix = "Cpu";

        public CpuPerfCounter(PerformanceMonitor monitor)
            : base(monitor)
        {
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
            {
                new Counter("used_cpu_sys", PerformanceCounterType.NumberOfItems64),
                new Counter("used_cpu_user_children", PerformanceCounterType.NumberOfItems64)
            };
        }

        internal override string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
