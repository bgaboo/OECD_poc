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

            PerformanceMonitor monitor = new PerformanceMonitor("Redis Category");
            monitor.RecordMetrics(result);
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
