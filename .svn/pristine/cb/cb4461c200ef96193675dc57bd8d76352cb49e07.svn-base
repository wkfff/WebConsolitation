using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

using BytesRoad.Net.Ftp;

namespace Krista.FM.Server.DataPumps.OOS1Pump
{

    // ООС - 0001 - Данные АС ООС
    public partial class OOS1PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        #region Не делятся по источнику

        // Территории.РФ (d_Territory_RF)
        // private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory = null;
        private int nullTerritory = -1;
        // АС ООС.ОКДП (d_OOS_OKDP)
        private IDbDataAdapter daOKDP;
        private DataSet dsOKDP;
        private IClassifier clsOKDP;
        private Dictionary<string, int> cacheOKDP = null;
        private DateTime dateLoadingOKDP;
        // АС ООС.ОКСМ (d_OOS_OKSM)
        private IDbDataAdapter daOKSM;
        private DataSet dsOKSM;
        private IClassifier clsOKSM;
        private Dictionary<string, int> cacheOKSM = null;
        private DateTime dateLoadingOKSM;
        private int nullOKSM = -1;
        // АС ООС.Организации (d_OOS_Org)
        private IDbDataAdapter daOrg;
        private DataSet dsOrg;
        private IClassifier clsOrg;
        private Dictionary<string, int> cacheOrg = null;
        // в этом кэше будут храниться уровень бюджета и территория
        private Dictionary<string, string> cacheOrgInfo = null;
        private DateTime dateLoadingOrg;
        private int maxOrgCode = 0;
        private int nullOrg = -1;
        // АС ООС.Бюджеты финансирования (d_OOS_Budgets)
        private IDbDataAdapter daBudgets;
        private DataSet dsBudgets;
        private IClassifier clsBudgets;
        private Dictionary<string, int> cacheBudgets = null;
        private int maxBudgetsCode = 0;
        private int nullBudgets = -1;
        // АС ООС.Виды внебюджетных средств (d_OOS_Extrabudget)
        private IDbDataAdapter daExtrabudget;
        private DataSet dsExtrabudget;
        private IClassifier clsExtrabudget;
        private Dictionary<string, int> cacheExtrabudget = null;
        private int maxExtrabudgetCode = 0;
        private int nullExtrabudget = -1;

        #endregion

        #region Делятся по источнику

        // АС ООС.Закупки (d_OOS_Purchase)
        private IDbDataAdapter daPurchase;
        private DataSet dsPurchase;
        private IClassifier clsPurchase;
        private Dictionary<string, DataRow> cachePurchase = null;
        private int nullPurchase = -1;
        private DateTime dateLoadingPurchase;
        private int maxPurchaseCode = 0;
        // АС ООС.Предметы контракта (d_OOS_Subject)
        private IDbDataAdapter daSubject;
        private DataSet dsSubject;
        private IClassifier clsSubject;
        private Dictionary<string, DataRow> cacheSubject = null;
        private int nullSubject = -1;
        // АС ООС.Контракты (d_OOS_Contracts)
        private IDbDataAdapter daContracts;
        private DataSet dsContracts;
        private IClassifier clsContracts;
        private Dictionary<string, DataRow> cacheContracts = null;
        private int nullContracts = -1;
        private DateTime dateLoadingContracts;
        private int maxContractsCode = 0;
        // АС ООС.Продукция (d_OOS_Products)
        private IDbDataAdapter daProducts;
        private DataSet dsProducts;
        private IClassifier clsProducts;
        private Dictionary<string, int> cacheProducts = null;
        private int maxProductsCode = 0;
        // АС ООС.Поставщики (d_OOS_Suppliers)
        private IDbDataAdapter daSuppliers;
        private DataSet dsSuppliers;
        private IClassifier clsSuppliers;
        private int nullSuppliers = -1;
        private Dictionary<string, int> cacheSuppliers = null;
        private int maxSuppliersCode = 0;

        #endregion

        #region Фиксированные

        // АС ООС.Уровни бюджета РФ (fx_OOS_LevelOrder)
        private IDbDataAdapter daLevelOrder;
        private DataSet dsLevelOrder;
        private IClassifier fxLevelOrder;
        private Dictionary<int, int> cacheLevelOrder = null;
        private int nullLevelOrder = -1;
        // АС ООС.Типы изменений (fx_OOS_Modification)
        private IDbDataAdapter daModification;
        private DataSet dsModification;
        private IClassifier fxModification;
        private Dictionary<string, int> cacheModification = null;
        private int nullModification = -1;
        // АС ООС.Состояния контракта (fx_OOS_ContractStage)
        private IDbDataAdapter daContractStage;
        private DataSet dsContractStage;
        private IClassifier fxContractStage;
        private Dictionary<string, int> cacheContractStage = null;
        private int nullContractStage = -1;
        // АС ООС.Правовые формы организаций (fx_OOS_OrgForm)
        private IDbDataAdapter daOrgForm;
        private DataSet dsOrgForm;
        private IClassifier fxOrgForm;
        private Dictionary<string, int> cacheOrgForm = null;
        private int nullOrgForm = -1;
        // АС ООС.Типы участников (fx_OOS_PartType)
        private IDbDataAdapter daPartType;
        private DataSet dsPartType;
        private IClassifier fxPartType;
        private Dictionary<string, int> cachePartType = null;
        private int nullPartType = -1;
        // АС ООС.Статусы поставщика (fx_OOS_StatusSup)
        private IDbDataAdapter daStatusSup;
        private DataSet dsStatusSup;
        private IClassifier fxStatusSup;
        private Dictionary<string, int> cacheStatusSup = null;
        private int nullStatusSup = -1;
        // Период.Год Квартал Месяц (fx_Date_YearDayUNV)
        private IClassifier fxPeriod;
        private List<int> cachePeriod = new List<int>();

        #endregion

        #endregion Классификаторы

        #region Факты

        // АС ООС.ООС_АС ООС_Данные (f_OOS_Value)
        private IDbDataAdapter daOOSValue;
        private DataSet dsOOSValue;
        private IFactTable fctOOSValue;
        // АС ООС.ООС_АС ООС_Продукция по контрактам (f_OOS_Product)
        private IDbDataAdapter daOOSProduct;
        private DataSet dsOOSProduct;
        private IFactTable fctOOSProduct;

        #endregion Факты

        private ReportType reportType;
        private DateTime reportDate;
        private XmlNamespaceManager nsmgr;
        private int sourceID = -1;
        private Dictionary<string, ReportType> pumpFolders = null;
        // временная папка для загружаемых данных
        private string tempDir = string.Empty;
        // данные конфигурационного файла
        private string ftpServer = string.Empty;
        private string ftpPort = string.Empty;
        private string ftpUsername = string.Empty;
        private string ftpPassword = string.Empty;
        private string proxyServer = string.Empty;
        private string proxyPort = string.Empty;
        private string proxyUsername = string.Empty;
        private string proxyPassword = string.Empty;
        // индивидуальные параметры закачки
        private string ftpRegionFolder = string.Empty;
        private int refTerritoryRF = -1;
        private DataRow territoryRF = null;
        private bool useArchive = false;

        #endregion Поля

        #region Перечисления и структуры

        /// <summary>
        /// Тип закачиваемых данных
        /// </summary>
        private enum ReportType
        {
            /// <summary>
            /// Классификатор видов экономической деятельности, продукции и услуг
            /// </summary>
            Product,
            /// <summary>
            /// Классификатор стран мира
            /// </summary>
            OKSM,
            /// <summary>
            /// Классификатор организаций
            /// </summary>
            Organization,
            /// <summary>
            /// Данные по закупкам
            /// </summary>
            Notifications,
            /// <summary>
            /// Данные по контрактам
            /// </summary>
            Contracts
        }

        /// <summary>
        /// Cпособ закупки
        /// </summary>
        private enum NotificationType
        {
            /// <summary>
            /// Открытый конкурс
            /// </summary>
            OK,
            /// <summary>
            /// Открытый аукцион в электронной форме
            /// </summary>
            EF,
            /// <summary>
            /// Запрос котировок
            /// </summary>
            ZK,
            /// <summary>
            /// Предварительный отбор и запрос котировок при чрезвычайных ситуациях
            /// </summary>
            PO,
            /// <summary>
            /// Сообщение о заинтересованности в проведении конкурса
            /// </summary>
            SZ
        }

        #endregion Перечисления и структуры

        #region Закачка данных

        #region Работа с базой и кэшами

        // формируем новый SourceID в зависимости от папки, которую качаем с ftp
        // также выполняем все действия, которые зависят от этого SourceID: удаление старых данных, инициализация датасетов,
        // запонение кэшей и т.п.
        private void SetNewSourceID(string variant)
        {
            sourceID = AddDataSource("ООС", "0001", ParamKindTypes.Variant, string.Empty, 0, 0, variant, 0, string.Empty).ID;

            string constraint = string.Format("SourceID = {0}", sourceID);
            InitDataSet(ref daPurchase, ref dsPurchase, clsPurchase, constraint);
            InitDataSet(ref daSubject, ref dsSubject, clsSubject, constraint);
            InitDataSet(ref daContracts, ref dsContracts, clsContracts, constraint);
            InitDataSet(ref daProducts, ref dsProducts, clsProducts, constraint);
            InitDataSet(ref daSuppliers, ref dsSuppliers, clsSuppliers, constraint);

            InitFactDataSet(ref daOOSValue, ref dsOOSValue, fctOOSValue);
            InitFactDataSet(ref daOOSProduct, ref dsOOSProduct, fctOOSProduct);

            FillRowsCache(ref cachePurchase, dsPurchase.Tables[0], new string[] { "NotificationNum" });
            FillRowsCache(ref cacheSubject, dsSubject.Tables[0], new string[] { "Sid" });
            FillRowsCache(ref cacheContracts, dsContracts.Tables[0], new string[] { "RegNum" });
            FillRowsCache(ref cacheProducts, dsProducts.Tables[0], new string[] { "Name", "Unit", "RefOKDP" }, "|", "ID");
            FillRowsCache(ref cacheSuppliers, dsSuppliers.Tables[0], new string[] { "Name", "INN", "KPP", "RefFormOrg" }, "|", "ID");

            maxPurchaseCode = GetMaxCode(clsPurchase, constraint);
            maxContractsCode = GetMaxCode(clsContracts, constraint);
            maxProductsCode = GetMaxCode(clsProducts, constraint);
            maxSuppliersCode = GetMaxCode(clsSuppliers, constraint);

            dateLoadingPurchase = GetMaxDateLoading(clsPurchase, constraint);
            dateLoadingContracts = GetMaxDateLoading(clsContracts, constraint);
        }

        private void FillOrgCaches()
        {
            cacheOrg = new Dictionary<string, int>();
            cacheOrgInfo = new Dictionary<string, string>();
            foreach (DataRow row in dsOrg.Tables[0].Rows)
            {
                string regNumber = Convert.ToString(row["RegNumber"]);
                if (!cacheOrg.ContainsKey(regNumber))
                {
                    cacheOrg.Add(regNumber, Convert.ToInt32(row["ID"]));
                    cacheOrgInfo.Add(regNumber, string.Format("{0}|{1}", row["SubordinationType"], row["RefTerritory"]));
                }
            }
        }

        // возвращает запрос, рекурсивно выбирающий территории, начиная с территории ID = startID
        private string GetTerritoryHeirarchyQuery(int startID)
        {
            switch (this.ServerDBMSName)
            {
                // для СУБД Oracle
                case DBMSName.Oracle:
                    return string.Format(@"
select ID, OKATO, PARENTID
from {0}
start with ID = {1}
connect by PARENTID = prior ID
", clsTerritory.FullDBName, startID);

                // для СУБД MSSQL
                case DBMSName.SQLServer:
                    return string.Format(@"
with TERRITORY_HIERARCHY (ID, PARENTID, OKATO) as
(
    select ID, PARENTID, OKATO
    from {0}
    where ID = {1}
    union all
    select T.ID, T.PARENTID, T.OKATO
    from {0} T, TERRITORY_HIERARCHY H
    where T.PARENTID = H.ID
)
select * from TERRITORY_HIERARCHY
", clsTerritory.FullDBName, startID);

            }
            return string.Empty;
        }

        // формирует кэш территорий, выбирая все подчиненные записи для территории,
        // указанной в параметре закачки "Субъект РФ" (переменная refTerritoryRF)
        // ключ кэша - ОКАТО территории, значение - ID записи с этой территорией
        private void FillTerritoryCache()
        {
            cacheTerritory = new Dictionary<string, int>();

            string query = GetTerritoryHeirarchyQuery(refTerritoryRF);
            using (DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { }))
            {
                foreach (DataRow row in dt.Rows)
                {
                    // в поле ОКАТО может быть указано несколько ОКАТО через запятую
                    string[] okatos = Convert.ToString(row["OKATO"]).Split(',');
                    foreach (string okato in okatos)
                    {
                        if (!cacheTerritory.ContainsKey(okato))
                            cacheTerritory.Add(okato, Convert.ToInt32(row["ID"]));
                    }
                }
            }
        }

        private void FillPeriodCache()
        {
            string query = string.Format("select * from {0}", fxPeriod.FullDBName);
            DataTable dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            foreach (DataRow row in dt.Rows)
            {
                int date = Convert.ToInt32(row["ID"]);
                if (!cachePeriod.Contains(date))
                    cachePeriod.Add(date);
            }
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheOKDP, dsOKDP.Tables[0], "CodeStr");
            FillRowsCache(ref cacheOKSM, dsOKSM.Tables[0], "CountryCode");
            FillRowsCache(ref cacheBudgets, dsBudgets.Tables[0], new string[] { "CodeBudget", "Name" }, "|", "ID");
            FillRowsCache(ref cacheExtrabudget, dsExtrabudget.Tables[0], new string[] { "CodeExBudget", "Name" }, "|", "ID");
            FillTerritoryCache();
            FillOrgCaches();

            FillRowsCache(ref cacheLevelOrder, dsLevelOrder.Tables[0], "Code");
            FillRowsCache(ref cacheModification, dsModification.Tables[0], "CodeOOS");
            FillRowsCache(ref cacheContractStage, dsContractStage.Tables[0], "CodeOOS");
            FillRowsCache(ref cacheOrgForm, dsOrgForm.Tables[0], "CodeOOS");
            FillRowsCache(ref cachePartType, dsPartType.Tables[0], "CodeOOS");
            FillRowsCache(ref cacheStatusSup, dsStatusSup.Tables[0], "CodeOOS");
            FillPeriodCache();
        }

        private DateTime GetMaxDateLoading(IEntity cls, string constraint)
        {
            string query = string.Format(" select max(DateLoading) from {0} ", cls.FullDBName);
            if (constraint != string.Empty)
                query = string.Format("{0} where {1}", query, constraint);
            object result = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((result == null) || (result == DBNull.Value))
                return new DateTime();
            return Convert.ToDateTime(result);
        }

        private void SetDateLoading()
        {
            dateLoadingOKDP = GetMaxDateLoading(clsOKDP, string.Empty);
            dateLoadingOKSM = GetMaxDateLoading(clsOKSM, string.Empty);
            dateLoadingOrg = GetMaxDateLoading(clsOrg, string.Empty);
        }

        private int GetMaxCode(IEntity cls, string constraint)
        {
            string query = string.Format("select max(Code) from {0}", cls.FullDBName);
            if (constraint != string.Empty)
                query = string.Format("{0} where {1}", query, constraint);
            object result = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((result == null) || (result == DBNull.Value))
                return 0;
            return Convert.ToInt32(result);
        }

        private void SetMaxCodes()
        {
            maxOrgCode = GetMaxCode(clsOrg, string.Empty);
            maxBudgetsCode = GetMaxCode(clsBudgets, string.Empty);
            maxExtrabudgetCode = GetMaxCode(clsExtrabudget, string.Empty);
        }

        protected override void QueryData()
        {
            InitDataSet(ref daOKDP, ref dsOKDP, clsOKDP, string.Empty);
            InitDataSet(ref daOKSM, ref dsOKSM, clsOKSM, string.Empty);
            InitDataSet(ref daOrg, ref dsOrg, clsOrg, string.Empty);
            InitDataSet(ref daBudgets, ref dsBudgets, clsBudgets, string.Empty);
            InitDataSet(ref daExtrabudget, ref dsExtrabudget, clsExtrabudget, string.Empty);

            InitDataSet(ref daLevelOrder, ref dsLevelOrder, fxLevelOrder, string.Empty);
            InitDataSet(ref daModification, ref dsModification, fxModification, string.Empty);
            InitDataSet(ref daContractStage, ref dsContractStage, fxContractStage, string.Empty);
            InitDataSet(ref daOrgForm, ref dsOrgForm, fxOrgForm, string.Empty);
            InitDataSet(ref daPartType, ref dsPartType, fxPartType, string.Empty);
            InitDataSet(ref daStatusSup, ref dsStatusSup, fxStatusSup, string.Empty);

            FillCaches();
            SetDateLoading();
            SetMaxCodes();
        }

        #region GUIDs

        private const string D_TERRITORY_RF_GUID = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string D_OOS_OKDP_GUID = "923f00d7-1ae7-44c4-b0b7-7f4f19f73a77";
        private const string D_OOS_OKSM_GUID = "f4ecce21-a700-4b97-8c2c-b6d9baed5cf5";
        private const string D_OOS_ORG_GUID = "c13967d7-45c8-4208-8221-63759f2d859c";
        private const string D_OOS_BUDGETS_GUID = "8858544e-b9ce-4dfd-8f04-0c70a0be8457";
        private const string D_OOS_EXTRABUDGET_GUID = "0415aa89-4b23-405d-ace6-2ab9cc462131";

        private const string D_OOS_PURCHASE_GUID = "7c5240f5-e864-415a-ae6a-ab73ffbdaec1";
        private const string D_OOS_SUBJECT_GUID = "8a8dc059-e646-4ff9-8ca9-df683c881aa1";
        private const string D_OOS_CONTRACTS_GUID = "67906535-e8b2-41d0-8246-bfa1cfc11dfa";
        private const string D_OOS_PRODUCTS_GUID = "ada85f14-66cb-452f-ab53-a7a1d0673b13";
        private const string D_OOS_SUPPLIERS_GUID = "ee0c7f1d-04b9-4f8e-9764-05f3c4c981db";

        private const string FX_OOS_LEVEL_ORDER_GUID = "b48d6364-4bdb-47e6-8979-e67b6a4ae18c";
        private const string FX_OOS_MODIFICATION_GUID = "6252cfbd-2cc6-47bc-80d9-92782e6a993a";
        private const string FX_OOS_CONTRACT_STAGE_GUID = "be1b4fb7-9e57-4d37-a9c2-f822dc4559e2";
        private const string FX_OOS_ORG_FORM_GUID = "e6600264-7e6b-4f25-8645-c843e6b95079";
        private const string FX_OOS_PART_TYPE_GUID = "396a937a-0950-443f-a2b0-45a1fd8884f9";
        private const string FX_OOS_STATUS_SUP_GUID = "7920d3dc-6f9a-46c4-a736-8270a7f7cd2b";
        private const string FX_DATE_YEAR_DAY_UNV_GUID = "b4612528-0e51-4e6b-8891-64c22611816b";

        private const string F_OOS_VALUE_GUID = "51214903-480d-4869-bd6c-8ee93824cc40";
        private const string F_OOS_PRODUCT_GUID = "28f50914-e848-467c-9b45-8b5510a90178";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsTerritory = this.Scheme.Classifiers[D_TERRITORY_RF_GUID];
            clsOKDP = this.Scheme.Classifiers[D_OOS_OKDP_GUID];
            clsOKSM = this.Scheme.Classifiers[D_OOS_OKSM_GUID];
            clsOrg = this.Scheme.Classifiers[D_OOS_ORG_GUID];
            clsBudgets = this.Scheme.Classifiers[D_OOS_BUDGETS_GUID];
            clsExtrabudget = this.Scheme.Classifiers[D_OOS_EXTRABUDGET_GUID];

            clsPurchase = this.Scheme.Classifiers[D_OOS_PURCHASE_GUID];
            clsSubject = this.Scheme.Classifiers[D_OOS_SUBJECT_GUID];
            clsContracts = this.Scheme.Classifiers[D_OOS_CONTRACTS_GUID];
            clsProducts = this.Scheme.Classifiers[D_OOS_PRODUCTS_GUID];
            clsSuppliers = this.Scheme.Classifiers[D_OOS_SUPPLIERS_GUID];

            fxLevelOrder = this.Scheme.Classifiers[FX_OOS_LEVEL_ORDER_GUID];
            fxModification = this.Scheme.Classifiers[FX_OOS_MODIFICATION_GUID];
            fxContractStage = this.Scheme.Classifiers[FX_OOS_CONTRACT_STAGE_GUID];
            fxOrgForm = this.Scheme.Classifiers[FX_OOS_ORG_FORM_GUID];
            fxPartType = this.Scheme.Classifiers[FX_OOS_PART_TYPE_GUID];
            fxStatusSup = this.Scheme.Classifiers[FX_OOS_STATUS_SUP_GUID];
            fxPeriod = this.Scheme.Classifiers[FX_DATE_YEAR_DAY_UNV_GUID];

            fctOOSValue = this.Scheme.FactTables[F_OOS_VALUE_GUID];
            fctOOSProduct = this.Scheme.FactTables[F_OOS_PRODUCT_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOKDP, dsOKDP, clsOKDP);
            UpdateDataSet(daOKSM, dsOKSM, clsOKSM);
            UpdateDataSet(daOrg, dsOrg, clsOrg);
            UpdateDataSet(daBudgets, dsBudgets, clsBudgets);
            UpdateDataSet(daExtrabudget, dsExtrabudget, clsExtrabudget);

            UpdateDataSet(daSubject, dsSubject, clsSubject);
            UpdateDataSet(daPurchase, dsPurchase, clsPurchase);
            UpdateDataSet(daProducts, dsProducts, clsProducts);
            UpdateDataSet(daSuppliers, dsSuppliers, clsSuppliers);
            UpdateDataSet(daContracts, dsContracts, clsContracts);

            UpdateDataSet(daOOSValue, dsOOSValue, fctOOSValue);
            UpdateDataSet(daOOSProduct, dsOOSProduct, fctOOSProduct);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsOOSValue);
            ClearDataSet(ref dsOOSProduct);

            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsOKDP);
            ClearDataSet(ref dsOKSM);
            ClearDataSet(ref dsOrg);
            ClearDataSet(ref dsBudgets);
            ClearDataSet(ref dsExtrabudget);

            ClearDataSet(ref dsPurchase);
            ClearDataSet(ref dsSubject);
            ClearDataSet(ref dsContracts);
            ClearDataSet(ref dsProducts);
            ClearDataSet(ref dsSuppliers);

            ClearDataSet(ref dsLevelOrder);
            ClearDataSet(ref dsModification);
            ClearDataSet(ref dsContractStage);
            ClearDataSet(ref dsOrgForm);
            ClearDataSet(ref dsPartType);
            ClearDataSet(ref dsStatusSup);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xml

        #region Общие методы

        private string GetXmlTagValue(XmlNode node, string xpath)
        {
            XmlNode tag = node.SelectSingleNode(xpath, nsmgr);
            if (tag == null)
                return string.Empty;
            return tag.InnerText.Trim();
        }

        private decimal CleanFactValue(string value)
        {
            decimal factValue = 0;
            if (!Decimal.TryParse(value.Replace('.', ',').Trim().PadLeft(1, '0'), out factValue))
                WriteToTrace(string.Format("Обнаружено некорректное значение показателя {0}. Заменено на 0.", value),
                    TraceMessageKind.Information);
            return factValue;
        }

        // преобразует дату из формата YYYY-MM-DDTHH:MM:SSZ в формат YYYYMMDD
        private int ConvertOOSDateBig(string value, string errorMsg)
        {
            if (value.Length < 10)
                return -1;
            string date = value.Trim().Substring(0, 10);
            return ConvertOOSDateSmall(date, errorMsg);
        }

        // преобразует дату из формата YYYY-MM-DD в формат YYYYMMDD
        private int ConvertOOSDateSmall(string value, string errorMsg)
        {
            if (value.Length < 10)
                return -1;

            string date = value.Trim().Replace("-", string.Empty);
            if (date == string.Empty)
                return -1;

            int refDate = Convert.ToInt32(date);
            if (!cachePeriod.Contains(refDate))
            {
                WriteToTrace(string.Format(
                    "{0} проставлено значение -1, поскольку в исходном файле некорректная дата = \"{1}\"",
                    errorMsg, value), TraceMessageKind.Warning);
                return -1;
            }

            return refDate;
        }

        // преобразует дату из значений года и месяца в формат YYYYMMDD
        private int ConvertOOSDateYearMonth(string year, string month)
        {
            if (year == string.Empty)
                return -1;
            int refDate = Convert.ToInt32(year) * 10000 + Convert.ToInt32(month.PadLeft(1, '0')) * 100;
            if (!cachePeriod.Contains(refDate))
                return -1;
            return refDate;
        }

        // преобразует дату из формата YYYY-MM-DD в формат DD.MM.YYYY
        private string ConvertOOSDate(string value)
        {
            string[] date = value.Split('-');
            if (date.Length != 3)
                return value;
            return string.Format("{0}.{1}.{2}", date[2], date[1], date[0]);
        }

        #endregion

        #region Классификатор ОКДП

        private void PumpXmlOKDP(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                string code = GetXmlTagValue(node, "oos:code");
                string name = GetXmlTagValue(node, "oos:name");

                PumpCachedRow(cacheOKDP, dsOKDP.Tables[0], clsOKDP, code, new object[] {
                    "CodeStr", code, "Name", name, "DateLoading", reportDate });
                if (dsOKDP.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateDataSet(daOKDP, dsOKDP, clsOKDP);
                    ClearDataSet(daOKDP, dsOKDP.Tables[0]);
                }
            }
        }

        #endregion

        #region Классификатор ОКСМ

        private void PumpXmlOksm(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                int code = Convert.ToInt32(GetXmlTagValue(node, "oos:countryCode"));
                string name = GetXmlTagValue(node, "oos:countryFullName");

                PumpCachedRow(cacheOKSM, dsOKSM.Tables[0], clsOKDP, code.ToString(), new object[] {
                    "CountryCode", code, "CountryFullName", name, "DateLoading", reportDate });
                if (dsOKSM.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateDataSet(daOKSM, dsOKSM, clsOKSM);
                    ClearDataSet(daOKSM, dsOKSM.Tables[0]);
                }
            }
        }

        #endregion

        #region Классификатор Организации

        private void PumpXmlOrganization(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                string okato = GetXmlTagValue(node, "oos:factualAddress/oos:OKATO");
                if (!cacheTerritory.ContainsKey(okato))
                    continue;
                int refTerritory = cacheTerritory[okato];

                long regNumber = Convert.ToInt64(GetXmlTagValue(node, "oos:regNumber"));
                if (cacheOrg.ContainsKey(regNumber.ToString()))
                    continue;

                maxOrgCode++;
                string fullName = GetXmlTagValue(node, "oos:fullName");

                int countryCode = 0;
                string countryCodeStr = GetXmlTagValue(node, "oos:factualAddress/oos:country/oos:countryCode");
                if (!Int32.TryParse(countryCodeStr, out countryCode))
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Для организации {0} '{1}' не установлен или указан некорректный код страны ({2}). Будет проставлено значение по умолчанию.",
                        regNumber, fullName, countryCodeStr));
                }

                int subordinationType = 0;
                string subordinationTypeStr = GetXmlTagValue(node, "oos:subordinationType/oos:id");
                if (!Int32.TryParse(subordinationTypeStr, out subordinationType))
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "Для организации {0} '{1}' не установлен или указан некорректный уровень организации ({2}). Будет проставлено значение по умолчанию.",
                        regNumber, fullName, subordinationTypeStr));
                }

                object[] mapping = new object[] {
                    "Code", maxOrgCode,
                    "RegNumber", regNumber,
                    "FullName", fullName,
                    "ShortName", GetXmlTagValue(node, "oos:shortName"),
                    // "OKATO", okato,
                    "PostalAddress", GetXmlTagValue(node, "oos:factualAddress/oos:addressLine"),
                    "Phone", GetXmlTagValue(node, "oos:phone"),
                    "Email", GetXmlTagValue(node, "oos:email"),
                    "Fax", GetXmlTagValue(node, "oos:fax"),
                    "ContactPerson", string.Format("{0} {1} {2}",
                        GetXmlTagValue(node, "oos:contactPerson/oos:lastName"),
                        GetXmlTagValue(node, "oos:contactPerson/oos:firstName"),
                        GetXmlTagValue(node, "oos:contactPerson/oos:middleName")),
                    "INN", GetXmlTagValue(node, "oos:inn"),
                    "KPP", GetXmlTagValue(node, "oos:kpp"),
                    "SubordinationType", subordinationType,
                    "DateLoading", reportDate,
                    "RefOKSM", FindCachedRow(cacheOKSM, countryCode.ToString(), nullOKSM),
                    "RefTerritory", refTerritory
                };

                if (!cacheOrgInfo.ContainsKey(regNumber.ToString()))
                {
                    cacheOrgInfo.Add(regNumber.ToString(), string.Format("{0}|{1}", subordinationType, refTerritory));
                }
                PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, regNumber.ToString(), mapping);
                if (dsOrg.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                {
                    UpdateDataSet(daOrg, dsOrg, clsOrg);
                    ClearDataSet(daOrg, dsOrg.Tables[0]);
                }
            }
        }

        #endregion

        #region Классификатор Закупки

        private int GetRefGiveDate(XmlNode node, NotificationType notification)
        {
            string errorMsg = string.Format("В таблице {0} для закупки с номером {1} в поле RefGiveDate",
                clsPurchase.FullDBName, GetXmlTagValue(node, "oos:notificationNumber"));
            switch (notification)
            {
                case NotificationType.OK:
                case NotificationType.EF:
                case NotificationType.SZ:
                    return ConvertOOSDateBig(GetXmlTagValue(node, "oos:notificationCommission/oos:p1Date"), errorMsg);
                case NotificationType.ZK:
                case NotificationType.PO:
                    return ConvertOOSDateBig(GetXmlTagValue(node, "oos:notificationCommission/oos:p2Date"), errorMsg);
            }
            return -1;
        }

        private int GetRefConsiderDate(XmlNode node, NotificationType notification)
        {
            string errorMsg = string.Format("В таблице {0} для закупки с номером {1} в поле RefConsiderDate",
                clsPurchase.FullDBName, GetXmlTagValue(node, "oos:notificationNumber"));
            switch (notification)
            {
                case NotificationType.EF:
                    return ConvertOOSDateBig(GetXmlTagValue(node, "oos:notificationCommission/oos:p3Date"), errorMsg);
            }
            return -1;
        }

        private int GetRefMatchDate(XmlNode node, NotificationType notification)
        {
            string errorMsg = string.Format("В таблице {0} для закупки с номером {1} в поле RefMatchDate",
                clsPurchase.FullDBName, GetXmlTagValue(node, "oos:notificationNumber"));
            switch (notification)
            {
                case NotificationType.EF:
                    return ConvertOOSDateBig(GetXmlTagValue(node, "oos:notificationCommission/oos:p3Date"), errorMsg);
            }
            return -1;
        }

        private int GetRefResultDate(XmlNode node, NotificationType notification)
        {
            string errorMsg = string.Format("В таблице {0} для закупки с номером {1} в поле RefResultDate",
               clsPurchase.FullDBName, GetXmlTagValue(node, "oos:notificationNumber"));
            switch (notification)
            {
                case NotificationType.OK:
                    return ConvertOOSDateBig(GetXmlTagValue(node, "oos:notificationCommission/oos:p3Date"), errorMsg);
            }
            return -1;
        }

        private int GetRefPurchTyp(NotificationType notification)
        {
            switch (notification)
            {
                case NotificationType.OK:
                    return 1;
                case NotificationType.EF:
                    return 3;
                case NotificationType.ZK:
                    return 4;
                case NotificationType.PO:
                    return 5;
                case NotificationType.SZ:
                    return 8;
            }
            return -1;
        }

        private int PumpXmlPurchase(XmlNode node, NotificationType notification)
        {
            string notificationNumber = GetXmlTagValue(node, "oos:notificationNumber");
            long regNum = Convert.ToInt64(GetXmlTagValue(node, "oos:order/oos:placer/oos:regNum"));

            string errorMsg = string.Format("В таблице {0} для закупки с номером {1} в поле ", clsPurchase.FullDBName, notificationNumber);
            object[] mapping = new object[] {
                "NotificationNum", notificationNumber,
                "Name", GetXmlTagValue(node, "oos:orderName"),
                "PrintForm", GetXmlTagValue(node, "oos:printForm/oos:url"),
                "Link", GetXmlTagValue(node, "oos:href"),
                "RefOrganizer", FindCachedRow(cacheOrg, regNum.ToString(), nullOrg),
                "RefCreateDate", ConvertOOSDateBig(GetXmlTagValue(node, "oos:createDate"), errorMsg + "RefCreateDate"),
                "RefPublicDate", ConvertOOSDateBig(GetXmlTagValue(node, "oos:publishDate"), errorMsg + "RefPublicDate"),
                "RefGiveDate", GetRefGiveDate(node, notification),
                "RefConsiderDate", GetRefConsiderDate(node, notification),
                "RefMatchDate", GetRefMatchDate(node, notification),
                "RefResultDate", GetRefResultDate(node, notification),
                "RefTypePurch", GetRefPurchTyp(notification),
                "DateLoading", reportDate,
                "SourceID", sourceID
            };

            if (!cachePurchase.ContainsKey(notificationNumber))
            {
                maxPurchaseCode++;
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "Code", maxPurchaseCode });
            }

            return RepumpCachedRow(cachePurchase, dsPurchase.Tables[0], clsPurchase, notificationNumber, mapping);
        }

        #endregion

        #region Классификатор Предметы контракта

        private int PumpXmlSubject(XmlNode node)
        {
            string sid = GetXmlTagValue(node, "oos:sid");

            object[] mapping = new object[] {
                "Sid", sid,
                "CodeStr", GetXmlTagValue(node, "oos:ordinalNumber"),
                "Name", GetXmlTagValue(node, "oos:subject"),
                "DateLoading", reportDate,
                "SourceID", sourceID
            };

            return RepumpCachedRow(cacheSubject, dsSubject.Tables[0], clsSubject, sid, mapping);
        }

        #endregion

        #region Данные по закупкам

        private int GetRefOrg(XmlNode customerRequirement)
        {
            string regNumber = Convert.ToInt64(GetXmlTagValue(customerRequirement, "oos:customer/oos:regNum")).ToString();
            if (cacheOrg.ContainsKey(regNumber))
                return cacheOrg[regNumber];

            maxOrgCode++;
            object[] mapping = new object[] {
                "Code", maxOrgCode,
                "RegNumber", regNumber,
                "FullName", GetXmlTagValue(customerRequirement, "oos:customer/oos:fullName"),
                "ShortName", "Значение не указано",
                "PostalAddress", "Значение не указано",
                "Phone", DBNull.Value,
                "Email", DBNull.Value,
                "Fax", DBNull.Value,
                "ContactPerson", DBNull.Value,
                "INN", 0,
                "KPP", 0,
                "SubordinationType", 0,
                "DateLoading", DBNull.Value,
                "RefOKSM", nullOKSM,
                "RefTerritory", nullTerritory
            };

            int refOrg = PumpCachedRow(cacheOrg, dsOrg.Tables[0], clsOrg, regNumber, mapping);

            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                "Не найден заказчик id = {0}. Возможно, у него указан не правильный ОКАТО. " +
                "Отформатируйте данные по этой организации.", refOrg));

            return refOrg;
        }

        private void PumpXmlNotification(XmlNodeList nodes, NotificationType notification)
        {
            long purchaseNumber = 0;
            foreach (XmlNode node in nodes)
            {
                int refPurchase = PumpXmlPurchase(node, notification);

                long regNum = Convert.ToInt64(GetXmlTagValue(node, "oos:order/oos:placer/oos:regNum"));
                int refTerritory = nullTerritory;
                int refLevelOrder = nullLevelOrder;
                if (cacheOrgInfo.ContainsKey(regNum.ToString()))
                {
                    string[] value = cacheOrgInfo[regNum.ToString()].Split('|');
                    refLevelOrder = FindCachedRow(cacheLevelOrder, Convert.ToInt32(value[0]), nullLevelOrder);
                    refTerritory = Convert.ToInt32(value[1]);
                }

                XmlNodeList lots = node.SelectNodes("oos:lots/oos:lot", nsmgr);
                foreach (XmlNode lot in lots)
                {
                    int refSubject = PumpXmlSubject(lot);

                    #region comment
                    // в таблице фактов заполняем вспомогательное поле Auxiliary,
                    // в которое заносится количество тактов текущего времени
                    // это позволит идентифицировать неактуальные данные (те записи, у которых Auxiliary меньше максимального)
                    // и удалить их (см. метод DeleteObsoleteData)
                    // т.к. за один такт может добавиться сразу несколько строк,
                    // то для пущей уникальности будем приписывать номер текущей закупки purchaseNumber
                    //
                    // P.S.: всё это можно было бы сделать и по полю ID (чем больше ID, тем актуальнее запись),
                    // но проблема в том, что у одной закупки может быть несколько сумм в таблице фактов -
                    // они обе являются актуальными и должны остаться
                    #endregion
                    purchaseNumber++;
                    string auxiliary = string.Concat(DateTime.Now.Ticks.ToString().PadLeft(30, '0'), purchaseNumber.ToString().PadLeft(10, '0'));
                    XmlNodeList customerRequirements = lot.SelectNodes("oos:customerRequirements/oos:customerRequirement", nsmgr);
                    foreach (XmlNode customerRequirement in customerRequirements)
                    {
                        object[] mapping = new object[] {
                            "PMP", CleanFactValue(GetXmlTagValue(customerRequirement, "oos:maxPrice")),
                            "RefOrg", GetRefOrg(customerRequirement),
                            "RefSubject", refSubject,
                            "RefPurchase", refPurchase,
                            "RefLevelOrder", refLevelOrder,
                            "RefTerritory", refTerritory,
                            "RefSupplier", nullSuppliers,
                            "RefContract", nullContracts,
                            "Auxiliary", auxiliary,
                            "SourceID", sourceID
                        };

                        PumpRow(dsOOSValue.Tables[0], mapping);
                        if (dsOOSValue.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                        {
                            UpdateData();
                            ClearDataSet(daOOSValue, ref dsOOSValue);
                        }
                    }
                }
            }
        }

        #endregion

        #region Классификатор Бюджеты финансирования

        private int PumpXmlBudgets(XmlNode node)
        {
            XmlNode budget = node.SelectSingleNode("oos:finances/oos:budget", nsmgr);
            if (budget == null)
                return nullBudgets;

            string codeBudget = GetXmlTagValue(budget, "oos:code");
            string name = GetXmlTagValue(budget, "oos:name");

            string key = string.Format("{0}|{1}", codeBudget, name);
            if (cacheBudgets.ContainsKey(key))
                return cacheBudgets[key];

            maxBudgetsCode++;

            object[] mapping = new object[] {
                "Code", maxBudgetsCode,
                "CodeBudget", codeBudget,
                "Name", name,
                "DateLoading", reportDate
            };

            return PumpCachedRow(cacheBudgets, dsBudgets.Tables[0], clsBudgets, key, mapping);
        }

        #endregion

        #region Классификатор Виды внебюджетных средств

        private int PumpXmlExtrabudget(XmlNode node)
        {
            XmlNode extrabudget = node.SelectSingleNode("oos:finances/oos:extrabudget", nsmgr);
            if (extrabudget == null)
                return nullExtrabudget;

            string codeBudget = GetXmlTagValue(extrabudget, "oos:code");
            string name = GetXmlTagValue(extrabudget, "oos:name");

            string key = string.Format("{0}|{1}", codeBudget, name);
            if (cacheExtrabudget.ContainsKey(key))
                return cacheExtrabudget[key];

            maxExtrabudgetCode++;

            object[] mapping = new object[] {
                "Code", maxExtrabudgetCode,
                "CodeExBudget", codeBudget,
                "Name", name,
                "DateLoading", reportDate
            };

            return PumpCachedRow(cacheExtrabudget, dsExtrabudget.Tables[0], clsExtrabudget, key, mapping);
        }

        #endregion

        #region Классификатор Контракты

        private int GetRefTypePurch(XmlNode node)
        {
            XmlNode placing = node.SelectSingleNode("oos:foundation/oos:order/oos:placing", nsmgr);
            if (placing == null)
                placing = node.SelectSingleNode("oos:foundation/oos:other/oos:placing", nsmgr);
            if (placing == null)
                return 6;
            string typePurch = placing.InnerText.Trim();
            if (typePurch == string.Empty)
                return -1;
            return Convert.ToInt32(typePurch);
        }

        // получаем значение служебного поля для закачки
        // оно содержит информацию для заполнения инфы о контракте в таблице фактов ООС_АС ООС_Данные (см. метод FillContractsInfo)
        private const string NO_NOTIFICATION_MARK = "N";
        private string GetContractsAuxiliary(XmlNode node, int refSuppliers, int refOrg, long orgNum)
        {
            string notificationNumber = GetXmlTagValue(node, "oos:foundation/oos:order/oos:notificationNumber");
            if (notificationNumber != string.Empty)
            {
                // если номер закупки указан, то в служебное поле записываем
                // 1) ссылку на закупку
                // 2) ссылку на заказчика
                // 3) номер лота
                // 4) ссылку на поставщика
                // 5) цену контракта

                int refPurchase = -1;
                if (cachePurchase.ContainsKey(notificationNumber))
                    refPurchase = Convert.ToInt32(cachePurchase[notificationNumber]["ID"]);

                return string.Format("{0}|{1}|{2}|{3}|{4}",
                    refPurchase,
                    refOrg,
                    GetXmlTagValue(node, "oos:foundation/oos:order/oos:lotNumber"),
                    refSuppliers,
                    GetXmlTagValue(node, "oos:price"));
            }
            else
            {
                // если номер закупки не указан, то в служебное поле записываем
                // 1) метка, указывающая, что закупка не опрделена
                // 2) ссылку на заказчика
                // 3) ссылку на территорию
                // 4) ссылку на уровень бюджета
                // 5) ссылку на поставщика
                // 6) цену контракта

                int refTerritory = nullTerritory;
                int refLevelOrder = nullLevelOrder;
                if (cacheOrgInfo.ContainsKey(orgNum.ToString()))
                {
                    string[] value = cacheOrgInfo[orgNum.ToString()].Split('|');
                    refLevelOrder = FindCachedRow(cacheLevelOrder, Convert.ToInt32(value[0]), nullLevelOrder);
                    refTerritory = Convert.ToInt32(value[1]);
                }

                return string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                    NO_NOTIFICATION_MARK,
                    refOrg,
                    refTerritory,
                    refLevelOrder,
                    refSuppliers,
                    GetXmlTagValue(node, "oos:price"));
            }
        }

        private int PumpXmlContracts(XmlNode node, int refSuppliers, int refOrg, long orgNum)
        {
            string regNum = GetXmlTagValue(node, "oos:regNum");
            string errorMsg = string.Format("В таблице {0} для контракта с номером {1} в поле ", clsContracts.FullDBName, regNum);

            object[] mapping = new object[] {
                "IDOOS", GetXmlTagValue(node, "oos:id"),
                "RegNum", regNum,
                "RegNum807", GetXmlTagValue(node, "oos:regNum807"),
                "ContractNumber", GetXmlTagValue(node, "oos:number"),
                "VersionNumber", Convert.ToInt32(GetXmlTagValue(node, "oos:versionNumber").PadLeft(1, '0')),
                "DocumentBase", GetXmlTagValue(node, "oos:documentBase"),
                "Currency", GetXmlTagValue(node, "oos:currency/oos:name"),
                "FinSource", GetXmlTagValue(node, "oos:finances/oos:financeSource"),
                "PrintForm", GetXmlTagValue(node, "oos:printForm/oos:url"),
                "LinkContract", GetXmlTagValue(node, "oos:href"),
                "Name", string.Format("Контракт № {0} от {1} ({2})", GetXmlTagValue(node, "oos:number"), ConvertOOSDate(GetXmlTagValue(node, "oos:signDate")), regNum),
                "RefPublishDate", ConvertOOSDateBig(GetXmlTagValue(node, "oos:publishDate"), errorMsg + "RefPublishDate"),
                "RefSignDate", ConvertOOSDateSmall(GetXmlTagValue(node, "oos:signDate"), errorMsg + "RefSignDate"),
                "RefProtocolDate", ConvertOOSDateSmall(GetXmlTagValue(node, "oos:protocolDate"), errorMsg + "RefProtocolDate"),
                "RefExecution", ConvertOOSDateYearMonth(GetXmlTagValue(node, "oos:execution/oos:year"), GetXmlTagValue(node, "oos:execution/oos:month")),
                "RefModification", FindCachedRow(cacheModification, GetXmlTagValue(node, "oos:modification/oos:type"), nullModification),
                "RefStage", FindCachedRow(cacheContractStage, GetXmlTagValue(node, "oos:currentContractStage"), nullContractStage),
                "RefTypePurch", GetRefTypePurch(node),
                "RefBudget", PumpXmlBudgets(node),
                "RefExtrabudget", PumpXmlExtrabudget(node),
                "Auxiliary", GetContractsAuxiliary(node, refSuppliers, refOrg, orgNum),
                "DateLoading", reportDate,
                "SourceID", sourceID
            };

            if (!cacheContracts.ContainsKey(regNum))
            {
                maxContractsCode++;
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "Code", maxContractsCode });
            }

            return RepumpCachedRow(cacheContracts, dsContracts.Tables[0], clsContracts, regNum, mapping);
        }

        #endregion

        #region Классификатор Продукция

        private object GetRefOKDP(XmlNode node)
        {
            object refOkdp = DBNull.Value;
            string okdpCode = GetXmlTagValue(node, "oos:OKDP/oos:code");
            if (cacheOKDP.ContainsKey(okdpCode))
                refOkdp = cacheOKDP[okdpCode];
            return refOkdp;
        }

        private int PumpXmlProducts(XmlNode node)
        {
            string name = string.Format("{0} ({1})", GetXmlTagValue(node, "oos:name"), GetXmlTagValue(node, "oos:OKEI/oos:name"));
            if (name.Length > 4000)
                name = name.Substring(0, 4000);
            string unit = GetXmlTagValue(node, "oos:OKEI/oos:name");
            object refOKDP = GetRefOKDP(node);

            string key = string.Format("{0}|{1}|{2}", name, unit, refOKDP);
            if (cacheProducts.ContainsKey(key))
                return cacheProducts[key];

            maxProductsCode++;
            object[] mapping = new object[] {
                "Code", maxProductsCode,
                "Name", name,
                "Unit", unit,
                "RefOKDP", refOKDP,
                "DateLoading", reportDate,
                "SourceID", sourceID
            };

            return PumpCachedRow(cacheProducts, dsProducts.Tables[0], clsProducts, key, mapping);
        }

        #endregion

        #region Классификатор Поставщики

        private int PumpXmlSuppliers(XmlNode node)
        {
            string contactPerson = string.Format("{0} {1} {2}",
                GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:contactInfo/oos:lastName"),
                GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:contactInfo/oos:firstName"),
                GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:contactInfo/oos:middleName")).Trim();
            string name = GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:organizationName");
            if (name == string.Empty)
            {
                if (contactPerson != string.Empty)
                    name = contactPerson;
                else
                    name = "Неуказанная организация";
            }

            long inn = Convert.ToInt64(GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:inn").PadLeft(1, '0'));
            long kpp = Convert.ToInt64(GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:kpp").PadLeft(1, '0'));
            int refOrgForm = FindCachedRow(cacheOrgForm, GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:organizationForm"), nullOrgForm);

            string key = string.Format("{0}|{1}|{2}|{3}", name, inn, kpp, refOrgForm);
            if (cacheSuppliers.ContainsKey(key))
                return cacheSuppliers[key];

            maxSuppliersCode++;

            object[] mapping = new object[] {
                "Code", maxSuppliersCode,
                "Name", name,
                "INN", inn,
                "KPP", kpp,
                "IDNumber", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:idNumber"),
                "IDNumberEx", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:idNumberExtension"),
                "FactualAddress", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:factualAddress"),
                "PostAddress", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:postAddress"),
                "ContactPerson", contactPerson,
                "ContactEmail", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:contactEMail"),
                "ContactPhone", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:contactPhone"),
                "ContactFax", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:contactFax"),
                "AddInfo", GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:additionalInfo"),
                "RefFormOrg", refOrgForm,
                "RefPartType", FindCachedRow(cachePartType, GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:participantType"), nullPartType),
                "RefStatusSup", FindCachedRow(cacheStatusSup, GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:status"), nullStatusSup),
                "RefOKSM", FindCachedRow(cacheOKSM, GetXmlTagValue(node, "oos:suppliers/oos:supplier/oos:country/oos:countryCode"), nullOKSM),
                "DateLoading", reportDate,
                "SourceID", sourceID
            };

            return PumpCachedRow(cacheSuppliers, dsSuppliers.Tables[0], clsSuppliers, key, mapping);
        }

        #endregion

        #region Данные по контрактам

        private void PumpXmlContractsData(XmlNodeList nodes)
        {
            long contractNumber = 0;
            foreach (XmlNode node in nodes)
            {
                long orgNum = Convert.ToInt64(GetXmlTagValue(node, "oos:customer/oos:regNum").PadLeft(1, '0'));
                int refOrg = FindCachedRow(cacheOrg, orgNum.ToString(), nullOrg);
                int refSuppliers = PumpXmlSuppliers(node);
                int refContracts = PumpXmlContracts(node, refSuppliers, refOrg, orgNum);

                #region comment
                // в таблице фактов заполняем вспомогательное поле Auxiliary,
                // в которое заносится количество тактов текущего времени
                // это позволит идентифицировать неактуальные данные (те записи, у которых Auxiliary меньше максимального)
                // и удалить их (см. метод DeleteObsoleteData)
                // т.к. за один такт может добавиться сразу несколько строк,
                // то для пущей уникальности будем приписывать номер текущего контракта contractNumber
                //
                // P.S.: всё это можно было бы сделать и по полю ID (чем больше ID, тем актуальнее запись),
                // но проблема в том, что у одного контракта может быть несколько сумм в таблице фактов -
                // они обе являются актуальными и должны остаться
                #endregion
                contractNumber++;
                string auxiliary = string.Concat(DateTime.Now.Ticks.ToString().PadLeft(30, '0'), contractNumber.ToString().PadLeft(10, '0'));
                XmlNodeList products = node.SelectNodes("oos:products/oos:product", nsmgr);
                foreach (XmlNode product in products)
                {
                    object[] mapping = new object[] {
                        "Price", CleanFactValue(GetXmlTagValue(product, "oos:price")),
                        "Quantity", CleanFactValue(GetXmlTagValue(product, "oos:quantity")),
                        "Sum", CleanFactValue(GetXmlTagValue(product, "oos:sum")),
                        "RefContract", refContracts,
                        "RefSupplier", refSuppliers,
                        "RefProduct", PumpXmlProducts(product),
                        "RefOrg", refOrg,
                        "Auxiliary", auxiliary,
                        "SourceID", sourceID
                    };

                    PumpRow(dsOOSProduct.Tables[0], mapping);
                    if (dsOOSProduct.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateData();
                        ClearDataSet(daOOSProduct, ref dsOOSProduct);
                    }
                }
            }
        }

        #endregion

        #region XPath
        private const string XPATH_PRODUCT = "oos:nsiProduct";
        private const string XPATH_OKSM = "oos:nsiOksm";
        private const string XPATH_ORGANIZATION = "oos:organization";
        private const string XPATH_NOTIFICATION_OK = "oos:notificationOK";
        private const string XPATH_NOTIFICATION_EF = "oos:notificationEF";
        private const string XPATH_NOTIFICATION_ZK = "oos:notificationZK";
        private const string XPATH_NOTIFICATION_PO = "oos:notificationPO";
        private const string XPATH_NOTIFICATION_SZ = "oos:notificationSZ";
        private const string XPATH_CONTRACT = "oos:contract";
        #endregion
        private void PumpXmlDoc(XmlNode root)
        {
            switch (reportType)
            {
                case ReportType.Product:
                    PumpXmlOKDP(root.SelectNodes(XPATH_PRODUCT, nsmgr));
                    break;
                case ReportType.OKSM:
                    PumpXmlOksm(root.SelectNodes(XPATH_OKSM, nsmgr));
                    break;
                case ReportType.Organization:
                    PumpXmlOrganization(root.SelectNodes(XPATH_ORGANIZATION, nsmgr));
                    break;
                case ReportType.Notifications:
                    PumpXmlNotification(root.SelectNodes(XPATH_NOTIFICATION_OK, nsmgr), NotificationType.OK);
                    PumpXmlNotification(root.SelectNodes(XPATH_NOTIFICATION_EF, nsmgr), NotificationType.EF);
                    PumpXmlNotification(root.SelectNodes(XPATH_NOTIFICATION_ZK, nsmgr), NotificationType.ZK);
                    PumpXmlNotification(root.SelectNodes(XPATH_NOTIFICATION_PO, nsmgr), NotificationType.PO);
                    PumpXmlNotification(root.SelectNodes(XPATH_NOTIFICATION_SZ, nsmgr), NotificationType.SZ);
                    break;
                case ReportType.Contracts:
                    PumpXmlContractsData(root.SelectNodes(XPATH_CONTRACT, nsmgr));
                    break;
            }
        }

        // проверка, будем ли качать файл:
        // качаем только те файлы, данные в которых новее закачанных
        // - дата файла берется из наименования
        // - дата закачанных данных берется из поля DateLoading
        private bool IsPumpFile(DateTime reportDate)
        {
            switch (reportType)
            {
                case ReportType.Product:
                    return (reportDate.CompareTo(dateLoadingOKDP) > 0);
                case ReportType.OKSM:
                    return (reportDate.CompareTo(dateLoadingOKSM) > 0);
                // для организаций не проверяем дату загрузки,
                // т.к. в разное время качаются организации разных регионов
                // и если в первый раз закачали организации для одного региона с максимальной датой,
                // то в следующий для другого региона не закачается ни одной организации
                case ReportType.Organization:
                    return true; // (reportDate.CompareTo(dateLoadingOrg) > 0);
                case ReportType.Notifications:
                    return (reportDate.CompareTo(dateLoadingPurchase) > 0);
                case ReportType.Contracts:
                    return (reportDate.CompareTo(dateLoadingContracts) > 0);
            }
            return false;
        }

        // Получаем дату отчета из имени файла, например:
        //    nsiProduct_all_ГГГГММДД_010001_9.xml.zip (имя файла содержит _all_)
        //    notification__Jamalo-Neneckij_AO_inc_20120116_000000_ГГГГММДД_000000_128.xml.zip (имя файла содержит _inc_)
        private DateTime GetDateTime(string filename)
        {
            string strDate = string.Empty;
            if (filename.ToUpper().Contains("_INC_"))
                strDate = CommonRoutines.TrimLetters(filename).Split('_')[2];
            else
                strDate = CommonRoutines.TrimLetters(filename).Split('_')[0];

            DateTime date = new DateTime(
                Convert.ToInt32(strDate.Substring(0, 4)),
                Convert.ToInt32(strDate.Substring(4, 2)),
                Convert.ToInt32(strDate.Substring(6, 2)));

            return date;
        }

        private void PumpXmlFile(FileInfo file)
        {
            reportDate = GetDateTime(file.Name);
            if (!IsPumpFile(reportDate))
            {
                WriteToTrace(string.Format("Файл '{0}' пропущен, т.к. он уже был закачан.", file.Name), TraceMessageKind.Information);
                return;
            }

            WriteToTrace("Открытие документа: " + file.FullName, TraceMessageKind.Information);
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                // приходится работать с диспетчером пространства имен,
                // но в документе у некоторых тегов (не у всех!) есть свой namespace (oos), который нужно выжечь
                string xml = File.ReadAllText(file.FullName).Replace("oos:", string.Empty);

                // загружаем xml-документ уже без нэймспэйсов
                xmlDoc.LoadXml(xml);
                XmlNode root = xmlDoc.DocumentElement;

                // настраиваем диспетчер нэймспэйсов (его адрес хранится в атрибуте xmlns корневого тега)
                nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("oos", root.Attributes["xmlns"].Value);

                PumpXmlDoc(root);
            }
            finally
            {
                if (xmlDoc != null)
                    XmlHelper.ClearDomDocument(ref xmlDoc);
            }
        }

        #endregion Работа с Xml

        #region Работа с Ftp

        private FtpProxyInfo GetProxyInfo()
        {
            if (proxyServer == string.Empty)
                return null;

            FtpProxyInfo proxy = new FtpProxyInfo();
            proxy.Type = FtpProxyType.HttpConnect;
            proxy.PreAuthenticate = true;
            proxy.Server = proxyServer;
            proxy.Port = Convert.ToInt32(proxyPort.PadLeft(1, '0'));
            proxy.User = proxyUsername;
            proxy.Password = proxyPassword;

            return proxy;
        }

        private bool SkipFtpItem(FtpItem item, string destPath)
        {
            if (item.ItemType != FtpItemType.File)
                return true;

            string pumpedFile = string.Concat(destPath, Path.DirectorySeparatorChar, item.Name);
            // пропускаем уже закачанные файлы
            if (File.Exists(pumpedFile))
            {
                if (new FileInfo(pumpedFile).Length == item.Size)
                    return true;
                File.Delete(pumpedFile);
            }
            return false;
        }

        // выкачивает все файлы, лежащие в папке path
        private void PumpFtpFolder(string path, ReportType type)
        {
            string destPath = string.Concat(tempDir, Path.DirectorySeparatorChar, path);
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);
            pumpFolders.Add(destPath, type);

            string ftpAddress = string.Format("ftp://{0}/{1}/", ftpServer, path);
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, "Старт закачки данных из каталога " + ftpAddress);
            using (FtpClient ftp = new FtpClient())
            {
                ftp.PassiveMode = true;
                ftp.ProxyInfo = GetProxyInfo();
                ftp.Connect(FTP_CONNECT_TIMEOUT, ftpServer, Convert.ToInt32(ftpPort));
                ftp.Login(FTP_CONNECT_TIMEOUT, ftpUsername, ftpPassword);

                FtpItem[] items = ftp.GetDirectoryList(FTP_CONNECT_TIMEOUT, path);
                for (int i = 0; i < items.Length; i++)
                {
                    SetProgress(items.Length, i + 1,
                        string.Format("Загрузка файлов из каталога {0}...", ftpAddress),
                        string.Format("Файл {0} из {1}", i + 1, items.Length));

                    if (SkipFtpItem(items[i], destPath))
                        continue;

                    // WriteToTrace(string.Format("Загрузка файла {0}... ({1}/{2})", items[i].Name, i + 1, items.Length), TraceMessageKind.Information);
                    string pumpedFile = string.Concat(destPath, Path.DirectorySeparatorChar, items[i].Name);
                    ftp.GetFile(FTP_CONNECT_TIMEOUT, pumpedFile, string.Concat(path, Path.AltDirectorySeparatorChar, items[i].Name));
                }

                ftp.Disconnect(FTP_CONNECT_TIMEOUT);
            }
        }

        private List<string> GetFtpFolders()
        {
            List<string> folder = new List<string>();

            using (FtpClient ftp = new FtpClient())
            {
                ftp.PassiveMode = true;
                ftp.ProxyInfo = GetProxyInfo();
                ftp.Connect(FTP_CONNECT_TIMEOUT, ftpServer, Convert.ToInt32(ftpPort));
                ftp.Login(FTP_CONNECT_TIMEOUT, ftpUsername, ftpPassword);

                FtpItem[] items = ftp.GetDirectoryList(FTP_CONNECT_TIMEOUT, null);
                foreach (FtpItem item in items)
                {
                    if (item.ItemType != FtpItemType.Directory)
                        continue;
                    folder.Add(item.Name);
                }

                ftp.Disconnect(FTP_CONNECT_TIMEOUT);
            }

            return folder;
        }

        #region FTP-paths
        private const string FTP_PATH_PRODUCT = "auto/product";
        private const string FTP_PATH_OKSM = "auto/oksm";
        private const string FTP_PATH_ORGANIZATION = "auto/organization/all";
        private const string FTP_PATH_CONTRACTS = "contracts";
        private const string FTP_PATH_CONTRACTS_DAILY = "contracts/daily";
        private const string FTP_PATH_NOTIFICATIONS = "notifications";
        private const string FTP_PATH_NOTIFICATIONS_DAILY = "notifications/daily";
        #endregion
        private const int FTP_CONNECT_TIMEOUT = 10000;
        // сначала выгружаем все файлы с ftp-сервера, а уже потом спокойно закачиваем в базёнку,
        // т.к. во время закачки может оборваться соединение с ftp и закачка закончится на самом интересном месте
        private void PumpAllFilesFromFtp()
        {
            string pumpFolder = string.Empty;
            // данные справочников лежат в свободном доступе, заходим под анонимной учеткой
            ftpUsername = "anonymous";
            ftpPassword = string.Empty;
            PumpFtpFolder(FTP_PATH_PRODUCT, ReportType.Product);
            PumpFtpFolder(FTP_PATH_OKSM, ReportType.OKSM);
            PumpFtpFolder(FTP_PATH_ORGANIZATION, ReportType.Organization);

            // данные по закупкам закрыты учеткой free/free
            ftpUsername = "free";
            ftpPassword = "free";
            List<string> folders = GetFtpFolders();
            if (ftpRegionFolder == string.Empty)
            {
                foreach (string folder in folders)
                {
                    // сначала качаем извещения
                    string path = string.Concat(folder, Path.DirectorySeparatorChar, FTP_PATH_NOTIFICATIONS);
                    PumpFtpFolder(path, ReportType.Notifications);
                    path = string.Concat(folder, Path.DirectorySeparatorChar, FTP_PATH_NOTIFICATIONS_DAILY);
                    PumpFtpFolder(path, ReportType.Notifications);
                    // потом контракты
                    path = string.Concat(folder, Path.DirectorySeparatorChar, FTP_PATH_CONTRACTS);
                    PumpFtpFolder(path, ReportType.Contracts);
                    path = string.Concat(folder, Path.DirectorySeparatorChar, FTP_PATH_CONTRACTS_DAILY);
                    PumpFtpFolder(path, ReportType.Contracts);
                }
            }
            else
            {
                if (!folders.Contains(ftpRegionFolder))
                    throw new Exception(string.Format("Папка с названием '{0}' на FTP-сервере не найдена.", ftpRegionFolder));
                // сначала качаем извещения
                string path = string.Concat(ftpRegionFolder, Path.AltDirectorySeparatorChar, FTP_PATH_NOTIFICATIONS);
                PumpFtpFolder(path, ReportType.Notifications);
                path = string.Concat(ftpRegionFolder, Path.AltDirectorySeparatorChar, FTP_PATH_NOTIFICATIONS_DAILY);
                PumpFtpFolder(path, ReportType.Notifications);
                // потом контракты
                path = string.Concat(ftpRegionFolder, Path.AltDirectorySeparatorChar, FTP_PATH_CONTRACTS);
                PumpFtpFolder(path, ReportType.Contracts);
                path = string.Concat(ftpRegionFolder, Path.AltDirectorySeparatorChar, FTP_PATH_CONTRACTS_DAILY);
                PumpFtpFolder(path, ReportType.Contracts);
            }
        }

        #endregion Работа с Ftp

        #region Работа с источником

        private const string CONFIG_FILENAME = "config.txt";
        private void LoadConfig(DirectoryInfo dir)
        {
            string configFilename = string.Format("{0}\\{1}", dir.FullName, CONFIG_FILENAME);
            if (!File.Exists(configFilename))
                throw new Exception("Не найден конфигурационный файл.");

            string[] configs = File.ReadAllLines(configFilename);
            foreach (string configStr in configs)
            {
                if (configStr.Trim() == string.Empty)
                    continue;
                string[] config = configStr.Split('=');
                switch (config[0].Trim().ToUpper())
                {
                    case "FTP_SERVER":
                        ftpServer = config[1].Trim();
                        break;
                    case "FTP_PORT":
                        ftpPort = config[1].Trim();
                        break;
                    case "PROXY_SERVER":
                        proxyServer = config[1].Trim();
                        break;
                    case "PROXY_PORT":
                        proxyPort = config[1].Trim();
                        break;
                    case "PROXY_USERNAME":
                        proxyUsername = config[1].Trim();
                        break;
                    case "PROXY_PASSWORD":
                        proxyPassword = config[1].Trim();
                        break;
                }
            }
        }

        private void PumpZipFile(FileInfo file)
        {
            DirectoryInfo archiveDir = CommonRoutines.ExtractArchiveFileToTempDir(file.FullName,
                FilesExtractingOption.SingleDirectory, ArchivatorName.Zip);
            try
            {
                FileInfo[] archiveFiles = archiveDir.GetFiles("*.xml", SearchOption.AllDirectories);
                foreach (FileInfo archiveFile in archiveFiles)
                    PumpXmlFile(archiveFile);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(archiveDir);
            }
        }

        // в таблице фактов fct удаляем устаревшие данные, у которых AUXILIARY меньше максимального
        // в пределах записей, сгруппированных по полю groupedField
        private void DeleteObsoleteData(IFactTable fct, string groupedField, int pumpedSourceID)
        {
            WriteToTrace("Удаление устаревших данных из таблицы фактов " + fct.FullCaption, TraceMessageKind.Information);

            string constraint = string.Format(" SOURCEID = {1} and not AUXILIARY in " +
                " (select max(AUXILIARY) from {0} where SOURCEID = {1} group by {2}) ",
                fct.FullDBName, pumpedSourceID, groupedField);

            DirectDeleteFactData(new IFactTable[] { fct }, -1, -1, constraint);
        }

        #region Заполнение информации о контрактах

        private struct ContractInfo
        {
            public int contractID;
            public int refSuppliers;
            public int refOrg;
            public int refTerritory;
            public int refLevelOrder;
            public decimal price;
        }

        private void FillAuxContractsCache(ref Dictionary<string, ContractInfo> cache, ref List<ContractInfo> unusedContracts, DataTable dt)
        {
            if (cache != null)
                cache.Clear();
            cache = new Dictionary<string, ContractInfo>();

            if (unusedContracts != null)
                unusedContracts.Clear();
            unusedContracts = new List<ContractInfo>();

            foreach (DataRow row in dt.Rows)
            {
                string[] auxiliary = Convert.ToString(row["Auxiliary"]).Split('|');
                if (auxiliary[0].Trim() == string.Empty)
                    continue;

                if (auxiliary[0] != NO_NOTIFICATION_MARK)
                {
                    // если по контракту есть закупка, то поле Auxiliary состоит из:
                    // {ID закупки}|{ID заказчика}|{номер лота}|{ID поставщика}|{цена контракта}

                    // для ключа вспомогательного кэша используем первые 3 значения из Auxiliary
                    string key = string.Format("{0}|{1}|{2}", auxiliary[0], auxiliary[1], auxiliary[2]);
                    if (cache.ContainsKey(key))
                        continue;

                    // остальные данные записываем в структуру ContractInfo
                    ContractInfo contractInfo = new ContractInfo();
                    contractInfo.contractID = Convert.ToInt32(row["ID"]);
                    contractInfo.refSuppliers = Convert.ToInt32(auxiliary[3]);
                    contractInfo.price = CleanFactValue(auxiliary[4]);

                    cache.Add(key, contractInfo);
                }
                else
                {
                    // если по контракту нет закупки, то поле Auxiliary состоит из:
                    // NO_NOTIFICATION_MARK|{ID заказчика}|{ID территории}|{ID уровня бюджета}|{ID поставщика}|{цена контракта}

                    ContractInfo contractInfo = new ContractInfo();
                    contractInfo.contractID = Convert.ToInt32(row["ID"]);
                    contractInfo.refOrg = Convert.ToInt32(auxiliary[1]);
                    contractInfo.refTerritory = Convert.ToInt32(auxiliary[2]);
                    contractInfo.refLevelOrder = Convert.ToInt32(auxiliary[3]);
                    contractInfo.refSuppliers = Convert.ToInt32(auxiliary[4]);
                    contractInfo.price = CleanFactValue(auxiliary[5]);

                    unusedContracts.Add(contractInfo);
                }
            }
        }

        // заполняем поля RefContract, RefSupplier и Price в таблице фактов ООС_АС ООС_Данные
        // используем для этого содержимое поля Auxiliary в классификаторе "Контракты"
        // оно состоит из: {ID закупки}|{ID заказчика}|{номер лота}|{ID поставщика}|{цена контракта}
        private void FillContractsInfo(int pumpedSourceID)
        {
            WriteToTrace("Заполнение информации о контрактах в таблице фактов ООС_АС ООС_Данные", TraceMessageKind.Information);

            string constraint = string.Format("SourceID = {0}", pumpedSourceID);

            // формируем вспомогательный кэш по предметам контракта (ключ: ID, значение: CodeStr - номе лота)
            // используется, чтобы извлечь номер лота из таблицы фактов
            InitDataSet(ref daSubject, ref dsSubject, clsSubject, constraint);
            Dictionary<int, string> auxSubjectCache = null;
            FillRowsCache(ref auxSubjectCache, dsSubject.Tables[0], "ID", "CodeStr");
            ClearDataSet(ref dsSubject);

            // формируем вспомогательный кэш по контрактам (ключ: {ID закупки}|{ID заказчика}|{номер лота}, значение: инфа о контракте)
            // и список контрактов, по которым не удалось определить номер извещения
            InitDataSet(ref daContracts, ref dsContracts, clsContracts, constraint);
            Dictionary<string, ContractInfo> auxContractCache = null;
            List<ContractInfo> unusedContracts = null;
            FillAuxContractsCache(ref auxContractCache, ref unusedContracts, dsContracts.Tables[0]);
            ClearDataSet(ref dsContracts);

            // формируем вспомогательный кэш по извещениям, у которых контракт уже проставлен
            Dictionary<string, int> auxNotificationCache = null;
            InitDataSet(ref daOOSValue, ref dsOOSValue, fctOOSValue, constraint);
            FillRowsCache(ref auxNotificationCache, dsOOSValue.Tables[0], "RefContract");

            foreach (DataRow row in dsOOSValue.Tables[0].Rows)
            {
                string key = string.Format("{0}|{1}|{2}", row["RefPurchase"], row["RefOrg"],
                    FindCachedRow(auxSubjectCache, Convert.ToInt32(row["RefSubject"]), "0"));
                if (auxContractCache.ContainsKey(key))
                {
                    row["RefContract"] = auxContractCache[key].contractID;
                    row["RefSupplier"] = auxContractCache[key].refSuppliers;
                    row["Price"] = auxContractCache[key].price;
                }
            }

            // добавляем в таблицу фактов записи со ссылками на контракты,
            // по которым не удалось определить номер извещения, т.к. у них указан только один поставщик
            foreach (ContractInfo contract in unusedContracts)
            {
                if (auxNotificationCache.ContainsKey(contract.contractID.ToString()))
                    continue;

                object[] mapping = new object[] {
                    "PMP", 0,
                    "Price", contract.price,
                    "RefOrg", contract.refOrg,
                    "RefSubject", nullSubject,
                    "RefPurchase", nullPurchase,
                    "RefLevelOrder", contract.refLevelOrder,
                    "RefTerritory", contract.refTerritory,
                    "RefSupplier", contract.refSuppliers,
                    "RefContract", contract.contractID,
                    "Auxiliary", "0",
                    "SourceID", pumpedSourceID
                };

                PumpRow(dsOOSValue.Tables[0], mapping);
            }

            UpdateData();
        }

        #endregion

        // закачиваем полученные с ftp файлы
        private void PumpAllFiles()
        {
            List<int> pumpedSourceIDs = new List<int>();
            List<string> sourceVariants = new List<string>();
            foreach (KeyValuePair<string, ReportType> folder in pumpFolders)
            {
                reportType = folder.Value;
                switch (reportType)
                {
                    case ReportType.Notifications:
                    case ReportType.Contracts:
                        string dir = folder.Key.Replace(tempDir, string.Empty);
                        string variant = dir.Split(new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar },
                            StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                        if (!sourceVariants.Contains(variant))
                        {
                            SetNewSourceID(variant);
                            pumpedSourceIDs.Add(sourceID);
                            sourceVariants.Add(variant);
                        }
                        break;
                }

                ProcessFilesTemplate(new DirectoryInfo(folder.Key), "*.zip", new ProcessFileDelegate(PumpZipFile), false, SearchOption.TopDirectoryOnly);
                ProcessFilesTemplate(new DirectoryInfo(folder.Key), "*.xml", new ProcessFileDelegate(PumpXmlFile), false, SearchOption.TopDirectoryOnly);
                UpdateData();
            }

            foreach (int pumpedSourceID in pumpedSourceIDs)
            {
                DeleteObsoleteData(fctOOSValue, "RefPurchase", pumpedSourceID);
                DeleteObsoleteData(fctOOSProduct, "RefContract", pumpedSourceID);
                FillContractsInfo(pumpedSourceID);
            }
        }

        private void WriteDebugInfo()
        {
            if (territoryRF != null)
            {
                WriteToTrace(string.Format("Субъект РФ: {0} - {1} ({2})",
                    territoryRF["OKATO"], territoryRF["Name"], territoryRF["ID"]),
                    TraceMessageKind.Information);
            }
            if (ftpRegionFolder != string.Empty)
            {
                WriteToTrace("Папка на FTP: " + ftpRegionFolder, TraceMessageKind.Information);
            }
            WriteToTrace(string.Format("Ftp-сервер: {0}:{1}",
                ftpServer, ftpPort), TraceMessageKind.Information);
            if (proxyServer != string.Empty)
            {
                WriteToTrace(string.Format("Прокси-сервер: {0}:{1}",
                    proxyServer, proxyPort), TraceMessageKind.Information);
            }
        }

        // перемещаем файлы из источника в папку архива
        private void MoveSourceFilesToArchive(DirectoryInfo dir)
        {
            if (useArchive)
            {
                // Получаем путь к архивному каталогу
                string archive = string.Format("{0}{1}OOS Source Date",
                    this.Scheme.DataSourceManager.ArchiveDirectory,
                    Path.DirectorySeparatorChar);

                // Если такого каталога нет - создаем
                if (!Directory.Exists(archive))
                    Directory.CreateDirectory(archive);

                // Перемещаем подкаталоги
                DirectoryInfo[] subDirs = dir.GetDirectories("*", SearchOption.AllDirectories);
                for (int i = 0; i < subDirs.GetLength(0); i++)
                {
                    string archiveSubDir = subDirs[i].FullName.Replace(dir.FullName, archive);
                    if (!Directory.Exists(archiveSubDir))
                        Directory.CreateDirectory(archiveSubDir);
                }

                // Перемещаем файлы
                FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.GetLength(0); i++)
                {
                    string archiveFile = files[i].FullName.Replace(dir.FullName, archive);
                    if (File.Exists(archiveFile))
                        File.Delete(archiveFile);
                    files[i].MoveTo(archiveFile);
                }

                Directory.Delete(dir.FullName, true);
            }
        }

        #endregion Работа с источником

        #region Перекрытые методы

        protected override void DirectClsHierarchySetting()
        {
            base.DirectClsHierarchySetting();
            DataSet ds = null;
            try
            {
                clsOKDP.DivideAndFormHierarchy(-1, false);
            }
            finally
            {
                ClearDataSet(ref ds);
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            tempDir = string.Format("{0}\\temp", dir.FullName);
            pumpFolders = new Dictionary<string, ReportType>();
            try
            {
                if (!Directory.Exists(tempDir))
                    Directory.CreateDirectory(tempDir);
                LoadConfig(dir);
                WriteDebugInfo();

                PumpAllFilesFromFtp();
                PumpAllFiles();

                MoveSourceFilesToArchive(new DirectoryInfo(tempDir));
            }
            finally
            {
                pumpFolders.Clear();
            }
        }

        private void SetDataPumpParams()
        {
            refTerritoryRF = Convert.ToInt32(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "uneTerritoryRF", "-1"));
            ftpRegionFolder = GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "uneFTPFolder", string.Empty).Trim();
        }

        protected override void  DirectPumpData()
        {
            useArchive = this.UseArchive;
            // отключаем перемещение в архивы, чтобы не сработала стандартная процедура
            this.UseArchive = false;
            SetDataPumpParams();
            PumpDataVTemplate();
        }

        #endregion Перекрытые методы

        #endregion Закачка данных

    }

}
