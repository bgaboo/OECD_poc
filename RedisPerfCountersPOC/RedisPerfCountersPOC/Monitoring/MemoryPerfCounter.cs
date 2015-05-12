using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class MemoryPerfCounter : PerfCounterBase
    {
        //•	used_memory: total number of bytes allocated by Redis using its allocator (either standard libc, jemalloc, or an alternative allocator such as tcmalloc
        //•	used_memory_human: Human readable representation of previous value
        //•	used_memory_rss: Number of bytes that Redis allocated as seen by the operating system (a.k.a resident set size). This is the number reported by tools such as top and ps.
        //•	used_memory_peak: Peak memory consumed by Redis (in bytes)
        //•	used_memory_peak_human: Human readable representation of previous value
        //•	used_memory_lua: Number of bytes used by the Lua engine
        //•	mem_fragmentation_ratio: Ratio between used_memory_rss and used_memory

        private const string prefix = "Memory";

        public MemoryPerfCounter(PerformanceMonitor monitor) : base(monitor)
        {
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
            {
                //new Counter("used_memory", PerformanceCounterType.NumberOfItems64),
                new Counter("used_memory_rss", PerformanceCounterType.NumberOfItems64),
                new Counter("used_memory_peak", PerformanceCounterType.NumberOfItems64),
                //new Counter("mem_fragmentation_ratio", PerformanceCounterType.RawFraction)
                new Counter("mem_fragmentation_ratio", PerformanceCounterType.NumberOfItems64)
            };

            //_complementerCounters = new List<Counter>()
            //                                {
            //                                    new Counter("mem_fragmentation_ratio" + BaseCounterPostfix, PerformanceCounterType.RawBase)
            //                                };

        }

        internal override string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
