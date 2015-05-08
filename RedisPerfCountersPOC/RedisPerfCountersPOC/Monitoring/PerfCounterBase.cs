using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class PerfCounterBase
    {
        internal List<Counter> _countersRecorded = new List<Counter>();

        public List<Counter> CountersRecorded { get { return _countersRecorded; } }

        public List<PerformanceCounter> PerfCounters { get; internal set; }

        public string Name { get; internal set; }

        public PerfCounterBase()
        {

        }

        public virtual CounterCreationDataCollection InitCounters()
        {

        }

        public virtual PerformanceCounter GetCounter(string counterName)
        {
            
        }

    }
}
