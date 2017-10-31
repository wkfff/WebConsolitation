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


namespace Krista.FM.Server.DataPumps.EO8Pump
{

    // эо 8 - средние цены
    public class EO8PumpModule : CorrectedPumpModuleBase
    {
        #region Поля

        #region Классификаторы

        // Мониторинг.Каталог продукции (d_StOrder_Product)
        private IDbDataAdapter daProduct;
        private DataSet dsProduct;
        private IClassifier clsProduct;
        private Dictionary<string, int> cacheProduct = null;
        // Мониторинг.Единицы измерения (d_StOrder_Units)
        private IDbDataAdapter daUnits;
        private DataSet dsUnits;
        private IClassifier clsUnits;
        private Dictionary<string, int> cacheUnits = null;
        private int nullUnits = -1;
        // Мониторинг.ОКДП (d_StOrder_OKDP)
        private IClassifier clsOKDP;
        private int nullOKDP = -1;
        // Мониторинг.ОКП (d_StOrder_OKP)
        private IClassifier clsOKP;
        private int nullOKP = -1;
        // Мониторинг.Номенклатура продукции (d_StOrder_RangeProduct)
        private IClassifier clsRangeProduct;
        private int nullRangeProduct = -1;

        #endregion Классификаторы

        #region Факты

        // Мониторинг.Средние цены на товары и услуги (f_StOrder_AveragePriseList)
        private IDbDataAdapter daPrice;
        private DataSet dsPrice;
        private IFactTable fctPrice;

        #endregion Факты

        private ExcelHelper excelHelper;
        private object excelObj = null;
        private int eo7SourceId;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void InitUpdatedFixedRows()
        {
            nullUnits = clsUnits.UpdateFixedRows(this.DB, this.SourceID);
            nullOKDP = clsOKDP.UpdateFixedRows(this.DB, this.SourceID);
            nullOKP = clsOKP.UpdateFixedRows(this.DB, this.SourceID);
            nullRangeProduct = clsRangeProduct.UpdateFixedRows(this.DB, this.SourceID);
        }

        protected override void QueryData()
        {
            string variant = string.Format("{0} - {1}", this.DataSource.Year, this.DataSource.Month);
            eo7SourceId = AddDataSource("ЭО", "0007", ParamKindTypes.Variant, string.Empty, 0, 0, variant, 0, string.Empty).ID;
            string constr = string.Format("SOURCEID = {0}", eo7SourceId);
            DirectDeleteFactData(this.UsedFacts, -1, -1, constr);
            DirectDeleteClsData(this.UsedClassifiers, -1, -1, constr);

            InitDataSet(ref daProduct, ref dsProduct, clsProduct, false, constr, string.Empty);
            InitDataSet(ref daUnits, ref dsUnits, clsUnits, true, string.Empty, string.Empty);
            InitDataSet(ref daPrice, ref dsPrice, fctPrice, false, constr, string.Empty);

            FillCaches();
            InitUpdatedFixedRows();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheProduct, dsProduct.Tables[0], "CODE", "ID");
            FillRowsCache(ref cacheUnits, dsUnits.Tables[0], "Name", "ID");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daProduct, dsProduct, clsProduct);
            UpdateDataSet(daPrice, dsPrice, fctPrice);
        }

        private const string D_PRODUCT_GUID = "e134f3b4-7f9c-4e13-b146-e0ff6a54f724";
        private const string D_UNITS_GUID = "6b07f538-0d77-4b77-b76a-4a4fa4a378be";
        private const string D_OKDP_GUID = "5de0589b-2e8c-4842-93c5-96e30d331846";
        private const string D_OKP_GUID = "3ca397aa-2edb-4652-9734-71bb7641ecd0";
        private const string D_RANGE_PRODUCT_GUID = "6d81d937-ed7d-456c-8507-286518202eb4";
        private const string F_PRICE_GUID = "a2f11793-0c2d-4ca7-8a2a-8ef8894bc3b5";
        protected override void InitDBObjects()
        {
            clsUnits = this.Scheme.Classifiers[D_UNITS_GUID];
            clsOKDP = this.Scheme.Classifiers[D_OKDP_GUID];
            clsOKP = this.Scheme.Classifiers[D_OKP_GUID];
            clsRangeProduct = this.Scheme.Classifiers[D_RANGE_PRODUCT_GUID];
            this.UsedClassifiers = new IClassifier[] {
                clsProduct = this.Scheme.Classifiers[D_PRODUCT_GUID] };
            this.UsedFacts = new IFactTable[] { 
                fctPrice = this.Scheme.FactTables[F_PRICE_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsPrice);
            ClearDataSet(ref dsProduct);
        }

        #endregion Работа с базой и кэшами

        #region работа с Excel

        private void PumpFactRow(int curRow, object sheet, int refProduct, int refDate)
        {
            decimal price = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 3).Value.Trim().
                Replace("-", string.Empty).PadLeft(1, '0'));
            decimal priceChange = Convert.ToDecimal(excelHelper.GetCell(sheet, curRow, 4).Value.Trim().
                Replace("-", string.Empty).PadLeft(1, '0'));
            if ((price == 0) && (priceChange == 0))
                return;

            object[] mapping = new object[] { "AveragePrice", price, "PriceChange", priceChange, 
                "RefProduct", refProduct, "RefYearDayUNV", refDate, "SourceId", eo7SourceId };
            PumpRow(dsPrice.Tables[0], mapping);
            if (dsPrice.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daPrice, ref dsPrice);
            }
        }

        private int PumpProduct(int curRow, object sheet)
        {
            string nameValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
            string name = nameValue.Split(',')[0].Trim();
            string code = excelHelper.GetCell(sheet, curRow, 2).Value.Trim();

            string unit = nameValue.Split(',')[1].Trim();
            int refUnits = FindCachedRow(cacheUnits, unit, nullUnits);

            object[] mapping = new object[] { "NAME", name, "CODE", code, "RefUnits", refUnits, "RefOKDP", nullOKDP, 
                "RefOKP", nullOKP, "RefRangeProduct", nullRangeProduct, "SourceId", eo7SourceId };
            return PumpCachedRow(cacheProduct, dsProduct.Tables[0], clsProduct, code, mapping);
        }

        private void PumpXLSSheetData(object sheet)
        {
            int refDate = this.DataSource.Year * 10000 + this.DataSource.Month * 100;
            for (int curRow = 6; ; curRow++)
            {
                try
                {
                    string cellValue = excelHelper.GetCell(sheet, curRow, 1).Value.Trim();
                    if (cellValue == string.Empty)
                        return;
                    int refProduct = PumpProduct(curRow, sheet);
                    PumpFactRow(curRow, sheet, refProduct, refDate);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("При обработке строки {0} возникла ошибка ({1})", curRow, ex.Message), ex);
                }
            }
        }

        private void PumpXLSFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            object workbook = excelHelper.InitWorkBook(ref excelObj, file.FullName);
            try
            {
                object sheet = excelHelper.GetSheet(workbook, 1);
                PumpXLSSheetData(sheet);
            }
            finally
            {
                excelHelper.CloseExcel(ref excelObj);
            }
        }

        #endregion работа с Excel

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStart, "Старт инициализации Excel.");
            excelHelper = new ExcelHelper();
            try
            {
                ProcessFilesTemplate(dir, "*.xls", new ProcessFileDelegate(PumpXLSFile), false);
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
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }

}
