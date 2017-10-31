using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.IO.Compression;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.EO9Pump
{
    // эо 9 - Закупки малого объема
    public class EO9PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Госзаказ.Территории (d.StOrder.Territory)
        private IDbDataAdapter daTerr;
        private DataSet dsTerr;
        private IClassifier clsTerr;
        private Dictionary<string, int> cacheTerr = null;

        // Мониторинг.Договора (d_StOrder_Agreements)
        private IDbDataAdapter daAgreements;
        private DataSet dsAgreements;
        private IClassifier clsAgreements;
        private Dictionary<int, DataRow> cacheAgreements = null;
        private int nullAgreement = -1;
        // Мониторинг.Организации (d_StOrder_Organization)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<int, DataRow> cacheOrg = null;
        private int nullOrg = -1;
        // Госзаказ.Каталог продукции (d_StOrder_Product)
        private IDbDataAdapter daProduct;
        private DataSet dsProduct;
        private IClassifier clsProduct;
        private Dictionary<int, DataRow> cacheProduct = null;
        private int nullProduct = -1;
        // Мониторинг.Период размещения заказа (d_StOrder_RegPeriod)
        private IDbDataAdapter daRegPeriod;
        private DataSet dsRegPeriod;
        private IClassifier clsRegPeriod;
        private Dictionary<int, DataRow> cacheRegPeriod = null;
        private int nullRegPeriod = -1;
        // Мониторинг.Единицы измерения (d_StOrder_Units)
        private IClassifier clsUnits;
        private int nullUnits = -1;
        // Мониторинг.ОКДП (d_StOrder_OKDP)
        private IClassifier clsOkdp;
        private int nullOkdp = -1;

        #endregion Классификаторы

        #region Факты

        // Мониторинг.ЭО_Мониторинг_Закупки малого объема (f_StOrder_TenderLowSize)
        private IDbDataAdapter daEO9;
        private DataSet dsEO9;
        private IFactTable fctEO9;
        private Dictionary<int, DataRow> cacheEO9 = null;

        #endregion Факты

        private int eo7SourceId;
        private string variant;
        private Database budDB = null;
        private int refTerr;
        private Dictionary<int, int> cacheDateUNV = null;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void InitUpdatedFixedRows()
        {
            nullOrg = clsOrg.UpdateFixedRows(this.DB, this.SourceID);
            nullRegPeriod = clsRegPeriod.UpdateFixedRows(this.DB, this.SourceID);
            nullAgreement = clsAgreements.UpdateFixedRows(this.DB, this.SourceID);
            nullProduct = clsProduct.UpdateFixedRows(this.DB, this.SourceID);
            nullUnits = clsUnits.UpdateFixedRows(this.DB, this.SourceID);
        }

        private void InitTerrTables()
        {
            InitDataSet(ref daTerr, ref dsTerr, clsTerr, true, string.Empty, string.Empty);
            FillRowsCache(ref cacheTerr, dsTerr.Tables[0], "ShortName");
        }

        protected override void QueryData()
        {
            eo7SourceId = AddDataSource("ЭО", "0007", ParamKindTypes.Variant, string.Empty, 0, 0, variant, 0, string.Empty).ID;
            string constr = string.Format("SOURCEID = {0}", eo7SourceId);

            InitClsDataSet(ref daAgreements, ref dsAgreements, clsAgreements);
            InitClsDataSet(ref daOrg, ref dsOrg, clsOrg);
            InitClsDataSet(ref daProduct, ref dsProduct, clsProduct);
            InitDataSet(ref daRegPeriod, ref dsRegPeriod, clsRegPeriod, false, constr, string.Empty);

            InitDataSet(ref daEO9, ref dsEO9, fctEO9, false, constr, string.Empty);

            FillCaches();
            InitUpdatedFixedRows();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheAgreements, dsAgreements.Tables[0], "SourceKey");
            FillRowsCache(ref cacheOrg, dsOrg.Tables[0], "SourceKey");
            FillRowsCache(ref cacheProduct, dsProduct.Tables[0], "SourceKey");
            FillRowsCache(ref cacheRegPeriod, dsRegPeriod.Tables[0], "SourceKey");
            FillRowsCache(ref cacheEO9, dsEO9.Tables[0], "SourceKey");

            DataTable dt = (DataTable)this.DB.ExecQuery("select id from fx_Date_YearDayUNV", 
                QueryResultTypes.DataTable, new IDbDataParameter[] { });
            FillRowsCache(ref cacheDateUNV, dt, "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daProduct, dsProduct, clsProduct);
            UpdateDataSet(daAgreements, dsAgreements, clsAgreements);
            UpdateDataSet(daEO9, dsEO9, fctEO9);
        }

        private const string D_TERR_GUID = "ce940102-9e33-40f0-8a6a-989fd6e09534";
        private const string D_AGREEMENT_GUID = "c6a59d4d-f6d9-4b33-a2ae-5e92817cfbcb";
        private const string D_ORG_GUID = "0cce5ec1-fbf2-452f-8db2-168376c76bcd";
        private const string D_PRODUCT_GUID = "e134f3b4-7f9c-4e13-b146-e0ff6a54f724";
        private const string D_REG_PERIOD_GUID = "3c7aff61-814a-46b2-a337-dc048d013114";
        private const string D_OKP_GUID = "3ca397aa-2edb-4652-9734-71bb7641ecd0";
        private const string D_UNITS_GUID = "6b07f538-0d77-4b77-b76a-4a4fa4a378be";
        private const string F_EO9_GUID = "4f9b6a0d-3f64-42e6-971d-ffd32fc08789";
        protected override void InitDBObjects()
        {
            clsTerr = this.Scheme.Classifiers[D_TERR_GUID];

            clsRegPeriod = this.Scheme.Classifiers[D_REG_PERIOD_GUID];
            clsOkdp = this.Scheme.Classifiers[D_OKP_GUID];
            clsUnits = this.Scheme.Classifiers[D_UNITS_GUID];
            clsProduct = this.Scheme.Classifiers[D_PRODUCT_GUID];
            this.UsedClassifiers = new IClassifier[] { 
                clsAgreements = this.Scheme.Classifiers[D_AGREEMENT_GUID], 
                clsOrg = this.Scheme.Classifiers[D_ORG_GUID] };

            this.UsedFacts = new IFactTable[] { fctEO9 = this.Scheme.FactTables[F_EO9_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsEO9);
            ClearDataSet(ref dsAgreements);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsProduct);
            ClearDataSet(ref dsRegPeriod);
        }

        #endregion Работа с базой и кэшами

        #region работа с базой бюджета

        #region инициализация и подключение к базе бюджета

        protected Database ConnectToDatabase(FileInfo udlFile)
        {
            IDbConnection connection = null;
            bool isOleDbConnection = true;
            UDLFileDataAccess.GetConnectionFromUdl(udlFile.FullName, ref connection, ref isOleDbConnection);
            if (connection == null)
                throw new Exception("Невозможно установить соединение с источником данных. Проверьте подключение к базе Бюджета.");
            if (isOleDbConnection)
                return new Database(connection, DbProviderFactories.GetFactory("System.Data.Odbc"), false, constCommandTimeout);
            else
                return new Database(connection, DbProviderFactories.GetFactory("System.Data.OleDb"), false, constCommandTimeout);
        }

        protected void InitBudDB(FileInfo udlFile)
        {
            budDB = ConnectToDatabase(udlFile);
            if (budDB == null)
                throw new Exception("Ошибка при подключении к базе бюджета."); 
        }

        private void DisposeBudDB()
        {
            if (budDB != null)
                budDB.Dispose();
            budDB = null;
        }

        #endregion инициализация и подключение к базе бюджета

        #region общие методы

        private void UpdateSourceKeyRow(DataRow row, object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                row[mapping[i].ToString()] = mapping[i + 1];
        }

        private void SetClsRefs(DataRow row, string[] clsRefsNames, Dictionary<int, DataRow>[] clsCaches, int[] nullClsRefs)
        {
            for (int i = 0; i <= clsRefsNames.GetLength(0) - 1; i++)
                if (row[clsRefsNames[i]] == DBNull.Value)
                    row[clsRefsNames[i]] = nullClsRefs[i];
                else
                {
                    int clsSourceKey = Convert.ToInt32(row[clsRefsNames[i]]);
                    if (!clsCaches[i].ContainsKey(clsSourceKey))
                        row[clsRefsNames[i]] = nullClsRefs[i];
                    else
                        row[clsRefsNames[i]] = Convert.ToInt32(clsCaches[i][clsSourceKey]["Id"]);
                }
        }

        private object[] GetRowMapping(DataRow row, object[] mapping)
        {
            for (int i = 0; i <= mapping.GetLength(0) - 1; i += 2)
                try
                {
                    mapping[i + 1] = row[mapping[i + 1].ToString()];
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("При получении значения поля '{0}' произошла ошибка: {1}",
                        mapping[i + 1].ToString(), exp.Message));
                }
            return mapping;
        }

        private void CheckBudDate(DataRow row, string dateFieldName)
        {
            int date = Convert.ToInt32(row[dateFieldName]);
            if (!cacheDateUNV.ContainsKey(date))
            {
                string message = string.Format("Запись бюджета с Id '{0}' содержит некорректную дату ({1}) в поле '{2}'.",
                    row["SourceKey"], date, dateFieldName);
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                row[dateFieldName] = -1;
            }
        }

        private void ProcessBudTable(string query, object[] mapping, Dictionary<int, DataRow> cache, DataTable dt, IEntity obj,
            string[] clsRefsNames, Dictionary<int, DataRow>[] clsCaches, int[] nullClsRefs, string[] dateFieldNames)
        {
            DataTable budData = (DataTable)budDB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            try
            {
                foreach (DataRow budRow in budData.Rows)
                {
                    object[] rowMapping = GetRowMapping(budRow, (object[])mapping.Clone());
                    int sourceKey = Convert.ToInt32(rowMapping[1]);
                    DataRow row = null;
                    if (cache.ContainsKey(sourceKey))
                    {
                        row = cache[sourceKey];
                        UpdateSourceKeyRow(row, rowMapping);
                    }
                    else
                    {
                        PumpCachedRow(cache, dt, obj, sourceKey, rowMapping);
                        row = cache[sourceKey];
                    }
                    if (clsRefsNames != null)
                        SetClsRefs(row, clsRefsNames, clsCaches, nullClsRefs);
                    if (dateFieldNames != null)
                        foreach (string dateFieldName in dateFieldNames)
                            CheckBudDate(row, dateFieldName);
                }
            }
            finally
            {
                budData.Clear();
                budData = null;
            }
        }

        private DateTime GetTableMaxDate(string entityDbName)
        {
            object maxDate = DBNull.Value;
            if (!this.DeleteEarlierData)
                maxDate = this.DB.ExecQuery(string.Format("select max(DateModify) FROM {0}", entityDbName),
                    QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if (maxDate == DBNull.Value)
                return new DateTime(1990, 1, 1, 0, 0, 0);
            else
                return Convert.ToDateTime(maxDate);
        }

        private void SetRowsFxValue(DataTable dt, string[] pairNameValue)
        {
            foreach (DataRow row in dt.Rows)
                for (int i = 0; i < pairNameValue.GetLength(0); i += 2)
                    row[pairNameValue[i]] = pairNameValue[i + 1];
        }

        #endregion общие методы

        #region закачка классификаторов

        private const string QUERY_AGREEMENT = "select * from Agreements where (StateOrderRef is null) and (LastAccepted = 1) and (UpdateDate >= '{0}')";
        private void PumpAgreements()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Мониторинг.Договора'");
            string query = string.Format(QUERY_AGREEMENT, GetTableMaxDate(clsAgreements.FullDBName));
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "DocNumber", 
                "Designation", "Note", "ReestrNumber", "ReestrNumber", "RefCustomer", "Client_Ref", 
                "RefSupplier", "Executer_Ref", "RefAgreementDate", "AgreementDate", 
                "RefAgreementEndDate", "AgreementEndDate", "DateModify", "UpdateDate"};
            ProcessBudTable(query, mapping, cacheAgreements, dsAgreements.Tables[0], clsAgreements,
                new string[] { "RefCustomer", "RefSupplier" }, new Dictionary<int, DataRow>[] { cacheOrg, cacheOrg },
                new int[] { nullOrg, nullOrg }, new string[] { "RefAgreementDate", "RefAgreementEndDate" });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Мониторинг.Договора'");
        }

        private const string QUERY_ORG = "select * from Organizations where (UpdateDate >= '{0}')";
        private void PumpOrg()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Мониторинг.Организации'");
            string query = string.Format(QUERY_ORG, GetTableMaxDate(clsOrg.FullDBName));
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", 
                "Address", "Address", "Code", "INN", "DateModify", "UpdateDate"};
            ProcessBudTable(query, mapping, cacheOrg, dsOrg.Tables[0], clsOrg,
                null, null, null, null);
            SetRowsFxValue(dsOrg.Tables[0], new string[] { "RefOkato", "-1" });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Мониторинг.Организации'");
        }

        private const string QUERY_PRODUCT = "select * from TenderObjects where (UpdateDate >= '{0}')";
        private void PumpProduct()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки классификатора 'Мониторинг.Каталог продукции'");
            string query = string.Format(QUERY_PRODUCT, GetTableMaxDate(clsProduct.FullDBName));
            object[] mapping = new object[] { "SourceKey", "Id", "Name", "Name", 
                "Code", "Code", "Description", "ObjectSpec",  "DateModify", "UpdateDate"};
            ProcessBudTable(query, mapping, cacheProduct, dsProduct.Tables[0], clsProduct, null, null, null, null);
            SetRowsFxValue(dsProduct.Tables[0], new string[] { "RefOKDP", nullOkdp.ToString(), "RefUnits", nullUnits.ToString(), 
                "RefOkp", "-1", "RefRangeProduct", "-1" });
            foreach (DataRow row in dsProduct.Tables[0].Rows)
                if (row["Code"].ToString().Trim() == string.Empty)
                    row["Code"] = constDefaultClsName;
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки классификатора 'Мониторинг.Каталог продукции'");
        }

        private void PumpCls()
        {
            PumpOrg();
            UpdateData();
            PumpAgreements();
            PumpProduct();
            UpdateData();
        }

        #endregion закачка классификаторов

        #region закачка фактов

        private const string QUERY_EO9 = "select id, Amount, Price, Cost, RecordIndex, ProductCLS, UpdateDate from Estimate where (UpdateDate >= '{0}')";
        private void PumpFact()
        {
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Начало закачки факта 'Мониторинг.ЭО_Мониторинг_Закупки малого объема'");
            string query = string.Format(QUERY_EO9, GetTableMaxDate(fctEO9.FullDBName));
            object[] mapping = new object[] { "SourceKey", "Id", "Quantity", "Amount", 
                "Price", "Price", "Cost", "Cost", "RefAgreements", "RecordIndex", 
                "RefProduct ", "ProductCLS", "RefPeriod ", "ProductCLS", "DateModify", "UpdateDate"};
            ProcessBudTable(query, mapping, cacheEO9, dsEO9.Tables[0], fctEO9,
                new string[] { "RefAgreements", "RefProduct", "RefPeriod" }, 
                new Dictionary<int, DataRow>[] { cacheAgreements, cacheProduct, cacheRegPeriod },
                new int[] { nullAgreement, nullProduct, nullRegPeriod }, null);
            SetRowsFxValue(dsEO9.Tables[0], new string[] { "RefTerritory", refTerr.ToString() });
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Завершение закачки факта 'Мониторинг.ЭО_Мониторинг_Закупки малого объема'");
        }

        #endregion закачка фактов

        private void PumpBudDB(FileInfo udlFile)
        {
            InitBudDB(udlFile);
            try
            {
                refTerr = cacheTerr[variant];
                PumpCls();
                PumpFact();
                UpdateData();
            }
            finally
            {
                DisposeBudDB();
            }
        }
        
        #endregion работа с базой бюджета

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.udl", new ProcessFileDelegate(PumpBudDB), false);
        }

        protected override void DirectPumpData()
        {
            InitTerrTables();
            try
            {
                // Выбираем каталоги с вариантами = краткое наименование территории
                DirectoryInfo[] dir_variant = this.RootDir.GetDirectories();
                for (int j = 0; j < dir_variant.GetLength(0); j++)
                {
                    variant = dir_variant[j].Name;
                    if (variant.StartsWith("__"))
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                            string.Format("Каталог источника {0} пропущен по указанию пользователя.", variant));
                        continue;
                    }
                    SetDataSource(ParamKindTypes.Variant, string.Empty, 0, 0, variant, 0, string.Empty);
                    PumpDataSource(dir_variant[j]);
                }
            }
            finally
            {
                ClearDataSet(ref dsTerr);
            }
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region расчет кубов

        private const string EO9_CUBE_GUID = "4f9b6a0d-3f64-42e6-971d-ffd32fc08789";
        private const string EO9_CUBE_NAME = "ЭО_Мониторинг_Закупки малого объема";
        protected override void DirectProcessCube()
        {
            base.DirectProcessCube();
            this.UsedClassifiers = new IClassifier[] { };
            cubesForProcess = new string[] { EO9_CUBE_GUID, EO9_CUBE_NAME };
            base.DirectProcessCube();
        }

        #endregion расчет кубов

    }
}
