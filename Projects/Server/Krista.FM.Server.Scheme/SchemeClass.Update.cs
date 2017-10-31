using System;
using System.Data;
using System.IO;
using Krista.Diagnostics;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.GlobalConsts;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.Server.DataPumpManagement;
using Krista.FM.Server.Users;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme
{
    /// <summary>
    /// Обновление объектов.
    /// </summary>
    internal partial class SchemeClass 
    {
        /// <summary>
        /// Формирует список отличий (операций изменения).
        /// </summary>
        /// <returns></returns>
        /// <returns>Список отличий (операций изменения).</returns>
        public IModificationItem GetChanges()
        {
            return GetChanges(null);
        }

        /// <summary>
        /// Формирует список отличий (операций изменения) текущего объекта от toObject.
        /// </summary>
        /// <param name="toObject">Объект с которым будет производиться сравнение.</param>
        /// <returns>Список отличий (операций изменения).</returns>
        public IModificationItem GetChanges(IModifiable toObject)
        {
            LogicalCallContextData userContext = LogicalCallContextData.GetContext();
            try
            {
                MutexSchemeAutoUpdate.WaitOne();

                SessionContext.SetSystemContext();
                return GetChangesFromRepositoryScheme(configFile);
            }
            catch (Exception e)
            {
                Trace.TraceError("В процессе поиска отличий произошла ошибка: {0}", KristaDiagnostics.ExpandException(e));
                throw new Exception(e.Message, e);
            }
            finally
            {
                LogicalCallContextData.SetContext(userContext);

                MutexSchemeAutoUpdate.ReleaseMutex();
            }
        }

        /// <summary>
        /// Применение изменений. Приводит текущий объект к виду объекта toObject
        /// </summary>
        /// <param name="toObject">Объект к виду которого будет приведен текущий объект</param>
        public void Update(IModifiable toObject)
        {
        }

        /// <summary>
        /// Получает дерево отличий текущей структуры схемы от структуры схемы в репозитории на диске.
        /// </summary>
        /// <param name="configFile">Конфигурационный файл схемы.</param>
        /// <returns>Дерево отличий.</returns>
        private IModificationItem GetChangesFromRepositoryScheme(string configFile)
        {
            Package rootPackage = new Package(SystemSchemeObjects.ROOT_PACKAGE_KEY, this, "Корневой пакет", ServerSideObjectStates.New);
            Package systemPackage = CreateSystemPackage(rootPackage);
            rootPackage.Packages.Add(
                KeyIdentifiedObject.GetKey(systemPackage.ObjectKey, systemPackage.Name),
                systemPackage);
            DateTime startTime = DateTime.Now;
            ((PackageCollection)rootPackage.Packages).Initialize(configFile, rootPackage);
            Trace.TraceVerbose("Выполнение PostInitialize");
            rootPackage.PostInitialize();
            Trace.TraceVerbose("Время загрузки пакетов: {0}", DateTime.Now - startTime);

            ModificationItem mi = new SchemeModificationItem(this.Name, this);
            
            ModificationItem gcmi = GetChangesGlobalConstants();
            if (gcmi != null)
            {
                mi.Items.Add(gcmi.Key, gcmi);
            }

            ModificationItem prmi = GetChangesPumpRegistry();
            if (prmi != null)
            {
                mi.Items.Add(prmi.Key, prmi);
            }

            ModificationItem packameMI = ((PackageCollection)this.rootPackage.Packages).GetChanges(rootPackage.Packages);
            if (packameMI.Items.Count > 0)
            {
                mi.Items.Add("Корневой пакет", packameMI);
            }
            
            mi.Purge();

            return mi;
        }

        /// <summary>
        /// Из каталока Configuration считывает таблицу с конфигурационными данными схемы.
        /// </summary>
        /// <param name="tableName">Имя таблицы данных.</param>
        /// <returns>Таблица с конфигурационными данными.</returns>
        private static DataTable GetConfigurationDataTable(string tableName)
        {
            string[] files = Directory.GetFiles(
                Instance.BaseDirectory + @"\Configuration\",
                "*.xml",
                SearchOption.TopDirectoryOnly);

            DataSet dataSet = new DataSet();
            foreach (string fileName in files)
            {
                DataSet ds = new DataSet();
                ds.ReadXml(fileName, XmlReadMode.Auto);
                if (ds.Tables.Contains(tableName))
                {
                    dataSet.Merge(ds.Tables[tableName]);
                }
            }
            return dataSet.Tables[tableName];
        }

        /// <summary>
        /// Поиск изменений для глобальных констант.
        /// </summary>
        /// <returns>Операция модификации.</returns>
        private static ModificationItem GetChangesGlobalConstants()
        {
            ModificationItem mi = new GlobalConstantsModificationItem("Глобальные константы", Instance.GlobalConstsManager, null);

            DataTable fromDataTable = ((GlobalConstsManager)Instance.GlobalConstsManager).GetDataTable().Copy();
            fromDataTable.Columns.Remove(DataAttribute.IDColumnName);

            DataTable toDataTable = GetConfigurationDataTable("GlobalConstsDataTable");
            toDataTable.Columns.Remove(DataAttribute.IDColumnName);

            // Значения для настраиваемых констант не должны меняться при обновлении
            // 0 - конфигурационные
            // 1 - настраиваемые
            // 2 - пользовательские
            foreach (DataRow row in toDataTable.Select("CONSTTYPE = 1"))
            {
                DataRow[] rows = fromDataTable.Select(
                    String.Format("CONSTTYPE = 1 and NAME = '{0}'", row["NAME"]));
                if (rows.Length > 0)
                {
                    row["VALUE"] = rows[0]["VALUE"];
                }
            }

            return DataTableModifications.GetChangesDataTable(mi, "GlobalConsts", "NAME", toDataTable, fromDataTable, null);
        }

        private static ModificationItem GetChangesPumpRegistry()
        {
            ModificationItem mi = new PumpRegistryModificationItem("Конфигурация реестра закачек", Instance.DataPumpManager.DataPumpInfo, null);

            // таблица из БД
        	DataTable fromDataTable;
			using (IDatabase db = Instance.SchemeDWH.DB)
			{
				fromDataTable = DataPumpInfo.PumpRegistryDataTable(db, String.Empty);
			}

        	// таблица восстановленная по xml
            DataTable toDataTable = GetConfigurationDataTable("PumpRegistryDataTable");

			return DataTableModifications.GetChangesDataTable(mi, "PumpRegistry", "ProgramIdentifier", toDataTable, fromDataTable, PumpRegistryRowAfterApplay); 
        }

		internal static void PumpRegistryRowAfterApplay(ModificationItem sender, ModificationContext context)
		{
			if (sender.Type == ModificationTypes.Create && sender.ToObject is DataRow)
			{
				// Регистрируем закачку в системе безопасности
				PumpRegistryCollection.RegisterPumpProgram(
					Convert.ToString(((DataRow)sender.ToObject)["ProgramIdentifier"]),
					Convert.ToString(((DataRow)sender.ToObject)["Name"]),
					(UsersManager)Instance.UsersManager);	
			}

			if (sender.Type == ModificationTypes.Remove)
			{
				// Удаляем закачку из системы безопасности
				PumpRegistryCollection.UnregisterPumpProgram(
					Convert.ToString(((DataRow)sender.ToObject)["ProgramIdentifier"]),
					(UsersManager)Instance.UsersManager);
			}
		}
	}
}
