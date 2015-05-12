using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class CommandstatsPerfCounter : PerfCounterBase
    {
        private const string prefix = "Commandstats";

        internal List<Counter> _baseCounters = new List<Counter>();

        public CommandstatsPerfCounter(PerformanceMonitor monitor)
            : base(monitor)
        {
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
            {
                new Counter("cmdstat_get", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_set", PerformanceCounterType.NumberOfItems64),
            };

            _baseCounters = new List<Counter>()
            {
                new Counter("cmdstat_get", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_hget", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_hgetall", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_hmget", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_mget", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_getbit", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_getrange", PerformanceCounterType.NumberOfItems64),

                new Counter("cmdstat_set", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_hset", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_hmset", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_hsetnx", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_lset", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_mset", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_msetnx", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_setbit", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_setex", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_setrange", PerformanceCounterType.NumberOfItems64),
                new Counter("cmdstat_setnx", PerformanceCounterType.NumberOfItems64),
            };
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

        public override List<Tuple<Counter, Tuple<string, string>>> GetCountersToBeRecorded(InfoRegion region)
        {
            // Sample: cmdstat_get:calls=30,usec=4882,usec_per_call=162.73
            // Sample structure: 'command: the number of calls, the total CPU time consumed by these commands, the average CPU consumed per command execution'
            // DEVNOTE: we only need the number of calls metric

            const string counterNameDelimiter = "_";

            List<Tuple<Counter, Tuple<string, string>>> retVal = new List<Tuple<Counter, Tuple<string, string>>>();

            foreach (Counter counter in CountersRecorded)
            {
                // collect base entries from which the recorded entry will be computed

                long counterValue = 0;
                string counterTypeName = counter.Name.Substring(counter.Name.IndexOf(counterNameDelimiter) + 1);

                IEnumerable<Counter> baseCountersForType = _baseCounters.Where(bc => bc.Name.Substring(bc.Name.IndexOf(counterNameDelimiter) + 1).Contains(counterTypeName));

                foreach (Counter baseCounter in baseCountersForType)
                {
                    Tuple<string, string> entry = region.Entries.FirstOrDefault(e => e.Item1.ToUpper() == baseCounter.Name.ToUpper());
                    if (entry != null)
                    {
                        string subEntry = entry.Item2.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(s => s.StartsWith("calls"));
                        if (subEntry != null)
                        {
                            string subValue = subEntry.Substring(subEntry.IndexOf("=") + 1);
                            long value = 0;
                            if (Int64.TryParse(subValue, out value))
                                counterValue += value;
                        }
                    }
                }

                retVal.Add(new Tuple<Counter, Tuple<string, string>>(counter, new Tuple<string, string>(counter.Name, counterValue.ToString())));
            }

            return retVal;
        }

        internal override string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
