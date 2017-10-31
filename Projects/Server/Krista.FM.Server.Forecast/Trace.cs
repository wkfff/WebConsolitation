using System;
using System.Collections.Generic;
using System.Text;

using Krista.Diagnostics;

namespace Krista.FM.Server.Forecast
{
    /// <summary>
    /// ������������� ����� ������� � ������� ��� ����������� ���������.
    /// </summary>
    internal sealed class Trace
    {
        /// <summary>
        /// ��� ���������.
        /// </summary>
        private static string sourceName = "Krista.FM.Server.Forecast";

        /// <summary>
        /// �������������� ��������� ������� ������� ������� �������� �����
        /// </summary>
        /// <param name="eventType">��� ���������</param>
        /// <param name="format">������ ������� ���������</param>
        /// <param name="args">��������� �������</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceEvent(System.Diagnostics.TraceEventType eventType, string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(eventType, sourceName, format, args);
        }

        /// <summary>
        /// �������������� ��������� ������� ������� ������� �������� �����
        /// </summary>
        /// <param name="format">������ ������� ���������</param>
        /// <param name="args">��������� �������</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceCritical(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Critical, sourceName, format, args);
        }

        /// <summary>
        /// �������������� ��������� ������� ������� ������� �������� �����
        /// </summary>
        /// <param name="format">������ ������� ���������</param>
        /// <param name="args">��������� �������</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Error, sourceName, format, args);
        }

        /// <summary>
        /// �������������� ��������� ������� ������� ������� �������� �����
        /// </summary>
        /// <param name="format">������ ������� ���������</param>
        /// <param name="args">��������� �������</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Warning, sourceName, format, args);
        }

        /// <summary>
        /// �������������� ��������� ������� ������� ������� �������� �����
        /// </summary>
        /// <param name="format">������ ������� ���������</param>
        /// <param name="args">��������� �������</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Information, sourceName, format, args);
        }

        /// <summary>
        /// �������������� ��������� ������� ������� ������� �������� �����
        /// </summary>
        /// <param name="format">������ ������� ���������</param>
        /// <param name="args">��������� �������</param>
        [System.Diagnostics.Conditional("TRACE")]
        public static void TraceVerbose(string format, params object[] args)
        {
            KristaDiagnostics.TraceEvent(System.Diagnostics.TraceEventType.Verbose, sourceName, format, args);
        }

        /// <summary>
        /// ����������� ������� �������
        /// </summary>
        [System.Diagnostics.Conditional("TRACE")]
        public static void Indent()
        {
            KristaDiagnostics.Indent();
        }

        /// <summary>
        /// ��������� ������� �������
        /// </summary>
        [System.Diagnostics.Conditional("TRACE")]
        public static void Unindent()
        {
            KristaDiagnostics.Unindent();
        }

        //
        // Summary:
        //     Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        //     collection.
        //
        // Parameters:
        //   message:
        //     A message to write.
        [System.Diagnostics.Conditional("TRACE")]
        public static void Write(string message)
        {
            KristaTrace.TraceInformation(sourceName, message);
        }
        //
        // Summary:
        //     Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        //     collection.
        //
        // Parameters:
        //   message:
        //     A message to write.
        [System.Diagnostics.Conditional("TRACE")]
        public static void WriteLine(string message)
        {
            KristaTrace.TraceInformation(sourceName, message);
        }
    }
}
