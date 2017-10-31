using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Krista.FM.Common.IoC.Logging;
using Krista.FM.Extensions;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Common
{
    public class LogInterceptor : ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var assemblyLogAttributes =
                (LogAttribute[])input.MethodBase.ReflectedType.Assembly.GetCustomAttributes(typeof(LogAttribute), false);
            var classLogAttributes =
                (LogAttribute[])input.MethodBase.ReflectedType.GetCustomAttributes(typeof(LogAttribute), false);
            var methodLogAttributes =
                (LogAttribute[])input.MethodBase.GetCustomAttributes(typeof(LogAttribute), false);

            if (assemblyLogAttributes.Length == 0 && classLogAttributes.Length == 0 && methodLogAttributes.Length == 0)
            {
                return getNext()(input, getNext);
            }

            LogAttributeSettings logAttributeSettings = GetLoggingLevels(assemblyLogAttributes, classLogAttributes, methodLogAttributes);

            OnEnter(input, logAttributeSettings.EntryLevel);

            // Perform the operation
            var methodReturn = getNext()(input, getNext);
            
            // Method failed, go ahead
            if (methodReturn.Exception != null)
            {
                OnException(input, methodReturn.Exception, logAttributeSettings.ExceptionLevel);

                return methodReturn;
            }

            OnSuccess(input, methodReturn, logAttributeSettings.SuccessLevel);

            return methodReturn;
        }

        private static LogAttributeSettings GetLoggingLevels(
            IEnumerable<LogAttribute> assemblyLogAttributes,
            IEnumerable<LogAttribute> classLogAttributes, 
            IEnumerable<LogAttribute> methodLogAttributes)
        {
            var logAttributeSettings = new LogAttributeSettings();
            logAttributeSettings = GetLoggingLevels(assemblyLogAttributes, logAttributeSettings);
            logAttributeSettings = GetLoggingLevels(classLogAttributes, logAttributeSettings);
            logAttributeSettings = GetLoggingLevels(methodLogAttributes, logAttributeSettings);

            return logAttributeSettings;
        }

        private static LogAttributeSettings GetLoggingLevels(IEnumerable<LogAttribute> logAttributes, LogAttributeSettings logAttributeSettings)
        {
            foreach (LogAttribute logAttribute in logAttributes)
            {
                if (logAttribute.Settings.EntryLevel > logAttributeSettings.EntryLevel)
                {
                    logAttributeSettings.EntryLevel = logAttribute.Settings.EntryLevel;
                }

                if (logAttribute.Settings.SuccessLevel > logAttributeSettings.SuccessLevel)
                {
                    logAttributeSettings.SuccessLevel = logAttribute.Settings.SuccessLevel;
                }

                if (logAttribute.Settings.ExceptionLevel > logAttributeSettings.ExceptionLevel)
                {
                    logAttributeSettings.ExceptionLevel = logAttribute.Settings.ExceptionLevel;
                }
            }

            return logAttributeSettings;
        }

        private static void OnEnter(IMethodInvocation input, TraceLevel traceLevel)
        {
            var logMessage = new StringBuilder();
            logMessage.Append(string.Format("{0}(", input.MethodBase.Name));

            ParameterInfo[] parameterInfos = input.MethodBase.GetParameters();
            if (input.Arguments.Count > 0 && parameterInfos != null)
            {
                for (int i = 0; i < input.Arguments.Count; i++)
                {
                    if (i > 0)
                    {
                        logMessage.Append(" ");
                    }

                    logMessage.Append(string.Format("{0}:[{1}]", parameterInfos[i].Name, input.Arguments[i]));
                }
            }
            logMessage.Append(")");

            LogEnter(traceLevel, logMessage.ToString());
        }

        private static void OnSuccess(IMethodInvocation input, IMethodReturn methodReturn, TraceLevel traceLevel)
        {
            LogSuccess(
                traceLevel, 
                "{0} Returns:[{1}]", 
                input.MethodBase.Name,
                methodReturn.ReturnValue != null ? methodReturn.ReturnValue.ToString() : "");
        }

        private static void OnException(IMethodInvocation input, Exception exception, TraceLevel traceLevel)
        {
            LogException(traceLevel, "{0} ThrowException:[{1}]", input.MethodBase.Name, exception.ExpandException());
        }

        private static void LogEnter(TraceLevel traceLevel, string format, params object[] args)
        {
            if (traceLevel != TraceLevel.Off)
            {
                Trace.TraceEvent(ConverTraceLevel2EventType(traceLevel), format, args);
            }
        }

        private static void LogSuccess(TraceLevel traceLevel, string format, params object[] args)
        {
            if (traceLevel != TraceLevel.Off)
            {
                Trace.TraceEvent(ConverTraceLevel2EventType(traceLevel), format, args);
            }
        }
        
        private static void LogException(TraceLevel traceLevel, string format, params object[] args)
        {
            if (traceLevel != TraceLevel.Off)
            {
                Trace.TraceEvent(ConverTraceLevel2EventType(traceLevel), format, args);
            }
        }

        private static TraceEventType ConverTraceLevel2EventType(TraceLevel traceLevel)
        {
            switch (traceLevel)
            {
                case TraceLevel.Error:
                    return TraceEventType.Error;
                case TraceLevel.Info:
                    return TraceEventType.Information;
                case TraceLevel.Verbose:
                    return TraceEventType.Verbose;
                case TraceLevel.Warning:
                    return TraceEventType.Warning;
                default:
                    return TraceEventType.Verbose;
            }
        }
    }
}
