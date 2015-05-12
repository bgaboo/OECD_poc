using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class StatsPerfCounter : PerfCounterBase
    {
        private const string prefix = "Stats";

        public StatsPerfCounter(PerformanceMonitor monitor) : base(monitor)
        {
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
            {
                new Counter("expired_keys", PerformanceCounterType.NumberOfItems64),
                new Counter("evicted_keys", PerformanceCounterType.NumberOfItems64),
                new Counter("instantaneous_ops_per_sec", PerformanceCounterType.NumberOfItems64),
                new Counter("rejected_connections", PerformanceCounterType.NumberOfItems64),
                new Counter("keyspace_misses", PerformanceCounterType.NumberOfItems64),
                new Counter("keyspace_hits", PerformanceCounterType.NumberOfItems64)
            };
        }

        internal override string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
