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
using System.Xml;
using Krista.FM.Common.Xml;

using WorkPlace;
using VariablesTools;
using System.Runtime.InteropServices;

namespace Krista.FM.Server.DataPumps.FO35Pump
{

    public partial class FO35PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region классификаторы

        // СубКОСГУ.ФО_ИспКасПлан (d_SubKESR_ExctCachPl)
        private IDbDataAdapter daSubKESR;
        private DataSet dsSubKESR;
        private IClassifier clsSubKESR;
        private Dictionary<string, int> cacheSubKESR = null;

        #endregion классификаторы

        #region Факты

        // Факт.ФО_Доходы бюджета (f_F_EarningsBud)
        private IDbDataAdapter daEarningsBud;
        private DataSet dsEarningsBud;
        private IFactTable fctEarningsBud;
        // Факт.ФО_Расходы бюджета (f_F_ChargesBud)
        private IDbDataAdapter daChargesBud;
        private DataSet dsChargesBud;
        private IFactTable fctChargesBud;

        #endregion Факты

        #endregion Поля

        #region Работа с базой и кэшами

        private const string D_SUB_KESR_GUID = "369316e9-325c-448f-a008-e6b8a8c79dfd";
        private const string F_F_EARNINGS_BUD_GUID = "0b46d2ff-30c7-4683-96c5-8077312943ff";
        private const string F_F_CHARGES_BUD_GUID = "86b6c77e-c101-412d-bdbd-36b1c86efdf4";
        private void InitDBObjectsVologda()
        {
            clsKd = this.Scheme.Classifiers[D_KD_GUID];
            clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID];
            clsEkr = this.Scheme.Classifiers[D_EKR_GUID];
            clsSubKESR = this.Scheme.Classifiers[D_SUB_KESR_GUID];

            fctEarningsBud = this.Scheme.FactTables[F_F_EARNINGS_BUD_GUID];
            fctChargesBud = this.Scheme.FactTables[F_F_CHARGES_BUD_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.HierarchyClassifiers = new[] { clsKd, clsOutcomesCls, clsEkr, clsSubKESR };
            this.UsedFacts = new IFactTable[] { };
        }

        private void FillCachesVologda()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheOutcomesCls, dsOutcomesCls.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheSubKESR, dsSubKESR.Tables[0], "Code", "Id");
        }

        private void QueryDataVologda()
        {
            InitClsDataSet(ref daKd, ref dsKd, clsKd);
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
            InitClsDataSet(ref daSubKESR, ref dsSubKESR, clsSubKESR);

            InitFactDataSet(ref daEarningsBud, ref dsEarningsBud, fctEarningsBud);
            InitFactDataSet(ref daChargesBud, ref dsChargesBud, fctChargesBud);

            FillCachesVologda();
        }

        private void UpdateDataVologda()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daSubKESR, dsSubKESR, clsSubKESR);

            UpdateDataSet(daEarningsBud, dsEarningsBud, fctEarningsBud);
            UpdateDataSet(daChargesBud, dsChargesBud, fctChargesBud);
        }

        private void PumpFinalizingVologda()
        {
            ClearDataSet(ref dsEarningsBud);
            ClearDataSet(ref dsChargesBud);

            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsOutcomesCls);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsSubKESR);
        }

        #endregion Работа с базой и кэшами

        #region fctEarningsBud (f_F_EarningsBud)

        private void PumpFctEarningsBudVologda09010191()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Доходы бюджета - 09.01.01.91'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(09.01.01.91) Доходы_ФЭА.xlc", 
                    "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1", "НачальнаяДата", startDate.ToString(), 
                    "КонечнаяДата", curDate.ToString(), "ТипСредствБюджЧек", "1", "ТипСредствБюдж", "010000-010800,030000-039603,=060000" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kdCode = row["КодДохода"].ToString().Trim();
                    string kdName = row["КодДоходаИмя"].ToString().Trim();
                    if (kdCode == string.Empty)
                        continue;
                    // берем только  Вид дохода = 1.XX.ХХ.ХХХ.ХХ.ХХХХ (это с 4 символа в коде дохода)
                    if (kdCode.Substring(3, 1) != "1")
                        continue;
                    int refKd = PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, kdCode,
                        new object[] { "CodeStr", kdCode, "Name", kdName });
                    decimal factPeriod = Convert.ToDecimal(row["Итог"].ToString().Trim().PadLeft(1, '0'));
                    if (factPeriod == 0)
                        continue;
                    int acceptDate = CommonRoutines.ShortDateToNewDate(row["ДатаПринятия"].ToString().Trim());
                    PumpRow(dsEarningsBud.Tables[0], new object[] { "RefYearDayUNV", acceptDate, "RefKD", refKd, "FactPeriod", factPeriod });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Доходы бюджета - 09.01.01.91'", TraceMessageKind.Information);
        }

        private void PumpFctEarningsBudVologda()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Доходы бюджета'", TraceMessageKind.Information);
            PumpFctEarningsBudVologda09010191();
            WriteToTrace("конец закачки - 'Факт.ФО_Доходы бюджета'", TraceMessageKind.Information);
        }

        #endregion fctEarningsBud (f_F_EarningsBud)

        #region fctChargesBud (f_F_ChargesBud)

        private int PumpOutcomesClsVologda(string code, string name)
        {
            code = code.PadLeft(4, '0').PadRight(14, '0').TrimStart('0').PadLeft(1, '0');
            return PumpCachedRow(cacheOutcomesCls, dsOutcomesCls.Tables[0], clsOutcomesCls, code,
                new object[] { "Code", code, "Name", name });
        }

        private int PumpEkrVologda(string code, string name)
        {
            return PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, code,
                new object[] { "Code", code, "Name", name });
        }

        private int PumpSubKesrVologda(string code, string name)
        {
            code = code.Replace(".", string.Empty).TrimStart('0').PadLeft(1, '0');
            return PumpCachedRow(cacheSubKESR, dsSubKESR.Tables[0], clsSubKESR, code,
                new object[] { "Code", code, "Name", name });
        }

        private void PumpFctChargesBudVologda00031388()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расходы бюджета - 00.03.13.88'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(00.03.13.88) Роспись_ФЭА.xlc", 
                    "РосписьЧек", "1", "ВариантРосписи", "2", "НачальнаяДата", startDate.ToString(), 
                    "КонечнаяДата", curDate.ToString(), "УведомленияЧек", "1", "Месяц", dateMonth, "Год", dateYear }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    string outcomesCode = row["КФСР"].ToString().Trim();
                    if (outcomesCode == string.Empty)
                        continue;
                    int refOutcomes = PumpOutcomesClsVologda(outcomesCode, row["КФСРИмя"].ToString().Trim());
                    int refEkr = PumpEkrVologda(row["КЭСР"].ToString().Trim(), row["КЭСРИмя"].ToString().Trim());
                    int refSubKesr = PumpSubKesrVologda(row["СубКЭСР"].ToString().Trim(), row["СубКЭСР_Изменено"].ToString().Trim());

                    decimal planeYear = Convert.ToDecimal(row["РоспКвартСуммаНаГод"].ToString().Trim().PadLeft(1, '0'));
                    if (planeYear == 0)
                        continue;
                    PumpRow(dsChargesBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlaneYear", planeYear, 
                        "RefR", refOutcomes, "RefEKR", refEkr, "RefSEKRBridge", refSubKesr});
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расходы бюджета - 00.03.13.88'", TraceMessageKind.Information);
        }

        private void PumpFctChargesBudVologda00031390()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расходы бюджета - 00.03.13.90'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string monthStr = CommonRoutines.MonthByNumber[(curDate / 100 % 100) - 1];
                string budgetDate = CommonRoutines.NewDateToLongDate(curDate.ToString());

                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(00.03.13.90) ПОФ по ПБС_ФЭА.xlc", 
                    "РосписьЧек", "1", "Знач_ВариантРосписи", "3", "УведомленияЧек", "1", 
                    "ОднаДата", budgetDate, "Месяц", monthStr, "Год", dateYear }, ref reportData))
                    return;

                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    string outcomesCode = row["КФСР"].ToString().Trim();
                    if (outcomesCode == string.Empty)
                        continue;
                    int refOutcomes = PumpOutcomesClsVologda(outcomesCode, row["КФСР_Изменено"].ToString().Trim());
                    int refEkr = PumpEkrVologda(row["КЭСР"].ToString().Trim(), row["КЭСР_Имя"].ToString().Trim());
                    int refSubKesr = PumpSubKesrVologda(row["СубКЭСР"].ToString().Trim(), row["СубКЭСР_Изменено"].ToString().Trim());

                    decimal planeMonth = Convert.ToDecimal(row["Росп" + monthStr].ToString().Trim().PadLeft(1, '0'));
                    if (planeMonth == 0)
                        continue;
                    PumpRow(dsChargesBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlaneMonth", planeMonth, 
                        "RefR", refOutcomes, "RefEKR", refEkr, "RefSEKRBridge", refSubKesr});
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расходы бюджета - 00.03.13.90'", TraceMessageKind.Information);
        }

        private void DoPumpFctChargesBudVologda01040199(string repStartDate, string repEndDate, string sumFieldName)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расходы бюджета - 01.04.01.99' - " + sumFieldName, TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(01.04.01.99) Расходы Казначейство Факт_ФЭА.xlc", 
                    "КазнРС", "", "УчетКазнЧек", "1", "НачальнаяДата", repStartDate, 
                    "КонечнаяДата", repEndDate, "УчетФинЧек", "1", "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1",  
                    "ТипСредствБюдж", "010000-010800,030000-039603,=060000", "ПричинаОтклонЧек", "1", "ТипДеятельности", "1", 
                    "ТипКлассификацииРасх", "0" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    string outcomesCode = row["КФСР"].ToString().Trim();
                    if (outcomesCode == string.Empty)
                        continue;
                    int refOutcomes = PumpOutcomesClsVologda(outcomesCode, row["КФСР_Изменено"].ToString().Trim());
                    int refEkr = PumpEkrVologda(row["КЭСР"].ToString().Trim(), row["КЭСР_Изменено"].ToString().Trim());
                    int refSubKesr = PumpSubKesrVologda(row["СубКЭСР"].ToString().Trim(), row["СубКЭСР_Изменено"].ToString().Trim());

                    decimal sum = Convert.ToDecimal(row["Расход"].ToString().Trim().PadLeft(1, '0'));
                    if (sum == 0)
                        continue;
                    PumpRow(dsChargesBud.Tables[0], new object[] { "RefYearDayUNV", curDate, sumFieldName, sum, 
                        "RefR", refOutcomes, "RefEKR", refEkr, "RefSEKRBridge", refSubKesr});
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расходы бюджета - 01.04.01.99' - " + sumFieldName, TraceMessageKind.Information);
        }

        private void PumpFctChargesBudVologda01040199()
        {
            DoPumpFctChargesBudVologda01040199(startDate.ToString(), curDate.ToString(), "FactYear");
            DoPumpFctChargesBudVologda01040199(curDate.ToString(), curDate.ToString(), "FactDay");
            string monthStartDate = (curDate / 100 * 100 + 1).ToString();
            DoPumpFctChargesBudVologda01040199(monthStartDate, curDate.ToString(), "FactMonth");
        }

        private void PumpFctChargesBudVologda()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расходы бюджета'", TraceMessageKind.Information);
            PumpFctChargesBudVologda00031388();
            PumpFctChargesBudVologda00031390();
            PumpFctChargesBudVologda01040199();

            string constr = string.Format("SOURCEID = {0} and RefYearDayUNV = {1}", this.SourceID, curDate);
            GroupTable(fctChargesBud, new string[] { "RefYearDayUNV", "RefR", "RefEKR", "RefSEKRBridge" },
                new string[] { "FactDay", "PlaneMonth", "FactMonth", "PlaneYear", "FactYear" }, constr);
            UpdateData();

            WriteToTrace("конец закачки - 'Факт.ФО_Расходы бюджета'", TraceMessageKind.Information);
        }

        #endregion fctChargesBud (f_F_ChargesBud)

        private void DeleteDataVologda()
        {
            DirectDeleteFactData(new IFactTable[] { fctEarningsBud }, -1, this.SourceID, string.Empty);
            string constr = string.Format("RefYearDayUNV = {0}", curDate);
            DirectDeleteFactData(new IFactTable[] { fctChargesBud }, -1, this.SourceID, constr);
        }

        private void PumpVologdaData(DirectoryInfo dir)
        {
            GetDateNovosib();
            DeleteDataVologda();
            budWP = new WorkplaceAutoObjectClass();
            try
            {
                string[] udlData = GetUdlData(dir);
                string dbName = string.Empty;
                string login = string.Empty;
                string password = string.Empty;
                GetUdlParams(udlData, ref dbName, ref login, ref password);
                budWP.Login(dbName, login, password);

                PumpFctEarningsBudVologda();
                PumpFctChargesBudVologda();
                UpdateData();
            }
            finally
            {
                Marshal.ReleaseComObject(budWP);
            }
        }


    }

}
