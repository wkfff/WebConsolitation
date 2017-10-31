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

        #region Факты

        // Факт.ФО_Кассовые выплаты (f_F_FOPayment)
        private IDbDataAdapter daFOPayment;
        private DataSet dsFOPayment;
        private IFactTable fctFOPayment;

        #endregion Факты

        // директория с данными для закачки
        string dataDir = string.Empty;
        int yanaoReportDate = 0;
        int totalKvsrRef = 0;

        #endregion Поля

        #region Работа с базой и кэшами

        private const string F_FO_PAYMENT_GUID = "25e7635b-356c-4db7-9a09-31daed1d6f20";
        private void InitDBObjectsYanao()
        {
            clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];
            clsEkr = this.Scheme.Classifiers[D_EKR_GUID];
            clsOutcomesCls = this.Scheme.Classifiers[D_OUTCOMES_CLS_GUID];

            fctFactFO35Outcomes = this.Scheme.FactTables[F_FO35_OUTCOMES_GUID];
            fctFactFO35 = this.Scheme.FactTables[F_FO35_GUID];
            fctFOPayment = this.Scheme.FactTables[F_FO_PAYMENT_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] {  };
            this.CubeClassifiers = new IClassifier[] { clsKvsr, clsEkr, clsOutcomesCls };
        }

        private void QueryDataYanao()
        {
            InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomesCls);

            InitFactDataSet(ref daFactFO35Outcomes, ref dsFactFO35Outcomes, fctFactFO35Outcomes);
            InitFactDataSet(ref daFactFO35, ref dsFactFO35, fctFactFO35);
            InitFactDataSet(ref daFOPayment, ref dsFOPayment, fctFOPayment);

            FillCachesYanao();
        }

        private void FillCachesYanao()
        {
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheOutcomesCls, dsOutcomesCls.Tables[0], "Code", "Id");
        }

        private void UpdateDataYanao()
        {
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomesCls);

            UpdateDataSet(daFactFO35Outcomes, dsFactFO35Outcomes, fctFactFO35Outcomes);
            UpdateDataSet(daFactFO35, dsFactFO35, fctFactFO35);
            UpdateDataSet(daFOPayment, dsFOPayment, fctFOPayment);
        }

        private void PumpFinalizingYanao()
        {
            ClearDataSet(ref dsFactFO35Outcomes);
            ClearDataSet(ref dsFactFO35);
            ClearDataSet(ref dsFOPayment);

            ClearDataSet(ref dsKvsr);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsOutcomesCls);
        }

        #endregion Работа с базой и кэшами

        #region 01090305

        private int GetRefMarks01090305(string name)
        {
            switch (name.Trim().ToLower())
            {
                case "доходы всего":
                    return 3;
                case "безвоздмездные поступления":
                    return 6;
                case "собственные доходы":
                    return 5;
                case "поступления по источникам финансирования":
                    return 9;
                case "привлечение заемных средств":
                    return 10;
                case "возврат кредитов":
                    return 13;
                case "средства продажи акций и иных форм участия в капитале":
                    return 15;
            }
            return -1;
        }

        private void PumpYanao01090305()
        {
            WriteToTrace("начало закачки - 01090305'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            string searchPattern = string.Format("*{0}_(01.09.03.05) Исполнение кассового плана по поступлениям.xml", this.DataSource.Year);
            try
            {
                FileInfo[] fi = new DirectoryInfo(dataDir).GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
                string fileDate = fi[0].Name.Split('_')[0];
                yanaoReportDate = Convert.ToInt32(string.Format("{0}{1}{2}", fileDate.Split('.')[2],
                    fileDate.Split('.')[1].PadLeft(2, '0'), fileDate.Split('.')[0].PadLeft(2, '0')));
                string constr = string.Format("RefYearDayUNV = {0}", yanaoReportDate);
                DirectDeleteFactData(new IFactTable[] { fctFactFO35 }, -1, this.SourceID, constr);

                reportData.ReadXml(fi[0].FullName, XmlReadMode.ReadSchema);
                foreach (DataRow row in reportData.Tables[0].Rows)
                {
                    int refMarks = GetRefMarks01090305(row["НаименованиеПоказателя"].ToString());
                    if (refMarks == -1)
                        continue;
                    decimal planRep = Convert.ToDecimal(row["ГодовойКассовыйПлан"].ToString().Trim().PadRight(1, '0'));
                    decimal factRep = Convert.ToDecimal(row["КассовоеИсполнение"].ToString().Trim().PadRight(1, '0'));
                    if ((factRep == 0) && (planRep == 0))
                        continue;
                    PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", yanaoReportDate, "PlanRep", planRep, "FactRep", factRep, "RefMarks", refMarks });
                }
            }
            finally
            {
                reportData.Clear();
            }
            MoveFilesToArchive(new DirectoryInfo(dataDir), this.PumpRegistryElement.SupplierCode,
                this.PumpRegistryElement.DataCode.PadLeft(4, '0'), this.PumpID, this.SourceID, searchPattern);
            UpdateData();
            WriteToTrace("окончание закачки - 01090305'", TraceMessageKind.Information);
        }

        #endregion 01090305

        #region 01090306

        private void PumpYanao01090306FactFO35(DataSet reportData, int reportDate)
        {
            foreach (DataRow row in reportData.Tables[0].Rows)
            {
                decimal planRep = Convert.ToDecimal(row["РосписьЗаГод"].ToString().Trim().PadRight(1, '0'));
                decimal factRep = Convert.ToDecimal(row["КазнФактРасходСВозвратомЗаПериод"].ToString().Trim().PadRight(1, '0'));
                if ((factRep == 0) && (planRep == 0))
                    continue;

                string kvsr = row["КВСР"].ToString().Trim();
                string kfsr = row["КФСР"].ToString().Trim();

                string clsType = row["ТипКлассификации"].ToString().Trim().ToLower();
                if (clsType == "расходы")
                {
                    if ((kvsr == string.Empty) && (kfsr == string.Empty))
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", reportDate, "PlanRep", planRep, "FactRep", factRep, "RefMarks", 8 });
                }
                if (clsType == "выплаты по иф")
                {
                    if ((kvsr == string.Empty) && (kfsr == string.Empty))
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", reportDate, "PlanRep", -planRep, "FactRep", -factRep, "RefMarks", 9 });
                }

                string kif = row["ИсточникВнутрФинансирования"].ToString().Trim();
                if (kif.EndsWith("810"))
                    PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", yanaoReportDate, "PlanRep", planRep, "FactRep", factRep, "RefMarks", 11 });
                if (kif.EndsWith("540"))
                    PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", yanaoReportDate, "PlanRep", planRep, "FactRep", factRep, "RefMarks", 12 });
                if (kif.EndsWith("530"))
                    PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", yanaoReportDate, "PlanRep", planRep, "FactRep", factRep, "RefMarks", 14 });
            }
            UpdateData();
        }

        private void PumpYanao01090306FactFO35Outcomes(DataSet reportData, int reportDate)
        {
            foreach (DataRow row in reportData.Tables[0].Rows)
            {
                decimal planRep = Convert.ToDecimal(row["РосписьЗаГод"].ToString().Trim().PadRight(1, '0'));
                decimal factRep = Convert.ToDecimal(row["КазнФактРасходСВозвратомЗаПериод"].ToString().Trim().PadRight(1, '0'));
                if ((factRep == 0) && (planRep == 0))
                    continue;

                string clsType = row["ТипКлассификации"].ToString().Trim().ToLower();
                if (clsType != "расходы")
                    continue;

                string kvsr = row["КВСР"].ToString().Trim();
                if (kvsr == string.Empty)
                    continue;
                kvsr = kvsr.TrimStart('0').PadLeft(1, '0');
                int refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsr,
                    new object[] { "Code", kvsr, "Name", row["КВСР_Изменено"].ToString().Trim(), "CodeLine", row["RowIndex"].ToString() });

                int refRCachPl = 0;
                string kesr = row["КЭСР"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                if ((kesr == "211") || (kesr == "212") || (kesr == "213"))
                    refRCachPl = 1;
                else if (kesr == "310")
                    refRCachPl = 12;
                else if (kesr == "223")
                    refRCachPl = 3;
                else if (kesr == "251")
                    refRCachPl = 4;
                else
                    refRCachPl = 5;

                PumpRow(dsFactFO35Outcomes.Tables[0], new object[] { "RefYearDay", yanaoReportDate, "PlanRep", planRep, "FactRep", factRep, 
                    "RefRCachPl", refRCachPl, "RefKVSR", refKvsr });
            }
            UpdateData();
        }

        private void PumpYanao01090306FactFOPayment(DataSet reportData, int reportDate)
        {
            foreach (DataRow row in reportData.Tables[0].Rows)
            {
                decimal plan = Convert.ToDecimal(row["РосписьЗаГод"].ToString().Trim().PadRight(1, '0'));
                decimal fact = Convert.ToDecimal(row["КазнФактРасходСВозвратомЗаПериод"].ToString().Trim().PadRight(1, '0'));
                if ((fact == 0) && (plan == 0))
                    continue;

                string clsType = row["ТипКлассификации"].ToString().Trim().ToLower();
                if (clsType != "расходы")
                    continue;

                string kvsr = row["КВСР"].ToString().Trim();
                if (kvsr == string.Empty)
                    continue;
                kvsr = kvsr.TrimStart('0').PadLeft(1, '0');
                int refKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, kvsr,
                    new object[] { "Code", kvsr, "Name", row["КВСР_Изменено"].ToString().Trim(), "CodeLine", row["RowIndex"].ToString() });

                string ekr = row["КЭСР"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                int refEkr = PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, ekr,
                    new object[] { "Code", ekr, "Name", row["КЭСР_Изменено"].ToString().Trim() });

                string fkr = row["КФСР"].ToString().Trim().TrimStart('0').PadLeft(1, '0');
                string kcsr = row["КЦСР"].ToString().Trim();
                string kvr = row["КВР"].ToString().Trim();
                string outcomesCode = string.Concat(fkr, kcsr, kvr);
                int refR = PumpCachedRow(cacheOutcomesCls, dsOutcomesCls.Tables[0], clsOutcomesCls, outcomesCode,
                    new object[] { "Code", outcomesCode, "Name", row["КВР_Изменено"].ToString().Trim(), 
                                   fkr.TrimStart('0').PadLeft(1, '0'), kcsr.TrimStart('0').PadLeft(1, '0') });

                PumpRow(dsFOPayment.Tables[0], new object[] { "RefYearDayUNV", yanaoReportDate, "Plane", plan, "Fact", fact, 
                    "RefR", refR, "RefKVSR", refKvsr, "RefEKR", refEkr });
            }
            UpdateData();
        }

        private void PumpYanao01090306()
        {
            WriteToTrace("начало закачки - 01090306'", TraceMessageKind.Information);

            totalKvsrRef = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, "0",
                new object[] { "Code", "0", "Name", "Расходы-всего", "CodeLine", "0" });

            string searchPattern = string.Format("*{0}_(01.09.03.06) Исполнение кассового плана по выплатам.xml", this.DataSource.Year);
            DataSet reportData = new DataSet();
            try
            {
                FileInfo[] fi = new DirectoryInfo(dataDir).GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
                string fileDate = fi[0].Name.Split('_')[0];
                yanaoReportDate = Convert.ToInt32(string.Format("{0}{1}{2}", fileDate.Split('.')[2],
                    fileDate.Split('.')[1].PadLeft(2, '0'), fileDate.Split('.')[0].PadLeft(2, '0')));
                string constr = string.Format("RefYearDay = {0}", yanaoReportDate);
                DirectDeleteFactData(new IFactTable[] { fctFactFO35Outcomes,  }, -1, this.SourceID, constr);
                constr = string.Format("RefYearDayUNV = {0}", yanaoReportDate);
                DirectDeleteFactData(new IFactTable[] { fctFOPayment, }, -1, this.SourceID, constr);

                reportData.ReadXml(fi[0].FullName, XmlReadMode.ReadSchema);
                PumpYanao01090306FactFO35(reportData, yanaoReportDate);
                PumpYanao01090306FactFO35Outcomes(reportData, yanaoReportDate);
                PumpYanao01090306FactFOPayment(reportData, yanaoReportDate);
            }
            finally
            {
                reportData.Clear();
            }
            MoveFilesToArchive(new DirectoryInfo(dataDir), this.PumpRegistryElement.SupplierCode,
                this.PumpRegistryElement.DataCode.PadLeft(4, '0'), this.PumpID, this.SourceID, searchPattern);
            UpdateData();
            WriteToTrace("окончание закачки - 01090306'", TraceMessageKind.Information);
        }

        #endregion 01090306

        private void GetDataDir()
        {
            DirectoryInfo dir = new DirectoryInfo(string.Format("{0}\\{1}", GetCurrentDir().FullName, "PumpRegionalParams"));
            FileInfo[] fi = dir.GetFiles("FO35Pump.txt", SearchOption.TopDirectoryOnly);
            if (fi.GetLength(0) == 0)
                throw new Exception("Не задана директория с данными отчетов бюджета.");
            string fileText = File.ReadAllText(fi[0].FullName);
            dataDir = fileText.Split('=')[1];
        }

        private void PumpYanaoData()
        {
            GetDataDir();
            PumpYanao01090305();
            PumpYanao01090306();
            UpdateData();
        }

        #region обработка

        private void AddIncomeSumsFactFO35Yanao(ref Dictionary<int, DataRow> factCache, ref DataSet ds)
        {
            string[] sumFields = new string[] { "FactRep", "PlanRep" };
            // 2 = 3 + 10 + 13 + 15
            object[] sumMapping = GetCalcSums(sumFields, new int[] { 3, 10, 13, 15 }, new int[] { 1, 1, 1, 1 }, factCache);
            DataRow row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 2 }));
            factCache.Add(2, row);
            // 7 = 8 + 11 + 12 + 14
            sumMapping = GetCalcSums(sumFields, new int[] { 8, 11, 12, 14 }, new int[] { 1, 1, 1, 1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 7 }));
            factCache.Add(7, row);
            // 17 = 2 -7
            sumMapping = GetCalcSums(sumFields, new int[] { 2, 7 }, new int[] { 1, -1 }, factCache);
            row = PumpRow(ds.Tables[0], (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefMarks", 17 }));
            factCache.Add(17, row);
        }

        private void AddRefRCachPl9(IFactTable fct, string[] refsCls, string[] sumFields, string factConstr)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, factConstr, string.Empty);
            // Ключ - ИД классификатора, Значение - данные фактов. 
            // Данные фактов: ключ - синтетическое значение из разреза классификации строки фактов, значение - строка фактов.
            Dictionary<string, DataRow> factCache = new Dictionary<string, DataRow>();
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string key = GetGroupCacheKey(row, refsCls);
                    if (!factCache.ContainsKey(key))
                    {
                        DataRow newRow = ds.Tables[0].NewRow();
                        CopyRowToRow(row, newRow);
                        factCache.Add(key, newRow);
                        continue;
                    }
                    DataRow cacheRow = factCache[key];
                    foreach (string sumField in sumFields)
                        if (row[sumField] != DBNull.Value)
                            cacheRow[sumField] = Convert.ToDecimal(cacheRow[sumField].ToString().PadLeft(1, '0')) +
                                                 Convert.ToDecimal(row[sumField]);
                }

                // добавляем сгруппированные записи с RefRCachPl = 9
                foreach (KeyValuePair<string, DataRow> item in factCache)
                    PumpRow(ds.Tables[0], new object[] { "RefRCachPl", 9, "RefYearDay", item.Value["RefYearDay"], "RefKVSR", item.Value["RefKVSR"], 
                                                         "PlanRep", item.Value["PlanRep"], "FactRep", item.Value["FactRep"] });

                UpdateDataSet(da, ds, fct);
            }
            finally
            {
                factCache.Clear();
            }
        }

        private void AddTotalKvsr(IFactTable fct, string[] refsCls, string[] sumFields, string factConstr)
        {
            IDbDataAdapter da = null;
            DataSet ds = null;
            InitDataSet(ref da, ref ds, fct, false, factConstr, string.Empty);
            // Ключ - ИД классификатора, Значение - данные фактов. 
            // Данные фактов: ключ - синтетическое значение из разреза классификации строки фактов, значение - строка фактов.
            Dictionary<string, DataRow> factCache = new Dictionary<string, DataRow>();
            try
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string key = GetGroupCacheKey(row, refsCls);
                    if (!factCache.ContainsKey(key))
                    {
                        DataRow newRow = ds.Tables[0].NewRow();
                        CopyRowToRow(row, newRow);
                        factCache.Add(key, newRow);
                        continue;
                    }
                    DataRow cacheRow = factCache[key];
                    foreach (string sumField in sumFields)
                        if (row[sumField] != DBNull.Value)
                            cacheRow[sumField] = Convert.ToDecimal(cacheRow[sumField].ToString().PadLeft(1, '0')) +
                                                 Convert.ToDecimal(row[sumField]);
                }

                // добавляем сгруппированные записи с refKvsr = totalKvsrRef
                foreach (KeyValuePair<string, DataRow> item in factCache)
                    PumpRow(ds.Tables[0], new object[] { "RefRCachPl", item.Value["RefRCachPl"], "RefYearDay", item.Value["RefYearDay"], "RefKVSR", totalKvsrRef, 
                                                         "PlanRep", item.Value["PlanRep"], "FactRep", item.Value["FactRep"] });

                UpdateDataSet(da, ds, fct);
            }
            finally
            {
                factCache.Clear();
            }
        }

        private void ProcessYanaoData()
        {
            curDate = yanaoReportDate;

            string constr = string.Format("SOURCEID = {0} and RefYearDay = {1}", this.SourceID, curDate);
            GroupTable(fctFactFO35Outcomes, new string[] { "RefYearDay", "RefKVSR", "RefRCachPl" }, new string[] { "PlanRep", "FactRep" }, constr);

            // МЕГА ХУЕТА КОТОРУЮ НАДО ДЕЛАТЬ В КУБАХ - КАК ЖЕ ЗАЕБАЛИ БЛЯ!!!!!!!!!!! 
            // по каждому квср добавляем RefRCachPl = 9
            AddRefRCachPl9(fctFactFO35Outcomes, new string[] { "RefYearDay", "RefKVSR" }, new string[] { "PlanRep", "FactRep" }, constr);
            // бонусная мегахуета
            // по каждому RefRCachPl добавляем вычисляем итог, ссылку ставим на квср - итог
            AddTotalKvsr(fctFactFO35Outcomes, new string[] { "RefYearDay", "RefRCachPl" }, new string[] { "PlanRep", "FactRep" }, constr);

            AddAuxSumsOmsk(fctFactFO35, daFactFO35, dsFactFO35, new AddIncomeSumsOmskDelegate(AddIncomeSumsFactFO35Yanao));
            UpdateData();
        }

        #endregion обработка

    }

}
