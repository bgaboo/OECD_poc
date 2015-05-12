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
            PerfCounterTypes.Add(new StatsPerfCounter(this));
            PerfCounterTypes.Add(new ClientsPerfCounter(this));
            PerfCounterTypes.Add(new CpuPerfCounter(this));
            PerfCounterTypes.Add(new CommandstatsPerfCounter(this));
            PerfCounterTypes.Add(new KeyspacePerfCounter(this));

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

        public InfoResult RecordMetrics(InfoResult result)
        {
            InfoResult collectedMetrics = new InfoResult();
            foreach (InfoRegion region in result.Regions)
            {
                PerfCounterBase perfCounterType = PerfCounterTypes.FirstOrDefault(p => p.Name.ToUpper() == region.Name.ToUpper());
                if (perfCounterType != null)
                    RecordMetrics(region, perfCounterType, ref collectedMetrics);
            }
            return collectedMetrics;
        }

        private void RecordMetrics(InfoRegion region, PerfCounterBase perfCounterType, ref InfoResult collectedMetrics)
        {
            if (region == null)
                throw new ArgumentNullException("region");
            if (perfCounterType == null)
                throw new ArgumentNullException("perfCounterType");

            if (perfCounterType.Name.ToUpper() != region.Name.ToUpper())
                throw new ArgumentException("Argument mismatch!");

            List<Tuple<string, string>> collectedEntries = new List<Tuple<string, string>>();
            
            List<Tuple<Counter, Tuple<string, string>>> countersToBeRecorded = perfCounterType.GetCountersToBeRecorded(region);

            foreach (Tuple<Counter, Tuple<string, string>> counterToBeRecorded in countersToBeRecorded)
            {
                SaveMetric(perfCounterType, counterToBeRecorded.Item1, counterToBeRecorded.Item2);
                collectedEntries.Add(counterToBeRecorded.Item2);
            }

            InfoRegion collectedRegoin = new InfoRegion();
            collectedRegoin.Name = region.Name;
            collectedRegoin.Entries.AddRange(collectedEntries);
            collectedMetrics.Regions.Add(collectedRegoin);
        }

        private bool SaveMetric(PerfCounterBase perfCounterType, Counter counter, Tuple<string, string> entry)
        {
            bool retVal = false;

            try
            {
                PerformanceCounter perfCounter = perfCounterType.GetCounter(counter.Name);
                if (perfCounter != null)
                {
                    switch (perfCounter.CounterType)
                    {
                        case PerformanceCounterType.NumberOfItems64:
                            perfCounter.RawValue = Convert.ToInt64(entry.Item2);
                            retVal = true;
                            break;
                        //case PerformanceCounterType.RawFraction:
                        //    PerformanceCounter perfCounterBase = perfCounterType.GetBaseCounter(counter.Name);
                        //    if (perfCounterBase != null)
                        //    {
                        //        perfCounterBase.RawValue = 100;
                        //        perfCounter.RawValue = Convert.ToInt64(Convert.ToDecimal(entry.Item2) * 100);
                        //        retVal = true;
                        //    }
                        //    break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            catch
            {
                // DEVNOTE: log the exception but do not rethrow
            }
            return retVal;
        }
    }
}
