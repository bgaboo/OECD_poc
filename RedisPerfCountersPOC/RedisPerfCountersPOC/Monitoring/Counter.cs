using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class Counter
    {
        public string Name { get; internal set; }

        public PerformanceCounterType Type { get; internal set; }
        
        public Counter(string name, PerformanceCounterType type)
        {
            Name = name;
            Type = type;
        }
    }
}
