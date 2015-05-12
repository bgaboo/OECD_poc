using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC.Monitoring
{
    public class KeyspacePerfCounter : PerfCounterBase
    {
        private const string prefix = "Keyspace";

        public KeyspacePerfCounter(PerformanceMonitor monitor)
            : base(monitor)
        {
            Name = prefix;
            PerformanceCounters = new List<PerformanceCounter>();

            _countersRecorded = new List<Counter>()
            {
                new Counter("keys", PerformanceCounterType.NumberOfItems64),
                new Counter("keys_to_expire", PerformanceCounterType.NumberOfItems64)
            };
        }

        public override List<Tuple<Counter, Tuple<string, string>>> GetCountersToBeRecorded(InfoRegion region)
        {
            // Sample: db0:keys=2,expires=2,avg_ttl=132670
            // Sample structure: 'db name: key count, count of keys to be expire, the average ttl of key expiration'
            // DEVNOTE: we only need the key count and count of keys to be expire

            const string counterDelimiter = "=";
            const string keysCounter = "keys";
            const string expiresCounter = "expires";

            List<Tuple<Counter, Tuple<string, string>>> retVal = new List<Tuple<Counter, Tuple<string, string>>>();

            foreach (Counter counter in CountersRecorded)
            {
                switch (counter.Name.ToLower())
                {
                    case "keys":
                        Dictionary<string, Tuple<string, string>> dbKeys = CollectKeys(region, keysCounter, counterDelimiter, counter.Name);
                        
                        break;
                    case "keys_to_expire":
                        Dictionary<string, Tuple<string, string>> dbExpireKeys = CollectKeys(region, expiresCounter, counterDelimiter, counter.Name);
                        break;
                    default:
                        break;
                }
                
                //retVal.Add(new Tuple<Counter, Tuple<string, string>>(counter, new Tuple<string, string>(counter.Name, counterValue.ToString())));
            }

            return retVal;
        }

        private Dictionary<string, Tuple<string, string>> CollectKeys(InfoRegion region, string subCounterName, string counterDelimiter, string counterNameRecorded)
        {
            Dictionary<string, Tuple<string, string>> dbKeys = new Dictionary<string, Tuple<string, string>>();

            foreach (var entry in region.Entries)
            {
                string[] dbEntryNodes = entry.Item2.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                string keyNode = dbEntryNodes.FirstOrDefault(n => n.StartsWith(subCounterName + counterDelimiter));
                if (keyNode != null)
                {
                    string value = keyNode.Split(new string[] { counterDelimiter }, StringSplitOptions.RemoveEmptyEntries).ElementAt(1);
                    dbKeys.Add(entry.Item1, new Tuple<string,string>(counterNameRecorded, value));
                }
            }

            return dbKeys;
        }
        
        internal override string GetCounterHelp(string counterName)
        {
            // DEVNOTE: it should come from resource
            return "counter help";
        }
    }
}
