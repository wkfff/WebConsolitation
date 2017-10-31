using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.MOFO28Pump
{

    // МОФО - 0028 - Начисление арендной платы
    public class MOFO28PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.МОФО_Доходные источники (d_Marks_Sourse)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<int, int> cacheMarks = null;
        private int nullMarks = -1;
        // Комментарии.МОФО_Начисление арендной платы (d_Note_Comment)
        private IDbDataAdapter daComment;
        private DataSet dsComment;
        private IClassifier clsComment;
        private Dictionary<string, int> cacheComment = null;
        // Районы.Планирование (d_Regions_Plan)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.МОФО_Начисление арендной платы (f_Marks_ChargeRent)
        private IDbDataAdapter daChargeRent;
        private DataSet dsChargeRent;
        private IFactTable fctChargeRent;

        #endregion Факты

        private int sourceID = -1;
        private List<int> deletedRegionsList = null;
        private List<int> deletedSourceIDList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой кэшами

        private void SetNewSourceID()
        {
            sourceID = this.AddDataSource("ФО", "0029", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;

            if (deletedSourceIDList == null)
                deletedSourceIDList = new List<int>();

            if (!deletedSourceIDList.Contains(sourceID) && this.DeleteEarlierData)
            {
                DirectDeleteFactData(new IFactTable[] { fctChargeRent }, -1, sourceID, string.Empty);
                deletedSourceIDList.Add(sourceID);
            }
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
                {
                    cacheRegions.Add(codeLine, Convert.ToInt32(row["ID"]));
                }
            }
        }

        private void FillCaches()
        {
            FillRegionsCache(dsRegions.Tables[0]);
            FillRowsCache(ref cacheComment, dsComment.Tables[0], "Name");
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CodeStr");
        }

        protected override void QueryData()
        {
            SetNewSourceID();

            string constraint = string.Format("SourceID = {0}", sourceID);
            InitDataSet(ref daRegions, ref dsRegions, clsRegions, constraint);
            InitDataSet(ref daMarks, ref dsMarks, clsMarks, string.Empty);
            InitDataSet(ref daComment, ref dsComment, clsComment, string.Empty);
            InitFactDataSet(ref daChargeRent, ref dsChargeRent, fctChargeRent);

            FillCaches();
        }

        #region GUIDs

        private const string D_MARKS_SOURSE_GUID = "c2bb8c4a-60f5-4234-8a66-e562c28ba22d";
        private const string D_NOTE_COMMENT_GUID = "48e62b12-7adc-482e-8e47-7c1f1baedf37";
        private const string D_REGIONS_PLAN_GUID = "1f34cc90-16fd-4fcf-b994-0c8a680d7e23";
        private const string F_MARKS_CHARGE_RENT_GUID = "53bc2d38-b411-4974-86cf-936268a92cf9";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsMarks = this.Scheme.Classifiers[D_MARKS_SOURSE_GUID];
            clsComment = this.Scheme.Classifiers[D_NOTE_COMMENT_GUID];
            clsRegions = this.Scheme.Classifiers[D_REGIONS_PLAN_GUID];
            fctChargeRent = this.Scheme.FactTables[F_MARKS_CHARGE_RENT_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctChargeRent };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daComment, dsComment, clsComment);
            UpdateDataSet(daChargeRent, dsChargeRent, fctChargeRent);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsChargeRent);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsComment);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой кэшами

        #region Работа с Xls

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            Decimal.TryParse(CommonRoutines.TrimLetters(value).Replace('.', ','), out factValue);
            return factValue;
        }

        private void DeleteEarlierDataByRegions(int refRegions)
        {
            if (!deletedRegionsList.Contains(refRegions) && !this.DeleteEarlierData)
            {
                string constr = string.Format("RefRegions = {0}", refRegions);
                DirectDeleteFactData(new IFactTable[] { fctChargeRent }, -1, sourceID, constr);
                deletedRegionsList.Add(refRegions);
            }
        }

        private int GetRefRegions(string value)
        {
            // код берем из названия муниципального образования, например:
            // 064  Городской округ  Реутов - код 064
            // 02904 Сельское поселение Данковское - код 02904
            string code = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];

            if (!cacheRegions.ContainsKey(code))
            {
                throw new PumpDataFailedException(string.Format(
                    "Не найдено муниципальное образование с кодом {0} в справочнике «Районы.Планирование».", code));
            }

            DeleteEarlierDataByRegions(cacheRegions[code]);

            return cacheRegions[code];
        }

        private int PumpComment(string name)
        {
            name = name.Trim();
            return PumpCachedRow(cacheComment, dsComment.Tables[0], clsComment, name, new object[] { "Name", name });
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int sectionIndex, int refDate, int marksCode)
        {
            object[] mapping = null;
            switch (sectionIndex)
            {
                case 1:
                    mapping = new object[] {
                        "Amount", CleanFactValue(excelDoc.GetValue(curRow, 3)),
                        "RentArea", CleanFactValue(excelDoc.GetValue(curRow, 4)),
                        "ChargeAnnual", CleanFactValue(excelDoc.GetValue(curRow, 5)) * 1000,
                        "Facility", CleanFactValue(excelDoc.GetValue(curRow, 6)) * 1000,
                        "FactArrivalAnnual", CleanFactValue(excelDoc.GetValue(curRow, 7)) * 1000,
                        "RefRegions", GetRefRegions(excelDoc.GetValue(curRow, 2)),
                        "RefComment", PumpComment(excelDoc.GetValue(curRow, 8)),
                        "RefMarks", FindCachedRow(cacheMarks, marksCode, nullMarks),
                        "RefYearDayUNV", refDate,
                        "SourceID", sourceID
                    };
                    break;
                case 2:
                    mapping = new object[] {
                        "BorrowArea", CleanFactValue(excelDoc.GetValue(curRow, 4)),
                        "ChargeAnnual", CleanFactValue(excelDoc.GetValue(curRow, 5)) * 1000,
                        "Facility", CleanFactValue(excelDoc.GetValue(curRow, 6)) * 1000,
                        "FactArrivalAnnual", CleanFactValue(excelDoc.GetValue(curRow, 7)) * 1000,
                        "FactArrivalOMSY", CleanFactValue(excelDoc.GetValue(curRow, 8)) * 1000,
                        "RefRegions", GetRefRegions(excelDoc.GetValue(curRow, 3)),
                        "RefComment", PumpComment(excelDoc.GetValue(curRow, 9)),
                        "RefMarks", FindCachedRow(cacheMarks, marksCode, nullMarks),
                        "RefYearDayUNV", refDate,
                        "SourceID", sourceID
                    };
                    break;
            }

            PumpRow(dsChargeRent.Tables[0], mapping);
            if (dsChargeRent.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daChargeRent, ref dsChargeRent);
            }
        }

        private int GetSectionIndex(string sheetName)
        {
            sheetName = sheetName.ToUpper();
            if (sheetName.Contains("11"))
                return 1;
            if (sheetName.Contains("12"))
                return 2;
            return -1;
        }

        private void GetSectionBorders(ExcelHelper excelDoc, ref int firstRow, ref int lastRow)
        {
            bool toStart = false;
            for (int curRow = 1; ; curRow++)
            {
                string cellValue = excelDoc.GetValue(curRow, 2).Trim();

                if (toStart)
                {
                    if (cellValue.ToUpper().StartsWith("ГЛАВА"))
                    {
                        lastRow = curRow - 1;
                        return;
                    }
                    continue;
                }

                if (cellValue == "1")
                {
                    toStart = true;
                    firstRow = curRow + 1;
                }
            }
        }

        private void SetMarksCode(ExcelHelper excelDoc, int curRow, int sectionIndex, ref int marksCode)
        {
            if (sectionIndex == 1)
                return;

            string cellValue = excelDoc.GetValue(curRow, 1).Trim();
            if (cellValue == string.Empty)
                return;

            marksCode = Convert.ToInt32(cellValue);
        }

        private bool SkipRow(ExcelHelper excelDoc, int curRow, int sectionIndex)
        {
            switch (sectionIndex)
            {
                case 1:
                    string cellValue = excelDoc.GetValue(curRow, 2).Trim().ToUpper();
                    return (
                        (cellValue == string.Empty) ||
                        (cellValue == "МУНИЦИПАЛЬНЫЙ РАЙОН, ВСЕГО") ||
                        cellValue.Contains("В ТОМ ЧИСЛЕ"));
                case 2:
                    return (
                        (excelDoc.GetValue(curRow, 2).Trim() != string.Empty)  ||
                        (excelDoc.GetValue(curRow, 3).Trim() == string.Empty) ||
                        excelDoc.GetValue(curRow, 3).Trim().ToUpper().Contains("В ТОМ ЧИСЛЕ"));
            }
            return true;
        }

        private void PumpXlsSheet(ExcelHelper excelDoc, int refDate)
        {
            int sectionIndex = GetSectionIndex(excelDoc.GetWorksheetName());

            int marksCode = sectionIndex == 1 ? 1 : 2;

            int firstRow = 0;
            int lastRow = 0;
            GetSectionBorders(excelDoc, ref firstRow, ref lastRow);
            for (int curRow = firstRow; curRow <= lastRow; curRow++)
                try
                {
                    SetMarksCode(excelDoc, curRow, sectionIndex, ref marksCode);

                    if (SkipRow(excelDoc, curRow, sectionIndex))
                        continue;

                    PumpXlsRow(excelDoc, curRow, sectionIndex, refDate, marksCode);
                }
                catch (Exception ex)
                {
                    throw new PumpDataFailedException(string.Format(
                        "При обработке листа '{0}' возникла ошибка ({1})",
                        excelDoc.GetWorksheetName(), ex.Message));
                }
        }

        private bool SkipSheet(string sheetname)
        {
            return !sheetname.Trim().ToUpper().StartsWith("ПРИЛОЖЕНИЕ");
        }

        private int GetRefDate(string filename)
        {
            // из названия файла Nach_arendaГГ_ААА.xls берем год - ГГ
            int year = Convert.ToInt32(CommonRoutines.TrimLetters(filename.Split('_')[1])) + 2000;
            return year * 10000 + 1;
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                int refDate = GetRefDate(file.Name);

                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.EnableEvents = false;
                excelDoc.OpenDocument(file.FullName);

                for (int index = 1; index <= excelDoc.GetWorksheetsCount(); index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (SkipSheet(excelDoc.GetWorksheetName()))
                        continue;
                    PumpXlsSheet(excelDoc, refDate);
                }
            }
            catch (PumpDataFailedException ex)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                    "Ошибка при обработке файла '{0}': {1} Файл будет пропущен.", file.Name, ex.Message));
            }
            finally
            {
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
            deletedRegionsList = new List<int>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
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

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
