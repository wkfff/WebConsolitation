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

namespace Krista.FM.Server.DataPumps.RNDV1Pump
{
    // РОСНЕДВИЖИМ_0001 - земельный кадастр
    public class RNDV1PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Районы.Роснедвижимость (d_Regions_Posnedvigimost)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        // Кадастровый квартал.Роснедвижимость (d_CadastQuarter_Posnedvigimost)
        private IDbDataAdapter daCadastQuarter;
        private DataSet dsCadastQuarter;
        private IClassifier clsCadastQuarter;
        private Dictionary<string, int> cacheCadastQuarter = null;
        // Налоги.Роснедвижимость_Земля (d_Tax_Land)
        private IDbDataAdapter daTax;
        private DataSet dsTax;
        private IClassifier clsTax;
        private Dictionary<string, int> cacheTax = null;
        private Dictionary<string, int> cacheTaxNumber = null;

        #endregion Классификаторы

        #region Факты

        // Доходы.Роснедвижимость_Земельный кадастр (f.D.LandRegister)
        private IDbDataAdapter daRNDV1;
        private DataSet dsRNDV1;
        private IFactTable fctRNDV1;

        #endregion Факты

        private int regionID = 0;
        private int taxParentId = -1;
        private int lastCadastrQuarterId = -1;
        private int clsDataSourceId = -1;

        private int columnsCount = 0;
        // YYYY|refRegions
        private List<string> deletedList = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void GetClsDataSourceId()
        {
            clsDataSourceId = AddDataSource("РОСНЕДВИЖИМ", "0001", ParamKindTypes.Year, string.Empty, 
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        protected override void QueryData()
        {
            // не делится по источникам
            InitDataSet(ref daCadastQuarter, ref dsCadastQuarter, clsCadastQuarter, false, string.Empty, string.Empty);
            // классификаторы формируются на год
            GetClsDataSourceId();
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty, clsDataSourceId);
            InitClsDataSet(ref daTax, ref dsTax, clsTax, false, string.Empty, clsDataSourceId);
            // факт формируется на год - вариант
            InitFactDataSet(ref daRNDV1, ref dsRNDV1, fctRNDV1);
            FillCaches();
        }

        private void FillTaxCache()
        {
            DataTable dt = dsTax.Tables[0];
            if (dt == null)
                return;

            if (cacheTax != null)
                cacheTax.Clear();
            cacheTax = new Dictionary<string, int>(dt.Rows.Count);

            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
                if (row.RowState != DataRowState.Deleted)
                {
                    string key = row["Name"].ToString().ToUpper();
                    if (!cacheTax.ContainsKey(key))
                        cacheTax.Add(key, Convert.ToInt32(row["Id"]));
                }
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "Name", "ParentId" }, "|", "Id");
            FillRowsCache(ref cacheCadastQuarter, dsCadastQuarter.Tables[0], "QuarterNumber");
            FillRowsCache(ref cacheTaxNumber, dsTax.Tables[0], "KindNumber");
            if ((this.Region == RegionName.Omsk) || (this.DataSource.Year >= 2011))
                FillTaxCache();
            else
                FillRowsCache(ref cacheTax, dsTax.Tables[0], "Name");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daCadastQuarter, dsCadastQuarter, clsCadastQuarter);
            UpdateDataSet(daTax, dsTax, clsTax);
            UpdateDataSet(daRNDV1, dsRNDV1, fctRNDV1);
        }

        private const string D_CADAST_QUARTER_POSNEDVIGIMOST_GUID = "074c0822-bf2b-4746-a03e-ad5369937ee6";
        private const string D_REGIONS_POSNEDVIGIMOST_GUID = "6fcd3c7d-b87b-4480-88d8-e983f17249c4";
        private const string D_TAX_LAND_GUID = "81b4e2a6-ada2-476b-bacb-8096d702f2a4";
        private const string F_D_LAND_REGISTER_GUID = "f7f5851a-6751-4894-9c5e-4fd57d183728";
        protected override void InitDBObjects()
        {
            clsCadastQuarter = this.Scheme.Classifiers[D_CADAST_QUARTER_POSNEDVIGIMOST_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_POSNEDVIGIMOST_GUID],
                clsTax = this.Scheme.Classifiers[D_TAX_LAND_GUID] };
            this.AssociateClassifiersEx = this.UsedClassifiers;
            this.UsedFacts = new IFactTable[] { fctRNDV1 = this.Scheme.FactTables[F_D_LAND_REGISTER_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRNDV1);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsCadastQuarter);
            ClearDataSet(ref dsTax);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        #region Общие методы

        private bool CheckFileName(FileInfo file)
        {
            if ((this.Region == RegionName.Omsk) && file.Name.ToUpper().Contains("МО.XLS"))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Данные файла {0} закачаны не будут.", file.FullName));
                return false;
            }
            return true;
        }

        private bool IsEmptySheet(ExcelHelper excelDoc)
        {
            return (excelDoc.GetRowsCount() <= 10);
        }

        private bool IsEmptyRow(ExcelHelper excelDoc, int curRow)
        {
            for (int curCol = 1; curCol < 5; curCol++)
                if (excelDoc.GetValue(curRow, curCol).Trim() != string.Empty)
                    return false;
            return true;
        }

        private const string TOTAL_ROW = "ИТОГО ПО";
        private bool IsTotalRow(string value)
        {
            return value.Trim().ToUpper().Contains(TOTAL_ROW);
        }

        private decimal ConvertFactValue(string cellValue)
        {
            if ((this.DataSource.Year == 2011) && (this.Region == RegionName.Omsk))
                cellValue = cellValue.Trim().Split(new char[] { ' ' })[0].Replace('.', ',');
            return Convert.ToDecimal(cellValue.Trim().PadLeft(1, '0'));
        }

        private void DeleteRegionData(int refRegions, string regionName)
        {
            int refDate = (this.DataSource.Year * 10000);
            string key = string.Concat(refDate.ToString(), refRegions.ToString());
            if (!deletedList.Contains(key))
            {
                DeleteData(
                    string.Format("RefYearDayUNV = {0} and RefRegions = {1}", refDate, refRegions),
                    string.Format("Год: {0}, Район: {1}.", this.DataSource.Year, regionName));
                deletedList.Add(key);
            }
        }

        private int PumpRegion(string name, string code, string parentId)
        {
            string regKey = string.Format("{0}|{1}", name, parentId);
            object[] mapping = new object[] { "Name", name, "Code", code, "SourceID", clsDataSourceId };
            if (parentId != string.Empty)
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", parentId });
            return PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, regKey, mapping);
        }

        private int PumpParentRegion(string name, string code)
        {
            return PumpRegion(name, code, string.Empty);
        }

        private void PumpFactRow(object[] mapping)
        {
            PumpRow(dsRNDV1.Tables[0], mapping);
            if (dsRNDV1.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daRNDV1, ref dsRNDV1);
            }
        }

        #endregion Общие методы

        #region Омск

        #region Классификаторы

        private const int RNDV_SIZE_CODE = 8;
        private void FormatRegionCodeOmsk(ref string regionCode)
        {
            if (this.DataSource.Year >= 2008)
            {
                regionCode = regionCode.Replace(":", string.Empty);
                // устанавливаем код родительскому элементу
                DataRow[] parentRow = dsRegions.Tables[0].Select(string.Format("Id = {0}", regionID));
                if (parentRow.GetLength(0) != 0)
                    parentRow[0]["Code"] = regionCode;
            }
        }

        private int PumpChildRegionOmsk(ExcelHelper excelDoc, int curRow, string filename)
        {
            string regionCode = "0";
            if (columnsCount == 5)
            {
                regionCode = excelDoc.GetValue(curRow, 1).Trim();
                if (regionCode == string.Empty)
                    return -1;
                FormatRegionCodeOmsk(ref regionCode);
            }
            string regionName = filename.Substring(0, filename.Length - 4);
            int refRegion = PumpRegion(regionName, regionCode, regionID.ToString());
            DeleteRegionData(refRegion, regionName);
            return refRegion;
        }

        private string CleanTaxName(string taxName)
        {
            taxName = taxName.Trim().Trim(new char[] { '-' });
            if (taxName.Length > 250)
                taxName = taxName.Substring(0, 250);
            return taxName;
        }

        private int PumpTaxOmsk(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, GetColumnOffset()).Trim();
            if (name != string.Empty)
            {
                if (name.Length > 250)
                    name = name.Substring(0, 250);
                taxParentId = PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name,
                    new object[] { "Name", name, "SourceID", clsDataSourceId });
                return taxParentId;
            }
            name = excelDoc.GetValue(curRow, GetColumnOffset() + 1).Trim();
            if (name.Length > 250)
                name = name.Substring(0, 250);
            return PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name,
                new object[] { "Name", name, "ParentId", taxParentId, "SourceID", clsDataSourceId });
        }

        private int PumpTaxOmsk2011(ExcelHelper excelDoc, int curRow)
        {
            string name = CleanTaxName(excelDoc.GetValue(curRow, GetColumnOffset()));
            if (name == string.Empty)
            {
                name = CleanTaxName(excelDoc.GetValue(curRow, GetColumnOffset() + 1));
                if (name == string.Empty)
                    return taxParentId;
                taxParentId = PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name.ToUpper(),
                    new object[] { "Name", name, "SourceID", clsDataSourceId });
            }
            taxParentId = PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name.ToUpper(),
                new object[] { "Name", name, "SourceID", clsDataSourceId });
            name = CleanTaxName(excelDoc.GetValue(curRow, GetColumnOffset() + 1));
            if (name == string.Empty)
                return taxParentId;
            return PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name.ToUpper(),
                new object[] { "Name", name, "ParentId", taxParentId, "SourceID", clsDataSourceId });
        }

        private int PumpTaxOmsk2012(ExcelHelper excelDoc, int curRow)
        {
            string name = CleanTaxName(excelDoc.GetValue(curRow, GetColumnOffset()));
            if (name != string.Empty)
            {
                taxParentId = PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name.ToUpper(),
                new object[] { "Name", name, "SourceID", clsDataSourceId });
            }
            name = CleanTaxName(excelDoc.GetValue(curRow, GetColumnOffset() + 1));
            if (name == string.Empty)
                return taxParentId;
            return PumpCachedRow(cacheTax, dsTax.Tables[0], clsTax, name.ToUpper(),
                new object[] { "Name", name, "ParentId", taxParentId, "SourceID", clsDataSourceId });
        }

        // форматируем номер кадастра, маска ХХ:ХХ:ХХ ХХ ХХ
        private void FormatCadastrQaurter(ref string cadastrQuarter)
        {
            cadastrQuarter = cadastrQuarter.Replace(":", string.Empty).Replace(" ", string.Empty).PadRight(10, '0');
            cadastrQuarter = cadastrQuarter.Insert(2, ":").Insert(5, ":").Insert(8, " ").Insert(11, " ");
        }

        private int PumpCadastQuarterOmsk(string quarterNumber)
        {
            if (this.DataSource.Year >= 2008)
                FormatCadastrQaurter(ref quarterNumber);
            return PumpCachedRow(cacheCadastQuarter, dsCadastQuarter.Tables[0], clsCadastQuarter,
                quarterNumber, new object[] { "QuarterNumber", quarterNumber });
        }

        #endregion Классификаторы

        // возвращает смещение столбцов в зависимости от типа отчета
        private int GetColumnOffset()
        {
            if (columnsCount >= 7)
                return 4;
            if (columnsCount == 4)
                return 1;
            return 2;
        }

        // если строка имеет дочерние записи и хотя бы одна дочерняя запись имеет ненулевую сумму,
        // то суммы родительской записи качать не надо 
        private bool CheckChildRows(ExcelHelper excelDoc, int curRow)
        {
            if (excelDoc.GetValue(curRow, GetColumnOffset()).Trim() != string.Empty)
            {
                curRow++;
                while (excelDoc.GetValue(curRow, GetColumnOffset()).Trim() == string.Empty)
                {
                    if (excelDoc.GetValue(curRow, GetColumnOffset() + 1).Trim() == string.Empty)
                        return true;
                    decimal valueTotal = ConvertFactValue(excelDoc.GetValue(curRow, GetColumnOffset() + 2));
                    decimal valueCadastr = ConvertFactValue(excelDoc.GetValue(curRow, GetColumnOffset() + 3));
                    if (valueTotal != 0 || valueCadastr != 0)
                        return false;
                    curRow++;
                }
            }
            return true;
        }

        private void PumpXlsRowOmsk(ExcelHelper excelDoc, int curRow, int refRegion, int refCadastQuarter)
        {
            int refTax = -1;
            if (this.DataSource.Year < 2010)
            {
                refTax = PumpTaxOmsk(excelDoc, curRow);
                if (!CheckChildRows(excelDoc, curRow))
                    return;
            }
            else if (this.DataSource.Year == 2011)
                refTax = PumpTaxOmsk2011(excelDoc, curRow);
            else refTax = PumpTaxOmsk2012(excelDoc, curRow);
            
            decimal totalArea = 0;
            if (this.DataSource.Year != 2012)
                totalArea = ConvertFactValue(excelDoc.GetValue(curRow, GetColumnOffset() + 2));
            decimal cadastralCost = ConvertFactValue(excelDoc.GetValue(curRow, GetColumnOffset() + 3));
            if (totalArea == 0 && cadastralCost == 0)
                return;
            if (this.DataSource.Year == 2011)
                cadastralCost *= 1000;

            object[] mapping = new object[] {
                "TotalArea", totalArea,
                "CadastralCost", cadastralCost,
                "RefCadastQuarter", refCadastQuarter,
                "RefYearDayUNV", this.DataSource.Year * 10000,
                "RefRegions", refRegion,
                "RefTax", refTax };
            PumpFactRow(mapping);
        }

        // возвращает первую строку в отчёте, с которой можно начинать качать данные
        private int GetFirstRow(ExcelHelper excelDoc)
        {
            if (this.DataSource.Year >= 2012)
                return 5;
            if (this.DataSource.Year >= 2011)
                return 7;

            for (int curRow = 1; ; curRow++)
            {
                if (this.DataSource.Year == 2012)
                {
                    if (excelDoc.GetValue(curRow, 2).Trim().ToUpper().Contains("КАТЕГОРИЯ") && excelDoc.GetValue(curRow, 2).Trim().ToUpper().Contains("ЗЕМЕЛЬ"))
                        return (curRow + 1);
                }
                else if (excelDoc.GetValue(curRow, 1).Trim() != string.Empty)
                    return (curRow + 1);
            }
        }

        private int GetColumnsCount(ExcelHelper excelDoc, int curRow)
        {
            if (this.DataSource.Year >= 2012)
                return 3;
            if (this.DataSource.Year >= 2011)
                return 9;
            for (int curColumn = 1; ; curColumn++)
                if (excelDoc.GetValue(curRow, curColumn).Trim() == string.Empty)
                    return (curColumn - 1);
        }

        private bool CheckColumnsCount(int columnsCount)
        {
            if (this.DataSource.Year >= 2012)
                return columnsCount >= 3;
            if (this.DataSource.Year >= 2011)
                return (columnsCount >= 7);
            else if (this.DataSource.Year >= 2008)
                return ((columnsCount == 4) || (columnsCount == 5));
            return (columnsCount == 5);
        }

        private void PumpXlsSheetOmsk(ExcelHelper excelDoc, FileInfo file)
        {
            int firstRow = GetFirstRow(excelDoc);
            int countEmptyRows = 0;
            columnsCount = GetColumnsCount(excelDoc, firstRow - 1);
            if (!CheckColumnsCount(columnsCount))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                    "Количество столбцов не соответствует формату. Файл {0} закачан не будет.", file.FullName));
                return;
            }

            int refRegion = regionID;
            if (this.DataSource.Year != 2011)
            {
                refRegion = PumpChildRegionOmsk(excelDoc, firstRow, file.Name);
                if (refRegion == -1)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "В файле {0} не указан код района.", file.FullName));
                    return;
                }
            }

            int refCadastQuarter = -1;
            if (this.DataSource.Year == 2011)
                refCadastQuarter = PumpCadastQuarterOmsk(excelDoc.GetValue("D3").Trim().PadLeft(1, '0'));

            int rowsCount = excelDoc.GetRowsCount();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = firstRow; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    if (IsEmptyRow(excelDoc, curRow))
                    {
                        if (this.DataSource.Year == 2012)
                        {
                            countEmptyRows++;
                            if (countEmptyRows < 10)
                                continue;
                        }
                        return;
                    }

                    countEmptyRows = 0;

                    if (IsTotalRow(excelDoc.GetValue(curRow, 2)))
                        continue;

                    string cellValue = excelDoc.GetValue(curRow, 1).Trim();
                    if ((columnsCount == 5) && (cellValue != string.Empty))
                    {
                        refCadastQuarter = PumpCadastQuarterOmsk(cellValue);
                    }

                    PumpXlsRowOmsk(excelDoc, curRow, refRegion, refCadastQuarter);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, file.FullName, ex.Message), ex);
                }
        }

        #endregion Омск

        #region Краснодар

        #region Классификаторы

        private int PumpParentRegionKrasnodar(ExcelHelper excelDoc)
        {
            string regionName = excelDoc.GetValue(3, 1).Trim('-').Trim();
            string regionCode = excelDoc.GetValue(KRASNODAR_FIRST_ROW, 4).Trim().Substring(0, 5).Remove(2, 1);
            return PumpParentRegion(regionName, regionCode);
        }

        private int PumpTaxKrasnodar(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 5).Trim();
            string number = excelDoc.GetValue(curRow, 6).Trim();
            return PumpCachedRow(cacheTaxNumber, dsTax.Tables[0], clsTax, number,
                new object[] { "Name", name, "KindNumber", number, "SourceID", clsDataSourceId });
        }

        private void UpdateCadastQuarterKrasnodar()
        {
            if (dsCadastQuarter.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateDataSet(daCadastQuarter, dsCadastQuarter, clsCadastQuarter);
                dsCadastQuarter.Tables[0].Clear();
            }
        }

        private int PumpCadastQuarterKrasnodar(ExcelHelper excelDoc, int curRow)
        {
            string number = excelDoc.GetValue(curRow, 4).Trim();
            string location = excelDoc.GetValue(curRow, 3).Trim();
            string parentNumber = number.Substring(0, 15);
            int parentId = PumpCachedRow(cacheCadastQuarter, dsCadastQuarter.Tables[0], clsCadastQuarter,
                parentNumber, new object[] { "QuarterNumber", parentNumber });
            int id = PumpCachedRow(cacheCadastQuarter, dsCadastQuarter.Tables[0], clsCadastQuarter,
                number, new object[] { "QuarterNumber", number, "Location", location, "ParentId", parentId });
            // ахуенно много записей - чтобы не занимать много памяти и тормозить закачку - сохраняем построчно
            UpdateCadastQuarterKrasnodar();
            return id;
        }

        private int PumpRegionKrasnodar(ExcelHelper excelDoc, int curRow)
        {
            string name = excelDoc.GetValue(curRow, 2).Trim();
            string code = excelDoc.GetValue(curRow, 4).Trim().Substring(0, 5).Remove(2, 1);
            int refRegion = PumpRegion(name, code, regionID.ToString());
            DeleteRegionData(refRegion, name);
            return refRegion;
        }

        #endregion Классификаторы

        private void PumpXlsRowKrasnodar(ExcelHelper excelDoc, int curRow)
        {
            int refTax = PumpTaxKrasnodar(excelDoc, curRow);
            int refCadastQuarter = PumpCadastQuarterKrasnodar(excelDoc, curRow);
            int refRegion = PumpRegionKrasnodar(excelDoc, curRow);
            decimal totalArea = ConvertFactValue(excelDoc.GetValue(curRow, 7));
            decimal specificIndex = ConvertFactValue(excelDoc.GetValue(curRow, 8));
            decimal cadastralCost = ConvertFactValue(excelDoc.GetValue(curRow, 9));
            if ((totalArea == 0) && (cadastralCost == 0) && (specificIndex == 0))
                return;
            object[] mapping = new object[] {
                "TotalArea", totalArea,
                "CadastralCost", cadastralCost,
                "SpecificIndex", specificIndex,
                "RefCadastQuarter", refCadastQuarter,
                "RefYearDayUNV", this.DataSource.Year * 10000,
                "RefRegions", refRegion,
                "RefTax", refTax };
            PumpFactRow(mapping);
        }

        private const int KRASNODAR_FIRST_ROW = 7;
        private void PumpXlsSheetKrasnodar(ExcelHelper excelDoc, FileInfo file)
        {
            // закачиваем родительский район - один на файл
            regionID = PumpParentRegionKrasnodar(excelDoc);
            int rowsCount = excelDoc.GetRowsCount();
            string dataSourcePath = GetShortSourcePathBySourceID(this.SourceID);
            for (int curRow = KRASNODAR_FIRST_ROW; curRow <= rowsCount; curRow++)
                try
                {
                    SetProgress(rowsCount, curRow,
                        string.Format("Обработка файла {0}\\{1}...", dataSourcePath, file.Name),
                        string.Format("Строка {0} из {1}", curRow, rowsCount));

                    if (IsEmptyRow(excelDoc, curRow))
                        return;
                    if (IsTotalRow(excelDoc.GetValue(curRow, 2)))
                        continue;
                    PumpXlsRowKrasnodar(excelDoc, curRow);
                    break;
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        curRow, file.FullName, ex.Message), ex);
                }
        }

        #endregion Краснодар

        // проверить, поддерживает ли установленный Excel формат закачиваемого файла
        private bool CheckXlsFileVersion(ExcelHelper excelDoc)
        {
            string version = excelDoc.Version.Trim();
            version = version.Split(new char[] { '.' })[0].Trim().PadLeft(1, '0');
            if (Convert.ToInt32(version) <= 11)
            {
                string value = excelDoc.GetValue("A1");
                return !value.Contains("Content_Types");
            }
            return true;
        }

        private void PumpXlsFile(FileInfo file)
        {
            if (!CheckFileName(file))
                return;

            ExcelHelper excelDoc = new ExcelHelper();
            try
            {
                excelDoc.AskToUpdateLinks = false;
                excelDoc.DisplayAlerts = false;
                excelDoc.OpenDocument(file.FullName);
                if (!CheckXlsFileVersion(excelDoc))
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError, string.Format(
                        "Неизвестный формат файла. Файл {0} закачан не будет.", file.FullName));
                    return;
                }

                int sheetsCount = excelDoc.GetWorksheetsCount();
                for (int index = 1; index <= sheetsCount; index++)
                {
                    excelDoc.SetWorksheet(index);

                    if (this.Region == RegionName.Omsk)
                    {
                        // за 2007 и 2011 год качаем только 1й лист
                        if ((this.DataSource.Year == 2007 || this.DataSource.Year >= 2011) && index > 1)
                            break;
                        // не качаем пустые листы
                        if (IsEmptySheet(excelDoc))
                            continue;
                        PumpXlsSheetOmsk(excelDoc, file);
                    }
                    else if (this.Region == RegionName.Krasnodar)
                    {
                        if (excelDoc.GetValue(1, 1).Trim() == string.Empty)
                            continue;
                        PumpXlsSheetKrasnodar(excelDoc, file);
                    }
                }
            }
            finally
            {
                if (excelDoc != null)
                    excelDoc.CloseDocument();
            }
        }

        private void ProcessXlsFilesOmsk(DirectoryInfo dir)
        {
            if (this.DataSource.Year >= 2011)
            {
                // иерархия каталогов с 2011 года
                // Год / Вариант / Район / *.xls (refRegion ставим на район)
                // или
                // Год / Вариант / Район / Поселение / *.xls (refRegion ставим на поселение)
                DirectoryInfo[] regionDirs = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (DirectoryInfo regionDir in regionDirs)
                {
                    regionID = PumpParentRegion(regionDir.Name, "0");
                    DeleteRegionData(regionID, regionDir.Name);
                    ProcessFilesTemplate(regionDir, "*.xls", new ProcessFileDelegate(PumpXlsFile), false, SearchOption.TopDirectoryOnly);
                    int parentId = regionID;
                    foreach (DirectoryInfo subregionDir in regionDir.GetDirectories())
                    {
                        regionID = PumpRegion(subregionDir.Name, "0", parentId.ToString());
                        DeleteRegionData(regionID, subregionDir.Name);
                        ProcessFilesTemplate(subregionDir, "*.xls", new ProcessFileDelegate(PumpXlsFile));
                    }
                    UpdateData();
                }
            }
            else
            {
                foreach (DirectoryInfo d in dir.GetDirectories())
                {
                    // закачиваем родительский район - один на директорию
                    regionID = PumpParentRegion(d.Name, "0");
                    ProcessFilesTemplate(d, "*.xls", new ProcessFileDelegate(PumpXlsFile));
                    UpdateData();
                }
            }
        }

        private void ProcessXlsFilesKrasnodar(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXlsFile));
            UpdateData();
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            deletedList = new List<string>();
            try
            {
                switch (this.Region)
                {
                    case RegionName.Omsk:
                        ProcessXlsFilesOmsk(dir);
                        break;
                    case RegionName.Krasnodar:
                        ProcessXlsFilesKrasnodar(dir);
                        break;
                }
            }
            finally
            {
                deletedList.Clear();
            }
        }
        
        protected override void DirectPumpData()
        {
            PumpDataYVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Сопоставление

        protected override int GetClsSourceID(int sourceID)
        {
            if (sourceID <= 0)
                return -1;
            IDataSource ds = this.Scheme.DataSourceManager.DataSources[sourceID];
            IDataSource clsDs = FindDataSource(ParamKindTypes.Year, ds.SupplierCode, ds.DataCode, string.Empty, ds.Year, 0, string.Empty, 0, string.Empty);
            if (clsDs == null)
                return -1;
            return clsDs.ID;
        }

        #endregion Сопоставление

    }
}