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

        #region общие функции получения данных бюджета

        private string[] GetUdlData(DirectoryInfo dir)
        {
            // берем только удл с годами в названии, удл - Budget.udl - для прямого подключения к базе бюджета
            // я ибу, хуета детектед, ололо нахуй
            FileInfo[] fi = dir.GetFiles("20*.udl");
            if (fi.GetLength(0) == 0)
                throw new Exception("в каталоге источника отсутствует UDl файл (параметры подключения к базе АС 'бюджет')");
            return CommonRoutines.GetTxtReportData(fi[0], CommonRoutines.GetTxtWinCodePage())[2].Split(';');
        }

        private void GetUdlParams(string[] udlData, ref string dbName, ref string login, ref string password)
        {
            foreach (string udlParam in udlData)
                if (udlParam.ToUpper().Contains("DATA SOURCE"))
                    dbName = udlParam.Split('=')[1].Trim();
                else if (udlParam.ToUpper().Contains("USER ID"))
                    login = udlParam.Split('=')[1].Trim();
                else if (udlParam.ToUpper().Contains("PASSWORD"))
                    password = udlParam.Split('=')[1].Trim();
            WriteToTrace(string.Format("информация о подключении: {0}, {1}, {2}", dbName, login, password), TraceMessageKind.Warning);
        }

        private VariablesClass GetBudVars(string[] budParams)
        {
            VariablesClass budVars = new VariablesClass();
            for (int i = 0; i <= budParams.GetLength(0) - 1; i += 2)
                budVars.set_Values(budParams[i], budParams[i + 1]);
            return budVars;
        }

        private bool GetBudReportData(string budScript, string[] budParams, ref DataSet reportData)
        {
            VariablesClass budVars = GetBudVars(budParams);
            VariablesClass budRepData = new VariablesClass();
            try
            {
                object outParams = null;
                object ExecException = null;
                TExecScriptResultX esr = budWP.ExecScript(budScript, budVars.SaveToVariant(), true, -1, ref outParams, ref ExecException);
                if (esr == TExecScriptResultX.esrxScriptNotFound)
                    ExecException = "Скрипт 'Построение отчета' не найден";
                if (esr != TExecScriptResultX.esrxSuccess)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("ошибка при получении данных отчета бюджета: {0}", ExecException.ToString()));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                    return false;
                }
                budRepData.LoadFromVariant(outParams);
                string xmlStr = budRepData.get_Values("Данные").ToString();
                StringReader strReader = new StringReader(xmlStr);
                reportData.ReadXml(strReader, XmlReadMode.ReadSchema);
                return true;
            }
            finally
            {
                Marshal.ReleaseComObject(budVars);
                Marshal.ReleaseComObject(budRepData);
            }
        }

        private bool GetBudReportData(string[] budParams, ref DataSet reportData)
        {
            return GetBudReportData(BUD_SCRIPT_OMSK, budParams, ref reportData);
        }

        #endregion общие функции получения данных бюджета

        #region CachPlBudR

        private int PumpKvsr02010101(DataRow row)
        {
            string kvsrCode = row["КВСР"].ToString().Trim();
            if (kvsrCode == string.Empty)
                return -1;
            kvsrCode = kvsrCode.TrimStart('0').PadLeft(1, '0');
            string kvsrKey = string.Format("{0}|{0}", kvsrCode);
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsrKey,
                new object[] { "Code", kvsrCode, "Name", kvsrCode });
        }

        private void PumpCachPlBudR02010101()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02010101'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(02.01.01.01) 2010 роспись организации.xlc", 
                    "ОднаДата", curDate.ToString(), "ТипСредствБюдж", "=010100,=010101", "ТипСредствБюджЧек", "1", 
                    "ВидПлана", "6.00", "ВидПланаЧек", "1", "ТипКлассификацииРасх", "0" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refKvsr = PumpKvsr02010101(row);
                    decimal sum = Convert.ToDecimal(row["СуммаЗаГод"].ToString().PadLeft(1, '0'));
                    if (sum == 0)
                        continue;
                    if (refKvsr == -1)
                        continue;
                    PumpRow(dsCachPlBudR.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefKVSR", refKvsr, "SBR", sum });
                }
                UpdateData();

                GetBudReportData(new string[] { "Шаблон", "(02.01.01.01) 2010 роспись организации.xlc", 
                    "ОднаДата", curDate.ToString(), "ТипСредствБюдж", "=010100,=010101", "ТипСредствБюджЧек", "1", 
                    "ВидПлана", "7.12", "ВидПланаЧек", "1", "ТипКлассификацииРасх", "0" }, ref reportData);
                rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refKvsr = PumpKvsr02010101(row);
                    decimal q1 = Convert.ToDecimal(row["СуммаКвартал1"].ToString().PadLeft(1, '0'));
                    decimal q2 = Convert.ToDecimal(row["СуммаКвартал2"].ToString().PadLeft(1, '0'));
                    decimal q3 = Convert.ToDecimal(row["СуммаКвартал3"].ToString().PadLeft(1, '0'));
                    decimal q4 = Convert.ToDecimal(row["СуммаКвартал4"].ToString().PadLeft(1, '0'));
                    decimal y1 = Convert.ToDecimal(row["СуммаЗаГод"].ToString().PadLeft(1, '0'));
                    if ((q1 == 0) && (q2 == 0) && (q3 == 0) && (q4 == 0) && (y1 == 0))
                        continue;

                    if (i == rowsCount - 1)
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 17,  
                           "Quart1", q1, "Quart2", q2, "Quart3", q3, "Quart4", q4 });

                    if (refKvsr == -1)
                        continue;
                    PumpRow(dsCachPlBudR.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefKVSR", refKvsr, "YearPlan", y1, 
                        "Quart1", q1, "Quart2", q2, "Quart3", q3, "Quart4", q4 });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02010101'", TraceMessageKind.Information);
        }

        private void PumpCachPlBudR02010138()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02010138'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string startDate = string.Format("{0}0101", this.DataSource.Year);
                GetBudReportData(new string[] { "Шаблон", "(02.01.01.38) 207, 208  помесячно.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(), "ТипСредствБюдж", "=010100,=010101", 
                    "ТипСредствБюджЧек", "1", "ВидПлана", "7.14", "ВидПланаЧек", "1" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refKvsr = PumpKvsr02010101(row);
                    decimal m1 = Convert.ToDecimal(row["РоспЯнварь"].ToString().PadLeft(1, '0'));
                    decimal m2 = Convert.ToDecimal(row["РоспФевраль"].ToString().PadLeft(1, '0'));
                    decimal m3 = Convert.ToDecimal(row["РоспМарт"].ToString().PadLeft(1, '0'));
                    decimal m4 = Convert.ToDecimal(row["РоспАпрель"].ToString().PadLeft(1, '0'));
                    decimal m5 = Convert.ToDecimal(row["РоспМай"].ToString().PadLeft(1, '0'));
                    decimal m6 = Convert.ToDecimal(row["РоспИюнь"].ToString().PadLeft(1, '0'));
                    decimal m7 = Convert.ToDecimal(row["РоспИюль"].ToString().PadLeft(1, '0'));
                    decimal m8 = Convert.ToDecimal(row["РоспАвгуст"].ToString().PadLeft(1, '0'));
                    decimal m9 = Convert.ToDecimal(row["РоспСентябрь"].ToString().PadLeft(1, '0'));
                    decimal m10 = Convert.ToDecimal(row["РоспОктябрь"].ToString().PadLeft(1, '0'));
                    decimal m11 = Convert.ToDecimal(row["РоспНоябрь"].ToString().PadLeft(1, '0'));
                    decimal m12 = Convert.ToDecimal(row["РоспДекабрь"].ToString().PadLeft(1, '0'));
                    if ((m1 == 0) && (m2 == 0) && (m3 == 0) && (m4 == 0) && (m5 == 0) && (m6 == 0) &&
                        (m7 == 0) && (m8 == 0) && (m9 == 0) && (m10 == 0) && (m11 == 0) && (m12 == 0))
                        continue;

                    if (i == rowsCount - 1)
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 17,
                            "Month1", m1, "Month2", m2, "Month3", m3, "Month4", m4, "Month5", m5, "Month6", m6, 
                            "Month7", m7, "Month8", m8, "Month9", m9, "Month10", m10, "Month11", m11, "Month12", m12 });

                    if (refKvsr == -1)
                        continue;
                    PumpRow(dsCachPlBudR.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefKVSR", refKvsr,
                        "Month1", m1, "Month2", m2, "Month3", m3, "Month4", m4, "Month5", m5, "Month6", m6, 
                        "Month7", m7, "Month8", m8, "Month9", m9, "Month10", m10, "Month11", m11, "Month12", m12 });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02010138'", TraceMessageKind.Information);
        }

        private void PumpCachPlBudR01020664()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 01020664'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                string startDate = string.Format("{0}0101", this.DataSource.Year);
                GetBudReportData(new string[] { "Шаблон", "(01.02.06.64) Отчет о поступлениях и выбытиях (казначейский) За период.xlc", 
                    "ТипДатыУнивер", "4", "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(), "ТипСредствБюдж", "=010100,=010101", 
                    "ТипСредствБюджЧек", "1", "ТипКлассификацииРасх", "0", 
                    "ДатаНачалаПериода", startDate, "ДатаКонцаПериода", curDate.ToString() }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refKvsr = PumpKvsr02010101(row);
                    decimal sum = Convert.ToDecimal(row["РасходСВозвратом"].ToString().PadLeft(1, '0'));
                    if (sum == 0)
                        continue;

                    if (i == rowsCount - 1)
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 17, "Fact", sum });

                    if (refKvsr == -1)
                        continue;
                    PumpRow(dsCachPlBudR.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefKVSR", refKvsr, "Fact", sum });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 01020664'", TraceMessageKind.Information);
        }

        private void PumpCachPlBudR02010198()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02010198'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(02.01.01.98) 2010 роспись организации_ФЭА.xlc", 
                    "ОднаДата", curDate.ToString(), "ТипСредствБюдж", "=010100,=010101", "ТипСредствБюджЧек", "1", 
                    "ВидПлана", "6.00", "ВидПланаЧек", "1", "ТипКлассификацииРасх", "0" }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    int refKvsr = PumpKvsr02010101(row);
                    decimal sbr = Convert.ToDecimal(row["РоспНаПервыйГод"].ToString().PadLeft(1, '0'));
                    if (sbr == 0)
                        continue;
                    if (i == rowsCount - 1)
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 17, "SBR", sbr });

                    if (refKvsr == -1)
                        continue;
                    PumpRow(dsCachPlBudR.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefKVSR", refKvsr, "SBR", sbr });
                }
                UpdateData();

                GetBudReportData(new string[] { "Шаблон", "(02.01.01.98) 2010 роспись организации_ФЭА.xlc", 
                    "ОднаДата", curDate.ToString(), "ТипСредствБюдж", "=010100,=010101", "ТипСредствБюджЧек", "1", 
                    "ВидПлана", "7.12", "ВидПланаЧек", "1", "ТипКлассификацииРасх", "0" }, ref reportData);
                rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    if (i == rowsCount - 1)
                    {
                        decimal sbr = Convert.ToDecimal(row["РоспНаПервыйГод"].ToString().PadLeft(1, '0'));
                        if (sbr != 0)
                            PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 17, "YearPlan", sbr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 02010198'", TraceMessageKind.Information);
        }

        private void PumpCachPlBudR()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы'", TraceMessageKind.Information);
            PumpCachPlBudR02010101();
            PumpCachPlBudR02010138();
            PumpCachPlBudR01020664();
            PumpCachPlBudR02010198();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы'", TraceMessageKind.Information);
        }

        #endregion CachPlBudR

        #region CachPlBud

        private void PumpCachPlBud09010105WithParams(string[] budParams)
        {
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(budParams, ref reportData);
                int count = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal sum = Convert.ToDecimal(row["ДоходыСНачалаГода"].ToString().PadLeft(1, '0'));
                    string kd = row["КодДохода"].ToString().Trim();
                    if (kd == string.Empty)
                        continue;
                    if (kd.Substring(3, 1) == "1")
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 4 });
                        continue;
                    }
                    if (kd.EndsWith("110"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 5 });
                        continue;
                    }
                    int kdPart = Convert.ToInt32(kd.Substring(3, 2));
                    if ((kdPart >= 11) && (kdPart < 20))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 6});
                        continue;
                    }
                    if (kd.Substring(3, 10) == "2020208602")
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 7 });
                        continue;
                    }
                    if ((kd.Substring(3, 10) == "2020100302") || (kd.Substring(3, 10) == "2020100802"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 8 });
                        continue;
                    }
                }
                UpdateData();
            }
            finally
            {
                reportData.Clear();
            }
        }

        private void PumpCachPlBud09010105()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 09010105'", TraceMessageKind.Information);
            string startDate = string.Format("{0}0101", this.DataSource.Year);
            PumpCachPlBud09010105WithParams(new string[] { "Шаблон", "(09.01.01.05)Доходы с группировкой по коду дохода_огурцова_C ИТОГАМИ.xlc", 
                    "ТипДатыУнивер", "4", "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(), 
                    "ТипСредствБюдж", "=010100,=010101", "ТипСредствБюджЧек", "1"});
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 09010105'", TraceMessageKind.Information);
        }

        private void PumpCachPlBud09100116WithParams(string[] budParams)
        {
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(budParams, ref reportData);
                int count = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal credit = Convert.ToDecimal(row["ДоходыКредитЗаПериод"].ToString().PadLeft(1, '0'));
                    decimal debit = Convert.ToDecimal(row["ДебетЗаПериод"].ToString().PadLeft(1, '0'));
                    if (i == count - 1)
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", credit - debit, "RefMarks", 10 });
                        continue;
                    }
                    string finSourceCode = row["ИсточникВнутрФинансирования"].ToString().Trim();
                    if (finSourceCode.EndsWith("01010000020000710"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", credit - debit, "RefMarks", 11 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01030000020000710"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", credit - debit, "RefMarks", 12 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01060501020000640"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", credit - debit, "RefMarks", 13 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01060502020000640"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", credit - debit, "RefMarks", 14 });
                        continue;
                    }
                    if (finSourceCode == "77701060600020000650")
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", credit - debit, "RefMarks", 9 });
                        continue;
                    }
                }
                UpdateData();
            }
            finally
            {
                reportData.Clear();
            }
        }

        private void PumpCachPlBud09100116()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 09100116'", TraceMessageKind.Information);
            string startDate = string.Format("{0}0101", this.DataSource.Year);
            PumpCachPlBud09100116WithParams(new string[] { "Шаблон", "(09.10.01.16) Заимствования из доходов.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString() });
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 09100116'", TraceMessageKind.Information);
        }

        private void PumpCachPlBud01020664WithParams(string[] budParams)
        {
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(budParams, ref reportData);
                if (reportData.Tables[0].Rows.Count == 0)
                    return;
                int count = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    decimal sum = Convert.ToDecimal(row["РасходСВозвратом"].ToString().PadLeft(1, '0'));
                    if (i == count - 1)
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 19 });
                        continue;
                    }
                    string finSourceCode = row["ИсточникВнутрФинансирования"].ToString().Trim();
                    if (finSourceCode.EndsWith("01050202020000520"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 20 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01010000020000810"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 21 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01030000020000810"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 22 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01060501020000540"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 23 });
                        continue;
                    }
                    if (finSourceCode.EndsWith("01060502020000540"))
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 24 });
                        continue;
                    }
                    if (finSourceCode == "77701060600020000550")
                    {
                        PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "Fact", sum, "RefMarks", 18 });
                        continue;
                    }
                }
                UpdateData();
            }
            finally
            {
                reportData.Clear();
            }
        }

        private void PumpCachPlBud01020664()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 01020664'", TraceMessageKind.Information);
            string startDate = string.Format("{0}0101", this.DataSource.Year);
            PumpCachPlBud01020664WithParams(new string[] { "Шаблон", "(01.02.06.64) Отчет о поступлениях и выбытиях (казначейский) За период.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(), "ТипДатыУнивер", "4", 
                    "ТипСредствБюдж", "=010100,=010101", "ТипСредствБюджЧек", "1", "ТипКлассификацииРасх", "1", 
                    "ДатаНачалаПериода", startDate, "ДатаКонцаПериода", curDate.ToString() });
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 01020664'", TraceMessageKind.Information);
        }

        private void PumpCachPlBud19010101WithParams(string[] budParams, int refMarks, string finSourceCode)
        {
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(budParams, ref reportData);
                if (reportData.Tables[0].Rows.Count == 0)
                    return;

                bool is600 = false;
                foreach (DataRow row in reportData.Tables[0].Rows)
                {
                    string rowNote = row["RowNote"].ToString().Trim().ToUpper();
                    if (rowNote != string.Empty)
                        is600 = (row["RowNote"].ToString().Trim() == "600");
                    decimal sum = Convert.ToDecimal(row["РоспНаПервыйГод"].ToString().PadLeft(1, '0'));
                    if (refMarks == 19)
                    {
                        if (rowNote == "ИТОГО ПО 6.00")
                            PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "SBR", sum, "RefMarks", refMarks });
                        else if (rowNote == "ИТОГО ПО 7.12")
                            PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "YearPlan", sum, "RefMarks", refMarks });
                    }
                    else
                    {
                        if (is600)
                            if (row["ИсточникВнутрФинансирования"].ToString().Trim() == finSourceCode)
                                PumpRow(dsCachPlBud.Tables[0], new object[] { "RefYearDayUNV", curDateMonth, "SBR", sum, "RefMarks", refMarks });
                    }
                }
                UpdateData();
            }
            finally
            {
                reportData.Clear();
            }
        }

        private void PumpCachPlBud19010101()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 19010101'", TraceMessageKind.Information);
            string startDate = string.Format("{0}0101", this.DataSource.Year);
            PumpCachPlBud19010101WithParams(new string[] { "Шаблон", "(19.01.01.01) 2010Источники - по виду плана.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(), 
                    "ГруппаПлановыхДокументов", "" }, 19, string.Empty);
            PumpCachPlBud19010101WithParams(new string[] { "Шаблон", "(19.01.01.01) 2010Источники - по виду плана.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(),
                    "ГруппаПлановыхДокументов", "2" }, 20, "77701050202020000520");
            PumpCachPlBud19010101WithParams(new string[] { "Шаблон", "(19.01.01.01) 2010Источники - по виду плана.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(), 
                    "ГруппаПлановыхДокументов", "2" }, 21, "77701010000020000810");
            PumpCachPlBud19010101WithParams(new string[] { "Шаблон", "(19.01.01.01) 2010Источники - по виду плана.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(),
                    "ГруппаПлановыхДокументов", "2" }, 22, "77701030000020000810");
            PumpCachPlBud19010101WithParams(new string[] { "Шаблон", "(19.01.01.01) 2010Источники - по виду плана.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(),
                    "ГруппаПлановыхДокументов", "2" }, 23, "77701060501020000540");
            PumpCachPlBud19010101WithParams(new string[] { "Шаблон", "(19.01.01.01) 2010Источники - по виду плана.xlc", 
                    "НачальнаяДата", startDate, "КонечнаяДата", curDate.ToString(),
                    "ГруппаПлановыхДокументов", "2" }, 24, "77701060502020000540");
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 19010101'", TraceMessageKind.Information);
        }

        private void PumpCachPlBud()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета'", TraceMessageKind.Information);
            PumpCachPlBud09010105();
            PumpCachPlBud09100116();
            PumpCachPlBud01020664();
            PumpCachPlBud19010101();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета'", TraceMessageKind.Information);
        }

        #endregion CachPlBud

        private void DeleteDataSamara()
        {
            string constr = string.Format("RefYearDayUNV = {0}", curDateMonth);
            DirectDeleteFactData(new IFactTable[] { fctCachPlBudR, fctCachPlBud }, -1, this.SourceID, constr);
        }

        private void PumpSamaraData(DirectoryInfo dir)
        {
//            curDate = CommonRoutines.ShortDateToNewDate(DateTime.Now.ToShortDateString());
            curDateMonth = string.Format("{0}{1}00", this.DataSource.Year, this.DataSource.Month.ToString().PadLeft(2, '0'));
            curDate = Convert.ToInt32(string.Format("{0}{1}31", this.DataSource.Year, this.DataSource.Month.ToString().PadLeft(2, '0')));
            DeleteDataSamara();

            budWP = new WorkplaceAutoObjectClass();
            try
            {
                string[] udlData = GetUdlData(dir);
                string dbName = string.Empty;
                string login = string.Empty;
                string password = string.Empty;
                GetUdlParams(udlData, ref dbName, ref login, ref password);
                budWP.Login(dbName, login, password);

                PumpCachPlBudR();
                PumpCachPlBud();
                UpdateData();
            }
            finally
            {
                Marshal.ReleaseComObject(budWP);
            }
        }

        #region обработка

        private object[] GetCalcSums(string[] sumFields, int[] refMarks, int[] mult, Dictionary<int, DataRow> factCache)
        {
            object[] calcSums = new object[] { };
            foreach (string sumField in sumFields)
            {
                decimal sum = 0;
                for (int i = 0; i < refMarks.GetLength(0); i++)
                {
                    if (!factCache.ContainsKey(refMarks[i]))
                        continue;
                    sum = sum + (mult[i] * Convert.ToDecimal(factCache[refMarks[i]][sumField].ToString().PadLeft(1, '0')));
                }
                calcSums = (object[])CommonRoutines.ConcatArrays(calcSums, new object[] { sumField, sum.ToString() });
            }
            return calcSums;
        }

        private Dictionary<int, DataRow> GetFactCacheSamara(DataTable dt)
        {
            Dictionary<int, DataRow> factCache = new Dictionary<int, DataRow>();
            foreach (DataRow row in dt.Rows)
            {
                int key = Convert.ToInt32(row["RefMarks"]);
                if (!factCache.ContainsKey(key))
                    factCache.Add(key, row);
            }
            return factCache;
        }

        private void AddIncomeSums(ref DataSet ds, ref Dictionary<int, DataRow> factCache)
        {
            // 15 (Иные источники финансирования дефицита бюджета)=10-11-12-13-14
            object[] sumMapping = GetCalcSums(sumFieldsSamara, new int[] { 10, 11, 12, 13, 14 },
                new int[] { 1, -1, -1, -1, -1 }, factCache);
            DataRow row = PumpRow(dsCachPlBud.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 15 }));
            factCache.Add(15, row);
            // 25 (Иные источники финансирования дефицита бюджета)=19-20-21-22-23-24
            sumMapping = GetCalcSums(sumFieldsSamara, new int[] { 19, 20, 21, 22, 23, 24 },
                new int[] { 1, -1, -1, -1, -1, -1 }, factCache);
            row = PumpRow(dsCachPlBud.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 25 }));
            factCache.Add(25, row);
            // 3 (Кассовые поступления – всего)=4+9+10
      /*      sumMapping = GetCalcSums(sumFieldsSamara, new int[] { 4, 9, 10 },
                new int[] { 1, 1, 1 }, factCache);
            row = PumpRow(dsCachPlBud.Tables[0],
                (object[])CommonRoutines.ConcatArrays(sumMapping, new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 3 }));
            factCache.Add(3, row);
            // 16 (Кассовые выплаты – всего)=17+18+19
            sumMapping = GetCalcSums(sumFieldsSamara, new int[] { 17, 18, 19 }, 
                new int[] { 1, 1, 1 }, factCache);
            row = PumpRow(dsCachPlBud.Tables[0],
                (object[])CommonRoutines.ConcatArrays(sumMapping, new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 16 }));
            factCache.Add(16, row);
            // 26 (Сальдо операций по поступлениям и выплатам)=3-16
            sumMapping = GetCalcSums(sumFieldsSamara, new int[] { 3, 16 },
                new int[] { 1, -1 }, factCache);
            row = PumpRow(dsCachPlBud.Tables[0],
                (object[])CommonRoutines.ConcatArrays(sumMapping, new object[] { "RefYearDayUNV", curDateMonth, "RefMarks", 26 }));
            factCache.Add(26, row);*/
        }

        private void AddAuxSumsSamara()
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            string constr = string.Format("SOURCEID = {0}", this.SourceID);
            InitDataSet(ref da, ref ds, fctCachPlBud, false, constr, string.Empty);
            Dictionary<int, DataRow> factCache = GetFactCacheSamara(ds.Tables[0]);
            try
            {
                // Расчет сумм поступлений и выбытий
                AddIncomeSums(ref ds, ref factCache);
                UpdateDataSet(da, ds, fctCachPlBud);
            }
            finally
            {
                factCache.Clear();
            }
        }

        private string[] sumFieldsSamara = new string[] { "Quart1", "Quart2", "Quart3", "Quart4", "Month1", "Month2", "Month3", "Month4", "Month5", 
                        "Month6", "Month7", "Month8", "Month9", "Month10", "Month11", "Month12", "YearPlan", "SBR", "Fact"};
        private void ProcessSamaraData()
        {
            string constr = string.Format("SOURCEID = {0}", this.SourceID);
            GroupTable(fctCachPlBudR, new string[] { "RefYearDayUnv", "RefKVSR" }, sumFieldsSamara, constr);
            GroupTable(fctCachPlBud, new string[] { "RefYearDayUnv", "RefMarks" }, sumFieldsSamara, constr);
            UpdateData();

            AddAuxSumsSamara();
            UpdateData();
        }

        #endregion обработка

    }

}
