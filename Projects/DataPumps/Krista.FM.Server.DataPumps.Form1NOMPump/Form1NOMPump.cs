using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.Form1NOMPump
{
    // ФНС - 0015 - Форма 1-НОМ 
    public class Form1NOMPumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ОКВЭД.ФНС (d_OKVED_FNS)
        private IDbDataAdapter daOKVED;
        private DataSet dsOKVED;
        private IClassifier clsOKVED;
        private Dictionary<string, int> okvedCache = null;
        private int nullOKVED;
        // Доходы.Группы ФНС (d_D_GroupFNS)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IClassifier clsIncomes;
        private Dictionary<string, int> incomesCache = null;
        private int nullIncomes;
        // Районы.ФНС (d_Regions_FNS)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionsCache = null;
        private int nullRegions;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_1 НОМ_Сводный (f_D_FNS1NOMTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_1 НОМ_Районы (f_D_FNS1NOMRegions)
        private IDbDataAdapter daIncomesRegion;
        private DataSet dsIncomesRegion;
        private IFactTable fctIncomesRegion;

        #endregion Факты

        private List<string> strCodeList;
        private ReportType reportType;
        private double[] controlSums = null;
        private decimal[] totalSums = new decimal[13];

        #endregion Поля

        #region Структуры, перечисления

        // тип отчета
        private enum ReportType
        {
            Svod,
            Str,
            Region
        }

        #endregion Структуры, перечисления

        #region Константы

        // списки кодов строк
        private string[] strCodes2005 = new string[] { "1010", "1015", "1020", "1025", "1030", "1035", "1040", "1045", "1050", "1055", 
                "1060", "1065", "1070", "1075", "1080", "1085", "1090", "1095", "1100", "1105", "1110", "1115", "1120", "1125", "1130",
                "1135", "1140", "1145", "1150", "1155", "1160", "1165", "1170", "1175", "1180", "1190", "1195", "1200", "1205", "1210",
                "1220", "1230", "1240", "1250", "1251", "1260", "1270", "1280", "1290", "1300", "1310", "1320", "1330", "1340", "1350",
                "1360", "1370", "1380", "1400" };
        private string[] strCodes2007 = new string[] { "1010", "1015", "1020", "1025", "1030", "1035", "1040", "1045", "1050", "1055", 
                "1060", "1065", "1070", "1075", "1080", "1085", "1090", "1095", "1100", "1105", "1110", "1115", "1120", "1125", "1130", 
                "1135", "1140", "1145", "1150", "1155", "1160", "1165", "1170", "1175", "1180", "1190", "1195", "1200", "1205", "1210",
                "1215", "1220", "1225", "1240", "1245", "1250", "1255", "1270", "1280", "1285", "1290", "1300", "1305", "1315", "1320",
                "1325", "1330", "1340", "1345", "1350", "1355", "1370", "1375", "1380", "1390", "1400", "1410", "1420", "1430", "1440",
                "1450", "1500", "1510", "2010", "2015", "2020", "2025", "2030", "2035", "2040", "2045", "2050", "2055", "2060", "2065",
                "2070", "2075", "2080", "2085", "2090", "2095", "2100", "2105", "2110", "2115", "2120", "2125", "2130", "2135", "2140",
                "2145", "2150", "2155", "2160", "2165", "2170", "2175", "2185", "2190", "2195", "2200", "2205", "2210", "2215", "2220",
                "2225", "2240", "2245", "2250", "2255", "2270", "2280", "2285", "2290", "2300", "2305", "2315", "2320", "2325", "2330",
                "2340", "2345", "2350", "2355", "2370", "2375", "2380", "2390", "2400", "2410", "2420", "2430", "2440", "2450", "2500", 
                "2510" };
        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        // сслылки на доходы.группы ФНС: индекс столбца в отчете; код доходов фнс, на которое закачиваем 
        private int[] incomesRefsSection1Const = new int[] { 1, 0, 2, 0, 3, 100000000, 4, 101000000, 5, 101010000, 6, 102000000, 
            7, 108000000, 8, 103000000, 9, 103010000, 10, 104000000, 11, 200000000, 12, 300000000, 13, 400000000 };
        private int[] incomesRefsSection1Const_2011 = new int[] { 1, 0, 2, 0, 3, 100000000, 4, 101000000, 5, 101010000, 6, 109000000, 
            7, 102000000, 8, 108000000, 9, 103000000, 10, 103010000, 11, 104000000, 12, 200000000, 13, 201000000, 14, 202000000, 15, 300000000, 16, 400000000 };
        private int[] incomesRefsSection2Const = new int[] { 1, 109000000, 2, 109000000, 3, 109010000, 4, 109010100, 5, 109010200, 
            6, 109010300, 7, 109010400, 8, 109020000, 9, 109020100, 10, 109020200, 11, 109020300, 12, 109020400, 13, 109030000, 
            14, 109030100, 15, 109030200, 16, 109030300, 17, 109030400 };
        private int[] incomesRefsSection1 = null;
        private int[] incomesRefsSection2 = null;

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void GetIncomesRefs(int[] incomesRefs)
        {
            int count = incomesRefs.GetLength(0);
            for (int i = 0; i < count; i += 2)
                incomesRefs[i + 1] = FindCachedRow(incomesCache, incomesRefs[i + 1].ToString(), nullIncomes);
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daOKVED, ref dsOKVED, clsOKVED);
            nullOKVED = clsOKVED.UpdateFixedRows(this.DB, this.SourceID);
            int incomesSourceId = AddDataSource("ФНС", "0015", ParamKindTypes.YearMonth, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitDataSet(ref daIncomes, ref dsIncomes, clsIncomes, false, string.Format("SOURCEID = {0}", incomesSourceId), string.Empty);
            nullIncomes = clsIncomes.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
            if (this.DataSource.Year >= 2011)
                GetIncomesRefs(incomesRefsSection1 = (int[])incomesRefsSection1Const_2011.Clone());
            else GetIncomesRefs(incomesRefsSection1 = (int[])incomesRefsSection1Const.Clone());
            if (this.DataSource.Year >= 2007)
                GetIncomesRefs(incomesRefsSection2 = (int[])incomesRefsSection2Const.Clone());
        }

        private void FillCaches()
        {
            FillRowsCache(ref okvedCache, dsOKVED.Tables[0], "RowCode", "ID");
            FillRowsCache(ref incomesCache, dsIncomes.Tables[0], "CODE", "ID");
            FillRowsCache(ref regionsCache, dsRegions.Tables[0], "CODE", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOKVED, dsOKVED, clsOKVED);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_D_GROUP_FNS_GUID = "b9169eb6-de81-420b-8a2b-05ffa2fd35c1";
        private const string D_OKVED_FNS_GUID = "9f549d45-9e27-4c0a-948e-b99294de79bf";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string F_D_FNS_1NOM_TOTAL_GUID = "285e06aa-f281-4945-b50e-9876f89424d5";
        private const string F_D_FNS_1NOM_REGIONS_GUID = "167b2c71-2549-447a-a177-75fd16e21db6";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsOKVED = this.Scheme.Classifiers[D_OKVED_FNS_GUID], 
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID] };

            this.AssociateClassifiersEx = new IClassifier[] {
                clsIncomes = this.Scheme.Classifiers[D_D_GROUP_FNS_GUID] };

            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_1NOM_TOTAL_GUID], 
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_1NOM_REGIONS_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsOKVED);
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции

        private void CheckOKVED()
        {
            // для отчетов в разрезе районов и строк классификатор ОКВЭД.фнс должен быть заполнен
            if ((reportType != ReportType.Svod) && (okvedCache.Count == 0))
                throw new Exception("Не заполнен классификатор ОКВЭД.ФНС - закачайте сводные отчеты");
        }

        private void CheckIncomes()
        {
            // если не заполнен классификатор Доходы.группы ФНС - предупреждение
            if (incomesCache.Count <= 1)
                throw new Exception("Не заполнен классификатор 'Доходы.Группы ФНС'. Данные по этому источнику закачаны не будут.");
        }

        private void CheckDirectories(DirectoryInfo dir)
        {
            DirectoryInfo[] svod = dir.GetDirectories(constSvodDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] str = dir.GetDirectories(constStrDirName, SearchOption.TopDirectoryOnly);
            DirectoryInfo[] reg = dir.GetDirectories(constRegDirName, SearchOption.TopDirectoryOnly);
            // Каталог "Сводный" должен присутствовать
            if (svod.GetLength(0) == 0)
            {
                dir.CreateSubdirectory(constSvodDirName);
                throw new Exception(string.Format("Отсутствует каталог \"{0}\"", constSvodDirName));
            }
            if (str.GetLength(0) == 0)
                dir.CreateSubdirectory(constStrDirName);
            if (reg.GetLength(0) == 0)
                dir.CreateSubdirectory(constRegDirName);
            // Каталоги Строки и Районы для одного месяца не могут быть заполнены одновременно
            if ((str.GetLength(0) > 0 && str[0].GetFiles().GetLength(0) > 0) &&
                (reg.GetLength(0) > 0 && reg[0].GetFiles().GetLength(0) > 0))
                throw new Exception("Каталоги \"Строки\" и \"Районы\" для одного месяца не могут быть заполнены одновременно");
        }

        private void SetClsHierarchy()
        {
            string d_OKVED_FNS15_HierarchyFileName = string.Empty;
            if (this.DataSource.Year >= 2007)
                d_OKVED_FNS15_HierarchyFileName = const_d_OKVED_FNS15_HierarchyFile2007;
            else if (this.DataSource.Year == 2006)
                d_OKVED_FNS15_HierarchyFileName = const_d_OKVED_FNS15_HierarchyFile2006;
            else
                d_OKVED_FNS15_HierarchyFileName = const_d_OKVED_FNS15_HierarchyFile2005;
            SetClsHierarchy(clsOKVED, ref dsOKVED, "RowCode", d_OKVED_FNS15_HierarchyFileName, ClsHierarchyMode.Special);
        }

        private void SetStrCodeList()
        {
            strCodeList = new List<string>();
            if (this.DataSource.Year >= 2007)
                strCodeList.AddRange(strCodes2007);
            else
            {
                strCodeList.AddRange(strCodes2005);
                if (this.DataSource.Year >= 2006)
                {
                    // с 2006 года добавляется код 1500
                    strCodeList.Add("1500");
                    // со второго полугодия 2006 года добавляется код 1510
                    if (!((this.DataSource.Year == 2006) && (this.DataSource.Month <= 6)))
                        strCodeList.Add("1510");
                }
            }
        }

        private void ShowAbsentStrCodes()
        {
            if (strCodeList.Count != 0)
            {
                string codes = string.Empty;
                foreach (string code in strCodeList)
                    codes += code + ", ";
                codes = codes.Remove(codes.Length - 2);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                  string.Format("Отсутствует файл в разрезе районов по кодам строк ({0})", codes));
            }
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            if (dir.GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) > 0)
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);

            if (dir.GetFiles("*.txt", SearchOption.AllDirectories).GetLength(0) > 0)
                ProcessFilesTemplate(dir, "*.txt", new ProcessFileDelegate(PumpTxtFile), false);

            // если есть распаковываем архивы rar
            FileInfo[] rarFiles = dir.GetFiles("*.rar", SearchOption.AllDirectories);
            foreach (FileInfo rarFile in rarFiles)
            {
                DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(rarFile.FullName,
                    FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
                try
                {
                    ProcessAllFiles(tempDir);
                }
                finally
                {
                    CommonRoutines.DeleteDirectory(tempDir);
                }
            }
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            CheckIncomes();
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            // reportType = ReportType.Region;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            // ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            reportType = ReportType.Str;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            // должны быть представлены все коды по строкам разделов, заполняем список кодов и проверяем на их присутствие в файлах
            SetStrCodeList();
            try
            {
                if ((dir.GetDirectories(constStrDirName)[0].GetFiles("*.xls", SearchOption.AllDirectories).GetLength(0) == 0) &&
                    (dir.GetDirectories(constStrDirName)[0].GetFiles("*.txt", SearchOption.AllDirectories).GetLength(0) == 0))
                    strCodeList.Clear();
                ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
                // оставшиеся коды в списке отсутствовали в файлах, предупреждаем
                ShowAbsentStrCodes();
            }
            finally
            {
                strCodeList.Clear();
            }
        }

        #endregion Общие функции

        #region работа с Excel

        private int PumpXlsOkved(ExcelHelper excelDoc, int curRow, ref string strCode)
        {
            string okvedName = excelDoc.GetValue(curRow, 1).Trim();
            if (okvedName.Length > 255)
                okvedName = okvedName.Substring(0, 255);
            string okvedCode = excelDoc.GetValue(curRow, 2).Trim();
            strCode = excelDoc.GetValue(curRow, 3).Trim();
            object[] mapping = new object[] { "NAME", okvedName, "CodeStr", okvedCode, "RowCode", strCode };
            return PumpCachedRow(okvedCache, dsOKVED.Tables[0], clsOKVED, mapping, strCode, "ID");
        }

        private const string SHIT_REGION_NAME1 = "УМНС";
        private const string SHIT_REGION_NAME2 = "УФНС";
        private const string OTHER_REGIONS = "DR_REG";
        private int PumpXlsRegions(ExcelHelper excelDoc, int curRow)
        {
            string regionName = excelDoc.GetValue(curRow, 1).Trim();
            string regionCode = excelDoc.GetValue(curRow, 2).Trim();
            if ((this.Region == RegionName.Krasnodar) || (this.Region == RegionName.Samara))
            {
                if (regionCode.ToUpper() == OTHER_REGIONS)
                    regionCode = "1111";
                else
                {
                    // заменяем все не цифры нулями
                    Regex rx = new Regex(@"\D", RegexOptions.IgnoreCase);
                    regionCode = rx.Replace(regionCode, "0");
                }
            }
            else
            {
                // есть такие хуевые районы - меняем код на ноль
                if ((regionName == SHIT_REGION_NAME1) || (regionName == SHIT_REGION_NAME2))
                    regionCode = "0";
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            return PumpCachedRow(regionsCache, dsRegions.Tables[0], clsRegions, mapping, regionCode, "ID");
        }

        private int GetSumColumnStartIndex()
        {
            if (reportType == ReportType.Svod)
                return 3;
            return 2;
        }

        private DataTable GetFactDataTable()
        {
            if (reportType == ReportType.Svod)
                return dsIncomesTotal.Tables[0];
            return dsIncomesRegion.Tables[0];
        }

        private int[] GetIncomesRefsByStrCode(int strCode)
        {
            if ((this.DataSource.Year < 2007) || (strCode < 2000))
                return incomesRefsSection1;
            return incomesRefsSection2;
        }

        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, string strCode)
        {
            if (excelDoc.GetValue(curRow, 3).Trim() == string.Empty)
                return;

            int okvedId = nullOKVED;
            if (reportType == ReportType.Svod)
                okvedId = PumpXlsOkved(excelDoc, curRow, ref strCode);

            int regionId = nullRegions;
            if (reportType == ReportType.Str)
            {
                regionId = PumpXlsRegions(excelDoc, curRow);
                okvedId = FindCachedRow(okvedCache, strCode, nullOKVED);
            }

            object[] sumsMapping = null;
            int[] incomesRefs = GetIncomesRefsByStrCode(Convert.ToInt32(strCode));
            int count = incomesRefs.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                string cellValue = CommonRoutines.TrimLetters(
                    excelDoc.GetValue(curRow, incomesRefs[i] + GetSumColumnStartIndex()).Trim());
                if (cellValue == string.Empty)
                    continue;
                double value = Convert.ToDouble(cellValue) * 1000;
                controlSums[i / 2] += value;
                // первые две суммы - в одну запись фактов
                if (i == 0)
                {
                    sumsMapping = new object[] { "EarnedReport", value };
                    continue;
                }
                else if (i == 2)
                    sumsMapping = (object[])CommonRoutines.ConcatArrays(sumsMapping, new object[] { "InpaymentsReport", value });
                else
                    sumsMapping = new object[] { "InpaymentsReport", value, "EarnedReport", 0 };
                object[] mapping = new object[] { "RefYearDayUNV", refDate, "RefOKVED", okvedId, "RefD", incomesRefs[i + 1] };
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, sumsMapping);
                if (reportType == ReportType.Str)
                    mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", regionId });
                PumpRow(GetFactDataTable(), mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
        }

        private const string TABLE_END_TEXT_SVOD = "КОНТРОЛЬНАЯ СУММА";
        private const string TABLE_END_TEXT_STR = "ВСЕГО";
        private bool IsTableEnd(string cellValue)
        {
            if (reportType == ReportType.Svod)
                return (cellValue.StartsWith(TABLE_END_TEXT_SVOD));
            return (cellValue.StartsWith(TABLE_END_TEXT_STR));
        }

        private const string REPORT_END_TEXT = "РУКОВОДИТЕЛ";
        private bool IsReportEnd(string cellValue)
        {
            return (cellValue.Contains(REPORT_END_TEXT));
        }

        private bool IsControlSum(string cellValue)
        {
            return (cellValue.Contains("КОНТРОЛЬН") && cellValue.Contains("СУММ"));
        }

        private const string STRCODE_TEXT = "РАЗРЕЗ ПО СТРОКЕ";
        private bool IsStrCode(string cellValue)
        {
            return (cellValue.StartsWith(STRCODE_TEXT));
        }

        private const string TABLE_TITLE_TEXT = "А";
        private bool IsTableTitle(string cellValue)
        {
            return (string.Compare(cellValue, TABLE_TITLE_TEXT, true) == 0);
        }

        private void CheckXlsControlSum(ExcelHelper excelDoc, int curRow, string strCode)
        {
            int[] incomesRefs = GetIncomesRefsByStrCode(Convert.ToInt32(strCode));
            int count = incomesRefs.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                int curCol = incomesRefs[i] + GetSumColumnStartIndex();
                string cellValue = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow, curCol).Trim());
                if (cellValue == string.Empty)
                    continue;
                double value = Convert.ToDouble(cellValue) * 1000;
                if (value != controlSums[i / 2])
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "сумма строк {0} не совпадает с контрольной {1}. Столбец '{2}'",
                        value, controlSums[i / 2], curCol));
                }
            }
        }

        private void PumpXlsSheetData(string fileName, ExcelHelper excelDoc, int refDate)
        {
            string strCode = string.Empty;
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
            bool dontPump = false;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}...", fileName),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));
                    string cellValue = excelDoc.GetValue(curRow, 1).Trim().ToUpper();
                    if (cellValue == string.Empty)
                        toPumpRow = false;

                    if (IsReportEnd(cellValue))
                        return;
                    if ((reportType == ReportType.Str) && (IsStrCode(cellValue)))
                    {
                        strCode = excelDoc.GetValue(curRow + 1, 1).Split('-')[0].Trim();
                        dontPump = false;
                        if (strCode == string.Empty)
                            continue;
                        strCodeList.Remove(strCode);
                    }

                    if (IsTableEnd(cellValue))
                    {
                        toPumpRow = false;
                        // для сводного файла передаем максимальное кол - во столбцов в таблице отчета (второй раздел)
                        if (reportType == ReportType.Svod)
                            strCode = "2000";
                        if (!dontPump)
                            CheckXlsControlSum(excelDoc, curRow, strCode);
                        strCode = string.Empty;
                    }

                    if (!dontPump && IsControlSum(cellValue) && (this.Region == RegionName.Samara))
                        dontPump = true;

                    if ((toPumpRow) && (!dontPump))
                        PumpXlsRow(excelDoc, curRow, refDate, strCode);
                    if (IsTableTitle(cellValue))
                    {
                        controlSums = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                        toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
        }

        private const string TITLE_SHEET_NAME = "ТИТУЛЬНЫЙ ЛИСТ";
        private bool IsTitleXlsSheet(string sheetName)
        {
            return (sheetName.Trim().ToUpper() == TITLE_SHEET_NAME);
        }

        private const string DATE_CELL_TEXT = "ПО СОСТОЯНИЮ НА";
        private int GetXlsReportDateRow(ExcelHelper excelDoc)
        {
            if (IsTitleXlsSheet(excelDoc.GetWorksheetName()))
            {
                // пытаемся найти дату на титульном листе в диапазоне ячеек A10..A16
                for (int rowIndex = 10; rowIndex <= 16; rowIndex++)
                {
                    string cellText = excelDoc.GetValue(rowIndex, 1).Trim().ToUpper();
                    if (cellText.Contains(DATE_CELL_TEXT))
                        return rowIndex;
                }
            }
            else
            {
                // пытаемся найти дату в диапазоне ячеек A4..A14
                for (int rowIndex = 4; rowIndex <= 14; rowIndex++)
                {
                    string cellText = excelDoc.GetValue(rowIndex, 1).Trim().ToUpper();
                    if (cellText.Contains(DATE_CELL_TEXT))
                        return rowIndex;
                }
            }
            return -1;
        }

        private int GetXlsReportDate(ExcelHelper excelDoc)
        {
            int refDate = -1;
            int reportDateRow = GetXlsReportDateRow(excelDoc);
            if (reportDateRow > 0)
            {
                if (IsTitleXlsSheet(excelDoc.GetWorksheetName()))
                {
                    // на титульном листе дата хранится в двух ячейках: в одной день и месяц, во второй год
                    string cellValue = string.Format("{0} {1}",
                        excelDoc.GetValue(reportDateRow, 2).Trim(),
                        excelDoc.GetValue(reportDateRow, 5).Trim());
                    refDate = Convert.ToInt32(CommonRoutines.LongDateToNewDate(cellValue));
                }
                else
                {
                    string cellValue = excelDoc.GetValue(reportDateRow, 1).Trim();
                    refDate = CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(cellValue));
                }
                refDate = CommonRoutines.DecrementDate(refDate);
            }
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Concat(
                    "Не удалось найти дату отчета в диапазоне ячеек А4..А14 или неправильно задана подстрока для поиска даты 'по состоянию на'",
                    ", дата будет определена параметрами источника."));
                refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            }
            CheckDataSourceByDate(refDate, true);
            return refDate;
        }

        private void PumpXlsFile(FileInfo file)
        {
            CheckOKVED();
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetXlsReportDate(excelDoc);
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (IsTitleXlsSheet(excelDoc.GetWorksheetName()))
                        continue;
                    PumpXlsSheetData(file.Name, excelDoc, refDate);
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion работа с Excel

        #region Работа с Txt

        private void SetNullTotalSum()
        {
            int count = totalSums.GetLength(0);
            for (int i = 0; i < count; i++)
                totalSums[i] = 0;
        }

        private string CleanFactValue(string factValue)
        {
            return factValue.Replace(" ", string.Empty).PadLeft(1, '0');
        }

        // проверка итоговой суммы в текстовых файлах
        private void CheckTxtTotalSum(string[] reportRow)
        {
            for (int i = 0; i < 13; i++)
            {
                decimal controlValue = Convert.ToDecimal(CleanFactValue(reportRow[i + 3]));
                if (controlValue != totalSums[i])
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Контрольная сумма {0} не совпадает с итоговой {1}. Столбец '{2}'",
                        controlSums, totalSums[i], i + 3));
                }
            }
        }

        private int PumpTxtOkved(string[] reportRow, string okvedName, ref string rowCode)
        {
            if (okvedName.Length > 255)
                okvedName = okvedName.Substring(0, 255);
            rowCode = reportRow[2].Trim();
            object[] mapping = new object[] { "NAME", okvedName, "RowCode", rowCode };
            return PumpCachedRow(okvedCache, dsOKVED.Tables[0], clsOKVED, mapping, rowCode, "ID");
        }

        private void PumpTxtRow(string[] reportRow, string okvedName, int refDate)
        {
            string strCode = string.Empty;
            int refOkved = PumpTxtOkved(reportRow, okvedName, ref strCode);
            int[] refIncomes = GetIncomesRefsByStrCode(Convert.ToInt32(strCode));
            for (int i = 0; i < 13; i++)
            {
                decimal factValue = Convert.ToDecimal(CleanFactValue(reportRow[i + 3]));
                totalSums[i] += factValue;

                object[] mapping = null;
                if (i > 0)
                {
                    mapping = new object[] { "InpaymentsReport", factValue, "RefOKVED", refOkved,
                        "RefD", refIncomes[i * 2 + 1], "RefYearDayUNV", refDate };
                }
                else
                {
                    mapping = new object[] { "EarnedReport", factValue, "RefOKVED", refOkved,
                        "RefD", refIncomes[i * 2 + 1], "RefYearDayUNV", refDate };
                }

                PumpRow(dsIncomesTotal.Tables[0], mapping);
                if (dsIncomesTotal.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesTotal, ref dsIncomesTotal);
                }
            }
        }

        private int GetReportDate(string[] txtReport)
        {
            int refDate = -1;
            for (int rowIndex = 1; rowIndex <= 19; rowIndex++)
            {
                string rowText = txtReport[rowIndex].Trim();
                if (rowText.ToUpper().Contains(DATE_CELL_TEXT.ToUpper()))
                {
                    refDate = CommonRoutines.ShortDateToNewDate(CommonRoutines.TrimLetters(rowText));
                    refDate = CommonRoutines.DecrementDate(refDate);
                    break;
                }
            }
            if (refDate == -1)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    String.Concat("Не удалось найти дату отчета в диапазоне строк 1..19",
                        " или неправильно задана подстрока для поиска даты 'по состоянию на',",
                        " дата будет определена параметрами источника."));
                refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            }
            CheckDataSourceByDate(refDate, true);
            return refDate;
        }

        // код разделителя "|"
        private const int DELIMETER_CODE = 9474;
        private char DELIMETER = Convert.ToChar(DELIMETER_CODE);
        private void PumpTxtReport(FileInfo file)
        {
            string[] txtReport = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtDosCodePage());
            int refDate = GetReportDate(txtReport);
            string okvedName = string.Empty;
            bool toPumpRow = false;
            int rowsCount = txtReport.Length;
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = 0; curRow < rowsCount - 1; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string[] reportRow = txtReport[curRow].Trim().Split(DELIMETER);
                    if (reportRow.GetLength(0) <= 1)
                    {
                        continue;
                    }

                    if (reportRow[1].Trim().StartsWith(TABLE_END_TEXT_SVOD))
                    {
                        CheckTxtTotalSum(reportRow);
                        toPumpRow = false;
                    }

                    if (toPumpRow)
                    {
                        okvedName = string.Format("{0} {1}", okvedName.Trim(), reportRow[1].Trim());
                        if (reportRow[2].Trim() != string.Empty)
                        {
                            PumpTxtRow(reportRow, okvedName, refDate);
                            okvedName = string.Empty;
                        }
                        continue;
                    }

                    if (reportRow[3].Trim() == "1")
                    {
                        SetNullTotalSum();
                        toPumpRow = true;
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow + 1, ex.Message), ex);
                }
            }
        }

        private void PumpTxtFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            try
            {
                PumpTxtReport(file);
            }
            finally
            {
                GC.GetTotalMemory(true);
            }
        }

        #endregion Работа с Txt

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            CheckDirectories(dir);
            PumpFiles(dir);
            UpdateData();
            SetClsHierarchy();
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        protected override void ProcessDataSource()
        {
            F1NMSumCorrectionConfig f1nomSumCorrectionConfig = new F1NMSumCorrectionConfig();
            f1nomSumCorrectionConfig.EarnedField = "Earned";
            f1nomSumCorrectionConfig.EarnedReportField = "EarnedReport";
            f1nomSumCorrectionConfig.InpaymentsField = "Inpayments";
            f1nomSumCorrectionConfig.InpaymentsReportField = "InpaymentsReport";
            CorrectFactTableSums(fctIncomesTotal, dsOKVED.Tables[0], clsOKVED, "RefOKVED",
                f1nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefD", "RefYearDayUNV" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesTotal, dsIncomes.Tables[0], clsIncomes, "RefD",
                f1nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefOKVED", "RefYearDayUNV" }, string.Empty, string.Empty, false);
            CorrectFactTableSums(fctIncomesRegion, dsOKVED.Tables[0], clsOKVED, "RefOKVED",
                f1nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefD", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsIncomes.Tables[0], clsIncomes, "RefD",
                f1nomSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefOKVED", "RefYearDayUNV" }, "RefRegions", string.Empty, false);
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Коррекции сумм фактов по данным источника");
        }

        #endregion Обработка данных

        #region Сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            IDataSource clsDs = FindDataSource(ParamKindTypes.YearMonth, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
            if (clsDs == null)
                return -1;
            return clsDs.ID;
        }

        #endregion Сопоставление

    }
}
