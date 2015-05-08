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
        public const string BaseCounterPostfix = "_base";

        internal List<Counter> _countersRecorded = new List<Counter>();
        internal List<Counter> _complementerCounters = new List<Counter>();

        public List<Counter> CountersRecorded { get { return _countersRecorded; } }

        public List<PerformanceCounter> PerfCounters { get; internal set; }

        public string Name { get; internal set; }

        public PerfCounterBase()
        {

        }

        public virtual CounterCreationDataCollection InitCounters()
        {
            throw new NotImplementedException();
        }

        public virtual PerformanceCounter GetCounter(string counterName)
        {
            throw new NotImplementedException();
        }

        public virtual PerformanceCounter GetBaseCounter(string counterName)
        {
            throw new NotImplementedException();
        }
    }
}
