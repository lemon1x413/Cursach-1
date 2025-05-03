using System;
using System.Diagnostics;

namespace Cursach_1
{
    public static class ComplexityMeter
    {
        public static long Measure(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }
    }
}