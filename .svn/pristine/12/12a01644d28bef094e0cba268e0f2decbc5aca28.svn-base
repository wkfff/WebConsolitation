using System;
using System.Diagnostics;

namespace Krista.FM.Common.IoC.Logging
{
    [Serializable]
    public class LogAttributeSettings
    {
        public LogAttributeSettings()
            : this(TraceLevel.Off, TraceLevel.Off, TraceLevel.Off, 0) { }

        public LogAttributeSettings(TraceLevel entryLevel, TraceLevel successLevel, TraceLevel exceptionLevel, int order)
        {
            Order = order;
            EntryLevel = entryLevel;
            SuccessLevel = successLevel;
            ExceptionLevel = exceptionLevel;
        }

        public int Order { get; set; }
        public TraceLevel EntryLevel { get; set; }
        public TraceLevel SuccessLevel { get; set; }
        public TraceLevel ExceptionLevel { get; set; }
    }
}
