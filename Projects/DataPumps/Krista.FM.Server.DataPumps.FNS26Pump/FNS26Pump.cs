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

namespace Krista.FM.Server.DataPumps.FNS26Pump
{

    // ФНС - 0026 - Форма 5-П
    public class FNS26PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФНС 5 П (d_Marks_FNS5PO)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
             //для москвы области нужна проверка на существующие коды. когда будут другие закачки.
             //нужно возможно переделать поле cacheMarks, если для других закачек не нужно будет дублирование кодов в базе
            // и поле cacheMarksCodesFromDataSet тогда удалить
        private Dictionary<int, int> cacheMarksCodesFromDataSet = null;
        private Dictionary<int, int> cacheMarksCodes = null;
        private Dictionary<string, int> cacheMarksNames = null;
        private int nullMarks;
        // Налогоплательщики.ФНС 5 П (d_TaxPayer_FNS5PO)
        private IDbDataAdapter daTaxPayer;
        private DataSet dsTaxPayer;
        private IClassifier clsTaxPayer;
        private Dictionary<string, int> cacheTaxPayer = null;
        // Районы.ФНС (d_Regions_FNS)
         private IDbDataAdapter daRegions;
         private DataSet dsRegions;
         private IClassifier clsRegions;
         private Dictionary<string, DataRow> cacheRegions = null;
         private Dictionary<string, string> cacheRegionsNames = null;
         private Dictionary<string, DataRow> cacheRegionsFirstRow = null;
        // ЕдИзмер.ОКЕИ (d_Units_OKEI)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;

        #endregion Классификаторы

        #region Факты

        // Доходы.ФНС_5 П_Сводный (f_D_FNS5POTotal)
        private IDbDataAdapter daIncomesTotal;
        private DataSet dsIncomesTotal;
        private IFactTable fctIncomesTotal;
        // Доходы.ФНС_5 П_Районы (f_D_FNS5PORegions)
         private IDbDataAdapter daIncomesRegion;
         private DataSet dsIncomesRegion;
         private IFactTable fctIncomesRegion;

        #endregion Факты

        private ReportType reportType;
        // номер раздела
        private int sectionIndex = -1;
        // контрольная сумма
        private decimal[] totalSums = new decimal[6];
        private int marksSectionId = -1;
        // IDs налогоплательщиков (для раздела Б)
        private int[] refTaxPayers = new int[2];
        private int refRegion = 0;
        // коэффициент перевода тысяч в рубли
        private decimal sumMultiplier = 1;
        // параметры обработки
        private int year = -1;
        private int month = -1;

        private Dictionary<int, int> marksHierarchy;
        private bool isTyva2008 = false;
        private bool isMoskva2008 = false;

        private bool isPumpSpravSection = false;
        private int refMarksM = -1; //ссылка на показатель 2го уровня в секции М

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

        // наименования разделов
        private string[] sectionNames = new string[] {
            "РАЗДЕЛ А", "РАЗДЕЛ Б", "РАЗДЕЛ Г", "РАЗДЕЛ Д", "РАЗДЕЛ Е", "РАЗДЕЛ Ж", "РАЗДЕЛ К", "РАЗДЕЛ Л", "РАЗДЕЛ М", "ПРИЛОЖЕНИЕ"};

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], new string[] { "CODE", "NAME" }, "|", "ID");
            //для москвы области нужна проверка на существующие коды.
            FillRowsCache(ref cacheMarksCodesFromDataSet, dsMarks.Tables[0], "CODE", "ID" );

            FillRowsCache(ref cacheTaxPayer, dsTaxPayer.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "NAME", "ID");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitDataSet(ref daTaxPayer, ref dsTaxPayer, clsTaxPayer, false, string.Empty, string.Empty);
            InitClsDataSet(ref daMarks, ref dsMarks, clsMarks);
            nullMarks = clsMarks.UpdateFixedRows(this.DB, this.SourceID);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);
            InitFactDataSet(ref daIncomesTotal, ref dsIncomesTotal, fctIncomesTotal);
            InitFactDataSet(ref daIncomesRegion, ref dsIncomesRegion, fctIncomesRegion);
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daTaxPayer, dsTaxPayer, clsTaxPayer);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daIncomesTotal, dsIncomesTotal, fctIncomesTotal);
            UpdateDataSet(daIncomesRegion, dsIncomesRegion, fctIncomesRegion);
        }

        private const string D_MARKS_FNS_5PO_GUID = "8f36f377-4127-4f0f-bfb7-2d2ea2183de8";
        private const string D_TAX_PAYER_GUID = "9a5033f7-3166-48fc-bbcf-5df383d23362";
        private const string D_REGIONS_FNS_GUID = "cf3202f9-e897-43ce-a158-5c617bedff55";
        private const string D_UNITS_OKEI_GUID = "7ef0edfd-9461-4333-8420-ccb102051826";
        private const string F_D_FNS_5PO_TOTAL_GUID = "06f9ac04-5ebd-4193-8cfa-8609ad712878";
        private const string F_D_FNS_5PO_REGIONS_GUID = "59238157-e225-4660-8325-2ee5fbc5da70";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_OKEI_GUID];
            clsTaxPayer = this.Scheme.Classifiers[D_TAX_PAYER_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsMarks = this.Scheme.Classifiers[D_MARKS_FNS_5PO_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FNS_GUID]
            };
            this.UsedFacts = new IFactTable[] { 
                fctIncomesTotal = this.Scheme.FactTables[F_D_FNS_5PO_TOTAL_GUID],
                fctIncomesRegion = this.Scheme.FactTables[F_D_FNS_5PO_REGIONS_GUID]
            };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIncomesTotal);
            ClearDataSet(ref dsIncomesRegion);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsTaxPayer);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsUnits);
        }

        #endregion Работа с базой и кэшами

        #region Общие функции закачка

        #region Иерархия показателей

        private Dictionary<int, int> GetMarksHierarchyDefault()
        {
            Dictionary<int, int> hierarchy = new Dictionary<int, int>();

            hierarchy.Add(1100, 1090);
            hierarchy.Add(1130, 1120);
            hierarchy.Add(1150, 1140);
            hierarchy.Add(1200, 1190);
            hierarchy.Add(1230, 1220);
            hierarchy.Add(1240, 1220);
            hierarchy.Add(1381, 1380);
            hierarchy.Add(1391, 1390);
            hierarchy.Add(1650, 1640);
            hierarchy.Add(1660, 1640);
            hierarchy.Add(1840, 1830);
            hierarchy.Add(1860, 1850);
            hierarchy.Add(1870, 1850);

            return hierarchy;
        }

        private Dictionary<int, int> GetMarksHierarchyTyva2008()
        {
            Dictionary<int, int> hierarchy = new Dictionary<int, int>();

            hierarchy.Add(1100, 1090);
            hierarchy.Add(1130, 1120);
            hierarchy.Add(1150, 1140);
            hierarchy.Add(1200, 1190);
            hierarchy.Add(1230, 1220);
            hierarchy.Add(1240, 1220);
            hierarchy.Add(1320, 1310);
            hierarchy.Add(1390, 1380);
            hierarchy.Add(1650, 1640);
            hierarchy.Add(1660, 1640);
            hierarchy.Add(1840, 1830);
            hierarchy.Add(1860, 1850);
            hierarchy.Add(1870, 1850);

            return hierarchy;
        }

        private Dictionary<int, int> GetMarksHierarchyTyva2009()
        {
            Dictionary<int, int> hierarchy = new Dictionary<int, int>();

            hierarchy.Add(1100, 1090);
            hierarchy.Add(1130, 1120);
            hierarchy.Add(1150, 1140);
            hierarchy.Add(1200, 1190);
            hierarchy.Add(1230, 1220);
            hierarchy.Add(1240, 1220);
            hierarchy.Add(1311, 1310);
            hierarchy.Add(1321, 1320);
            hierarchy.Add(1381, 1380);
            hierarchy.Add(1391, 1390);
            hierarchy.Add(1650, 1640);
            hierarchy.Add(1660, 1640);
            hierarchy.Add(1770, 1765);
            hierarchy.Add(1771, 1765);
            hierarchy.Add(1772, 1765);
            hierarchy.Add(1773, 1765);
            hierarchy.Add(1840, 1830);
            hierarchy.Add(1860, 1850);
            hierarchy.Add(1870, 1850);

            return hierarchy;
        }

        private Dictionary<int, int> GetMarksHierarchy2011()
        {
            Dictionary<int, int> hierarchy = new Dictionary<int, int>();

            hierarchy.Add(1100, 1090);
            hierarchy.Add(1130, 1120);
            hierarchy.Add(1150, 1140);
            hierarchy.Add(1200, 1190);
            hierarchy.Add(1221, 1220);
            hierarchy.Add(1311, 1310);
            hierarchy.Add(1320, 1310);
            hierarchy.Add(1321, 1320);
            hierarchy.Add(1381, 1380);
            hierarchy.Add(1391, 1390);
            hierarchy.Add(1641, 1640);
            hierarchy.Add(1840, 1830);
            hierarchy.Add(1851, 1850);
            hierarchy.Add(3101, 3100);
            hierarchy.Add(3102, 3100);
            hierarchy.Add(3103, 3100);
            hierarchy.Add(3104, 3100);
            hierarchy.Add(3111, 3110);
            hierarchy.Add(3112, 3110);
            hierarchy.Add(3113, 3110);
            hierarchy.Add(3118, 3117);
            hierarchy.Add(3189, 3188);
            hierarchy.Add(3190, 3188);

            return hierarchy;
        }

        private Dictionary<int, int> GetMarksHierarchy()
        {
            if (this.Region == RegionName.Tyva)
            {
                if (this.DataSource.Year >= 2009)
                    return GetMarksHierarchyTyva2009();
                return GetMarksHierarchyTyva2008();
            }
            else if (this.DataSource.Year >= 2011)
                return GetMarksHierarchy2011();
            return GetMarksHierarchyDefault();
        }

        private int GetMarksParentId(int marksCode)
        {
            if (refMarksM != -1)
                return refMarksM;
            if (marksHierarchy.ContainsKey(marksCode))
            {
                int parentMarksCode = marksHierarchy[marksCode];
                if (cacheMarksCodes.ContainsKey(parentMarksCode))
                    return cacheMarksCodes[parentMarksCode];
            }
            return marksSectionId;
        }

        #endregion Иерархия показаталей

        // обнуление итоговой суммы
        private void SetNullTotalSum()
        {
            int sumsCount = totalSums.GetLength(0);
            for (int i = 0; i < sumsCount; i++)
            {
                totalSums[i] = 0;
            }
        }

        // проверка контрольной суммы
        private void CheckTotalSum(decimal totalSum, decimal controlSum, string comment)
        {
            if (totalSum != controlSum)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Контрольная сумма {0:F} не сходится с итоговой {1:F} {2}",
                    controlSum, totalSum, comment));
            }
        }

        private decimal CleanFactValue(string factValue)
        {
            factValue = factValue.Trim().ToUpper().Trim('X').Trim('Х').PadLeft(1, '0');
            return Convert.ToDecimal(factValue);
        }

        // получить дату отчётов по параметрам источника
        private int GetReportDate()
        {
            return (this.DataSource.Year * 10000 + this.DataSource.Month * 100);
        }

        // получить номер раздела по значению ячейки (для всех, кроме тывы)
        private int GetSectionIndex(string cellValue)
        {
            cellValue = cellValue.ToUpper();
            for (int i = 0; i < sectionNames.GetLength(0); i++)
            {
                if (cellValue.Contains(sectionNames[i]))
                    return i;
            }
            return -1;
        }

        // установить номер раздела по наименованию листа книги (только для тывы)
        private int GetSectionIndexFromSheetName(string worksheetName)
        {
            worksheetName = worksheetName.Trim().ToUpper();
            if (worksheetName == "ЛИСТ1")
                return 0;
            else if (worksheetName == "ЛИСТ2")
                return 1;
            else if (worksheetName == "ЛИСТ3")
                return 2;
            else if (worksheetName == "ЛИСТ4")
                return 3;
            else if (worksheetName == "ЛИСТ5")
                return 4;
            else if (worksheetName == "ЛИСТ6")
                return 5;
            else if (worksheetName == "ЛИСТ7")
                return 6;
            else if (worksheetName == "ЛИСТ8")
                return 7;
            else if (worksheetName == "ЛИСТ9")
                return 8;
            else if (worksheetName == "ЛИСТ10")
                return 9;
            return -1;
        }

        // установить коэффициент sumMultiplier
        private void SetSumMultiplier(int marksCode)
        {
            if ((marksCode >= 1220 && marksCode <= 1260) ||
                (marksCode >= 1400 && marksCode <= 1410) ||
                (marksCode >= 1640 && marksCode <= 1660) ||
                (marksCode >= 1850 && marksCode <= 1880) ||
                (marksCode == 1328))
                sumMultiplier = 1;
            else
                sumMultiplier = 1000;
        }

        // проверить наличие записей в классификаторе 'Показатели.ФНС 5 П'
        private void CheckMarks()
        {
            if ((reportType != ReportType.Svod) && (cacheMarks.Count == 0) && (!((reportType == ReportType.Region) && (this.Region == RegionName.MoskvaObl))))
                throw new Exception("Не заполнен классификатор 'Показатели.ФНС 5 П' - закачайте сводные отчеты");
        }

        private void SetFlags()
        {
            isTyva2008 = (this.Region == RegionName.Tyva) && (this.DataSource.Year <= 2008);
            isMoskva2008 = (this.Region == RegionName.MoskvaObl) && (this.DataSource.Year <= 2008);
        }

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            SetFlags();
            CheckMarks();
            marksHierarchy = GetMarksHierarchy();
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false);
            ProcessFilesTemplate(dir, "*.rar", new ProcessFileDelegate(PumpRarFile), false);
        }

        private void PumpFiles(DirectoryInfo dir)
        {
            reportType = ReportType.Svod;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных сводных отчетов.");
            ProcessAllFiles(dir.GetDirectories(constSvodDirName)[0]);
            reportType = ReportType.Region;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе районов.");
            ProcessAllFiles(dir.GetDirectories(constRegDirName)[0]);
            // reportType = ReportType.Str;
            // WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping, "Старт закачки данных отчетов в разрезе строк.");
            // ProcessAllFiles(dir.GetDirectories(constStrDirName)[0]);
        }

        // наименования служебных каталогов
        private const string constSvodDirName = "Сводный";
        private const string constStrDirName = "Строки";
        private const string constRegDirName = "Районы";
        // проверить корректное заполнение служебных каталогов
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

        #endregion Общие фукции закачки

        #region Работа с Excel

        // проверка контрольной суммы
        private void CheckXlsTotalSum(ExcelHelper excelDoc, int curRow)
        {
            int columnCount = 1;
            if (this.DataSource.Year >= 2011)
            {
                if (sectionIndex == 0)
                    columnCount = 6;
                else if ((sectionIndex == 1) || (sectionIndex >= 6))
                    columnCount = 2;
                if (isPumpSpravSection)
                    columnCount = 1;
            }
            else
            {
                if (sectionIndex <= 1)
                    columnCount = 2;
            }
            for (int i = 0; i < columnCount; i++)
            {
                string comment = string.Format("по столбцу '{0}' раздела '{1}'", i + 3, sectionNames[sectionIndex]);
                decimal controlSum = CleanFactValue(excelDoc.GetValue(curRow, i + 3));
                CheckTotalSum(totalSums[i], controlSum, comment);
            }
        }

        // закачка записи в таблицу фактов из Xls-отчета
        private void PumpFactRow(decimal factValue, string factField, int refDate, int refMarks, int refTaxPayers, int refOrganization, int sumIndex)
        {
            if (factValue == 0)
                return;

            totalSums[sumIndex] += factValue;
            factValue *= sumMultiplier;

            object[] mapping = new object[] { factField, factValue, "RefYearDayUNV", refDate, "RefFNS5PO", refMarks, "RefOrg", refOrganization };
            if (refTaxPayers != -1)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefTaxPayer", refTaxPayers });
            if (this.reportType == ReportType.Region && (refRegion != -1))
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "RefRegions", refRegion });
            if (this.reportType == ReportType.Region)
            {
                PumpRow(dsIncomesRegion.Tables[0], mapping);
                if (dsIncomesRegion.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesRegion);
                }
            }
            else
            {
                PumpRow(dsIncomesTotal.Tables[0], mapping);
                if (dsIncomesTotal.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateData();
                    ClearDataSet(daIncomesRegion, ref dsIncomesTotal);
                }
            }
        }

        private int PumpParentMarks()
        {
            string marksName = sectionNames[sectionIndex];
            object[] mapping = new object[] { "Name", marksName, "Code", 0, "RefUnits", nullUnits };
            string marksKey = string.Format("0|{0}", marksName);

            if ((reportType == ReportType.Svod) || reportType == ReportType.Region)
            {
                int refM = PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, marksKey, "ID");
                if (!cacheMarksCodesFromDataSet.ContainsKey(0))
                    cacheMarksCodesFromDataSet.Add(0, refM);
                return refM;
            }
            return FindCachedRow(cacheMarks, marksKey, nullMarks);
        }


        // закачка записи в классификатор "Показатели.ФНС 5 П" из Xls-отчета
        private int PumpXlsMarks(ExcelHelper excelDoc, int curRow, string marksName)
        {
            string marksCodeStr = excelDoc.GetValue(curRow, 2).Trim();
            if ((marksCodeStr == string.Empty) && (sectionIndex != 8))
                return -1;
            int marksCode = 0;
            if (marksCodeStr != string.Empty)
            {
                marksCode = Convert.ToInt32(marksCodeStr);
            }

            if (marksName.Length > 255)
                marksName = marksName.Substring(0, 255);

            SetSumMultiplier(marksCode);

            if (cacheMarksCodesFromDataSet.ContainsKey(marksCode) && (marksCode !=0))
            {
                return cacheMarksCodesFromDataSet[marksCode];
            }
            if ((marksCode == 0) && (cacheMarks.ContainsKey(string.Format("0|{0}", marksName))))
            {
                return cacheMarks[string.Format("0|{0}", marksName)];
            }

            if (marksCode == 0)
                refMarksM = -1;

            object[] mapping = new object[] {
                "Name", marksName, "Code", marksCode, "RefUnits", nullUnits, "ParentId", GetMarksParentId(marksCode) };
            string marksKey = string.Format("{0}|{1}", marksCode, marksName);

            int refMarks = nullMarks;
            if ((reportType == ReportType.Svod) || (reportType == ReportType.Region))
            {
                refMarks = PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, mapping, marksKey, "ID");
                if (marksCode != 0)
                    cacheMarksCodesFromDataSet.Add(marksCode, refMarks);
            }
            else
                refMarks = FindCachedRow(cacheMarks, marksKey, nullMarks);

            if ((marksCode != 0) && (!cacheMarksCodes.ContainsKey(marksCode)))
                cacheMarksCodes.Add(marksCode, refMarks);

            return refMarks;
        }

        // закачка записи в классификатор "Налогоплательщики.ФНС 5 П" из Xls-отчета
        private int PumpXlsTaxPayer(ExcelHelper excelDoc, int curRow, int curColumn)
        {
            string name = excelDoc.GetValue(curRow - 1, curColumn).Trim();
            int code = Convert.ToInt32(excelDoc.GetValue(curRow, curColumn).Trim());
            object[] mapping = new object[] { "NAME", name, "CODE", code };
            return PumpCachedRow(cacheTaxPayer, dsTaxPayer.Tables[0], clsTaxPayer, mapping, code.ToString(), "ID");
        }

        // в разделе Б нужно ещё закачать данные в классификатор "налогоплательщики"
        private void PumpXlsTaxPayers(ExcelHelper excelDoc, int curRow)
        {
            if ((sectionIndex != 1) || isTyva2008 || isMoskva2008)
                return;
            refTaxPayers[0] = PumpXlsTaxPayer(excelDoc, curRow, 3);
            refTaxPayers[1] = PumpXlsTaxPayer(excelDoc, curRow, 4);
        }

        //для разделов А, К, Л, М нужна ссылка на на классификатор Организации
        int GetRefOrganozation(int column)
        {
            int refOrganization = 0;
            if (sectionIndex == 0)
                refOrganization = column - 2;
            else if (sectionIndex >= 6)
            {
                if (column == 3)
                    refOrganization = 1;
                else if (column == 4)
                    refOrganization = 4;
            }
            return refOrganization;
        }

        // закачка строки из Xls-отчета
        private void PumpXlsRow(ExcelHelper excelDoc, int curRow, int refDate, int refRegion, string marksName)
        {
            int refMarks = PumpXlsMarks(excelDoc, curRow, marksName);
            if (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
            {
                refMarksM = refMarks;
                return;
            }
            if (refMarks == -1)
                return;

            if (this.DataSource.Year >= 2011)
            {
                decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                PumpFactRow(factValue, "ValueReport", refDate, refMarks, (sectionIndex == 1)? refTaxPayers[0] : -1, GetRefOrganozation(3), 0);

                if (!isPumpSpravSection)
                {
                    if (sectionIndex == 0)//По столбцам C, D, E, F, G, H Excel для Раздела А
                    {
                        for (int i = 4; i <= 8; i++)
                        {
                            factValue = CleanFactValue(excelDoc.GetValue(curRow, i));
                            PumpFactRow(factValue, "ValueReport", refDate, refMarks, -1, GetRefOrganozation(i), i - 3);
                        }
                    }
                    else if ((sectionIndex == 1) || (sectionIndex >= 6))
                    {
                        factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                        PumpFactRow(factValue, "ValueReport", refDate, refMarks, (sectionIndex == 1) ? refTaxPayers[1] : -1, GetRefOrganozation(4), 1);
                    }
                }
            }
            else
            {
                if ((sectionIndex == 1) && !isTyva2008 && !isMoskva2008)
                {
                    decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                    PumpFactRow(factValue, "ValueReport", refDate, refMarks, refTaxPayers[0], 0, 0);
                    factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                    PumpFactRow(factValue, "ValueReport", refDate, refMarks, refTaxPayers[1], 0, 1);
                }
                else
                {
                    decimal factValue = CleanFactValue(excelDoc.GetValue(curRow, 3));
                    PumpFactRow(factValue, "ValueReport", refDate, refMarks, -1, 0, 0);
                    if (sectionIndex == 0)
                    {
                        factValue = CleanFactValue(excelDoc.GetValue(curRow, 4));
                        PumpFactRow(factValue, "TaxUnderfundsSumReport", refDate, refMarks, -1, 0, 1);
                    }
                }
            }
        }
        
        // является ли текущая ячейка завершением раздела
        private bool IsSectionEnd(string cellValue)
        {
            return (cellValue.ToUpper() == "КОНТРОЛЬНАЯ СУММА");
        }

        // является ли текущая ячейка началом раздела
        private bool IsSectionStart(string cellValue)
        {
            return (cellValue.ToUpper() == "А");
        }

        private const string AUX_TABLE_MARK_REGION = "КОД ОКАТО";
        private bool IsAuxTable(string cellValue)
        {
            return (cellValue.Trim().ToUpper() == AUX_TABLE_MARK_REGION);
        }

        private int PumpRegion(string regionCode, string regionName)
        {
            if (regionName == string.Empty)
                regionName = constDefaultClsName;
            if (regionCode == string.Empty)
                regionCode = "0";
            // если у регионов наименования одинаковые, а коды разные,
            // то к наименованию необходимо приписывать код в скобках
            if (!cacheRegionsNames.ContainsKey(regionCode))
            {
                // проверка: встречалось ли такое наименование, но с другим кодом
                if (cacheRegionsNames.ContainsValue(regionName))
                {
                    // если да, то необходимо изменить наименование у первой попавшейся записи с таким же наименованием
                    if (cacheRegionsFirstRow.ContainsKey(regionName))
                    {
                        DataRow firstRow = cacheRegionsFirstRow[regionName];
                        firstRow["Name"] = string.Format("{0} ({1})", firstRow["Name"], firstRow["Code"]);
                        cacheRegionsFirstRow.Remove(regionName);
                    }
                    regionName = string.Format("{0} ({1})", regionName, regionCode);
                }
                cacheRegionsNames.Add(regionCode, regionName);
            }
            object[] mapping = new object[] { "NAME", regionName, "CODE", regionCode };
            string regionKey = string.Format("{0}|{1}", regionCode, regionName);
            DataRow regionRow = PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regionKey, mapping, false);
            // запоминаем регионы с уникальными наименованиями
            if (!cacheRegionsFirstRow.ContainsKey(regionName))
                cacheRegionsFirstRow.Add(regionName, regionRow);
            return Convert.ToInt32(regionRow["ID"]);
        }

        private int getRegionRef(ExcelHelper excelDoc, int curRow)
        {
            string strOKATO = string.Empty;
            string strRegionName = string.Empty;
            
            string cellValue = excelDoc.GetValue(curRow, 1).Trim();

            if (excelDoc.GetWorksheetsCount() == 1)
            {
                if (cellValue.ToUpper().Contains("РАЗДЕЛ А") && cellValue.Contains("ОКАТО"))
                {
                    //выделим код и наименование из строки "Раздел А ОКАТО 46434000000 г. Королев"
                    strOKATO = CommonRoutines.TrimLetters(cellValue).Trim();
                    if (strOKATO.Length > 11)
                        strOKATO = strOKATO.Substring(0, 11).Trim();
                    strRegionName = cellValue.Remove(0, cellValue.ToUpper().IndexOf("ОКАТО") + 5).Trim();
                    strRegionName = CommonRoutines.TrimNumbers(strRegionName).Trim();
                }
                else if (cellValue.ToUpper().Contains("НАИМЕНОВАНИЕ") && cellValue.ToUpper().Contains("ОБРАЗОВАНИЯ"))
                {
                    int i = 1;
                    strRegionName = excelDoc.GetValue(curRow, 3).Trim();
                    strOKATO = excelDoc.GetValue(curRow - 2, 3).Trim();
                    if (strOKATO.Length > 11)
                        strOKATO = strOKATO.Substring(0, 11).Trim();
                }
                else if (cellValue.ToUpper().StartsWith("МУНИЦИПАЛЬНОЕ") && (cellValue.ToUpper().Contains("ОБРАЗОВАНИЕ")))
                {
                    strRegionName = excelDoc.GetValue(curRow + 1, 1).Trim();
                    strOKATO = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 2, 1)).Trim();
                    if (strOKATO.Length > 11)
                        strOKATO = strOKATO.Substring(0, 11).Trim();
                }
                else return -1;
            }
            else
            {
                if (cellValue.ToUpper().StartsWith("НАЛОГОВЫЙ") && cellValue.ToUpper().Contains("ОРГАН"))
                {
                    if (excelDoc.GetValue(curRow + 1, 1).ToUpper().Contains("РАЗДЕЛ А"))
                    {
                        if (curRow > 2)
                            strRegionName = excelDoc.GetValue(curRow - 2, 2).Trim();
                    }
                    else
                    {
                        strRegionName = CommonRoutines.TrimNumbers(excelDoc.GetValue(curRow + 1, 1)).Trim();
                        strOKATO = CommonRoutines.TrimLetters(excelDoc.GetValue(curRow + 1, 1)).Trim();
                        if (strOKATO.Length > 11)
                            strOKATO = strOKATO.Substring(0, 11).Trim();
                    }
                }
                else return -1;
            }

            //сюда дойдем если считали наименование/код
            if ((strRegionName == string.Empty) && (strOKATO == string.Empty))
                return -1;
            
            return PumpRegion(strOKATO, strRegionName);            
        }

        // получаем полное наименование показателя,
        // т.к. оно может находиться на нескольких строках
        private string GetXlsMarksName(ExcelHelper excelDoc, ref int curRow)
        {
            List<string> marksName = new List<string>();
            if (!((excelDoc.GetValue(curRow, 2).Trim() == string.Empty) && (sectionIndex == 8))) //для раздела М нужно сформировать иерархию 2го уровня
            {
                while (excelDoc.GetValue(curRow, 2).Trim() == string.Empty)
                {
                    marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
                    curRow++;
                }
            }
            marksName.Add(excelDoc.GetValue(curRow, 1).Trim());
            return string.Join(" ", marksName.ToArray());
        }

        private void PumpXlsSheetData(FileInfo file, ExcelHelper excelDoc, int refDate)
        {
            sectionIndex = -1;
            bool toPumpRow = false;
            int rowsCount = excelDoc.GetRowsCount();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            //для 2011 года справочный раздел может идти сазу после контрольной суммы и без какой-либо идентификации заголовка для этого раздела
            isPumpSpravSection = false;

            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if (cellValue == string.Empty)
                        continue;

                    if (IsSectionEnd(cellValue))
                    {
                        CheckXlsTotalSum(excelDoc, curRow);
                        toPumpRow = false;
                        isPumpSpravSection = false;

                        continue;
                    }

                    if (toPumpRow)
                    {
                        string marksName = GetXlsMarksName(excelDoc, ref curRow);
                        PumpXlsRow(excelDoc, curRow, refDate, refRegion, marksName);
                        continue;
                    }

                    if (cellValue.ToUpper().Contains("СПРАВОЧ"))
                    {
                        if (IsSectionEnd(excelDoc.GetValue(curRow - 1, 1).Trim()))
                            isPumpSpravSection = true;
                    }

                    if ((cellValue.ToUpper().Contains("РАЗДЕЛ") || cellValue.ToUpper().Contains("ПРИЛОЖ")) && (!isPumpSpravSection))
                    {
                        sectionIndex = GetSectionIndex(cellValue);
                    }

                    //закачаем районы, если найдем
                    int rRegion = getRegionRef(excelDoc, curRow);
                    if (rRegion != -1)
                    {
                        refRegion = rRegion;
                        continue;
                    }

                    if (IsSectionStart(cellValue))
                    {
                        //в некоторых отчетах бывает таблица ОКАТО. пропускаем
                        if (IsAuxTable(excelDoc.GetValue(curRow + 1, 1)))
                            continue;
                        if ((this.Region == RegionName.Tyva) || ((this.Region == RegionName.MoskvaObl) && (sectionIndex == -1)))
                            sectionIndex = GetSectionIndexFromSheetName(excelDoc.GetWorksheetName());
                        marksSectionId = PumpParentMarks();
                        PumpXlsTaxPayers(excelDoc, curRow);
                        SetNullTotalSum();

                        toPumpRow = true;
                        refMarksM = -1;
                        
                        continue;
                    }
                    if (isPumpSpravSection)
                    {
                        curRow = curRow - 1;
                        SetNullTotalSum();
                        toPumpRow = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
            }
        }

        private bool IsSkipWorksheet(string worksheetName)
        {
            if ((this.Region != RegionName.Tyva) && (this.Region != RegionName.MoskvaObl))
                return false;
            worksheetName = worksheetName.ToUpper();
            return (worksheetName.StartsWith("ТИТ") && worksheetName.EndsWith("ЛИСТ"));
        }

        private int PumpRegionFromTitleWorkSheet(ExcelHelper excelDoc)
        {
            int rowsCount = excelDoc.GetRowsCount();
            string cellValue;
            for (int curRow = 1; curRow <= rowsCount; curRow++)
            {
                try
                {
                    cellValue = excelDoc.GetValue(curRow, 2).Trim();
                    if (cellValue.ToUpper().StartsWith("МУНИЦИПАЛЬНОЕ") && cellValue.ToUpper().Contains("ОБРАЗОВАНИЕ"))
                    { 
                        return PumpRegion(excelDoc.GetValue(curRow, 4), excelDoc.GetValue(curRow, 5));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(
                        "При обработке строки {0} листа '{1}' возникла ошибка ({2})",
                        curRow, excelDoc.GetWorksheetName(), ex.Message), ex);
                }
            }
            return -1;
        }

        private void PumpXlsFile(FileInfo file)
        {
            cacheMarksCodes = new Dictionary<int, int>();
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                int refDate = GetReportDate();
                int wsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= wsCount; index++)
                {
                    excelDoc.SetWorksheet(index);
                    if (!IsSkipWorksheet(excelDoc.GetWorksheetName()))
                        PumpXlsSheetData(file, excelDoc, refDate);
                    else
                    {
                        if (this.Region == RegionName.MoskvaObl)
                        {
                            refRegion = PumpRegionFromTitleWorkSheet(excelDoc);
                        }
                    }
                }
            }
            finally
            {
                cacheMarksCodes.Clear();
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        #endregion Работа с Excel

        #region Работа с Rar

        private void PumpRarFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Rar);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        #endregion Работа с Rar

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Дата будет определена параметрами источника");
            cacheRegionsNames = new Dictionary<string, string>();
            cacheRegionsFirstRow = new Dictionary<string, DataRow>();
            try
            {
                CheckDirectories(dir);
                PumpFiles(dir);
                UpdateData();
            }
            finally
            {
                cacheRegionsFirstRow.Clear();
                cacheRegionsNames.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private const string ROUBLE_UNIT_NAME = "Рубль";
        private const string UNIT_UNIT_NAME = "Единица";
        private string GetUnitName(int marksCode)
        {
            if ((marksCode >= 1220 && marksCode <= 1260) ||
                (marksCode >= 1640 && marksCode <= 1660) ||
                (marksCode >= 1400 && marksCode <= 1410) ||
                (marksCode >= 1850 && marksCode <= 1880) ||
                (marksCode == 1328))
                return UNIT_UNIT_NAME;
            return ROUBLE_UNIT_NAME;
        }

        private int GetRefUnits(int marksCode)
        {
            string unitName = GetUnitName(marksCode);
            return FindCachedRow(cacheUnits, unitName, nullUnits);
        }

        private void SetRefUnits()
        {
            if (cacheUnits.Count <= 1)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, "Классификатор «ЕдИзмер.ОКЕИ» не заполнен.");
                return;
            }
            foreach (DataRow row in dsMarks.Tables[0].Rows)
            {
                if (row["Code"].ToString() != "0")
                {
                    int refUnits = GetRefUnits(Convert.ToInt32(row["Code"]));
                    row["RefUnits"] = refUnits;
                }
            }
        }

        private void CorrectSumByHierarchy()
        {
            F1NMSumCorrectionConfig f1nmSumCorrectionConfig = new F1NMSumCorrectionConfig();
            f1nmSumCorrectionConfig.EarnedField = "Value";
            f1nmSumCorrectionConfig.EarnedReportField = "ValueReport";
            f1nmSumCorrectionConfig.InpaymentsField = "TaxUnderfundsSum";
            f1nmSumCorrectionConfig.InpaymentsReportField = "TaxUnderfundsSumReport";
            CorrectFactTableSums(fctIncomesTotal, dsMarks.Tables[0], clsMarks, "RefFNS5PO",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefOrg", "RefTaxPayer" }, string.Empty, string.Empty, true);
            CorrectFactTableSums(fctIncomesRegion, dsMarks.Tables[0], clsMarks, "RefFNS5PO",
                f1nmSumCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefYearDayUNV", "RefOrg", "RefTaxPayer" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            CorrectSumByHierarchy();
            SetRefUnits();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            year = -1;
            month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month,
                "Выполняется корректировка сумм фактов по данным источника и установка ссылок на классификатор 'ЕдИзмер.ОКЕИ'");
        }

        #endregion Обработка данных

    }

}
