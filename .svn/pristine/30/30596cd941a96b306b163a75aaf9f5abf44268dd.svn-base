using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.DataPumps.Common;

namespace Krista.FM.Server.DataPumps.SKIFMonthRepPump
{
    // Главный модуль закачки. Содержит реализацию функций закачки

    /// <summary>
    /// ФО_0002_Ежемесячные отчеты.
    /// Закачка данных СКИФ
    /// </summary>
    public partial class SKIFMonthRepPumpModule : SKIFRepPumpModuleBase
    {
        #region Поля

        #region Факты

        // Факт.ФО_МесОтч_Дефицит Профицит (f_F_MonthRepDefProf)
        private IDbDataAdapter daMonthRepDefProf;
        private DataSet dsMonthRepDefProf;
        private IFactTable fctMonthRepDefProf;
        // Факт.ФО_МесОтч_Доходы (f_F_MonthRepIncomes)
        private IDbDataAdapter daMonthRepIncomes;
        private DataSet dsMonthRepIncomes;
        private IFactTable fctMonthRepIncomes;
        // Факт.ФО_МесОтч_ИсточникВнешФинансирования (f_F_MonthRepOutFin)
        private IDbDataAdapter daMonthRepOutFin;
        private DataSet dsMonthRepOutFin;
        private IFactTable fctMonthRepOutFin;
        // Факт.ФО_МесОтч_ИсточникВнутрФинансирования (f_F_MonthRepInFin)
        private IDbDataAdapter daMonthRepInFin;
        private DataSet dsMonthRepInFin;
        private IFactTable fctMonthRepInFin;
        // Факт.ФО_МесОтч_Расходы (f_F_MonthRepOutcomes)
        private IDbDataAdapter daMonthRepOutcomes;
        private DataSet dsMonthRepOutcomes;
        private IFactTable fctMonthRepOutcomes;
        // Факт.ФО_МесОтч_СпрВнешДолг (f_F_MonthRepOutDebtBooks)
        private IDbDataAdapter daMonthRepOutDebtBooks;
        private DataSet dsMonthRepOutDebtBooks;
        private IFactTable fctMonthRepOutDebtBooks;
        // Факт.ФО_МесОтч_СпрВнутрДолг (f_F_MonthRepInDebtBooks)
        private IDbDataAdapter daMonthRepInDebtBooks;
        private DataSet dsMonthRepInDebtBooks;
        private IFactTable fctMonthRepInDebtBooks;
        // Факт.ФО_МесОтч_СпрДоходы (f_F_MonthRepIncomesBooks)
        private IDbDataAdapter daMonthRepIncomesBooks;
        private DataSet dsMonthRepIncomesBooks;
        private IFactTable fctMonthRepIncomesBooks;
        // Факт.ФО_МесОтч_СпрЗадолженность (f_F_MonthRepArrearsBooks)
        private IDbDataAdapter daMonthRepArrearsBooks;
        private DataSet dsMonthRepArrearsBooks;
        private IFactTable fctMonthRepArrearsBooks;
        // Факт.ФО_МесОтч_СпрРасходы (f_F_MonthRepOutcomesBooks)
        private IDbDataAdapter daMonthRepOutcomesBooks;
        private DataSet dsMonthRepOutcomesBooks;
        private IFactTable fctMonthRepOutcomesBooks;
        // Факт.ФО_МесОтч_СпрРасходыДоп (f_F_MonthRepOutcomesBooksAdd)
        private IDbDataAdapter daMonthRepOutcomesBooksEx;
        private DataSet dsMonthRepOutcomesBooksEx;
        private IFactTable fctMonthRepOutcomesBooksEx;
        // Факт.ФО_МесОтч_СпрОстатки (f_F_MonthRepExcessBooks)
        private IDbDataAdapter daMonthRepExcessBooks;
        private DataSet dsMonthRepExcessBooks;
        private IFactTable fctMonthRepExcessBooks;
        // Факт.ФО_МесОтч_КонсРасчеты (f_F_MonthRepAccount)
        private IDbDataAdapter daMonthRepAccount;
        private DataSet dsMonthRepAccount;
        private IFactTable fctMonthRepAccount;
        // Факт.ФО_МесОтч_Задолженность (f_F_MonthRepArrears)
        private IDbDataAdapter daMonthRepArrears;
        private DataSet dsMonthRepArrears;
        private IFactTable fctMonthRepArrears;

        #endregion Факты

        #region Классификаторы

        // РзПр.Анализ (d_FKR_Analysis)
        private IDbDataAdapter daAnalFKR;
        private DataSet dsAnalFKR;
        private IClassifier clsAnalFKR;
        private Dictionary<string, string> analFKRCache = null;
        // Районы.МесОтч (d_Regions_MonthRep)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> regionCache = null;
        private int nullRegions = -1;
        // Районы.Служебный для закачки СКИФ (d_Regions_ForPumpSKIF)
        private IDbDataAdapter daRegions4Pump;
        private DataSet dsRegions4Pump;
        private IClassifier clsRegions4Pump;
        private Dictionary<string, int> region4PumpCache = null;
        // КД.МесОтч (d_KD_MonthRep)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<string, int> kdCache = null;
        private int nullKD = -1;
        // КИВнешФ.МесОтч (d_SOF_MonthRep)
        private IDbDataAdapter daSrcOutFin;
        private DataSet dsSrcOutFin;
        private IClassifier clsSrcOutFin;
        private Dictionary<string, int> srcOutFinCache = null;
        private Dictionary<string, int> srcOutFinWithKstCache = null;
        private Dictionary<string, int> scrOutFinSourcesRefCache = null;
        private int nullSrcOutFin = -1;
        // КИВнутрФ.МесОтч (d_SIF_MonthRep)
        private IDbDataAdapter daSrcInFin;
        private DataSet dsSrcInFin;
        private Dictionary<string, int> srcInFinCache = null;
        private Dictionary<string, int> srcInFinWithKstCache = null;
        private Dictionary<string, int> scrInFinSourcesRefCache = null;
        private IClassifier clsSrcInFin;
        private int nullSrcInFin = -1;
        // Расходы.МесОтч (d_R_MonthRep)
        private IDbDataAdapter daFKR;
        private DataSet dsFKR;
        private IClassifier clsFKR;
        private Dictionary<string, int> fkrCache = null;
        private int nullFKR = -1;
        // РзПр.МесОтчСправРасходы (d_FKR_MonthRepBook)
        private IDbDataAdapter daFKRBook;
        private DataSet dsFKRBook;
        private IClassifier clsFKRBook;
        private Dictionary<string, int> fkrBookCache = null;
        private int nullFKRBook = -1;
        // КОСГУ.МесОтчСправРасходы (d_EKR_MonthRepBook)
        private IDbDataAdapter daEKRBook;
        private DataSet dsEKRBook;
        private IClassifier clsEKRBook;
        private Dictionary<string, int> ekrBookCache = null;
        private int nullEKRBook = -1;
        // Показатели.МесОтч_СпрВнешДолг (d_Marks_MonthRepOutDebt)
        private IDbDataAdapter daMarksOutDebt;
        private DataSet dsMarksOutDebt;
        private IClassifier clsMarksOutDebt;
        private int nullMarksOutDebt = -1;
        // Показатели.МесОтч_СпрВнутрДолг (d_Marks_MonthRepInDebt)
        private IDbDataAdapter daMarksInDebt;
        private DataSet dsMarksInDebt;
        private IClassifier clsMarksInDebt;
        private int nullMarksInDebt = -1;
        // Администратор.МесОтч (d_KVSR_MonthRep)
        private IDbDataAdapter daKVSR;
        private DataSet dsKVSR;
        private IClassifier clsKVSR;
        private Dictionary<string, int> kvsrCache = null;
        private int nullKVSR = -1;
        // Показатели.МесОтч_СпрЗадолженность (d_Marks_MonthRepArrears)
        private IDbDataAdapter daMarksArrears;
        private DataSet dsMarksArrears;
        private IClassifier clsMarksArrears;
        private Dictionary<string, int> arrearsCache = null;
        private int nullMarksArrears = -1;
        // Показатели.МесОтч_СпрРасходы (d_Marks_MonthRepOutcomes)
        private IDbDataAdapter daMarksOutcomes;
        private DataSet dsMarksOutcomes;
        private IClassifier clsMarksOutcomes;
        private Dictionary<string, int> marksOutcomesCache = null;
        private int nullMarksOutcomes = -1;
        // Тип средств.СКИФ (fx_MeansType_SKIF)
        private IDbDataAdapter daMeansType;
        private DataSet dsMeansType;
        private IClassifier fxcMeansType;
        // КОСГУ.МесОтч (d_EKR_MonthRep)
        private IDbDataAdapter daEKR;
        private DataSet dsEKR;
        private IClassifier clsEKR;
        private Dictionary<string, int> ekrCache = null;
        private int nullEKR = -1;
        // Показатели.МесОтч_СпрОстатки (d_Marks_MonthRepExcess)
        private IDbDataAdapter daMarksExcess;
        private DataSet dsMarksExcess;
        private IClassifier clsMarksExcess;
        private Dictionary<string, int> excessCache = null;
        private int nullMarksExcess = -1;
        // Показатели.МесОтч_КонсРасчеты (d_Marks_MonthRepAccount)
        private IDbDataAdapter daMarksAccount;
        private DataSet dsMarksAccount;
        private Dictionary<string, int> marksAccountCache = null;
        private IClassifier clsMarksAccount;
        private int nullMarksAccount = -1;
        // ПланСчетов.МесОтч (d_Account_MonthRep)
        private IDbDataAdapter daAccount;
        private DataSet dsAccount;
        private Dictionary<string, int> accountCache = null;
        private IClassifier clsAccount;
        private int nullAccount = -1;

        #endregion Классификаторы

        #region Иерархия классификаторов

        // Ключ - подчиненный код (-1 - любой код), значение - родительский код
        private Dictionary<int, int> kvsrHierarchy = new Dictionary<int, int>();

        private Dictionary<int, int> marksOutDebtHierarchy2004 = new Dictionary<int, int>();
        private Dictionary<int, int> marksOutDebtHierarchy2005 = new Dictionary<int, int>();
        private Dictionary<int, int> marksOutDebtHierarchy2007 = new Dictionary<int, int>();
        private Dictionary<int, int> marksOutDebtHierarchy2007Pulse = new Dictionary<int, int>();

        private Dictionary<int, int> marksInDebtHierarchy2004 = new Dictionary<int, int>();
        private Dictionary<int, int> marksInDebtHierarchy2005 = new Dictionary<int, int>();
        private Dictionary<int, int> marksInDebtHierarchy2007 = new Dictionary<int, int>();
        private Dictionary<int, int> marksInDebtHierarchy2007Pulse = new Dictionary<int, int>();

        private Dictionary<int, int> marksArrearsHierarchy2004 = new Dictionary<int, int>();
        private Dictionary<int, int> marksArrearsHierarchy2005 = new Dictionary<int, int>();
        private Dictionary<int, int> marksArrearsHierarchy2007 = new Dictionary<int, int>();
        private Dictionary<int, int> marksArrearsHierarchy2007Pulse = new Dictionary<int, int>();

        private Dictionary<int, int> marksOutcomesHierarchy2004 = new Dictionary<int, int>();
        private Dictionary<int, int> marksOutcomesHierarchy2005 = new Dictionary<int, int>();
        private Dictionary<int, int> marksOutcomesHierarchy2007 = new Dictionary<int, int>();

        #endregion

        #region блоки

        private Block block;
        private bool toPumpIncomes;
        private bool toPumpOutcomes;
        private bool toPumpDefProf;
        private bool toPumpInnerFinSources;
        private bool toPumpOuterFinSources;
        private bool toPumpIncomesRefs;
        private bool toPumpOutcomesRefs;
        private bool toPumpOutcomesRefsAdd;
        private bool toPumpInnerFinSourcesRefs;
        private bool toPumpOuterFinSourcesRefs;
        private bool toPumpArrearsRefs;
        private bool toPumpExcessRefs;
        private bool toPumpAccount;
        private bool toPumpArrears;

        #endregion блоки

        #endregion Поля

        #region Инициализация

        /// <summary>
        /// Конструктор
        /// </summary>
        public SKIFMonthRepPumpModule()
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

        #region константы

        private enum Block
        {
            bIncomes,
            bOutcomes,
            bDefProf,
            bInnerFinSources,
            bOuterFinSources,
            bIncomesRefs,
            bOutcomesRefs,
            bOutcomesRefsAdd,
            bInnerFinSourcesRefs,
            bOuterFinSourcesRefs,
            bArrearsRefs,
            bExcessRefs,
            bAccount,
            bRegions,
            bArrears
        }

        #endregion константы

        #region Закачка данных

        #region Работа с базой и кэшами

        #region GUID

        private const string FX_MEANS_TYPE_SKIF_GUID = "34c65dbf-0bd9-4e5d-a345-00151b4f97eb";
        private const string FX_TYPE_ARREARS_GUID = "0b6d83a4-ad89-4a09-9212-a5f631d4ade7";

        private const string D_REGIONS_FOR_PUMP_SKIF_GUID = "e9a95119-21f1-43d8-8dc2-8d4af7c195d0";
        private const string D_FKR_ANALYSIS_GUID = "59ae5067-beb9-4ff6-b5b2-38d606fde212";

        private const string D_REGIONS_MONTH_REP_GUID = "e6f74010-8362-4480-a999-279f5155dc9a";
        private const string D_KD_MONTH_REP_GUID = "e151c4f4-9fbd-4234-bb55-c6427a995963";
        private const string D_SRC_OUT_FIN_MONTH_REP_GUID = "8dcb79f0-9020-450f-b8e7-8d4d21549311";
        private const string D_SRC_IN_FIN_MONTH_REP_GUID = "a4d4fa53-0061-4c24-9879-dc037448cc2a";
        private const string D_FKR_MONTH_REP_GUID = "0299a09f-9d23-4e6c-b39a-930cbe219c3a";
        private const string D_FKR_MONTH_REP_BOOK_GUID = "c894bd09-fcfd-4b59-b0f8-e768579c92b1";
        private const string D_EKR_MONTH_REP_BOOK_GUID = "a80d1371-428a-4684-9cec-c781b66ead07";
        private const string D_KVSR_MONTH_REP_GUID = "ac763971-1e48-438e-9b47-7ff69e0deeda";
        private const string D_MARKS_MONTH_REP_OUT_DEBT_GUID = "6956d267-2362-4b2a-af06-22a0490d34a5";
        private const string D_MARKS_MONTH_REP_IN_DEBT_GUID = "57cdd72b-29d4-4d17-8802-4bc45939645d";
        private const string D_MARKS_MONTH_REP_ARREARS_GUID = "785d195b-b550-4bd8-8e55-3fa79beefd44";
        private const string D_MARKS_MONTH_REP_OUTCOMES_GUID = "3e3f5488-3b59-4432-a10e-3b242fb6c40c";
        private const string D_EKR_MONTH_REP_GUID = "86f29f0a-5e84-48ba-8f64-1375439498f9";
        private const string D_MARKS_EXCESS_GUID = "26176f06-f7bb-49e9-a0c6-8325f2d1d585";
        private const string D_MARKS_ACCOUNT_GUID = "9da1d685-7a9e-49c4-a7f2-70bc56903296";
        private const string D_ACCOUNT_MONTH_REP_GUID = "f24b024d-5b8e-42dd-ab63-29c5781e558b";

        private const string F_F_MONTH_REP_DEF_PROF_GUID = "99821468-8a3d-433f-81fa-f133ddbfb2d5";
        private const string F_F_MONTH_REP_INCOMES_GUID = "98f750c5-628d-431e-9608-363ad5b8d1fc";
        private const string F_F_MONTH_REP_OUT_FIN_GUID = "18b9cf93-c310-4652-9693-10eb940f3675";
        private const string F_F_MONTH_REP_IN_FIN_GUID = "2366942c-fbc9-43d8-9c23-6e8d4bf7fb84";
        private const string F_F_MONTH_REP_OUTCOMES_GUID = "ecd2edd2-030f-4b17-9fdf-0b61ea99de80";
        private const string F_F_MONTH_REP_OUT_DEBT_BOOKS_GUID = "0679d356-3f6d-45d4-bc4e-b5cc95e054a7";
        private const string F_F_MONTH_REP_IN_DEBT_BOOKS_GUID = "65bd6a0d-d518-4c70-a666-64369a7d04fb";
        private const string F_F_MONTH_REP_INCOMES_BOOKS_GUID = "456c9610-2681-4bf7-b286-abf44cc13ed0";
        private const string F_F_MONTH_REP_ARREARS_BOOKS_GUID = "ced8fd6f-fc53-4162-aa29-998b5b914e77";
        private const string F_F_MONTH_REP_OUTCOMES_BOOKS_GUID = "ebcd0b75-14ea-437a-bb95-c76702a81dab";
        private const string F_F_MONTH_REP_OUTCOMES_BOOKS_ADD_GUID = "3ce9d910-b154-4600-b5bb-ad2614dd1e66";
        private const string F_F_MONTH_REP_EXCESS_BOOKS_GUID = "caee1001-1608-4ba3-b490-a738344f2282";
        private const string F_F_MONTH_REP_ACCOUNT_GUID = "043f97b0-4057-4dfa-8567-548a891fd7fa";
        private const string F_F_MONTH_REP_ARREARS_GUID = "b84af6ae-ec44-46ca-bf51-6fb034a3cc3e";

        #endregion GUID
        protected override void InitDBObjects()
        {
            GetPumpedBlocks();

            clsAnalFKR = this.Scheme.Classifiers[D_FKR_ANALYSIS_GUID];
            clsRegions4Pump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_SKIF_GUID];

            // если структур бд нет - блок качать не нужно
            if (toPumpArrears)
                toPumpArrears = this.Scheme.Classifiers.ContainsKey(D_ACCOUNT_MONTH_REP_GUID);

            this.UsedClassifiers = new IClassifier[] {
                clsRegions = this.Scheme.Classifiers[D_REGIONS_MONTH_REP_GUID],
                clsKD = this.Scheme.Classifiers[D_KD_MONTH_REP_GUID],
                clsSrcOutFin = this.Scheme.Classifiers[D_SRC_OUT_FIN_MONTH_REP_GUID],
                clsSrcInFin = this.Scheme.Classifiers[D_SRC_IN_FIN_MONTH_REP_GUID],
                clsFKR = this.Scheme.Classifiers[D_FKR_MONTH_REP_GUID],
                clsFKRBook = this.Scheme.Classifiers[D_FKR_MONTH_REP_BOOK_GUID],
                clsEKRBook = this.Scheme.Classifiers[D_EKR_MONTH_REP_BOOK_GUID],
                clsKVSR = this.Scheme.Classifiers[D_KVSR_MONTH_REP_GUID],
                clsMarksOutDebt = this.Scheme.Classifiers[D_MARKS_MONTH_REP_OUT_DEBT_GUID],
                clsMarksInDebt = this.Scheme.Classifiers[D_MARKS_MONTH_REP_IN_DEBT_GUID],
                clsMarksArrears = this.Scheme.Classifiers[D_MARKS_MONTH_REP_ARREARS_GUID],
                clsMarksOutcomes = this.Scheme.Classifiers[D_MARKS_MONTH_REP_OUTCOMES_GUID],
                clsEKR = this.Scheme.Classifiers[D_EKR_MONTH_REP_GUID],
                clsMarksExcess = this.Scheme.Classifiers[D_MARKS_EXCESS_GUID],
                clsMarksAccount = this.Scheme.Classifiers[D_MARKS_ACCOUNT_GUID] };

            this.CubeClassifiers = (IClassifier[])CommonRoutines.ConcatArrays(this.UsedClassifiers,
                new IClassifier[] { fxcMeansType = this.Scheme.Classifiers[FX_MEANS_TYPE_SKIF_GUID] });

            this.VersionClassifiers = new IClassifier[] { clsKD, clsSrcInFin, clsSrcOutFin };

            this.UsedFacts = new IFactTable[] {
                fctMonthRepDefProf = this.Scheme.FactTables[F_F_MONTH_REP_DEF_PROF_GUID],
                fctMonthRepIncomes = this.Scheme.FactTables[F_F_MONTH_REP_INCOMES_GUID],
                fctMonthRepOutFin = this.Scheme.FactTables[F_F_MONTH_REP_OUT_FIN_GUID],
                fctMonthRepInFin = this.Scheme.FactTables[F_F_MONTH_REP_IN_FIN_GUID],
                fctMonthRepOutcomes = this.Scheme.FactTables[F_F_MONTH_REP_OUTCOMES_GUID],
                fctMonthRepOutDebtBooks = this.Scheme.FactTables[F_F_MONTH_REP_OUT_DEBT_BOOKS_GUID],
                fctMonthRepInDebtBooks = this.Scheme.FactTables[F_F_MONTH_REP_IN_DEBT_BOOKS_GUID],
                fctMonthRepIncomesBooks = this.Scheme.FactTables[F_F_MONTH_REP_INCOMES_BOOKS_GUID],
                fctMonthRepArrearsBooks = this.Scheme.FactTables[F_F_MONTH_REP_ARREARS_BOOKS_GUID],
                fctMonthRepOutcomesBooks = this.Scheme.FactTables[F_F_MONTH_REP_OUTCOMES_BOOKS_GUID],
                fctMonthRepOutcomesBooksEx = this.Scheme.FactTables[F_F_MONTH_REP_OUTCOMES_BOOKS_ADD_GUID],
                fctMonthRepExcessBooks = this.Scheme.FactTables[F_F_MONTH_REP_EXCESS_BOOKS_GUID],
                fctMonthRepAccount = this.Scheme.FactTables[F_F_MONTH_REP_ACCOUNT_GUID] };

            this.CubeFacts = new IFactTable[] { };
            if (toPumpIncomes)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepIncomes });
            if (toPumpOutcomes)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepOutcomes });
            if (toPumpDefProf)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepDefProf });
            if (toPumpInnerFinSources)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepInFin });
            if (toPumpOuterFinSources)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepOutFin });
            if (toPumpAccount)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepAccount });
            if (toPumpIncomesRefs)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepIncomesBooks });
            if (toPumpOutcomesRefs)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepOutcomesBooks });
            if (toPumpOutcomesRefsAdd)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepOutcomesBooksEx });
            if (toPumpInnerFinSourcesRefs)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepInDebtBooks });
            if (toPumpOuterFinSourcesRefs)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepOutDebtBooks });
            if (toPumpArrearsRefs)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepArrearsBooks });
            if (toPumpExcessRefs)
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctMonthRepExcessBooks });
            if (toPumpArrears)
            {
                clsAccount = this.Scheme.Classifiers[D_ACCOUNT_MONTH_REP_GUID];
                this.CubeClassifiers = (IClassifier[])CommonRoutines.ConcatArrays(this.CubeClassifiers,
                    new IClassifier[] { clsAccount, this.Scheme.Classifiers[FX_TYPE_ARREARS_GUID] });
                this.dimensionsForProcess = new string[] { F_F_MONTH_REP_ARREARS_GUID, "МесОтч_Задолженность" };
                fctMonthRepArrears = this.Scheme.FactTables[F_F_MONTH_REP_ARREARS_GUID];
                this.UsedFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.UsedFacts, new IFactTable[] { fctMonthRepArrears });
                this.CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(this.CubeFacts, new IFactTable[] { fctMonthRepArrears });
            }
        }

        private void FillCache()
        {
            FillRegionsCache(ref regionCache, dsRegions.Tables[0], "ID");
            FillRegionsCache(ref region4PumpCache, dsRegions4Pump.Tables[0], "REFDOCTYPE");

            FillRowsCache(ref analFKRCache, dsAnalFKR.Tables[0], "CODE", "NAME");

            FillRowsCache(ref kvsrCache, dsKVSR.Tables[0], "CODE");
            FillRowsCache(ref marksOutcomesCache, dsMarksOutcomes.Tables[0], "LONGCODE");
            FillRowsCache(ref scrOutFinSourcesRefCache, dsMarksOutDebt.Tables[0], "LONGCODE");
            FillRowsCache(ref scrInFinSourcesRefCache, dsMarksInDebt.Tables[0], "LONGCODE");
            FillRowsCache(ref arrearsCache, dsMarksArrears.Tables[0], "LONGCODE");
            FillRowsCache(ref fkrCache, dsFKR.Tables[0], "CODE");
            FillRowsCache(ref ekrCache, dsEKR.Tables[0], "CODE");
            FillRowsCache(ref fkrBookCache, dsFKRBook.Tables[0], "CODE");
            FillRowsCache(ref ekrBookCache, dsEKRBook.Tables[0], "CODE");
            FillRowsCache(ref excessCache, dsMarksExcess.Tables[0], "LongCode");
            FillRowsCache(ref kdCache, dsKD.Tables[0], "CODESTR");
            FillRowsCache(ref srcOutFinCache, dsSrcOutFin.Tables[0], "CODESTR");
            FillRowsCache(ref srcInFinCache, dsSrcInFin.Tables[0], "CODESTR");
            FillRowsCache(ref srcOutFinWithKstCache, dsSrcOutFin.Tables[0], new string[] { "CODESTR", "Kst" }, "|", "Id");
            FillRowsCache(ref srcInFinWithKstCache, dsSrcInFin.Tables[0], new string[] { "CODESTR", "Kst" }, "|", "Id");
            FillRowsCache(ref marksAccountCache, dsMarksAccount.Tables[0], "CODE");
            if (toPumpArrears)
                FillRowsCache(ref accountCache, dsAccount.Tables[0], "CODE");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daMeansType, ref dsMeansType, fxcMeansType, true, string.Empty, string.Empty);

            regForPumpSourceID = GetRegions4PumpSourceID();
            yearSourceID = GetYearSourceID();
            InitDataSet(ref daRegions4Pump, ref dsRegions4Pump, clsRegions4Pump, false,
                string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);

            InitDataSet(ref daAnalFKR, ref dsAnalFKR, clsAnalFKR, false,
                string.Format("SOURCEID = {0}", regForPumpSourceID), string.Empty);

            if (toPumpArrears)
                InitDataSet(ref daAccount, ref dsAccount, clsAccount, false,
                    string.Format("SOURCEID = {0}", yearSourceID), string.Empty);

            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daSrcOutFin, ref dsSrcOutFin, clsSrcOutFin, false, string.Empty);
            InitClsDataSet(ref daSrcInFin, ref dsSrcInFin, clsSrcInFin, false, string.Empty);
            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR, false, string.Empty);
            InitClsDataSet(ref daFKRBook, ref dsFKRBook, clsFKRBook, false, string.Empty);
            InitClsDataSet(ref daEKRBook, ref dsEKRBook, clsEKRBook, false, string.Empty);
            InitClsDataSet(ref daMarksArrears, ref dsMarksArrears, clsMarksArrears, false, string.Empty);
            InitClsDataSet(ref daMarksInDebt, ref dsMarksInDebt, clsMarksInDebt, false, string.Empty);
            InitClsDataSet(ref daKVSR, ref dsKVSR, clsKVSR, false, string.Empty);
            InitClsDataSet(ref daMarksOutcomes, ref dsMarksOutcomes, clsMarksOutcomes, false, string.Empty);
            InitClsDataSet(ref daMarksOutDebt, ref dsMarksOutDebt, clsMarksOutDebt, false, string.Empty);
            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR, false, string.Empty);
            InitClsDataSet(ref daMarksExcess, ref dsMarksExcess, clsMarksExcess, false, string.Empty);
            InitClsDataSet(ref daMarksAccount, ref dsMarksAccount, clsMarksAccount, false, string.Empty);

            InitFactDataSet(ref daMonthRepDefProf, ref dsMonthRepDefProf, fctMonthRepDefProf);
            InitFactDataSet(ref daMonthRepIncomes, ref dsMonthRepIncomes, fctMonthRepIncomes);
            InitFactDataSet(ref daMonthRepOutFin, ref dsMonthRepOutFin, fctMonthRepOutFin);
            InitFactDataSet(ref daMonthRepInFin, ref dsMonthRepInFin, fctMonthRepInFin);
            InitFactDataSet(ref daMonthRepOutcomes, ref dsMonthRepOutcomes, fctMonthRepOutcomes);
            InitFactDataSet(ref daMonthRepOutDebtBooks, ref dsMonthRepOutDebtBooks, fctMonthRepOutDebtBooks);
            InitFactDataSet(ref daMonthRepInDebtBooks, ref dsMonthRepInDebtBooks, fctMonthRepInDebtBooks);
            InitFactDataSet(ref daMonthRepIncomesBooks, ref dsMonthRepIncomesBooks, fctMonthRepIncomesBooks);
            InitFactDataSet(ref daMonthRepArrearsBooks, ref dsMonthRepArrearsBooks, fctMonthRepArrearsBooks);
            InitFactDataSet(ref daMonthRepOutcomesBooks, ref dsMonthRepOutcomesBooks, fctMonthRepOutcomesBooks);
            InitFactDataSet(ref daMonthRepOutcomesBooksEx, ref dsMonthRepOutcomesBooksEx, fctMonthRepOutcomesBooksEx);
            InitFactDataSet(ref daMonthRepExcessBooks, ref dsMonthRepExcessBooks, fctMonthRepExcessBooks);
            InitFactDataSet(ref daMonthRepAccount, ref dsMonthRepAccount, fctMonthRepAccount);
            if (toPumpArrears)
                InitFactDataSet(ref daMonthRepArrears, ref dsMonthRepArrears, fctMonthRepArrears);

            InitUpdatedFixedRows();
            FillCache();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daRegions4Pump, dsRegions4Pump, clsRegions4Pump);
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daSrcOutFin, dsSrcOutFin, clsSrcOutFin);
            UpdateDataSet(daSrcInFin, dsSrcInFin, clsSrcInFin);
            UpdateDataSet(daFKR, dsFKR, clsFKR);
            UpdateDataSet(daEKR, dsEKR, clsEKR);
            UpdateDataSet(daFKRBook, dsFKRBook, clsFKRBook);
            UpdateDataSet(daEKRBook, dsEKRBook, clsEKRBook);
            UpdateDataSet(daMarksOutDebt, dsMarksOutDebt, clsMarksOutDebt);
            UpdateDataSet(daMarksInDebt, dsMarksInDebt, clsMarksInDebt);
            UpdateDataSet(daKVSR, dsKVSR, clsKVSR);
            UpdateDataSet(daMarksArrears, dsMarksArrears, clsMarksArrears);
            UpdateDataSet(daMarksOutcomes, dsMarksOutcomes, clsMarksOutcomes);
            UpdateDataSet(daMarksExcess, dsMarksExcess, clsMarksExcess);
            UpdateDataSet(daMarksAccount, dsMarksAccount, clsMarksAccount);
            if (toPumpArrears)
                UpdateDataSet(daAccount, dsAccount, clsAccount);

            UpdateDataSet(daMonthRepDefProf, dsMonthRepDefProf, fctMonthRepDefProf);
            UpdateDataSet(daMonthRepIncomes, dsMonthRepIncomes, fctMonthRepIncomes);
            UpdateDataSet(daMonthRepOutFin, dsMonthRepOutFin, fctMonthRepOutFin);
            UpdateDataSet(daMonthRepInFin, dsMonthRepInFin, fctMonthRepInFin);
            UpdateDataSet(daMonthRepOutcomes, dsMonthRepOutcomes, fctMonthRepOutcomes);
            UpdateDataSet(daMonthRepOutDebtBooks, dsMonthRepOutDebtBooks, fctMonthRepOutDebtBooks);
            UpdateDataSet(daMonthRepInDebtBooks, dsMonthRepInDebtBooks, fctMonthRepInDebtBooks);
            UpdateDataSet(daMonthRepIncomesBooks, dsMonthRepIncomesBooks, fctMonthRepIncomesBooks);
            UpdateDataSet(daMonthRepArrearsBooks, dsMonthRepArrearsBooks, fctMonthRepArrearsBooks);
            UpdateDataSet(daMonthRepOutcomesBooks, dsMonthRepOutcomesBooks, fctMonthRepOutcomesBooks);
            UpdateDataSet(daMonthRepOutcomesBooksEx, dsMonthRepOutcomesBooksEx, fctMonthRepOutcomesBooksEx);
            UpdateDataSet(daMonthRepExcessBooks, dsMonthRepExcessBooks, fctMonthRepExcessBooks);
            UpdateDataSet(daMonthRepAccount, dsMonthRepAccount, fctMonthRepAccount);
            if (toPumpArrears)
                UpdateDataSet(daMonthRepArrears, dsMonthRepArrears, fctMonthRepArrears);
        }

        private void GetPumpedBlocks()
        {
            toPumpIncomes = (ToPumpBlock(Block.bIncomes));
            toPumpOutcomes = (ToPumpBlock(Block.bOutcomes));
            toPumpDefProf = (ToPumpBlock(Block.bDefProf));
            toPumpInnerFinSources = (ToPumpBlock(Block.bInnerFinSources));
            toPumpOuterFinSources = (ToPumpBlock(Block.bOuterFinSources));
            toPumpAccount = (ToPumpBlock(Block.bAccount));
            toPumpIncomesRefs = (ToPumpBlock(Block.bIncomesRefs));
            toPumpOutcomesRefs = (ToPumpBlock(Block.bOutcomesRefs));
            toPumpOutcomesRefsAdd = (ToPumpBlock(Block.bOutcomesRefsAdd));
            toPumpInnerFinSourcesRefs = (ToPumpBlock(Block.bInnerFinSourcesRefs));
            toPumpOuterFinSourcesRefs = (ToPumpBlock(Block.bOuterFinSourcesRefs));
            toPumpArrearsRefs = (ToPumpBlock(Block.bArrearsRefs));
            toPumpExcessRefs = (ToPumpBlock(Block.bExcessRefs));
            toPumpArrears = (ToPumpBlock(Block.bArrears));
        }

        private void InitUpdatedFixedRows()
        {
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullSrcOutFin = clsSrcOutFin.UpdateFixedRows(this.DB, this.SourceID);
            nullSrcInFin = clsSrcInFin.UpdateFixedRows(this.DB, this.SourceID);
            nullFKR = clsFKR.UpdateFixedRows(this.DB, this.SourceID);
            nullFKRBook = clsFKRBook.UpdateFixedRows(this.DB, this.SourceID);
            nullEKRBook = clsEKRBook.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksOutDebt = clsMarksOutDebt.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksInDebt = clsMarksInDebt.UpdateFixedRows(this.DB, this.SourceID);
            nullKVSR = clsKVSR.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksArrears = clsMarksArrears.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksOutcomes = clsMarksOutcomes.UpdateFixedRows(this.DB, this.SourceID);
            nullEKR = clsEKR.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksExcess = clsMarksExcess.UpdateFixedRows(this.DB, this.SourceID);
            nullMarksAccount = clsMarksAccount.UpdateFixedRows(this.DB, this.SourceID);
            if (toPumpArrears)
                nullAccount = clsAccount.UpdateFixedRows(this.DB, yearSourceID);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMonthRepDefProf);
            ClearDataSet(ref dsMonthRepIncomes);
            ClearDataSet(ref dsMonthRepOutFin);
            ClearDataSet(ref dsMonthRepInFin);
            ClearDataSet(ref dsMonthRepOutcomes);
            ClearDataSet(ref dsMonthRepOutDebtBooks);
            ClearDataSet(ref dsMonthRepInDebtBooks);
            ClearDataSet(ref dsMonthRepIncomesBooks);
            ClearDataSet(ref dsMonthRepArrearsBooks);
            ClearDataSet(ref dsMonthRepOutcomesBooks);
            ClearDataSet(ref dsMonthRepOutcomesBooksEx);
            ClearDataSet(ref dsMonthRepExcessBooks);
            ClearDataSet(ref dsMonthRepAccount);
            if (toPumpArrears)
                ClearDataSet(ref dsMonthRepArrears);

            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsRegions4Pump);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsSrcOutFin);
            ClearDataSet(ref dsSrcInFin);
            ClearDataSet(ref dsFKR);
            ClearDataSet(ref dsFKRBook);
            ClearDataSet(ref dsEKRBook);
            ClearDataSet(ref dsMarksOutDebt);
            ClearDataSet(ref dsMarksInDebt);
            ClearDataSet(ref dsKVSR);
            ClearDataSet(ref dsMarksArrears);
            ClearDataSet(ref dsMarksOutcomes);
            ClearDataSet(ref dsMeansType);
            ClearDataSet(ref dsEKR);
            ClearDataSet(ref dsMarksExcess);
            ClearDataSet(ref dsMarksAccount);

            ClearDataSet(ref dsAnalFKR);
            if (toPumpArrears)
                ClearDataSet(ref dsAccount);

            pumpedRegions.Clear();

            kvrAuxCache.Clear();
            kcsrAuxCache.Clear();
            fkrAuxCache.Clear();
        }

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
                case Block.bInnerFinSources:
                    return "ucbInnerFinSources";
                case Block.bOuterFinSources:
                    return "ucbOuterFinSources";
                case Block.bIncomesRefs:
                    return "ucbIncomesRefs";
                case Block.bOutcomesRefs:
                    return "ucbOutcomesRefs";
                case Block.bOutcomesRefsAdd:
                    return "ucbOutcomesRefsAdd";
                case Block.bInnerFinSourcesRefs:
                    return "ucbInnerFinSourcesRefs";
                case Block.bOuterFinSourcesRefs:
                    return "ucbOuterFinSourcesRefs";
                case Block.bArrearsRefs:
                    return "ucbArrearsRefs";
                case Block.bExcessRefs:
                    return "ucbExcessRefs";
                case Block.bAccount:
                    return "ucbAccounts";
                case Block.bArrears:
                    return "ucbArrears";
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
                DirectDeleteFactData(new IFactTable[] { fctMonthRepIncomes }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsKD }, -1, this.SourceID, string.Empty);
            }
            // расходы
            if (ToPumpBlock(Block.bOutcomes))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepOutcomes }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsEKR, clsFKR }, -1, this.SourceID, string.Empty);
            }
            // дефицит профицит
            if (ToPumpBlock(Block.bDefProf))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepDefProf }, -1, this.SourceID, string.Empty);
            }
            // источники внутреннего финансирования
            if (ToPumpBlock(Block.bInnerFinSources))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepInFin }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsSrcInFin }, -1, this.SourceID, string.Empty);
            }
            // источники внешнего финансирования
            if (ToPumpBlock(Block.bOuterFinSources))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepOutFin }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsSrcOutFin }, -1, this.SourceID, string.Empty);
            }
            // конс расходы
            if (ToPumpBlock(Block.bAccount))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepAccount }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksAccount }, -1, this.SourceID, string.Empty);
            }
            // справДоходы
            if (ToPumpBlock(Block.bIncomesRefs))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepIncomesBooks }, -1, this.SourceID, string.Empty);
            }

            if ((ToPumpBlock(Block.bIncomesRefs)) && (ToPumpBlock(Block.bOutcomes)))
            {
                DirectDeleteClsData(new IClassifier[] { clsKVSR }, -1, this.SourceID, string.Empty);
            }

            // справРасходы
            if (ToPumpBlock(Block.bOutcomesRefs))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepOutcomesBooks }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsEKRBook, clsFKRBook }, -1, this.SourceID, string.Empty);
            }
            // справРасходыДоп
            if (ToPumpBlock(Block.bOutcomesRefsAdd))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepOutcomesBooksEx }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksOutcomes }, -1, this.SourceID, string.Empty);
            }
            // справВнутреннийДолг
            if (ToPumpBlock(Block.bInnerFinSourcesRefs))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepInDebtBooks }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksInDebt }, -1, this.SourceID, string.Empty);
            }
            // справВнешнийДолг
            if (ToPumpBlock(Block.bOuterFinSourcesRefs))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepOutDebtBooks }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksOutDebt }, -1, this.SourceID, string.Empty);
            }
            // справЗадолженность
            if (ToPumpBlock(Block.bArrearsRefs))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepArrearsBooks }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksArrears }, -1, this.SourceID, string.Empty);
            }
            // справОстатки
            if (ToPumpBlock(Block.bExcessRefs))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepExcessBooks }, -1, this.SourceID, string.Empty);
                DirectDeleteClsData(new IClassifier[] { clsMarksExcess }, -1, this.SourceID, string.Empty);
            }
            // кредиторская и дебиторская задолженность
            if (ToPumpBlock(Block.bArrears))
            {
                DirectDeleteFactData(new IFactTable[] { fctMonthRepArrears }, -1, this.SourceID, string.Empty);
            }
            // районы удаляем, если закачиваются все блоки
            if (ToPumpBlock(Block.bIncomes) && ToPumpBlock(Block.bOutcomes) && ToPumpBlock(Block.bDefProf) &&
                ToPumpBlock(Block.bInnerFinSources) && ToPumpBlock(Block.bOuterFinSources) && ToPumpBlock(Block.bIncomesRefs) &&
                ToPumpBlock(Block.bOutcomesRefs) && ToPumpBlock(Block.bOutcomesRefsAdd) && ToPumpBlock(Block.bInnerFinSourcesRefs) &&
                ToPumpBlock(Block.bOuterFinSourcesRefs) && ToPumpBlock(Block.bArrearsRefs) && ToPumpBlock(Block.bExcessRefs) &&
                ToPumpBlock(Block.bAccount) && ToPumpBlock(Block.bArrears))
            {
                DirectDeleteClsData(new IClassifier[] { clsRegions }, -1, this.SourceID, string.Empty);
            }
        }

        #endregion Работа с базой и кэшами

        #region иерархия

        // мочим записи, по которым нет данных
        private void DeleteUnusedKD(IFactTable fct, ref IClassifier cls, ref IDbDataAdapter daCls, ref DataSet dsCls, string refFieldName)
        {
            string query = string.Format("update {0} set parentId = null where sourceid = {1}",
                cls.FullDBName, this.SourceID);
            if (dsCls.Tables[0].Columns.Contains("ParentId"))
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });

            int recCount = dsCls.Tables[0].Rows.Count;
            query = string.Format("delete from {0} where sourceid = {2} and id not in (select distinct {3} from {1} where sourceid = {2})",
                cls.FullDBName, fct.FullDBName, this.SourceID, refFieldName);

            if (cls == clsKVSR)
                query += string.Format(" and (id not in (select distinct {0} from {1} where sourceid = {2}))",
                    "RefKVSR", fctMonthRepIncomesBooks.FullDBName, this.SourceID);

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

        private void SetClsHeirKd2011()
        {
            SetClsHierarchy(ref dsKD, clsKD, null, "CodeStr", ClsHierarchyMode.Standard);
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010102000010000110'",
                "CodeStr = '00010102011010000110'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00010202000000000000'",
                "CodeStr = '00010202041060000160'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011105000000000120'",
                "CodeStr = '00011105026000000120'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011110000010000120'",
                "CodeStr = '00011111000010000120'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011301400010000000'",
                "CodeStr = '00011301410010000130' or CodeStr = '00011301420010000180' or " +
                "CodeStr = '00011301430010000410' or CodeStr = '00011301440010000420' or " +
                "CodeStr = '00011301450010000440'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011706000000000180'",
                "CodeStr = '00011706011060000180'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                "CodeStr = '00020205112060000151' or CodeStr = '00020205113060000151' or " +
                "CodeStr = '00020205114060000151' or CodeStr = '00020205115060000151' or " +
                "CodeStr = '00020205116060000151' or CodeStr = '00020205117060000151' or " +
                "CodeStr = '00020205119060000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                "CodeStr = '00020205120060000151' or CodeStr = '00020205122060000151' or " +
                "CodeStr = '00020205123060000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205300070000151'",
                "CodeStr = '00020205310070000151' or CodeStr = '00020205311070000151' or " +
                "CodeStr = '00020205312070000151' or CodeStr = '00020205313070000151' or " +
                "CodeStr = '00020205314070000151' or CodeStr = '00020205315070000151' or " +
                "CodeStr = '00020205316070000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205800090000151'",
                "CodeStr = '00020205810090000151' or CodeStr = '00020205811090000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020701010010000180'",
                "CodeStr = '00020701011010000180' or CodeStr = '00020701012010000180'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00021906010000000151'",
                "CodeStr = '00021906011060000151' or CodeStr = '00021906012070000151' or " +
                "CodeStr = '00021906013080000151' or CodeStr = '00021906014090000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00021906020000000151'",
                "CodeStr = '00021906021060000151' or CodeStr = '00021906022070000151' or " +
                "CodeStr = '00021906023080000151' or CodeStr = '00021906024090000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00021906030000000151'",
                "CodeStr = '00021906031060000151' or CodeStr = '00021906032070000151' or " +
                "CodeStr = '00021906033080000151' or CodeStr = '00021906034090000151'");
            FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00087000000000000000'",
                "CodeStr = '00087000000000001151' or CodeStr = '00087000000000002151'");
            // доп иерархия с 2011 11
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 201111)
            {
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00011600000000000000'",
                    "CodeStr = '00011630010010000140'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020202000000000151'",
                    "CodeStr = '00020202133020000151'");
            }
        }

        /// <summary>
        /// Вызывает функции установки иерархии для всех классификаторов
        /// </summary>
        protected override void SetClsHierarchy()
        {
            WriteToTrace("Начало установки доп. иерархии", TraceMessageKind.Information);

            int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
            // очередная тупопездность постановки - типа надо удалять записи кд, по которым нет данных факта
            DeleteUnusedKD(fctMonthRepIncomes, ref clsKD, ref daKD, ref dsKD, "refKd");
            // Доходы
            ClearHierarchy(dsKD.Tables[0]);
            SetPresentationContext(clsKD);

            if (this.DataSource.Year >= 2011)
            {
                SetClsHeirKd2011();
            }
            else if (this.DataSource.Year >= 2010)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, "CodeStr", ClsHierarchyMode.Standard);
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
                SetClsHierarchy(ref dsKD, clsKD, null, "CodeStr", ClsHierarchyMode.Standard);
                // доп установка для 2008
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
                if ((sourceDate >= 200808) && (sourceDate <= 200812))
                {
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
                }
            }
            else if (this.DataSource.Year >= 2007)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, "CodeStr", ClsHierarchyMode.Standard);
                // доп установка для 2007
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020203900090000151'",
                    "CodeStr = '00020203901090000151' or CodeStr = '00020203902090000151' or " +
                    "CodeStr = '00020203903090000151' or CodeStr = '00020203999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020200000000000000'",
                    "CodeStr = '00020203999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204010000000151'",
                    "CodeStr = '00020204011020000151' or CodeStr = '00020204012020000151' or " +
                    "CodeStr = '00020204013020000151' or CodeStr = '00020204014020000151' or " +
                    "CodeStr = '00020204015020000151' or CodeStr = '00020204016020000151' or " +
                    "CodeStr = '00020204017020000151' or CodeStr = '00020204018020000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204900090000151'",
                    "CodeStr = '00020204901090000151' or CodeStr = '00020204903090000151' or " +
                    "CodeStr = '00020204904090000151' or CodeStr = '00020204905090000151' or " +
                    "CodeStr = '00020204906090000151' or CodeStr = '00020204908090000151' or " +
                    "CodeStr = '00020204999000000151' or CodeStr = '00020204907090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204000000000151'",
                    "CodeStr = '00020204999000000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020205100060000151'",
                    "CodeStr = '00020205110060000151' or CodeStr = '00020205111060000151' or " +
                    "CodeStr = '00020205112060000151'");
            }
            else if (this.DataSource.Year >= 2006)
            {
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020202900000000151'",
                    "CodeStr = '00020202910020000151' or CodeStr = '00020202920030000151' or " +
                    "CodeStr = '00020202930040000151' or CodeStr = '00020202940050000151' or " +
                    "CodeStr = '00020202940100000151' or CodeStr = '00020202950060000151' or " +
                    "CodeStr = '00020202960070000151' or CodeStr = '00020202970080000151' or " +
                    "CodeStr = '00020202980090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204900000000151'",
                    "CodeStr = '00020204910020000151' or CodeStr = '00020204920030000151' or " +
                    "CodeStr = '00020204920040000151' or CodeStr = '00020204930050000151' or " +
                    "CodeStr = '00020204930100000151' or CodeStr = '00020204950060000151' or " +
                    "CodeStr = '00020204960070000151' or CodeStr = '00020204970080000151' or " +
                    "CodeStr = '00020204980090000151'");
                FormClsGroupHierarchy(dsKD.Tables[0], "CodeStr = '00020204000000000151'",
                    "CodeStr = '00020204019020000151' or CodeStr = '00020204241000000151' or " +
                    "CodeStr = '00020204242000000151' or CodeStr = '00020204243000000151' or " +
                    "CodeStr = '00020204244000000151'");
            }
            else if (this.DataSource.Year >= 2005)
            {
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
                SetClsHierarchy(ref dsKD, clsKD, null, string.Empty, ClsHierarchyMode.KD2004);
            // Источники внешнего финансирования
            DeleteUnusedKD(fctMonthRepOutFin, ref clsSrcOutFin, ref daSrcOutFin, ref dsSrcOutFin, "RefSOF");
            SetPresentationContext(clsSrcOutFin);
            SetClsHierarchy(ref dsSrcOutFin, clsSrcOutFin, null, string.Empty, ClsHierarchyMode.Standard);
            // Источники внутреннего финансирования
            DeleteUnusedKD(fctMonthRepInFin, ref clsSrcInFin, ref daSrcInFin, ref dsSrcInFin, "RefSIF");
            SetPresentationContext(clsSrcInFin);
            SetClsHierarchy(ref dsSrcInFin, clsSrcInFin, null, string.Empty, ClsHierarchyMode.Standard);
            if (this.DataSource.Year >= 2008)
            {
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], string.Empty,
                    "CodeStr = '00001050000000000000' or CodeStr = '00001000000000000000' or " +
                    "CodeStr = '00057000000000000000' or CodeStr = '00090000000000000000' ");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '00001000000000000000'",
                    "CodeStr = '0000105000000000000A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105000000000000A'",
                    "CodeStr = '0000105000000000050A' or CodeStr = '0000105000000000060A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105000000000050A'",
                    "CodeStr = '0000105010000000050A' or CodeStr = '0000105020000000050A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105000000000060A'",
                    "CodeStr = '0000105010000000060A' or CodeStr = '0000105020000000060A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105010000000050A'",
                    "CodeStr = '00001050102000000520'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105020000000050A'",
                    "CodeStr = '00001050202000000520' or CodeStr = '00001050201060000550'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105010000000060A'",
                    "CodeStr = '00001050102000000620'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000105020000000060A'",
                    "CodeStr = '00001050202000000620' or CodeStr = '00001050201060000650'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И5000000000000000' or CodeStr = '000И8000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И5000000000000000'",
                    "CodeStr = '000И6000000000000000' or CodeStr = '000И7000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И8000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000106000000000000A'",
                    "CodeStr = '0000106060000000060A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000106000000000000А'",
                    "CodeStr = '0000106060000000050А'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000106060000000060A'",
                    "CodeStr = '00001060608010000610' or CodeStr = '00001060608020000610' or " +
                    "CodeStr = '00001060608030000610' or CodeStr = '00001060608040000610' or " + 
                    "CodeStr = '00001060608050000610' or CodeStr = '00001060608100000610' or " +
                    "CodeStr = '00001060609010000610'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000106060000000050А'",
                    "CodeStr = '00001060608010000510' or CodeStr = '00001060608020000510' or " +
                    "CodeStr = '00001060608030000510' or CodeStr = '00001060608040000510' or " +
                    "CodeStr = '00001060608050000510' or CodeStr = '00001060608100000510' or " +
                    "CodeStr = '00001060609010000510'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000100000000000000A'",
                    "CodeStr = '00001050000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000100000000000000A'",
                    "CodeStr = '0000106000000000000A'");
            }
            else
            {
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '00050000000000000000'",
                    "CodeStr = '00001010000000000000' or CodeStr = '00002010000000000000' or " +
                    "CodeStr = '00005000000000000000' or CodeStr = '00003010000000000000' or " +
                    "CodeStr = '00004010000000000000' or CodeStr = '00005000000000000000' or " +
                    "CodeStr = '00006000000000000000' or CodeStr = '00007000000010000000' or " +
                    "CodeStr = '0000800000000000000A' or CodeStr = '00009000000000000171' or " +
                    "CodeStr = '00010000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000801000000000050A'",
                    "CodeStr = '00008010200000000520'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '00008010200100000520'",
                    "CodeStr = '0000802000000000050A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000801000000000060A'",
                    "CodeStr = '00008010200000000620'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000802000000000060A'",
                    "CodeStr = '00008020200000000620'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000801000000000050A'",
                    "CodeStr = '00008010200000000520'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000802000000000050A'",
                    "CodeStr = '00008020200000000520'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000800000000000000A'",
                    "CodeStr = '0000800000000000050A' or CodeStr = '0000800000000000060A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000800000000000050A'",
                    "CodeStr = '0000801000000000050A' or CodeStr = '0000802000000000050A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000801000000000050A'",
                    "CodeStr = '00008010200000000520'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '00050000000000000000'",
                    "CodeStr = '0000801000000000060A' or CodeStr = '0000802000000000060A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '0000800000000000060A'",
                    "CodeStr = '0000801000000000060A' or CodeStr = '0000802000000000060A'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И5000000000000000' or CodeStr = '000И8000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И5000000000000000'",
                    "CodeStr = '000И6000000000000000' or CodeStr = '000И7000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И4000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '000И8000000000000000'",
                    "CodeStr = '000И9000000000000000' or CodeStr = '000И1000000000000000'");
                FormClsGroupHierarchy(dsSrcInFin.Tables[0], "CodeStr = '00057000000000000000'",
                    "CodeStr = '00050000000000000710' or CodeStr = '00050000000000000810'");
            }

            // Расходы
            SetClsHierarchy(ref dsFKR, clsFKR, null, string.Empty, ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsEKR, clsEKR, null, string.Empty, ClsHierarchyMode.Standard);

            // устанавливаем уровень иерархии
            foreach (DataRow fkrRow in dsFKR.Tables[0].Rows)
            {
                string fkrCode = fkrRow["Code"].ToString().PadLeft(14, '0');
                if (fkrCode.EndsWith("000000000000"))
                    fkrRow["HierarchyLevel"] = 1;
                else if (fkrCode.EndsWith("0000000000"))
                    fkrRow["HierarchyLevel"] = 2;
                else if (fkrCode.EndsWith("0000000"))
                    fkrRow["HierarchyLevel"] = 3;
                else if (fkrCode.EndsWith("00000"))
                    fkrRow["HierarchyLevel"] = 4;
                else if (fkrCode.EndsWith("000"))
                    fkrRow["HierarchyLevel"] = 5;
                else
                    fkrRow["HierarchyLevel"] = 6;
            }

            // СправВнешнийДолг
            if (this.DataSource.Year >= 2011)
            {
                SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksOutDebt.Tables[0], "KST = '10632'", "KST = '10633'");
                FormClsGroupHierarchy(dsMarksOutDebt.Tables[0], "KST = '10634'", "KST = '10635'");
                FormClsGroupHierarchy(dsMarksOutDebt.Tables[0], string.Empty,
                    "KST = '10634' or KST = '10632' or KST = '10640'");
            }
            else if (this.DataSource.Year >= 2010)
            {
                SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksOutDebt.Tables[0], "KST = '9732'", "KST = '9733'");
                FormClsGroupHierarchy(dsMarksOutDebt.Tables[0], "KST = '9734'", "KST = '9735'");
                FormClsGroupHierarchy(dsMarksOutDebt.Tables[0], string.Empty,
                    "KST = '9732' or KST = '9734' or KST = '9740'");
            }
            else if (this.DataSource.Year >= 2008)
                SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, null, "KST", ClsHierarchyMode.Standard);
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                if (this.isKitPatterns)
                    SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, null, "KST", ClsHierarchyMode.Standard);
                else if (this.Region == RegionName.Novosibirsk)
                    SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, marksOutDebtHierarchy2007Pulse, "KST", ClsHierarchyMode.Special);
                else
                    SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, marksOutDebtHierarchy2007, "KL", ClsHierarchyMode.Special);
            }
            else if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, marksOutDebtHierarchy2005, "KL", ClsHierarchyMode.Special);
            else
                SetClsHierarchy(ref dsMarksOutDebt, clsMarksOutDebt, marksOutDebtHierarchy2004, "KL", ClsHierarchyMode.Special);
            // СправВнутреннийДолг
            if (this.DataSource.Year >= 2011)
            {
                SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '10562'", "KST = '10563'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '10564'", "KST = '10565'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '10762'", "KST = '10763'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '10764'", "KST = '10765'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], string.Empty,
                    "KST = '10770' or KST = '10564' or KST = '10562' or KST = '10570' or KST = '10764' or KST = '10762'");
            }
            else if (this.DataSource.Year >= 2010)
            {
                SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '9662'", "KST = '9663'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '9664'", "KST = '9665'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '9862'", "KST = '9863'");
                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], "KST = '9864'", "KST = '9865'");

                FormClsGroupHierarchy(dsMarksInDebt.Tables[0], string.Empty,
                    "KST = '9870' or KST = '9662' or KST = '9664' or KST = '9670' or KST = '9862' or KST = '9864'");
            }
            else if (this.DataSource.Year >= 2008)
            {
                SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, null, "KST", ClsHierarchyMode.Standard);
                if (this.DataSource.Year == 2009)
                    FormClsGroupHierarchy(dsMarksInDebt.Tables[0], string.Empty, "KST = '9999'");
            }
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                if (this.isKitPatterns)
                    SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, null, "KST", ClsHierarchyMode.Standard);
                else if (this.Region == RegionName.Novosibirsk)
                    SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, marksInDebtHierarchy2007Pulse, "KST", ClsHierarchyMode.Special);
                else
                    SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, marksInDebtHierarchy2007, "KL", ClsHierarchyMode.Special);
            }
            else if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, marksInDebtHierarchy2005, "KL", ClsHierarchyMode.Special);
            else
                SetClsHierarchy(ref dsMarksInDebt, clsMarksInDebt, marksInDebtHierarchy2004, "KL", ClsHierarchyMode.Special);
            // СправДоходы
            DeleteUnusedKD(fctMonthRepOutcomes, ref clsKVSR, ref daKVSR, ref dsKVSR, "RefKVSR");
            //SetClsHierarchy(ref dsKVSR, clsKVSR, kvsrHierarchy, "CODE", ClsHierarchyMode.Special);
            // СправЗадолженность
            if (this.DataSource.Year >= 2011)
            {
                SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksArrears.Tables[0], "KST = '10900'", "KST = '10911' or KST = '10912' or KST = '10913'");
                FormClsGroupHierarchy(dsMarksArrears.Tables[0], "KST = '11100'", "KST = '11200' or KST = '11300' or KST = '11400' or KST = '11500'");
                FormClsGroupHierarchy(dsMarksArrears.Tables[0], string.Empty, "KST = '11710'");
            }
            else if (this.DataSource.Year >= 2010)
            {
                SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksArrears.Tables[0], "KST = '10000'", "KST = '10011' or KST = '10012' or KST = '10013'");
            }
            else if (sourceDate >= 200904)
            {
                SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, null, "KST", ClsHierarchyMode.Standard);
                FormClsGroupHierarchy(dsMarksArrears.Tables[0], "KST = '10100'", "KST = '10111' or KST = '10112' or KST = '10113'");
            }
            else if (this.DataSource.Year >= 2008)
                SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, null, "KST", ClsHierarchyMode.Standard);
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                if (this.isKitPatterns)
                {
                    SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, null, "KST", ClsHierarchyMode.Standard);
                    FormClsGroupHierarchy(dsMarksArrears.Tables[0], "KST = '7000'", "KST = '8000'");
                }
                else if (this.Region == RegionName.Novosibirsk)
                    SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, marksArrearsHierarchy2007Pulse, "KST", ClsHierarchyMode.Special);
                else
                    SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, marksArrearsHierarchy2007, "KL", ClsHierarchyMode.Special);
            }
            else if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, marksArrearsHierarchy2005, "KL", ClsHierarchyMode.Special);
            else
                SetClsHierarchy(ref dsMarksArrears, clsMarksArrears, marksArrearsHierarchy2004, "KL", ClsHierarchyMode.Special);
            // СправРасходы
            SetClsHierarchy(ref dsFKRBook, clsFKRBook, null, string.Empty, ClsHierarchyMode.Standard);
            SetClsHierarchy(ref dsEKRBook, clsEKRBook, null, string.Empty, ClsHierarchyMode.Standard);
            // СправРасходыДоп
            if (this.DataSource.Year >= 2008)
            {
                SetClsHierarchy(ref dsMarksOutcomes, clsMarksOutcomes, null, "KST", ClsHierarchyMode.Standard);
                if (this.DataSource.Year >= 2012)
                {
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '170'", "KST = '140' or KST = '150' or KST = '160'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '270'", "KST = '240' or KST = '250' or KST = '260'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], string.Empty, "KST = '750' or KST = '10350'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '1802'", "KST >= '1803' and KST <= '1815'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '7000'", "KST = '7100' or KST = '7200' or KST = '7300' or KST = '7400'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '13000'", "KST = '13100' or KST = '13200' or KST = '13300' or KST = '13400' or KST = '13500' or KST = '13600'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '14000'", "KST = '14100' or KST = '14200' or KST = '14300' or KST = '14400' or KST = '14500' or KST = '14600'");
                }
                else if (this.DataSource.Year >= 2011)
                {
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '1802'", "KST >= '1803' and KST <= '1815'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], string.Empty, "KST = '750' or KST = '10350'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '13000'", "KST = '13100' or KST = '13200' or KST = '13300' or KST = '13400' or KST = '13500' or KST = '13600'");
                }
                else if (this.DataSource.Year >= 2009)
                {
                    string childFilter = "KST = '2011' or KST = '2012' or KST = '2013' or KST = '2014' or KST = '2015' or KST = '2016' or KST = '2017'";
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '2000'", childFilter);
                }
                else
                {
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '2000'", "KST = '2011' or KST = '2012'");
                    FormClsGroupHierarchy(dsMarksOutcomes.Tables[0], "KST = '7000'", "KST = '8000'");
                }
            }
            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                SetClsHierarchy(ref dsMarksOutcomes, clsMarksOutcomes, marksOutcomesHierarchy2007, "KST", ClsHierarchyMode.MarksOutcomes);
            else if (this.DataSource.Year >= 2005)
                SetClsHierarchy(ref dsMarksOutcomes, clsMarksOutcomes, marksOutcomesHierarchy2005, "KL", ClsHierarchyMode.MarksOutcomes);
            else
                SetClsHierarchy(ref dsMarksOutcomes, clsMarksOutcomes, marksOutcomesHierarchy2004, "KL", ClsHierarchyMode.MarksOutcomes);
            // справ остатки
            SetClsHierarchy(ref dsMarksExcess, clsMarksExcess, null, "KST", ClsHierarchyMode.Standard);
            // конс расходы
            SetClsHierarchy(ref dsMarksAccount, clsMarksAccount, null, "Code", ClsHierarchyMode.Standard);
            // районы служебный
            SetClsHierarchy(ref dsRegions4Pump, clsRegions4Pump, null, string.Empty, ClsHierarchyMode.Standard);
        }

        #endregion иерархия

        #region пиздец

        /// <summary>
        /// Заполняет коллекцию с данными по иерархии
        /// </summary>
        private void FillKvsrHierarchy()
        {
            kvsrHierarchy.Clear();
            kvsrHierarchy.Add(-1, 0);
        }

        /// <summary>
        /// Заполняет коллекцию с данными по иерархии - СправВнешнийДолг
        /// </summary>
        private void FillMarksOutDebtHierarchy()
        {
            marksOutDebtHierarchy2004.Clear();
            marksOutDebtHierarchy2004.Add(105, 104);
            marksOutDebtHierarchy2004.Add(106, 105);

            marksOutDebtHierarchy2005.Clear();
            marksOutDebtHierarchy2005.Add(128, 127);
            marksOutDebtHierarchy2005.Add(130, 129);
            marksOutDebtHierarchy2005.Add(131, 130);
            marksOutDebtHierarchy2005.Add(133, 132);
            marksOutDebtHierarchy2005.Add(134, 133);

            marksOutDebtHierarchy2007.Clear();
            marksOutDebtHierarchy2007.Add(20, 10);
            marksOutDebtHierarchy2007.Add(30, 10);
            marksOutDebtHierarchy2007.Add(40, 30);
            marksOutDebtHierarchy2007.Add(50, 10);
            marksOutDebtHierarchy2007.Add(70, 60);

            marksOutDebtHierarchy2007Pulse.Clear();
            marksOutDebtHierarchy2007Pulse.Add(4110, 4100);
            marksOutDebtHierarchy2007Pulse.Add(4120, 4100);
            marksOutDebtHierarchy2007Pulse.Add(4121, 4120);
            marksOutDebtHierarchy2007Pulse.Add(4130, 4100);
            marksOutDebtHierarchy2007Pulse.Add(4200, 4100);
            marksOutDebtHierarchy2007Pulse.Add(4210, 4200);
        }

        /// <summary>
        /// Заполняет коллекцию с данными по иерархии - СправВнутреннийДолг
        /// </summary>
        private void FillMarksInDebtHierarchy()
        {
            marksInDebtHierarchy2004.Clear();
            marksInDebtHierarchy2004.Add(86, 85);
            marksInDebtHierarchy2004.Add(87, 86);
            marksInDebtHierarchy2004.Add(88, 86);
            marksInDebtHierarchy2004.Add(89, 85);
            marksInDebtHierarchy2004.Add(90, 85);
            marksInDebtHierarchy2004.Add(92, 91);
            marksInDebtHierarchy2004.Add(94, 93);
            marksInDebtHierarchy2004.Add(95, 93);
            marksInDebtHierarchy2004.Add(96, 93);
            marksInDebtHierarchy2004.Add(97, 93);
            marksInDebtHierarchy2004.Add(98, 93);
            marksInDebtHierarchy2004.Add(100, 99);
            marksInDebtHierarchy2004.Add(101, 99);
            marksInDebtHierarchy2004.Add(103, 102);

            marksInDebtHierarchy2005.Clear();
            marksInDebtHierarchy2005.Add(77, 76);
            marksInDebtHierarchy2005.Add(78, 77);
            marksInDebtHierarchy2005.Add(79, 77);
            marksInDebtHierarchy2005.Add(80, 77);
            marksInDebtHierarchy2005.Add(81, 77);
            marksInDebtHierarchy2005.Add(82, 76);
            marksInDebtHierarchy2005.Add(84, 83);
            marksInDebtHierarchy2005.Add(85, 84);
            marksInDebtHierarchy2005.Add(86, 85);
            marksInDebtHierarchy2005.Add(87, 86);
            marksInDebtHierarchy2005.Add(88, 86);
            marksInDebtHierarchy2005.Add(89, 86);
            marksInDebtHierarchy2005.Add(90, 86);
            marksInDebtHierarchy2005.Add(91, 85);
            marksInDebtHierarchy2005.Add(92, 85);
            marksInDebtHierarchy2005.Add(93, 84);
            marksInDebtHierarchy2005.Add(94, 93);
            marksInDebtHierarchy2005.Add(95, 94);
            marksInDebtHierarchy2005.Add(96, 94);
            marksInDebtHierarchy2005.Add(97, 93);
            marksInDebtHierarchy2005.Add(98, 93);
            marksInDebtHierarchy2005.Add(99, 83);
            marksInDebtHierarchy2005.Add(100, 99);
            marksInDebtHierarchy2005.Add(101, 99);
            marksInDebtHierarchy2005.Add(102, 99);
            marksInDebtHierarchy2005.Add(103, 83);
            marksInDebtHierarchy2005.Add(104, 103);
            marksInDebtHierarchy2005.Add(105, 103);
            marksInDebtHierarchy2005.Add(107, 106);
            marksInDebtHierarchy2005.Add(108, 107);
            marksInDebtHierarchy2005.Add(109, 108);
            marksInDebtHierarchy2005.Add(110, 108);
            marksInDebtHierarchy2005.Add(111, 108);
            marksInDebtHierarchy2005.Add(112, 107);
            marksInDebtHierarchy2005.Add(113, 112);
            marksInDebtHierarchy2005.Add(114, 112);
            marksInDebtHierarchy2005.Add(116, 115);
            marksInDebtHierarchy2005.Add(117, 116);
            marksInDebtHierarchy2005.Add(118, 117);
            marksInDebtHierarchy2005.Add(119, 117);
            marksInDebtHierarchy2005.Add(120, 117);
            marksInDebtHierarchy2005.Add(121, 116);
            marksInDebtHierarchy2005.Add(122, 121);

            marksInDebtHierarchy2007.Clear();
            marksInDebtHierarchy2007.Add(970, 960);
            marksInDebtHierarchy2007.Add(980, 960);
            marksInDebtHierarchy2007.Add(990, 980);
            marksInDebtHierarchy2007.Add(1000, 980);
            marksInDebtHierarchy2007.Add(1010, 980);
            marksInDebtHierarchy2007.Add(1020, 960);
            marksInDebtHierarchy2007.Add(1030, 1020);
            marksInDebtHierarchy2007.Add(1040, 960);
            marksInDebtHierarchy2007.Add(1050, 960);
            marksInDebtHierarchy2007.Add(1060, 1050);
            marksInDebtHierarchy2007.Add(1070, 1050);
            marksInDebtHierarchy2007.Add(1080, 960);

            marksInDebtHierarchy2007Pulse.Clear();
            marksInDebtHierarchy2007Pulse.Add(4010, 4000);
            marksInDebtHierarchy2007Pulse.Add(4020, 4000);
            marksInDebtHierarchy2007Pulse.Add(4021, 4020);
            marksInDebtHierarchy2007Pulse.Add(4022, 4020);
            marksInDebtHierarchy2007Pulse.Add(4023, 4020);
            marksInDebtHierarchy2007Pulse.Add(4030, 4000);
            marksInDebtHierarchy2007Pulse.Add(4031, 4030);
            marksInDebtHierarchy2007Pulse.Add(4040, 4000);
            marksInDebtHierarchy2007Pulse.Add(4050, 4000);
            marksInDebtHierarchy2007Pulse.Add(4051, 4050);
            marksInDebtHierarchy2007Pulse.Add(4052, 4050);
            marksInDebtHierarchy2007Pulse.Add(4060, 4000);
        }

        /// <summary>
        /// Заполняет коллекцию с данными по иерархии - СправЗадолженность
        /// </summary>
        private void FillMarksArrearsHierarchy()
        {
            marksArrearsHierarchy2004.Clear();
            marksArrearsHierarchy2004.Add(116, 115);
            marksArrearsHierarchy2004.Add(118, 117);
            marksArrearsHierarchy2004.Add(119, 117);
            marksArrearsHierarchy2004.Add(120, 117);
            marksArrearsHierarchy2004.Add(121, 117);
            marksArrearsHierarchy2004.Add(122, 117);
            marksArrearsHierarchy2004.Add(123, 122);
            marksArrearsHierarchy2004.Add(124, 122);
            marksArrearsHierarchy2004.Add(130, 117);
            marksArrearsHierarchy2004.Add(131, 130);
            marksArrearsHierarchy2004.Add(132, 130);
            marksArrearsHierarchy2004.Add(133, 117);
            marksArrearsHierarchy2004.Add(134, 117);
            marksArrearsHierarchy2004.Add(135, 117);
            marksArrearsHierarchy2004.Add(136, 117);
            marksArrearsHierarchy2004.Add(137, 117);

            marksArrearsHierarchy2005.Clear();
            marksArrearsHierarchy2005.Add(138, 137);
            marksArrearsHierarchy2005.Add(139, 138);
            marksArrearsHierarchy2005.Add(140, 139);
            marksArrearsHierarchy2005.Add(141, 140);
            marksArrearsHierarchy2005.Add(142, 140);
            marksArrearsHierarchy2005.Add(143, 140);
            marksArrearsHierarchy2005.Add(144, 140);
            marksArrearsHierarchy2005.Add(145, 140);
            marksArrearsHierarchy2005.Add(146, 139);
            marksArrearsHierarchy2005.Add(147, 138);
            marksArrearsHierarchy2005.Add(148, 147);
            marksArrearsHierarchy2005.Add(149, 148);
            marksArrearsHierarchy2005.Add(150, 148);
            marksArrearsHierarchy2005.Add(151, 148);
            marksArrearsHierarchy2005.Add(152, 148);
            marksArrearsHierarchy2005.Add(153, 147);
            marksArrearsHierarchy2005.Add(154, 137);
            marksArrearsHierarchy2005.Add(155, 154);
            marksArrearsHierarchy2005.Add(156, 154);
            marksArrearsHierarchy2005.Add(157, 154);
            marksArrearsHierarchy2005.Add(158, 137);
            marksArrearsHierarchy2005.Add(159, 137);
            marksArrearsHierarchy2005.Add(160, 137);
            marksArrearsHierarchy2005.Add(161, 160);
            marksArrearsHierarchy2005.Add(162, 160);
            marksArrearsHierarchy2005.Add(163, 160);
            marksArrearsHierarchy2005.Add(164, 137);
            marksArrearsHierarchy2005.Add(165, 164);
            marksArrearsHierarchy2005.Add(166, 164);
            marksArrearsHierarchy2005.Add(167, 137);
            marksArrearsHierarchy2005.Add(168, 137);
            marksArrearsHierarchy2005.Add(169, 137);
            marksArrearsHierarchy2005.Add(170, 137);

            marksArrearsHierarchy2007.Clear();
            marksArrearsHierarchy2007.Add(670, 660);
            marksArrearsHierarchy2007.Add(680, 660);
            marksArrearsHierarchy2007.Add(690, 660);
            marksArrearsHierarchy2007.Add(700, 660);
            marksArrearsHierarchy2007.Add(750, 740);
            marksArrearsHierarchy2007.Add(760, 750);
            marksArrearsHierarchy2007.Add(770, 750);
            marksArrearsHierarchy2007.Add(780, 750);
            marksArrearsHierarchy2007.Add(790, 750);
            marksArrearsHierarchy2007.Add(800, 750);
            marksArrearsHierarchy2007.Add(810, 750);
            marksArrearsHierarchy2007.Add(820, 740);
            marksArrearsHierarchy2007.Add(840, 830);
            marksArrearsHierarchy2007.Add(850, 840);
            marksArrearsHierarchy2007.Add(860, 840);
            marksArrearsHierarchy2007.Add(870, 840);
            marksArrearsHierarchy2007.Add(880, 840);
            marksArrearsHierarchy2007.Add(890, 840);
            marksArrearsHierarchy2007.Add(900, 840);
            marksArrearsHierarchy2007.Add(920, 910);
            marksArrearsHierarchy2007.Add(930, 920);
            marksArrearsHierarchy2007.Add(940, 920);
            marksArrearsHierarchy2007.Add(950, 920);
            marksArrearsHierarchy2007.Add(960, 920);
            marksArrearsHierarchy2007.Add(970, 910);
            marksArrearsHierarchy2007.Add(1000, 990);
            marksArrearsHierarchy2007.Add(1010, 990);
            marksArrearsHierarchy2007.Add(1020, 990);
            marksArrearsHierarchy2007.Add(1050, 1040);
            marksArrearsHierarchy2007.Add(1060, 1040);
            marksArrearsHierarchy2007.Add(1070, 1040);

            marksArrearsHierarchy2007Pulse.Clear();
            marksArrearsHierarchy2007Pulse.Add(7100, 7000);
            marksArrearsHierarchy2007Pulse.Add(7200, 7000);
            marksArrearsHierarchy2007Pulse.Add(7300, 7000);
            marksArrearsHierarchy2007Pulse.Add(7400, 7000);
            marksArrearsHierarchy2007Pulse.Add(7500, 7000);
            marksArrearsHierarchy2007Pulse.Add(7600, 7000);
            marksArrearsHierarchy2007Pulse.Add(7700, 7000);
            marksArrearsHierarchy2007Pulse.Add(7800, 7000);
            marksArrearsHierarchy2007Pulse.Add(7900, 7000);
            marksArrearsHierarchy2007Pulse.Add(8000, 7000);
            marksArrearsHierarchy2007Pulse.Add(7010, 7000);
            marksArrearsHierarchy2007Pulse.Add(7020, 7000);
            marksArrearsHierarchy2007Pulse.Add(7030, 7000);
            marksArrearsHierarchy2007Pulse.Add(7040, 7000);
            marksArrearsHierarchy2007Pulse.Add(7410, 7400);
            marksArrearsHierarchy2007Pulse.Add(7420, 7400);
            marksArrearsHierarchy2007Pulse.Add(7411, 7410);
            marksArrearsHierarchy2007Pulse.Add(7412, 7410);
            marksArrearsHierarchy2007Pulse.Add(7413, 7410);
            marksArrearsHierarchy2007Pulse.Add(7414, 7410);
            marksArrearsHierarchy2007Pulse.Add(7415, 7410);
            marksArrearsHierarchy2007Pulse.Add(7416, 7410);
            marksArrearsHierarchy2007Pulse.Add(7510, 7500);
            marksArrearsHierarchy2007Pulse.Add(7520, 7500);
            marksArrearsHierarchy2007Pulse.Add(7511, 7510);
            marksArrearsHierarchy2007Pulse.Add(7512, 7510);
            marksArrearsHierarchy2007Pulse.Add(7513, 7510);
            marksArrearsHierarchy2007Pulse.Add(7514, 7510);
            marksArrearsHierarchy2007Pulse.Add(7515, 7510);
            marksArrearsHierarchy2007Pulse.Add(7610, 7600);
            marksArrearsHierarchy2007Pulse.Add(7620, 7600);
            marksArrearsHierarchy2007Pulse.Add(7611, 7610);
            marksArrearsHierarchy2007Pulse.Add(7612, 7610);
            marksArrearsHierarchy2007Pulse.Add(7613, 7610);
            marksArrearsHierarchy2007Pulse.Add(7614, 7610);
            marksArrearsHierarchy2007Pulse.Add(7810, 7800);
            marksArrearsHierarchy2007Pulse.Add(7820, 7800);
            marksArrearsHierarchy2007Pulse.Add(7830, 7800);
            marksArrearsHierarchy2007Pulse.Add(8010, 8000);
            marksArrearsHierarchy2007Pulse.Add(8020, 8000);
            marksArrearsHierarchy2007Pulse.Add(8030, 8000);
        }

        /// <summary>
        /// Заполняет коллекцию с данными по иерархии - СправРасходыДоп
        /// </summary>
        private void FillMarksOutcomesHierarchy()
        {
            marksOutcomesHierarchy2004.Clear();
            marksOutcomesHierarchy2004.Add(22, 21);
            marksOutcomesHierarchy2004.Add(23, 21);
            marksOutcomesHierarchy2004.Add(24, 21);
            marksOutcomesHierarchy2004.Add(25, 21);
            marksOutcomesHierarchy2004.Add(27, 26);
            marksOutcomesHierarchy2004.Add(28, 26);
            marksOutcomesHierarchy2004.Add(29, 26);
            marksOutcomesHierarchy2004.Add(31, 30);
            marksOutcomesHierarchy2004.Add(32, 30);
            marksOutcomesHierarchy2004.Add(33, 30);
            marksOutcomesHierarchy2004.Add(34, 30);
            marksOutcomesHierarchy2004.Add(36, 35);
            marksOutcomesHierarchy2004.Add(37, 35);
            marksOutcomesHierarchy2004.Add(39, 38);
            marksOutcomesHierarchy2004.Add(40, 38);
            marksOutcomesHierarchy2004.Add(42, 41);
            marksOutcomesHierarchy2004.Add(43, 41);
            marksOutcomesHierarchy2004.Add(46, 45);
            marksOutcomesHierarchy2004.Add(47, 46);
            marksOutcomesHierarchy2004.Add(53, 52);
            marksOutcomesHierarchy2004.Add(54, 52);
            marksOutcomesHierarchy2004.Add(55, 52);
            marksOutcomesHierarchy2004.Add(56, 52);
            marksOutcomesHierarchy2004.Add(58, 57);
            marksOutcomesHierarchy2004.Add(59, 57);
            marksOutcomesHierarchy2004.Add(60, 57);
            marksOutcomesHierarchy2004.Add(65, 64);
            marksOutcomesHierarchy2004.Add(66, 64);
            marksOutcomesHierarchy2004.Add(67, 64);
            marksOutcomesHierarchy2004.Add(68, 64);
            marksOutcomesHierarchy2004.Add(71, 70);
            marksOutcomesHierarchy2004.Add(73, 72);
            marksOutcomesHierarchy2004.Add(74, 72);
            marksOutcomesHierarchy2004.Add(75, 72);
            marksOutcomesHierarchy2004.Add(76, 72);
            marksOutcomesHierarchy2004.Add(77, 72);
            marksOutcomesHierarchy2004.Add(79, 78);
            marksOutcomesHierarchy2004.Add(80, 78);
            marksOutcomesHierarchy2004.Add(81, 78);
            marksOutcomesHierarchy2004.Add(82, 78);
            marksOutcomesHierarchy2004.Add(83, 78);
            marksOutcomesHierarchy2004.Add(84, 78);

            marksOutcomesHierarchy2005.Clear();
            marksOutcomesHierarchy2005.Add(18, 17);
            marksOutcomesHierarchy2005.Add(19, 18);
            marksOutcomesHierarchy2005.Add(20, 18);
            marksOutcomesHierarchy2005.Add(21, 18);
            marksOutcomesHierarchy2005.Add(22, 18);
            marksOutcomesHierarchy2005.Add(23, 18);
            marksOutcomesHierarchy2005.Add(24, 18);
            marksOutcomesHierarchy2005.Add(25, 18);
            marksOutcomesHierarchy2005.Add(26, 18);
            marksOutcomesHierarchy2005.Add(27, 17);
            marksOutcomesHierarchy2005.Add(28, 27);
            marksOutcomesHierarchy2005.Add(29, 27);
            marksOutcomesHierarchy2005.Add(30, 27);
            marksOutcomesHierarchy2005.Add(31, 27);
            marksOutcomesHierarchy2005.Add(32, 27);
            marksOutcomesHierarchy2005.Add(34, 33);
            marksOutcomesHierarchy2005.Add(35, 33);
            marksOutcomesHierarchy2005.Add(36, 33);
            marksOutcomesHierarchy2005.Add(38, 37);
            marksOutcomesHierarchy2005.Add(39, 38);
            marksOutcomesHierarchy2005.Add(40, 39);
            marksOutcomesHierarchy2005.Add(41, 39);
            marksOutcomesHierarchy2005.Add(42, 39);
            marksOutcomesHierarchy2005.Add(43, 39);
            marksOutcomesHierarchy2005.Add(44, 39);
            marksOutcomesHierarchy2005.Add(45, 38);
            marksOutcomesHierarchy2005.Add(46, 37);
            marksOutcomesHierarchy2005.Add(47, 46);
            marksOutcomesHierarchy2005.Add(48, 47);
            marksOutcomesHierarchy2005.Add(49, 47);
            marksOutcomesHierarchy2005.Add(50, 47);
            marksOutcomesHierarchy2005.Add(51, 47);
            marksOutcomesHierarchy2005.Add(52, 46);
            marksOutcomesHierarchy2005.Add(54, 53);
            marksOutcomesHierarchy2005.Add(55, 53);
            marksOutcomesHierarchy2005.Add(56, 53);
            marksOutcomesHierarchy2005.Add(62, 61);
            marksOutcomesHierarchy2005.Add(63, 61);
            marksOutcomesHierarchy2005.Add(66, 65);
            marksOutcomesHierarchy2005.Add(67, 65);
            marksOutcomesHierarchy2005.Add(68, 65);
            marksOutcomesHierarchy2005.Add(73, 72);
            marksOutcomesHierarchy2005.Add(74, 72);
            marksOutcomesHierarchy2005.Add(75, 72);

            marksOutcomesHierarchy2007.Clear();
            marksOutcomesHierarchy2007.Add(110, 100);
            marksOutcomesHierarchy2007.Add(111, 110);
            marksOutcomesHierarchy2007.Add(112, 110);
            marksOutcomesHierarchy2007.Add(120, 100);
            marksOutcomesHierarchy2007.Add(121, 120);
            marksOutcomesHierarchy2007.Add(122, 120);
            marksOutcomesHierarchy2007.Add(130, 100);
            marksOutcomesHierarchy2007.Add(131, 130);
            marksOutcomesHierarchy2007.Add(132, 130);
            marksOutcomesHierarchy2007.Add(210, 200);
            marksOutcomesHierarchy2007.Add(220, 200);
            marksOutcomesHierarchy2007.Add(310, 300);
            marksOutcomesHierarchy2007.Add(320, 300);
            marksOutcomesHierarchy2007.Add(410, 400);
            marksOutcomesHierarchy2007.Add(420, 400);
            marksOutcomesHierarchy2007.Add(510, 500);
            marksOutcomesHierarchy2007.Add(610, 600);
            marksOutcomesHierarchy2007.Add(710, 700);
            marksOutcomesHierarchy2007.Add(720, 700);
            marksOutcomesHierarchy2007.Add(810, 800);
            marksOutcomesHierarchy2007.Add(910, 900);
            marksOutcomesHierarchy2007.Add(1010, 1000);
            marksOutcomesHierarchy2007.Add(1020, 1000);
            marksOutcomesHierarchy2007.Add(1030, 1000);
            marksOutcomesHierarchy2007.Add(1040, 1000);
            marksOutcomesHierarchy2007.Add(1050, 1000);
            marksOutcomesHierarchy2007.Add(1060, 1000);
            marksOutcomesHierarchy2007.Add(1070, 1000);
            marksOutcomesHierarchy2007.Add(1080, 1000);
            marksOutcomesHierarchy2007.Add(1090, 1000);
            marksOutcomesHierarchy2007.Add(1100, 1000);
            marksOutcomesHierarchy2007.Add(1110, 1000);
            marksOutcomesHierarchy2007.Add(1120, 1000);
            marksOutcomesHierarchy2007.Add(1210, 1200);
            marksOutcomesHierarchy2007.Add(1310, 1300);
            marksOutcomesHierarchy2007.Add(1320, 1300);
            marksOutcomesHierarchy2007.Add(1410, 1400);
            marksOutcomesHierarchy2007.Add(1510, 1500);
            marksOutcomesHierarchy2007.Add(1610, 1600);
            marksOutcomesHierarchy2007.Add(1620, 1600);
            marksOutcomesHierarchy2007.Add(1630, 1600);
            marksOutcomesHierarchy2007.Add(1710, 1700);
            marksOutcomesHierarchy2007.Add(1810, 1800);
            marksOutcomesHierarchy2007.Add(1910, 1900);
            marksOutcomesHierarchy2007.Add(1920, 1900);
            marksOutcomesHierarchy2007.Add(1930, 1900);
            marksOutcomesHierarchy2007.Add(1940, 1900);
            marksOutcomesHierarchy2007.Add(1950, 1900);
            marksOutcomesHierarchy2007.Add(1960, 1900);
            marksOutcomesHierarchy2007.Add(1970, 1900);
            marksOutcomesHierarchy2007.Add(1980, 1900);
            marksOutcomesHierarchy2007.Add(1990, 1900);
            marksOutcomesHierarchy2007.Add(2110, 2100);
            marksOutcomesHierarchy2007.Add(2120, 2100);
            marksOutcomesHierarchy2007.Add(2130, 2100);
            marksOutcomesHierarchy2007.Add(2140, 2100);
            marksOutcomesHierarchy2007.Add(2150, 2100);
            marksOutcomesHierarchy2007.Add(2160, 2100);
            marksOutcomesHierarchy2007.Add(2210, 2200);
            marksOutcomesHierarchy2007.Add(2220, 2200);
            marksOutcomesHierarchy2007.Add(2310, 2300);
            marksOutcomesHierarchy2007.Add(2320, 2300);
            marksOutcomesHierarchy2007.Add(2330, 2300);
            marksOutcomesHierarchy2007.Add(2340, 2300);
            marksOutcomesHierarchy2007.Add(2350, 2300);
            marksOutcomesHierarchy2007.Add(2351, 2350);
            marksOutcomesHierarchy2007.Add(2360, 2300);
            marksOutcomesHierarchy2007.Add(2370, 2300);
            marksOutcomesHierarchy2007.Add(2380, 2300);
            marksOutcomesHierarchy2007.Add(2390, 2300);
            marksOutcomesHierarchy2007.Add(2400, 2300);
            marksOutcomesHierarchy2007.Add(2410, 2300);
            marksOutcomesHierarchy2007.Add(2420, 2300);
            marksOutcomesHierarchy2007.Add(2430, 2300);
            marksOutcomesHierarchy2007.Add(2440, 2300);
            marksOutcomesHierarchy2007.Add(2450, 2300);
            marksOutcomesHierarchy2007.Add(2460, 2300);
            marksOutcomesHierarchy2007.Add(2470, 2300);
            marksOutcomesHierarchy2007.Add(2480, 2300);
            marksOutcomesHierarchy2007.Add(2610, 2600);
            marksOutcomesHierarchy2007.Add(2620, 2600);
            marksOutcomesHierarchy2007.Add(2630, 2600);
            marksOutcomesHierarchy2007.Add(2640, 2600);
            marksOutcomesHierarchy2007.Add(2650, 2600);
            marksOutcomesHierarchy2007.Add(2660, 2600);
            marksOutcomesHierarchy2007.Add(2670, 2600);
            marksOutcomesHierarchy2007.Add(2680, 2600);
            marksOutcomesHierarchy2007.Add(2690, 2600);
            marksOutcomesHierarchy2007.Add(2810, 2800);
            marksOutcomesHierarchy2007.Add(2910, 2900);
            marksOutcomesHierarchy2007.Add(4310, 4300);
            marksOutcomesHierarchy2007.Add(4311, 4310);
            marksOutcomesHierarchy2007.Add(4312, 4310);
            marksOutcomesHierarchy2007.Add(4320, 4300);
            marksOutcomesHierarchy2007.Add(4321, 4320);
            marksOutcomesHierarchy2007.Add(4322, 4320);
            marksOutcomesHierarchy2007.Add(4410, 4400);
            marksOutcomesHierarchy2007.Add(4420, 4400);
            marksOutcomesHierarchy2007.Add(4710, 4700);
            marksOutcomesHierarchy2007.Add(4711, 4710);
            marksOutcomesHierarchy2007.Add(4712, 4710);
            marksOutcomesHierarchy2007.Add(4713, 4710);
            marksOutcomesHierarchy2007.Add(4714, 4710);
            marksOutcomesHierarchy2007.Add(4715, 4710);
            marksOutcomesHierarchy2007.Add(4716, 4710);
            marksOutcomesHierarchy2007.Add(4720, 4700);
            marksOutcomesHierarchy2007.Add(4810, 4800);
            marksOutcomesHierarchy2007.Add(4811, 4810);
            marksOutcomesHierarchy2007.Add(4812, 4810);
            marksOutcomesHierarchy2007.Add(4813, 4810);
            marksOutcomesHierarchy2007.Add(4814, 4810);
            marksOutcomesHierarchy2007.Add(4815, 4810);
            marksOutcomesHierarchy2007.Add(4820, 4800);
            marksOutcomesHierarchy2007.Add(4910, 4900);
            marksOutcomesHierarchy2007.Add(4911, 4910);
            marksOutcomesHierarchy2007.Add(4912, 4910);
            marksOutcomesHierarchy2007.Add(4913, 4910);
            marksOutcomesHierarchy2007.Add(4914, 4910);
            marksOutcomesHierarchy2007.Add(4920, 4900);
            marksOutcomesHierarchy2007.Add(5010, 5000);
            marksOutcomesHierarchy2007.Add(5020, 5000);
            marksOutcomesHierarchy2007.Add(5030, 5000);
            marksOutcomesHierarchy2007.Add(5110, 5100);
            marksOutcomesHierarchy2007.Add(5120, 5100);
            marksOutcomesHierarchy2007.Add(5210, 5200);
            marksOutcomesHierarchy2007.Add(5220, 5200);
            marksOutcomesHierarchy2007.Add(5310, 5300);
            marksOutcomesHierarchy2007.Add(5320, 5300);
            marksOutcomesHierarchy2007.Add(5330, 5300);
            marksOutcomesHierarchy2007.Add(5410, 5400);
            marksOutcomesHierarchy2007.Add(5411, 5410);
            marksOutcomesHierarchy2007.Add(5420, 5400);
            marksOutcomesHierarchy2007.Add(5421, 5420);
            marksOutcomesHierarchy2007.Add(5430, 5400);
            marksOutcomesHierarchy2007.Add(5431, 5430);
            marksOutcomesHierarchy2007.Add(5440, 5400);
            marksOutcomesHierarchy2007.Add(5441, 5440);
            marksOutcomesHierarchy2007.Add(5510, 5500);
            marksOutcomesHierarchy2007.Add(5511, 5510);
            marksOutcomesHierarchy2007.Add(5610, 5600);
            marksOutcomesHierarchy2007.Add(5620, 5600);
        }

        /// <summary>
        /// Заполняет коллекции иерархии классификаторов
        /// </summary>
        private void FillClsHierarchy()
        {
            FillKvsrHierarchy();
            FillMarksOutDebtHierarchy();
            FillMarksInDebtHierarchy();
            FillMarksArrearsHierarchy();
            FillMarksOutcomesHierarchy();
        }

        #endregion пиздец

        protected override void DirectPumpData()
        {
            this.SkifReportFormat = SKIFFormat.MonthReports;
            // логика пиздец
            base.DirectPumpData();
            FillClsHierarchy();
            PumpDataYMVTemplate();
        }

        #endregion Закачка данных

        #region Обработка данных

        private void DeleteUnusedAccount()
        {
            if (!ToPumpBlock(Block.bArrears))
                return;

            IFactTable[] facts = new IFactTable[] { fctMonthRepArrears };
            string[] refFieldNames = new string[] { "RefAccount" };
            if (this.Scheme.FactTables.ContainsKey("82847e3d-d47c-4099-9e7d-64981221670b"))
            {
                facts = (IFactTable[])CommonRoutines.ConcatArrays(facts, new IFactTable[] { this.Scheme.FactTables["82847e3d-d47c-4099-9e7d-64981221670b"] });
                refFieldNames = (string[])CommonRoutines.ConcatArrays(refFieldNames, new string[] { "RefAccount" });
            }

            int sourceId = yearSourceID;
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

        private void SetClsNames(ref DataSet ds, Dictionary<string, string> cache)
        {
            int count = ds.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = ds.Tables[0].Rows[i];
                string code = row["CODE"].ToString().TrimStart('0');
                if (code == string.Empty)
                    continue;
                if (!cache.ContainsKey(code))
                    continue;
                if (row["NAME"].ToString().ToUpper() == "НЕУКАЗАННОЕ НАИМЕНОВАНИЕ")
                    row["Name"] = cache[code].ToString();
            }
        }

        private void CorrectSums()
        {
            MRSumCorrectionConfig mrSumCorrectionConfig = new MRSumCorrectionConfig();
            mrSumCorrectionConfig.FactField = "FACT";
            mrSumCorrectionConfig.FactReportField = "FACTREPORT";
            mrSumCorrectionConfig.MonthPlanField = "MONTHPLAN";
            mrSumCorrectionConfig.MonthPlanReportField = "MONTHPLANREPORT";
            mrSumCorrectionConfig.QuarterPlanField = "QUARTERPLAN";
            mrSumCorrectionConfig.QuarterPlanReportField = "QUARTERPLANREPORT";
            mrSumCorrectionConfig.SpreadMonthPlanField = "SPREADFACTMONTHPLAN";
            mrSumCorrectionConfig.SpreadMonthPlanReportField = "SPREADFACTMONTHPLANREPORT";
            mrSumCorrectionConfig.SpreadYearPlanField = "SPREADFACTYEARPLAN";
            mrSumCorrectionConfig.SpreadYearPlanReportField = "SPREADFACTYEARPLANREPORT";
            mrSumCorrectionConfig.YearPlanField = "YEARPLAN";
            mrSumCorrectionConfig.YearPlanReportField = "YEARPLANREPORT";
            mrSumCorrectionConfig.AssignedField = string.Empty;
            mrSumCorrectionConfig.AssignedReportField = string.Empty;
            mrSumCorrectionConfig.ExcSumP = string.Empty;
            mrSumCorrectionConfig.ExcSumPRep = string.Empty;
            mrSumCorrectionConfig.ExcSumF = string.Empty;
            mrSumCorrectionConfig.ExcSumFRep = string.Empty;
            // ДефицитПрофицит
            GroupTable(fctMonthRepDefProf, new string[] { "REFMEANSTYPE", "REFREGIONS", "REFBDGTLEVELS" }, mrSumCorrectionConfig);
            TransferSourceSums(fctMonthRepDefProf, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepDefProf, mrSumCorrectionConfig);

            mrSumCorrectionConfig.ExcSumP = "ExcSumP";
            mrSumCorrectionConfig.ExcSumPRep = "ExcSumPRep";
            mrSumCorrectionConfig.ExcSumF = "ExcSumF";
            mrSumCorrectionConfig.ExcSumFRep = "ExcSumFRep";
            // Доходы
            TransferSourceSums(fctMonthRepIncomes, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepIncomes, mrSumCorrectionConfig);
            GroupTable(fctMonthRepIncomes, new string[] { "REFKD", "REFMEANSTYPE", "REFREGIONS", "REFBDGTLEVELS" }, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepIncomes, dsKD.Tables[0], clsKD,
                "REFKD", mrSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS");
            // Источники внешнего финансирования
            TransferSourceSums(fctMonthRepOutFin, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepOutFin, mrSumCorrectionConfig);
            GroupTable(fctMonthRepOutFin, new string[] { "RefSOF", "REFMEANSTYPE", "REFREGIONS", "REFBDGTLEVELS" }, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepOutFin, dsSrcOutFin.Tables[0], clsSrcOutFin,
                "RefSOF", mrSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS");
            // Источники внутреннего финансирования
            TransferSourceSums(fctMonthRepInFin, mrSumCorrectionConfig);
            GroupTable(fctMonthRepInFin, new string[] { "RefSIF", "REFMEANSTYPE", "REFREGIONS", "REFBDGTLEVELS" }, mrSumCorrectionConfig);
            AddParentRecords(fctMonthRepInFin, dsSrcInFin.Tables[0], "CodeStr", "RefSIF",
                new string[] { "REFMEANSTYPE", "REFREGIONS", "REFBDGTLEVELS" }, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepInFin, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepInFin, dsSrcInFin.Tables[0], clsSrcInFin,
                "RefSIF", mrSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS");
            // Расходы
            TransferSourceSums(fctMonthRepOutcomes, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepOutcomes, mrSumCorrectionConfig);
            GroupTable(fctMonthRepOutcomes, new string[] { "REFFKR", "REFEKR", "REFMEANSTYPE", "REFREGIONS", "REFBDGTLEVELS" }, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepOutcomes, dsFKR.Tables[0], clsFKR, "REFFKR",
                mrSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "REFEKR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", true);
            CorrectFactTableSums(fctMonthRepOutcomes, dsEKR.Tables[0], clsEKR,
                "REFEKR", mrSumCorrectionConfig, BlockProcessModifier.MROutcomes,
                new string[] { "REFFKR", "REFMEANSTYPE" }, "REFREGIONS", "REFBDGTLEVELS", false);
            mrSumCorrectionConfig.MonthPlanField = string.Empty;
            mrSumCorrectionConfig.MonthPlanReportField = string.Empty;
            mrSumCorrectionConfig.QuarterPlanField = string.Empty;
            mrSumCorrectionConfig.QuarterPlanReportField = string.Empty;
            mrSumCorrectionConfig.YearPlanField = string.Empty;
            mrSumCorrectionConfig.YearPlanReportField = string.Empty;
            mrSumCorrectionConfig.ExcSumP = string.Empty;
            mrSumCorrectionConfig.ExcSumPRep = string.Empty;
            mrSumCorrectionConfig.ExcSumF = string.Empty;
            mrSumCorrectionConfig.ExcSumFRep = string.Empty;
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                mrSumCorrectionConfig.AssignedField = "ASSIGNED";
                mrSumCorrectionConfig.AssignedReportField = "ASSIGNEDREPORT";
            }
            // СправВнешнийДолг
            TransferSourceSums(fctMonthRepOutDebtBooks, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepOutDebtBooks, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepOutDebtBooks, dsMarksOutDebt.Tables[0],
                clsMarksOutDebt, "REFMARKSOUTDEBT", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
            // СправВнутреннийДолг
            TransferSourceSums(fctMonthRepInDebtBooks, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepInDebtBooks, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepInDebtBooks, dsMarksInDebt.Tables[0],
                clsMarksInDebt, "REFMARKSINDEBT", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
            mrSumCorrectionConfig.AssignedField = string.Empty;
            mrSumCorrectionConfig.AssignedReportField = string.Empty;
            // СправДоходы
            TransferSourceSums(fctMonthRepIncomesBooks, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepIncomesBooks, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepIncomesBooks, dsKVSR.Tables[0],
                clsKVSR, "REFKVSR", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                mrSumCorrectionConfig.AssignedField = "ASSIGNED";
                mrSumCorrectionConfig.AssignedReportField = "ASSIGNEDREPORT";
            }
            // СправЗадолженность
            TransferSourceSums(fctMonthRepArrearsBooks, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepArrearsBooks, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepArrearsBooks, dsMarksArrears.Tables[0],
                clsMarksArrears, "REFMARKSARREARS", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
            mrSumCorrectionConfig.AssignedField = string.Empty;
            mrSumCorrectionConfig.AssignedReportField = string.Empty;
            // СправРасходы
            TransferSourceSums(fctMonthRepOutcomesBooks, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepOutcomesBooks, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepOutcomesBooks, dsFKRBook.Tables[0],
                clsFKRBook, "REFFKR", mrSumCorrectionConfig, BlockProcessModifier.MRStandard,
                new string[] { "REFEKR" }, "REFREGIONS", "REFBDGTLEVELS", true);
            CorrectFactTableSums(fctMonthRepOutcomesBooks, dsEKRBook.Tables[0],
                clsEKRBook, "REFEKR", mrSumCorrectionConfig, BlockProcessModifier.MROutcomesBooks,
                new string[] { "REFFKR" }, "REFREGIONS", "REFBDGTLEVELS", false);
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
            {
                mrSumCorrectionConfig.AssignedField = "ASSIGNED";
                mrSumCorrectionConfig.AssignedReportField = "ASSIGNEDREPORT";
            }
            // СправРасходыДоп
            TransferSourceSums(fctMonthRepOutcomesBooksEx, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepOutcomesBooksEx, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepOutcomesBooksEx, dsMarksOutcomes.Tables[0],
                clsMarksOutcomes, "REFMARKSOUTCOMES", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
            // справ остатки
            mrSumCorrectionConfig.MonthPlanField = string.Empty;
            mrSumCorrectionConfig.MonthPlanReportField = string.Empty;
            mrSumCorrectionConfig.QuarterPlanField = string.Empty;
            mrSumCorrectionConfig.QuarterPlanReportField = string.Empty;
            mrSumCorrectionConfig.SpreadMonthPlanField = string.Empty;
            mrSumCorrectionConfig.SpreadMonthPlanReportField = string.Empty;
            mrSumCorrectionConfig.SpreadYearPlanField = string.Empty;
            mrSumCorrectionConfig.SpreadYearPlanReportField = string.Empty;
            mrSumCorrectionConfig.YearPlanField = string.Empty;
            mrSumCorrectionConfig.YearPlanReportField = string.Empty;
            mrSumCorrectionConfig.AssignedField = string.Empty;
            mrSumCorrectionConfig.AssignedReportField = string.Empty;
            TransferSourceSums(fctMonthRepExcessBooks, mrSumCorrectionConfig);
            FillSpreadSums(fctMonthRepExcessBooks, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepExcessBooks, dsMarksExcess.Tables[0],
                clsMarksExcess, "RefMarks", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
            // конс расходы
            mrSumCorrectionConfig.MonthPlanField = "Arrival";
            mrSumCorrectionConfig.MonthPlanReportField = "ArrivalRep";
            mrSumCorrectionConfig.FactField = string.Empty;
            mrSumCorrectionConfig.FactReportField = string.Empty;

            TransferSourceSums(fctMonthRepAccount, mrSumCorrectionConfig);
            CorrectFactTableSums(fctMonthRepAccount, dsMarksAccount.Tables[0],
                clsMarksAccount, "RefAccount", mrSumCorrectionConfig, BlockProcessModifier.MRStandard);
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

        // заполняем уровень бюджета у таблицы фактов, уровень определяется типом документа района
        private void SetFactBudgetLevel(IFactTable fct)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, string.Format("SOURCEID = {0}", this.SourceID), string.Empty);
            DataTable dt = ds.Tables[0];
            int count = dt.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dt.Rows[i];
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
            UpdateDataSet(da, ds, fct);
        }

        private void SetFactsBudgetLevel()
        {
            SetFactBudgetLevel(fctMonthRepDefProf);
            SetFactBudgetLevel(fctMonthRepIncomes);
            SetFactBudgetLevel(fctMonthRepOutFin);
            SetFactBudgetLevel(fctMonthRepInFin);
            SetFactBudgetLevel(fctMonthRepOutcomes);
            SetFactBudgetLevel(fctMonthRepOutDebtBooks);
            SetFactBudgetLevel(fctMonthRepInDebtBooks);
            SetFactBudgetLevel(fctMonthRepIncomesBooks);
            SetFactBudgetLevel(fctMonthRepArrearsBooks);
            SetFactBudgetLevel(fctMonthRepOutcomesBooks);
            SetFactBudgetLevel(fctMonthRepOutcomesBooksEx);
            SetFactBudgetLevel(fctMonthRepExcessBooks);
        }

        protected override void ProcessDataSource()
        {
            DeleteUnusedAccount();
            toSetHierarchy = false;
            FillClsHierarchy();
            SetClsHierarchy();
            UpdateData();
            // разыменовка классификатора ФКР.Мес отч (только для новосибирска с 2006 года)
            if ((this.Region == RegionName.Novosibirsk) && (this.DataSource.Year >= 2006))
            {
                SetClsNames(ref dsFKR, analFKRCache);
                UpdateData();
            }
            SetRegDocType();
            UpdateData();
            if (this.DataSource.Year < 2006)
                SetFactsBudgetLevel();
            CorrectSums();
            UpdateData();
        }

        /// <summary>
        /// Этап обработки данных
        /// </summary>
        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Коррекции сумм фактов по данным источника");
        }

        #endregion Обработка данных

        #region Проверка скорректированных сумм

        protected override void QueryDataForCheck()
        {
            QueryData();
            string constraint = string.Format("SourceID = {0}", this.SourceID);
            InitDataSet(ref daMonthRepIncomes, ref dsMonthRepIncomes, fctMonthRepIncomes, false, constraint, string.Empty);
        }

        private List<int> GetClsParents(DataTable clsTable)
        {
            List<int> list = new List<int>();
            foreach (DataRow row in clsTable.Rows)
            {
                if (row.IsNull("ParentId"))
                    continue;
                int parentId = Convert.ToInt32(row["ParentId"]);
                if (!list.Contains(parentId))
                    list.Add(parentId);
            }
            return list;
        }

        private string GetFieldCaption(string fieldName)
        {
            switch (fieldName.ToUpper())
            {
                case "YEARPLAN":
                    return "Годовые назначения";
                case "MONTHPLAN":
                    return "Месячные назначения";
                case "QUARTERPLAN":
                    return "Квартальные назначения";
                case "FACT":
                    return "Факт";
                default:
                    return string.Empty;
            }
        }

        private void CheckParentSums(DataRow factRow, string[] sumFieldNames, DataRow clsRow)
        {
            foreach (string sumFieldName in sumFieldNames)
            {
                decimal sum = 0;
                if (factRow[sumFieldName] != DBNull.Value)
                    sum = Convert.ToDecimal(factRow[sumFieldName]);
                if (sum != 0)
                {
                    string message = string.Format("Неправильно скорректирована сумма факта (Id:{0}; Поле: {1}; Сумма: {2})." +
                        "Возможно, у подчиненных коду {3} (Id: {4}) записей классификатора данных 'КД.МесОтч' неверно установлена иерархия.",
                        factRow["Id"], GetFieldCaption(sumFieldName), sum, clsRow["CodeStr"], clsRow["Id"]);
                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
                }
            }
        }

        private void CheckCorrectedSums(DataTable factTable, string refClsFieldName, DataTable clsTable, string[] sumFieldNames)
        {
            Dictionary<int, DataRow> clsCache = null;
            FillRowsCache(ref clsCache, clsTable, "Id");
            List<int> clsParentsList = GetClsParents(clsTable);
            List<int> incorrectClsList = new List<int>();
            try
            {
                foreach (DataRow row in factTable.Rows)
                {
                    int clsId = Convert.ToInt32(row[refClsFieldName]);
                    if (!clsParentsList.Contains(clsId))
                        continue;
                    CheckParentSums(row, sumFieldNames, clsCache[clsId]);
                }
            }
            finally
            {
                clsParentsList.Clear();
                incorrectClsList.Clear();
            }
        }

        protected override void CheckDataSource()
        {
            CheckCorrectedSums(dsMonthRepIncomes.Tables[0], "RefKD", dsKD.Tables[0],
                new string[] { "YearPlan", "MonthPlan", "QuarterPlan", "Fact" });
        }

        protected override void DirectCheckData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            CheckDataSourcesTemplate(year, month, "Проверка скорректированных сумм.");
        }

        #endregion Проверка скорректированных сумм

    }
}