using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.InteropServices;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;
using WorkPlace;

namespace Krista.FM.Server.DataPumps.FO35Pump
{

    public partial class FO35PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФО_Предельные объемы финансирования (d_Marks_FOPOF)
        private IDbDataAdapter daMarksPof;
        private DataSet dsMarksPof;
        private IClassifier clsMarksPof;
        private Dictionary<string, int> cacheMarksPof = null;
        // Мероприятия.Перечень праздников (d_Fact_Holidays)
        private IDbDataAdapter daHolidays;
        private DataSet dsHolidays;
        private IClassifier clsHolidays;
        private Dictionary<string, int> cacheHolidays = null;
        // Показатели.ФО_Поступления в бюджет (d_Marks_FOInpayBud)
        private IDbDataAdapter daMarksFOInpayBud;
        private DataSet dsMarksFOInpayBud;
        private IClassifier clsMarksFOInpayBud;
        private Dictionary<string, int> cacheMarksFOInpayBud = null;
        // Показатели.ФО_Код цели (d_Marks_FOTarget)
        private IDbDataAdapter daMarksFOTarget;
        private DataSet dsMarksFOTarget;
        private IClassifier clsMarksFOTarget;
        private Dictionary<string, int> cacheMarksFOTarget = null;
        // Код целевых средств.ФО_ИспКасПлан (d_Transfert_ExctCachPl)
        private IDbDataAdapter daTransfert;
        private DataSet dsTransfert;
        private IClassifier clsTransfert;
        private Dictionary<string, int> cacheTransfert = null;
        // Мероприятия.ФО_ИспКасПлан (d_Fact_ExctCachPl)
        private IDbDataAdapter daFactExct;
        private DataSet dsFactExct;
        private IClassifier clsFactExct;
        private Dictionary<string, int> cacheFactExct = null;

        #endregion Классификаторы

        #region Факты

        // Факт.ФО_Предельные объемы финансирования (f_F_FOPOF)
        private IDbDataAdapter daPof;
        private DataSet dsPof;
        private IFactTable fctPof;
        // Факт.ФО_Реестр платежных поручений (f_F_FOPayDoc)
        private IDbDataAdapter daFOPayDoc;
        private DataSet dsFOPayDoc;
        private IFactTable fctFOPayDoc;
        // Факт.ФО_Бюджетные обязательства (f_F_FOBudLiab)
        private IDbDataAdapter daFOBudLiab;
        private DataSet dsFOBudLiab;
        private IFactTable fctFOBudLiab;
        // Факт.ФО_Остатки средств бюджета (f_F_CalcBalanc)
        private IDbDataAdapter daCalcBalanc;
        private DataSet dsCalcBalanc;
        private IFactTable fctCalcBalanc;
        // Факт.ФО_Поступления в бюджет (f_F_FOInpayBud)
        private IDbDataAdapter daFOInpayBud;
        private DataSet dsFOInpayBud;
        private IFactTable fctFOInpayBud;
        // Факт.ФО_Остатки средств бюджета_ГРБС (f_F_BalancKVSR)
        private IDbDataAdapter daBalancKVSR;
        private DataSet dsBalancKVSR;
        private IFactTable fctBalancKVSR;
        // Факт.ФО_Поступление и расходование МБТ (f_F_FOTransfer)
        private IDbDataAdapter daFOTransfer;
        private DataSet dsFOTransfer;
        private IFactTable fctFOTransfer;
        // Факт.ФО_Финансирование по объектам (f_F_FOFinObj)
        private IDbDataAdapter daFOFinObj;
        private DataSet dsFOFinObj;
        private IFactTable fctFOFinObj;

        #endregion Факты

        private Dictionary<string, int> cacheKvsrName = null;
        private string reportDate = string.Empty;
        private string udlYear = string.Empty;

        // работа с базой АС Бюджет для получения КВСР
        private Database budgetDB = null;
        private Dictionary<string, string> cacheKvsrFromBudget = null;
        private bool isOleDbConnection = true;

        #endregion Поля

        #region Работа с базой и кэшами

        #region GUIDs

        private const string D_MARKS_POF_GUID = "53692170-8fde-42cb-990f-bf77c3f7bfe9";
        private const string D_HOLIDAYS_GUID = "5db49310-e35b-4f15-8654-64cbbfb63d3a";
        private const string D_MARKS_FO_INPAY_BUD_GUID = "08a6ebe6-646c-47f9-91c1-59341f75bea5";
        private const string D_MARKS_FO_TARGET_GUID = "1ceb430a-c58c-4751-aa6c-bcd6f78840ef";
        private const string D_TRANSFERT_GUID = "c4e3e116-a8da-4ff8-8417-b0814bcd3a23";
        private const string D_FACT_EXCT_CACH_PL_GUID = "f9715a28-2022-429e-907b-7f317822090d";
        private const string F_F_POF_GUID = "19c8a12e-67c1-426a-afc6-a3696f361f5f";
        private const string F_F_PAY_DOC_GUID = "c3f34177-a888-44ab-891f-29a6709a12ec";
        private const string F_F_BUD_LIAB_GUID = "e054155c-f9e2-470c-a02f-3a6eb9809e12";
        private const string F_F_CALC_BALANC_GUID = "fa710a1b-ef81-463e-a8c1-0a83c5e4a367";
        private const string F_F_FO_INPAY_BUD_GUID = "77de263b-f100-43c7-bafe-64a19694105a";
        private const string F_F_BALANC_KVSR_GUID = "2d0cb0f1-1918-423f-833f-b503f7f394f4";
        private const string F_F_FO_TRANSFER_GUID = "d57ac1f9-4110-492c-8cbc-62779e560df5";
        private const string F_F_FO_FIN_OBJ_GUID = "084f614f-aae5-4655-a6de-935ced66969d";

        private const string DIMENSION_YEAR_GUID = "b4612528-0e51-4e6b-8891-64c22611816b";
        private const string DIMENSION_YEAR_BEGIN_NAME = "Год начала строительства";
        private const string DIMENSION_YEAR_PLAN_NAME = "Год планируемого ввода";

        #endregion GUIDs
        private void InitDBObjectsNovosib()
        {
            clsRegion = this.Scheme.Classifiers[D_REGION_GUID];
            clsMarksPof = this.Scheme.Classifiers[D_MARKS_POF_GUID];
            clsHolidays = this.Scheme.Classifiers[D_HOLIDAYS_GUID];
            clsMarksFOInpayBud = this.Scheme.Classifiers[D_MARKS_FO_INPAY_BUD_GUID];
            clsMarksFOTarget = this.Scheme.Classifiers[D_MARKS_FO_TARGET_GUID];
            clsTransfert = this.Scheme.Classifiers[D_TRANSFERT_GUID];
            clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];
            clsEkr = this.Scheme.Classifiers[D_EKR_GUID];
            clsKd = this.Scheme.Classifiers[D_KD_GUID];
            clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID];
            clsFactExct = this.Scheme.Classifiers[D_FACT_EXCT_CACH_PL_GUID];

            this.UsedFacts = new IFactTable[] {
                fctPof = this.Scheme.FactTables[F_F_POF_GUID],
                fctFOPayDoc = this.Scheme.FactTables[F_F_PAY_DOC_GUID],
                fctFOBudLiab = this.Scheme.FactTables[F_F_BUD_LIAB_GUID],
                fctCalcBalanc = this.Scheme.FactTables[F_F_CALC_BALANC_GUID],
                fctFOInpayBud = this.Scheme.FactTables[F_F_FO_INPAY_BUD_GUID],
                fctBalancKVSR = this.Scheme.FactTables[F_F_BALANC_KVSR_GUID],
                fctFOTransfer = this.Scheme.FactTables[F_F_FO_TRANSFER_GUID],
                fctFOFinObj = this.Scheme.FactTables[F_F_FO_FIN_OBJ_GUID]
            };

            this.UsedClassifiers = new IClassifier[] { clsMarksPof };
            this.CubeClassifiers = new IClassifier[] { clsMarksPof, clsKvsr, clsEkr, clsMarksFOTarget, clsRegion, clsFactExct, clsTransfert };
            this.dimensionsForProcess = new string[] { DIMENSION_YEAR_GUID, DIMENSION_YEAR_BEGIN_NAME, DIMENSION_YEAR_GUID, DIMENSION_YEAR_PLAN_NAME };
        }

        private void QueryDataNovosib()
        {
            InitClsDataSet(ref daRegion, ref dsRegion, clsRegion);
            InitClsDataSet(ref daMarksPof, ref dsMarksPof, clsMarksPof);
            InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
            InitClsDataSet(ref daMarksFOInpayBud, ref dsMarksFOInpayBud, clsMarksFOInpayBud);
            InitClsDataSet(ref daMarksFOTarget, ref dsMarksFOTarget, clsMarksFOTarget);
            InitClsDataSet(ref daTransfert, ref dsTransfert, clsTransfert);
            InitClsDataSet(ref daKd, ref dsKd, clsKd);
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls);
            InitClsDataSet(ref daFactExct, ref dsFactExct, clsFactExct);

            InitDataSet(ref daHolidays, ref dsHolidays, clsHolidays, string.Empty);

            InitFactDataSet(ref daPof, ref dsPof, fctPof);
            InitFactDataSet(ref daFOPayDoc, ref dsFOPayDoc, fctFOPayDoc);
            InitFactDataSet(ref daFOBudLiab, ref dsFOBudLiab, fctFOBudLiab);
            InitFactDataSet(ref daCalcBalanc, ref dsCalcBalanc, fctCalcBalanc);
            InitFactDataSet(ref daFOInpayBud, ref dsFOInpayBud, fctFOInpayBud);
            InitFactDataSet(ref daBalancKVSR, ref dsBalancKVSR, fctBalancKVSR);
            InitFactDataSet(ref daFOTransfer, ref dsFOTransfer, fctFOTransfer);
            InitFactDataSet(ref daFOFinObj, ref dsFOFinObj, fctFOFinObj);

            FillCachesNovosib();
        }

        private void FillCachesNovosib()
        {
            FillRowsCache(ref cacheRegion, dsRegion.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheMarksPof, dsMarksPof.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheHolidays, dsHolidays.Tables[0], "DayoffDate", "DayOff");
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheKvsrName, dsKvsr.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheMarksFOInpayBud, dsMarksFOInpayBud.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheMarksFOTarget, dsMarksFOTarget.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheTransfert, dsTransfert.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheOutcomesCls, dsOutcomesCls.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheFactExct, dsFactExct.Tables[0], "Code", "Id");
        }

        private void UpdateDataNovosib()
        {
            UpdateDataSet(daRegion, dsRegion, clsRegion);
            UpdateDataSet(daMarksPof, dsMarksPof, clsMarksPof);
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daMarksFOInpayBud, dsMarksFOInpayBud, clsMarksFOInpayBud);
            UpdateDataSet(daMarksFOTarget, dsMarksFOTarget, clsMarksFOTarget);
            UpdateDataSet(daTransfert, dsTransfert, clsTransfert);
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);
            UpdateDataSet(daFactExct, dsFactExct, clsFactExct);

            UpdateDataSet(daPof, dsPof, fctPof);
            UpdateDataSet(daFOPayDoc, dsFOPayDoc, fctFOPayDoc);
            UpdateDataSet(daFOBudLiab, dsFOBudLiab, fctFOBudLiab);
            UpdateDataSet(daCalcBalanc, dsCalcBalanc, fctCalcBalanc);
            UpdateDataSet(daFOInpayBud, dsFOInpayBud, fctFOInpayBud);
            UpdateDataSet(daBalancKVSR, dsBalancKVSR, fctBalancKVSR);
            UpdateDataSet(daFOTransfer, dsFOTransfer, fctFOTransfer);
            UpdateDataSet(daFOFinObj, dsFOFinObj, fctFOFinObj);
        }

        private void PumpFinalizingNovosib()
        {
            ClearDataSet(ref dsPof);
            ClearDataSet(ref dsFOPayDoc);
            ClearDataSet(ref dsFOBudLiab);
            ClearDataSet(ref dsCalcBalanc);
            ClearDataSet(ref dsFOInpayBud);
            ClearDataSet(ref dsBalancKVSR);
            ClearDataSet(ref dsFOTransfer);
            ClearDataSet(ref dsFOFinObj);

            ClearDataSet(ref dsKvsr);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsMarksPof);
            ClearDataSet(ref dsHolidays);
            ClearDataSet(ref dsMarksFOInpayBud);
            ClearDataSet(ref dsMarksFOTarget);
            ClearDataSet(ref dsRegion);
            ClearDataSet(ref dsTransfert);
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsOutcomesCls);
            ClearDataSet(ref dsFactExct);
        }

        #endregion Работа с базой и кэшами

        #region fctCalcBalanc (f_F_CalcBalanc)

        private int fctCalcBalancGetRefMarks(int strNumber)
        {
            switch (strNumber)
            {
                case 1:
                    return 0;
                case 2:
                    return 1;
                case 3:
                    return 2;
                case 4:
                    return 3;
                case 5:
                    return 4;
                case 6:
                    return 5;
                case 7:
                    return 6;
            }
            return 0;
        }

        private void PumpCalcBalancNovosib()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Остатки средств бюджета'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                int nextDate = FOPayDocNovosibGetNextDay(curDate, 1);
                string budgetStartDate = CommonRoutines.NewDateToLongDate(startDate.ToString());
                string budgetCurDate = CommonRoutines.NewDateToLongDate(curDate.ToString());

                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(09.01.01.04) Остаток средств областного бюджета.xlc",
                    "КонечнаяДата", budgetCurDate, "НачальнаяДата", budgetStartDate,
                    "ТипДатыУнивер", "2", "ТипКлассификации", "0,1,2", "ДатаРеестраТипЧек", "1" }, ref reportData))
                    return;

                int rowsCount = reportData.Tables[0].Rows.Count;

                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int strNumber = Convert.ToInt32(row["НомерСтроки"].ToString().PadLeft(1, '0'));
                    int refMarks = fctCalcBalancGetRefMarks(strNumber);

                    decimal balance = Convert.ToDecimal(row["Всего"].ToString().Replace("X", string.Empty).PadLeft(1, '0'));
                    decimal income = Convert.ToDecimal(row["БезвозмездныеПеречисления"].ToString().Replace("X", string.Empty).PadLeft(1, '0'));
                    decimal unReturn = Convert.ToDecimal(row["Возврат"].ToString().Replace("X", string.Empty).PadLeft(1, '0'));
                    decimal fBReturn = Convert.ToDecimal(row["ВозвратПрошлыхЛет"].ToString().Replace("X", string.Empty).PadLeft(1, '0'));
                    decimal returnOld = Convert.ToDecimal(row["НевыясненныеПоступления"].ToString().Replace("X", string.Empty).PadLeft(1, '0'));
                    decimal returnCurr = Convert.ToDecimal(row["ОстальныеДоходы"].ToString().Replace("X", string.Empty).PadLeft(1, '0'));
                    if ((balance == 0) && (income == 0) && (unReturn == 0) && (fBReturn == 0) && (returnOld == 0) && (returnCurr == 0))
                        continue;

                    PumpRow(dsCalcBalanc.Tables[0], new object[] { "RefYearDayUNV", nextDate, "RefMarks", refMarks, "balance", balance, "income", income,
                        "unReturn", unReturn, "fBReturn", fBReturn, "ReturnOld", returnOld, "returnCurr", returnCurr });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();

            WriteToTrace("конец закачки - 'Факт.ФО_Остатки средств бюджета'", TraceMessageKind.Information);
        }

        #endregion fctCalcBalanc (f_F_CalcBalanc)

        #region fctPof (f_F_FOPOF)

        private void PumpPofNovosib()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Предельные объемы финансирования'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "Платежный календарь новый_для закачки ФМ.xlc",
                    "ОднаДата", reportDate, "РосписьЧек", "1", "ВариантРосписи", "0", "УведомленияЧек", "1", "ТипКлассификацииРасх", "",
                    "РСФО", "", "РСФОЧек", "1", "ТипСредствБюдж", "00.00.00-02.99.99", "ТипСредствБюджЧек", "8" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    if ((row["Критерий"].ToString().Trim() == string.Empty) && (row["НомПП"].ToString().Trim() == string.Empty))
                        continue;
                    string code = row["Критерий"].ToString().Trim();
                    if (code == string.Empty)
                        code = row["Группа"].ToString().Trim();
                    string name = row["Наименование"].ToString().Trim();
                    int refMarks = PumpCachedRow(cacheMarksPof, dsMarksPof.Tables[0], clsMarksPof, code,
                        new object[] { "Code", code, "Name", name } );

                    decimal planeRep = Convert.ToDecimal(row["ПланНаМесяц"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factDRep = Convert.ToDecimal(row["РасходЗаДень"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factMRep = Convert.ToDecimal(row["РасходЗаМесяц"].ToString().PadLeft(1, '0')) * 1000;
                    decimal balancRep = Convert.ToDecimal(row["Остаток"].ToString().PadLeft(1, '0')) * 1000;

                    if ((planeRep == 0) && (factDRep == 0) && (factMRep == 0) && (balancRep == 0))
                        continue;

                    PumpRow(dsPof.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", refMarks,
                            "planeRep", planeRep, "factDRep", factDRep, "factMRep", factMRep, "balancRep", balancRep });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();

            WriteToTrace("конец закачки - 'Факт.ФО_Предельные объемы финансирования'", TraceMessageKind.Information);
        }

        #endregion fctPof (f_F_FOPOF)

        #region fctFOPayDoc (f_F_FOPayDoc)

        private void PumpFOPayDocNovosib(string reportName, string refStateDoc, int date)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Реестр платежных поручений' " + refStateDoc, TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", reportName,
                    "ОднаДата", date.ToString(), "ТипКлассификацииРасх", string.Empty,
                    "ТипСредствБюдж", "10000-10600", "ТипСредствБюджЧек", "1" }, ref reportData);

                if (reportData.Tables.Count == 0)
                    return;

                int rowsCount = reportData.Tables[0].Rows.Count;

                int refKvsr = -1;
                int refEkr = -1;
                int kvsrCode = 1;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    string kvsrName = row["RowNote"].ToString().Trim();
                    if (kvsrName != string.Empty)
                        if (!kvsrName.ToUpper().StartsWith("ИТОГО"))
                        {
                            refKvsr = PumpCachedRow(cacheKvsrName, dsKvsr.Tables[0], clsKvsr, kvsrName,
                                new object[] { "Code", kvsrCode, "Name", kvsrName, "CodeLine", kvsrCode });
                            kvsrCode++;
                        }
                    string ekrName = row["КЭСР"].ToString().Trim();
                    if (ekrName == string.Empty)
                        continue;
                    string ekrCode = CommonRoutines.TrimLetters(ekrName).Replace(".", string.Empty).TrimStart('0').PadLeft(1, '0');
                    ekrName = ekrName.Split(')')[1].Trim();
                    refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                        new object[] { "Code", ekrCode, "Name", ekrName });

                    decimal fact = Convert.ToDecimal(row["Сумма"].ToString().PadLeft(1, '0'));
                    if (fact == 0)
                        continue;

                    PumpRow(dsFOPayDoc.Tables[0], new object[] { "RefYearDayUNV", date,
                        "fact", fact, "RefState", refStateDoc, "RefKVSR", refKvsr, "RefEKR", refEkr });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Реестр платежных поручений' " + refStateDoc, TraceMessageKind.Information);
        }

        private int FOPayDocNovosibGetNextDay(int currentDate, double increment)
        {
            DateTime nxtDate = new DateTime(currentDate / 10000, (currentDate / 100) % 100, currentDate % 100);
            do
            {
                nxtDate = nxtDate.AddDays(increment);
                if (cacheHolidays.ContainsKey(nxtDate.ToString()))
                {
                    if (cacheHolidays[nxtDate.ToString()] == 0)
                        break;
                }
                else
                {
                    if ((nxtDate.DayOfWeek != DayOfWeek.Saturday) && (nxtDate.DayOfWeek != DayOfWeek.Sunday))
                        break;
                }

            }
            while (1 == 1);
            return CommonRoutines.ShortDateToNewDate(nxtDate.ToShortDateString());
        }

        private void PumpFOPayDocNovosib()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Реестр платежных поручений' " + curDate.ToString(), TraceMessageKind.Information);
            PumpFOPayDocNovosib("(03.09.01.01) Реестр плат поруч_1.xlc", "1", curDate);
            PumpFOPayDocNovosib("(03.09.01.01) Реестр плат поруч_2.xlc", "2", curDate);
            PumpFOPayDocNovosib("(03.09.01.01) Реестр плат поруч_3.xlc", "3", curDate);
            WriteToTrace("конец закачки - 'Факт.ФО_Реестр платежных поручений' " + curDate.ToString(), TraceMessageKind.Information);

            // след рабочий день
            int nextDate = FOPayDocNovosibGetNextDay(curDate, 1);
            WriteToTrace("начало закачки - 'Факт.ФО_Реестр платежных поручений' " + nextDate.ToString(), TraceMessageKind.Information);
            PumpFOPayDocNovosib("(03.09.01.01) Реестр плат поруч_1.xlc", "1", nextDate);
            PumpFOPayDocNovosib("(03.09.01.01) Реестр плат поруч_2.xlc", "2", nextDate);
            WriteToTrace("конец закачки - 'Факт.ФО_Реестр платежных поручений' " + nextDate.ToString(), TraceMessageKind.Information);
        }

        #endregion fctFOPayDoc (f_F_FOPayDoc)

        #region fctFOBudLiab (f_F_FOBudLiab)

        private void PumpFOBudLiab(string reportName, int reportIndex)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Бюджетные обязательства' - " + reportName, TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", reportName,
                    "ОднаДата", curDate.ToString(),
                    "ТипСредствБюдж", "10000-10600", "ТипСредствБюджЧек", "1",
                    "ДатаКонцаПериода", curDate.ToString(),
                    "ДатаНачалаПериода", Convert.ToString(curDate / 10000 * 10000),
                    "ДатаОтчета", curDate.ToString(),
                    "ДатаДоТекущей", Convert.ToString(curDate - 1),
                    "РосписьЧек", "1", "ВариантРосписи", "0", "УведомленияЧек", "1",  }, ref reportData);

                if (reportData.Tables.Count == 0)
                    return;

                int rowsCount = reportData.Tables[0].Rows.Count;

                int refKvsr = -1;
                int refEkr = -1;
                int kvsrCode = 1;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    string kvsrName = row["ЛицевойСчет"].ToString().Trim();
                    if (kvsrName != string.Empty)
                    {
                        refKvsr = PumpCachedRow(cacheKvsrName, dsKvsr.Tables[0], clsKvsr, kvsrName,
                            new object[] { "Code", kvsrCode, "Name", kvsrName, "CodeLine", kvsrCode });
                        kvsrCode++;
                    }
                    string ekrName = row["КЭСР"].ToString().Trim();
                    if (ekrName == string.Empty)
                        continue;
                    string ekrCode = CommonRoutines.TrimLetters(ekrName).Replace(".", string.Empty).TrimStart('0').PadLeft(1, '0');
                    ekrName = ekrName.Split(')')[1].Trim();
                    refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                        new object[] { "Code", ekrCode, "Name", ekrName });

                    decimal limY = 0;
                    decimal budLiab = 0;
                    decimal budLiabD = 0;
                    decimal restLim = 0;
                    decimal budLiabCon = 0;

                    if (reportIndex == 1)
                    {
                        limY = Convert.ToDecimal(row["ЛимитыБОНаГодВыч"].ToString().PadLeft(1, '0'));
                        budLiabCon = Convert.ToDecimal(row["БОНаРассм"].ToString().PadLeft(1, '0'));
                    }
                    else
                    {
                        budLiab = Convert.ToDecimal(row["Колонка_25"].ToString().PadLeft(1, '0'));
                        budLiabD = Convert.ToDecimal(row["Колонка_26"].ToString().PadLeft(1, '0'));
                    }

                    if ((limY == 0) && (budLiab == 0) && (budLiabD == 0) && (restLim == 0) && (budLiabCon == 0))
                        continue;

                    PumpRow(dsFOBudLiab.Tables[0], new object[] { "RefYearDayUNV", curDate,
                        "limY", limY, "budLiab", budLiab, "budLiabD", budLiabD, "restLim", restLim,
                        "budLiabCon", budLiabCon, "RefKVSR", refKvsr, "RefEKR", refEkr });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();

            WriteToTrace("конец закачки - 'Факт.ФО_Бюджетные обязательства' - " + reportName, TraceMessageKind.Information);
        }

        private void PumpFOBudLiab()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Бюджетные обязательства'", TraceMessageKind.Information);
            PumpFOBudLiab("(25.02.03.90) справка об исполнении принятых на учет бюджетных обязательств_1.xlc", 1);
            PumpFOBudLiab("(25.02.03.98) справка об исполнении принятых на учет бюджетных обязательств_ФЭА.xlc", 2);
            WriteToTrace("конец закачки - 'Факт.ФО_Бюджетные обязательства'", TraceMessageKind.Information);
        }

        #endregion fctFOBudLiab (f_F_FOBudLiab)

        #region fctFOInpayBud (f_F_FOInpayBud)

        private void PumpFOInpayBudNovosib()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Поступления в бюджет'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                int prevDate = FOPayDocNovosibGetNextDay(curDate, -1);
                string budgetPrevDate = CommonRoutines.NewDateToLongDate(prevDate.ToString());

                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(00.00.15.02) Доходы областного бюджета на тек. дату тек. месяца.xlc",
                    "ОднаДата", budgetPrevDate }, ref reportData))
                    return;

                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string code = row["ID"].ToString().Trim('=');
                    if (code == string.Empty)
                        continue;
                    int refMarks = PumpCachedRow(cacheMarksFOInpayBud, dsMarksFOInpayBud.Tables[0], clsMarksFOInpayBud, code,
                        new object[] { "Code", code, "Name", row["Имя"].ToString(), "Note", row["КБКФ"].ToString() });

                    decimal planeM = Convert.ToDecimal(row["ПланНаМесяц"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factM = Convert.ToDecimal(row["ПоступлениеЗаМесяц"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factD = Convert.ToDecimal(row["ПоступлениеЗаДень"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factMPastY = Convert.ToDecimal(row["ПоступлениеЗаМесяцПрошлыйГод"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factWPastY = Convert.ToDecimal(row["ПостСНачГодРабПрошлыйГод"].ToString().PadLeft(1, '0')) * 1000;
                    decimal factW = Convert.ToDecimal(row["ПостСНачГодРабТекГод"].ToString().PadLeft(1, '0')) * 1000;
                    if ((planeM == 0) && (factM == 0) && (factD == 0) && (factMPastY == 0) && (factWPastY == 0) && (factW == 0))
                        continue;

                    PumpRow(dsFOInpayBud.Tables[0], new object[] { "RefYearDayUNV", prevDate, "RefMarks", refMarks, "PlaneM", planeM, "FactM", factM,
                        "FactD", factD, "FactMPastY", factMPastY, "FactWPastY", factWPastY, "FactW", factW });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();

            WriteToTrace("конец закачки - 'Факт.ФО_Поступления в бюджет'", TraceMessageKind.Information);
        }

        #endregion fctFOInpayBud (f_F_FOInpayBud)

        #region Получение КВСР из базы АС Бюджет

        private void InitBudgetDB(FileInfo udlFile)
        {
            IDbConnection connection = null;
            string errString = UDLFileDataAccess.GetConnectionFromUdl(udlFile.FullName, ref connection, ref isOleDbConnection);

            if (connection == null)
            {
                throw new Exception("Невозможно установить соединение с источником данных. " +
                    "Проверьте настройку подключения к базе АС Бюджет. (" + errString + ")");
            }

            if (isOleDbConnection)
            {
                budgetDB = new Database(connection,
                    DbProviderFactories.GetFactory("System.Data.OleDb"), false, constCommandTimeout);
            }
            else
            {
                budgetDB = new Database(connection,
                    DbProviderFactories.GetFactory("System.Data.Odbc"), false, constCommandTimeout);
            }
        }

        private void DisposeBudgetDB()
        {
            if (budgetDB != null)
            {
                budgetDB.Dispose();
                budgetDB = null;
            }
        }

        // заполняем кэш КВСР из АС Бюджет
        private void PumpKvsrFromBudget(DirectoryInfo dir)
        {
            cacheKvsrFromBudget = new Dictionary<string, string>();
            string udlFilename = string.Format("{0}\\budget.udl", dir.FullName);
            if (!File.Exists(udlFilename))
                return;

            WriteToTrace("Обработка 'budget.udl'", TraceMessageKind.Information);
            FileInfo udlFile = new FileInfo(udlFilename);
            InitBudgetDB(udlFile);
            try
            {
                // в таблице TRANSFERTCLS есть соответствие CODE -> TRANSFERKVSR,
                // где CODE - код целевых средств, который получим при закачке Xls-файла
                // из таблицы KVSR по TRANSFERKVSR берем CODE и NAME администратора
                DataTable kvsrData = (DataTable)budgetDB.ExecQuery(
                    " select TR.CODE TRCODE, K.CODE CODE, K.NAME NAME " +
                    " from KVSR K, TRANSFERTCLS TR " +
                    " where TR.TRANSFERKVSR = K.ID ", QueryResultTypes.DataTable, new IDbDataParameter[] { });

                List<string> usedKeys = new List<string>();
                foreach (DataRow kvsrRow in kvsrData.Rows)
                {
                    string key = Convert.ToString(kvsrRow["TRCODE"]);
                    // если одному Коду целевых средств соответствует несколько администраторов,
                    // то администраторов из Бюджета не берем, а при закачке будет ставить ссылку на -1
                    if (!usedKeys.Contains(key))
                    {
                        usedKeys.Add(key);
                        cacheKvsrFromBudget.Add(key, string.Format("{0}|{1}", Convert.ToString(kvsrRow["CODE"]), Convert.ToString(kvsrRow["NAME"])));
                    }
                    else if (cacheKvsrFromBudget.ContainsKey(key))
                    {
                        cacheKvsrFromBudget.Remove(key);
                    }
                }
            }
            finally
            {
                DisposeBudgetDB();
            }
        }

        #endregion Получение КВСР из базы АС Бюджет

        private void DeleteDataNovosib()
        {
            string constr = string.Format("RefYearDayUNV = {0}", curDate);
            DirectDeleteFactData(new IFactTable[] { fctPof, fctFOPayDoc, fctFOBudLiab }, -1, this.SourceID, constr);

            int prevDate = FOPayDocNovosibGetNextDay(curDate, -1);
            constr = string.Format("RefYearDayUNV = {0}", prevDate);
            DirectDeleteFactData(new IFactTable[] { fctFOInpayBud }, -1, this.SourceID, constr);

            int nextDate = FOPayDocNovosibGetNextDay(curDate, 1);
            constr = string.Format("RefYearDayUNV = {0}", nextDate);
            DirectDeleteFactData(new IFactTable[] { fctCalcBalanc, fctFOPayDoc }, -1, this.SourceID, constr);
        }

        private void GetDateNovosib()
        {
            reportDate = GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "meReportDate", string.Empty);
            if (reportDate == string.Empty)
            {
                curDate = CommonRoutines.ShortDateToNewDate(DateTime.Now.ToShortDateString());
                reportDate = string.Format("{0}.{1}.{2}", DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Year.ToString());
            }
            else
                curDate = CommonRoutines.ShortDateToNewDate(reportDate.Replace(".", ""));

            // если год даты не совпадает годом из удл - заменяем год в датах на год из удл
            if (udlYear != string.Empty)
                if ((curDate / 10000).ToString() != udlYear)
                {
                    curDate = Convert.ToInt32(udlYear) * 10000 + curDate % 10000;
                    reportDate = string.Format("{0}.{1}.{2}", reportDate.Split('.')[0], reportDate.Split('.')[1], udlYear);
                }

            startDate = curDate / 10000 * 10000 + 101;
            startMonthDate = curDate / 100 * 100 + 1;
            dateMonth = (curDate / 100 % 100).ToString().PadLeft(2, '0');
            dateYear = (curDate / 10000).ToString();
            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format("Обработка данных по дате: {0}", curDate));
        }

        private void PumpNovosibData(DirectoryInfo dir)
        {
            if (dir.GetFiles("20*.udl").GetLength(0) == 0)
                return;

            FileInfo[] fi = dir.GetFiles("20*.udl");
            foreach (FileInfo f in fi)
            {
                WriteToTrace(string.Format("обработка {0}", f.Name), TraceMessageKind.Information);
                udlYear = f.Name.Split('.')[0];
                GetDateNovosib();

                DeleteDataNovosib();

                budWP = new WorkplaceAutoObjectClass();
                try
                {
                    string[] udlData = CommonRoutines.GetTxtReportData(f, CommonRoutines.GetTxtWinCodePage())[2].Split(';');
                    string dbName = string.Empty;
                    string login = string.Empty;
                    string password = string.Empty;
                    GetUdlParams(udlData, ref dbName, ref login, ref password);
                    budWP.Login(dbName, login, password);

                    PumpCalcBalancNovosib();
                    PumpPofNovosib();
                    PumpFOPayDocNovosib();
                    PumpFOBudLiab();
                    PumpFOInpayBudNovosib();

                    UpdateData();
                }
                finally
                {
                    Marshal.ReleaseComObject(budWP);
                }
            }
        }

        #region Обработка

        #region Проставление ссылок на классификатор "Администратор.ИспКасПлан"

        // получить данные из таблицы f_R_FacialFinDetail, которые будут участвовать в проставлении ссылок на КВСР
        // данные сгруппированы по полю ID таблицы F_F_FOTRANSFER
        private Dictionary<int, List<DataRow>> GetGroupedFinData()
        {
            DataTable finData = this.DB.ExecQuery(string.Format(
                " select FFD.ID ID, FFD.DEBIT DEBIT, FFD.RETURNDEBIT RETURNDEBIT, FFD.REFKVSR REFKVSR, " +
                "        KVSR.CODE CODE, KVSR.NAME NAME, FTR.ID FTRID " +
                " from F_R_FACIALFINDETAIL FFD inner join D_KVSR_BUDGET KVSR on (FFD.REFKVSR = KVSR.ID), " +
                "      F_F_FOTRANSFER FTR " +
                " where " +
                // только для записей из таблицы f_F_FOTransfer с RefKVSR = -1
                "     FTR.REFKVSR = -1 and " +
                // год источника f_R_FacialFinDetail равен году источника записи таблицы f_F_FOTransfer
                "     FFD.SOURCEID in ( " +
                "         select DS.ID " +
                "         from DATASOURCES DS " +
                "         where upper(DS.SUPPLIERCODE) = upper('ФО') and " +
                "             DS.DATACODE = 1 and " +
                "             DS.KINDSOFPARAMS = {0} and " +
                "             DS.YEAR = {1} and " +
                "             DS.DELETED <> 1 " +
                "     ) and " +
                // ссылка на этот же район (d_Regions_ExctCachPl.Code = d_Regions_Budget.Code)
                "     FFD.REFREGIONS in ( " +
                "         select RB.ID " +
                "         from D_REGIONS_BUDGET RB inner join D_REGIONS_EXCTCACHPL RE " +
                "             on (RB.CODE = RE.CODE) " +
                "         where RE.ID = FTR.REFREGION " +
                "     ) and " +
                // ссылка на этот же код целевых средств (d_Transfert_Budget.Code = d_Transfert_ExctCachPl.Code)
                "     FFD.REFTRANSF in ( " +
                "         select TB.ID " +
                "         from D_TRANSFERT_BUDGET TB inner join D_TRANSFERT_EXCTCACHPL TE " +
                "             on (TB.CODE = TE.CODE) " +
                "         where TE.ID = FTR.REFTRANSF " +
                "     ) ", (int)ParamKindTypes.Budget, this.DataSource.Year),
                QueryResultTypes.DataTable, new IDbDataParameter[] { }) as DataTable;

            Dictionary<int, List<DataRow>> groupedFinData = new Dictionary<int, List<DataRow>>();
            foreach (DataRow row in finData.Rows)
            {
                int ftrID = Convert.ToInt32(row["FTRID"]);
                if (!groupedFinData.ContainsKey(ftrID))
                    groupedFinData.Add(ftrID, new List<DataRow>(new DataRow[] { row }));
                else
                    groupedFinData[ftrID].Add(row);
            }

            return groupedFinData;
        }

        // группируем записи по коду администратора и вычисляем суммы следующи образом: Расход = Debit – ReturnDebit
        // ключ - RefKVSR, значение - сумма Расход = Debit – ReturnDebit
        private Dictionary<string, decimal> GroupFinDataByKVSR(List<DataRow> finData)
        {
            Dictionary<string, decimal> groupedData = new Dictionary<string, decimal>();

            foreach (DataRow row in finData)
            {
                string kvsrKey = GetComplexCacheKey(row, new string[] { "CODE", "NAME" }, "|");
                decimal debit = Convert.ToDecimal(row["Debit"]);
                decimal returnDebit = Convert.ToDecimal(row["ReturnDebit"]);
                if (!groupedData.ContainsKey(kvsrKey))
                    groupedData.Add(kvsrKey, debit - returnDebit);
                else
                    groupedData[kvsrKey] += (debit - returnDebit);
            }

            return groupedData;
        }

        private int GetRefKVSR(string kvsrKey)
        {
            if (cacheKvsr.ContainsKey(kvsrKey))
                return cacheKvsr[kvsrKey];
            string[] kvsrData = kvsrKey.Split(new char[] { '|' });
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrKey, new object[] {
                "Code", kvsrData[0], "Name", kvsrData[1], "CodeLine", kvsrData[0] });
        }

        private int GetKVSRFromFinData(DataRow transferData, List<DataRow> finData)
        {
            Dictionary<string, decimal> finDataByKvsr = GroupFinDataByKVSR(finData);
            // сравниваем сумму в поле Proceeds с суммами Расхода, если находим равную сумму,
            // то в таблице фактов f_F_FOTransfer ставим ссылку на указанный код KVSR
            decimal proceeds = Convert.ToDecimal(transferData["Proceeds"]);
            decimal outgoing = Convert.ToDecimal(transferData["Outgoing"]);
            foreach (KeyValuePair<string, decimal> sumByKvsr in finDataByKvsr)
            {
                if (proceeds == sumByKvsr.Value)
                    return GetRefKVSR(sumByKvsr.Key);
                if (outgoing == sumByKvsr.Value)
                    return GetRefKVSR(sumByKvsr.Key);
            }
            return -1;
        }

        private void WriteUnsetRecords(Dictionary<int, List<string>> unsetRecords)
        {
            foreach (KeyValuePair<int, List<string>> unsetRecord in unsetRecords)
            {
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                    "В таблице фактов \"ФО_Поступление и расходование МБТ\" за {0} остались записи с неуказанным кодом администратора: {1}.",
                    CommonRoutines.NewDateToLongDate(unsetRecord.Key.ToString()), string.Join(", ", unsetRecord.Value.ToArray())));
            }
        }

        // Проставление ссылок на классификатор "Администратор.ИспКасПлан" (d_KVSR_ExctCachPl)
        // в таблице фактов "ФО_Поступление и расходование МБТ" (f_F_FOTransfer)
        private void SetRefsKvsrToTransfer()
        {
            Dictionary<int, List<DataRow>> groupedFinData = GetGroupedFinData();

            // получаем данные, у которых не проставлены ссылки на "Администратор.ИспКасПлан"
            InitDataSet(ref daFOTransfer, ref dsFOTransfer, fctFOTransfer,
                string.Format("SourceId = {0} and RefKVSR = -1", this.SourceID));

            // коллекция записей, у которых не удалось проставить ссылку на администратора
            Dictionary<int, List<string>> unsetRecords = new Dictionary<int, List<string>>();
            try
            {
                foreach (DataRow row in dsFOTransfer.Tables[0].Rows)
                {
                    int id = Convert.ToInt32(row["ID"]);
                    int refKvsr = -1;
                    if (groupedFinData.ContainsKey(id))
                        refKvsr = GetKVSRFromFinData(row, groupedFinData[id]);

                    row["RefKVSR"] = refKvsr;
                    if (refKvsr == -1)
                    {
                        int refDate = Convert.ToInt32(row["RefYearDayUNV"]);
                        if (!unsetRecords.ContainsKey(refDate))
                            unsetRecords.Add(refDate, new List<string>());
                        unsetRecords[refDate].Add(row["ID"].ToString());
                    }
                }

                WriteUnsetRecords(unsetRecords);

                UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
                UpdateDataSet(daFOTransfer, dsFOTransfer, fctFOTransfer);
            }
            finally
            {
                unsetRecords.Clear();
            }
        }

        #endregion Проставление ссылок на классификатор "Администратор.ИспКасПлан"

        private void CalcSumFOBudLiab(string constr)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fctFOBudLiab, false, constr, string.Empty);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                row["RestLim"] = Convert.ToDecimal(row["LimY"]) -
                    Convert.ToDecimal(row["BudLiab"]) - Convert.ToDecimal(row["BudLiabD"]);
            }
            UpdateDataSet(da, ds, fctFOBudLiab);
        }

        private void ProcessNovosibData()
        {
            SetRefsKvsrToTransfer();
            GetDateNovosib();
            string constr = string.Format("SOURCEID = {0} and RefYearDayUNV = {1}", this.SourceID, curDate);
            // корректировка сумм ПОФ
            CommonSumCorrectionConfig scc = new CommonSumCorrectionConfig();
            scc.Sum1Report = "planeRep";
            scc.Sum1 = "plane";
            scc.Sum2Report = "factDRep";
            scc.Sum2 = "FactDay";
            scc.Sum3Report = "factMRep";
            scc.Sum3 = "FactMonth";
            scc.Sum4Report = "balancRep";
            scc.Sum4 = "Balance";
            CorrectFactTableSums(fctPof, dsMarksPof.Tables[0], clsMarksPof, "RefMarks", scc, BlockProcessModifier.MRStandard,
                new string[] { "RefYearDayUNV" }, string.Empty, string.Empty, true, constr);
            // группировка и вычисление полей БО
            GroupTable(fctFOBudLiab, new string[] { "RefKVSR", "RefEKR", "RefYearDayUNV" },
                new string[] { "LimY", "BudLiab", "BudLiabD", "BudLiabCon", "RestLim" }, constr);
            CalcSumFOBudLiab(constr);
        }

        #endregion Обработка

    }

}
