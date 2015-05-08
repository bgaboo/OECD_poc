using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisPerfCountersPOC
{
    public class InfoRegion
    {
        public string Name { get; internal set; }

        public List<Tuple<string, string>> Entries = new List<Tuple<string, string>>();

        public InfoRegion(string region)
        {
            int indexOfCrLf = region.IndexOf("\r\n");
            string regionName = region.Substring(0, indexOfCrLf).Trim();
            string regionData = region.Substring(indexOfCrLf);

            string[] entries = regionData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string entry in entries)
            {
                int indexOfColon = entry.IndexOf(":");
                string entryName = entry.Substring(0, indexOfColon).Trim();
                string entryData = entry.Substring(indexOfColon + 1);

                Entries.Add(new Tuple<string, string>(entryName, entryData));
            }

            Name = regionName;
        }
    }
}
