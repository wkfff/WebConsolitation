using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.STAT3Pump
{
    // стат 0003 - население_численность и состав
    public class STAT3PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ОК.Основные возрастные группы (d.OK.AgeGroup)
        private IDbDataAdapter daAgeGroup;
        private DataSet dsAgeGroup;
        private IClassifier clsAgeGroup;
        private Dictionary<string, int> cacheAgeGroup = null;
        private int nullAgeGroup;

        // ОК.Отдельные возрастные группы (d.OK.IndAgeGroup)
        private IDbDataAdapter daIndAgeGroup;
        private DataSet dsIndAgeGroup;
        private IClassifier clsIndAgeGroup;
        private Dictionary<string, int> cacheIndAgeGroup = null;
        private int nullIndAgeGroup;

        // ОК.Группировки (d.OK.Group)
        private IDbDataAdapter daGroup;
        private DataSet dsGroup;
        private IClassifier clsGroup;
        private Dictionary<string, int> cacheGroup = null;

        // Территории.РФ (d.Territory.RF)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory = null;
        private int nullTerritory;

        #endregion Классификаторы

        #region Факты

        // Население.СТАТ_Население_Численность и состав (f.People.ST3Const)
        private IDbDataAdapter daPeopleST3Const;
        private DataSet dsPeopleST3Const;
        private IFactTable fctPeopleST3Const;

        #endregion Факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int refDate = -1;
        // таблица разбита на три различного вида листа
        private SheetType sheetType;
        private int refTerritory;
        // таблица соответствий наименований территорий их ОКАТО
        // (в классификаторе хранятся иные наименования территорий с такими же ОКАТО)
        private Dictionary<string, string> territoryRenditionTable = new Dictionary<string, string>();
        private bool ignoreSheet = false;
        // данные закачиваются на источник СТАТ_0001_Вариант
        private int sourceId = -1;

        #endregion Поля

        #region Структуры, перечисления

        private enum SheetType
        {
            Beginning,
            Prolong,
            Finish,
            Unknown
        }

        #endregion Структуры, перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daAgeGroup, ref dsAgeGroup, clsAgeGroup, true, string.Empty, string.Empty);
            nullAgeGroup = clsAgeGroup.UpdateFixedRows(this.DB, sourceId);
            InitDataSet(ref daIndAgeGroup, ref dsIndAgeGroup, clsIndAgeGroup, true, string.Empty, string.Empty);
            nullIndAgeGroup = clsIndAgeGroup.UpdateFixedRows(this.DB, sourceId);
            InitDataSet(ref daGroup, ref dsGroup, clsGroup, true, string.Empty, string.Empty);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, true, string.Empty, string.Empty);
            string constr = string.Format("SOURCEID = {0}", sourceId);
            InitDataSet(ref daPeopleST3Const, ref dsPeopleST3Const, fctPeopleST3Const, false, constr, string.Empty);
            FillCaches();
        }

        private void FillRenditionTable()
        {
            territoryRenditionTable.Add("Г.САМАРАИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36401000");
            territoryRenditionTable.Add("Г.ТОЛЬЯТТИИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36440000");
            territoryRenditionTable.Add("Г.СЫЗРАНЬИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36435000");
            territoryRenditionTable.Add("Г.НОВОКУЙБЫШЕВСКИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36413000");
            territoryRenditionTable.Add("Г.ЖИГУЛЕВСКИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36404000");
            territoryRenditionTable.Add("Г.КИНЕЛЬИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36408000");
            territoryRenditionTable.Add("Г.ОКТЯБРЬСК", "36418000");
            territoryRenditionTable.Add("Г.ОТРАДНЫЙ", "36424000");
            territoryRenditionTable.Add("Г.ПОХВИСТНЕВОИПОДЧИНЕННЫЕЕГОАДМИНИСТРАЦИИНАСЕЛЕННЫЕПУНКТЫ", "36427000");
            territoryRenditionTable.Add("Г.ЧАПАЕВСК", "36450000");
            territoryRenditionTable.Add("АЛЕКСЕЕВСКИЙРАЙОН", "36202000");
            territoryRenditionTable.Add("БЕЗЕНЧУКСКИЙРАЙОН", "36204000");
            territoryRenditionTable.Add("БОГАТОВСКИЙРАЙОН", "36206000");
            territoryRenditionTable.Add("БОЛЬШЕГЛУШИЦКИЙРАЙОН", "36208000");
            territoryRenditionTable.Add("БОЛЬШЕЧЕРНИГОВСКИЙРАЙОН", "36210000");
            territoryRenditionTable.Add("БОРСКИЙРАЙОН", "36212000");
            territoryRenditionTable.Add("ВОЛЖСКИЙРАЙОН", "36214000");
            territoryRenditionTable.Add("ЕЛХОВСКИЙРАЙОН", "36215000");
            territoryRenditionTable.Add("ИСАКЛИНСКИЙРАЙОН", "36216000");
            territoryRenditionTable.Add("КАМЫШЛИНСКИЙРАЙОН", "36217000");
            territoryRenditionTable.Add("КИНЕЛЬСКИЙРАЙОН", "36218000");
            territoryRenditionTable.Add("КИНЕЛЬ-ЧЕРКАССКИЙРАЙОН", "36220000");
            territoryRenditionTable.Add("КЛЯВЛИНСКИЙРАЙОН", "36222000");
            territoryRenditionTable.Add("КОШКИНСКИЙРАЙОН", "36224000");
            territoryRenditionTable.Add("КРАСНОАРМЕЙСКИЙРАЙОН", "36226000");
            territoryRenditionTable.Add("КРАСНОЯРСКИЙРАЙОН", "36228000");
            territoryRenditionTable.Add("НЕФТЕГОРСКИЙРАЙОН", "36230000");
            territoryRenditionTable.Add("ПЕСТРАВСКИЙРАЙОН", "36232000");
            territoryRenditionTable.Add("ПОХВИСТНЕВСКИЙРАЙОН", "36234000");
            territoryRenditionTable.Add("ПРИВОЛЖСКИЙРАЙОН", "36236000");
            territoryRenditionTable.Add("СЕРГИЕВСКИЙРАЙОН", "36238000");
            territoryRenditionTable.Add("СТАВРОПОЛЬСКИЙРАЙОН", "36240000");
            territoryRenditionTable.Add("СЫЗРАНСКИЙРАЙОН", "36242000");
            territoryRenditionTable.Add("ХВОРОСТЯНСКИЙРАЙОН", "36244000");
            territoryRenditionTable.Add("ЧЕЛНО-ВЕРШИНСКИЙРАЙОН", "36246000");
            territoryRenditionTable.Add("ШЕНТАЛИНСКИЙРАЙОН", "36248000");
            territoryRenditionTable.Add("ШИГОНСКИЙРАЙОН", "36250000");
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheAgeGroup, dsAgeGroup.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheIndAgeGroup, dsIndAgeGroup.Tables[0], "NAME", "ID");
            FillRowsCache(ref cacheGroup, dsGroup.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], "OKATO", "ID");
            FillRenditionTable();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daPeopleST3Const, dsPeopleST3Const, fctPeopleST3Const);
        }
         
        private const string D_OK_AGEGROUP_GUID      = "0b20ad47-fa12-452f-9f12-194357b8c061";
        private const string D_OK_INDAGEGROUP_GUID   = "6a84acae-b2b4-44cc-af65-6715a9d3b906";
        private const string D_OK_GROUP_GUID         = "efff29b3-a585-4217-abc3-b2da1acc4fde";
        private const string D_TERRITORY_RF_GUID     = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string F_PEOPLE_ST3CONST_GUID  = "c5516868-bfbf-4d23-87ef-78d8eb7a6d38";
        protected override void InitDBObjects()
        {
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_RF_GUID];
            clsAgeGroup = this.Scheme.Classifiers[D_OK_AGEGROUP_GUID];
            clsIndAgeGroup = this.Scheme.Classifiers[D_OK_INDAGEGROUP_GUID];
            clsGroup = this.Scheme.Classifiers[D_OK_GROUP_GUID];
            fctPeopleST3Const = this.Scheme.FactTables[F_PEOPLE_ST3CONST_GUID];
            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { };
        }

        protected override void DeleteEarlierPumpedData()
        {
            sourceId = AddDataSource("СТАТ", "0001", ParamKindTypes.Variant, string.Empty, 0, 0, this.DataSource.Variant, 0, string.Empty).ID;
            DirectDeleteFactData(new IFactTable[] { fctPeopleST3Const }, -1, sourceId, string.Empty);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsAgeGroup);
            ClearDataSet(ref dsIndAgeGroup);
            ClearDataSet(ref dsGroup);
            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsPeopleST3Const);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Excel

        private void CheckClassifiers()
        {
            if (cacheAgeGroup.Count <= 1)
                throw new Exception("Не заполнен классификатор 'ОК.Основные возрастные группы'");
            if (cacheIndAgeGroup.Count <= 1)
                throw new Exception("Не заполнен классификатор 'ОК.Отдельные возрастные группы'");
            if (cacheGroup.Count <= 1)
                throw new Exception("Не заполнен классификатор 'ОК.Группировки'");
        }

        private void GetReportDate()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            try
            {
                string dateStr = this.DataSource.Variant.Trim();
                refDate = Convert.ToInt32(dateStr.Substring(dateStr.Length - 4));
                refDate = refDate * 10000 + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Не удалось определить дату отчёта ({0})", ex.Message), ex);
            }
        }

        private const string MARK_START = "ЧИСЛЕННОСТЬ НАСЕЛЕНИЯ";
        private const string MARK_PROLONG = "ПРОДОЛЖЕНИЕ";
        private const string MARK_FINISH = "ОКОНЧАНИЕ";
        private SheetType GetSheetType(object sheet)
        {
            string cellValue = excelHelper.GetCell(sheet, 2, 1).Value.Trim().ToUpper();
            if (cellValue.Contains(MARK_PROLONG))
                return SheetType.Prolong;
            else if (cellValue.Contains(MARK_FINISH))
                return SheetType.Finish;
            cellValue = excelHelper.GetCell(sheet, 2, 9).Value.Trim().ToUpper();
            if (cellValue.Contains(MARK_FINISH))
                return SheetType.Finish;
            cellValue = excelHelper.GetCell(sheet, 2, 10).Value.Trim().ToUpper();
            if (cellValue.Contains(MARK_FINISH))
                return SheetType.Finish;
            cellValue = excelHelper.GetCell(sheet, 1, 1).Value.Trim().ToUpper();
            if (cellValue.StartsWith(MARK_START))
            {
                ignoreSheet = false;
                return SheetType.Beginning;
            }
            return SheetType.Unknown;
        }

        private int GetFirstRow()
        {
            if (sheetType == SheetType.Beginning)
                return 8;
            return 6;
        }

        private int GetLastRow()
        {
            if (sheetType == SheetType.Beginning)
                return 48;
            else if (sheetType == SheetType.Prolong)
                return 46;
            return 42;
        }

        private int GetRefTerritory(object sheet)
        {
            int refTerritory = -1;
            string cellValue = excelHelper.GetCell(sheet, 2, 1).Value.Trim();
            string territoryName = cellValue;
            cellValue = cellValue.ToUpper().Replace(" ", string.Empty);
            if (territoryRenditionTable.ContainsKey(cellValue))
                refTerritory = FindCachedRow(cacheTerritory, territoryRenditionTable[cellValue], -1);
            if (refTerritory == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Не удалось найти запись '{0}' в классификаторе Территория.РФ. Лист '{1}' закачан не будет.",
                        territoryName, excelHelper.GetSheetName(sheet)));
                ignoreSheet = true;
            }
            return refTerritory;
        }

        private int GetRefIndAgeGroup(string name)
        {
            return FindCachedRow(cacheIndAgeGroup, name, nullIndAgeGroup);
        }

        private int[] REF_FX = new int[] { 2, 1, 2, 1 };
        private int[] REF_FX_LOCALITY_TYPES = new int[] { 1, 1, 2, 2 };
        private void PumpFactRow(DataRow row, int refIndAgeGroup, int refOK, int refOKGroup)
        {
            for (int curCol = 0; curCol < 4; curCol++)
            {
                string value = row[string.Format("Value{0}", curCol + 1)].ToString().Trim();
                if ((value == string.Empty) || (value == "-"))
                    continue;
                decimal valuePeriod = Convert.ToDecimal(value);
                object[] mapping = new object[] {
                    "SourceID", sourceId, "ValuePeriod", valuePeriod, "RefYearDayUNV", refDate,
                    "RefPeople", 1000000004, "RefTerritory", refTerritory, "RefOKDepartment", -1,
                    "RefFXLocalityTypes", REF_FX_LOCALITY_TYPES[curCol], "RefFX", REF_FX[curCol],
                    "RefOK", refOK, "RefOKGroup", refOKGroup, "RefOKIndAgeGroup", refIndAgeGroup };
                PumpRow(dsPeopleST3Const.Tables[0], mapping);
                if (dsPeopleST3Const.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daPeopleST3Const, dsPeopleST3Const.Tables[0]);
                }
            }
        }

        private object[] XLS_MAPPING = new object[]
            { "AgeGroup", 1, "Value1", 6, "Value2", 7, "Value3", 9, "Value4", 10 };
        private void PumpXlsSheetData(object sheet, FileInfo file)
        {
            if (sheetType == SheetType.Beginning)
            {
                refTerritory = GetRefTerritory(sheet);
                if (refTerritory == -1)
                    return;
            }

            int firstRow = GetFirstRow();
            int lastRow = GetLastRow();
            DataTable dt = excelHelper.GetSheetDataTable(sheet, firstRow, lastRow, XLS_MAPPING);
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            int rowsCount = dt.Rows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", firstRow + curRow, lastRow));

                    string ageGroup = dt.Rows[curRow]["AgeGroup"].ToString();
                    if (ageGroup.Contains("-"))
                        continue;

                    int refIndAgeGroup = GetRefIndAgeGroup(ageGroup.Trim());
                    PumpFactRow(dt.Rows[curRow], refIndAgeGroup, nullAgeGroup, 217);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})",
                        firstRow + curRow, ex.Message), ex);
                }
            }

            if (sheetType == SheetType.Finish)
            {
                dt = excelHelper.GetSheetDataTable(sheet, 44, 46, XLS_MAPPING);
                PumpFactRow(dt.Rows[0], nullIndAgeGroup, 2, 216);
                PumpFactRow(dt.Rows[1], nullIndAgeGroup, 3, 216);
                PumpFactRow(dt.Rows[2], nullIndAgeGroup, 4, 216);
            }
        }

        private void PumpXlsFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    sheetType = GetSheetType(sheet);
                    if (ignoreSheet || (sheetType == SheetType.Unknown))
                        continue;
                    PumpXlsSheetData(sheet, file);
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                GetReportDate();
                CheckClassifiers();
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
