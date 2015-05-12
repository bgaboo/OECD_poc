using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class ClientsPerfCounter : PerfCounterBase
    {
        private const string prefix = "Clients";

        public ClientsPerfCounter(PerformanceMonitor monitor) : base(monitor)
        {
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
            {
                new Counter("connected_clients", PerformanceCounterType.NumberOfItems64),
                new Counter("blocked_clients", PerformanceCounterType.NumberOfItems64)
            };
        }

        internal override string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
