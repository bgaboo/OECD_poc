using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC
{
    public class InfoResult
    {
        private string result = string.Empty;

        public List<InfoRegion> Regions = new List<InfoRegion>();

        public InfoResult()
        {
        }

        public InfoResult(string result)
        {
            this.result = result;

            Parse(result);
        }

        //•	server: General information about the Redis server
        //•	clients: Client connections section
        //•	memory: Memory consumption related information
        //•	persistence: RDB and AOF related information
        //•	stats: General statistics
        //•	replication: Master/slave replication information
        //•	cpu: CPU consumption statistics
        //•	commandstats: Redis command statistics
        //•	cluster: Redis Cluster section
        //•	keyspace: Database related statistics

        private void Parse(string result)
        {
            string[] regions = result.Split(new string[]{ "#" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string regionData in regions)
            {
                InfoRegion infoRegion = new InfoRegion(regionData);

                if (infoRegion.Entries.Count > 0)
                    Regions.Add(infoRegion);
            }
        }
    }
}
