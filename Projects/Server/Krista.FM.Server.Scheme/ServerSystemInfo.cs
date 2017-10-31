using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.Scheme;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Scheme.Services.OLAP;

namespace Krista.FM.Server.Scheme
{
    /// <summary>
    /// Класс для сбора информации о сервере приложений.
    /// </summary>
    internal class ServerSystemInfo : SystemInfo
    {
        private DataTable collectedInfo;
        /// <summary>
        /// 
        /// </summary>
        private SchemeClass scheme;

        public ServerSystemInfo()
            : base("Сервер")
        { }

        /// <summary>
        /// Собирает информацию и сохраняет ее во внутреннюю таблицу.
        /// </summary>
        /// <returns>Таблица содержащая собранную информацию.</returns>
        protected override DataTable CollectInfo()
        {
            scheme = SchemeClass.Instance;

            collectedInfo = base.CollectInfo();

            LogicalCallContextData usercontext = LogicalCallContextData.GetContext();
            try
            {
                Krista.FM.Server.Common.SessionContext.SetSystemContext();

                CollectServerInfo("Информация о сервере");
                CollectDataPumpInfo("Закачка данных");
            }
            catch(Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                LogicalCallContextData.SetContext(usercontext);
            }

            CheckRows(collectedInfo);
            return collectedInfo;
        }

        #region Сбор информации о сервере

        private void CollectServerInfo(string category)
        {
            try
            {
                // Общие параметры сервера
                GlobalInfo(category);

                // Информация о версиях
                CollectServerVersionsInfo(category);

                // Параметры сервера
                CollectServerparameters(category);

                // Реляционная БД
                CollectDWHInfo(category);

                // Многомерная БД
                CollectOLAPInfo(category);
            }
            catch(Exception e)
            {
                Trace.TraceError("Ошибка при получении параметра" + e.ToString());
            }
        }

        /// <summary>
        /// Общие параметры сервера
        /// </summary>
        /// <param name="category"></param>
        private void GlobalInfo(string category)
        {
            AddRow(category, "ProcessID", "ID процесса", String.Format("{0}", System.Diagnostics.Process.GetCurrentProcess().Id), null);
            AddRow(category, "GС", "Режим работы GC (режим сборки мусора)", (System.Runtime.GCSettings.IsServerGC) ? "серверный" : "клиентский", null);
            AddRow(category, "CLR", "Версия CLR (версия .NET Framework)", String.Format("{0}", Environment.Version), null);
            AddRow(category, "MachineName", "Имя машины", Environment.MachineName, null);
            AddRow(category, "UserDomainName", "Имя домена", Environment.UserDomainName, null);
            AddRow(category, "UserName", "Запущен под учетной записью", Environment.UserName, null);
            AddRow(category, "ServerBaseDirectory", "Каталог запуска", AppDomain.CurrentDomain.BaseDirectory, null);
            AddRow(category, "ServerAppName", "Имя приложения", AppDomain.CurrentDomain.SetupInformation.ApplicationName, null);
            AddRow(category, "Machine", "Сервер", scheme.Server.Machine, null);
            AddRow(category, "SchemeName", "Имя схемы", scheme.Name, null);
            AddRow(category, "MultiServerMode", "MultiServerMode", scheme.MultiServerMode.ToString(), null);
        }

        /// <summary>
        /// Параметры реляционной БД
        /// </summary>
        /// <param name="category"></param>
        private void CollectDWHInfo(string category)
        {
            Guid DWHID = AddRow(category, "DWHID", "Параметры реляционной БД", String.Format("СУБД: {0}\nБД: {1}", scheme.SchemeDWH.ServerVersion, scheme.SchemeDWH.DataBaseName), null);

			AddRow(category, "ServerName", "Сервер", (scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient || scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess || scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess) ? string.Empty : scheme.SchemeDWH.ServerName, DWHID);
            AddRow(category, "FactoryName", "Тип провайдера", scheme.SchemeDWH.FactoryName, DWHID);
            AddRow(category, "DBVersion", "Версия сервера", scheme.SchemeDWH.ServerVersion, DWHID);
            AddRow(category, "DBName", "Имя (алиас)", scheme.SchemeDWH.DataBaseName, DWHID);
            AddRow(category, "Version", "Версия базы данных", scheme.SchemeDWH.DatabaseVersion, DWHID);
        }

        /// <summary>
        /// Параметры многомерной базы 
        /// </summary>
        /// <param name="category"></param>
        private void CollectOLAPInfo(string category)
        {
            Guid MDStoreID = AddRow(category, "MDInfo", "Параметры многомерной базы", String.Format("СУБД: {0}\nМногомерная база: {1}", scheme.SchemeMDStore.IsAS2005() ? "SSAS2005" : "SSAS2000", scheme.SchemeMDStore.CatalogName), null);

            AddRow(category, "MDStoreName", "Сервер", SchemeMDStore.Instance.ServerName, MDStoreID);
            AddRow(category, "MDStoreVersion", "Версия сервера", SchemeMDStore.Instance.OlapDatabase.ServerVersion, MDStoreID);
            AddRow(category, "MDName", "Имя", SchemeMDStore.Instance.CatalogName, MDStoreID);
            AddRow(category, "MDStoreRName", "Реляционная база", SchemeMDStore.Instance.OlapDataSourceName, MDStoreID);
            AddRow(category, "MDStoreRServer", "Сервер реляционной базы", SchemeMDStore.Instance.OlapDataSourceServer, MDStoreID);
            
            if (!String.IsNullOrEmpty(SchemeMDStore.Instance.OlapDatabase.DatabaseVersion))
                AddRow(category, "VersionMD", "Версия многомерной базы", SchemeMDStore.Instance.OlapDatabase.DatabaseVersion, MDStoreID);

            AddRow(category, "MDXUniqueNameStyle", "Алгоритм генерации уникальных имен", SchemeMDStore.Instance.OlapDatabase.ConnectionString.MDXUniqueNameStyle, MDStoreID);
            GetCubesRevision(category, MDStoreID);
        }

        private void GetCubesRevision(string category, Guid mdStoreId)
        {
            DataTable cubestable = SchemeClass.Instance.Processor.OlapDBWrapper.GetPartitions("objecttype = 1");
            Guid CubesID = AddRow(category, "CubesInfo", "Список кубов",
                                      String.Format("число кубов - {0}", cubestable.Rows.Count), mdStoreId);
            foreach (DataRow row in cubestable.Rows)
            {

                if (row["Revision"] != null)
                {
                    AddRow(category, "Cube", row["ObjectName"].ToString(), row["Revision"].ToString(), CubesID);
                }
            }
        }

        /// <summary>
        /// Параметры сервера
        /// </summary>
        /// <param name="category"></param>
        private void CollectServerparameters(string category)
        {
            Guid parametrID = AddRow(category, "ServerParameters", "Параметры сервера", String.Format("число параметров - {0}", System.Configuration.ConfigurationManager.AppSettings.Count), null);
            foreach (string parametr in System.Configuration.ConfigurationManager.AppSettings.Keys)
            {
                AddRow(category, parametr, parametr, scheme.Server.GetConfigurationParameter(parametr), parametrID);
            }
        }

        private void CollectServerVersionsInfo(string category)
        {
            AddRow(category, "ServerLibraryVersion", "Базовая версия сервера", AppVersionControl.GetAssemblyBaseVersion(AppVersionControl.GetServerLibraryVersion()), null);
            Guid versionID = AddRow(category, "ServerAssemblyes", "Версии сборок",
                                    String.Format("число сборок - {0}",
                                                  scheme.UsersManager.GetServerAssemblyesInfo(
                                                      AppVersionControl.ServerAssemblyesSearchMaskDll).Count + scheme.UsersManager.GetServerAssemblyesInfo(
                                                      AppVersionControl.ServerAssemblyesSearchMaskExe).Count), null);

            CollectVersionInfo(category, AppVersionControl.ServerAssemblyesSearchMaskDll, versionID);
            CollectVersionInfo(category, AppVersionControl.ServerAssemblyesSearchMaskExe, versionID);
            CollectVersionInfo(category, "Krista.Diagnostics.dll", versionID);
        }

        private void CollectVersionInfo(string category, string filter, object parentID)
        {
            Dictionary<string, string> commonVer = AppVersionControl.GetAssemblyesVersions(filter);
            foreach (string commonVersion in commonVer.Keys)
            {
               AddRow(category, "Version", commonVersion, commonVer[commonVersion], parentID);
            }
        }
        
        #endregion

        private void CollectDataPumpInfo(string category)
        {
            string filter = "Krista.FM.Server.DataPump*.dll";

            Guid versionsID = AddRow(category, "DataPumpAssemblyVersions", "Версии сборок", String.Format("число сборок - {0}", AppVersionControl.GetAssemblyesVersions(filter).Count), null); 
            CollectVersionInfo(category, filter, versionsID);
        }
    }
}
