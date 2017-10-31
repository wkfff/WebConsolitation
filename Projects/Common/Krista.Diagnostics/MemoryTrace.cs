using System;
using System.Diagnostics;

namespace Krista.Diagnostics
{
    public static class MemoryTrace
    {
		private const string EventSource = "Krista.MemoryTrace";

        private static TraceSource source = new TraceSource(EventSource);

        private static long prevMem;

        public static void Trace(string label)
        {
            GC.Collect();
            long current = GC.GetTotalMemory(true);
            source.TraceEvent(TraceEventType.Verbose, 0, "C:{0} P:{1} D:{2} {3}", current, prevMem, current - prevMem, label);
            prevMem = current;
        }
    }
}
