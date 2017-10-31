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
using Krista.FM.Server.DataPumps.FO99Pump;
using Krista.FM.ServerLibrary;
using System.Xml;
using Krista.FM.Common.Xml;

using WorkPlace;
using VariablesTools;
using System.Runtime.InteropServices;
using System.Web.Services.Protocols;

namespace Krista.FM.Server.DataPumps.FO35Pump
{

    public partial class FO35PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region классификаторы

        // Показатели.График финансирования кассовых выплат (d_Marks_ChartFin)
        private IDbDataAdapter daMarksChart;
        private DataSet dsMarksChart;
        private IClassifier clsMarksChart;
        private Dictionary<string, int> cacheMarksChart = null;

        #endregion классификаторы

        #region Факты

        // Факт.ФО_Расчет ожидаемого исполнения бюджета (f_F_CalcPerfBud)
        private IDbDataAdapter daCalcPerfBud;
        private DataSet dsCalcPerfBud;
        private IFactTable fctCalcPerfBud;
        // Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы (f_F_CalcPerfBR)
        private IDbDataAdapter daCalcPerfBR;
        private DataSet dsCalcPerfBR;
        private IFactTable fctCalcPerfBR;
        // Факт.ФО_График финансирования кассовых выплат (f_F_ChartFin)
        private IDbDataAdapter daChartFin;
        private DataSet dsChartFin;
        private IFactTable fctChartFin;

        #endregion Факты

        #endregion Поля

        #region Работа с базой и кэшами

        private const string F_F_CALC_PERF_B_GUID = "29518380-a1a5-449c-94a6-9ecb813f7b81";
        private const string F_F_CALC_PERF_BR_GUID = "7c9f00c1-d218-4ade-9d6e-63702b22302a";
        private const string F_F_CHART_FIN_GUID = "30ada33f-0b39-446f-902e-3e5786d57830";
        private const string D_MARKS_CHART_GUID = "febb660a-a4c7-4a6c-a55c-35ef0e7d7a11";
        private void InitDBObjectsOmsk()
        {
            clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];
            clsEkr = this.Scheme.Classifiers[D_EKR_GUID];
            clsMarksChart = this.Scheme.Classifiers[D_MARKS_CHART_GUID];

            fctFactFO35Outcomes = this.Scheme.FactTables[F_FO35_OUTCOMES_GUID];
            fctFactFO35 = this.Scheme.FactTables[F_FO35_GUID];
            fctCalcPerfBud = this.Scheme.FactTables[F_F_CALC_PERF_B_GUID];
            fctCalcPerfBR = this.Scheme.FactTables[F_F_CALC_PERF_BR_GUID];
            fctChartFin = this.Scheme.FactTables[F_F_CHART_FIN_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctFactFO35Outcomes, fctFactFO35, fctCalcPerfBud, fctCalcPerfBR, fctChartFin };
            this.CubeClassifiers = new IClassifier[] { clsKvsr, clsEkr, clsMarksChart };
        }

        private void QueryDataOmsk()
        {
            InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
            InitClsDataSet(ref daMarksChart, ref dsMarksChart, clsMarksChart);

            InitFactDataSet(ref daFactFO35Outcomes, ref dsFactFO35Outcomes, fctFactFO35Outcomes);
            InitFactDataSet(ref daFactFO35, ref dsFactFO35, fctFactFO35);
            InitFactDataSet(ref daCalcPerfBud, ref dsCalcPerfBud, fctCalcPerfBud);
            InitFactDataSet(ref daCalcPerfBR, ref dsCalcPerfBR, fctCalcPerfBR);
            InitFactDataSet(ref daChartFin, ref dsChartFin, fctChartFin);

            FillCachesOmsk();
        }

        private void FillCachesOmsk()
        {
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheMarksChart, dsMarksChart.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
        }

        private void UpdateDataOmsk()
        {
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daMarksChart, dsMarksChart, clsMarksChart);

            UpdateDataSet(daFactFO35Outcomes, dsFactFO35Outcomes, fctFactFO35Outcomes);
            UpdateDataSet(daFactFO35, dsFactFO35, fctFactFO35);
            UpdateDataSet(daCalcPerfBud, dsCalcPerfBud, fctCalcPerfBud);
            UpdateDataSet(daCalcPerfBR, dsCalcPerfBR, fctCalcPerfBR);
            UpdateDataSet(daChartFin, dsChartFin, fctChartFin);
        }

        private void PumpFinalizingOmsk()
        {
            ClearDataSet(ref dsFactFO35Outcomes);
            ClearDataSet(ref dsFactFO35);
            ClearDataSet(ref dsCalcPerfBud);
            ClearDataSet(ref dsCalcPerfBR);
            ClearDataSet(ref dsChartFin);

            ClearDataSet(ref dsKvsr);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsMarksChart);
        }

        #endregion Работа с базой и кэшами

        #region fctFactFO35Outcomes (f_F_ExpExctCachPl)

        private int PumpKvsrOmsk(DataRow row, string nameFieldName, string codeFieldName)
        {
            string kvsrCode = row[codeFieldName].ToString().Trim();
            if (kvsrCode == "0")
                return -1;
            string kvsrName = row[nameFieldName].ToString().Trim();
            if (kvsrName == string.Empty)
                kvsrName = constDefaultClsName;
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                new object[] { "Code", kvsrCode, "Name", kvsrName, "CodeLine", kvsrCode });
        }

        private int GetRefRCachPlOmsk(string kesr)
        {
            switch (kesr)
            {
                case "-1":
                    return 9;
                case "211":
                    return 1;
                case "213":
                    return 10;
                case "223":
                    return 3;
                case "251":
                    return 4;
                case "310":
                    return 12;
            }
            if (kesr.StartsWith("26"))
                return 11;
            else
                return 5;
        }

        private void PumpCachPlBudR00030100()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 00030190'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(00.03.01.90)Кассовые выплаты.xlc", 
                    "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1", "НачальнаяДата", startDate.ToString(), 
                    "КонечнаяДата", curDate.ToString(), "ТипКлассификации", "0",  "РасхТипКлсЧек", "1", "РСФО", "45" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    // итоговую строчку не качаем
                    if (i == rowsCount - 1)
                        continue;

                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal sum = Convert.ToDecimal(row["БухИтог"].ToString().PadLeft(1, '0'));
                    if (sum == 0)
                        continue;

                    string kesr = row["КЭСР_"].ToString().Trim();
                    if (kesr == "-1")
                    {
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "FactRep", sum, "RefMarks", 8 });
                    }

                    int refKvsr = PumpKvsrOmsk(row, "RowNote", "КВСР_");
                    if (refKvsr == -1)
                        continue;
                    int refRCachPl = GetRefRCachPlOmsk(kesr);
                    PumpRow(dsFactFO35Outcomes.Tables[0], new object[] { "RefYearDay", curDate, "RefKVSR", refKvsr, 
                        "FactRep", sum, "RefRCachPl", refRCachPl });
                    if (dsFactFO35Outcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateData();
                        ClearDataSet(daFactFO35Outcomes, ref dsFactFO35Outcomes);
                    }
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 00030190'", TraceMessageKind.Information);
        }

        private void PumpCachPlBudR02070101()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02070101'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(02.07.01.01)СБР (БА)_для закачки ФМ.XLC", 
                    "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1", "НачальнаяДата", startDate.ToString(), 
                    "КонечнаяДата", curDate.ToString(), "РосписьЧек", "1",  "ВариантРосписи", "0", "УведомленияЧек", "1" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                int refKvsr = -1;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal sum = Convert.ToDecimal(row["СБРНаПервыйГод"].ToString().PadLeft(1, '0'));
                    if (sum == 0)
                        continue;

                    if (i == rowsCount - 1)
                    {
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", sum, "RefMarks", 8 });
                        continue;
                    }

                    string kfsr = row["КФСР_"].ToString().Trim();
                    if (kfsr == "0")
                        refKvsr = PumpKvsrOmsk(row, "RowNote", "КВСР_");
                    if (refKvsr == -1)
                        continue;

                    string kesr = row["КЭСР_"].ToString().Trim();
                    if ((kesr == "-1") && (kfsr != "0"))
                        continue;
                    string budgAssCode = row["КодБюджАссигнования_"].ToString().Trim();
                    if (budgAssCode != string.Empty)
                        continue;

                    int refRCachPl = GetRefRCachPlOmsk(kesr);
                    PumpRow(dsFactFO35Outcomes.Tables[0], new object[] { "RefYearDay", curDate, "RefKVSR", refKvsr, 
                        "PlanRep", sum, "RefRCachPl", refRCachPl });
                    if (dsFactFO35Outcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
                    {
                        UpdateData();
                        ClearDataSet(daFactFO35Outcomes, ref dsFactFO35Outcomes);
                    }
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02070101'", TraceMessageKind.Information);
        }

        private void PumpFactFO35OutcomesOmsk()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы'", TraceMessageKind.Information);
            PumpCachPlBudR00030100();
            PumpCachPlBudR02070101();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы'", TraceMessageKind.Information);
        }

        #endregion fctFactFO35Outcomes (f_F_ExpExctCachPl)

        #region fctFactFO35 (f_F_ExctCachPl)

        #region 12250112

        private void PumpFactFO35Omsk12250112()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 12250112'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string budgetDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(12.25.01.12)Главная книга по ЭК ОМСК(КОСГУ 180 в СВР).xlc", 
                    "НачальнаяДата", budgetDate, "КонечнаяДата", budgetDate, 
                    "РСФО", "02522000010", "Баланс", "1.0 - Финансовый орган", "ВидДеятельности", "1 - Деятельность за счет средств бюджета", 
                    "Знач_РСФО", "45", "РСФОЧек", "0" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal factRep = 0;
                    decimal planRep = 0;
                    string accountNumber = row["ВычНомерСчета"].ToString().Trim();
                    if (accountNumber == "00000000000000000020211000")
                    {
                        factRep = Convert.ToDecimal(row["ДебетНаНачалоГода"].ToString().PadLeft(1, '0'));
                        planRep = Convert.ToDecimal(row["ДебетНаНачалоГода"].ToString().PadLeft(1, '0'));
                        if ((factRep != 0) && (planRep != 0))
                            PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", planRep, "FactRep", factRep, "RefMarks", 1 });
                    }
                    else if (accountNumber.EndsWith("550"))
                    {
                        factRep = Convert.ToDecimal(row["ОстатокКредит"].ToString().PadLeft(1, '0')) - Convert.ToDecimal(row["ОстатокДебет"].ToString().PadLeft(1, '0'));
                        if (factRep != 0)
                            PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", 0, "FactRep", factRep, "RefMarks", 37 });
                    }
                    else
                        continue;
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 12250112'", TraceMessageKind.Information);
        }

        #endregion 12250112

        #region 09020190

        private void PumpFactFO35Omsk09020113WithParams(string[] budParams, int refMarks)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 09020113'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, budParams, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                decimal sum = 0;
                DataRow row;
                if (refMarks == 3)
                {
                    for (int i = 0; i < rowsCount; i++)
                    {
                        row = reportData.Tables[0].Rows[i];
                        string incomeCode = row["Вид_"].ToString().Trim();
                        if ((incomeCode != "10000000") && (incomeCode != "20000000"))
                            continue;
                        sum = Convert.ToDecimal(row["КПСИзменениями"].ToString().PadLeft(1, '0'));
                        if (sum != 0)
                            PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", sum, "RefMarks", refMarks });
                    }
                }
                else
                {
                    for (int i = 0; i < rowsCount; i++)
                    {
                        row = reportData.Tables[0].Rows[i];
                        string meansType = row["ТипСредств"].ToString().Trim();
                        if (meansType != "11.00.00")
                            continue;
                        string incomeCode = row["Вид_"].ToString().Trim();
                        if (!incomeCode.StartsWith("1") && !incomeCode.StartsWith("2"))
                            continue;
                        sum = Convert.ToDecimal(row["КПСИзменениями"].ToString().PadLeft(1, '0'));
                        if (sum != 0)
                            PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", sum, "RefMarks", refMarks });
                    }
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 09020113'", TraceMessageKind.Information);
        }

        private void PumpFactFO35Omsk09020113()
        {
            string repDate = (curDate / 10000 * 10000).ToString();
            string repDateEnd = curDate.ToString();
            repDate = CommonRoutines.NewDateToLongDate(repDate);
            repDateEnd = CommonRoutines.NewDateToLongDate(repDateEnd);

            if (this.DataSource.Year >= 2012)
                PumpFactFO35Omsk09020113WithParams(new string[] { "Шаблон", "(09.02.01.13)Отчет об исполнении годового КП по доходам с ТС (динамический по видам доходов)_для закачки ФМ.xlc", 
                    "НачальнаяДата", repDate, "КонечнаяДата", repDateEnd, "РосписьЧек", "1", "Знач_ВариантРосписи", "0", "УведомленияЧек", "1", 
                    "ТипСредствБюдж", "11.00.00", "ТипСредствБюджЧек", "1" }, 5);
            else
                PumpFactFO35Omsk09020113WithParams(new string[] { "Шаблон", "(09.02.01.13)Отчет об исполнении годового КП по доходам с ТС (динамический по видам доходов)_для закачки ФМ.xlc", 
                    "НачальнаяДата", repDate, "КонечнаяДата", repDateEnd, "РосписьЧек", "1", "Знач_ВариантРосписи", "0", "УведомленияЧек", "1", 
                    "ТипСредствБюдж", "01.01.00", "ТипСредствБюджЧек", "1" }, 5);

            PumpFactFO35Omsk09020113WithParams(new string[] { "Шаблон", "(09.02.01.13)Отчет об исполнении годового КП по доходам с ТС (динамический по видам доходов)_для закачки ФМ.xlc", 
                    "НачальнаяДата", repDate, "КонечнаяДата", repDateEnd, "РосписьЧек", "1", "Знач_ВариантРосписи", "0", "УведомленияЧек", "1" }, 3);
        }

        #endregion 09020190

        #region 19020102

        private void PumpFactFO35Omsk19020102WithParams(string[] budParams)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 19020102'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(budParams, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refMarks = 0;
                    string finSourceCode = row["ИсточникВнутрФинансирования"].ToString().Trim();
                    if (finSourceCode.EndsWith("710"))
                        refMarks = 10;
                    else if (finSourceCode.EndsWith("810"))
                        refMarks = 11;
                    else if (finSourceCode.EndsWith("540"))
                        refMarks = 12;
                    else if (finSourceCode.EndsWith("640"))
                        refMarks = 13;
                    else if (finSourceCode.EndsWith("530"))
                        refMarks = 14;
                    else if (finSourceCode.EndsWith("630"))
                        refMarks = 15;
                    else
                        continue;
                    decimal sum = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                    if (sum != 0)
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", sum, "RefMarks", refMarks });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 19020102'", TraceMessageKind.Information);
        }

        private void PumpFactFO35Omsk19020102()
        {
            PumpFactFO35Omsk19020102WithParams(new string[] { "Шаблон", "(19.02.01.02)Исполнение СБР и КП по источникам.xlc", 
                    "ОднаДата", curDate.ToString() });
        }

        #endregion 19020102

        #region 00030200

        private void PumpFactFO35Omsk00030200WithParams(string[] budParams, bool isIncomes, int refMarks)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 00030290'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, budParams, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string incomeCode = row["БюджКлассификация"].ToString().Trim();
                    if (isIncomes)
                    {
                        // ссылка на показатели задается параметром, ограничение идет по типу средств
                        // качаем все, кроме пустой бюдж классификации
                        if (incomeCode == string.Empty)
                            continue;
                    }
                    else
                    {
                        if (incomeCode == string.Empty)
                            continue;
                        if (incomeCode.EndsWith("710"))
                            refMarks = 10;
                        else if (incomeCode.EndsWith("640"))
                            refMarks = 13;
                        else if (incomeCode.EndsWith("630"))
                            refMarks = 15;
                        else
                            continue;
                    }
                    decimal sum = Convert.ToDecimal(row["БухИтог"].ToString().PadLeft(1, '0'));
                    if (sum != 0)
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "FactRep", sum, "RefMarks", refMarks });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 00030290'", TraceMessageKind.Information);
        }

        private void PumpFactFO35Omsk00030200RefMarks3()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 00030290'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            string budgetStartDate = CommonRoutines.NewDateToLongDate(startDate.ToString());
            string budgetEndDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(00.03.02.90)Кассовые поступления_ФЭА.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ДохТипКлсЧек", "1", "ТипКлассификации", "2"}, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string incomeCode = row["БюджКлассификация"].ToString().Trim();
                    if (incomeCode == string.Empty)
                        continue;
                    if ((incomeCode.Split('.')[1] != "1") && (incomeCode.Split('.')[1] != "2"))
                        continue;
                    decimal sum = Convert.ToDecimal(row["БухИтог"].ToString().PadLeft(1, '0'));
                    if (sum != 0)
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "FactRep", sum, "RefMarks", 3 });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 00030290'", TraceMessageKind.Information);
        }

        private void PumpFactFO35Omsk00030200()
        {
            string budgetStartDate = CommonRoutines.NewDateToLongDate(startDate.ToString());
            string budgetEndDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
            if (this.DataSource.Year >= 2012)
                PumpFactFO35Omsk00030200WithParams(new string[] { "Шаблон", "(00.03.02.90)Кассовые поступления_ФЭА.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ДохТипКлсЧек", "1", "ТипКлассификации", "2", 
                    "ТипСредствБюдж", "11.00.00", "ТипСредствБюджЧек", "1" }, true, 5);
            else
                PumpFactFO35Omsk00030200WithParams(new string[] { "Шаблон", "(00.03.02.90)Кассовые поступления_ФЭА.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ДохТипКлсЧек", "1", "ТипКлассификации", "2", 
                    "ТипСредствБюдж", "01.01.00", "ТипСредствБюджЧек", "1" }, true, 5);

            PumpFactFO35Omsk00030200WithParams(new string[] { "Шаблон", "(00.03.02.90)Кассовые поступления_ФЭА.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ИстТипКлсЧек", "1", "ТипКлассификации", "1" }, false, 0);

            PumpFactFO35Omsk00030200RefMarks3();
        }

        #endregion 00030200

        #region 00030100

        private void PumpFactFO35Omsk00030100()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 00030191'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(00.03.01.91)Кассовые выплаты_ИФ.xlc", 
                    "НачальнаяДата", startDate.ToString(), "КонечнаяДата", curDate.ToString(), "ИстТипКлсЧек", "1", 
                    "ТипКлассификации", "1", "ДатаПринятияТипЧек", "1", "ТипДатыУнивер", "4" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refMarks = 0;
                    string finSourceCode = row["ИсточникВнутрФинансирования"].ToString().Trim();
                    if (finSourceCode == string.Empty)
                        continue;
                    if (finSourceCode.EndsWith("810"))
                        refMarks = 11;
                    else if (finSourceCode.EndsWith("540"))
                        refMarks = 12;
                    else if (finSourceCode.EndsWith("530"))
                        refMarks = 14;
                    else
                        continue;
                    decimal sum = Convert.ToDecimal(row["БухИтог"].ToString().PadLeft(1, '0'));
                    if ((refMarks == 11) || (refMarks == 12))
                        sum *= -1;
                    if (sum != 0)
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "FactRep", sum, "RefMarks", refMarks });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 00030191'", TraceMessageKind.Information);
        }

        #endregion 00030100

        private void PumpFactFO35Omsk()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Исполнение кассового плана'", TraceMessageKind.Information);
            PumpFactFO35Omsk12250112();
            PumpFactFO35Omsk09020113();
            PumpFactFO35Omsk19020102();
            PumpFactFO35Omsk00030200();
            PumpFactFO35Omsk00030100();
            WriteToTrace("конец закачки - 'Факт.ФО_Исполнение кассового плана'", TraceMessageKind.Information);
        }

        #endregion fctFactFO35 (f_F_ExctCachPl)

        #region fctCalcPerfBud

        private void PumpCalcPerfBudOmsk02070112()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 02070112'", TraceMessageKind.Information);

            string budgetStartDate = CommonRoutines.NewDateToLongDate(startDate.ToString());
            string budgetEndDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(02.07.01.12)Кассовый план по источникам_для закачки ФМ.XLC", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kif = row["_эки"].ToString().Trim();
                    if (kif == string.Empty)
                        continue;

                    int refMarks = -1;
                    string monthStr = CommonRoutines.MonthByNumber[Convert.ToInt32(dateMonth) - 1];
                    decimal monthPlan = Convert.ToDecimal(row["Росп" + monthStr].ToString().PadLeft(1, '0'));
                    if (monthPlan == 0)
                        continue;

                    switch (kif)
                    {
                        case "01601020000020000710":
                            refMarks = 35;
                            break;
                        case "01601030000020000710":
                            refMarks = 36;
                            break;
                        case "00701060100020000630":
                            refMarks = 37;
                            break;
                        case "01601020000020000810":
                            refMarks = 40;
                            break;
                        case "01601030000020000810":
                            refMarks = 41;
                            break;
                        case "01601060502020000640":
                            refMarks = 43;
                            break;
                        case "01601060501020000640":
                            refMarks = 44;
                            break;
                    }

                    if (refMarks != -1)
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", refMarks });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 02070112'", TraceMessageKind.Information);
        }

        private void PumpCalcPerfBudOmsk22010201()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 22010201'", TraceMessageKind.Information);

            string budgetStartDate = CommonRoutines.NewDateToLongDate(startDate.ToString());
            string budgetEndDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(22.01.02.01)Кассовый план доходов по видам доходов_для закачки ФМ.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, 
                    "РосписьЧек", "1", "Знач_ВариантРосписи", "0", "УведомленияЧек", "1", "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kd = row["Имя_КД"].ToString().Trim();
                    if (kd == string.Empty)
                        continue;

                    string monthStr = CommonRoutines.MonthByNumber[Convert.ToInt32(dateMonth) - 1];
                    decimal monthPlan = Convert.ToDecimal(row["ПланДоходов" + monthStr].ToString().PadLeft(1, '0'));
                    if (monthPlan == 0)
                        continue;

                    if (i == rowsCount - 1)
                    {
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 47 });
                        continue;
                    }

                    // закачиваем по типу средств
                    string meansType = row["ТипСредств"].ToString().Trim().Replace(".", string.Empty);
                    switch (meansType)
                    {
                        case "010401":
                            if (this.DataSource.Year < 2012)
                            {
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 31 });
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 32 });
                            }
                            break;
                        case "140001":
                            if (this.DataSource.Year >= 2012)
                            {
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 31 });
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 32 });
                            }
                            break;
                        case "010402":
                        case "010403":
                            if (this.DataSource.Year < 2012)
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 33 });
                            break;
                        case "130100":
                            if (this.DataSource.Year >= 2012)
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 31 }); 
                            break;
                    }
                    if (this.DataSource.Year < 2012)
                    {
                        if (meansType.StartsWith("0103"))
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 31 });
                    }
                    else
                    {
                        if (meansType.StartsWith("1300"))
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 31 });

                        int meansTypeInt = Convert.ToInt32(meansType.PadLeft(1, '0'));
                        if ((meansTypeInt >= 140002) && (meansTypeInt <= 140004))
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 33 });
                    }

                    // закачиваем по виду дохода - тип средств должен быть пустым
                    if (meansType != string.Empty)
                        continue;
                    string incomeKind = row["Вид_"].ToString().Trim();
                    if (incomeKind.StartsWith("1"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 1 });
                    if (incomeKind.StartsWith("10101"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 2 });
                    if (incomeKind.StartsWith("10102"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 3 });
                    if (incomeKind.StartsWith("10202"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 4 });
                    if (incomeKind.StartsWith("106"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 5 });
                    if (incomeKind.StartsWith("10601"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 6 });
                    if (incomeKind.StartsWith("10602"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 7 });
                    if (incomeKind.StartsWith("10604"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 8 });
                    if (incomeKind.StartsWith("10606"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 9 });
                    if (incomeKind.StartsWith("10302"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 10 });
                    if (incomeKind.StartsWith("10701"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 13 });
                    if (incomeKind.StartsWith("10704"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 51 });
                    if (incomeKind.StartsWith("105"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 14 });
                    if (incomeKind.StartsWith("10501"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 15 });
                    if (incomeKind.StartsWith("10502"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 16 });
                    if (incomeKind.StartsWith("10503"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 17 });
                    if (incomeKind.StartsWith("108"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 18 });
                    if (incomeKind.StartsWith("109"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 19 });
                    if (incomeKind.StartsWith("111"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 20 });
                    if (incomeKind.StartsWith("112"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 21 });
                    if (incomeKind.StartsWith("113"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 22 });
                    if (incomeKind.StartsWith("114"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 23 });
                    if (incomeKind.StartsWith("115"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 24 });
                    if (incomeKind.StartsWith("116"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 25 });
                    if (incomeKind.StartsWith("117"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 26 });
                    if (incomeKind.StartsWith("118"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 27 });
                    if (incomeKind.StartsWith("119"))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 28 });
                    if (incomeKind == "20201001")
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 29 });
                    if (incomeKind == "20201003")
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthPlan", monthPlan, "RefMarks", 30 });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 22010201'", TraceMessageKind.Information);
        }

        private void PumpCalcPerfBudOmsk19020102(int dateType)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 19020102'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                string repDate = curDate.ToString();
                if (dateType == 0)
                    repDate = curDate.ToString();
                if (dateType == 1)
                    repDate = (curDate / 10000 * 10000 + 101).ToString();
                if (dateType == 2)
                {
                    string prevMonth = dateMonth;
                    if (dateMonth != "01")
                        prevMonth = (Convert.ToInt32(dateMonth) - 1).ToString().PadLeft(2, '0');
                    repDate = string.Format("{0}{1}31", dateYear, prevMonth);
                }

                GetBudReportData(new string[] { "Шаблон", "(19.02.01.02)Исполнение СБР и КП по источникам.xlc", 
                    "ОднаДата", repDate, "РосписьЧек", "1", "ВариантРосписи", "0", "УведомленияЧек", "1" }, ref reportData);
                if (reportData.Tables.Count == 0)
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refMarks = -1;

                    decimal yearPlan = 0;
                    decimal monthFact = 0;
                    decimal primPlan = 0;
                    decimal yearFact = 0;

                    if (dateType == 1)
                        primPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));

                    string kif = row["ИсточникВнутрФинансирования"].ToString().Trim();
                    switch (kif)
                    {
                        case "016.01.02.00.00.02.0000.710":
                            refMarks = 35;
                            if (dateType == 0)
                            {
                                yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                                monthFact = Convert.ToDecimal(row["ПоступлениеЗаМес"].ToString().PadLeft(1, '0'));
                            }
                            if (dateType == 2)
                                yearFact = Convert.ToDecimal(row["ПоступлениеСНачГода"].ToString().PadLeft(1, '0'));
                            break;
                        case "016.01.03.00.00.02.0000.710":
                            refMarks = 36;
                            if (dateType == 0)
                            {
                                yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                                monthFact = Convert.ToDecimal(row["ПоступлениеЗаМес"].ToString().PadLeft(1, '0'));
                            }
                            if (dateType == 2)
                                yearFact = Convert.ToDecimal(row["ПоступлениеСНачГода"].ToString().PadLeft(1, '0'));
                            break;
                        case "007.01.06.01.00.02.0000.630":
                            refMarks = 37;
                            if (dateType == 0)
                            {
                                yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                                monthFact = Convert.ToDecimal(row["ПоступлениеЗаМес"].ToString().PadLeft(1, '0'));
                            }
                            if (dateType == 2)
                                yearFact = Convert.ToDecimal(row["ПоступлениеСНачГода"].ToString().PadLeft(1, '0'));
                            break;

                        case "016.01.02.00.00.02.0000.810":
                            refMarks = 40;
                            if (dateType == 0)
                            {
                                yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                                monthFact = Convert.ToDecimal(row["ВыбытиеЗаМес"].ToString().PadLeft(1, '0')) * (-1);
                            }
                            if (dateType == 2)
                                yearFact = Convert.ToDecimal(row["ВыбытиеСНачГода"].ToString().PadLeft(1, '0')) * (-1);
                            break;
                        case "016.01.03.00.00.02.0000.810":
                            refMarks = 41;
                            if (dateType == 0)
                            {
                                yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                                monthFact = Convert.ToDecimal(row["ВыбытиеЗаМес"].ToString().PadLeft(1, '0')) * (-1);
                            }
                            if (dateType == 2)
                                yearFact = Convert.ToDecimal(row["ВыбытиеСНачГода"].ToString().PadLeft(1, '0')) * (-1);
                            break;
                        case "016.01.06.05.02.02.0000.540":
                            refMarks = 49;
                            if (dateType == 0)
                            {
                                yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                                monthFact = Convert.ToDecimal(row["ВыбытиеЗаМес"].ToString().PadLeft(1, '0')) * (-1);
                            }
                            if (dateType == 2)
                                yearFact = Convert.ToDecimal(row["ВыбытиеСНачГода"].ToString().PadLeft(1, '0')) * (-1);
                            break;
                    }

                    if (kif.EndsWith("01.06.05.02.02.0000.640"))
                    {
                        refMarks = 43;
                        if (dateType == 0)
                        {
                            yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                            monthFact = Convert.ToDecimal(row["ПоступлениеЗаМес"].ToString().PadLeft(1, '0'));
                        }
                        if (dateType == 2)
                            yearFact = Convert.ToDecimal(row["ПоступлениеСНачГода"].ToString().PadLeft(1, '0'));
                    }
                    else if (kif.EndsWith("01.06.05.01.02.0000.640"))
                    {
                        refMarks = 44;
                        if (dateType == 0)
                        {
                            yearPlan = Convert.ToDecimal(row["КП"].ToString().PadLeft(1, '0'));
                            monthFact = Convert.ToDecimal(row["ПоступлениеЗаМес"].ToString().PadLeft(1, '0'));
                        }
                        if (dateType == 2)
                            yearFact = Convert.ToDecimal(row["ПоступлениеСНачГода"].ToString().PadLeft(1, '0'));
                    }

                    if ((yearPlan == 0) && (yearFact == 0) && (monthFact == 0) && (primPlan == 0))
                        continue;

                    if (refMarks != -1)
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", refMarks, 
                            "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact, "primPlan", primPlan });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 19020102'", TraceMessageKind.Information);
        }

        private void PumpCalcPerfBudOmsk09020113(int dateType)
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 09020113'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                string repDate = curDate.ToString();
                string repDateEnd = curDate.ToString();
                if (dateType == 0)
                {
                    repDate = (curDate / 10000 * 10000).ToString();
                    repDateEnd = curDate.ToString();
                }
                if (dateType == 1)
                {
                    repDate = (curDate / 10000 * 10000).ToString();
                    string prevMonth = dateMonth;
                    if (dateMonth != "01")
                        prevMonth = (Convert.ToInt32(dateMonth) - 1).ToString().PadLeft(2, '0');
                    repDateEnd = string.Format("{0}{1}31", dateYear, prevMonth);
                }
                if (dateType == 2)
                {
                    repDate = string.Format("{0}{1}01", dateYear, dateMonth);
                    repDateEnd = curDate.ToString();
                }

                repDate = CommonRoutines.NewDateToLongDate(repDate);
                repDateEnd = CommonRoutines.NewDateToLongDate(repDateEnd);
                if (!GetBudReportData(new string[] { "Шаблон", "(09.02.01.13)Отчет об исполнении годового КП по доходам с ТС (динамический по видам доходов)_для закачки ФМ.xlc", 
                    "НачальнаяДата", repDate, "КонечнаяДата", repDateEnd, 
                    "РосписьЧек", "1", "Знач_ВариантРосписи", "0", "УведомленияЧек", "1" }, ref reportData))
                    return;

                string curIncomeKind = string.Empty;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    decimal primPlan = 0;
                    decimal yearPlan = 0;
                    decimal yearFact = 0;
                    decimal monthFact = 0;
                    if (dateType == 0)
                    {
                        primPlan = Convert.ToDecimal(row["КПУтвержден"].ToString().PadLeft(1, '0'));
                        yearPlan = Convert.ToDecimal(row["КПСИзменениями"].ToString().PadLeft(1, '0'));
                    }
                    if (dateType == 1)
                        yearFact = Convert.ToDecimal(row["ДоходыСуммаЗаПериод"].ToString().PadLeft(1, '0'));
                    if (dateType == 2)
                        monthFact = Convert.ToDecimal(row["ДоходыСуммаЗаПериод"].ToString().PadLeft(1, '0'));

                    if ((primPlan == 0) && (yearPlan == 0) && (yearFact == 0) && (monthFact == 0))
                        continue;

                    if (i == rowsCount - 1)
                        continue;

                    string meansType = row["ТипСредств"].ToString().Trim();
                    switch (meansType)
                    {
                        case "10401":
                            if (this.DataSource.Year < 2012)
                            {
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 32, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 31, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                            }
                            break;
                        case "140001":
                            if (this.DataSource.Year >= 2012)
                            {
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 32, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 31, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                            }
                            break;
                        case "10402":
                        case "10403":
                            if (this.DataSource.Year < 2012)
                            {
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 33, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                            }
                            break;
                        case "130100":
                            if (this.DataSource.Year >= 2012)
                            {
                                PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 31, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                            }
                            break;
                    }

                    if (this.DataSource.Year < 2012)
                    {
                        if (meansType.StartsWith("103"))
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 31, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                    }
                    else
                    {
                        if (meansType.StartsWith("1300"))
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 31, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });

                        int meansTypeInt = Convert.ToInt32(meansType.PadLeft(1, '0'));
                        if ((meansTypeInt >= 140002) && (meansTypeInt <= 140004))
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 33, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                    }

                    if (meansType != string.Empty)
                        continue;
                    int refMarks = -1;
                    string el = row["Эл_"].ToString().Trim().TrimStart('0');
                    string incomeKind = row["Вид_"].ToString().Trim();

                    // чтобы суммы не дублировались - берем только первую запись по виду дохода
                    if (dateType == 0)
                    {
                        if (incomeKind != curIncomeKind)
                            curIncomeKind = incomeKind;
                        else
                            continue;
                    }

                    switch (incomeKind)
                    {
                        case "10000000":
                            // качается и 47 и 1
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 47, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                            refMarks = 1;
                            break;
                        case "20000000":
                            // качается только 47
                            PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", 47, 
                                "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                            continue;
                        case "10101000":
                            refMarks = 2;
                            break;
                        case "10102000":
                            refMarks = 3;
                            break;
                        case "10202000":
                            refMarks = 4;
                            break;
                        case "10600000":
                            refMarks = 5;
                            break;
                        case "10601000":
                            refMarks = 6;
                            break;
                        case "10602000":
                            refMarks = 7;
                            break;
                        case "10604000":
                            refMarks = 8;
                            break;
                        case "10606000":
                            refMarks = 9;
                            break;
                        case "10302000":
                            refMarks = 10;
                            break;
                        case "10701000":
                            refMarks = 13;
                            break;
                        case "10704000":
                            refMarks = 51;
                            break;
                        case "10500000":
                            refMarks = 14;
                            break;
                        case "10501000":
                            refMarks = 15;
                            break;
                        case "10502000":
                            refMarks = 16;
                            break;
                        case "10503000":
                            refMarks = 17;
                            break;
                        case "10800000":
                            refMarks = 18;
                            break;
                        case "10900000":
                            refMarks = 19;
                            break;
                        case "11100000":
                            refMarks = 20;
                            break;
                        case "11200000":
                            refMarks = 21;
                            break;
                        case "11300000":
                            refMarks = 22;
                            break;
                        case "11400000":
                            refMarks = 23;
                            break;
                        case "11500000":
                            refMarks = 24;
                            break;
                        case "11600000":
                            refMarks = 25;
                            break;
                        case "11700000":
                            refMarks = 26;
                            break;
                        case "11800000":
                            refMarks = 27;
                            break;
                        case "11900000":
                            refMarks = 28;
                            break;
                        case "20201001":
                            if (el == string.Empty)
                                refMarks = 29;
                            break;
                        case "20201003":
                            if (el == string.Empty)
                                refMarks = 30;
                            break;
                    }
                    if (refMarks != -1)
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", refMarks, 
                            "primPlan", primPlan, "yearPlan", yearPlan, "yearFact", yearFact, "monthFact", monthFact });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 09020113'", TraceMessageKind.Information);
        }

        // оригинальный отчет - 02 07 01 52
        private void PumpCalcPerfBOmsk02070190()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 02070190'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string prevMonth = dateMonth;
                if (dateMonth != "01")
                    prevMonth = (Convert.ToInt32(dateMonth) - 1).ToString().PadLeft(2, '0');
                string repDate = string.Format("{0}{1}31", dateYear, prevMonth);
                GetBudReportData(new string[] { "Шаблон", "(02.07.01.90)Сопоставление ОФ и расхода нарастающим.XLC", 
                    "НачальнаяДата", startDate.ToString(), "КонечнаяДата", repDate.ToString(), 
                    "РосписьЧек", "1",  "ВариантРосписи", "0", "УведомленияЧек", "1" }, ref reportData);
                if (reportData.Tables.Count == 0)
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    if (i != rowsCount - 1)
                        continue;
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal yearFact = Convert.ToDecimal(row["КПГод"].ToString().PadLeft(1, '0'));
                    PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "yearFact", yearFact, "RefMarks", 45 });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 02070190'", TraceMessageKind.Information);
        }

        #region 12250112

        private void PumpCalcPerfBudOmsk12250112()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 12250112'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string budgetDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
                int prevDateInt = CommonRoutines.DecrementDateWithLastDay(curDate);
                string prevBudgetDate = CommonRoutines.NewDateToLongDate(prevDateInt.ToString());

                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(12.25.01.12)Главная книга по ЭК ОМСК(КОСГУ 180 в СВР).xlc", 
                    "НачальнаяДата", prevBudgetDate, "КонечнаяДата", budgetDate, 
                    "РСФО", "02522000010", "Баланс", "1.0 - Финансовый орган", "ВидДеятельности", "1 - Деятельность за счет средств бюджета", 
                    "Знач_РСФО", "45", "РСФОЧек", "0" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string accountNumber = row["ВычНомерСчета"].ToString().Trim();
                    if (!accountNumber.EndsWith("550"))
                        continue;
                    decimal yearFact = Convert.ToDecimal(row["ОстатокПоДебетуНаНачалоПериода"].ToString().PadLeft(1, '0')) -
                                        Convert.ToDecimal(row["ОстатокПоКредитуНаНачалоПериода"].ToString().PadLeft(1, '0'));
                    decimal monthFact = Convert.ToDecimal(row["ОстатокДебет"].ToString().PadLeft(1, '0')) -
                                        Convert.ToDecimal(row["ОстатокКредит"].ToString().PadLeft(1, '0'));
                    if ((yearFact != 0) && (monthFact != 0))
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "yearFact", yearFact, "monthFact", monthFact, "RefMarks", 50 });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Ожидаемое исполнение бюджета - 12250112'", TraceMessageKind.Information);
        }

        #endregion 12250112

        private void PumpCalcPerfBudOmsk()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Ожидаемое исполнение бюджета'", TraceMessageKind.Information);
            PumpCalcPerfBudOmsk12250112();

            PumpCalcPerfBudOmsk02070112();

            PumpCalcPerfBudOmsk22010201();

            PumpCalcPerfBudOmsk19020102(0);
            PumpCalcPerfBudOmsk19020102(1);
            PumpCalcPerfBudOmsk19020102(2);

            PumpCalcPerfBudOmsk09020113(0);
            PumpCalcPerfBudOmsk09020113(1);
            PumpCalcPerfBudOmsk09020113(2);

            PumpCalcPerfBOmsk02070190(); // 02070152
            WriteToTrace("конец закачки - 'Факт.ФО_Ожидаемое исполнение бюджета'", TraceMessageKind.Information);
        }

        #endregion fctCalcPerfBud

        #region fctCalcPerfBR

        // оригинальный отчет - 02 07 01 52
        private void PumpCalcPerfBROmsk02070190()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 02070190'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(02.07.01.90)Сопоставление ОФ и расхода нарастающим.XLC", 
                    "НачальнаяДата", startDate.ToString(), "КонечнаяДата", curDate.ToString(), 
                    "РосписьЧек", "1",  "ВариантРосписи", "0", "УведомленияЧек", "1" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                int refKvsr = -1;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    decimal primPlan = Convert.ToDecimal(row["КПНач"].ToString().PadLeft(1, '0'));
                    decimal yearPlan = Convert.ToDecimal(row["КПУточн"].ToString().PadLeft(1, '0'));

                    if (i == rowsCount - 1)
                    {
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "PrimPlan", primPlan, 
                            "YearPlan", yearPlan, "RefMarks", 45 });
                    }

                    int refEkr = -1;
                    string name = row["RowNote"].ToString().Trim();
                    if (name == string.Empty)
                        continue;
                    string kvsrCode = row["КВСР_"].ToString().PadLeft(1, '0');
                    string ekrCode = row["КЭСР_"].ToString();
                    if (ekrCode == "-1")
                    {
                        refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                            new object[] { "Code", kvsrCode, "Name", name, "CodeLine", kvsrCode });
                    }
                    else
                    {
                        refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                            new object[] { "Code", ekrCode, "Name", name });
                    }
                    if (refEkr == -1)
                        continue;

                    if ((primPlan == 0) && (yearPlan == 0))
                        continue;
                    PumpRow(dsCalcPerfBR.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefKVSR", refKvsr, "RefEKR", refEkr,  
                        "primPlan", primPlan, "yearPlan", yearPlan });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 02070190'", TraceMessageKind.Information);
        }

        private void PumpCalcPerfBROmsk00020608()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 00020608'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string budgetStartDate = CommonRoutines.NewDateToLongDate(startMonthDate.ToString());
                string budgetEndDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(00.02.06.08)Исполнение графика финансирования_для закачки ФМ.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "РасхТипКлсЧек", "1", "ТипКлассификации", "0" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    decimal monthFact = Convert.ToDecimal(row["КазнФактПриходЗаПериод"].ToString().PadLeft(1, '0'));
                    if (monthFact == 0)
                        continue;

                    if (i == rowsCount - 1)
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "MonthFact", monthFact, "RefMarks", 45 });

                    string kvsrCode = row["КВСР"].ToString().Trim();
                    if (kvsrCode == string.Empty)
                        continue;
                    kvsrCode = kvsrCode.TrimStart('0').PadLeft(1, '0');
                    string ekrCode = row["КЭСР"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                        new object[] { "Code", kvsrCode, "Name", constDefaultClsName, "CodeLine", kvsrCode });
                    int refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                        new object[] { "Code", ekrCode, "Name", constDefaultClsName });
                    PumpRow(dsCalcPerfBR.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefKVSR", refKvsr, "RefEKR", refEkr, "MonthFact", monthFact });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 00020608'", TraceMessageKind.Information);
        }

        private void PumpCalcPerfBROmsk00020609()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 00020609'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string budgetStartDate = CommonRoutines.NewDateToLongDate(startMonthDate.ToString());
                string budgetEndDate = CommonRoutines.NewDateToLongDate(curDate.ToString());
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(00.02.06.09)Остатки графика финансирования кросс по датам графика_для закачки ФМ.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "РасхТипКлсЧек", "1", "ТипКлассификации", "0" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    decimal rest = Convert.ToDecimal(row["ВычОстаток"].ToString().PadLeft(1, '0'));
                    if (rest == 0)
                        continue;

                    if (i == rowsCount - 1)
                        PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "Rest", rest, "RefMarks", 45 });

                    string kvsrCode = row["КВСР_"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    string ekrCode = row["КЭСР_"].ToString().Trim();
                    if (ekrCode == string.Empty)
                        continue;
                    string subEkrCode = row["СубКЭСР_"].ToString().Trim();
                    if (subEkrCode != string.Empty)
                        continue;
                    int refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                        new object[] { "Code", kvsrCode, "Name", constDefaultClsName, "CodeLine", kvsrCode });
                    int refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                        new object[] { "Code", ekrCode, "Name", constDefaultClsName });
                    PumpRow(dsCalcPerfBR.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefKVSR", refKvsr, "RefEKR", refEkr, "Rest", rest });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 00020609'", TraceMessageKind.Information);
        }

        // оригинальный отчет - 02 07 01 52
        private void PumpCalcPerfBROmsk02070190Year()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 02070190 год'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string prevMonth = dateMonth;
                if (dateMonth != "01")
                    prevMonth = (Convert.ToInt32(dateMonth) - 1).ToString().PadLeft(2, '0');
                string repDate = string.Format("{0}{1}32", dateYear, prevMonth);
                GetBudReportData(new string[] { "Шаблон", "(02.07.01.90)Сопоставление ОФ и расхода нарастающим.XLC", 
                    "НачальнаяДата", startDate.ToString(), "КонечнаяДата", repDate, 
                    "РосписьЧек", "1",  "ВариантРосписи", "0", "УведомленияЧек", "1" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                int refKvsr = -1;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal yearFact = Convert.ToDecimal(row["КПГод"].ToString().PadLeft(1, '0'));
                    //if (i == rowsCount - 1)
                    //  {
                    //     PumpRow(dsCalcPerfBud.Tables[0], new object[] { "RefYearDayUNV", curDate, "PrimPlan", primPlan, 
                    //        "YearPlan", yearPlan, "YearFact", yearFact, "MonthFact", monthFact, "RefMarks", 45 });
                    //  }
                    int refEkr = -1;
                    string name = row["RowNote"].ToString().Trim();
                    if (name == string.Empty)
                        continue;
                    string kvsrCode = row["КВСР_"].ToString().PadLeft(1, '0');
                    string ekrCode = row["КЭСР_"].ToString();
                    if (ekrCode == "-1")
                    {
                        refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                            new object[] { "Code", kvsrCode, "Name", name, "CodeLine", kvsrCode });
                    }
                    else
                    {
                        refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                            new object[] { "Code", ekrCode, "Name", name });
                    }
                    if (refEkr == -1)
                        continue;
                    if (yearFact == 0)
                        continue;
                    PumpRow(dsCalcPerfBR.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefKVSR", refKvsr, "RefEKR", refEkr,  
                        "yearFact", yearFact });
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 02070190' год", TraceMessageKind.Information);
        }

        // оригинальный отчет - 00 02 03 01
        private void PumpCalcPerfBROmsk00020301()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 00020301'", TraceMessageKind.Information);
            string repDate = string.Format("{0}{1}01", dateYear, dateMonth);
            string repDateEnd = string.Format("{0}{1}31", dateYear, dateMonth);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(00.02.03.01)График финансирование (расходы, кросс)_Для закачки ФМ.xlc", 
                    "НачальнаяДата", repDate, "КонечнаяДата", repDateEnd, "РасхТипКлсЧек", "1", 
                    "ДатаНачалаПериода", repDate, "ДатаКонцаПериода", repDateEnd }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                int refKvsr = -1;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];

                    object[] mapping = null;
                    if (i == rowsCount - 1)
                    {
                        decimal sum = Convert.ToDecimal(row["Колонка_15"].ToString().PadLeft(1, '0'));
                        mapping = new object[] { "RefYearDayUNV", curDate, "MonthPlan", sum, "RefMarks", 45 };
                        PumpRow(dsCalcPerfBud.Tables[0], mapping);
                    }

                    int refEkr = -1;
                    string name = row["RowNote"].ToString().Trim();
                    if (name == string.Empty)
                        continue;
                    string kvsrCode = row["КВСР_"].ToString().PadLeft(1, '0');
                    if (kvsrCode == "-1")
                        continue;
                    string ekrCode = row["КЭСР_"].ToString();
                    if (ekrCode == "-1")
                    {
                        refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrCode,
                            new object[] { "Code", kvsrCode, "Name", name, "CodeLine", kvsrCode });
                    }
                    else
                    {
                        refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekrCode,
                            new object[] { "Code", ekrCode, "Name", name });
                    }
                    if (refEkr == -1)
                        continue;

                    decimal monthPlan = Convert.ToDecimal(row["Колонка_15"].ToString().PadLeft(1, '0'));
                    if (monthPlan == 0)
                        continue;
                    mapping = new object[] { "RefYearDayUNV", curDate, "RefKVSR", refKvsr, "RefEKR", refEkr, "MonthPlan", monthPlan };
                    PumpRow(dsCalcPerfBR.Tables[0], mapping);
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы - 00020301'", TraceMessageKind.Information);
        }

        private void PumpCalcPerfBROmsk()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы'", TraceMessageKind.Information);
            PumpCalcPerfBROmsk02070190(); // 02070152
            PumpCalcPerfBROmsk00020608();
            PumpCalcPerfBROmsk00020609();
            PumpCalcPerfBROmsk02070190Year(); // 02070152
            PumpCalcPerfBROmsk00020301(); // 00020301
            WriteToTrace("конец закачки - 'Факт.ФО_Расчет ожидаемого исполнения бюджета_Расходы'", TraceMessageKind.Information);
        }

        #endregion fctCalcPerfBR

        #region fctChartFin

        // оригинальный отчет - 00 02 08 05
        private void PumpChartFinOmsk00020890()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_График финансирования кассовых выплат - 00020890'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_OMSK, new string[] { "Шаблон", "(00.02.08.90)Аналитическая НОВАЯ С СБР И КП (график,ОФ).xlc", 
                    "ТипДатыУнивер", "4", "НачальнаяДата", startDate.ToString(), "КонечнаяДата", curDate.ToString(), 
                    "Год", dateYear, "Месяц", dateMonth, "РосписьЧек", "1", "ВариантРосписи", "0", "УведомленияЧек", "1", 
                    "ДатаКонцаПериода", curDate.ToString() }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string code = row["Кс"].ToString();
                    code = code.Substring(0, code.Length - 2);
                    string name = row["наименование"].ToString();
                    string key = string.Format("{0}|{1}", code, name);
                    int refMarks = PumpCachedRow(cacheMarksChart, dsMarksChart.Tables[0], clsMarksChart, key,
                        new object[] { "Code", code, "Name", name });

                    decimal sbr = Convert.ToDecimal(row["СБР_БА"].ToString().PadLeft(1, '0'));
                    decimal monthPlan = Convert.ToDecimal(row["Сумма0"].ToString().PadLeft(1, '0'));
                    decimal monthFact = Convert.ToDecimal(row["КазнФактПриходЗаПериод"].ToString().PadLeft(1, '0'));
                    decimal rest = Convert.ToDecimal(row["Остаток"].ToString().PadLeft(1, '0'));
                    if ((sbr == 0) && (monthPlan == 0) && (monthFact == 0) && (rest == 0))
                        continue;

                    object[] sumMapping = new object[] { };
                    for (int k = 1; k <= 31; k++)
                    {
                        string sumName = string.Format("D{0}", k);
                        string reportSumName = string.Format("Д{0}", k);
                        decimal sumValue = Convert.ToDecimal(row[reportSumName].ToString().PadLeft(1, '0'));
                        sumMapping = (object[])CommonRoutines.ConcatArrays(sumMapping, new object[] { sumName, sumValue });
                    }

                    object[] mapping = (object[])CommonRoutines.ConcatArrays(sumMapping,
                        new object[] { "RefYearDayUNV", curDate, "RefMarks", refMarks, "SBR", sbr, 
                        "MonthPlan", monthPlan, "MonthFact", monthFact, "Rest", rest });
                    PumpRow(dsChartFin.Tables[0], mapping);
                }
            }
            catch (Exception exc)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                    string.Format("ошибка при обработке данных отчета бюджета: {0} ({1})", exc.Message, exc.StackTrace));
                this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_График финансирования кассовых выплат - 00020890'", TraceMessageKind.Information);
        }

        private void PumpChartFinOmsk()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_График финансирования кассовых выплат'", TraceMessageKind.Information);
            PumpChartFinOmsk00020890(); // 00020805
            WriteToTrace("конец закачки - 'Факт.ФО_График финансирования кассовых выплат'", TraceMessageKind.Information);
        }

        #endregion fctChartFin

        private void DeleteDataOmsk()
        {
            string constr = string.Format("RefYearDay = {0}", curDate);
            DirectDeleteFactData(new IFactTable[] { fctFactFO35Outcomes }, -1, this.SourceID, constr);
            constr = string.Format("RefYearDayUNV = {0}", curDate);
            DirectDeleteFactData(new IFactTable[] { fctFactFO35, fctCalcPerfBud, fctCalcPerfBR, fctChartFin }, -1, this.SourceID, constr);
        }

        private void PumpOmskData(DirectoryInfo dir)
        {
            GetDateNovosib();

            DeleteDataOmsk();

            budWP = new WorkplaceAutoObjectClass();
            try
            {
                string[] udlData = GetUdlData(dir);
                string dbName = string.Empty;
                string login = string.Empty;
                string password = string.Empty;
                GetUdlParams(udlData, ref dbName, ref login, ref password);
                budWP.Login(dbName, login, password);

                PumpFactFO35OutcomesOmsk();
                PumpFactFO35Omsk();
                PumpCalcPerfBudOmsk();
                PumpCalcPerfBROmsk();
                PumpChartFinOmsk();

                UpdateData();
            }
            finally
            {
                Marshal.ReleaseComObject(budWP);
            }
        }

        #region обработка

        private delegate void AddIncomeSumsOmskDelegate(ref Dictionary<int, DataRow> factCache, ref DataSet ds);

        private void AddIncomeSumsFactFO35Omsk(ref Dictionary<int, DataRow> factCache, ref DataSet ds)
        {
            string[] sumFields = new string[] { "FactRep", "PlanRep" };
            // 6 = 3 - 5
            object[] sumMapping = GetCalcSums(sumFields, new int[] { 3, 5 }, new int[] { 1, -1 }, factCache);
            DataRow row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 6 }));
            factCache.Add(6, row);
            // 2 = 3 + 10 + 13 + 15
            sumMapping = GetCalcSums(sumFields, new int[] { 3, 10, 13, 15 }, new int[] { 1, 1, 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 2 }));
            factCache.Add(2, row);
            // 7 = 8 - 11 - 12 + 14
            sumMapping = GetCalcSums(sumFields, new int[] { 8, 11, 12, 14 }, new int[] { 1, -1, -1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 7 }));
            factCache.Add(7, row);
            // 16 = 1 + 2 -7 + 37
            sumMapping = GetCalcSums(sumFields, new int[] { 1, 2, 7, 37 }, new int[] { 1, 1, -1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 16 }));
            factCache.Add(16, row);
            // 9 = 10 + 13 + 15 + 11 + 12 - 14
            sumMapping = GetCalcSums(sumFields, new int[] { 10, 13, 15, 11, 12, 14 }, new int[] { 1, 1, 1, 1, 1, -1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 9 }));
            factCache.Add(9, row);
        }

        private void AddIncomeSumsCalcPerfBudOmsk(ref Dictionary<int, DataRow> factCache, ref DataSet ds)
        {
            string[] sumFields = new string[] { "SBR", "LBO", "PrimPlan", "YearPlan", "MonthPlan", "YearFact", "MonthFact", "Rest" };
            // 1 - берется по первому числу года
            string query = string.Format("select FactRep from f_F_ExctCachPl where sourceId = {0} and RefYearDayUNV = '{1}0101' and RefMarks = 1",
                                         this.SourceID, this.DataSource.Year);
            object factRep = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            DataRow row = PumpRow(ds.Tables[0],
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 38, "YearFact", factRep, "PrimPlan", factRep, "YearPlan", factRep });
            factCache.Add(38, row);
            // 34 = 35 + 36 + 37 + 38
            object[] sumMapping = GetCalcSums(sumFields, new int[] { 35, 36, 37, 38 }, new int[] { 1, 1, 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 34 }));
            factCache.Add(34, row);
            // 39 = 40 + 41 + 49
            sumMapping = GetCalcSums(sumFields, new int[] { 40, 41, 49 }, new int[] { 1, 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 39 }));
            factCache.Add(39, row);
            // 42 = 43 + 44
            sumMapping = GetCalcSums(sumFields, new int[] { 43, 44 }, new int[] { 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 42 }));
            factCache.Add(42, row);
            // 48 = 34 + 39 + 42
            sumMapping = GetCalcSums(sumFields, new int[] { 34, 39, 42 }, new int[] { 1, 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 48 }));
            factCache.Add(48, row);
            // 0 = 47 + 34 + 39 + 42 
            sumMapping = GetCalcSums(sumFields, new int[] { 47, 34, 39, 42 }, new int[] { 1, 1, 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 0 }));
            factCache.Add(0, row);
            // 46 = 0 - 45 + 50
            sumMapping = GetCalcSums(sumFields, new int[] { 0, 45, 50 }, new int[] { 1, -1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 46 }));
            factCache.Add(46, row);
        }

        private void AddAuxSumsOmsk(IFactTable fct, IDbDataAdapter da, DataSet ds, AddIncomeSumsOmskDelegate addIncOmskDelegate)
        {
            string constr = string.Format("SOURCEID = {0} and RefYearDayUNV = {1}", this.SourceID, curDate);
            InitDataSet(ref da, ref ds, fct, false, constr, string.Empty);
            Dictionary<int, DataRow> factCache = GetFactCacheSamara(ds.Tables[0]);
            try
            {
                addIncOmskDelegate(ref factCache, ref ds);
                UpdateDataSet(da, ds, fct);
            }
            finally
            {
                factCache.Clear();
            }
        }

        private void ProcessOmskData()
        {
            GetDateNovosib();

            string constr = string.Format("SOURCEID = {0} and RefYearDay = {1}", this.SourceID, curDate);
            GroupTable(fctFactFO35Outcomes, new string[] { "RefYearDay", "RefKVSR", "RefRCachPl" }, new string[] { "PlanRep", "FactRep" }, constr);

            constr = string.Format("SOURCEID = {0} and RefYearDayUNV = {1}", this.SourceID, curDate);
            GroupTable(fctFactFO35, new string[] { "RefYearDayUNV", "RefMarks" }, new string[] { "PlanRep", "FactRep" }, constr);

            GroupTable(fctCalcPerfBud, new string[] { "RefYearDayUNV", "RefMarks" },
                new string[] { "SBR", "LBO", "PrimPlan", "YearPlan", "MonthPlan", "YearFact", "MonthFact", "Rest", 
                    "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10", "D11", "D12", "D13", "D14", "D15", "D16",
                    "D17", "D18", "D19", "D20", "D21", "D22", "D23", "D24", "D25", "D26", "D27", "D28", "D29", "D30", "D31" }, constr);

            GroupTable(fctCalcPerfBR, new string[] { "RefYearDayUNV", "RefKVSR", "RefEKR" },
                new string[] { "SBR", "LBO", "PrimPlan", "YearPlan", "MonthPlan", "YearFact", "MonthFact", "Rest", 
                    "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10", "D11", "D12", "D13", "D14", "D15", "D16",
                    "D17", "D18", "D19", "D20", "D21", "D22", "D23", "D24", "D25", "D26", "D27", "D28", "D29", "D30", "D31" }, constr);

            GroupTable(fctChartFin, new string[] { "RefYearDayUNV", "RefMarks" },
                new string[] { "SBR", "MonthPlan", "MonthFact", "Rest", 
                    "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10", "D11", "D12", "D13", "D14", "D15", "D16",
                    "D17", "D18", "D19", "D20", "D21", "D22", "D23", "D24", "D25", "D26", "D27", "D28", "D29", "D30", "D31" }, constr);
            UpdateData();

            AddAuxSumsOmsk(fctFactFO35, daFactFO35, dsFactFO35, new AddIncomeSumsOmskDelegate(AddIncomeSumsFactFO35Omsk));
            UpdateData();
            AddAuxSumsOmsk(fctCalcPerfBud, daCalcPerfBud, dsCalcPerfBud, new AddIncomeSumsOmskDelegate(AddIncomeSumsCalcPerfBudOmsk));
            UpdateData();
        }

        #endregion обработка

        #region экспорт

        private void ExportData()
        {
            WriteToTrace("Начало экспорта данных", TraceMessageKind.Information);
            GetDateNovosib();
            FO99XmlExporter exporter = new FO99XmlExporter();
            try
            {
                string constrSource = string.Format(" SourceId = {0} ", this.SourceID);
                string deleteConstrFactFO35Outcomes = string.Format(" RefYearDay = {0} ", curDate);
                string constrFactFO35Outcomes = string.Format(" {0} and {1} ", deleteConstrFactFO35Outcomes, constrSource);

                string deleteConstrFactFO35Facts = string.Format(" RefYearDayUNV = {0} ", curDate);
                string constrFactFO35Facts = string.Format(" {0} and {1} ", deleteConstrFactFO35Facts, constrSource);
                string xmlStr = exporter.ExportDataToXml(new FO99XmlExporter.ExportedObject[] 
                    { 
                      new FO99XmlExporter.ExportedObject(clsKvsr, FmObjectsTypes.cls, null, constrSource, string.Empty), 
                      new FO99XmlExporter.ExportedObject(clsEkr, FmObjectsTypes.cls, null, constrSource, string.Empty), 
                      new FO99XmlExporter.ExportedObject(clsMarksChart, FmObjectsTypes.cls, null, constrSource, string.Empty), 

                      new FO99XmlExporter.ExportedObject(fctFactFO35, FmObjectsTypes.fct, null, constrFactFO35Facts, deleteConstrFactFO35Facts), 
                      new FO99XmlExporter.ExportedObject(fctFactFO35Outcomes, FmObjectsTypes.fct, null, constrFactFO35Outcomes, deleteConstrFactFO35Outcomes), 
                      new FO99XmlExporter.ExportedObject(fctCalcPerfBud, FmObjectsTypes.fct, null, constrFactFO35Facts, deleteConstrFactFO35Facts), 
                      new FO99XmlExporter.ExportedObject(fctCalcPerfBR, FmObjectsTypes.fct, null, constrFactFO35Facts, deleteConstrFactFO35Facts), 
                      new FO99XmlExporter.ExportedObject(fctChartFin, FmObjectsTypes.fct, null, constrFactFO35Facts, deleteConstrFactFO35Facts)
                    }, this.DataSource);

                string fileName = string.Format("{0}\\{1}\\ExportedData", this.RootDir, this.DataSource.Year);
                Directory.CreateDirectory(fileName);
                fileName += string.Format("\\ExportedData_{0}.xml", curDate);
                StreamWriter sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1251));
                sw.Write(xmlStr);
                sw.Close();

                // вызываем функцию веб сервиса
                DirectoryInfo dir = new DirectoryInfo(string.Format("{0}\\{1}", GetCurrentDir().FullName, "PumpRegionalParams"));
                FileInfo[] fi = dir.GetFiles("FO35PumpWeb.txt", SearchOption.TopDirectoryOnly);
                if (fi.GetLength(0) == 0)
                {
                    WriteToTrace("Не найден файл с параметрами веб сервиса", TraceMessageKind.Warning);
                    fi = dir.GetFiles("FO35PumpOmsk.txt", SearchOption.TopDirectoryOnly);
                    if (fi.GetLength(0) == 0)
                        WriteToTrace("Не найден файл с параметрами консоли, обращающейся к веб сервису", TraceMessageKind.Warning);
                    else
                    {
                        string fileText = File.ReadAllText(fi[0].FullName).Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
                        string consolePath = fileText.Split(';')[0].Trim().Split('=')[1];
                        string savingFileName = consolePath + Path.GetFileName(fileName);
                        WriteToTrace(string.Format("Сохранение файла: {0}", savingFileName), TraceMessageKind.Information);
                        sw = new StreamWriter(savingFileName, false, Encoding.GetEncoding(1251));
                        sw.Write(xmlStr);
                        sw.Close();

                    }
                }
                else
                {
                    string fileText = File.ReadAllText(fi[0].FullName).Trim().Replace("\r", string.Empty).Replace("\n", string.Empty);
                    string url = fileText.Split(';')[0].Trim().Split('=')[1];
                    string login = fileText.Split(';')[1].Trim().Split('=')[1];
                    string password = fileText.Split(';')[2].Trim().Split('=')[1];

                    using (PlaningServiceDataPumpWrapper webWrapper = new PlaningServiceDataPumpWrapper(url))
                    {
                        string err = string.Empty;
                        webWrapper.ConnectToScheme(login, password, ref err);
                        webWrapper.PumpExportedData(Path.GetFileName(fileName), xmlStr, ref err);
                    }
                }
            }
            finally
            {
                exporter = null;
            }
            WriteToTrace("Завершение экспорта данных", TraceMessageKind.Information);
        }

        #endregion экспорт

        #region расчет кубов 

        protected override void DirectProcessCube()
        {
            if (this.Region == RegionName.Omsk)
            {
                if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                {
                    foreach (int dataSourceID in this.PumpedSources.Keys)
                    {
                        SetDataSource(dataSourceID);
                        // экспорт и пересылка данных 
                        ExportData();
                    }
                }
            }
            // расчет кубов
            base.DirectProcessCube();
        }

        #endregion расчет кубов
    }


    [System.Web.Services.WebServiceBinding(Name = "PlaningServiceSoap", Namespace = "http://tempuri.org/")]
    public class PlaningServiceDataPumpWrapper : SoapHttpClientProtocol
    {
        
        public PlaningServiceDataPumpWrapper(string url)
        {
            CookieContainer = new System.Net.CookieContainer();
            Url = url;
            UseDefaultCredentials = true;
        }

        [SoapDocumentMethodAttribute("http://tempuri.org/ConnectToScheme", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool ConnectToScheme(string login, string pwd, ref string errStr)
        {
            object[] results = Invoke("ConnectToScheme", new object[] { login, pwd, errStr });
            return (bool)results[0];
        }

        [SoapDocumentMethodAttribute("http://tempuri.org/PumpExportedData", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void PumpExportedData(string fileName, string xmlData, ref string errStr)
        {
            Invoke("PumpExportedData", new object[] { fileName, xmlData, errStr });
        }
    }

}
