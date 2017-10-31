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

namespace Krista.FM.Server.DataPumps.ORG5Pump
{
    // ОРГАНИЗАЦИИ_0005 прайс - лист
    public class ORG5PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Организации.Реестр организаций (d_Org_RegistrOrg)
        private IDbDataAdapter daOrgReg;
        private DataSet dsOrgReg;
        private IClassifier clsOrgReg;
        private Dictionary<string, DataRow> cacheOrgReg = null;
        // Организации.Реестр продукции (d_Org_ReestrProduct)
        private IDbDataAdapter daProductReg;
        private DataSet dsProductReg;
        private IClassifier clsProductReg;
        private Dictionary<string, int> cacheProductReg = null;

        #endregion Классификаторы

        #region Факты

        // Организации.ОРГАНИЗАЦИИ_Прайс лист (f_Org_PriceList)
        private IDbDataAdapter daPriceList;
        private DataSet dsPriceList;
        private IFactTable fctPriceList;

        #endregion Факты

        private ExcelHelper excelHelper;
        private int rowsCount = 0;
        private object excelObj = null;
        private List<string> deletedDataList = null;
        int maxOrgRegCode = 0;
        int maxProductRegCode = 0;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitDataSet(ref daOrgReg, ref dsOrgReg, clsOrgReg, false, string.Empty, string.Empty);
            maxOrgRegCode = Convert.ToInt32(Convert.ToString(this.DB.ExecQuery(string.Format("select max(Code) FROM {0}", clsOrgReg.FullDBName),
                QueryResultTypes.Scalar, new IDbDataParameter[] { })).PadLeft(1, '0'));
            InitDataSet(ref daProductReg, ref dsProductReg, clsProductReg, false, string.Empty, string.Empty);
            maxProductRegCode = Convert.ToInt32(Convert.ToString(this.DB.ExecQuery(string.Format("select max(Code) FROM {0}", clsProductReg.FullDBName),
                QueryResultTypes.Scalar, new IDbDataParameter[] { })).PadLeft(1, '0'));
            InitFactDataSet(ref daPriceList, ref dsPriceList, fctPriceList);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheOrgReg, dsOrgReg.Tables[0], new string[] { "NameOrg" });
            FillRowsCache(ref cacheProductReg, dsProductReg.Tables[0], 
                new string[] { "Name", "Characteristic", "UnitProduct" }, "|", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrgReg, dsOrgReg, clsOrgReg);
            UpdateDataSet(daProductReg, dsProductReg, clsProductReg);
            UpdateDataSet(daPriceList, dsPriceList, fctPriceList);
        }

        private const string D_ORG_REG_GUID = "c924d846-afdc-4f08-8e64-c572eec75405";
        private const string D_PRODUCT_REG_GUID = "735d9773-8873-41a7-b322-664787e47346";
        private const string F_D_PRICE_LIST_GUID = "8709ad15-7f65-4248-887f-b945e7308e0d";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                    clsOrgReg = this.Scheme.Classifiers[D_ORG_REG_GUID],
                    clsProductReg = this.Scheme.Classifiers[D_PRODUCT_REG_GUID] };
            this.UsedFacts = new IFactTable[] { fctPriceList = this.Scheme.FactTables[F_D_PRICE_LIST_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsPriceList);
            ClearDataSet(ref dsOrgReg);
            ClearDataSet(ref dsProductReg);
        }

        #endregion Работа с базой и кэшами

        #region Работа с экселем

        // возвращает количетсво строк в выбранном Excel-листе отчёта
        private int GetRowsCount(object sheet)
        {
            int emptyStrCount = 0;
            int curRow = 1;
            while (emptyStrCount < 10)
            {
                if (excelHelper.GetCell(sheet, curRow, 1).Value.Trim() == string.Empty)
                    emptyStrCount++;
                else
                    emptyStrCount = 0;
                curRow++;
            }
            return (curRow - 10);
        }

        private int PumpProductReg(DataRow row)
        {
            string name = row["PRODUCT_NAME"].ToString().Trim();
            string unit = row["PRODUCT_UNIT"].ToString().Trim();
            string characteristic = row["PRODUCT_CHAR"].ToString().Trim(); 
            string key = string.Format("{0}|{1}|{2}", name, characteristic, unit);
            if (!cacheProductReg.ContainsKey(key))
                maxProductRegCode++;
            object[] mapping = new object[] { "Code", maxProductRegCode, "Name", name, "UnitProduct", unit,
                "Characteristic", characteristic, "PercentNDS", row["NDS"].ToString() };
            return PumpCachedRow(cacheProductReg, dsProductReg.Tables[0], clsProductReg, key, mapping);
        }

        private void PumpXlsRow(DataRow row, int refDate, int refOrgReg)
        {
            decimal price = Convert.ToDecimal(row["PRICE"].ToString().PadLeft(1, '0'));
            decimal priceNds = Convert.ToDecimal(row["PRICE_NDS"].ToString().PadLeft(1, '0'));
            if ((price == 0) && (priceNds == 0))
                return;
            int refProductReg = PumpProductReg(row);
            object[] mapping = new object[] { "PriceNDC", priceNds, "PriceNDSOut", price, 
                "RefYearDay", refDate, "RefOrg", refOrgReg, "RefOrgReestrProduct", refProductReg };
            PumpRow(dsPriceList.Tables[0], mapping);
            if (dsPriceList.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daPriceList, ref dsPriceList);
            }
        }

        private void DeletePriceData(int refDate, int refOrgReg)
        {
            string key = string.Format("{0}|{1}", refDate, refOrgReg);
            if (deletedDataList.Contains(key))
                return;
            DeleteData(string.Format("RefYearDay = {0} and RefOrg = {1}", refDate, refOrgReg),
                string.Format("Дата отчета: {0}. Id организации: {1}", refDate, refOrgReg));
            deletedDataList.Add(key);
        }

        private int PumpOrgReg(string orgName, object sheet)
        {
            object[] mapping = new object[] { "NameOrg", orgName,
                "Phone", excelHelper.GetCell(sheet, 7, 3).Value,
                "Email", excelHelper.GetCell(sheet, 8, 3).Value,
                "INN", excelHelper.GetCell(sheet, 5, 3).Value,
                "KPP", excelHelper.GetCell(sheet, 6, 3).Value };
            if (cacheOrgReg.ContainsKey(orgName))
            {
                DataRow row = cacheOrgReg[orgName];
                CopyValuesToRow(row, mapping);
                return Convert.ToInt32(row["Id"]);
            }
            else
            {
                maxOrgRegCode++;
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "Code", maxOrgRegCode });
                return PumpCachedRow(cacheOrgReg, dsOrgReg.Tables[0], clsOrgReg, orgName, mapping);
            }
        }

        private object[] XLS_MAPPING = new object[] { "PRODUCT_NAME", 2, "PRODUCT_UNIT", 3, "PRODUCT_CHAR", 4, "NDS", 7, "PRICE", 5, "PRICE_NDS", 6 };
        private void PumpXlsSheet(object sheet, string orgName, string fileName)
        {
            int refDate = CommonRoutines.ShortDateToNewDate(excelHelper.GetCell(sheet, 1, 5).Value);
            int refOrgReg = PumpOrgReg(orgName, sheet);
            DeletePriceData(refDate, refOrgReg);
            DataTable dt = excelHelper.GetSheetDataTable(sheet, 11, rowsCount - 1, XLS_MAPPING);
            for (int i = 0; i < dt.Rows.Count; i++)
                try
                {
                    PumpXlsRow(dt.Rows[i], refDate, refOrgReg);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} отчета {1} возникла ошибка ({2})",
                        i + 1, fileName, ex.Message), ex);
                }
        }

        private void PumpXLSFile(FileInfo file)
        {
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                int sheetCount = excelHelper.GetSheetCount(workbook);
                for (int i = 1; i <= sheetCount; i++)
                {
                    object sheet = excelHelper.GetSheet(workbook, i);
                    if (rowsCount < 10)
                        return;
                    PumpXlsSheet(sheet, file.Directory.Name, file.FullName);
                    UpdateData();
                }
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion Работа с экселем

        #region Перекрытые методы закачки

        protected override void DeleteEarlierPumpedData()
        {
            DirectDeleteData(-1, -1, string.Empty);
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            deletedDataList = new List<string>();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
                UpdateData();
            }
            finally
            {
                if (excelHelper != null)
                    excelHelper.Close();
                deletedDataList.Clear();
            }
        }

        protected override void DirectPumpData()
        {
            SetDataSource(ParamKindTypes.WithoutParams, string.Empty, 0, 0, string.Empty, 0, string.Empty);
            PumpDataSource(this.RootDir);
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
