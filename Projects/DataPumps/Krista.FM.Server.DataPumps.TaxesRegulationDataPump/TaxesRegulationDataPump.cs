using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.TaxesRegulationDataPump
{
    /// <summary>
    /// УФК_0009_Результаты регулировки налогов между уровнями бюджетов Краснодарского края
    /// </summary>
    public class TaxesRegulationDataPumpModule : CorrectedPumpModuleBase
    {
        #region Поля

        // ОКАТО.УФК (d.OKATO.UFK)
        private IDbDataAdapter daOkatoUfk;
        private DataSet dsOkatoUfk;
        // КД.УФК (d.KD.UFK)
        private IDbDataAdapter daKdUfk;
        private DataSet dsKdUfk;

        // Доходы.УФК_Доходы_Результаты регулировки (f.D.IncomesRC)
        private IDbDataAdapter daIncomesRC;
        private DataSet dsIncomesRC;

        private IClassifier clsOkatoUfk;
        private IClassifier clsKdUfk;

        private IFactTable fctIncomesRC;

        // Кэши классификаторов
        private Dictionary<string, int> kdCache = null;
        private Dictionary<string, int> okatoCache = null;

        private DBDataAccess dbDataAccess = new DBDataAccess();
        private Database dbfDB = null;
        private int totalFiles = 0;
        private int filesCount = 0;
        private int month = -1;

        private List<string> deletedData = new List<string>(200);

        #endregion Поля


        #region Константы

        /// <summary>
        /// Количество записей, с которым будем работать
        /// </summary>
        private const int constMaxQueryRecords = 10000;

        /// <summary>
        /// Ограничение для запрета удаления данных по заключительным оборотам
        /// </summary>
        private const string constOracleDisableDeletionFinalOverturnConstraint =
            "(mod(REFFKDAYUNV, 10000) <> 1232 and REFFKDAYUNV = {0} and GHOST = '{1}')";

        /// <summary>
        /// Ограничение для запрета удаления данных по заключительным оборотам
        /// </summary>
        private const string constSQLServerDisableDeletionFinalOverturnConstraint =
            "((REFFKDAYUNV % 10000) <> 1232 and REFFKDAYUNV = {0} and GHOST = '{1}')";

        /// <summary>
        /// Ограничение для выборки данных фактов по указанному месяцу
        /// </summary>
        private const string constOracleSelectFactDataByMonthConstraint =
            "(floor(mod(REFFKDAYUNV, 10000) / 100) = {0})";

        /// <summary>
        /// Ограничение для выборки данных фактов по указанному месяцу
        /// </summary>
        private const string constSQLServerSelectFactDataByMonthConstraint =
            "(floor((REFFKDAYUNV % 10000) / 100) = {0})";

        #endregion Константы


        #region Инициализация

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbDataAccess != null) dbDataAccess.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion Инициализация


        #region Закачка данных

        /// <summary>
        /// Возвращает ограничение на выборку согласно типу базы
        /// </summary>
        /// <returns>Ограничение</returns>
        private string GetDisableDeletionFinalOverturnConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return constSQLServerDisableDeletionFinalOverturnConstraint;

                default:
                    return constOracleDisableDeletionFinalOverturnConstraint;
            }
        }

        /// <summary>
        /// Закачивает файл формата дбф
        /// </summary>
        /// <param name="fileInfo">Файл</param>
        private void PumpDBFFile(FileInfo fileInfo)
        {
            int budgetLevel = 0;
            string ghost = string.Empty;

            // Если призрак для закачки DFB – уровень бюджета 01 (Федеральный бюджет)
            if (fileInfo.Name.ToUpper().StartsWith("DFB"))
            {
                budgetLevel = 1;
                ghost = "DFB";
            }
            // Если призрак для закачки DKB – уровень бюджета 03 (бюджет субъекта)
            else if (fileInfo.Name.ToUpper().StartsWith("DKB"))
            {
                budgetLevel = 3;
                ghost = "DKB";
            }
            // Если призрак для закачки DMB – уровень бюджета 14 (Конс.бюджет МО)
            else if (fileInfo.Name.ToUpper().StartsWith("DMB"))
            {
                budgetLevel = 14;
                ghost = "DMB";
            }
            else
            {
                return;
            }

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                string.Format("Старт обработки файла {0}.", fileInfo.FullName));

            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(dbfDB, ref da, ref ds, string.Format("select * from {0}", fileInfo.Name));
            DataTable dt = ds.Tables[0];

            bool pumpRSMB = this.DataSource.Year >= 2006;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];

                try
                {
                    int date = Convert.ToInt32(CommonRoutines.GlobalShortDateToNewDate(
                        Convert.ToString(row["D_OPER"])));

                    // Если установлен параметр "Закачка заключительных оборотов", то закачиваемый файл должен закачаться 
                    // со ссылкой на фиксированный Период.День по элементу "Заключительные обороты" предыдущего года.
                    if (this.FinalOverturn)
                    {
                        date = this.FinalOverturnDate;
                    }

                    CheckDataSourceByDate(date, true);

                    string key = GetComplexCacheKey(new object[] { date, ghost });
                    if (!deletedData.Contains(key))
                    {
                        // Удаляем закачанные ранее данные по этой дате
                        DeleteData(string.Format(GetDisableDeletionFinalOverturnConstraint(), date, ghost), 
                            string.Format("Дата отчета: {0}.", date));

                        deletedData.Add(key);
                    }

                    int okatoID;
                    if (pumpRSMB)
                    {
                        okatoID = PumpCachedRow(okatoCache, dsOkatoUfk.Tables[0], clsOkatoUfk,
                            GetComplexCacheKey(row, new string[] { "OKATO", "RS_MB" }),
                            new object[] { "CODE", row["OKATO"], "ACCOUNT", row["RS_MB"] });
                    }
                    else
                    {
                        okatoID = PumpCachedRow(okatoCache, dsOkatoUfk.Tables[0], clsOkatoUfk,
                            row["OKATO"], "CODE", null);
                    }

                    int kdID = PumpCachedRow(kdCache, dsKdUfk.Tables[0], clsKdUfk, row["C_PRIV"], "CODESTR", null);

                    PumpRow(dsIncomesRC.Tables[0], new object[] { "FORPERIOD", row["S_PAY"], "REFFKDAYUNV", date,
                        "RefFODayUNV", date, "REFKD", kdID, "REFOKATO", okatoID, "REFBUDGETLEVELS", budgetLevel,
                        "GHOST", ghost });

                    if (dsIncomesRC.Tables[0].Rows.Count > constMaxQueryRecords)
                    {
                        UpdateData();
                        ClearDataSet(daIncomesRC, ref dsIncomesRC);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка {1}", i, ex.Message), ex);
                }
            }

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                "Обработка файла {0} завершена. Обработано {1} строк.",
                fileInfo.FullName, dt.Rows.Count));
        }

        /// <summary>
        /// Закачивает список файлов нового формата
        /// </summary>
        /// <param name="filesList">Список файлов</param>
        protected override void ProcessFiles(DirectoryInfo dir)
		{
            if (this.FinalOverturn)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Закачка заключительных оборотов на {0}.", 
                    CommonRoutines.NewDateToLongDate(this.FinalOverturnDate.ToString())));
            }

            FileInfo[] files = dir.GetFiles("*.DBF", SearchOption.AllDirectories);
            for (int i = 0; i < files.GetLength(0); i++)
            {
                // Подключаемся к источнику
                dbDataAccess.ConnectToDataSource(ref dbfDB, files[i].DirectoryName, ODBCDriverName.Microsoft_dBase_Driver);                

                filesCount++;
                SetProgress(totalFiles, filesCount,
                    string.Format("Обработка файла {0}...", files[i].FullName),
                    string.Format("Файл {0} из {1}", filesCount, totalFiles), true);

                try
                {
                    PumpDBFFile(files[i]);
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, 
                        "Обработка файла закончена с ошибками", ex);
                    continue;
                }
            }
		}

		/// <summary>
		/// Запрос данных из базы
		/// </summary>
        protected override void QueryData()
		{
            InitClsDataSet(ref daKdUfk, ref dsKdUfk, clsKdUfk);
            InitClsDataSet(ref daOkatoUfk, ref dsOkatoUfk, clsOkatoUfk);

            InitFactDataSet(ref daIncomesRC, ref dsIncomesRC, fctIncomesRC);

            FillRowsCache(ref kdCache, dsKdUfk.Tables[0], "CODESTR");

            if (this.DataSource.Year >= 2006)
            {
                FillRowsCache(ref okatoCache, dsOkatoUfk.Tables[0], new string[] { "CODE", "ACCOUNT" }, "ID");
            }
            else
            {
                FillRowsCache(ref okatoCache, dsOkatoUfk.Tables[0], "CODE");
            }
		}

		/// <summary>
		/// Внести изменения в базу
		/// </summary>
        protected override void UpdateData()
		{
            UpdateDataSet(daKdUfk, dsKdUfk, clsKdUfk);
            UpdateDataSet(daOkatoUfk, dsOkatoUfk, clsOkatoUfk);

            UpdateDataSet(daIncomesRC, dsIncomesRC, fctIncomesRC);
		}

        private const string D_KD_UFK_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_OKATO_UFK_GUID = "4ae52664-ca7c-4994-bc5e-ba982421540e";
        private const string F_D_INCOMES_RC_GUID = "4285d4d8-2c6f-4cdc-952f-ea8d4d38bb98";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { 
                clsKdUfk = this.Scheme.Classifiers[D_KD_UFK_GUID],
                clsOkatoUfk = this.Scheme.Classifiers[D_OKATO_UFK_GUID] };

            this.UsedFacts = new IFactTable[] { 
                fctIncomesRC = this.Scheme.FactTables[F_D_INCOMES_RC_GUID] };
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            if (dbfDB != null) dbfDB.Close();

            ClearDataSet(ref dsIncomesRC);
            ClearDataSet(ref dsKdUfk);
            ClearDataSet(ref dsOkatoUfk);

            deletedData.Clear();
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            totalFiles = this.RootDir.GetFiles("*.DBF", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;

            deletedData.Clear();

            PumpDataYTemplate();
        }

		#endregion Закачка данных


        #region Обработка данных

        /// <summary>
        /// Возвращает ограничение на выборку согласно типу базы
        /// </summary>
        /// <returns>Ограничение</returns>
        private string GetSelectFactDataByMonthConstraint()
        {
            switch (this.ServerDBMSName)
            {
                case DBMSName.SQLServer:
                    return constSQLServerSelectFactDataByMonthConstraint;

                default:
                    return constOracleSelectFactDataByMonthConstraint;
            }
        }

        /// <summary>
        /// Функция запроса данных из базы
        /// </summary>
        protected override void QueryDataForProcess()
        {
            PrepareRegionsForSumDisint();
            PrepareOkatoForSumDisint(clsOkatoUfk);
        }

        /// <summary>
        /// Устанавливает соответствие операционных дней для текущего источника
        /// </summary>
        protected override void ProcessDataSource()
        {
            if (month > 0)
            {
                SetOperationDaysForFact(fctIncomesRC, "GHOST", "REFFKDAYUNV", "REFFODAYUNV",
                    string.Format(GetSelectFactDataByMonthConstraint(), month));
            }
            else
            {
                SetOperationDaysForFact(fctIncomesRC, "GHOST", "REFFKDAYUNV", "REFFODAYUNV", string.Empty);
            }
        }

        /// <summary>
        /// Функция сохранения закачанных данных в базу
        /// </summary>
        protected override void UpdateProcessedData()
        {
            UpdateOkatoData();
            UpdateData();
        }

        /// <summary>
        /// Функция выполнения завершающих действий закачки
        /// </summary>
        protected override void ProcessFinalizing()
        {

        }

        /// <summary>
        /// Действия, выполняемые после обработки данных
        /// </summary>
        protected override void AfterProcessDataAction()
        {
            WriteBadOkatoCodesCacheToBD();
            WriteNullAccountsCacheToBD();
        }

        /// <summary>
        /// Этап обработки данных
        /// </summary>
        protected override void DirectProcessData()
        {
            // Заполняем кэши соответствия операционных дней
            FillOperationDaysCorrCache();

            int year = -1;
            GetPumpParams(ref year, ref month);

            ProcessDataSourcesTemplate(year, month, "Установка соответствия операционных дней");
        }

        #endregion Обработка данных
    }
}
