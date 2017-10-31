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

namespace Krista.FM.Server.DataPumps.UFK21Pump
{

    // УФК_0021_Консолидированный отчет о кассовых поступлениях и выбытиях
    public class UFK21PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // КД.УФК (d_KD_UFK)
        private IDbDataAdapter daKd;
        private DataSet dsKd;
        private IClassifier clsKd;
        private Dictionary<string, int> cacheKd = null;
        // КИФ.УФК (d_KIF_UFK)
        private IDbDataAdapter daKif;
        private DataSet dsKif;
        private IClassifier clsKif;
        private Dictionary<string, int> cacheKif = null;
        // ФКР.УФК (d_FKR_UFK)
        private IDbDataAdapter daFkr;
        private DataSet dsFkr;
        private IClassifier clsFkr;
        private Dictionary<string, int> cacheFkr = null;
        // ЭКР.УФК (d_EKR_UFK)
        private IDbDataAdapter daEkr;
        private DataSet dsEkr;
        private IClassifier clsEkr;
        private Dictionary<string, int> cacheEkr = null;
        // Районы.УФК (d_Regions_UFK)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;

        #endregion Классификаторы

        #region Факты

        // Факт.УФК_КонсОтчет_Доходы (f_F_ConsRepIncomes)
        private IDbDataAdapter daConsRepIncomes;
        private DataSet dsConsRepIncomes;
        private IFactTable fctConsRepIncomes;
        // Факт.УФК_КонсОтчет_Расходы (f_F_ConsRepOutcomes)
        private IDbDataAdapter daConsRepOutcomes;
        private DataSet dsConsRepOutcomes;
        private IFactTable fctConsRepOutcomes;
        // Факт.УФК_КонсОтчет_Источники финансирования (f_F_ConsRepFin)
        private IDbDataAdapter daConsRepFin;
        private DataSet dsConsRepFin;
        private IFactTable fctConsRepFin;
        // Факт.УФК_КонсОтчет_Дефицит Профицит (f_F_ConsRepDefProf)
        private IDbDataAdapter daConsRepDefProf;
        private DataSet dsConsRepDefProf;
        private IFactTable fctConsRepDefProf;

        #endregion Факты

        private bool allowPump = false;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        protected override void QueryData()
        {
            InitClsDataSet(ref daKd, ref dsKd, clsKd);
            InitClsDataSet(ref daKif, ref dsKif, clsKif);
            InitClsDataSet(ref daFkr, ref dsFkr, clsFkr);
            InitClsDataSet(ref daEkr, ref dsEkr, clsEkr);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions);

            InitFactDataSet(ref daConsRepIncomes, ref dsConsRepIncomes, fctConsRepIncomes);
            InitFactDataSet(ref daConsRepOutcomes, ref dsConsRepOutcomes, fctConsRepOutcomes);
            InitFactDataSet(ref daConsRepFin, ref dsConsRepFin, fctConsRepFin);
            InitFactDataSet(ref daConsRepDefProf, ref dsConsRepDefProf, fctConsRepDefProf);

            FillCaches();
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheKd, dsKd.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheKif, dsKif.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheFkr, dsFkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheEkr, dsEkr.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], "Code", "Id");
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKd, dsKd, clsKd);
            UpdateDataSet(daKif, dsKif, clsKif);
            UpdateDataSet(daFkr, dsFkr, clsFkr);
            UpdateDataSet(daEkr, dsEkr, clsEkr);
            UpdateDataSet(daRegions, dsRegions, clsRegions);

            UpdateDataSet(daConsRepIncomes, dsConsRepIncomes, fctConsRepIncomes);
            UpdateDataSet(daConsRepOutcomes, dsConsRepOutcomes, fctConsRepOutcomes);
            UpdateDataSet(daConsRepFin, dsConsRepFin, fctConsRepFin);
            UpdateDataSet(daConsRepDefProf, dsConsRepDefProf, fctConsRepDefProf);
        }

        private const string D_KD_GUID = "b713e1df-5584-4e3d-a399-8828a2906971";
        private const string D_KIF_GUID = "73b83ed3-fa26-4d05-8e8e-30dbe226a801";
        private const string D_FKR_GUID = "2e555c6f-f99a-4a3f-8318-74bdf7ff2ab8";
        private const string D_EKR_GUID = "b234f8dc-d37d-4cc0-a32e-2e74b2bfb935";
        private const string D_REGIONS_GUID = "90375d17-5145-43b9-81f1-2145aba86b7c";
        private const string F_F_CONS_REP_INCOMES_GUID = "d07d7d1a-3fd8-42c7-8668-c67bc680ed85";
        private const string F_F_CONS_REP_OUTCOMES_GUID = "7c324d34-dc82-4670-8131-4c85da4514fd";
        private const string F_F_CONS_REP_FIN_GUID = "0a4c99a4-ddcc-4a64-ac8d-9353acc38144";
        private const string F_F_CONS_REP_DEFPROF_GUID = "1afdb2c3-2f31-4693-80e9-e2b990a43066";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {
                clsKd = this.Scheme.Classifiers[D_KD_GUID],
                clsKif = this.Scheme.Classifiers[D_KIF_GUID], 
                clsFkr = this.Scheme.Classifiers[D_FKR_GUID], 
                clsEkr = this.Scheme.Classifiers[D_EKR_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_GUID] };
            
            this.UsedFacts = new IFactTable[] {
                fctConsRepIncomes = this.Scheme.FactTables[F_F_CONS_REP_INCOMES_GUID],
                fctConsRepOutcomes = this.Scheme.FactTables[F_F_CONS_REP_OUTCOMES_GUID],
                fctConsRepFin = this.Scheme.FactTables[F_F_CONS_REP_FIN_GUID],
                fctConsRepDefProf = this.Scheme.FactTables[F_F_CONS_REP_DEFPROF_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsKd);
            ClearDataSet(ref dsKif);
            ClearDataSet(ref dsFkr);
            ClearDataSet(ref dsEkr);
            ClearDataSet(ref dsRegions);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Txt

        #region Закачка классификаторов

        private int PumpRegion(string[] reportData)
        {
            string name = string.Empty;
            int i = 0;
            while (!reportData[i].Replace("\n", string.Empty).StartsWith("УК"))
            {
                name = string.Format("{0} {1}", name, reportData[i].Replace("\n", string.Empty));
                i++;
            }
            name = name.Trim();
            string code = reportData[i].Split('=')[1].TrimStart('0').PadLeft(1, '0');
            return PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, code,
                new object[] { "CODE", code, "NAME", name });
        }

        private int PumpKD(string[] rowValues)
        {
            string code = string.Format("{0}{1}", rowValues[1], rowValues[2]);
            return PumpCachedRow(cacheKd, dsKd.Tables[0], clsKd, code,
                new object[] { "CODESTR", code, "NAME", "Неуказанное наименование" });
        }

        private int PumpFKR(string[] rowValues)
        {
            int code = Convert.ToInt32(rowValues[2].Trim().PadLeft(1, '0'));
            return PumpCachedRow(cacheFkr, dsFkr.Tables[0], clsFkr, code.ToString(), 
                new object[] { "CODE", code });
        }

        private int PumpEKR(string[] rowValues)
        {
            int code = Convert.ToInt32(rowValues[5].Trim().PadLeft(1, '0'));
            return PumpCachedRow(cacheEkr, dsEkr.Tables[0], clsEkr, code.ToString(),
                new object[] { "CODE", code });
        }

        private int PumpKIF(string[] rowValues)
        {
            string code = string.Format("{0}{1}", rowValues[1], rowValues[2]);
            return PumpCachedRow(cacheKif, dsKif.Tables[0], clsKif, code,
                new object[] { "CODESTR", code });
        }

        #endregion Закачка классификаторов

        #region Закачка фактов

        private void PumpIncomesFact(string factStr, int refDate, int refBudget, int refKd, int refRegion)
        {
            decimal fact = Convert.ToDecimal(factStr.Trim().Replace('.', ',').PadLeft(1, '0'));
            if (fact == 0)
                return;

            object[] mapping = new object[] { "Fact", fact, "RefKD", refKd,
                "RefYearDayUNV", refDate, "RefRegions", refRegion, "RefFX", refBudget };

            PumpRow(dsConsRepIncomes.Tables[0], mapping);
            if (dsConsRepIncomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daConsRepIncomes, ref dsConsRepIncomes);
            }
        }

        private void PumpOutcomesFact(string factStr, int refDate, int refBudget, int refFkr, int refEkr, int refRegion)
        {
            decimal fact = Convert.ToDecimal(factStr.Trim().Replace('.', ',').PadLeft(1, '0'));
            if (fact == 0)
                return;

            object[] mapping = new object[] { "Fact", fact, "RefFKR", refFkr, "RefEKR", refEkr,
                "RefYearDayUNV", refDate, "RefRegions", refRegion, "RefFX", refBudget };

            PumpRow(dsConsRepOutcomes.Tables[0], mapping);
            if (dsConsRepOutcomes.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daConsRepOutcomes, ref dsConsRepOutcomes);
            }
        }

        private void PumpDefProfFact(string factStr, int refDate, int refBudget, int refRegion)
        {
            decimal fact = Convert.ToDecimal(factStr.Trim().Replace('.', ',').PadLeft(1, '0'));
            if (fact == 0)
                return;

            object[] mapping = new object[] { "Fact", fact, "RefYearDayUNV", refDate,
                    "RefRegions", refRegion, "RefFX", refBudget };

            PumpRow(dsConsRepDefProf.Tables[0], mapping);
            if (dsConsRepDefProf.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daConsRepDefProf, ref dsConsRepDefProf);
            }
        }

        private void PumpFinFact(string factStr, int refDate, int refBudget, int refKif, int refRegion)
        {
            decimal fact = Convert.ToDecimal(factStr.Trim().Replace('.', ',').PadLeft(1, '0'));
            if (fact == 0)
                return;

            object[] mapping = new object[] { "Fact", fact, "RefYearDayUNV", refDate, "RefKif", refKif,
                    "RefRegions", refRegion, "RefFX", refBudget };

            PumpRow(dsConsRepFin.Tables[0], mapping);
            if (dsConsRepFin.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daConsRepFin, ref dsConsRepFin);
            }
        }

        #endregion Закачка фактов

        private const string INCOMES_MARK = "010";
        private const string OUTCOMES_MARK = "200";
        private const string DEFPROF_MARK = "450";
        private void PumpTxtRow(string[] rowValues, int refDate, int refRegion)
        {
            switch (rowValues[0].Trim())
            { 
                case INCOMES_MARK:
                    int refKd = PumpKD(rowValues);
                    PumpIncomesFact(rowValues[3], refDate, 3, refKd, refRegion);
                    PumpIncomesFact(rowValues[5], refDate, 15, refKd, refRegion);
                    PumpIncomesFact(rowValues[6], refDate, 5, refKd, refRegion);
                    PumpIncomesFact(rowValues[7], refDate, 6, refKd, refRegion);
                    return;
                case OUTCOMES_MARK:
                    int refFkr = PumpFKR(rowValues);
                    int refEkr = PumpEKR(rowValues);
                    PumpOutcomesFact(rowValues[6], refDate, 3, refFkr, refEkr, refRegion);
                    PumpOutcomesFact(rowValues[8], refDate, 15, refFkr, refEkr, refRegion);
                    PumpOutcomesFact(rowValues[9], refDate, 5, refFkr, refEkr, refRegion);
                    PumpOutcomesFact(rowValues[10], refDate, 6, refFkr, refEkr, refRegion);
                    return;
                case DEFPROF_MARK:
                    PumpDefProfFact(rowValues[6], refDate, 3, refRegion);
                    PumpDefProfFact(rowValues[8], refDate, 15, refRegion);
                    PumpDefProfFact(rowValues[9], refDate, 5, refRegion);
                    PumpDefProfFact(rowValues[10], refDate, 6, refRegion);
                    return;
            }
            if (allowPump)
            {
                int refKif = PumpKIF(rowValues);
                PumpFinFact(rowValues[3], refDate, 3, refKif, refRegion);
                PumpFinFact(rowValues[5], refDate, 15, refKif, refRegion);
                PumpFinFact(rowValues[6], refDate, 5, refKif, refRegion);
                PumpFinFact(rowValues[7], refDate, 6, refKif, refRegion);
            }
        }

        private bool IsNotPump(string row)
        {
            return (row.Contains("*") && row.Split(DELIMETER)[0].Trim() != DEFPROF_MARK);
        }

        private int GetReportDate()
        {
            // получаем из параметров источника
            return this.DataSource.Year * 10000 + this.DataSource.Month * 100;
        }

        private const string RD3_MARK = "РД=3";
        private char DELIMETER = '|';
        private void PumpTXTFile(FileInfo file)
        {
            string[] reportData = CommonRoutines.GetTxtReportData(file, CommonRoutines.GetTxtDosCodePage());
            int refDate = GetReportDate();
            int refRegion = PumpRegion(reportData);
            int rowIndex = 0;
            allowPump = false;
            foreach (string row in reportData)
                try
                {
                    rowIndex++;
                    if (row.Trim() == string.Empty)
                        return;

                    if (IsNotPump(row))
                        continue;

                    if (row.Trim().ToUpper() == RD3_MARK)
                    {
                        allowPump = true;
                        continue;
                    }

                    string[] rowValues = row.Split(DELIMETER);
                    if (rowValues.Length < 7)
                        continue;
                    PumpTxtRow(rowValues, refDate, refRegion);
                }
                catch (Exception exp)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                        string.Format("Ошибка при обработке строки {0} отчета {1}: {2}", rowIndex, file.Name, exp.Message));
                    this.DataSourceProcessingResult = DataSourceProcessingResult.ProcessedWithErrors;
                }
        }

        #endregion Работа с Txt

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.*", new ProcessFileDelegate(PumpTXTFile), false);
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataYMTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}