using System;
using System.Diagnostics;

namespace Mercraft.Maps.UnitTests
{
    /// <summary>
    ///     Helper class for performance measurement
    /// </summary>
    public class PerformanceLogger
    {
        /// <summary>
        ///     Holds the ticks when started.
        /// </summary>
        private long? _ticks;

        /// <summary>
        ///     Holds the amount of memory before start.
        /// </summary>
        private long? _memory;

        /// <summary>
        ///     Reports the start of the process/time period to measure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Start()
        {
            GC.Collect();

            Process p = Process.GetCurrentProcess();
            _memory = p.PrivateMemorySize64;
            _ticks = DateTime.Now.Ticks;
            Console.WriteLine("Started at {0}.", new DateTime(_ticks.Value).ToLongTimeString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Report(string message)
        {
            Seconds = new TimeSpan(DateTime.Now.Ticks - _ticks.Value).TotalMilliseconds/1000.0;

            Process p = Process.GetCurrentProcess();
            Memory = Math.Round((p.PrivateMemorySize64 - _memory.Value)/1024.0/1024.0, 4);

            Console.WriteLine("{0}: consume {1}s and {2}MB of memory diff.", message, Seconds, Memory);
        }

        /// <summary>
        ///     Reports the end of the process/time period to measure.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void Stop()
        {
            if (!_ticks.HasValue)
                return;

            Seconds = new TimeSpan(DateTime.Now.Ticks - _ticks.Value).TotalMilliseconds/1000.0;

            GC.Collect();
            Process p = Process.GetCurrentProcess();
            Memory = System.Math.Round((p.PrivateMemorySize64 - _memory.Value)/1024.0/1024.0, 4);

            Console.WriteLine("Ended at at {0}, spent {1}s and {2}MB of memory diff.",
                DateTime.Now.ToLongTimeString(),
                Seconds, Memory);
        }

        public double Seconds { get; set; }
        public double Memory { get; set; }
    }
}