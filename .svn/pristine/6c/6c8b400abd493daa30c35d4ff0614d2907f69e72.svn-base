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


namespace Krista.FM.Server.DataPumps.BudgetVaultPump
{
    // общий модуль закачки
    /// <summary>
    /// ФО_0004 Свод бюджета
    /// Предоставляется согласно «Требованиям к форматам и способам передачи в электронном виде
    /// консолидированных бюджетов субъектов РФ и бюджетов ЗАТО, предоставляемых в Министерство финансов РФ»
    /// </summary>
    public partial class BudgetVaultPumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы 

        // КД.ФО_Свод (d_KD_FOProj)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        private int nullKD;
        // ФКР.ФО_Свод (d.FKR.FOProject)
        private IDbDataAdapter daFKR;
        private DataSet dsFKR;
        private IClassifier clsFKR;
        private Dictionary<string, int> fkrCache = null;
        private int nullFKR;
        // КЦСР.ФО_Свод (d.KCSR.FOProject)
        private IDbDataAdapter daKCSR;
        private DataSet dsKCSR;
        private IClassifier clsKCSR;
        private Dictionary<string, int> kcsrCache = null;
        private int nullKCSR;
        // КВР.ФО_Свод (d.KVR.FOProject)
        private IDbDataAdapter daKVR;
        private DataSet dsKVR;
        private IClassifier clsKVR;
        private Dictionary<string, int> kvrCache = null;
        private int nullKVR;
        // ЭКР.ФО_Свод (d_EKR_FOProj)
        private IDbDataAdapter daEKR;
        private DataSet dsEKR;
        private IClassifier clsEKR;
        private Dictionary<string, int> ekrCache = null;
        private int nullEKR;
        // КИФ.ФО_Свод_2004 (d.KIF.FOProj2004)
        private IDbDataAdapter daKIF2004;
        private DataSet dsKIF2004;
        private IClassifier clsKIF2004;
        private Dictionary<string, int> kifCache = null;
        private int nullKIF2004;
        // КИФ.ФО_Свод_2005 (d.KIF.FOProj2005)
        private IDbDataAdapter daKIF2005;
        private DataSet dsKIF2005;
        private IClassifier clsKIF2005;
        private int nullKIF2005;
        // КСШК.ФО_Свод (d.KSSHK.FOProject)
        private IDbDataAdapter daKSHK;
        private DataSet dsKSHK;
        private IClassifier clsKSHK;
        private Dictionary<string, int> kshkCache = null;
        private int nullKSHK;
        // Показатели.ФО_Свод_СубКВР (d.Marks.FOProjSKVR)
        private IDbDataAdapter daSubKVR;
        private DataSet dsSubKVR;
        private IClassifier clsSubKVR;
        private Dictionary<string, int> subKVRCache = null;
        private int nullSubKVR;
        // Районы.ФО_Свод (d.Regions.FOProject)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        // Районы.Служебный для закачки СКИФ
        private IDbDataAdapter daRegionForPump;
        private DataSet dsRegionForPump;
        private IClassifier clsRegionForPump;
        private Dictionary<string, int> regionForPumpCache = null;
        // ФКР.Анализ (d.FKR.Analysis)
        private IDbDataAdapter daAnalFKR;
        private DataSet dsAnalFKR;
        private IClassifier clsAnalFKR;
        private Dictionary<string, string> analFKRCache = null;
        // КЦСР.Анализ (d.KCSR.Analysis)
        private IDbDataAdapter daAnalKCSR;
        private DataSet dsAnalKCSR;
        private IClassifier clsAnalKCSR;
        private Dictionary<string, string> analKCSRCache = null;
        // КВР.Анализ (d.KVR.Analysis)
        private IDbDataAdapter daAnalKVR;
        private DataSet dsAnalKVR;
        private IClassifier clsAnalKVR;
        private Dictionary<string, string> analKVRCache = null;
        // Расходы.ФО_Свод (d_R_FOProj)
        private IDbDataAdapter daOutcomesCls;
        private DataSet dsOutcomesCls;
        private IClassifier clsOutcomesCls;
        private Dictionary<string, int> outcomesClsCache = null;
        private int nullOutcomesCls;
        private int defaultOutcomesCls;

        #endregion Классификаторы

        #region Факты

        // ФО_Свод_Доходы (f.D.FOProject)
        private IDbDataAdapter daIncomes;
        private DataSet dsIncomes;
        private IFactTable fctIncomes;
        // ФО_Свод_Расходы (f.R.FOProject)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IFactTable fctOutcomes;
        // ФО_Свод_ДефицитПрофицит (f.DP.FOProject)
        private IDbDataAdapter daDefProf;
        private DataSet dsDefProf;
        private IFactTable fctDefProf;
        // ФО_Свод_ИФ (f.SrcFin.FOProject)
        private IDbDataAdapter daFinSources;
        private DataSet dsFinSources;
        private IFactTable fctFinSources;
        // ФО_Свод_Сеть (f.Net.FOProject)
        private IDbDataAdapter daNets;
        private DataSet dsNets;
        private IFactTable fctNets;

        #endregion Факты

        private int xmlFilesCount = 0;
        private int dbfFilesCount = 0;
        private int filesCount = 0;
        private int sumFactor = 1;
        private Block block;
        private string progressMsg;
        private string blockName;
        private int regForPumpSourceID;
        string[] periodID = null;
        private List<string> warnedRegions = new List<string>();
        // параметры обработки
        private int year = -1;
        private int month = -1;

        private Dictionary<int, DataRow> fkrRowsCache = null;
        private Dictionary<int, DataRow> kcsrRowsCache = null;
        private Dictionary<int, DataRow> kvrRowsCache = null;

        #endregion Поля

        #region Константы

        // блоки
        protected enum Block
        {
            bIncomes,
            bOutcomes,
            bDefProf,
            bFinSources,
            bNets
        }

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        /// <summary>
        /// Заполняет кэш классификаторов
        /// </summary>
        private void FillCaches()
        {
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref ekrCache, dsEKR.Tables[0], "CODE");
            if (this.DataSource.Year >= 2005)
            {
                FillRowsCache(ref kifCache, dsKIF2005.Tables[0], "CODESTR");
            }
            else
            {
                FillRowsCache(ref kifCache, dsKIF2004.Tables[0], "CODESTR");
            }
            FillRowsCache(ref fkrCache, dsFKR.Tables[0], "CODE");
            FillRowsCache(ref kcsrCache, dsKCSR.Tables[0], "CODE");
            FillRowsCache(ref kvrCache, dsKVR.Tables[0], "CODE");
            FillRowsCache(ref kshkCache, dsKSHK.Tables[0], "CODE");

            foreach (DataRow row in dsSubKVR.Tables[0].Rows)
                row["CodeRprt"] = row["CodeRprt"].ToString().ToUpper().Replace('Х', 'X');
            FillRowsCache(ref subKVRCache, dsSubKVR.Tables[0], new string[] { "CodeRprt" } , string.Empty, "ID", " ");

            FillRowsCache(ref regionCache, dsRegions.Tables[0], new string[] { "CODE", "NAME" }, "|", "ID");
            FillRowsCache(ref regionForPumpCache, dsRegionForPump.Tables[0], new string[] { "CODESTR", "NAME" }, "|", "REFDOCTYPE");

            FillRowsCache(ref outcomesClsCache, dsOutcomesCls.Tables[0], "CODE", "Id");
        }

        /// <summary>
        /// Инициализирует строки классификаторов "Неизвестные данные"
        /// </summary>
        private void InitNullClsRows()
        {
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullFKR = clsFKR.UpdateFixedRows(this.DB, this.SourceID);
            nullKCSR = clsKCSR.UpdateFixedRows(this.DB, this.SourceID);
            nullKVR = clsKVR.UpdateFixedRows(this.DB, this.SourceID);
            nullEKR = clsEKR.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF2004 = clsKIF2004.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF2005 = clsKIF2005.UpdateFixedRows(this.DB, this.SourceID);
            nullKSHK = clsKSHK.UpdateFixedRows(this.DB, this.SourceID);
            nullOutcomesCls = clsOutcomesCls.UpdateFixedRows(this.DB, this.SourceID);
        }

        /// <summary>
        /// Запрос данных из базы
        /// </summary>
        protected override void QueryData()
        {
            // классификаторы
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR, false, string.Empty);
            InitClsDataSet(ref daKCSR, ref dsKCSR, clsKCSR, false, string.Empty);
            InitClsDataSet(ref daKVR, ref dsKVR, clsKVR, false, string.Empty);
            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR, false, string.Empty);
            InitClsDataSet(ref daKIF2004, ref dsKIF2004, clsKIF2004, false, string.Empty);
            InitClsDataSet(ref daKIF2005, ref dsKIF2005, clsKIF2005, false, string.Empty);
            InitClsDataSet(ref daKSHK, ref dsKSHK, clsKSHK, false, string.Empty);
            InitClsDataSet(ref daSubKVR, ref dsSubKVR, clsSubKVR, false, string.Empty);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls, false, string.Empty);
            GetRegionsForPumpSourceID();
            InitDataSet(ref daRegionForPump, ref dsRegionForPump, clsRegionForPump, false, string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);
            // факты
            InitFactDataSet(ref daIncomes, ref dsIncomes, fctIncomes);
            InitFactDataSet(ref daOutcomes, ref dsOutcomes, fctOutcomes);
            InitFactDataSet(ref daDefProf, ref dsDefProf, fctDefProf);
            InitFactDataSet(ref daFinSources, ref dsFinSources, fctFinSources);
            InitFactDataSet(ref daNets, ref dsNets, fctNets);
            // нулевые значения классификаторов
            InitNullClsRows();
            // кэши
            FillCaches();
        }

        /// <summary>
        /// Внести изменения в базу
        /// </summary>
        protected override void UpdateData()
        {
            // классификаторы
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daFKR, dsFKR, clsFKR);
            UpdateDataSet(daKCSR, dsKCSR, clsKCSR);
            UpdateDataSet(daKVR, dsKVR, clsKVR);
            UpdateDataSet(daEKR, dsEKR, clsEKR);
            UpdateDataSet(daKIF2004, dsKIF2004, clsKIF2004);
            UpdateDataSet(daKIF2005, dsKIF2005, clsKIF2005);
            UpdateDataSet(daKSHK, dsKSHK, clsKSHK);
            UpdateDataSet(daSubKVR, dsSubKVR, clsSubKVR);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daRegionForPump, dsRegionForPump, clsRegionForPump);
            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);
            // факты
            UpdateDataSet(daIncomes, dsIncomes, fctIncomes);
            UpdateDataSet(daOutcomes, dsOutcomes, fctOutcomes);
            UpdateDataSet(daDefProf, dsDefProf, fctDefProf);
            UpdateDataSet(daFinSources, dsFinSources, fctFinSources);
            UpdateDataSet(daNets, dsNets, fctNets);
        }

        #region GUID

        private const string D_REGIONS_FOR_PUMP_SKIF_GUID = "e9a95119-21f1-43d8-8dc2-8d4af7c195d0";
        private const string D_FKR_ANALYSIS_GUID = "59ae5067-beb9-4ff6-b5b2-38d606fde212";
        private const string D_KCSR_ANALYSIS_GUID = "24fa721b-fa99-4567-8740-959ac85ff394";
        private const string D_KVR_ANALYSIS_GUID = "57bcfd65-d001-4f90-8889-faa174247ce5";
        private const string D_SUB_KVR_ANALYSIS_GUID = "706c19ed-75cb-45a9-85df-56d30b89b62c";

        private const string D_KD_FO_PROJ_GUID = "5221400b-e97f-4693-8d5e-0d1744a28a72";
        private const string D_FKR_FO_PROJECT_GUID = "f5ab6a91-c386-4476-aa54-65776d22ce7a";
        private const string D_KCSR_FO_PROJECT_GUID = "0ff93bdc-fb41-471d-83fb-abc34dfc8484";
        private const string D_KVR_FO_PROJECT_GUID = "793fde10-2d85-4e97-a238-2c885334f6fc";
        private const string D_EKR_FO_PROJ_GUID = "4774b2e1-a44d-46b7-9801-98422268cb09";
        private const string D_KIF_FO_PROJ_2004_GUID = "2582a392-61f0-411b-86d1-06a808a55ac9";
        private const string D_KIF_FO_PROJ_2005_GUID = "32a7e5ae-5c3b-4e5a-bd83-e9a475e70345";
        private const string D_KSSHK_FO_PROJECT_GUID = "33cec1a3-b14f-42ed-b69f-ff1026c6f212";
        private const string D_OUTCOMES_CLS_GUID = "51f7b2f5-8fc2-480a-8f13-e70924126727";
        private const string D_REGIONS_FO_PROJECT_GUID = "718390ac-49d9-4eb0-bd2b-00eeb554c287";

        private const string F_D_FO_PROJECT_GUID = "ccefd879-21c7-406e-beea-af9f27e074d2";
        private const string F_R_FO_PROJECT_GUID = "3ff949d7-91d4-414c-8d8c-5f727b4de979";
        private const string F_DF_FO_PROJECT_GUID = "f150c174-769c-4d27-bd9d-1adcf3116e06";
        private const string F_SRC_FIN_FO_PROJECT_GUID = "ae9736a7-53d1-4e48-846d-819610555d9f";
        private const string F_NET_FO_PROJECT_GUID = "4000b219-3e20-487e-a4f1-a2f36f005440";

        #endregion GUID

        protected override void InitDBObjects()
        {
            clsRegionForPump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_SKIF_GUID];
            clsAnalFKR = this.Scheme.Classifiers[D_FKR_ANALYSIS_GUID];
            clsAnalKCSR = this.Scheme.Classifiers[D_KCSR_ANALYSIS_GUID];
            clsAnalKVR = this.Scheme.Classifiers[D_KVR_ANALYSIS_GUID];
            clsSubKVR = this.Scheme.Classifiers[D_SUB_KVR_ANALYSIS_GUID];

            this.UsedClassifiers = new IClassifier[] { 
                clsKD = this.Scheme.Classifiers[D_KD_FO_PROJ_GUID],
                clsFKR = this.Scheme.Classifiers[D_FKR_FO_PROJECT_GUID],
                clsKCSR = this.Scheme.Classifiers[D_KCSR_FO_PROJECT_GUID],
                clsKVR = this.Scheme.Classifiers[D_KVR_FO_PROJECT_GUID],
                clsEKR = this.Scheme.Classifiers[D_EKR_FO_PROJ_GUID],
                clsKIF2004 = this.Scheme.Classifiers[D_KIF_FO_PROJ_2004_GUID],
                clsKIF2005 = this.Scheme.Classifiers[D_KIF_FO_PROJ_2005_GUID],
                clsKSHK = this.Scheme.Classifiers[D_KSSHK_FO_PROJECT_GUID],
                clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FO_PROJECT_GUID] };

            this.VersionClassifiers = new IClassifier[] {clsKD, clsEKR };

            this.UsedFacts = new IFactTable[] {
                fctIncomes = this.Scheme.FactTables[F_D_FO_PROJECT_GUID],
                fctOutcomes = this.Scheme.FactTables[F_R_FO_PROJECT_GUID],
                fctDefProf = this.Scheme.FactTables[F_DF_FO_PROJECT_GUID],
                fctFinSources = this.Scheme.FactTables[F_SRC_FIN_FO_PROJECT_GUID],
                fctNets = this.Scheme.FactTables[F_NET_FO_PROJECT_GUID] };
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            // классификаторы
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsFKR);
            ClearDataSet(ref dsKCSR);
            ClearDataSet(ref dsKVR);
            ClearDataSet(ref dsEKR);
            ClearDataSet(ref dsKIF2004);
            ClearDataSet(ref dsKIF2005);
            ClearDataSet(ref dsKSHK);
            ClearDataSet(ref dsSubKVR);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsRegionForPump);
            ClearDataSet(ref dsOutcomesCls);
            // факты
            ClearDataSet(ref dsIncomes);
            ClearDataSet(ref dsOutcomes);
            ClearDataSet(ref dsDefProf);
            ClearDataSet(ref dsFinSources);
            ClearDataSet(ref dsNets);

            pumpedRegions.Clear();
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private string GetConfigParamName(Block block)
        {
            switch (block)
            {
                case Block.bIncomes:
                    return "ucbIncomes";
                case Block.bOutcomes:
                    return "ucbOutcomes";
                case Block.bDefProf:
                    return "ucbDefProf";
                case Block.bFinSources:
                    return "ucbFinSources";
                case Block.bNets:
                    return "ucbNet";
                default:
                    return string.Empty;
            }
        }

        private bool ToPumpBlock(Block block)
        {
            string configParamName = GetConfigParamName(block);
            return (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, configParamName, "False")));
        }

        protected override void DeleteEarlierPumpedData()
        {
            if (!this.DeleteEarlierData)
                return;
            // доходы
            if (ToPumpBlock(Block.bIncomes))
            {
                DirectDeleteFactData(new IFactTable[] { fctIncomes }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKD }, -1, this.SourceID, string.Empty);
            }
            // расходы
            if (ToPumpBlock(Block.bOutcomes))
            {
                DirectDeleteFactData(new IFactTable[] { fctOutcomes }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsOutcomesCls }, -1, this.SourceID, string.Empty);
            }
            // дефицит профицит
            if (ToPumpBlock(Block.bDefProf))
            {
                DirectDeleteFactData(new IFactTable[] { fctDefProf }, -1, this.SourceID, string.Empty);
            }
            // источники финансирования
            if (ToPumpBlock(Block.bFinSources))
            {
                DirectDeleteFactData(new IFactTable[] { fctFinSources }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKIF2004, clsKIF2005 }, -1, this.SourceID, string.Empty);
            }
            // сети
            if (ToPumpBlock(Block.bNets))
            {
                DirectDeleteFactData(new IFactTable[] { fctNets }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKSHK }, -1, this.SourceID, string.Empty);
            }

            if ((ToPumpBlock(Block.bOutcomes)) && (ToPumpBlock(Block.bNets)))
                DirectDeleteClsData(new IClassifier[] { clsKVR, clsKCSR, clsFKR, clsEKR }, -1, this.SourceID, string.Empty);

            // районы удаляем, если закачиваются все блоки
            if (ToPumpBlock(Block.bIncomes) && ToPumpBlock(Block.bOutcomes) && ToPumpBlock(Block.bDefProf) &&
                ToPumpBlock(Block.bFinSources) && ToPumpBlock(Block.bNets))
                DirectDeleteClsData(new IClassifier[] { clsRegions }, -1, this.SourceID, string.Empty);
        }

        /// <summary>
        /// Возвращает ИД источника для Районы.Служебный
        /// </summary>
        /// <returns>ИД источника для Районы.Служебный</returns>
        private void GetRegionsForPumpSourceID()
        {
            regForPumpSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        /// <summary>
        /// получить строковый тип документа
        /// </summary>
        /// <param name="docType"> тип документа </param>
        /// <returns></returns>
        private string GetSKIFDocType(int docType)
        {
            switch (docType)
            {
                case 0:
                    return "Значение не указано";
                case 1:
                    return "Неуказанный тип отчетности";
                case 2:
                    return "Собственная отчетность муниципальных образований";
                case 3:
                    return "Консолидированная отчетность и отчетность внебюджетных территориальных фондов";
                case 4:
                    return "Отчетность главных распорядителей средств бюджета";
                case 5:
                    return "Собственный отчет по бюджету субъекта";
                case 20:
                    return "Данные в разрезе муниципальных образований";
                case 21:
                    return "Данные в разрезе муниципальных образований и поселений";
                default:
                    return string.Empty;
            }
        }

        private bool PumpRegionForPump(string code, string key, string name)
        {
            if (!regionForPumpCache.ContainsKey(key))
            {
                PumpCachedRow(regionForPumpCache, dsRegionForPump.Tables[0], clsRegionForPump, code, key, "CODESTR", "REFDOCTYPE",
                    new object[] { "NAME", name, "REFDOCTYPE", 1, "SOURCEID", regForPumpSourceID });
                return false;
            }
            return true;
        }

        private void GetClsValuesMapping(DataTable[] clsTable, int[] nullRefsToCls, Dictionary<string, int>[] clsCaches,
            ref object[] clsValuesMapping, string[] cacheKeys)//
        {
            int count = clsTable.GetLength(0);
            for (int i = 0; i < count; i++)
                if ((clsCaches[i] != null) && (cacheKeys[i] != string.Empty) && clsTable[i].Rows.Count != 0)
                    clsValuesMapping[i * 2 + 1] = FindCachedRow(clsCaches[i], cacheKeys[i], nullRefsToCls[i]);
        }

        // мочим записи, по которым нет данных
        private void DeleteUnusedClsRecords(IFactTable fct, ref IClassifier cls, ref IDbDataAdapter daCls, ref DataSet dsCls, string refFieldName)
        {
            string query = string.Format("update {0} set parentId = null where sourceid = {1}",
                cls.FullDBName, this.SourceID);
            if (dsCls.Tables[0].Columns.Contains("ParentId"))
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            int recCount = dsCls.Tables[0].Rows.Count;
            query = string.Format("delete from {0} where sourceid = {2} and id not in (select distinct {3} from {1} where sourceid = {2})",
                cls.FullDBName, fct.FullDBName, this.SourceID, refFieldName);

            if (cls == clsKVR)
                query += string.Format(" and (id not in (select distinct {0} from {1} where sourceid = {2}))",
                    "RefKVR", fctNets.FullDBName, this.SourceID);
            if (cls == clsKCSR)
                query += string.Format(" and (id not in (select distinct {0} from {1} where sourceid = {2}))",
                    "RefKCSR", fctNets.FullDBName, this.SourceID);

            this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            InitClsDataSet(ref daCls, ref dsCls, cls, false, string.Empty);
            int deletedRecCount = recCount - dsCls.Tables[0].Rows.Count;
            if (deletedRecCount != 0)
            {
                string message = string.Format("Классификатор '{0}' содержал записи, по которым не было данных в таблице фактов '{1}'. Эти записи были удалены (кол-во: {2})",
                    cls.FullCaption, fct.FullCaption, deletedRecCount);
                if (this.State == PumpProcessStates.PumpData)
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, message);
                else
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, message);
            }
        }

        /// <summary>
        /// Вызывает функции установки иерархии для всех классификаторов
        /// </summary>
        private void SetClsHierarchy()
        {
            // кд
            DeleteUnusedClsRecords(fctIncomes, ref clsKD, ref daKD, ref dsKD, "RefKDFOProj");
            ClearHierarchy(dsKD.Tables[0]);
            SetPresentationContext(clsKD);
            if (this.DataSource.Year >= 2010)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010102000010000110'",
                    "CodeStr = '00010102011010000110'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                    "CodeStr = '00010202041060000160'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                    "CodeStr = '00011105026000000120'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011500000000000140'",
                    "CodeStr = '00011502011060000140'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                    "CodeStr = '00011706011060000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205300070000151'",
                    "CodeStr = '00020205310070000151' or CodeStr = '00020205311070000151' or " +
                    "CodeStr = '00020205312070000151' or CodeStr = '00020205313070000151' or " +
                    "CodeStr = '00020205314070000151' or CodeStr = '00020205315070000151' or " +
                    "CodeStr = '00020205316070000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                    "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151' or " +
                    "CodeStr = '00020205116060000151' or CodeStr = '00020205117060000151' or " +
                    "CodeStr = '00020205119060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205120060000151' or CodeStr = '00020205122060000151' or " +
                    "CodeStr = '00020205123060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087000000000001151' or CodeStr = '00087000000000002151'");
            }
            else if (this.DataSource.Year >= 2008)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                    "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087100000000000000' or CodeStr = '00087200000000000000'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205800090000151'",
                    "CodeStr = '00020205810090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                    "CodeStr = '00010202041060000160'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                    "CodeStr = '00011706011060000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                    "CodeStr = '00011105026000000120'");
            }
            else if (this.DataSource.Year >= 2007)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020203900090000151'",
                    "CodeStr = '00020203901090000151' or CodeStr = '00020203902090000151' or " +
                    "CodeStr = '00020203903090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204010000000151'",
                    "CodeStr = '00020204011020000151' or CodeStr = '00020204012020000151' or " +
                    "CodeStr = '00020204013020000151' or CodeStr = '00020204014020000151' or " +
                    "CodeStr = '00020204015020000151' or CodeStr = '00020204016020000151' or " +
                    "CodeStr = '00020204017020000151' or CodeStr = '00020204018020000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204900090000151'",
                    "CodeStr = '00020204901090000151' or CodeStr = '00020204903090000151' or " +
                    "CodeStr = '00020204904090000151' or CodeStr = '00020204905090000151' or " +
                    "CodeStr = '00020204906090000151' or CodeStr = '00020204907090000151' or " +
                    "CodeStr = '00020204908090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151'");
            }
            else if (this.DataSource.Year >= 2006)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020202900000000151'",
                    "CodeStr = '00020202910020000151' or CodeStr = '00020202920030000151' or " +
                    "CodeStr = '00020202930040000151' or CodeStr = '00020202940050000151' or " +
                    "CodeStr = '00020202940100000151' or CodeStr = '00020202950060000151' or " +
                    "CodeStr = '00020202960070000151' or CodeStr = '00020202970080000151' or " +
                    "CodeStr = '00020202980090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204000000000151'",
                    "CodeStr = '00020204019020000151' or CodeStr = '00020204241000000151' or " +
                    "CodeStr = '00020204242000000151' or CodeStr = '00020204243000000151' or " +
                    "CodeStr = '00020204244000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204900000000151'",
                    "CodeStr = '00020204910020000151' or CodeStr = '00020204920030000151' or " +
                    "CodeStr = '00020204920040000151' or CodeStr = '00020204930050000151' or " +
                    "CodeStr = '00020204930100000151' or CodeStr = '00020204950060000151' or " +
                    "CodeStr = '00020204960070000151' or CodeStr = '00020204970080000151' or " +
                    "CodeStr = '00020204980090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087100000000000000' or CodeStr = '00087200000000000000' or " +
                    "CodeStr = '00087300000000000000'");
            }
            else if (this.DataSource.Year >= 2005)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020202200000000151'",
                    "CodeStr = '00020202210020000151' or CodeStr = '00020202220030000151' or " +
                    "CodeStr = '00020202250060000151' or CodeStr = '00020202260070000151' or " +
                    "CodeStr = '00020202270080000151' or CodeStr = '00020202280090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204100000000151'",
                    "CodeStr = '00020204110020000151' or CodeStr = '00020204120030000151' or " +
                    "CodeStr = '00020204150060000151' or CodeStr = '00020204160070000151' or " +
                    "CodeStr = '00020204170080000151' or CodeStr = '00020204180090000151'");
            }
            else
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);

            // киф
            DeleteUnusedClsRecords(fctFinSources, ref clsKIF2005, ref daKIF2005, ref dsKIF2005, "RefKIF");
            if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsKIF2005, clsKIF2005, null, string.Empty, ClsHierarchyMode.Standard);
            else
                SetClsHierarchy(ref dsKIF2004, clsKIF2004, null, string.Empty, ClsHierarchyMode.Standard);
            // Расходы
            DeleteUnusedClsRecords(fctOutcomes, ref clsKVR, ref daKVR, ref dsKVR, "RefKVR");
            DeleteUnusedClsRecords(fctOutcomes, ref clsKCSR, ref daKCSR, ref dsKCSR, "RefKCSR");

            SetClsHierarchy(ref dsKVR, clsKVR, null, string.Empty, ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsKCSR, clsKCSR, null, string.Empty, ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsFKR, clsFKR, null, string.Empty, ClsHierarchyMode.FKR);

            SetPresentationContext(clsEKR);
            SetClsHierarchy(ref dsEKR, clsEKR, null, string.Empty, ClsHierarchyMode.Standard);
            // сшк 
            DeleteUnusedClsRecords(fctNets, ref clsKSHK, ref daKSHK, ref dsKSHK, "RefKSSHK");
            // Районы служебный
            SetClsHierarchy(ref dsRegionForPump, clsRegionForPump, null, string.Empty, ClsHierarchyMode.Standard);
        }

        /// <summary>
        /// Проверяет правило исключения на инверсию - если впереди стоит !, то результат инвертируется
        /// </summary>
        /// <param name="codeExclusion">Правило исключения</param>
        /// <param name="result">Результат</param>
        private bool InverseExclusionResult(string codeExclusion, bool result)
        {
            if (codeExclusion.TrimStart('#').StartsWith("!"))
                return !result;
            return result;
        }

        /// <summary>
        /// Проверяет код на соответствие списку исключений
        /// </summary>
        /// <param name="code">Код</param>
        /// <param name="codeExclusions">Список исключений</param>
        /// <returns>true - код входит в список исключений</returns>
        private bool CheckCodeExclusion(object code, string[] codeExclusions)//
        {
            if (codeExclusions == null) 
                return false;
            string[] exclusions = codeExclusions;
            if (codeExclusions.GetLength(0) > 1)
                exclusions = GetFieldsValuesAsSubstring(codeExclusions, Convert.ToString(code), string.Empty);
            bool result = false;
            int count = exclusions.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                string codePart = Convert.ToString(code);
                if (codeExclusions.GetLength(0) > 1)
                    codePart = exclusions[i + 1];
                string[] rules = Convert.ToString(exclusions[i]).Split(';');
                int rulesCount = rules.GetLength(0);
                for (int j = 0; j < rulesCount; j++)
                {
                    if (rules[j] == string.Empty)
                        continue;
                    string positiveRule = rules[j].TrimStart('#').TrimStart('!');
                    // Префиксы правил:
                    // "!" - отрицание; "#" - превалирующее правило (если код ему не удовлетворяет, то он не будет пропущен вне зависимости от других правил)
                    // "*code*" - исключаются коды, содержащие указанный
                    if (positiveRule.StartsWith("*") && positiveRule.EndsWith("*"))
                        result = InverseExclusionResult(rules[j], codePart.Contains(positiveRule.Replace("*", string.Empty)));
                    // "code*" - исключаются коды, начинающиеся с указанного
                    else if (positiveRule.EndsWith("*"))
                        result = InverseExclusionResult(rules[j], codePart.StartsWith(positiveRule.Replace("*", string.Empty)));
                    // "*code" - исключаются коды, заканчивающиеся на указанный
                    else if (positiveRule.StartsWith("*"))
                        result = InverseExclusionResult(rules[j], codePart.EndsWith(positiveRule.Replace("*", string.Empty)));
                    // "code1..code2" - исключаются коды, входящие в диапазон code1..code2
                    else if (rules[j].Contains(".."))
                    {
                        string[] values = positiveRule.Split(new string[] { ".." }, StringSplitOptions.None);
                        if (values[0] != string.Empty && values[1] != string.Empty)
                        {
                            if (code is string)
                                result = InverseExclusionResult(rules[j], codePart.CompareTo(values[0]) >= 0 && codePart.CompareTo(values[1]) <= 0);
                            else if (code is int)
                                result = InverseExclusionResult(rules[j], Convert.ToInt32(code) >= Convert.ToInt32(values[0]) &&
                                    Convert.ToInt32(code) <= Convert.ToInt32(values[1]));
                        }
                    }
                    // "<=code" - исключаются коды, меньшие или равные code;
                    else if (positiveRule.StartsWith("<="))
                    {
                        if (code is string)
                            result = InverseExclusionResult(rules[j], codePart.CompareTo(positiveRule.Replace("<=", string.Empty)) < 0);
                        else if (code is int)
                            result = InverseExclusionResult(rules[j], Convert.ToInt32(code) <= Convert.ToInt32(positiveRule.Replace("<=", string.Empty)));
                    }
                    // ">=code" - исключаются коды >= code;
                    else if (positiveRule.StartsWith(">="))
                    {
                        if (code is string)
                            result = InverseExclusionResult(rules[j], codePart.CompareTo(positiveRule.Replace(">=", string.Empty)) > 0);
                        else if (code is int)
                            result = InverseExclusionResult(rules[j], Convert.ToInt32(code) >= Convert.ToInt32(positiveRule.Replace(">=", string.Empty)));
                    }
                    // "code" - исключаются коды, равные указанному;
                    else
                        result = InverseExclusionResult(rules[j], codePart == positiveRule);
                    if (!result && rules[j].StartsWith("#"))
                        return result;
                    if (result && rules[j].StartsWith("#")) 
                        result = false;
                    if (result) 
                        break;
                }
                if (result) break;
            }
            return result;
        }

        /// <summary>
        /// Возвращает массив значений полей классификатора, являющихся подстроками значения элемента хмл
        /// </summary>
        /// <param name="attrValuesMapping">Список пар поле-количество_символов.
        /// Поле - имя поля. Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов, num..-1 - все оставшиеся символы, начиная с num))</param>
        /// <param name="clsCode">Код для поиска подстрок</param>
        /// <param name="defaultValue">Значение по умолчанию для пустых кодов</param>
        /// <returns>Массив значений подстрок</returns>
        private string[] GetFieldsValuesAsSubstring(string[] attrValuesMapping, string clsCode, string defaultValue)
        {
            if (attrValuesMapping == null) 
                return null;
            int startIndex = 0;
            int count = attrValuesMapping.GetLength(0);
            string[] fieldsMapping = new string[count];
            for (int j = 0; j < count; j += 2)
            {
                // Копируем в массив результата название поля
                fieldsMapping[j] = attrValuesMapping[j];
                fieldsMapping[j + 1] = GetFieldValueAsSubstring(attrValuesMapping[j + 1], clsCode, ref startIndex, defaultValue);
            }
            return fieldsMapping;
        }

        /// <summary>
        /// Возвращает массив значений полей классификатора, являющихся подстроками значения элемента хмл
        /// </summary>
        /// <param name="attrValuesMapping">Список количество_символов.
        /// Формат количество_символов:
        /// "num" - номер символа (-1 - все символы);
        /// "num:mask" - mask определяет количество символов, до которого будет дополнено справа нулями полученное значение;
        /// "num1;num2;..." - символы num1, num2...;
        /// "num1..num2" - интервал num1..num2 (возможно использование в п.2; 0..num - первые num символов,
        /// -1..num - последние num символов)</param>
        /// <param name="clsCode">Код для поиска подстрок</param>
        /// <param name="defaultValue">Значение по умолчанию для пустых кодов</param>
        /// <returns>Массив значений подстрок</returns>
        private string[] GetCodeValuesAsSubstring(string[] attrValuesMapping, string clsCode, string defaultValue)
        {
            if (attrValuesMapping == null || clsCode == string.Empty) 
                return null;
            int startIndex = 0;
            int count = attrValuesMapping.GetLength(0);
            string[] fieldsMapping = new string[count];
            for (int j = 0; j < count; j++)
                fieldsMapping[j] = GetFieldValueAsSubstring(attrValuesMapping[j], clsCode, ref startIndex, defaultValue);
            return fieldsMapping;
        }

        /// <summary>
        /// Возвращает значение подстроки кода. Формат описания подстроки см. в GetFieldsValuesAsSubstring.attrValuesMapping
        /// </summary>
        /// <param name="attrSubstring">Значение массива GetFieldsValuesAsSubstring.attrValuesMapping</param>
        /// <param name="clsCode">Код</param>
        /// <param name="startIndex">Индекс текущего символа кода</param>
        /// <param name="defaultValue">Значение по умолчанию для пустых кодов</param>
        /// <returns>Значение подстроки</returns>
        private string GetFieldValueAsSubstring(string attrSubstring, string clsCode, ref int startIndex, string defaultValue)
        {
            string result = string.Empty;
            string[] mask = attrSubstring.Split(':');
            if (mask.GetLength(0) == 0) 
                return defaultValue;
            string[] parts = mask[0].Split(';');
            int count = parts.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string[] intervals = parts[i].Split(new string[] { ".." }, StringSplitOptions.None);
                if (intervals.GetLength(0) == 0)
                    continue;
                else if (intervals.GetLength(0) == 1)
                {
                    if (intervals[0] == "-1")
                        result += clsCode.Substring(startIndex);
                    else
                    {
                        result += Convert.ToString(clsCode[Convert.ToInt32(intervals[0])]);
                        startIndex++;
                    }
                }
                else
                {
                    int lo = Convert.ToInt32(intervals[0]);
                    int hi = Convert.ToInt32(intervals[1]);
                    if (lo == -1)
                        result += clsCode.Substring(clsCode.Length - hi);
                    else if (hi == -1)
                    {
                        if (lo < clsCode.Length)
                            result += clsCode.Substring(lo);
                    }
                    else if (lo == 0)
                        result += clsCode.Substring(lo, hi);
                    else
                    {
                        if (hi >= clsCode.Length)
                            hi = clsCode.Length - 1;
                        result += clsCode.Substring(lo, hi - lo + 1);
                    }
                    startIndex = hi + 1;
                }
            }
            if (result == string.Empty) 
                return defaultValue;
            if (mask.GetLength(0) == 2)
                result = result.PadRight(mask[1].Length, '0');
            return result;
        }

        /// <summary>
        /// Проверяет, чтобы код пристуствовал во всех классификаторах
        /// </summary>
        /// <param name="fieldsMapping">Список пар поле_кода_классификатора-значение</param>
        /// <param name="codesMapping">Список пар код_в_хмл - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsCode">Код классификатора из хмл</param>
        /// <returns>true - Код найден во всех codesMapping, иначе в каком-то нет.</returns>
        private bool CheckClsIDByCode(object[] fieldsMapping, Dictionary<string, int>[] codesMapping, string clsCode)
        {
            int count = fieldsMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                if (codesMapping[i / 2].ContainsKey(clsCode))
                    fieldsMapping[i + 1] = codesMapping[i / 2][clsCode];
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет, чтобы коды пристуствовали во всех классификаторах
        /// </summary>
        /// <param name="codesMapping">Список пар код - ИД_записи_в_датасете для каждого классификатора</param>
        /// <param name="clsValuesMapping">Массив значений классификаторов</param>
        /// <returns>true - Код найден во всех codesMapping, иначе в каком-то нет.</returns>
        private bool CheckClsIDByCode(Dictionary<string, int>[] codesMapping, object[] clsValuesMapping)
        {
            int count = clsValuesMapping.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                string key = Convert.ToString(clsValuesMapping[i]);
                if (!codesMapping[i].ContainsKey(key))
                    return false;
            }
            return true;
        }

        private bool CheckCode(string code)
        {
            if (block != Block.bOutcomes)
                return true;
            string FKR = code.Substring(0, 4);
            if ((FKR == "9600") || (FKR == "9800"))
                return true;
            if (FKR.EndsWith("00"))
                return false;
            if (this.DataSource.Year >= 2005)
            {
                // экр
                if (code.Substring(14, 3).TrimStart('0') == string.Empty)
                    return false;
                // кцср
                if (code.Substring(4, 7).TrimStart('0') == string.Empty)
                    return false;
                // квр - c 2009 года квр может быть нулевым
                if (this.DataSource.Year < 2009)
                    if (code.Substring(11, 3).TrimStart('0') == string.Empty)
                        return false;
            }
            else
            {
                // экр
                if (code.Substring(13, 6).TrimStart('0') == string.Empty)
                    return false;
                // кцср
                if (code.Substring(7, 3).TrimStart('0') == string.Empty)
                    return false;
                // квр 
                if (code.Substring(10, 3).TrimStart('0') == string.Empty)
                    return false;
            }
            return true;
        }

        private void FormCLSFromReport(string code)
        {
            string fkr = string.Empty;
            string kcsr = string.Empty;
            string kvr = string.Empty;
            string ekr = string.Empty;
            switch (block)
            {
                case Block.bIncomes:
                    if (this.DataSource.Year >= 2005)
                    {
                        if (CheckCodeExclusion(code, new string[] { "!000*" }))
                            return;
                        PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code,
                            new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    }
                    else
                    {
                        code = code.TrimStart('0');
                        PumpCachedRow(kdCache, dsKD.Tables[0], clsKD, code, 
                            new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    }
                    break;
                case Block.bOutcomes:
                    if (this.DataSource.Year >= 2010)
                        break;
                    fkr = code.Substring(0, 4).TrimStart('0').PadLeft(1, '0');
                    PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkr, new object[] { "CODE", fkr, "NAME", "Неуказанное наименование" });
                    if (this.DataSource.Year >= 2005)
                    {
                        ekr = code.Substring(14, 3).TrimStart('0').PadLeft(1, '0');
                        PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, ekr, 
                            new object[] { "CODE", ekr, "NAME", "Неуказанное наименование" });
                        kcsr = code.Substring(4, 7).TrimStart('0').PadLeft(1, '0');
                        kvr = code.Substring(11, 3).TrimStart('0').PadLeft(1, '0');
                    }
                    else
                    {
                        ekr = code.Substring(13, 6).TrimStart('0').PadLeft(1, '0');
                        PumpCachedRow(ekrCache, dsEKR.Tables[0], clsEKR, ekr, 
                            new object[] { "CODE", ekr, "NAME", "Неуказанное наименование" });
                        kcsr = code.Substring(7, 3).PadRight(7, '0').TrimStart('0').PadLeft(1, '0');
                        kvr = code.Substring(10, 3).TrimStart('0').PadLeft(1, '0');
                    }
                    PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, kcsr, new object[] { "CODE", kcsr, "NAME", "Неуказанное наименование" });
                    PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, kvr, new object[] { "CODE", kvr, "NAME", "Неуказанное наименование" });
                    break;
                case Block.bFinSources:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(kifCache, dsKIF2005.Tables[0], clsKIF2005, code, new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    else
                        PumpCachedRow(kifCache, dsKIF2004.Tables[0], clsKIF2004, code, new object[] { "CODESTR", code, "NAME", "Неуказанное наименование", "KL", 0, "KST", 0 });
                    break;
                case Block.bNets:
                    fkr = code.Substring(0, 4).TrimStart('0').PadLeft(1, '0');
                    PumpCachedRow(fkrCache, dsFKR.Tables[0], clsFKR, fkr, new object[] { "CODE", fkr, "NAME", "Неуказанное наименование" });
                    kcsr = string.Empty;
                    kvr = string.Empty;
                    if (this.DataSource.Year >= 2005)
                    {
                        kcsr = code.Substring(4, 7).TrimStart('0').PadLeft(1, '0');
                        kvr = code.Substring(11, 3).TrimStart('0').PadLeft(1, '0');
                    }
                    else
                    {
                        kcsr = code.Substring(7, 3).PadRight(7, '0').TrimStart('0').PadLeft(1, '0');
                        kvr = code.Substring(10, 3).TrimStart('0').PadLeft(1, '0');
                    }
                    PumpCachedRow(kcsrCache, dsKCSR.Tables[0], clsKCSR, kcsr, new object[] { "CODE", kcsr, "NAME", "Неуказанное наименование" });
                    PumpCachedRow(kvrCache, dsKVR.Tables[0], clsKVR, kvr, new object[] { "CODE", kvr, "NAME", "Неуказанное наименование" });
                    string kshk = code.Substring(code.Length - 3, 3);
                    PumpCachedRow(kshkCache, dsKSHK.Tables[0], clsKSHK, kshk, new object[] { "CODE", kshk, "NAME", "Неуказанное наименование" });
                    break;
            }
        }

        private void SetNetsSumFactor(int kshk)
        {
            if (this.DataSource.Year >= 2005)
            {
                if ((kshk == 850) || ((kshk >= 500) && (kshk < 700)))
                    sumFactor = 1000;
                else
                    sumFactor = 1;
            }
            else
            {
                if ((kshk >= 800) || ((kshk >= 500) && (kshk < 700)))
                    sumFactor = 1000;
                else
                    sumFactor = 1;
            }
        }

        #endregion Общие методы

        #region Перекрытые методы закачки

        /// <summary>
        /// Закачивает файлы из указанного каталога
        /// </summary>
        /// <param name="dir">Каталог</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            PumpXMLReports(dir);
            PumpDBFReports(dir);
            SetClsHierarchy();
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            xmlFilesCount = this.RootDir.GetFiles("*.xml", SearchOption.AllDirectories).GetLength(0);
            dbfFilesCount = this.RootDir.GetFiles("*.dbf", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных
        
        /// <summary>
        /// Этап обработки данных
        /// </summary>
        protected override void DirectProcessData()
        {
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Коррекции сумм фактов по данным источника");
        }

        /// <summary>
        /// Функция запроса данных из базы
        /// </summary>
        protected override void QueryDataForProcess()
        {
            InitDBObjects();
            QueryData();
            InitDataSet(ref daAnalFKR, ref dsAnalFKR, clsAnalFKR, false, string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);
            FillRowsCache(ref analFKRCache, dsAnalFKR.Tables[0], "CODE", "NAME");
            InitDataSet(ref daAnalKCSR, ref dsAnalKCSR, clsAnalKCSR, false, string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);
            FillRowsCache(ref analKCSRCache, dsAnalKCSR.Tables[0], "CODE", "NAME");
            InitDataSet(ref daAnalKVR, ref dsAnalKVR, clsAnalKVR, false, string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);
            FillRowsCache(ref analKVRCache, dsAnalKVR.Tables[0], "CODE", "NAME");
            InitDataSet(ref daIncomes, ref dsIncomes, fctIncomes, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            InitDataSet(ref daOutcomes, ref dsOutcomes, fctOutcomes, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            InitDataSet(ref daDefProf, ref dsDefProf, fctDefProf, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            InitDataSet(ref daFinSources, ref dsFinSources, fctFinSources, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
        }

        /// <summary>
        /// Функция сохранения закачанных данных в базу
        /// </summary>
        protected override void UpdateProcessedData()
        {
            UpdateData();
        }

        /// <summary>
        /// Функция выполнения завершающих действий закачки
        /// </summary>
        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
            ClearDataSet(ref dsAnalFKR);
            ClearDataSet(ref dsAnalKCSR);
            ClearDataSet(ref dsAnalKVR);
            warnedRegions.Clear();
        }

        private void SetClsNames(ref DataSet ds, Dictionary<string, string> cache, bool IsFKR)
        {
            int count = ds.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = ds.Tables[0].Rows[i];
                string code = row["CODE"].ToString().TrimStart('0');
                if (code == string.Empty) 
                    continue;
                bool ToSetName = false;
                ToSetName = (row["NAME"].ToString().ToUpper() == "НЕУКАЗАННОЕ НАИМЕНОВАНИЕ");
                // для фкр разыменуем для секций 1 - 31 (XX.00)
                int codeStart = Convert.ToInt32(code.PadLeft(4, '0').Substring(0, 2));
                if (IsFKR && code.EndsWith("00") && codeStart >= 1 && codeStart <= 31)
                    ToSetName = true;
                if (!cache.ContainsKey(code))
                    continue;
                if (ToSetName)
                    row["Name"] = cache[code].ToString();
            }
        }

        private void SetClssNames()
        {
            SetClsNames(ref dsFKR, analFKRCache, true);
            SetClsNames(ref dsKCSR, analKCSRCache, false);
            SetClsNames(ref dsKVR, analKVRCache, false);
        }

        private void SetRegionDocumentType()
        {
            // заполняем тип документа у районов, берем его из районов для закачки скиф
            int count = dsRegions.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dsRegions.Tables[0].Rows[i];
                string code = row["CODE"].ToString().PadLeft(10, '0');
                string name = row["NAME"].ToString();
                string key = code + "|" + name;
                if (!regionForPumpCache.ContainsKey(key))
                    continue;
                int docType = regionForPumpCache[key];
                switch (docType) 
                {
                    case 1:
                        if (!warnedRegions.Contains(code))
                        {
                            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                                "Значение признака 'Тип документа.СКИФ' для района '{0}' (код {1}) равно {2} ({3}). " +
                                "По данному району не будут откорректированы суммы по уровням бюджетов.",
                                name, code, docType, GetSKIFDocType(docType)));
                            warnedRegions.Add(code);
                        }
                        row["REFDOCTYPE"] = docType.ToString();
                        break;
                    case 20:
                    case 21:
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                            "Значение признака 'Тип документа.СКИФ' для района '{0}' (код {1}) равно {2} ({3}). " +
                            "Данный тип документа является аналитическим признаком и не подлежит использованию. " +
                            "Для коррекции сумм по уровням бюджетов заново заполните ТипДокумента.СКИФ для указанного района и запустите этап обработки.",
                            name, code, docType, GetSKIFDocType(docType)));
                        break;
                    default:
                        row["REFDOCTYPE"] = docType.ToString();
                        break;
                }
            }
        }

        private void SetDataSetBudgetLevel(ref DataSet ds)
        {
            // заполняем уровень бюджета у таблицы фактов, уровень определяется типом документа района
            int count = ds.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = ds.Tables[0].Rows[i];
                int budgetLevel = Convert.ToInt32(row["REFBDGTLEVELS"]);
                if ((budgetLevel == 1) || (budgetLevel == 2))
                    continue;
                int regionId = Convert.ToInt32(row["REFREGIONS"]);
                DataRow[] region = dsRegions.Tables[0].Select(string.Format("ID = {0}", regionId));
                string docType = region[0]["REFDOCTYPE"].ToString();
                if (docType == string.Empty)
                    continue;
                switch (Convert.ToInt32(docType))
                {
                    case 2:
                        budgetLevel = 9;
                        break;
                    case 5:
                        budgetLevel = 3;
                        break;
                    case 6:
                        budgetLevel = 5;
                        break;
                    case 7:
                        budgetLevel = 4;
                        break;
                    case 8:
                        budgetLevel = 6;
                        break;
                    case 9:
                        budgetLevel = 9;
                        break;
                    case 10:
                        budgetLevel = 10;
                        break;
                }
                row["REFBDGTLEVELS"] = budgetLevel.ToString();
            }
        }

        private void SetBudgetLevel()
        {
            if (this.DataSource.Year >= 2006)
                return;
            SetDataSetBudgetLevel(ref dsIncomes);
            SetDataSetBudgetLevel(ref dsOutcomes);
            SetDataSetBudgetLevel(ref dsDefProf);
            SetDataSetBudgetLevel(ref dsFinSources);
        }

        private void AddParentRecords()
        {
            int count = dsOutcomes.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dsOutcomes.Tables[0].Rows[i];
                string parentEKR = string.Empty;
                string refCurEKRFieldName = string.Empty;
                // получаем родительский экр
                refCurEKRFieldName = "RefEKRFOProj";
                parentEKR = dsEKR.Tables[0].Select(string.Format("ID = {0}", row[refCurEKRFieldName].ToString()))[0]["PARENTID"].ToString();
                if (parentEKR == string.Empty)
                    continue;
                // ищем родительскую запись (все поля кроме экр - такие же)
                string refKCSRValue = row["REFKCSR"].ToString();
                string refFKRValue = row["REFFKR"].ToString();
                string refKVRValue = row["REFKVR"].ToString();
                string refYearValue = row["REFYEARDAYUNV"].ToString();
                string refBudgetLevel = row["REFBDGTLEVELS"].ToString();
                string refRegions = row["REFREGIONS"].ToString();
                DataRow[] parentRecord = dsOutcomes.Tables[0].Select(
                    string.Format(refCurEKRFieldName + " = {0} and REFKCSR = {1} and REFFKR = {2} and REFKVR = {3} " + 
                    "and REFYEARDAYUNV = {4} and REFBDGTLEVELS = {5} and REFREGIONS = {6}",
                    parentEKR, refKCSRValue, refFKRValue, refKVRValue, refYearValue, refBudgetLevel, refRegions));
                if (parentRecord.Length != 0)
                    continue;
                // добавляем родительскую запись
                object[] mapping = new object[] { refCurEKRFieldName, parentEKR, "REFKCSR", refKCSRValue, 
                    "REFFKR", refFKRValue, "REFKVR", refKVRValue, "REFYEARDAYUNV", refYearValue, "REFBDGTLEVELS", 
                    refBudgetLevel, "REFREGIONS", refRegions, "ASSIGNEDREPORT", "0", "ASSIGNED", "0"};
                PumpRow(dsOutcomes.Tables[0], mapping);
            }
        }

        private void CorrectSums()
        {
            YRSumCorrectionConfig yrSumCorrectionConfig = new YRSumCorrectionConfig();
            yrSumCorrectionConfig.AssignedField = "ASSIGNED";
            yrSumCorrectionConfig.AssignedReportField = "ASSIGNEDREPORT";
            yrSumCorrectionConfig.PerformedField = string.Empty;
            yrSumCorrectionConfig.PerformedReportField = string.Empty;
            // Доходы
            CorrectFactTableSums(fctIncomes, dsKD.Tables[0], clsKD, "RefKDFOProj", yrSumCorrectionConfig,
                BlockProcessModifier.YRStandard, new string[] { "RefYearDayUNV" }, "REFREGIONS", "REFBDGTLEVELS");
            // Расходы
            CorrectFactTableSums(fctOutcomes, dsEKR.Tables[0], clsEKR, "RefEKRFOProj", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes, 
                new string[] { "REFFKR", "REFKCSR", "REFKVR", "RefYearDayUNV" }, "REFREGIONS", "REFBDGTLEVELS", true);
            CorrectFactTableSums(fctOutcomes, dsKCSR.Tables[0], clsKCSR, "REFKCSR", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes,
                new string[] { "REFFKR", "REFKVR", "RefEKRFOProj", "RefYearDayUNV" }, "REFREGIONS", "REFBDGTLEVELS", false);
            // ДефицитПрофицит
            TransferSourceSums(fctDefProf, yrSumCorrectionConfig);
            // Источники финансирования
            if (this.DataSource.Year >= 2005)
                CorrectFactTableSums(fctFinSources, dsKIF2005.Tables[0], clsKIF2005, "REFKIF", yrSumCorrectionConfig, BlockProcessModifier.YRStandard,
                    new string[] { "RefYearDayUNV" }, "REFREGIONS", "REFBDGTLEVELS");
            else
                CorrectFactTableSums(fctFinSources, dsKIF2004.Tables[0], clsKIF2004, "REFKIFFOPROJ2004", yrSumCorrectionConfig, BlockProcessModifier.YRStandard,
                    new string[] { "RefYearDayUNV" }, "REFREGIONS", "REFBDGTLEVELS");
        }

        #region заполнение классификатора "Расходы"

        private string GetOutcomesClsCode(string fkrCode, string kcsrCode, string kvrCode)
        {
            return string.Format("{0}{1}{2}", fkrCode, kcsrCode, kvrCode).TrimStart('0').PadLeft(1, '0');
        }

        private int PumpOutcomesClsRow(string fkrCode, string kcsrCode, string kvrCode, string name)
        {
            string outcomesClsCode = GetOutcomesClsCode(fkrCode, kcsrCode, kvrCode);
            object[] mapping = new object[] { "Code", outcomesClsCode, "Name", name };
            string cacheKey = outcomesClsCode;
            return PumpCachedRow(outcomesClsCache, dsOutcomesCls.Tables[0], clsOutcomesCls, cacheKey, mapping);
        }

        private string GetOutcomesClsName(string[] codes, string[] names)
        {
            string name = constDefaultClsName;
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                string code = codes[i];
                if (code.TrimStart('0') != string.Empty)
                {
                    name = names[i];
                    break;
                }
            }
            return name;
        }

        private void PumpOutcomesClsRows(DataRow factRow)
        {
            DataRow clsRow = FindCachedRow(fkrRowsCache, Convert.ToInt32(factRow["RefFKR"]));
            if (clsRow == null)
            {
                factRow["RefR"] = nullOutcomesCls;
                return;
            }
            string fkrCode = clsRow["Code"].ToString().PadLeft(4, '0');
            string fkrName = clsRow["Name"].ToString();

            clsRow = FindCachedRow(kcsrRowsCache, Convert.ToInt32(factRow["RefKCSR"]));
            if (clsRow == null)
            {
                factRow["RefR"] = nullOutcomesCls;
                return;
            }
            string kcsrCode = clsRow["Code"].ToString().PadLeft(7, '0');
            string kcsrName = clsRow["Name"].ToString();

            clsRow = FindCachedRow(kvrRowsCache, Convert.ToInt32(factRow["RefKVR"]));
            if (clsRow == null)
            {
                factRow["RefR"] = nullOutcomesCls;
                return;
            }
            string kvrCode = clsRow["Code"].ToString().PadLeft(3, '0');
            string kvrName = clsRow["Name"].ToString();

            // добавляем запись с заполненным фкр кцср и квр, наименование берем у последнего ненулевого кода
            string outcomesClsName = GetOutcomesClsName(new string[] { kvrCode, kcsrCode, fkrCode },
                                                        new string[] { kvrName, kcsrName, fkrName });

            factRow["RefRFOProject"] = PumpOutcomesClsRow(fkrCode, kcsrCode, kvrCode, outcomesClsName);

            UpdateProcessedData();
        }

        private void FillOutcomesCls()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                string.Format("Начало формирования классификатора '{0}'", clsOutcomesCls.FullCaption));

            object[] mapping = new object[] { "Code", 0, "Name", constDefaultClsName };
            defaultOutcomesCls = PumpCachedRow(outcomesClsCache, dsOutcomesCls.Tables[0], clsOutcomesCls, "0", mapping);

            FillRowsCache(ref fkrRowsCache, dsFKR.Tables[0], "Id");
            FillRowsCache(ref kcsrRowsCache, dsKCSR.Tables[0], "Id");
            FillRowsCache(ref kvrRowsCache, dsKVR.Tables[0], "Id");

            try
            {
                PartialDataProcessingTemplate(fctOutcomes, string.Empty, MAX_DS_RECORDS_AMOUNT, new DataPartRowProcessing(PumpOutcomesClsRows),
                    string.Format("формирование классификатора '{0}'", clsOutcomesCls.FullCaption));
            }
            finally
            {
                fkrRowsCache.Clear();
                kcsrRowsCache.Clear();
                kvrRowsCache.Clear();
            }
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                string.Format("Завершение формирования классификатора '{0}'", clsOutcomesCls.FullCaption));
        }

        #endregion заполнение классификатора "Расходы"

        /// <summary>
        /// Функция коррекции сумм фактов по данным источника
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        protected override void ProcessDataSource()
        {
            // установка иерархии
            SetClsHierarchy();
            UpdateData();
            // разыменовка классификаторов
            SetClssNames(); 
            UpdateProcessedData();
            // устанавливаем тип документа
            SetRegionDocumentType();
            UpdateProcessedData();
            // устанавливаем уровень бюджета
            SetBudgetLevel();
            UpdateProcessedData();
            // дополнительная обработка расходов (добавляем родительские записи, если их нет)
            AddParentRecords();
            UpdateProcessedData();
            // коррекция сумм
            CorrectSums();
            UpdateProcessedData();
            // формирование расходного клс
            FillOutcomesCls();
            UpdateProcessedData();
        }

        #endregion Обработка данных
        
    }
}
