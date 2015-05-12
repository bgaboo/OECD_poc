using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisPerfCountersPOC.External;
using RedisPerfCountersPOC.Monitoring;

namespace RedisPerfCountersPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateTestCategory();
            InfoResult result =  ExternalCommand.CallRedis();

            PerformanceMonitor monitor = new PerformanceMonitor("Redis Category", true);
            InfoResult collectedMetrics = monitor.RecordMetrics(result);

            Visualize(collectedMetrics);

            Console.ReadLine();
        }

        private static void Visualize(InfoResult result)
        {
            foreach (InfoRegion region in result.Regions)
            {
                Console.WriteLine("{0}", region.Name);
                foreach (Tuple<string, string> entry in region.Entries)
                {
                    Console.WriteLine("\t{0}\t\t{1}", entry.Item1, entry.Item2);
                }
            }
        }

        private static void CreateTestCategory()
        {
            PerformanceMonitor monitor = new PerformanceMonitor("Redis Category", true);

            //PerformanceCounter ctr = monitor.GetCounter("counter1");

            PerformanceCounterCategory[] categories = PerformanceCounterCategory.GetCategories();

            PerformanceCounterCategory cat = categories.Where(c => c.CategoryName == "Redis Category").FirstOrDefault();
        }
    }
}
