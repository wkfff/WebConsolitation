using System;
using System.Diagnostics;
using Krista.FM.Common.IoC.Logging;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Common
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LogAttribute : Attribute
    {
        public LogAttributeSettings Settings { get; set; }

        public int Order
        {
            get { return Settings.Order; }
            set { Settings.Order = value; }
        }

        public TraceLevel EntryLevel
        {
            get { return Settings.EntryLevel; }
            set { Settings.EntryLevel = value; }
        }

        public TraceLevel SuccessLevel
        {
            get { return Settings.SuccessLevel; }
            set { Settings.SuccessLevel = value; }
        }

        public TraceLevel ExceptionLevel
        {
            get { return Settings.ExceptionLevel; }
            set { Settings.ExceptionLevel = value; }
        }
        
        public LogAttribute()
        {
            Settings = new LogAttributeSettings(TraceLevel.Verbose, TraceLevel.Verbose, TraceLevel.Error, 0);
        }
        
        public ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogInterceptor { Order = Order };
        }
    }
}
