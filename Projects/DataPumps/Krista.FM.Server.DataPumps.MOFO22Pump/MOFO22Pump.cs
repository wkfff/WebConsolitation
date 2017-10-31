using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO22Pump
{

    // МОФО - 0022 - Задолженность по арендной плате
    public class MOFO22PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.Задолженность по арендной плате (d_Organizations_Rent)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrgInn = null;
        private Dictionary<string, int> cacheOrgInnName = null;
        private int nullOrg = -1;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        // кэш МР и ГО (связка CodeLine -> ID или CodeLine|Code -> ID)
        private Dictionary<string, int> cacheRegions = null;
        private int nullRegions = -1;

        #endregion Классификаторы

        #region Факты

        // Факт.Задолженность по арендной плате (f_F_Rent)
        private IDbDataAdapter daRent;
        private DataSet dsRent;
        private IFactTable fctRent;

        #endregion Факты

        private ReportType reportType;
        private bool settlePump = false;
        private bool toDeleteEarlierDataByRegion = false;
        private List<string> deletedDatesAndRegionsList = null;

        #endregion Поля

        #region Перечисления

        private enum ReportType
        {
            // данные по городским округам (файлы dolg_go_ГГКННН.xls)
            DolgGO,
            // данные по муниципальным районам (файлы dolg_mr_ГГКННН.xls)
            DolgMR
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillRegionsCache(DataTable dt)
        {
            cacheRegions = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                string codeLine = Convert.ToString(row["CodeLine"]);
                // если поле CodeLine заполнено, то это МР или ГО, его просто добавляем в кэш
                if (codeLine == string.Empty)
                {
                    // если нет, то это поселение. в этом случае порядковый номер CodeLine будет составной:
                    // в начале идут первые 1 или 2 цифры из поля Code, дополненные до 3-х знаков нулями;
                    // в конце - последние 2 цифры из поля Code
                    string code = Convert.ToString(row["Code"]);
                    if (code.Length < 5)
                        continue;
                    codeLine = code.Substring(0, code.Length - 4).PadLeft(3, '0') + code.Substring(code.Length - 2);
                }
                if (!cacheRegions.ContainsKey(codeLine))
                    cacheRegions.Add(codeLine, Convert.ToInt32(row["ID"]));
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheOrgInn, dsOrg.Tables[0], "INN");
            FillRowsCache(ref cacheOrgInnName, dsOrg.Tables[0], new string[] { "INN", "Name" }, "|", "ID");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        protected override void QueryData()
        {
            int regionsSourceID = AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, string.Format("SourceID = {0}", regionsSourceID));
            nullRegions = clsRegions.UpdateFixedRows(this.DB, regionsSourceID);
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, string.Empty);
            InitFactDataSet(ref daRent, ref dsRent, fctRent);
            FillCaches();
        }

        #region GUIDs

        private const string D_ORGANIZATIONS_RENT_GUID = "ad49fe6f-e110-4c98-bb3d-8d38a118fa41";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_F_RENT_GUID = "c5948369-66b9-42b8-8389-dd71d7e043ee";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsOrg = this.Scheme.Classifiers[D_ORGANIZATIONS_RENT_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctRent = this.Scheme.FactTables[F_F_RENT_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctRent };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daRent, dsRent, fctRent);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRent);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        private object CleanFactValue(string factValue, decimal multiplier)
        {
            factValue = CommonRoutines.TrimLetters(factValue.Trim()).Replace('.', ',').Trim();
            if (factValue == string.Empty)
                return DBNull.Value;
            return Convert.ToDecimal(factValue) * multiplier;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, string[] sumsAddress, int refDate, int refBudget,
            int refRegions, int refOrg, int refMarks, decimal multiplier)
        {
            object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefBudgetLevels", refBudget,
                "RefRegions", refRegions, "RefOrganizations", refOrg, "RefMarks", refMarks };

            bool nullSums = true;
            for (int i = 0; i < sumsAddress.Length; i += 2)
            {
                object factValue = CleanFactValue(excelDoc.GetValue(sumsAddress[i + 1]), multiplier);
                if (factValue != DBNull.Value)
                    nullSums = false;
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { sumsAddress[i], factValue });
            }
            if (nullSums)
                return;

            PumpRow(dsRent.Tables[0], mapping);
            if (dsRent.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRent, ref dsRent);
            }
        }

        private int PumpXlsOrg(ExcelHelper excelDoc, int curRow)
        {
            string innStr = excelDoc.GetValue(curRow, 2).Trim();
            long inn = Convert.ToInt64(innStr.PadLeft(1, '0'));
            string name = excelDoc.GetValue(curRow, 3).Trim();
            if (name == string.Empty)
            {
                if (inn == 0)
                    return nullOrg;
                name = constDefaultClsName;
            }

            if (inn == 0)
            {
                string key = string.Format("{0}|{1}", inn, name);
                if (cacheOrgInnName.ContainsKey(key))
                    return cacheOrgInnName[key];

                if (cacheOrgInnName.ContainsKey(string.Format("{0} ({1})", key, inn)))
                    return cacheOrgInnName[string.Format("{0} ({1})", key, inn)];

                return PumpCachedRow(cacheOrgInnName, dsOrg.Tables[0], clsOrg, key, new object[] { "INN", inn, "Name", name });
            }
            return PumpCachedRow(cacheOrgInn, dsOrg.Tables[0], clsOrg, inn.ToString(), new object[] { "INN", inn, "Name", name });
        }

        private int GetRefBudget(int column, bool fullTable)
        {
            switch (column)
            {
                // столбец D
                case 0:
                    return 0;
                // столбцы E и F
                case 1:
                    if (reportType == ReportType.DolgGO)
                        return 3;
                    if (reportType == ReportType.DolgMR)
                        return 5;
                    break;
                // столбцы G и H
                case 2:
                    if (reportType == ReportType.DolgGO)
                        return 15;
                    if (reportType == ReportType.DolgMR)
                    {
                        if (fullTable || settlePump)
                            return 6;
                        return 5;
                    }
                    break;
            }
            return 0;
        }

        /// <summary>
        /// Закачка одной таблицы из Xls-листа
        /// </summary>
        /// <param name="excelDoc">Объект Excel</param>
        /// <param name="curRow">Строка, с которой начинаются данные</param>
        /// <param name="fullTable">Полная таблица (присутствуют данные в столбцах E и F)</param>
        /// <param name="refDate">Ссылка на дату</param>
        /// <param name="refRegions">Ссылка на район</param>
        /// <param name="refMarks">Ссылка на показатели</param>
        private void PumpXlsTable(ExcelHelper excelDoc, int curRow, bool fullTable, int refDate, int refRegions, int refMarks)
        {
            int refOrg = nullOrg;
            PumpXlsRow(excelDoc,
                new string[] { "Quantity", "D" + curRow.ToString() },
                refDate, GetRefBudget(0, fullTable), refRegions, refOrg, refMarks, 1);

            if (fullTable)
            {
                PumpXlsRow(excelDoc,
                    new string[] { "TotalDeb", "E" + curRow.ToString(), "BeznDeb", "F" + curRow.ToString() },
                    refDate, GetRefBudget(1, fullTable), refRegions, refOrg, refMarks, 1000);
            }
            PumpXlsRow(excelDoc,
                new string[] { "TotalDeb", "G" + curRow.ToString(), "BeznDeb", "H" + curRow.ToString() },
                refDate, GetRefBudget(2, fullTable), refRegions, refOrg, refMarks, 1000);

            curRow += 2;
            refOrg = PumpXlsOrg(excelDoc, curRow);
            if (fullTable)
            {
                PumpXlsRow(excelDoc,
                    new string[] { "TotalDeb", "E" + curRow.ToString(), "BeznDeb", "F" + curRow.ToString() },
                    refDate, GetRefBudget(1, fullTable), refRegions, refOrg, refMarks, 1000);
            }
            PumpXlsRow(excelDoc,
                new string[] { "TotalDeb", "G" + curRow.ToString(), "BeznDeb", "H" + curRow.ToString() },
                refDate, GetRefBudget(2, fullTable), refRegions, refOrg, refMarks, 1000);

            curRow += 1;
            refOrg = PumpXlsOrg(excelDoc, curRow);
            if (fullTable)
            {
                PumpXlsRow(excelDoc,
                    new string[] { "TotalDeb", "E" + curRow.ToString(), "BeznDeb", "F" + curRow.ToString() },
                    refDate, GetRefBudget(1, fullTable), refRegions, refOrg, refMarks, 1000);
            }
            PumpXlsRow(excelDoc,
                new string[] { "TotalDeb", "G" + curRow.ToString(), "BeznDeb", "H" + curRow.ToString() },
                refDate, GetRefBudget(2, fullTable), refRegions, refOrg, refMarks, 1000);

            curRow += 1;
            refOrg = PumpXlsOrg(excelDoc, curRow);
            if (fullTable)
            {
                PumpXlsRow(excelDoc,
                    new string[] { "TotalDeb", "E" + curRow.ToString(), "BeznDeb", "F" + curRow.ToString() },
                    refDate, GetRefBudget(1, fullTable), refRegions, refOrg, refMarks, 1000);
            }
            PumpXlsRow(excelDoc,
                new string[] { "TotalDeb", "G" + curRow.ToString(), "BeznDeb", "H" + curRow.ToString() },
                refDate, GetRefBudget(2, fullTable), refRegions, refOrg, refMarks, 1000);
        }

        private void DeleteEarlierDataByDateAndRegions(int refDate, int refRegions)
        {
            if (toDeleteEarlierDataByRegion)
            {
                string key = string.Format("{0}|{1}", refDate, refRegions);
                if (!deletedDatesAndRegionsList.Contains(key))
                {
                    DeleteData(string.Format("RefRegions = {0} and RefYearDayUNV = {1} and SourceId = {2}",
                        refRegions, refDate, this.SourceID));
                    deletedDatesAndRegionsList.Add(key);
                }
            }
        }

        private void PumpXlsDoc(ExcelHelper excelDoc, int refDate, int refRegions)
        {
            if (reportType == ReportType.DolgMR)
            {
                if (!excelDoc.GetWorksheetName().Trim().ToUpper().StartsWith("Z_"))
                    return;
                refRegions = GetRefRegions(excelDoc.GetWorksheetName().Trim());
            }

            DeleteEarlierDataByDateAndRegions(refDate, refRegions);

            PumpXlsTable(excelDoc, 17, true, refDate, refRegions, 0);
            PumpXlsTable(excelDoc, 27, false, refDate, refRegions, 1);
            PumpXlsTable(excelDoc, 37, false, refDate, refRegions, 2);
            PumpXlsTable(excelDoc, 47, true, refDate, refRegions, 3);
        }

        private void SetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.Contains("GO"))
                reportType = ReportType.DolgGO;
            if (filename.Contains("MR"))
                reportType = ReportType.DolgMR;
        }

        private int GetRefDate(string filename)
        {
            // из названия файла dolg_go_ГГКННН.xls или dolg_mr_ГГКННН.xls
            // берем ГГ - год, К - квартал
            string strDate = CommonRoutines.TrimLetters(filename);
            int year = Convert.ToInt32(strDate.Substring(0, 2));
            year = (year > 90) ? year + 1900 : year + 2000;
            int quarter = Convert.ToInt32(strDate.Substring(2, 1));
            return year * 10000 + 9990 + quarter;
        }

        private int GetRefRegions(string value)
        {
            string codeLine = string.Empty;
            if (reportType == ReportType.DolgGO)
            {
                // из названия файла dolg_go_ГГКННН.xls берем ННН
                codeLine = CommonRoutines.TrimLetters(value).Substring(3);
            }
            else if (reportType == ReportType.DolgMR)
            {
                settlePump = true;
                // из названия листа z_РРРНН берем РРР - для МР, РРРНН - для поселений
                codeLine = CommonRoutines.TrimLetters(value);
                if (Convert.ToInt32(codeLine.Substring(codeLine.Length - 2)) == 0)
                {
                    codeLine = codeLine.Substring(0, 3);
                    settlePump = false;
                }
            }
            if (!cacheRegions.ContainsKey(codeLine))
            {
                throw new PumpDataFailedException("Не найдено муниципальное образование в справочнике «Районы.Планирование».");
            }
            return FindCachedRow(cacheRegions, codeLine, nullRegions);
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                SetReportType(file.Name);
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refDate = GetRefDate(file.Name);
                int refRegions = nullRegions;
                if (reportType == ReportType.DolgGO)
                    refRegions = GetRefRegions(file.Name);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    PumpXlsDoc(excelDoc, refDate, refRegions);
                }
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("'{0}' файл не был загружен. {1}", file.Name, ex.Message));
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        // Удаление организаций, по которым нет данных
        private void DeleteUnusedOrgRecords(IFactTable fct, string refFieldName)
        {
            string query = string.Format(
                "select count(*) from {0} where not ID in (select distinct {1} from {2}) and ID > 0",
                clsOrg.FullDBName, refFieldName, fct.FullDBName);

            int orgsDetetedCount = Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));
            if (orgsDetetedCount == 0)
                return;

            query = string.Format(
                "delete from {0} where not ID in (select distinct {1} from {2}) and ID > 0",
                clsOrg.FullDBName, refFieldName, fct.FullDBName);
            this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "Классификатор '{0}' содержал записи, по которым нет данных в таблице фактов '{1}'. Эти записи были удалены (кол-во: {2}).",
                clsOrg.FullCaption, fct.FullCaption, orgsDetetedCount));
        }

        // корректировка дублирующихся наименований организаций путем приписывания инн
        private void CorrectDoubleOrgsName()
        {
            Dictionary<string, long> auxCache = new Dictionary<string, long>();
            try
            {
                InitDataSet(ref daOrg, ref dsOrg, clsOrg, "ID > 0");
                foreach (DataRow org in dsOrg.Tables[0].Rows)
                {
                    long inn = Convert.ToInt64(org["Inn"]);

                    string name = Convert.ToString(org["Name"]);
                    // если наименование встречается впервые, то просто пишем его в кэш
                    if (!auxCache.ContainsKey(name))
                    {
                        auxCache.Add(name, inn);
                        continue;
                    }

                    // такого конечно быть не должно, но на всякий случай
                    if (auxCache[name] == inn)
                        continue;

                    // вот имя повторилось, а инн другой. дописываем его к наименованию
                    org["Name"] = string.Format("{0} ({1})", name, inn);
                }
            }
            finally
            {
                auxCache.Clear();
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedDatesAndRegionsList = new List<string>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
                DeleteUnusedOrgRecords(fctRent, "RefOrganizations");
                CorrectDoubleOrgsName();
            }
            finally
            {
                deletedDatesAndRegionsList.Clear();
            }
        }

        protected override void DeleteEarlierPumpedData()
        {
            if (!toDeleteEarlierDataByRegion)
            {
                base.DeleteEarlierPumpedData();
            }
        }

        protected override void DirectPumpData()
        {
            toDeleteEarlierDataByRegion = Convert.ToBoolean(GetParamValueByName(
                this.PumpRegistryElement.ProgramConfig, "ucbDeleteEarlierDataByRegion", "False"));
            PumpDataYQTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
