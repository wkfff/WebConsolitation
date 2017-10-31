using System;
using System.Web;

using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.Core.DataProviders
{
    public class DataProvidersFactory
    {
        #region Поставщики данных
        /// <summary>
        /// Первичный поставщик многомерных данных
        /// </summary>
        public static DataProvider PrimaryMASDataProvider
        {
            get
            {
                DataProvider res = GetMASDataProvider("primaryDataProvider", "mas");
                return res;
            }
        }

        /// <summary>
        /// Вторичный поставщик многомерных данных
        /// </summary>
        public static DataProvider SecondaryMASDataProvider
        {
            get
            {
                DataProvider res = GetMASDataProvider("secondaryDataProvider", "mas_secondary");
                return res;
            }
        }

        /// <summary>
        /// Запасный поставщик многомерных данных
        /// </summary>
        public static DataProvider SpareMASDataProvider
        {
            get
            {
                DataProvider res = GetMASDataProvider("spareDataProvider", "mas_spare");
                return res;
            }
        }

        /// <summary>
        /// Устанавливает первичного поставщика с произвольным подключением.
        /// </summary>
        internal static void SetCustomPrimaryMASDataProvider(string connectionString)
        {
            DataProvider res = null;
            try
            {
                Trace.TraceVerbose("Создание провайдера GetCustomMASDataProvider");
                res = new DataProvider(connectionString);
                // Подключение здесь проверять не будем, проверим когда будем выполнять запрос.
                HttpContext.Current.Session["primaryDataProvider"] = res;
            }
            catch (Exception ex)
            {
                Trace.TraceVerbose("Ошибка при создании провайдера GetCustomMASDataProvider: {0}", CRHelper.GetExceptionInfo(ex));
            }
        }

        /// <summary>
        /// Получение провайдера данных кубов
        /// </summary>
        /// <param name="providerKey">ключ объекта (провайдера)</param>
        /// <param name="connectionKey">ключ его подключения</param>
        /// <returns></returns>
        private static DataProvider GetMASDataProvider(string providerKey, string connectionKey)
        {
            try
            {
                Trace.TraceVerbose("Создание провайдера {0}", providerKey);
                if (HttpContext.Current.Session[providerKey] == null)
                {
                    DataProvider res = new DataProvider();
                    res.ConnectionKey = connectionKey;
                    HttpContext.Current.Session[providerKey] = res;
                }
                // Подключение здесь проверять не будем, проверим когда будем выполнять запрос.
                return ((DataProvider)HttpContext.Current.Session[providerKey]);
            }
            catch (Exception ex)
            {
                Trace.TraceVerbose("Ошибка при создании провайдера {0}: {1}", providerKey, CRHelper.GetExceptionInfo(ex));
                return null;
            }
        }
        #endregion
    }
}
