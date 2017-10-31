using System;
using System.Diagnostics;
using Krista.FM.Common;

namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Индикаторы производительности
    /// </summary>
    public class PerformanceCounters : DisposableObject
    {
        const string categoryName = "Krista.FM.Server";

        private static CounterCreationData[] ccd = new CounterCreationData[2] {
            new CounterCreationData("IDatabase", "", PerformanceCounterType.NumberOfItems32), 
            new CounterCreationData("IDataUpdater", "", PerformanceCounterType.NumberOfItems32)
        };

        /// <summary>
        /// Наименование экземпляра
        /// </summary>
        //private string instanceName;
        //Dictionary<string, PerformanceCounter> counters;

        // Конструктор типа
        static PerformanceCounters()
        {
            try
            {
                if (PerformanceCounterCategory.Exists(categoryName))
                    PerformanceCounterCategory.Delete(categoryName);

                CounterCreationDataCollection ccdc = new CounterCreationDataCollection(ccd);
                PerformanceCounterCategory.Create(categoryName, "", PerformanceCounterCategoryType.MultiInstance, ccdc);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
        }

        /// <summary>
        /// Индикаторы производительности
        /// </summary>
        public PerformanceCounters(string instanceName)
        {
/*            this.instanceName = instanceName;
            counters = new Dictionary<string, PerformanceCounter>();

            foreach (CounterCreationData item in ccd)
            {
                Debug.WriteLine(String.Format("CounterName = {0}", item.CounterName));
                counters.Add(item.CounterName, new PerformanceCounter(categoryName, item.CounterName, instanceName, false));
            }*/
        }

        /// <summary>
        /// Деструктор класса
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //counters.Clear();
                //foreach (PerformanceCounter counter in counters.Values)
                //    PerformanceCounterCategory.Delete(counter.CategoryName);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Создает глобальный индикатор производительности
        /// </summary>
        /// <param name="counterName"></param>
        /// <returns></returns>
        public static PerformanceCounter CreateGlobalPerformanceCounter(string counterName)
        {
            return new PerformanceCounter(categoryName, counterName, "_Global_", false);
        }

        /// <summary>
        /// Возвращает индикаторы производительности с указанным именем
        /// </summary>
        /// <param name="name">Имя индикатора производительности</param>
        /// <returns>Индикатор производительности</returns>
        /*public PerformanceCounter this[string name]
        {
            get
            {
                return counters[name];
            }
        }*/
    }
}
