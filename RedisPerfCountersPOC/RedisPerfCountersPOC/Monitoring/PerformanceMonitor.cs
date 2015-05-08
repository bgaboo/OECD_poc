using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class PerformanceMonitor
    {
        private CounterCreationDataCollection _counters = new CounterCreationDataCollection();
        private string _category = string.Empty;

        // DEVNOTE: should come from resource
        private string _categoryHelp = "Redis category help";

        public string Category
        {
            get { return _category; }
        }

        public List<PerfCounterBase> PerfCounterTypes { get; internal set; }
        //public MemoryPerfCounter Memory { get; internal set; }

        public PerformanceMonitor(string categoryName) : this(categoryName, false)
        {
        }

        public PerformanceMonitor(string categoryName, bool forceRecreate)
        {
            this._category = categoryName;

            PerfCounterTypes = new List<PerfCounterBase>();
            PerfCounterTypes.Add(new MemoryPerfCounter(this));

            CreateCategory(forceRecreate);
        }

        private void CreateCategory(bool forceRecreate)
        {
            if (!PerformanceCounterCategory.Exists(_category))
            {
                PerformanceCounterCategory.Create(_category, _categoryHelp, PerformanceCounterCategoryType.Unknown,
                    InitCounters());
            }
            else if (forceRecreate)
            {
                PerformanceCounterCategory.Delete(_category);
                PerformanceCounterCategory.Create(_category, _categoryHelp, PerformanceCounterCategoryType.Unknown,
                    InitCounters());
            }
        }

        private CounterCreationDataCollection InitCounters()
        {
            CounterCreationDataCollection perfCounters = new CounterCreationDataCollection();
            foreach (PerfCounterBase perfCounterType in PerfCounterTypes)
            {
                perfCounters.AddRange(perfCounterType.InitCounters());
            }
            return perfCounters;
        }

        public PerformanceCounter GetCounter(string counterName)
        {
            PerformanceCounter counter = new PerformanceCounter(_category, counterName);
            return counter;
        }

        public void RecordMetrics(InfoResult result)
        {
            foreach (InfoRegion region in result.Regions)
            {
                RecordMetrics(region, PerfCounterTypes.FirstOrDefault(p => p.Name.ToUpper() == region.Name.ToUpper()));
            }
        }

        private void RecordMetrics(InfoRegion region, PerfCounterBase perfCounterType)
        {
            if (region == null)
                throw new ArgumentNullException("region");
            if (perfCounterType == null)
                throw new ArgumentNullException("perfCounterType");

            if (perfCounterType.Name.ToUpper() == region.Name.ToUpper())
                throw new ArgumentException("Argument mismatch!");

            foreach (Counter counter in perfCounterType.CountersRecorded)
            {
                Tuple<string, string> entry = region.Entries.FirstOrDefault(e => e.Item1.ToUpper() == counter.Name.ToUpper());
                if (entry != null)
                {
                    SaveMetric(perfCounterType, counter, entry);
                }
            }
        }

        private bool SaveMetric(PerfCounterBase perfCounterType, Counter counter, Tuple<string, string> entry)
        {
            bool retVal = false;

            PerformanceCounter perfCounter = perfCounterType.GetCounter(counter.Name);
            if (perfCounter != null)
            {
                perfCounter.RawValue = Convert.ToInt64(entry.Item2);
                retVal = true;
            }

            return retVal;
        }
    }
}
