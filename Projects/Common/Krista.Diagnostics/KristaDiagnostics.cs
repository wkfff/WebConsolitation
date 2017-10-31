using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Krista.Diagnostics
{
    /// <summary>
    /// Протоколизатор
    /// </summary>
    public class KristaDiagnostics
    {
        private static int indentLevel;

        private static readonly Dictionary<string, TraceSource> TraceSourceCollection = new Dictionary<string, TraceSource>();
        
        /// <summary>
        /// Получение источника трассировки.
        /// </summary>
        /// <param name="sourceName">Имя источника трассировки.</param>
        /// <returns>Источник трассировки.</returns>
        public static TraceSource GetTraceSource(string sourceName)
        {
            try
            {
                TraceSource traceSource;
                lock (TraceSourceCollection)
                {
                    if (TraceSourceCollection.ContainsKey(sourceName))
                    {
                        traceSource = TraceSourceCollection[sourceName];
                    }
                    else
                    {
                        traceSource = new TraceSource(sourceName);
                        TraceSourceCollection.Add(sourceName, traceSource);
                    }
                }
                return traceSource;
            }
            catch (ArgumentException)
            {
                Debug.Assert(false, "Ошибка создания трассировщика сообщения");
                return null;
            }
        }
        
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        public static void TraceEvent(TraceEventType eventType, string source, string message)
        {
            TraceSource myTraceSource = GetTraceSource(source);
            if (myTraceSource != null)
            {
                foreach (TraceListener listener in myTraceSource.Listeners)
                {
                    listener.IndentLevel = indentLevel;
                }
                myTraceSource.TraceEvent(eventType, 0, message);
                myTraceSource.Flush();
            }
        }
        
        /// <summary>
        /// Вывод сообщения в протокол
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void TraceEvent(TraceEventType eventType, string source, string format, params object[] args)
        {
            if (args != null)
            {
                TraceEvent(eventType, source, string.Format(format, args));
            }
            else
            {
                TraceEvent(eventType, source, format);
            }
        }
        
        /// <summary>
        /// Разворачивает содержание исколючительной ситуации в нестролько строк
        /// с учетом InnerException
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ExpandException(Exception e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Exception scannedExeption = e;
            Exception ex = new Exception(e.Message, e.InnerException);

            BooleanSwitch stackTraceSwitch = new BooleanSwitch("StackTrace", "", "true");

            while (scannedExeption != null)
            {
                try
                {
                    // Для удобочитаемоси выделяем тип исключения
                    stringBuilder.Append("---------- ");
                    stringBuilder.Append(scannedExeption.GetType().ToString());
                    stringBuilder.AppendLine(" ----------");

                    PropertyInfo[] props = scannedExeption.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                    foreach (PropertyInfo pi in props)
                    {
                        // Выводим значения не только строковых полей, но и знечения других полезных свойств-значений.
                        if (pi.PropertyType.IsAssignableFrom(typeof(string)) || pi.PropertyType.IsValueType)
                        {
                            try
                            {
                                if (stackTraceSwitch.Enabled || (pi.Name != "StackTrace"))
                                {
                                    object value = pi.GetValue(scannedExeption, null).ToString();
                                    stringBuilder.AppendLine(String.Format("{0}={1}", pi.Name, (value == null ? "" : value.ToString())));
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    if (scannedExeption.Data != null && scannedExeption.Data.Count > 0)
                    {
                        stringBuilder.AppendLine("Data items:");
                        foreach (DictionaryEntry dataItem in scannedExeption.Data)
                        {
                            stringBuilder.AppendLine(String.Format("  key: {0}, value: {1}", dataItem.Key, dataItem.Value));
                        }
                    }
                }
                catch
                {
                    stringBuilder.AppendLine(string.Format("********** Исключительная ситуация при обработке исключительной ситуации. *********** {0}", e.GetType()));
                }
                scannedExeption = scannedExeption.InnerException;
                
                // Добавляем разрыв для отделения вложенных исключений друг от друга.
                stringBuilder.AppendLine();
            }
            try
            {
                stringBuilder.Append("Метод:" + ex.TargetSite.ReflectedType + "." + ex.TargetSite.Name);
            } 
            catch
            {
                // Здесь иногда водникает исключение - просто глушим его.
            }
            return stringBuilder.ToString();
        }
        
        /// <summary>
        /// Сдвиг сообщений вправо
        /// </summary>
        public static void Indent()
        {
            indentLevel++;
        }
        
        /// <summary>
        /// Сдвиг сообщений влево
        /// </summary>
        public static void Unindent()
        {
            indentLevel--;
        }
    }
}