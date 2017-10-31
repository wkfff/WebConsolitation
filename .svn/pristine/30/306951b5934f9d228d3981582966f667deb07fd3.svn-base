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

namespace Krista.FM.Server.DataPumps.FO42Pump
{
    public class FO42PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.Оценка качества ФМ_Исходные данные (d_Marks_QualityFMData)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        // Администратор.Анализ (d_KVSR_Analysis)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<string, int> cacheKvsr = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФО_Оценка качества ФМ_Исходные данные (f_Marks_FOQualityFMData)
        private IDbDataAdapter daFMQuality;
        private DataSet dsFMQuality;
        private IFactTable fctFMQuality;

        #endregion Факты

        WorkplaceAutoObjectClass budWP = null;
        private const string BUD_SCRIPT_UNV = "Обмен данными\\УниверсальноеПостроениеОтчета.abl";
        private int curDate = 0;
        private int startDate = 0;
        private int endDate = 0;
        private string budgetEndDate = string.Empty;
        private string budgetStartDate = string.Empty;
        private string budgetEndDate31 = string.Empty;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            int sourceIdKvsr = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            string constr = string.Format("SOURCEID = {0}", sourceIdKvsr);
            InitDataSet(ref daKvsr, ref dsKvsr, clsKvsr, false, constr, string.Empty);
            InitDataSet(ref daMarks, ref dsMarks, clsMarks, false, string.Empty, string.Empty);
            InitFactDataSet(ref daFMQuality, ref dsFMQuality, fctFMQuality);
            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "Symbol", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daFMQuality, dsFMQuality, fctFMQuality);
        }

        private const string D_KVSR_GUID = "bb3d9a88-8088-49ba-abcd-85c53e29ca57";
        private const string D_MARKS_GUID = "98102074-ae96-412d-9cda-63abc4297f83";
        private const string F_FM_QUALITY_GUID = "5fac684c-5434-467c-a858-ec2b957484cb";
        protected override void InitDBObjects()
        {
            this.UsedFacts = new IFactTable[] { };
            this.UsedClassifiers = new IClassifier[] { };
            clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];
            clsMarks = this.Scheme.Classifiers[D_MARKS_GUID];
            fctFMQuality = this.Scheme.FactTables[F_FM_QUALITY_GUID];
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFMQuality);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsKvsr);
        }

        #endregion Работа с базой и кэшами

        #region общие функции получения данных бюджета

        private string[] GetUdlData(DirectoryInfo dir)
        {
            FileInfo[] fi = dir.GetFiles("*.udl");
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

        #endregion общие функции получения данных бюджета

        #region закачка FMQuality

        private void PumpFMQuality02070192()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02070192'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(02.07.01.92) Сопоставление ОФ и расхода по месяцам_для закачки ФМ.xlc", 
                    "ОднаДата", budgetEndDate31,
                    "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1", "ТипСредствБюдж", "01.01.00" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrName = row["Квср"].ToString().Trim();
                    if (kvsrName == string.Empty)
                        continue;
                    string kvsrCode = row["Квср_"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P2_A", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["ПофГод"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }

                    refMarks = FindCachedRow(cacheMarks, "P2_B", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["КпГод"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }

                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02070192'", TraceMessageKind.Information);
        }

        private void PumpFMQuality02060180()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02060180'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(02.06.01.80) P9_A.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate31, "КазнРС", string.Empty, 
                    "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["КазнФактПриходСВозвратомЗаПериод"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P9_A", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["Количество"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }

                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02060180'", TraceMessageKind.Information);
        }

        private void PumpFMQuality25010190()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 25010190'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(25.01.01.90) p13_b.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ТипДок", "7.07.0" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["Квср"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P13_B", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["Колво"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 25010190'", TraceMessageKind.Information);
        }

        #region 02070199

        private void PumpFMQuality02070199WithParams(string kesr, string marksValue, string valueFieldName)
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02070199'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(02.07.01.99)СБР (БА).xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, 
                    "КЭСР", kesr }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["Квср_"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, marksValue, -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row[valueFieldName].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02070199'", TraceMessageKind.Information);
        }

        private void PumpFMQuality02070199()
        {
            PumpFMQuality02070199WithParams("2.1.1", "P3_B1", "Сбр211");
            PumpFMQuality02070199WithParams("2.1.3", "P3_B2", "Сбр213");
            PumpFMQuality02070199WithParams("2.9.0", "P3_B3", "Сбр290");
            PumpFMQuality02070199WithParams("3.1.0", "P3_B4", "Сбр310");
        }

        #endregion 02070199

        private void PumpFMQuality03020190()
        {
            if (this.DataSource.Year < 2011)
                return;

            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 03020190'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(03.02.01.90) Р11_А_для закачки ФМ.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ЛицСчет", "016.01.999.0", 
                    "КазнРС", "82", "ЛицСчетЧек", "1", "ТипДатыУнивер", "4", "ДатаПринятияТипЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["КвсрОрг"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P11_A", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["Количество"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 03020190'", TraceMessageKind.Information);
        }

        private void PumpFMQuality03020180()
        {
            if (this.DataSource.Year < 2011)
                return;

            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 03020180'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(03.02.01.80) Р12_А_для закачки ФМ.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ЛицСчет", "016.01.999.0", 
                    "КазнРС", "25690501", "ЛицСчетЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["Квср"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P12_A", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["Колво"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 03020180'", TraceMessageKind.Information);
        }

        private void PumpFMQuality09030180()
        {
            if (this.DataSource.Year < 2011)
                return;

            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 09030180'", TraceMessageKind.Information);

            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(09.03.01.80) Р12_B.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ЛицСчет", "016.01.999.0", 
                    "КазнРС", "25690501", "ЛицСчетЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["Квср"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P12_B", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["Колво"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 09030180'", TraceMessageKind.Information);
        }

        private void PumpFMQuality09030190()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 09030190'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(09.03.01.90) p11_b.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate31, 
                    "КазнРС", "82" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["Квср"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P11_B", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["p11_b"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 09030190'", TraceMessageKind.Information);
        }

        private void PumpFMQuality02060191()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02060191'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(02.06.01.91) Распределение КП по ПБС.xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate, "ТипСредствБюдж", "01.01.00", "ТипСредствБюджЧек", "1" }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["РоспНаПервыйГод"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    decimal value = 0;
                    int refMarks = FindCachedRow(cacheMarks, "P8_A", -1);
                    if (refMarks != -1)
                    {
                        value = Convert.ToDecimal(row["Количество"].ToString().PadLeft(1, '0'));
                        if (value != 0)
                            PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02060191'", TraceMessageKind.Information);
        }

        #region 02060199

        private void PumpFMQuality02060199Row(int refKvsr, string marksValue, DataRow row, string valueFieldName)
        {
            int refMarks = FindCachedRow(cacheMarks, marksValue, -1);
            if (refMarks == -1)
                return;

            decimal value = Convert.ToDecimal(row[valueFieldName].ToString().PadLeft(1, '0'));
            if (value != 0)
                PumpRow(dsFMQuality.Tables[0], new object[] { "RefYearDayUNV", curDate, "Value", value, "RefMarksData", refMarks, "RefKvsr", refKvsr });
        }

        private void PumpFMQuality02060199()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02060199'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                if (!GetBudReportData(BUD_SCRIPT_UNV, new string[] { "Шаблон", "(02.06.01.99)Журнал изменений БА БР(Ф 524).xlc", 
                    "НачальнаяДата", budgetStartDate, "КонечнаяДата", budgetEndDate }, ref reportData))
                    return;
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string kvsrCode = row["Квср"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                    int refKvsr = FindCachedRow(cacheKvsr, kvsrCode, -1);
                    if (refKvsr == -1)
                        continue;

                    PumpFMQuality02060199Row(refKvsr, "P3_A1", row, "РоспНаПервыйГод211");
                    PumpFMQuality02060199Row(refKvsr, "P3_A2", row, "РоспНаПервыйГод213");
                    PumpFMQuality02060199Row(refKvsr, "P3_A3", row, "РоспНаПервыйГод290");
                    PumpFMQuality02060199Row(refKvsr, "P3_A4", row, "РоспНаПервыйГод310");

                    PumpFMQuality02060199Row(refKvsr, "P4_A1", row, "РоспНаПервыйГод211Кол");
                    PumpFMQuality02060199Row(refKvsr, "P4_A2", row, "РоспНаПервыйГод213Кол");
                    PumpFMQuality02060199Row(refKvsr, "P4_A3", row, "РоспНаПервыйГод290Кол");
                    PumpFMQuality02060199Row(refKvsr, "P4_A4", row, "РоспНаПервыйГод310Кол");

                    PumpFMQuality02060199Row(refKvsr, "P4_B1", row, "СБР211Кол");
                    PumpFMQuality02060199Row(refKvsr, "P4_B2", row, "СБР213Кол");
                    PumpFMQuality02060199Row(refKvsr, "P4_B3", row, "СБР290Кол");
                    PumpFMQuality02060199Row(refKvsr, "P4_B4", row, "СБР310Кол");
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные - 02060199'", TraceMessageKind.Information);
        }

        #endregion 02060199

        private void PumpFMQuality()
        {
            WriteToTrace("начало закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные'", TraceMessageKind.Information);
            PumpFMQuality02070192();
            PumpFMQuality02060180(); 
            PumpFMQuality25010190();
            PumpFMQuality02070199();
            PumpFMQuality03020190();
            PumpFMQuality03020180();
            PumpFMQuality09030180();
            PumpFMQuality09030190();
            PumpFMQuality02060191();
            PumpFMQuality02060199();
            WriteToTrace("конец закачки - 'Показатели.ФО_Оценка качества ФМ_Исходные данные'", TraceMessageKind.Information);
        }

        #endregion закачка FMQuality

        #region Перекрытые методы закачки

        protected override void DeleteEarlierPumpedData()
        {
            endDate = this.DataSource.Year * 10000 + 1232;
            budgetEndDate = CommonRoutines.NewDateToLongDate(endDate.ToString());
            budgetEndDate31 = CommonRoutines.NewDateToLongDate((endDate - 1).ToString());
            startDate = this.DataSource.Year * 10000 + 101;
            budgetStartDate = CommonRoutines.NewDateToLongDate(startDate.ToString());

            curDate = this.DataSource.Year * 10000 + 1;
            string constr = string.Format(" RefYearDayUNV = {0} and PumpId > 0 ", curDate);
            DirectDeleteFactData(new IFactTable[] { fctFMQuality }, -1, this.SourceID, constr);
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {

            budWP = new WorkplaceAutoObjectClass();
            try
            {
                string[] udlData = GetUdlData(dir);
                string dbName = string.Empty;
                string login = string.Empty;
                string password = string.Empty;
                GetUdlParams(udlData, ref dbName, ref login, ref password);
                budWP.Login(dbName, login, password);

                PumpFMQuality();
                UpdateData();
            }
            finally
            {
                Marshal.ReleaseComObject(budWP);
            }
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
