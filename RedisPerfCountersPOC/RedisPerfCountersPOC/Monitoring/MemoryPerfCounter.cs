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
        
        private PerformanceMonitor Monitor { get; set; }

        private List<PerformanceCounter> PerformanceCounters { get; set; }

        public MemoryPerfCounter(PerformanceMonitor monitor)
        {
            Monitor = monitor;
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
                                            {
                                                new Counter("used_memory", PerformanceCounterType.NumberOfItems64),
                                                new Counter("used_memory_rss", PerformanceCounterType.NumberOfItems64),
                                                new Counter("used_memory_peak", PerformanceCounterType.NumberOfItems64),
                                                //new Counter("mem_fragmentation_ratio", PerformanceCounterType.RawFraction)
                                                new Counter("mem_fragmentation_ratio", PerformanceCounterType.NumberOfItems64)
                                            };

            ////_complementerCounters = new List<Counter>()
            ////                                {
            ////                                    new Counter("mem_fragmentation_ratio" + BaseCounterPostfix, PerformanceCounterType.RawBase)
            ////                                };

        }

        public override PerformanceCounter GetCounter(string counterName)
        {
            PerformanceCounter counter = null;
            if (PerformanceCounters.Any(p => p.CounterName == GetCounterName(counterName)))
            {
                counter = PerformanceCounters.FirstOrDefault(p => p.CounterName == GetCounterName(counterName));
            }
            else
            {
                counter = new PerformanceCounter(Monitor.Category, GetCounterName(counterName), false);
            }
            return counter;
        }

        public override PerformanceCounter GetBaseCounter(string counterName)
        {
            return GetCounter(counterName + BaseCounterPostfix);
        }

        public override CounterCreationDataCollection InitCounters()
        {
            CounterCreationDataCollection perfCounters = new CounterCreationDataCollection();

            foreach (Counter c in CountersRecorded)
            {
                CounterCreationData counter = new CounterCreationData(GetCounterName(c.Name), GetCounterHelp(c.Name), c.Type);
                perfCounters.Add(counter);
            }

            foreach (Counter c in _complementerCounters)
            {
                CounterCreationData counter = new CounterCreationData(GetCounterName(c.Name), GetCounterHelp(c.Name), c.Type);
                perfCounters.Add(counter);
            }

            return perfCounters;
        }

        public string GetCounterName(string counterName)
        {
            return string.Format("{0} {1}", prefix, counterName);
        }
        private string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
