using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.DataPumps.Common;

namespace Krista.FM.Server.DataPumps.SKIFYearRepPump
{
    // Главный модуль закачки. Содержит реализацию функций закачки

    /// <summary>
    /// ФО_0005_Ежегодные отчеты.
    /// Закачка данных СКИФ
    /// </summary>
    public partial class SKIFYearRepPumpModule : SKIFRepPumpModuleBase
    {
        #region Поля

        #region Факты

        // ДефицитПрофицит.ФО_ГодОтч_Дефицит Профицит (f_DP_FOYRDefProf)
        private IDbDataAdapter daFOYRDefProf;
        private DataSet dsFOYRDefProf;
        private IFactTable fctFOYRDefProf;
        // Доходы.ФО_ГодОтч_Доходы (f_D_FOYRIncomes)
        private IDbDataAdapter daFOYRIncomes;
        private DataSet dsFOYRIncomes;
        private IFactTable fctFOYRIncomes;
        // ИсточникиФинансирования.ФО_ГодОтч_Источники финансирования (f_SrcFin_FOYRSrcFin)
        private IDbDataAdapter daFOYRSrcFin;
        private DataSet dsFOYRSrcFin;
        private IFactTable fctFOYRSrcFin;
        // Факт.ФО_ГодОтч_Недостачи Хищения (f_F_FOYREmbezzles)
        private IDbDataAdapter daFOYREmbezzles;
        private DataSet dsFOYREmbezzles;
        private IFactTable fctFOYREmbezzles;
        // Расходы.ФО_ГодОтч_Расходы (f_R_FOYROutcomes)
        private IDbDataAdapter daFOYROutcomes;
        private DataSet dsFOYROutcomes;
        private IFactTable fctFOYROutcomes;
        // CШК.ФО_ГодОтч (f_Net_FOYR)
        private IDbDataAdapter daFOYRNet;
        private DataSet dsFOYRNet;
        private IFactTable fctFOYRNet;
        // Факт.ФО_ГодОтч_Баланс (f_F_FOYRBalanc)
        private IDbDataAdapter daFOYRBalanc;
        private DataSet dsFOYRBalanc;
        private IFactTable fctFOYRBalanc;
        // Факт.ФО_ГодОтч_Баланс_Справка (f_F_FOYRBalOff)
        private IDbDataAdapter daFOYRBalOff;
        private DataSet dsFOYRBalOff;
        private IFactTable fctFOYRBalOff;

        #endregion Факты

        #region Классификаторы

        // Районы.ФО_ГодОтч (d_Regions_FOYR)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        private int nullRegions;
        // КД.ФО_ГодОтч (d_KD_FOYR)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        private int nullKD;
        // КИФ.ФО_ГодОтч 2004 (d_KIF_FOYR2004)
        private IDbDataAdapter daKIF2004;
        private DataSet dsKIF2004;
        private IClassifier clsKIF2004;
        private Dictionary<string, int> kifCache = null;
        private int nullKIF2004;
        // КИФ.ФО_ГодОтч 2005 (d_KIF_FOYR2005)
        private IDbDataAdapter daKIF2005;
        private DataSet dsKIF2005;
        private IClassifier clsKIF2005;
        private int nullKIF2005;
        // Показатели.ФО_ГодОтч_Хищения (fx_Marks_FOYREmbezzles)
        private IDbDataAdapter daMarksEmbezzles;
        private DataSet dsMarksEmbezzles;
        private IClassifier fxcMarksEmbezzles;
        private Dictionary<string, int> marksEmbezzlesCache = null;
        // Тип средств.ФО_ГодОтч_Хищения (fx_MeansType_FOYREmbezzles)
        private IDbDataAdapter daMeansType;
        private DataSet dsMeansType;
        private IClassifier fxcMeansType;
        private Dictionary<string, int> meansTypeCache = null;
        // КВР.ФО_ГодОтч (d_KVR_FOYR)
        private IDbDataAdapter daKVR;
        private DataSet dsKVR;
        private IClassifier clsKVR;
        private Dictionary<string, int> kvrCache = null;
        private Dictionary<int, DataRow> kvrRowsCache = null;
        private int nullKVR;
        // КЦСР.ФО_ГодОтч (d_KCSR_FOYR)
        private IDbDataAdapter daKCSR;
        private DataSet dsKCSR;
        private IClassifier clsKCSR;
        private Dictionary<string, int> kcsrCache = null;
        private Dictionary<int, DataRow> kcsrRowsCache = null;
        private int nullKCSR;
        // РзПр.ФО_ГодОтч (d_FKR_FOYR)
        private IDbDataAdapter daFKR;
        private DataSet dsFKR;
        private IClassifier clsFKR;
        private Dictionary<string, int> fkrCache = null;
        private Dictionary<int, DataRow> fkrRowsCache = null;
        private int nullFKR;
        // КОСГУ.ФО_ГодОтч (d_EKR_FOYR)
        private IDbDataAdapter daEKR;
        private DataSet dsEKR;
        private IClassifier clsEKR;
        private Dictionary<string, int> ekrCache = null;
        private int nullEKR;
        // КСШК.ФО_ГодОтч (d_KSSHK_FOYR)
        private IDbDataAdapter daMarksNet;
        private DataSet dsMarksNet;
        private IClassifier clsMarksNet;
        private Dictionary<string, int> marksNetCache = null;
        private int nullMarksNet;
        // Районы.Служебный для закачки СКИФ (d_Regions_ForPumpSKIF)
        private IDbDataAdapter daRegions4Pump;
        private DataSet dsRegions4Pump;
        private IClassifier clsRegions4Pump;
        private Dictionary<string, int> region4PumpCache = null;
        // Показатели.ФО_ГодОтч_СубКВР (d_Marks_FOYRSKVR)
        private IDbDataAdapter daMarksSubKvr;
        private DataSet dsMarksSubKvr;
        private IClassifier clsMarksSubKvr;
        private Dictionary<string, int> marksSubKvrCache = null;
        private int nullMarksSubKvr;
        // Расходы.ФО_ГодОтч (d_R_FOYR)
        private IDbDataAdapter daOutcomesCls;
        private DataSet dsOutcomesCls;
        private IClassifier clsOutcomesCls;
        private Dictionary<string, int> outcomesClsCache = null;
        private int nullOutcomesCls;
        private int defaultOutcomesCls;
        // Администратор.ФО_ГодОтч (d_KVSR_FOYR)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<string, int> kvsrCache = null;
        private int nullKvsr;
        // ПланСчетов.МесОтч (d_Account_MonthRep)
        private IDbDataAdapter daAccount;
        private DataSet dsAccount;
        private IClassifier clsAccount;
        private Dictionary<string, int> accountCache = null;
        private int nullAccount;
        // Показатели.ФО_Забалансовые счета (d_Marks_FOYRBalOff)
        private IDbDataAdapter daMarksBalOff;
        private DataSet dsMarksBallOff;
        private IClassifier clsMarksBallOff;
        private Dictionary<string, int> marksBallOffCache;
        private int nullMarksBallOff;

        #endregion Классификаторы

        private bool toPumpIncomes;
        private bool toPumpOutcomes;
        private bool toPumpDefProf;
        private bool toPumpFinSources;
        private bool toPumpNet;

        private bool hasBalancBlock;

        #endregion Поля

        #region Константы

        private enum Block
        {
            bIncomes,
            bOutcomes,
            bDefProf,
            bFinSources,
            bNet,
            bRegions,
            bBalanc
        }

        #endregion Константы

        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public SKIFYearRepPumpModule()
            : base()
		{

        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #endregion Инициализация

        #region Закачка данных

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
                case Block.bNet:
                    return "ucbNet";
                case Block.bBalanc:
                    return "ucbBalanc";
                default:
                    return string.Empty;
            }
        }

        private bool ToPumpBlock(Block block)
        {
            string configParamName = GetConfigParamName(block);
            return (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, configParamName, "False")));
        }

        private void GetPumpedBlocks()
        {
            toPumpIncomes = ToPumpBlock(Block.bIncomes);
            toPumpOutcomes = ToPumpBlock(Block.bOutcomes);
            toPumpDefProf = ToPumpBlock(Block.bDefProf);
            toPumpFinSources = ToPumpBlock(Block.bFinSources);
            toPumpNet = ToPumpBlock(Block.bNet);
        }

        protected override void DeleteEarlierPumpedData()
        {
            if (!this.DeleteEarlierData)
                return;
            if (ToPumpBlock(Block.bIncomes))
            {
                DirectDeleteFactData(new IFactTable[] { fctFOYRIncomes }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKD }, -1, this.SourceID, string.Empty);
            }
            if (ToPumpBlock(Block.bOutcomes))
            {
                DirectDeleteFactData(new IFactTable[] { fctFOYROutcomes }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsEKR, clsKvsr }, -1, this.SourceID, string.Empty);
            }
            if (ToPumpBlock(Block.bDefProf))
            {
                DirectDeleteFactData(new IFactTable[] { fctFOYRDefProf }, -1, this.SourceID, string.Empty);
            }
            if (ToPumpBlock(Block.bFinSources))
            {
                DirectDeleteFactData(new IFactTable[] { fctFOYRSrcFin }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKIF2004, clsKIF2005 }, -1, this.SourceID, string.Empty);
            }
            if (ToPumpBlock(Block.bNet))
            {
                DirectDeleteFactData(new IFactTable[] { fctFOYRNet, fctFOYREmbezzles }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksNet }, -1, this.SourceID, string.Empty);
            }
            if (ToPumpBlock(Block.bNet) && ToPumpBlock(Block.bOutcomes))
            {
                DirectDeleteClsData(new IClassifier[] { clsKVR, clsKCSR, clsFKR, clsOutcomesCls }, -1, this.SourceID, string.Empty);
            }
            if (ToPumpBlock(Block.bBalanc) && hasBalancBlock)
            {
                DirectDeleteFactData(new IFactTable[] { fctFOYRBalanc, fctFOYRBalOff }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsAccount, clsMarksBallOff }, -1, this.SourceID, string.Empty);
            }

            // районы удаляем, если закачиваются все блоки
            if (ToPumpBlock(Block.bIncomes) && ToPumpBlock(Block.bOutcomes) && ToPumpBlock(Block.bDefProf) &&
                ToPumpBlock(Block.bFinSources) && ToPumpBlock(Block.bNet) && ToPumpBlock(Block.bBalanc))
                DirectDeleteClsData(new IClassifier[] { clsRegions }, -1, this.SourceID, string.Empty);
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
                    "RefKVR", fctFOYRNet.FullDBName, this.SourceID);
            if (cls == clsKCSR)
                query += string.Format(" and (id not in (select distinct {0} from {1} where sourceid = {2}))",
                    "RefKCSR", fctFOYRNet.FullDBName, this.SourceID);

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
        protected override void SetClsHierarchy()
        {
            DeleteUnusedClsRecords(fctFOYRIncomes, ref clsKD, ref daKD, ref dsKD, "refKd");
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
                // Доходы
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                    "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                    "CodeStr = '00087100000000000000' or CodeStr = '00087200000000000000'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010102000010000110'",
                    "CodeStr = '00010102011010000110'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205800090000151'",
                    "CodeStr = '00020205810090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                    "CodeStr = '00010202041060000160'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                    "CodeStr = '00011706011060000180'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                    "CodeStr = '00011105026000000120'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011402000000000000'",
                    "CodeStr = '00011402022020000410' or CodeStr = '00011402023020000410' or " +
                    "CodeStr = '00011402022020000440' or CodeStr = '00011402023020000440' or " +
                    "CodeStr = '00011402032030000410' or CodeStr = '00011402033030000410' or " +
                    "CodeStr = '00011402032030000440' or CodeStr = '00011402033030000440' or " +
                    "CodeStr = '00011402032040000410' or CodeStr = '00011402033040000410' or " +
                    "CodeStr = '00011402032040000440' or CodeStr = '00011402033040000440' or " +
                    "CodeStr = '00011402032050000410' or CodeStr = '00011402033050000410' or " +
                    "CodeStr = '00011402032050000440' or CodeStr = '00011402033050000440' or " +
                    "CodeStr = '00011402032100000410' or CodeStr = '00011402032100000410' or " +
                    "CodeStr = '00011402032100000440' or CodeStr = '00011402033100000440' or " +
                    "CodeStr = '00011402033100000410' or CodeStr = '00011402011010000410' or " +
                    "CodeStr = '00011402013010000410' or CodeStr = '00011402014010000410' or " +
                    "CodeStr = '00011402015010000410' or CodeStr = '00011402016010000410' or " +
                    "CodeStr = '00011402017010000410' or CodeStr = '00011402019010000410' or " +
                    "CodeStr = '00011402011010000440' or CodeStr = '00011402013010000440' or " +
                    "CodeStr = '00011402014010000440' or CodeStr = '00011402015010000440' or " +
                    "CodeStr = '00011402016010000440' or CodeStr = '00011402017010000440' or " +
                    "CodeStr = '00011402018010000440' or CodeStr = '00011402019010000440'");
                if (this.DataSource.Year >= 2009)
                {
                    FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                        "CodeStr = '00087000000000001151' or CodeStr = '00087000000000002151'");
                }
            }
            else if (this.DataSource.Year >= 2007)
            {
                // Доходы
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
                    "CodeStr = '00020204906090000151' or CodeStr = '00020204908090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151'");
            }
            else if (this.DataSource.Year >= 2006)
            {
                // Доходы
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);
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
                // Доходы
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);
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
            {
                // Доходы
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);
            }

            // Источники финансирования
            DeleteUnusedClsRecords(fctFOYRSrcFin, ref clsKIF2005, ref daKIF2005, ref dsKIF2005, "RefKIF2005");
            if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsKIF2005, clsKIF2005, null, string.Empty, ClsHierarchyMode.Standard);
            else
                SetClsHierarchy(ref dsKIF2004, clsKIF2004, null, string.Empty, ClsHierarchyMode.Standard);

            if (this.DataSource.Year >= 2008)
            {
                FormClsGroupHierarchy(dsKIF2005.Tables[0], string.Empty, "CodeStr = '00001050000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '00001000000000000000'",
                    "CodeStr = '0000105000000000000A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105000000000000A'",
                    "CodeStr = '0000105000000000050A' or CodeStr = '0000105000000000060A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105000000000050A'",
                    "CodeStr = '0000105010000000050A' or CodeStr = '0000105020000000050A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105000000000060A'",
                    "CodeStr = '0000105010000000060A' or CodeStr = '0000105020000000060A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105010000000050A'",
                    "CodeStr = '00001050102000000520'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105020000000050A'",
                    "CodeStr = '00001050202000000520'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105010000000060A'",
                    "CodeStr = '00001050102000000620'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000105020000000060A'",
                    "CodeStr = '00001050202000000620'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И5000000000000000' or CodeStr = '000И8000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И5000000000000000'",
                    "CodeStr = '000И6000000000000000' or CodeStr = '000И7000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И8000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
            }
            else
            {
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '00050000000000000000'",
                    "CodeStr = '00001010000000000000' or CodeStr = '00002010000000000000' or " +
                    "CodeStr = '00005000000000000000' or CodeStr = '00003010000000000000' or " +
                    "CodeStr = '00004010000000000000' or CodeStr = '00005000000000000000' or " +
                    "CodeStr = '00006000000000000000' or CodeStr = '00007000000010000000' or " +
                    "CodeStr = '0000800000000000000A' or CodeStr = '00009000000000000171' or " +
                    "CodeStr = '00010000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000801000000000050A'",
                    "CodeStr = '00008010200000000520'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '00008010200100000520'",
                    "CodeStr = '0000802000000000050A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000801000000000060A'",
                    "CodeStr = '00008010200000000620'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000802000000000060A'",
                    "CodeStr = '00008020200000000620'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000801000000000050A'",
                    "CodeStr = '00008010200000000520'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000802000000000050A'",
                    "CodeStr = '00008020200000000520'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000800000000000000A'",
                    "CodeStr = '0000800000000000050A' or CodeStr = '0000800000000000060A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000800000000000050A'",
                    "CodeStr = '0000801000000000050A' or CodeStr = '0000802000000000050A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000801000000000050A'",
                    "CodeStr = '00008010200000000520'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '00050000000000000000'",
                    "CodeStr = '0000801000000000060A' or CodeStr = '0000802000000000060A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '0000800000000000060A'",
                    "CodeStr = '0000801000000000060A' or CodeStr = '0000802000000000060A'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И5000000000000000' or CodeStr = '000И8000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И5000000000000000'",
                    "CodeStr = '000И6000000000000000' or CodeStr = '000И7000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '000И8000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsKIF2005.Tables[0], "CodeStr = '00057000000000000000'",
                    "CodeStr = '00050000000000000710' or CodeStr = '00050000000000000810'");
            }
            // Расходы
            DeleteUnusedClsRecords(fctFOYROutcomes, ref clsKVR, ref daKVR, ref dsKVR, "RefKVR");
            DeleteUnusedClsRecords(fctFOYROutcomes, ref clsKCSR, ref daKCSR, ref dsKCSR, "RefKCSR");

            SetClsHierarchy(ref dsKVR, clsKVR, null, string.Empty, ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsKCSR, clsKCSR, null, string.Empty, ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsFKR, clsFKR, null, string.Empty, ClsHierarchyMode.Standard);

            SetPresentationContext(clsEKR);
            SetClsHierarchy(ref dsEKR, clsEKR, null, string.Empty, ClsHierarchyMode.Standard);

            // районы служебный
            SetClsHierarchy(ref dsRegions4Pump, clsRegions4Pump, null, string.Empty, ClsHierarchyMode.Standard);
            // квср
            SetClsHierarchy(ref dsKvsr, clsKvsr, null, string.Empty, ClsHierarchyMode.Standard);
            // ксшк
            DeleteUnusedClsRecords(fctFOYRNet, ref clsMarksNet, ref daMarksNet, ref dsMarksNet, "RefKSSHK");
        }

        /// <summary>
        /// Заполняет кэш классификаторов
        /// </summary>
        private void FillCache()
        {
            FillRegionsCache(ref regionCache, dsRegions.Tables[0], "ID");
            FillRegionsCache(ref region4PumpCache, dsRegions4Pump.Tables[0], "REFDOCTYPE");

            FillRowsCache(ref fkrCache, dsFKR.Tables[0], "CODE");
            FillRowsCache(ref kcsrCache, dsKCSR.Tables[0], "CODE");
            FillRowsCache(ref marksNetCache, dsMarksNet.Tables[0], "Code");
            FillRowsCache(ref marksSubKvrCache, dsMarksSubKvr.Tables[0], "CodeRprt");
            FillRowsCache(ref marksEmbezzlesCache, dsMarksEmbezzles.Tables[0], "CODE");
            FillRowsCache(ref meansTypeCache, dsMeansType.Tables[0], "CODE");
            FillRowsCache(ref kvrCache, dsKVR.Tables[0], "CODE");
            FillRowsCache(ref ekrCache, dsEKR.Tables[0], "CODE");
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref kvsrCache, dsKvsr.Tables[0], "CODE");

            if (this.DataSource.Year >= 2005)
                FillRowsCache(ref kifCache, dsKIF2005.Tables[0], "CODESTR");
            else
                FillRowsCache(ref kifCache, dsKIF2004.Tables[0], "CODESTR");

            FillRowsCache(ref outcomesClsCache, dsOutcomesCls.Tables[0], new string[] { "CODE", "ParentId" }, "|", "Id");
            FillRowsCache(ref accountCache, dsAccount.Tables[0], "CODE");
            if (hasBalancBlock)
                FillRowsCache(ref marksBallOffCache, dsMarksBallOff.Tables[0], "CODE");
        }

        /// <summary>
        /// Запрос данных из базы
        /// </summary>
        protected override void QueryData()
        {
            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR, false, string.Empty);
            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR, false, string.Empty);
            InitClsDataSet(ref daKCSR, ref dsKCSR, clsKCSR, false, string.Empty);
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daKIF2004, ref dsKIF2004, clsKIF2004, false, string.Empty);
            InitClsDataSet(ref daKIF2005, ref dsKIF2005, clsKIF2005, false, string.Empty);
            InitClsDataSet(ref daKVR, ref dsKVR, clsKVR, false, string.Empty);
            InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr, false, string.Empty);
            InitDataSet(ref daMarksEmbezzles, ref dsMarksEmbezzles, fxcMarksEmbezzles, true, string.Empty, string.Empty);
            InitClsDataSet(ref daMarksNet, ref dsMarksNet, clsMarksNet, false, string.Empty);
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls, false, string.Empty);
            InitClsDataSet(ref daAccount, ref dsAccount, clsAccount, false, string.Empty);

            string query = string.Format("select max(SourceId) from d_Marks_FOYRSKVR d left join DataSources ds on (ds.id = d.sourceid) where ds.year = {0}",
                this.DataSource.Year);
            object subKvrSourceId = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if (subKvrSourceId == DBNull.Value)
                InitClsDataSet(ref daMarksSubKvr, ref dsMarksSubKvr, clsMarksSubKvr, false, string.Empty);
            else
            {
                InitDataSet(ref daMarksSubKvr, ref dsMarksSubKvr, clsMarksSubKvr, false,
                    string.Format("SOURCEID = {0} and rowType = 0", Convert.ToInt32(subKvrSourceId)), string.Empty);
            }

            InitDataSet(ref daMeansType, ref dsMeansType, fxcMeansType, true, string.Empty, string.Empty);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            regForPumpSourceID = GetRegions4PumpSourceID();
            InitDataSet(ref daRegions4Pump, ref dsRegions4Pump, clsRegions4Pump, false,
                string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);

            InitFactDataSet(ref daFOYRDefProf, ref dsFOYRDefProf, fctFOYRDefProf);
            InitFactDataSet(ref daFOYREmbezzles, ref dsFOYREmbezzles, fctFOYREmbezzles);
            InitFactDataSet(ref daFOYRIncomes, ref dsFOYRIncomes, fctFOYRIncomes);
            InitFactDataSet(ref daFOYRNet, ref dsFOYRNet, fctFOYRNet);
            InitFactDataSet(ref daFOYROutcomes, ref dsFOYROutcomes, fctFOYROutcomes);
            InitFactDataSet(ref daFOYRSrcFin, ref dsFOYRSrcFin, fctFOYRSrcFin);

            if (hasBalancBlock)
            {
                InitClsDataSet(ref daMarksBalOff, ref dsMarksBallOff, clsMarksBallOff, false, string.Empty);
                InitFactDataSet(ref daFOYRBalanc, ref dsFOYRBalanc, fctFOYRBalanc);
                InitFactDataSet(ref daFOYRBalOff, ref dsFOYRBalOff, fctFOYRBalOff);
            }

            InitNullClsRows();
            FillCache();

            if (subKvrSourceId != DBNull.Value)
                nullMarksSubKvr = PumpCachedRow(marksSubKvrCache, dsMarksSubKvr.Tables[0], clsMarksSubKvr, "0",
                    new object[] { "Code", 0, "CodeRprt", "0", "Name", constDefaultClsName, "SourceID", subKvrSourceId });
        }

        /// <summary>
        /// Внести изменения в базу
        /// </summary>
        protected override void UpdateData()
        {
            UpdateDataSet(daEKR, dsEKR, clsEKR);
            UpdateDataSet(daFKR, dsFKR, clsFKR);
            UpdateDataSet(daKCSR, dsKCSR, clsKCSR);
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);

            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);

            UpdateDataSet(daKIF2004, dsKIF2004, clsKIF2004);

            UpdateDataSet(daKIF2005, dsKIF2005, clsKIF2005);
            UpdateDataSet(daKVR, dsKVR, clsKVR);
            UpdateDataSet(daMarksNet, dsMarksNet, clsMarksNet);
            UpdateDataSet(daMarksSubKvr, dsMarksSubKvr, clsMarksSubKvr);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daRegions4Pump, dsRegions4Pump, clsRegions4Pump);

            UpdateDataSet(daAccount, dsAccount, clsAccount);

            UpdateDataSet(daFOYRDefProf, dsFOYRDefProf, fctFOYRDefProf);
            UpdateDataSet(daFOYREmbezzles, dsFOYREmbezzles, fctFOYREmbezzles);
            UpdateDataSet(daFOYRIncomes, dsFOYRIncomes, fctFOYRIncomes);
            UpdateDataSet(daFOYRNet, dsFOYRNet, fctFOYRNet);
            UpdateDataSet(daFOYROutcomes, dsFOYROutcomes, fctFOYROutcomes);
            UpdateDataSet(daFOYRSrcFin, dsFOYRSrcFin, fctFOYRSrcFin);

            if (hasBalancBlock)
            {
                UpdateDataSet(daMarksBalOff, dsMarksBallOff, clsMarksBallOff);
                UpdateDataSet(daFOYRBalanc, dsFOYRBalanc, fctFOYRBalanc);
                UpdateDataSet(daFOYRBalOff, dsFOYRBalOff, fctFOYRBalOff);
            }
        }

        /// <summary>
        /// Инициализирует строки классификаторов "Неизвестные данные"
        /// </summary>
        private void InitNullClsRows()
        {
            nullEKR = clsEKR.UpdateFixedRows(this.DB, this.SourceID);
            nullFKR = clsFKR.UpdateFixedRows(this.DB, this.SourceID);
            nullKCSR = clsKCSR.UpdateFixedRows(this.DB, this.SourceID);
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF2004 = clsKIF2004.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF2005 = clsKIF2005.UpdateFixedRows(this.DB, this.SourceID);
            nullKVR = clsKVR.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksNet = clsMarksNet.UpdateFixedRows(this.DB, this.SourceID);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            nullOutcomesCls = clsOutcomesCls.UpdateFixedRows(this.DB, this.SourceID);
            nullKvsr = clsKvsr.UpdateFixedRows(this.DB, this.SourceID);
            nullAccount = clsAccount.UpdateFixedRows(this.DB, this.SourceID);
            if (hasBalancBlock)
                nullMarksBallOff = clsMarksBallOff.UpdateFixedRows(this.DB, this.SourceID);
        }

        #region GUID

        private const string FX_MEANS_TYPE_FOYR_EMBEZZLES_GUID = "2b7a74cf-a828-4613-9518-d5e24e088dc7";
        private const string FX_MARKS_FOYR_EMBEZZLES_GUID = "2e952691-8969-4287-820c-582d06941fda";
        private const string D_REGIONS_FOR_PUMP_SKIF_GUID = "e9a95119-21f1-43d8-8dc2-8d4af7c195d0";

        private const string D_EKR_FOYR_GUID = "e03d01a7-4956-4017-8d14-84b39cdebaff";
        private const string D_FKR_FOYR_GUID = "7519d601-fd65-4246-99be-27919ccf3a35";
        private const string D_KCSR_FOYR_GUID = "3e1f3525-5ebf-439a-bc7b-0e7a51a6e6c3";
        private const string D_KD_FOYR_GUID = "8ce19831-1c98-468a-ba8c-125f2123a719";
        private const string D_KIF_FOYR_2004_GUID = "2ed65dfe-1a70-4d00-832d-3b96c6dc10b0";
        private const string D_KIF_FOYR_2005_GUID = "6b1de789-6b44-44ef-b35e-f4bde70cbc75";
        private const string D_KVR_FOYR_GUID = "4b4df36e-1182-4222-965e-e12485993e60";
        private const string D_MARKS_FOYR_NET_GUID = "e1bd49d2-5e19-4f3d-b6a5-43f44bc84ab0";
        private const string D_MARKS_SUB_KVR_GUID = "a573cff1-aefd-47d2-b79c-7524d8f01c82";
        private const string D_REGIONS_FOYR_GUID = "278f9e91-5a45-4a50-8b52-7392565e510e";
        private const string D_OUTCOMES_CLS_GUID = "0f2c9f44-8afc-4445-bef0-f876c2b58191";
        private const string D_KVSR_GUID = "b28be109-b805-45bf-9a9c-e4346c02b22e";
        private const string D_ACCOUNT_MONTH_REP_GUID = "f24b024d-5b8e-42dd-ab63-29c5781e558b";
        private const string D_MARKS_FOYR_BALOFF_GUID = "3c3fe991-e216-4abf-a487-62de03f63c9e";

        private const string F_F_FOYR_EMBEZZLES_GUID = "2137093e-3349-41b4-961c-8833ed4bc62c";
        private const string F_DP_FOYR_DEF_PROF_GUID = "2686e374-23f5-46f5-b514-ae0bcf3c8fa7";
        private const string F_D_FOYR_INCOMES_GUID = "7edae383-d3cc-43ec-91c0-d4e59e03a716";
        private const string F_F_FOYR_NET_GUID = "f7dbbc00-f550-4336-b1ae-cb3a53064ebd";
        private const string F_R_FOYR_OUTCOMES_GUID = "6385095a-5d23-48ef-861f-1df393dad0cf";
        private const string F_SRC_FIN_FOYR_SRC_FIN_GUID = "794cde51-5b05-4b0b-a7c0-2024e554cd47";
        private const string F_F_FOYR_BALANC_GUID = "82847e3d-d47c-4099-9e7d-64981221670b";
        private const string F_F_FOYR_BALOFF_GUID = "083111a4-09bc-4050-9a9a-1dd07a097ff5";

        #endregion GUID

        private bool HasBalancBlock()
        {
            if (!this.Scheme.Classifiers.ContainsKey(D_MARKS_FOYR_BALOFF_GUID) ||
                !this.Scheme.FactTables.ContainsKey(F_F_FOYR_BALANC_GUID) ||
                !this.Scheme.FactTables.ContainsKey(F_F_FOYR_BALOFF_GUID))
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, "Предметный блок \"Баланс исполнения бюджета\" отсутсвует в базе");
                return false;
            }
            return true;
        }

        protected override void InitDBObjects()
        {
            hasBalancBlock = HasBalancBlock();

            fxcMeansType = this.Scheme.Classifiers[FX_MEANS_TYPE_FOYR_EMBEZZLES_GUID];
            fxcMarksEmbezzles = this.Scheme.Classifiers[FX_MARKS_FOYR_EMBEZZLES_GUID];
            clsMarksSubKvr = this.Scheme.Classifiers[D_MARKS_SUB_KVR_GUID];
            clsRegions4Pump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_SKIF_GUID];

            List<IClassifier> classifiers = new List<IClassifier>();
            classifiers.Add(clsEKR = this.Scheme.Classifiers[D_EKR_FOYR_GUID]);
            classifiers.Add(clsFKR = this.Scheme.Classifiers[D_FKR_FOYR_GUID]);
            classifiers.Add(clsKCSR = this.Scheme.Classifiers[D_KCSR_FOYR_GUID]);
            classifiers.Add(clsKD = this.Scheme.Classifiers[D_KD_FOYR_GUID]);
            classifiers.Add(clsKIF2004 = this.Scheme.Classifiers[D_KIF_FOYR_2004_GUID]);
            classifiers.Add(clsKIF2005 = this.Scheme.Classifiers[D_KIF_FOYR_2005_GUID]);
            classifiers.Add(clsKVR = this.Scheme.Classifiers[D_KVR_FOYR_GUID]);
            classifiers.Add(clsMarksNet = this.Scheme.Classifiers[D_MARKS_FOYR_NET_GUID]);
            classifiers.Add(clsRegions = this.Scheme.Classifiers[D_REGIONS_FOYR_GUID]);
            classifiers.Add(clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID]);
            classifiers.Add(clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID]);
            classifiers.Add(clsAccount = this.Scheme.Classifiers[D_ACCOUNT_MONTH_REP_GUID]);

            List<IFactTable> factTables = new List<IFactTable>();
            factTables.Add(fctFOYREmbezzles = this.Scheme.FactTables[F_F_FOYR_EMBEZZLES_GUID]);
            factTables.Add(fctFOYRDefProf = this.Scheme.FactTables[F_DP_FOYR_DEF_PROF_GUID]);
            factTables.Add(fctFOYRIncomes = this.Scheme.FactTables[F_D_FOYR_INCOMES_GUID]);
            factTables.Add(fctFOYRNet = this.Scheme.FactTables[F_F_FOYR_NET_GUID]);
            factTables.Add(fctFOYROutcomes = this.Scheme.FactTables[F_R_FOYR_OUTCOMES_GUID]);
            factTables.Add(fctFOYRSrcFin = this.Scheme.FactTables[F_SRC_FIN_FOYR_SRC_FIN_GUID]);

            if (hasBalancBlock)
            {
                classifiers.Add(clsMarksBallOff = this.Scheme.Classifiers[D_MARKS_FOYR_BALOFF_GUID]);
                factTables.Add(fctFOYRBalanc = this.Scheme.FactTables[F_F_FOYR_BALANC_GUID]);
                factTables.Add(fctFOYRBalOff = this.Scheme.FactTables[F_F_FOYR_BALOFF_GUID]);
            }

            this.UsedClassifiers = classifiers.ToArray();
            this.VersionClassifiers = new IClassifier[] { clsKD, clsEKR };
            this.UsedFacts = factTables.ToArray();

            this.CubeFacts = new IFactTable[] { };
            if (ToPumpBlock(Block.bIncomes))
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctFOYRIncomes });
            if (ToPumpBlock(Block.bOutcomes))
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctFOYROutcomes });
            if (ToPumpBlock(Block.bDefProf))
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctFOYRDefProf });
            if (ToPumpBlock(Block.bFinSources))
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctFOYRSrcFin });
            if (ToPumpBlock(Block.bNet))
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctFOYRNet, fctFOYREmbezzles });
            if (ToPumpBlock(Block.bBalanc) && hasBalancBlock)
            {
                this.CubeClassifiers = (IClassifier[])CommonRoutines.ConcatArrays(this.UsedClassifiers, new IClassifier[] { clsAccount, clsMarksBallOff });
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctFOYRBalanc, fctFOYRBalOff });
            }
        }

        /// <summary>
        /// Функция выполнения завершающих действий этап
        /// </summary>
        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFOYRDefProf);
            ClearDataSet(ref dsFOYREmbezzles);
            ClearDataSet(ref dsFOYRIncomes);
            ClearDataSet(ref dsFOYRNet);
            ClearDataSet(ref dsFOYROutcomes);
            ClearDataSet(ref dsFOYRSrcFin);

            ClearDataSet(ref dsEKR);
            ClearDataSet(ref dsFKR);
            ClearDataSet(ref dsKCSR);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsKIF2004);
            ClearDataSet(ref dsKIF2005);
            ClearDataSet(ref dsKVR);
            ClearDataSet(ref dsMarksEmbezzles);
            ClearDataSet(ref dsMarksNet);
            ClearDataSet(ref dsMarksSubKvr);
            ClearDataSet(ref dsMeansType);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsRegions4Pump);
            ClearDataSet(ref dsOutcomesCls);
            ClearDataSet(ref dsKvsr);
            ClearDataSet(ref dsAccount);

            if (hasBalancBlock)
            {
                ClearDataSet(ref dsFOYRBalanc);
                ClearDataSet(ref dsFOYRBalOff);
                ClearDataSet(ref dsMarksBallOff);
            }
        }

        /// <summary>
        /// Закачка данных
        /// </summary>
        protected override void DirectPumpData()
        {
            this.SkifReportFormat = SKIFFormat.YearReports;

            base.DirectPumpData();

            PumpDataYVTemplate();
        }

        #endregion Закачка данных

        #region Обработка данных

        private void DeleteUnusedAccount()
        {
            if (!ToPumpBlock(Block.bBalanc))
                return;

            IFactTable[] facts = new IFactTable[] { fctFOYRBalanc };
            string[] refFieldNames = new string[] { "RefAccount" };
            if (this.Scheme.FactTables.ContainsKey("b84af6ae-ec44-46ca-bf51-6fb034a3cc3e"))
            {
                facts = (IFactTable[])CommonRoutines.ConcatArrays(facts, new IFactTable[] { this.Scheme.FactTables["b84af6ae-ec44-46ca-bf51-6fb034a3cc3e"] });
                refFieldNames = (string[])CommonRoutines.ConcatArrays(refFieldNames, new string[] { "RefAccount" });
            }

            int sourceId = this.SourceID;
            string query = string.Format(" select count(*) from {0} where sourceid = {1}", clsAccount.FullDBName, sourceId);
            int recCountBefore = Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));

            query = string.Format(" delete from {0} where (sourceid = {1}) ", clsAccount.FullDBName, sourceId);
            for (int i = 0; i < facts.Length; i++)
                query += string.Format(" and (id not in (select distinct {0} from {1})) ", refFieldNames[i], facts[i].FullDBName);
            this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });

            query = string.Format(" select count(*) from {0} where sourceid = {1}", clsAccount.FullDBName, sourceId);
            int recCountAfter = Convert.ToInt32(this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { }));
            int deletedRecCount = recCountBefore - recCountAfter;
            if (deletedRecCount != 0)
            {
                List<string> factsNames = new List<string>();
                for (int i = 0; i < facts.Length; i++)
                    factsNames.Add(facts[i].FullCaption);
                string message = string.Format(
                    "Классификатор '{0}' содержал записи, по которым не было данных в таблицах фактов '{1}'. Эти записи были удалены (кол-во: {2})",
                    clsAccount.FullCaption, string.Join("', '", factsNames.ToArray()), deletedRecCount);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, message);
            }
        }

        private void SetRegDocType()
        {
            // заполняем тип документа у районов, берем его из районов для закачки скиф
            int count = dsRegions.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dsRegions.Tables[0].Rows[i];
                string code = row["CODESTR"].ToString().PadLeft(10, '0');
                string name = row["NAME"].ToString();
                string key = code + "|" + name;
                if (!region4PumpCache.ContainsKey(key))
                    continue;
                int docType = region4PumpCache[key];
                row["REFDOCTYPE"] = docType.ToString();
                switch (docType)
                {
                    case 1:
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                            "Значение признака 'Тип документа.СКИФ' для района '{0}' (код {1}) равно {2} ({3}). " +
                            "По данному району не будут откорректированы суммы по уровням бюджетов.",
                            name, code, docType, GetSKIFDocType(docType)));
                        break;
                    case 20:
                    case 21:
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                            "Значение признака 'Тип документа.СКИФ' для района '{0}' (код {1}) равно {2} ({3}). " +
                            "Данный тип документа является аналитическим признаком и не подлежит использованию. " +
                            "Для коррекции сумм по уровням бюджетов заново заполните ТипДокумента.СКИФ для указанного района и запустите этап обработки.",
                            name, code, docType, GetSKIFDocType(docType)));
                        break;
                }
            }
        }

        #region удаление лишних записей из таблицы фактов CШК.ФО_ГодОтч

        private void FillGroupCache(ref Dictionary<string, List<string>> cache, DataTable dt, string[] keyFields)
        {
            cache = new Dictionary<string, List<string>>();
            int rowsCount = dt.Rows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                DataRow row = dt.Rows[curRow];
                string key = GetComplexCacheKey(row, keyFields, "|");
                if (cache.ContainsKey(key))
                    cache[key].Add(Convert.ToString(row["ID"]));
                else
                    cache.Add(key, new List<string>(new string[] { Convert.ToString(row["ID"]) }));
            }
        }

        // удаляем из таблицы фактов CШК.ФО_ГодОтч те записи,
        // у которых RefKCSR ссылается на нулевой КЦСР (Code = 0)
        // при условии, что RefKCSR ссылается так же и на другие КЦСР
        private void CleanYRNetDataTable()
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            List<string> deletingIds = new List<string>();
            Dictionary<string, List<string>> cacheWithZeroKcsr = null;
            try
            {
                // выбираем записи с нулевым КЦСР
                string restrict = string.Format(" sourceid = {0} AND refkcsr IN " +
                    " ( SELECT kcsr.id FROM {1} kcsr WHERE kcsr.sourceid = {0} AND kcsr.code = 0 ) ",
                    this.SourceID, clsKCSR.FullDBName);
                InitDataSet(ref da, ref ds, fctFOYRNet, restrict);
                // группируем их по классификаторам Районы, КСШК, Период, Уровни бюджета, РзПр
                string[] keyFields = new string[] { "RefRegions", "RefKSSHK", "RefYearDayUNV", "RefBdgtLevels", "RefFKR" };
                FillGroupCache(ref cacheWithZeroKcsr, ds.Tables[0], keyFields);

                // выбираем записи с ненулевым КЦСР
                restrict = string.Format(" sourceid = {0} AND refkcsr IN " +
                    " ( SELECT kcsr.id FROM {1} kcsr WHERE kcsr.sourceid = {0} AND kcsr.code <> 0 ) ",
                    this.SourceID, clsKCSR.FullDBName);
                InitDataSet(ref da, ref ds, fctFOYRNet, restrict);

                int rowsCount = ds.Tables[0].Rows.Count;
                for (int curRow = 0; curRow < rowsCount; curRow++)
                {
                    DataRow row = ds.Tables[0].Rows[curRow];
                    string key = GetComplexCacheKey(row, keyFields, "|");
                    if (cacheWithZeroKcsr.ContainsKey(key))
                        deletingIds.AddRange(cacheWithZeroKcsr[key]);
                }

                if (deletingIds.Count > 0)
                {
                    int index = 0;
                    for (; index < deletingIds.Count; index += 1000)
                    {
                        List<string> ids = null;
                        int count = index + 1000;
                        if (count > deletingIds.Count)
                            ids = deletingIds.GetRange(index, deletingIds.Count - index);
                        else
                            ids = deletingIds.GetRange(index, 1000);
                        string deleteQuery = string.Format(" DELETE FROM {0} WHERE id IN ({1}) ",
                            fctFOYRNet.FullDBName, string.Join(", ", ids.ToArray()));
                        this.DB.ExecQuery(deleteQuery, QueryResultTypes.Scalar, new IDbDataParameter[] { });
                    }
                    WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                        "Таблица фактов '{0}' содержала записи с нулевыми КЦСР. Эти записи были удалены (кол-во: {1})",
                        fctFOYRNet.FullCaption, deletingIds.Count));
                }
            }
            finally
            {
                if (deletingIds != null)
                    deletingIds.Clear();
                if (cacheWithZeroKcsr != null)
                    cacheWithZeroKcsr.Clear();
            }
        }

        #endregion

        private void CorrectSums()
        {
            YRSumCorrectionConfig yrSumCorrectionConfig = new YRSumCorrectionConfig();
            yrSumCorrectionConfig.AssignedField = "ASSIGNED";
            yrSumCorrectionConfig.AssignedReportField = "ASSIGNEDREPORT";
            yrSumCorrectionConfig.PerformedField = "PERFORMED";
            yrSumCorrectionConfig.PerformedReportField = "PERFORMEDREPORT";

            // ДефицитПрофицит
            TransferSourceSums(fctFOYRDefProf, yrSumCorrectionConfig);

            // Доходы
            CorrectFactTableSums(fctFOYRIncomes, dsKD.Tables[0], clsKD,
                "REFKD", yrSumCorrectionConfig, BlockProcessModifier.YRStandard,
                new string[] { "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS");

            // Источники финансирования
            CorrectFactTableSums(fctFOYRSrcFin, dsKIF2005.Tables[0], clsKIF2005,
                "REFKIF2005", yrSumCorrectionConfig, BlockProcessModifier.YRStandard,
                new string[] { "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS");

            // Расходы
            if (this.DataSource.Year >= 2005)
            {
                CorrectFactTableSums(fctFOYROutcomes, dsEKR.Tables[0], clsEKR,
                    "RefEKRFOYR", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes,
                    new string[] { "REFFKR", "REFKCSR", "REFKVR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", true);
                CorrectFactTableSums(fctFOYROutcomes, dsKCSR.Tables[0], clsKCSR,
                    "REFKCSR", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes,
                    new string[] { "REFFKR", "REFKVR", "RefEKRFOYR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", false);
                CorrectFactTableSums(fctFOYROutcomes, dsFKR.Tables[0], clsFKR,
                    "REFFKR", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes,
                    new string[] { "REFKCSR", "REFKVR", "RefEKRFOYR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", false);
            }
            else
            {
                CorrectFactTableSums(fctFOYROutcomes, dsEKR.Tables[0], clsEKR,
                    "RefEKRFOYR", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes,
                    new string[] { "REFFKR", "REFKCSR", "REFKVR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", true);
                CorrectFactTableSums(fctFOYROutcomes, dsKCSR.Tables[0], clsKCSR,
                    "REFKCSR", yrSumCorrectionConfig, BlockProcessModifier.YROutcomes,
                    new string[] { "REFFKR", "REFKVR", "RefEKRFOYR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", false);
            }

            // Сети, штаты, контингенты
            CleanYRNetDataTable();
            CommonSumCorrectionConfig yrNetSumCorrectionConfig = new CommonSumCorrectionConfig();
            yrNetSumCorrectionConfig.Sum1 = "BegYear";
            yrNetSumCorrectionConfig.Sum1Report = "BegYearRep";
            yrNetSumCorrectionConfig.Sum2 = "EndYear";
            yrNetSumCorrectionConfig.Sum2Report = "EndYearRep";
            yrNetSumCorrectionConfig.Sum3 = "BudMidYear";
            yrNetSumCorrectionConfig.Sum3Report = "BudMidYRep";
            yrNetSumCorrectionConfig.Sum4 = "FMidYear";
            yrNetSumCorrectionConfig.Sum4Report = "FMidYRep";
            CorrectFactTableSums(fctFOYRNet, dsFKR.Tables[0], clsFKR,
                "RefFKR", yrNetSumCorrectionConfig, BlockProcessModifier.YRNet,
                new string[] { "RefKSSHK" }, "RefRegions", "RefBdgtLevels", true);

            // Баланс
            if (ToPumpBlock(Block.bBalanc))
            {
                CommonSumCorrectionConfig yrBalancSumCorrectionConfig = new CommonSumCorrectionConfig();
                yrBalancSumCorrectionConfig.Sum1Report = "BegYear";
                yrBalancSumCorrectionConfig.Sum2Report = "EndYear";
                GroupTable(fctFOYRBalOff,
                    new string[] { "RefRegion", "RefYearDayUNV", "RefMarks", "RefBdgtLev", "RefMeansType" },
                    yrBalancSumCorrectionConfig);

                yrBalancSumCorrectionConfig.Sum3Report = "ExceptBegYear";
                yrBalancSumCorrectionConfig.Sum4Report = "ExceptEndYear";
                GroupTable(fctFOYRBalanc,
                    new string[] { "RefRegion", "RefYearDayUNV", "RefAccount", "RefBdgtLev", "RefMeansType" },
                    yrBalancSumCorrectionConfig);
            }
        }

        #region заполнение классификатора "Расходы"

        private string GetOutcomesClsCode(string fkrCode, string kcsrCode, string kvrCode)
        {
            return string.Format("{0}{1}{2}", fkrCode, kcsrCode, kvrCode).TrimStart('0').PadLeft(1, '0');
        }

        private int PumpOutcomesClsRow(string fkrCode, string kcsrCode, string kvrCode, string name, int parentId)
        {
            string outcomesClsCode = GetOutcomesClsCode(fkrCode, kcsrCode, kvrCode);
            object[] mapping = new object[] { "Code", outcomesClsCode, "Name", name };
            string cacheKey = string.Format("{0}|", outcomesClsCode);
            if (parentId != -1)
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", parentId });
                cacheKey += parentId.ToString();
            }
            return PumpCachedRow(outcomesClsCache, dsOutcomesCls.Tables[0], clsOutcomesCls, cacheKey, mapping);
        }

        private string GetClsNameByCode(int code, DataTable dt)
        {
            DataRow[] rows = dt.Select(string.Format("Code={0}", code));
            if (rows.GetLength(0) == 0)
                return constDefaultClsName;
            else
                return rows[0]["Name"].ToString();
        }

        private const string NULL_KCSR_CODE = "0000000";
        private const string NULL_KVR_CODE = "000";
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

            // 1 - последние два кода фкр заменяем на нули, добавляем запись
            string parentFkrCode = string.Format("{0}{1}", fkrCode.Substring(0, 2), "00");
            string parentFkrName = GetClsNameByCode(Convert.ToInt32(parentFkrCode.TrimStart('0').PadLeft(1, '0')), dsFKR.Tables[0]);
            int parentId = PumpOutcomesClsRow(parentFkrCode, NULL_KCSR_CODE, NULL_KVR_CODE, parentFkrName, -1);
            // 2 - добавляем запись с заполненным фкр
            parentId = PumpOutcomesClsRow(fkrCode, NULL_KCSR_CODE, NULL_KVR_CODE, fkrName, parentId);
            // 3 - последние 4 кода кцср заменяем на нули, добавляем запись
            string parentKcsrCode = string.Format("{0}{1}", kcsrCode.Substring(0, 3), "0000");
            string parentKcsrName = GetClsNameByCode(Convert.ToInt32(parentKcsrCode.TrimStart('0').PadLeft(1, '0')), dsKCSR.Tables[0]);
            parentId = PumpOutcomesClsRow(fkrCode, parentKcsrCode, NULL_KVR_CODE, parentKcsrName, parentId);
            // 4 - последние 2 кода кцср заменяем на нули, добавляем запись
            parentKcsrCode = string.Format("{0}{1}", kcsrCode.Substring(0, 5), "00");
            parentKcsrName = GetClsNameByCode(Convert.ToInt32(parentKcsrCode.TrimStart('0').PadLeft(1, '0')), dsKCSR.Tables[0]);
            parentId = PumpOutcomesClsRow(fkrCode, parentKcsrCode, NULL_KVR_CODE, parentKcsrName, parentId);
            // 5 - добавляем запись с заполненным фкр и кцср
            parentId = PumpOutcomesClsRow(fkrCode, kcsrCode, NULL_KVR_CODE, kcsrName, parentId);
            // 6 - добавляем запись с заполненным фкр кцср и квр
            parentId = PumpOutcomesClsRow(fkrCode, kcsrCode, kvrCode, kvrName, parentId);

            factRow["RefR"] = parentId;

            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);
        }

        private void FillOutcomesCls()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation,
                string.Format("Начало формирования классификатора '{0}'", clsOutcomesCls.FullCaption));
            object[] mapping = new object[] { "Code", 0, "Name", constDefaultClsName };
            string cacheKey = "0|";
            defaultOutcomesCls = PumpCachedRow(outcomesClsCache, dsOutcomesCls.Tables[0], clsOutcomesCls, cacheKey, mapping);

            FillRowsCache(ref fkrRowsCache, dsFKR.Tables[0], "Id");
            FillRowsCache(ref kcsrRowsCache, dsKCSR.Tables[0], "Id");
            FillRowsCache(ref kvrRowsCache, dsKVR.Tables[0], "Id");

            try
            {
                PartialDataProcessingTemplate(fctFOYROutcomes, string.Empty, MAX_DS_RECORDS_AMOUNT, new DataPartRowProcessing(PumpOutcomesClsRows),
                    "формирование классификатора 'Расходы.ФО_ГодОтч'");
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
            DeleteUnusedAccount();
            SetClsHierarchy();
            UpdateData();
            SetRegDocType();
            UpdateData();
            CorrectSums();
            UpdateData();
            FillOutcomesCls();
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
    }
}