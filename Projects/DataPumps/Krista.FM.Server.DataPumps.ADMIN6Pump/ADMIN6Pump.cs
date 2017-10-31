using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.ADMIN6Pump
{
    // АДМИН - 0006 - Прогноз поступления акцизов
    public class ADMIN6PumpModule : CorrectedPumpModuleBase
    {
        #region Константы
        #endregion

        #region Структуры, перечисления
        #endregion Структуры, перечисления

        #region Поля

        #region Классификаторы

        //Районы.Планирование
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        //Вариант.АДМИН_Прогноз поступления акцизов (d.Variant.Excise)
        private IDbDataAdapter daVariantExcise;
        private DataSet dsVariantExcise;
        private IClassifier clsVariantExcise;
        private Dictionary<string, int> cacheVariantExcise = null;

        //Организации.АДМИН_Прогноз (d.Org.Excise)
        private IDbDataAdapter daOrgExcise;
        private DataSet dsOrgExcise;
        private IClassifier clsOrgExcise;
        private Dictionary<int, int> cacheOrgExcise = null;

        //Показатели.АДМИН_Прогноз (d.Marks.Excise)
        private IDbDataAdapter daMarksExcise;
        private DataSet dsMarksExcise;
        private IClassifier clsMarksExcise;
        private Dictionary<string, int> cacheMarksExcise = null;

        #endregion Классификаторы

        #region Факты
        //Показатели.АДМИН_Прогноз (f.Marks.Excise)
        private IDbDataAdapter daFctMarksExcise;
        private DataSet dsFctMarksExcise;
        private IFactTable fctMarksExcise;

        #endregion Факты

        int maxVariantCode = 0;
        int sourceID;
        private List<int> deletedSourceID = null;
        int nullRegions;


        private List<int> deletedRegionsList = null;

        #endregion Поля

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
                DirectDeleteFactData(new IFactTable[] { fctMarksExcise }, -1, sourceID, string.Empty);
                deletedSourceID.Add(sourceID);
            }
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constr = string.Format("SOURCEID = {0}", sourceID);

            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty, sourceID);
            InitDataSet(ref daVariantExcise, ref dsVariantExcise, clsVariantExcise, false, string.Empty, string.Empty);
            InitDataSet(ref daOrgExcise, ref dsOrgExcise, clsOrgExcise, false, string.Empty, string.Empty);
            InitDataSet(ref daMarksExcise, ref dsMarksExcise, clsMarksExcise, false, string.Empty, string.Empty);

            InitDataSet(ref daFctMarksExcise, ref dsFctMarksExcise, fctMarksExcise, false, constr, string.Empty);

            nullRegions = clsRegions.UpdateFixedRows(this.DB, sourceID);

            FillCaches();
            SetMaxCodes();
        }

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
            maxVariantCode = GetMaxCode(clsVariantExcise);
        }

        private void FillCaches()
        {
            FillRegionsCache(dsRegions.Tables[0]);
            FillRowsCache(ref cacheVariantExcise, dsVariantExcise.Tables[0], "Name", "ID");
            FillRowsCache(ref cacheOrgExcise, dsOrgExcise.Tables[0], "INN", "ID");
            FillRowsCache(ref cacheMarksExcise, dsMarksExcise.Tables[0], new string[] { "Code", "Name", "Persent" }, "|", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daVariantExcise, dsVariantExcise, clsVariantExcise);
            UpdateDataSet(daOrgExcise, dsOrgExcise, clsOrgExcise);
            UpdateDataSet(daMarksExcise, dsMarksExcise, clsMarksExcise);
            UpdateDataSet(daFctMarksExcise, dsFctMarksExcise, fctMarksExcise);
        }

        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string D_VARIANT_EXCISE_GUID = "089621fb-3750-4dc2-827b-7bc193cb0862";
        private const string D_ORG_EXCISE_GUID = "e70692d6-d413-43d4-b40e-aaa4d088776b";
        private const string D_MARKS_EXCISE_GUID = "09e9a701-1943-45d8-8324-c71bc7cd1fb4";

        private const string F_MARKS_EXCISE_GUID = "fa5002de-25f7-45d1-81d0-03a481466a38";

        protected override void InitDBObjects()
        {
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            clsVariantExcise = this.Scheme.Classifiers[D_VARIANT_EXCISE_GUID];
            clsOrgExcise = this.Scheme.Classifiers[D_ORG_EXCISE_GUID];
            clsMarksExcise = this.Scheme.Classifiers[D_MARKS_EXCISE_GUID];
            fctMarksExcise = this.Scheme.FactTables[F_MARKS_EXCISE_GUID];

            this.UsedClassifiers = new IClassifier[] {};
            this.UsedFacts = new IFactTable[] {};
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsVariantExcise);
            ClearDataSet(ref dsOrgExcise);
            ClearDataSet(ref dsMarksExcise);
            ClearDataSet(ref dsFctMarksExcise);
        }

        #endregion Работа с базой и кэшами

        #region Перекрытые методы закачки

        /// <summary>
        /// удаляет данные по дате и коду организации
        /// </summary>
        /// <param name="code"></param>
        /// 
        private void DeleteEarlierDataByRegions(int refRegions)
        {
            if (!(deletedRegionsList.Contains(refRegions)))
            {
                DirectDeleteFactData(new IFactTable[] { fctMarksExcise }, -1, sourceID, string.Format("RefRegions = {0}", refRegions));
                deletedRegionsList.Add(refRegions);
            }
        }

        private int GetRefRegions(string value)
        {
            string codeLine = string.Empty;
            codeLine = CommonRoutines.TrimLetters(value);
            if (Convert.ToInt32(codeLine.Substring(codeLine.Length - 2)) == 0)
                codeLine = codeLine.Substring(0, 3);

            if (!cacheRegions.ContainsKey(codeLine))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Анализ».", codeLine));
            }

            return cacheRegions[codeLine];
        }

        private int PumpOrg(int inn, string name)
        {
            if (cacheOrgExcise.ContainsKey(inn))
                return cacheOrgExcise[inn];

            return PumpCachedRow(cacheOrgExcise, dsOrgExcise.Tables[0], clsOrgExcise, inn, "INN", new object[] {
                "Name", name, "INN", inn});
        }

        private int GetRefDate(int col)
        {
            if ((col >= 8) && (col <= 12))
                return (this.DataSource.Year + 1)* 10000 + 100;
            if ((col >= 13) && (col <= 17))
                return (this.DataSource.Year + 2)* 10000 + 100;
            return (this.DataSource.Year + 3)* 10000 + 100;
        }

        private int PumpMarks(string code, string name, string percent)
        {
            string key = String.Format("{0}|{1}|{2}", Convert.ToInt64(code), name, percent);
            if (cacheMarksExcise.ContainsKey(key))
                return cacheMarksExcise[key];

            return PumpCachedRow(cacheMarksExcise, dsMarksExcise.Tables[0], clsMarksExcise, key, new object[] {
                "Name", name, "Code", code, "Persent", percent });
        }

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private void PumpFact(object []mapping)
        {
            PumpRow(dsFctMarksExcise.Tables[0], mapping);
            if (dsFctMarksExcise.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFctMarksExcise, ref dsFctMarksExcise);
            }
        }

        private void PumpXLSRow(ExcelHelper excelDoc, int curRow, int refVariant)
        {
            int refRegions = GetRefRegions(CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, 2).Trim()).Trim());
            if (!this.DeleteEarlierData)
                DeleteEarlierDataByRegions(refRegions);
                
            int refOrg = PumpOrg(Convert.ToInt32(excelDoc.GetValue(curRow, 4).Trim()), excelDoc.GetValue(curRow, 3));


            string code = excelDoc.GetValue(curRow, 5).Trim();
            string name = excelDoc.GetValue(curRow, 6).Trim();
            string percent = excelDoc.GetValue(curRow, 7).Trim();
            int refMarks = PumpMarks(code, name, percent);

            object[] xlsMapping = new object[] { "AlcoholNeeds", 0, "Dimension", 0, "SumExciseBMO", 0,
                                    "SumExciseFB", 0, "SumExcise", 0};

            int beginCol = 8;

            int refDate;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < xlsMapping.Length/2; j ++)
                {
                    xlsMapping[j*2+1] = CleanFactValue(excelDoc.GetValue(curRow, beginCol + j).Trim());
                }
                refDate = GetRefDate(beginCol);
                object[] mapping = (object[])CommonRoutines.ConcatArrays(xlsMapping, new object[] { "RefVariant", refVariant, "RefMaks", refMarks, "REFYEARDAYUNV", refDate, "RefOrg", refOrg, "RefRegions", refRegions, "SOURCEID", sourceID });
                PumpFact(mapping);

                beginCol += 5;
            }
        }

        private int PumpVariant(string name)
        {
            if (cacheVariantExcise.ContainsKey(name))
                return cacheVariantExcise[name];

            return PumpCachedRow(cacheVariantExcise, dsVariantExcise.Tables[0], clsVariantExcise, name, new object[] {
                "Name", name, "Code", ++maxVariantCode, "VariantComment", name });
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, string fileName)
        {
            int countRows = excelDoc.GetRowsCount();
            string sheetName = excelDoc.GetWorksheetName();
            string strValue;

            string name = string.Format("Дата сбора {0}_(данные за {1}–{2}гг.)", this.DataSource.Year, this.DataSource.Year + 1, this.DataSource.Year + 3);
            int refVariant = PumpVariant(name);

            for (int curRow = 15; ; curRow++ )
            {
                try
                {
                    SetProgress(countRows, curRow,
                        string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1} листа '{2}'", curRow, countRows, sheetName));
                    strValue = excelDoc.GetValue(curRow, 2).Trim();

                    if (strValue.ToUpper().Contains("ИТОГО"))
                        return;

                    if (strValue == string.Empty)
                    {
                        continue;
                    }

                    PumpXLSRow(excelDoc, curRow, refVariant);
                }

                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} листа '{3}' отчета {1} возникла ошибка ({2})",
                        curRow, fileName, ex.Message, sheetName), ex);
                }
            }
        }

        private int GetStartRow(int section)
        {
            if (section <= 0)
                return -1;
            return 16;
        }

        private int GetSectionIndex(string sectionName)
        {
            if (sectionName == "TITUL")
                return 0;
            else return Convert.ToInt32(CommonRoutines.RemoveLetters(sectionName.Trim()));
        }

        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);
                int wsCount = excelDoc.GetWorksheetsCount();

                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    string wsName = excelDoc.GetWorksheetName().Trim().ToUpper();
                    PumpXlsSheet(excelDoc, file.Name);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }

        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedRegionsList = new List<int>();
            try
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                UpdateData();
            }
            finally
            {
                deletedRegionsList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных
    }
}
