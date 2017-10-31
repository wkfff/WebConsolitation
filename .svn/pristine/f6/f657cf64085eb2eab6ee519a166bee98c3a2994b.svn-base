using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Комплексный фильтр трассировочных сообщений.
    /// Строка инициализации:
    /// initializeData="TYPE=Error,Verbose;MESSAGE=(.*Lic.*)|(.*при.*)"
    /// Ограничение по типу сообщения и по содержанию сообщения объединены операцией И
    /// </summary>
    public class ComplexTraceFilter : TraceFilter
    {
        private readonly Regex filterRegex;
        private readonly List<TraceEventType> eventTypes = new List<TraceEventType>();

        /// <summary>
        /// Коструктор со строкой инициализации вида:
        /// TYPE=Error,Verbose;MESSAGE=(.*Lic.*)|(.*при.*)
        /// </summary>
        /// <param name="сonfig">Строка инициализации</param>
        public ComplexTraceFilter(string сonfig)
        {
            foreach (string configRow in сonfig.Split(';'))
            {
                string[] configRowItems = configRow.Split('=');
                if (configRowItems.Length == 2)
                {
                    if (configRowItems[0].ToUpper() == "MESSAGE")
                    {
                        try
                        {
                            filterRegex = new Regex(configRowItems[1]);
                        }
                        catch
                        {
                            filterRegex = null;
                        }
                    }
                    if (configRowItems[0].ToUpper() == "TYPE")
                    {
                        foreach (string eventTypeStr in configRowItems[1].Split(','))
                        {
                            try
                            {
                                eventTypes.Add((TraceEventType)Enum.Parse(typeof(TraceEventType), eventTypeStr));
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
        }
		
        /// <summary>
        /// Базовый конструктор
        /// </summary>
        public ComplexTraceFilter()
        {
        }
		
        /// <summary>
        /// Определяет, должно ли сообщение быть выведено в трассировку
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="source"></param>
        /// <param name="eventType"></param>
        /// <param name="id"></param>
        /// <param name="formatOrMessage"></param>
        /// <param name="args"></param>
        /// <param name="data1"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
        {
            try
            {
                return ((eventTypes.Count == 0 || eventTypes.Contains(eventType)) && ((filterRegex == null) || filterRegex.Match(formatOrMessage).Success));
            }
            catch
            {
                return true;
            }
        }
    }
}