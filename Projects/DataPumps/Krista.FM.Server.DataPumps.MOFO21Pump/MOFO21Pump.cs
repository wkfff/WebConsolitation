using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO21Pump
{

    // МОФО - 0021 - Прогноз показателей
    public class MOFO21PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Вариант.МОФО_Прогноз показателей (d_Variant_MOFOMarks)
        private IDbDataAdapter daVariant;
        private DataSet dsVariant;
        private IClassifier clsVariant;
        private Dictionary<string, int> cacheVariant = null;
        private int maxVariantCode = 0;
        // Показатели.МОФО_Прогноз показателей (d_Marks_Forecast)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<int, int> cacheMarks = null;
        private int nullMarks = 0;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        private Dictionary<string, int> cacheRegionsTerrType = null;
        private int nullRegions = -1;
        private int nullTerrType = 0;
        // Администратор.Планирование (d_KVSR_Plan)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<int, int> cacheKvsr = null;
        private int nullKvsr = -1;

        #endregion Классификаторы

        #region Факты

        // Показатели.МОФО_Прогноз показателей (f_Marks_Forecast)
        private IDbDataAdapter daForecast;
        private DataSet dsForecast;
        private IFactTable fctForecast;

        #endregion Факты

        private int sourceID = -1;
        private ReportType reportType;
        private List<int> deletedSourceID = null;
        private List<string> deletedRegionsAndVariants = null;
        private List<string> absentMarksCodes = null;

        #endregion Поля

        #region Перечисления

        private enum ReportType
        {
            /// <summary>
            /// Файлы с прогнозом показателей для ГО (pr011_go_*.xls)
            /// </summary>
            PrognosisGO,
            /// <summary>
            /// Файлы с прогнозом показателей для МР (pr011_mr_*.xls)
            /// </summary>
            PrognosisMR,
            /// <summary>
            /// Файлы с данными о плате за использование лесов (les11_*.xls)
            /// </summary>
            Forest,
            /// <summary>
            /// Файлы с данными о негативном воздействии (nv2_*.xls и nv3_*.xls)
            /// </summary>
            Negative
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private void SetNewSourceID()
        {
            sourceID = this.AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;

            if (deletedSourceID == null)
                deletedSourceID = new List<int>();

            if (!deletedSourceID.Contains(sourceID) && this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctForecast }, -1, sourceID, string.Empty);
                deletedSourceID.Add(sourceID);
            }
        }

        private void FillRegionsCache(DataTable dt)
        {
            cacheRegions = new Dictionary<string, int>();
            cacheRegionsTerrType = new Dictionary<string, int>();
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
                {
                    cacheRegions.Add(codeLine, Convert.ToInt32(row["ID"]));
                    cacheRegionsTerrType.Add(codeLine, Convert.ToInt32(row["RefTerr"]));
                }
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheVariant, dsVariant.Tables[0], "Name");
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "Codeind");
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Code");
            FillRegionsCache(dsRegions.Tables[0]);
        }

        private int GetMaxCode(IEntity cls)
        {
            string query = string.Format("select max(Code) from {0}", cls.FullDBName);
            object result = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((result == null) || (result == DBNull.Value))
                return 0;
            return Convert.ToInt32(result);
        }

        private void SetMaxCodes()
        {
            maxVariantCode = GetMaxCode(clsVariant);
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constraint = string.Format("SourceID = {0}", sourceID);
            InitDataSet(ref daVariant, ref dsVariant, clsVariant, string.Empty);
            InitDataSet(ref daMarks, ref dsMarks, clsMarks, string.Empty);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, constraint);
            InitDataSet(ref daKvsr, ref dsKvsr, clsKvsr, constraint);
            InitFactDataSet(ref daForecast, ref dsForecast, fctForecast);

            nullRegions = clsRegions.UpdateFixedRows(this.DB, sourceID);

            FillCaches();
            SetMaxCodes();
        }

        #region GUIDs

        private const string D_VARIANT_MOFO_MARKS_GUID = "fb32a561-7600-4058-abe7-14191ed295e4";
        private const string D_MARKS_FORECAST_GUID = "8c447694-a912-4bb8-8ca4-93dccd8a65a5";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string D_KVSR_PLAN_GUID = "dd69b4e1-f257-49ce-b553-442d094ae39a";
        private const string F_MARKS_FORECAST_GUID = "c47dafe3-33a5-419d-bf20-6b2b7b97337a";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsVariant = this.Scheme.Classifiers[D_VARIANT_MOFO_MARKS_GUID];
            clsMarks = this.Scheme.Classifiers[D_MARKS_FORECAST_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            clsKvsr = this.Scheme.Classifiers[D_KVSR_PLAN_GUID];
            fctForecast = this.Scheme.FactTables[F_MARKS_FORECAST_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctForecast };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daVariant, dsVariant, clsVariant);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
            UpdateDataSet(daForecast, dsForecast, fctForecast);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsForecast);
            ClearDataSet(ref dsVariant);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsKvsr);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xls

        #region Общие методы

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private void DeleteEarlierDataByRegionsAndVariant(int refRegions, int refVariant)
        {
            if (deletedRegionsAndVariants == null)
                deletedRegionsAndVariants = new List<string>();

            string key = string.Format("{0}|{1}", refRegions, refVariant);
            if (!deletedRegionsAndVariants.Contains(key) && !this.DeleteEarlierData)
            {
                string constaint = string.Format("RefRegions = {0} and RefVariant = {1}", refRegions, refVariant);
                DirectDeleteFactData(new IFactTable[] { fctForecast }, -1, sourceID, constaint);
                deletedRegionsAndVariants.Add(key);
            }
        }

        private void GetRefRegions(string value, ref int refRegions, ref int refTerrType)
        {
            string codeLine = string.Empty;
            switch (reportType)
            {
                case ReportType.PrognosisGO:
                    // из названия файла pr011_go_ННН.xls берем ННН
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[2].Trim());
                    break;
                case ReportType.PrognosisMR:
                    // из названия листа z_РРРНН берем РРР - для МР, РРРНН - для поселений
                    codeLine = CommonRoutines.TrimLetters(value);
                    if (Convert.ToInt32(codeLine.Substring(codeLine.Length - 2)) == 0)
                        codeLine = codeLine.Substring(0, 3);
                    break;
                case ReportType.Forest:
                    // из названия файла Les012_ННН.xls берем ННН
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[1].Trim());
                    break;
                case ReportType.Negative:
                    // из названия книги nv2_GG_AAA.xls или nv3_GG_AAA.xls берем ААА
                    codeLine = CommonRoutines.TrimLetters(value.Split('_')[2].Trim());
                    break;
            }

            if (!cacheRegions.ContainsKey(codeLine))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Планирование».", codeLine));
            }

            refRegions = cacheRegions[codeLine];
            refTerrType = cacheRegionsTerrType[codeLine];
        }

        private int GetRefMarks(string codeStr)
        {
            codeStr = codeStr.Trim();
            if (codeStr == string.Empty)
                return nullMarks;

            int code = Convert.ToInt32(codeStr);
            if (!cacheMarks.ContainsKey(code))
            {
                if (!absentMarksCodes.Contains(code.ToString()))
                    absentMarksCodes.Add(code.ToString());
                return nullMarks;
            }

            return cacheMarks[code];
        }

        private int GetBudgetLevel(int refTerrType, int sectionNumber)
        {
            if (sectionNumber == 1)
                return 2;
            switch (refTerrType)
            {
                case 4:
                    return 5;
                case 5:
                    return 16;
                case 6:
                    return 17;
                case 7:
                    return 15;
            }
            return 0;
        }

        private int PumpKvsr(string codeStr)
        {
            // код администратора берем из КБК
            // из 20-ти символов ХХХХХХХХХХХХХХХХХХХХ берем первые 3
            codeStr = codeStr.Replace(" ", string.Empty).PadLeft(20, '0').Substring(0, 3);
            int code = Convert.ToInt32(codeStr);
            object[] mapping = new object[] {
                "Code", code,
                "Name", constDefaultClsName,
                "SourceID", sourceID
            };
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, code, "Code", mapping);
        }

        private int PumpVariant(string name)
        {
            if (cacheVariant.ContainsKey(name))
                return cacheVariant[name];

            return PumpCachedRow(cacheVariant, dsVariant.Tables[0], clsVariant, name, new object[] {
                "Name", name, "Code", ++maxVariantCode, "VariantComment", name });
        }

        private void PumpFactRow(string fieldName, string value, int refVariant, int refMarks, int year,
            int refBudgetLevel, int refRegions, int refKvsr)
        {
            decimal factValue = CleanFactValue(value);
            if (factValue == 0)
                return;

            object[] mapping = new object[] {
                fieldName, factValue * 1000, // данные в тыс.руб. переводим в рубли
                "RefVariant", refVariant,
                "RefMarks", refMarks,
                "RefYearDayUNV", year * 10000 + 1,
                "RefBudgetLevels", refBudgetLevel,
                "RefRegions", refRegions,
                "RefKVSR", refKvsr,
                "SourceID", sourceID
            };

            PumpRow(dsForecast.Tables[0], mapping);
            if (dsForecast.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daForecast, ref dsForecast);
            }
        }

        #endregion Общие методы

        #region Прогноз показателей

        private void PumpXlsRowPrognosis(ExcelHelper excelDoc, int curRow, int sectionNumber, int refRegions, int refTerrType)
        {
            int refMarks = GetRefMarks(excelDoc.GetValue(curRow, 12));
            if (refMarks == nullMarks)
                return;

            int refBudgetLevel = GetBudgetLevel(refTerrType, sectionNumber);

            int year = this.DataSource.Year;
            int month = this.DataSource.Month;
            string name = string.Format("Дата сбора {0}_{1} (данные за {2}–{3}гг.)", year, month, year + 1, year + 3);
            int refVariant = PumpVariant(name);

            DeleteEarlierDataByRegionsAndVariant(refRegions, refVariant);

            PumpFactRow("Fact", excelDoc.GetValue(curRow, 7), refVariant, refMarks, year - 1, refBudgetLevel, refRegions, nullKvsr);
            PumpFactRow("Estimate", excelDoc.GetValue(curRow, 8), refVariant, refMarks, year, refBudgetLevel, refRegions, nullKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 9), refVariant, refMarks, year + 1, refBudgetLevel, refRegions, nullKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 10), refVariant, refMarks, year + 2, refBudgetLevel, refRegions, nullKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 11), refVariant, refMarks, year + 3, refBudgetLevel, refRegions, nullKvsr);
        }

        private void PumpXlsSheetPrognosis(ExcelHelper excelDoc, int refRegions, int refTerrType)
        {
            int sectionNumber = 0;
            bool toPump = false;
            // т.к. ячейки листа заблокированы, то функция, возвращая кол-во строк, не работает
            // поэтому качаем до тех пор, пока не будет 20 пустых строк подряд
            int emptyStrCount = 0;
            for (int curRow = 1; emptyStrCount <= 20; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                    {
                        emptyStrCount++;
                        toPump = false;
                        continue;
                    }
                    emptyStrCount = 0;

                    if ((cellValue == "1") && !toPump)
                    {
                        toPump = true;
                        sectionNumber++;
                        continue;
                    }

                    if (toPump)
                    {
                        PumpXlsRowPrognosis(excelDoc, curRow, sectionNumber, refRegions, refTerrType);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        #endregion Прогноз показателей

        #region Плата за использование лесов

        private void PumpXlsRowForest(ExcelHelper excelDoc, int curRow, int refRegions)
        {
            int refMarks = GetRefMarks("10" + excelDoc.GetValue(curRow, 1).Trim());
            if (refMarks == nullMarks)
                return;

            int year = this.DataSource.Year;
            int month = this.DataSource.Month;
            string name = string.Format("Плата за использование лесов {0}_{1} (данные за {2}–{3}гг.)", year, month, year + 1, year + 3);
            int refVariant = PumpVariant(name);

            DeleteEarlierDataByRegionsAndVariant(refRegions, refVariant);

            PumpFactRow("Fact", excelDoc.GetValue(curRow, 7), refVariant, refMarks, year - 1, 2, refRegions, nullKvsr);
            PumpFactRow("Estimate", excelDoc.GetValue(curRow, 8), refVariant, refMarks, year, 2, refRegions, nullKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 9), refVariant, refMarks, year + 1, 2, refRegions, nullKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 10), refVariant, refMarks, year + 2, 2, refRegions, nullKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 11), refVariant, refMarks, year + 3, 2, refRegions, nullKvsr);
        }

        private void PumpXlsSheetForest(ExcelHelper excelDoc, int refRegions)
        {
            bool toPump = false;
            // т.к. ячейки листа заблокированы, то функция, возвращая кол-во строк, не работает
            // поэтому качаем до тех пор, пока не будет 20 пустых строк подряд
            int emptyStrCount = 0;
            for (int curRow = 1; emptyStrCount <= 20; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                    {
                        emptyStrCount++;
                        toPump = false;
                        continue;
                    }
                    emptyStrCount = 0;

                    if ((cellValue == "1") && !toPump)
                    {
                        toPump = true;
                        continue;
                    }

                    if (toPump)
                    {
                        PumpXlsRowForest(excelDoc, curRow, refRegions);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        #endregion Плата за использование лесов

        #region Негативное воздействие

        private int GetRefMarksNegative(string form, int curRow)
        {
            switch (form.ToUpper())
            {
                case "NV3":
                    return GetRefMarks("201");
                case "NV2":
                    if (curRow == 13)
                        return GetRefMarks("202");
                    if (curRow == 14)
                        return GetRefMarks("203");
                    if (curRow == 15)
                        return GetRefMarks("204");
                    break;
            }
            return nullMarks;
        }

        private void PumpXlsRowNegative(ExcelHelper excelDoc, string form, int curRow, int refRegions, int refTerrType)
        {
            int refMarks = GetRefMarksNegative(form, curRow);
            if (refMarks == nullMarks)
                return;
            int refBudgetLevel = GetBudgetLevel(refTerrType, -1);
            int refKvsr = PumpKvsr(excelDoc.GetValue(curRow, 3).Trim());

            int year = this.DataSource.Year;
            int month = this.DataSource.Month;
            string name = string.Format("Плата за негативное воздействие {0}_{1} (данные за {0}–{2}гг.)", year, month, year + 3);
            int refVariant = PumpVariant(name);

            DeleteEarlierDataByRegionsAndVariant(refRegions, refVariant);

            PumpFactRow("PlanMO", excelDoc.GetValue(curRow, 4), refVariant, refMarks, year, refBudgetLevel, refRegions, refKvsr);
            PumpFactRow("Estimate", excelDoc.GetValue(curRow, 5), refVariant, refMarks, year, refBudgetLevel, refRegions, refKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 6), refVariant, refMarks, year + 1, refBudgetLevel, refRegions, refKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 7), refVariant, refMarks, year + 2, refBudgetLevel, refRegions, refKvsr);
            PumpFactRow("Forecast", excelDoc.GetValue(curRow, 8), refVariant, refMarks, year + 3, refBudgetLevel, refRegions, refKvsr);
        }

        private void PumpXlsSheetNegative(ExcelHelper excelDoc, string form, int refRegions, int refTerrType)
        {
            bool toPump = false;
            for (int curRow = 1; ; curRow++)
                try
                {
                    string cellValue = excelDoc.GetValue(curRow, 2).Trim();
                    if ((cellValue == string.Empty) && toPump)
                        break;

                    if (toPump)
                        PumpXlsRowNegative(excelDoc, form, curRow, refRegions, refTerrType);

                    if (cellValue == "1")
                        toPump = true;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
        }

        #endregion Негативное воздействие

        private void SetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.Contains("GO"))
                reportType = ReportType.PrognosisGO;
            if (filename.Contains("MR"))
                reportType = ReportType.PrognosisMR;
            if (filename.StartsWith("LES"))
                reportType = ReportType.Forest;
            if (filename.StartsWith("NV"))
                reportType = ReportType.Negative;
        }

        private bool SkipSheet(string sheetname)
        {
            sheetname = sheetname.ToUpper();
            return !sheetname.StartsWith("Z_");
        }

        private void ShowAbsentMarksCode(string filename)
        {
            if (absentMarksCodes.Count > 0)
            {
                absentMarksCodes.Sort();
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "В файле '{0}' обнаружены коды показателей, которых нет в справочнике {1}: {2}.",
                    filename, clsMarks.FullCaption, string.Join(", ", absentMarksCodes.ToArray())));
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            absentMarksCodes = new List<string>();
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                SetReportType(file.Name);

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                int refRegions = nullRegions;
                int refTerrType = nullTerrType;
                switch (reportType)
                {
                    case ReportType.PrognosisGO:
                        excelDoc.SetWorksheet(1);
                        GetRefRegions(file.Name, ref refRegions, ref refTerrType);
                        PumpXlsSheetPrognosis(excelDoc, refRegions, refTerrType);
                        break;
                    case ReportType.PrognosisMR:
                        int wsCount = excelDoc.GetWorksheetsCount();
                        for (int index = 1; index <= wsCount; index++)
                        {
                            excelDoc.SetWorksheet(index);
                            if (SkipSheet(excelDoc.GetWorksheetName()))
                                continue;
                            GetRefRegions(excelDoc.GetWorksheetName(), ref refRegions, ref refTerrType);
                            PumpXlsSheetPrognosis(excelDoc, refRegions, refTerrType);
                        }
                        break;
                    case ReportType.Forest:
                        excelDoc.SetWorksheet(1);
                        GetRefRegions(file.Name, ref refRegions, ref refTerrType);
                        PumpXlsSheetForest(excelDoc, refRegions);
                        break;
                    case ReportType.Negative:
                        excelDoc.SetWorksheet(1);
                        GetRefRegions(file.Name, ref refRegions, ref refTerrType);
                        string form = file.Name.Split('_')[0].Trim();
                        PumpXlsSheetNegative(excelDoc, form, refRegions, refTerrType);
                        break;
                }
                ShowAbsentMarksCode(file.Name);
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла '{0}': {1} Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
                absentMarksCodes.Clear();
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Xls

        #region Перекрытые методы

        protected override void DeleteEarlierPumpedData()
        {
            // заглушка
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
