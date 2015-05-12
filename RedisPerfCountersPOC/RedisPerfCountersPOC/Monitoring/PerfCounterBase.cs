using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public abstract class PerfCounterBase
    {
        public const string BaseCounterPostfix = "_base";

        internal List<Counter> _countersRecorded = new List<Counter>();
        internal List<Counter> _complementerCounters = new List<Counter>();

        internal PerformanceMonitor Monitor { get; set; }

        internal List<PerformanceCounter> PerformanceCounters { get; set; }

        public List<Counter> CountersRecorded { get { return _countersRecorded; } }
        
        public string Name { get; internal set; }

        public PerfCounterBase(PerformanceMonitor monitor)
        {
            Monitor = monitor;
        }

        public virtual PerformanceCounter GetCounter(string counterName)
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

        public virtual PerformanceCounter GetBaseCounter(string counterName)
        {
            return GetCounter(counterName + BaseCounterPostfix);
        }

        public virtual CounterCreationDataCollection InitCounters()
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

        public virtual List<Tuple<Counter, Tuple<string, string>>> GetCountersToBeRecorded(InfoRegion region)
        {
            List<Tuple<Counter, Tuple<string, string>>> retVal = new List<Tuple<Counter, Tuple<string, string>>>();

            foreach (Counter counter in CountersRecorded)
            {
                Tuple<string, string> entry = region.Entries.FirstOrDefault(e => e.Item1.ToUpper() == counter.Name.ToUpper());
                if (entry != null)
                {
                    retVal.Add(new Tuple<Counter, Tuple<string, string>>(counter, entry));
                }
            }

            return retVal;
        }

        public virtual string GetCounterName(string counterName)
        {
            return string.Format("{0} {1}", Name, counterName);
        }
        internal virtual string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            throw new NotImplementedException();
        }
    }
}
