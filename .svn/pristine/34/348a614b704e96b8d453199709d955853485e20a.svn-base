using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumpManagement
{
    /// <summary>
    /// Информационная часть программ закачки
    /// </summary>
    public sealed class DataPumpInfo : DisposableObject, IDataPumpInfo
    {
        #region Поля

        private IScheme scheme;
        private PumpRegistryCollection pumpRegistry = null;
        private Dictionary<string, IDataPumpProgress> executablePumpPrograms = null;

        #endregion Поля


        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public DataPumpInfo(IScheme scheme)
        {
            this.scheme = scheme;
            this.executablePumpPrograms = new Dictionary<string, IDataPumpProgress>(30);
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.pumpRegistry != null) this.pumpRegistry.Dispose();
                if (this.executablePumpPrograms != null) this.executablePumpPrograms.Clear();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Инициализация полей
        /// </summary>
        public void Initialize()
        {
            this.pumpRegistry = new PumpRegistryCollection(scheme);
        }

        #endregion Инициализация


        #region Общие функции

		public static DataTable PumpRegistryDataTable(IDatabase db, string filter)
		{
			if (!String.IsNullOrEmpty(filter))
				filter = " where " + filter;

			return (DataTable)db.ExecQuery(
                    "select ID, SUPPLIERCODE, DATACODE, PROGRAMIDENTIFIER, PROGRAMCONFIG, COMMENTS," +
                    " NAME, PUMPPROGRAM, STAGESPARAMS, SCHEDULE from PUMPREGISTRY" + filter,
					QueryResultTypes.DataTable);
		}

		#endregion Общие функции


        #region Реализация IDataPumpInfo

        #region Реализация методов

        /// <summary>
        /// Удаляет запись информации о закачке
        /// </summary>
        /// <param name="key">ИД закачки</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Remove(string key)
        {
            if (executablePumpPrograms.ContainsKey(key))
            {
                executablePumpPrograms.Remove(key);
            }
        }

        /// <summary>
        /// Преобразует строку в состояние процесса закачки
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Строка</returns>
        public PumpProcessStates StringToPumpProcessStates(string state)
        {
            string stateUp = state.ToUpper();

            switch (stateUp)
            {
                case "ABORTED": return PumpProcessStates.Aborted;

                case "ASSOCIATEDATA": return PumpProcessStates.AssociateData;

                case "CHECKDATA": return PumpProcessStates.CheckData;

                case "DELETEDATA": return PumpProcessStates.DeleteData;

                case "FINISHED": return PumpProcessStates.Finished;

                case "PAUSED": return PumpProcessStates.Paused;

                case "PREPARED": return PumpProcessStates.Prepared;

                case "PREVIEWDATA": return PumpProcessStates.PreviewData;

                case "PROCESSCUBE": return PumpProcessStates.ProcessCube;

                case "PROCESSDATA": return PumpProcessStates.ProcessData;

                case "PUMPDATA": return PumpProcessStates.PumpData;

                case "RUNNING": return PumpProcessStates.Running;

                case "SKIP": return PumpProcessStates.Skip;
            }

            return PumpProcessStates.Prepared;
        }

        /// <summary>
        /// Преобразует строку в состояние этапа закачки
        /// </summary>
        /// <param name="ss">Строка</param>
        /// <returns>Состояние этапа</returns>
        public StageState StringToStageState(string ss)
        {
            switch (ss.ToUpper())
            {
                case "FINISHEDWITHERRORS": return StageState.FinishedWithErrors;

                case "INPROGRESS": return StageState.InProgress;

                case "INQUEUE": return StageState.InQueue;

                case "OUTOFQUEUE": return StageState.OutOfQueue;

                case "SKIPPED": return StageState.Skipped;

                case "SUCCEFULLFINISHED": return StageState.SuccefullFinished;

                case "BLOCKED": return StageState.Blocked;
            }

            return StageState.OutOfQueue;
        }

        /// <summary>
        /// Создает объект информации о ходе закачки
        /// </summary>
        public IDataPumpProgress CreateDataPumpProgress()
        {
            return new DataPumpProgress();
        }

        /// <summary>
        /// Очищает реестр программ закачки
        /// </summary>
        public void ResetCache()
        {
            pumpRegistry = null;
        }

        #endregion Реализация методов


        #region Реализация свойств

		/// <summary>
		/// Возвращает таблицу с реестром закачек.
		/// </summary>
		public DataTable GetPumpRegistryInfo()
		{
			DataTable pumpRegistrySourceTable;
			using (IDatabase db = scheme.SchemeDWH.DB)
			{
				pumpRegistrySourceTable = PumpRegistryDataTable(db, String.Empty);
			}

			DataTable pumpRegistryTable = new DataTable("PumpRegistryTable");
			pumpRegistryTable.Columns.Add(new DataColumn("ID", typeof(int)));
			pumpRegistryTable.Columns.Add(new DataColumn("State", typeof(bool)));
			pumpRegistryTable.Columns.Add(new DataColumn("SupplierCode", typeof(String)));
			pumpRegistryTable.Columns.Add(new DataColumn("DataCode", typeof(String)));
			pumpRegistryTable.Columns.Add(new DataColumn("DataName", typeof(String)));
			pumpRegistryTable.Columns.Add(new DataColumn("Name", typeof(String)));
			pumpRegistryTable.Columns.Add(new DataColumn("Comments", typeof(String)));
			pumpRegistryTable.Columns.Add(new DataColumn("ProgramIdentifier", typeof(String)));
			pumpRegistryTable.Columns["State"].Caption = "Статус";
			pumpRegistryTable.Columns["SupplierCode"].Caption = "Код поставщика";
			pumpRegistryTable.Columns["DataCode"].Caption = "Код поступающей информации";
			pumpRegistryTable.Columns["DataName"].Caption = "Наименование поступающей информации";
			pumpRegistryTable.Columns["Name"].Caption = "Программа закачки";
			pumpRegistryTable.Columns["Comments"].Caption = "Комментарий";

			IDataSourceManager dataSourcesManager = scheme.DataSourceManager;

			foreach (DataRow sourceRow in pumpRegistrySourceTable.Rows)
			{
				DataRow row = pumpRegistryTable.NewRow();
				row["ID"] = sourceRow["ID"];
				row["ProgramIdentifier"] = sourceRow["ProgramIdentifier"];
				row["Name"] = sourceRow["Name"];
				string supplierCode = Convert.ToString(sourceRow["SupplierCode"]);
				row["SupplierCode"] = supplierCode;
				string dataCode = Convert.ToString(sourceRow["DataCode"]).PadLeft(4, '0');
				row["DataCode"] = dataCode;
				if (dataSourcesManager.DataSuppliers.ContainsKey(supplierCode))
					if (dataSourcesManager.DataSuppliers[supplierCode].DataKinds.ContainsKey(dataCode))
						row["DataName"] = dataSourcesManager.DataSuppliers[supplierCode].DataKinds[dataCode].Name;
				row["Comments"] = sourceRow["Comments"];

				// Определяем состояние закачки
				if (executablePumpPrograms.ContainsKey(Convert.ToString(sourceRow["ProgramIdentifier"])))
				{
					row["State"] = executablePumpPrograms[Convert.ToString(sourceRow["ProgramIdentifier"])].PumpInProgress;
				}
				else
					row["State"] = false;

				pumpRegistryTable.Rows.Add(row);
			}

			return pumpRegistryTable;
		}

		/// <summary>
        /// Реестр программ закачки
        /// </summary>
        public IPumpRegistryCollection PumpRegistry
        {
            get
            {
                if (pumpRegistry == null)
                {
                    pumpRegistry = new PumpRegistryCollection(scheme);
                }

                return pumpRegistry;
            }
        }

        /// <summary>
        /// Коллекция выполяемых программ закачки. Ключ - ИД программы, значение - информация о ходе закачки
        /// </summary>
        public IDataPumpProgress this[string key]
        {
            get
            {
                if (executablePumpPrograms.ContainsKey(key))
                {
                    return executablePumpPrograms[key];
                }

                DataPumpProgress dpp = new DataPumpProgress();
                dpp.Initialize();
                executablePumpPrograms.Add(key, dpp);

                return dpp as IDataPumpProgress;
            }
        }

        #endregion Реализация свойств

        #endregion Реализация IDataPumpInfo
        
    }
}