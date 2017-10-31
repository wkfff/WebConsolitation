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

        #region Работа с базой и кэшами

        private void InitDBObjectsHmao()
        {
            fctFactFO35 = this.Scheme.FactTables[F_FO35_GUID];
            fctFactFO35Outcomes = this.Scheme.FactTables[F_FO35_OUTCOMES_GUID];
            clsKvsr = this.Scheme.Classifiers[D_KVSR_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.UsedFacts = new IFactTable[] { fctFactFO35Outcomes, fctFactFO35 };
            this.AssociateClassifiers = new IClassifier[] { clsKvsr };
        }

        private void FillCachesHmao()
        {
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
        }

        private void QueryDataHmao()
        {
            InitClsDataSet(ref daKvsr, ref dsKvsr, clsKvsr);

            InitFactDataSet(ref daFactFO35Outcomes, ref dsFactFO35Outcomes, fctFactFO35Outcomes);
            InitFactDataSet(ref daFactFO35, ref dsFactFO35, fctFactFO35);

            FillCachesHmao();
        }

        private void UpdateDataHmao()
        {
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);

            UpdateDataSet(daFactFO35Outcomes, dsFactFO35Outcomes, fctFactFO35Outcomes);
            UpdateDataSet(daFactFO35, dsFactFO35, fctFactFO35);
        }

        private void PumpFinalizingHmao()
        {
            ClearDataSet(ref dsFactFO35Outcomes);
            ClearDataSet(ref dsFactFO35);

            ClearDataSet(ref dsKvsr);
       }

        #endregion Работа с базой и кэшами

        #region fctFactFO35Outcomes (f_F_ExpExctCachPl)

        private int PumpKvsrHmao(DataRow row, string nameFieldName, string codeFieldName)
        {
            string kvsrCode = row[codeFieldName].ToString().Trim();
            if (kvsrCode == string.Empty)
                kvsrCode = "0";
            string kvsrName = row[nameFieldName].ToString().Trim();
            if (kvsrName == string.Empty)
                kvsrName = "расходы - всего";
            string key = string.Format("{0}|{1}", kvsrCode, kvsrName);
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, key,
                new object[] { "Code", kvsrCode, "Name", kvsrName, "CodeLine", kvsrCode });
        }

        private void PumpFactFO35Outcomes01090302Row(DataRow row, string factName, string planName, int refRCachPl, int refKvsr)
        {
            decimal factRep = Convert.ToDecimal(row[factName].ToString().PadLeft(1, '0'));
            decimal planRep = Convert.ToDecimal(row[planName].ToString().PadLeft(1, '0'));
            if ((factRep == 0) && (planRep == 0))
                return;
            PumpRow(dsFactFO35Outcomes.Tables[0], new object[] { "RefYearDay", curDate, "RefKVSR", refKvsr, 
                        "PlanRep", planRep, "FactRep", factRep, "RefRCachPl", refRCachPl });
        }

        private void FactFO35Outcomes01090302()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 01090302'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(01.09.03.02) Исполнение кассового плана окружного бюджета по расходам.sts", 
                    "ОднаДата", curDate.ToString() }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    if (i == rowsCount - 1)
                        PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "PlanRep", Convert.ToDecimal(row["РоспНаПервыйГод"].ToString().PadLeft(1, '0')), 
                            "FactRep", Convert.ToDecimal(row["КазнФактРасходСВозвратомЗаПериод"].ToString().PadLeft(1, '0')), "RefMarks", 8 });

                    int refKvsr = PumpKvsrHmao(row, "КВСР_", "КВСР");
                    PumpFactFO35Outcomes01090302Row(row, "КазнФактРасходСВозвратомЗаПериод", "РоспНаПервыйГод", 9, refKvsr);

                    if (i == rowsCount - 1)
                    {
                        PumpFactFO35Outcomes01090302Row(row, "ЗарплатаКазнВсе", "ЗарплатаРоспВсе", 1, refKvsr);
                        PumpFactFO35Outcomes01090302Row(row, "КапитВложКазнВсе", "КапитВложРоспВсе", 12, refKvsr);
                        PumpFactFO35Outcomes01090302Row(row, "КоммунУслугиКазнВсе", "КоммунУслугиРоспВсе", 3, refKvsr);
                        PumpFactFO35Outcomes01090302Row(row, "БезвоздмПеречисленКазнВсе", "БезвоздмПеречисленРоспВсе", 4, refKvsr);
                    }
                    else
                    {
                        PumpFactFO35Outcomes01090302Row(row, "ЗарПлатаКазн", "ЗарПлатаРосп", 1, refKvsr);
                        PumpFactFO35Outcomes01090302Row(row, "КапитВложКазн", "КапитВложРосп", 12, refKvsr);
                        PumpFactFO35Outcomes01090302Row(row, "КоммунУслугиКазн", "КоммунУслугиРосп", 3, refKvsr);
                        PumpFactFO35Outcomes01090302Row(row, "БезвоздмездныеПеречисленДоход", "БезвоздмПеречисленРосп", 4, refKvsr);
                    }
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы - 01090302'", TraceMessageKind.Information);
        }

        private void PumpFactFO35OutcomesHmao()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы'", TraceMessageKind.Information);
            FactFO35Outcomes01090302();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета_Расходы'", TraceMessageKind.Information);
        }

        #endregion fctFactFO35Outcomes (f_F_ExpExctCachPl)

        #region fctFactFO35 (f_F_ExctCachPl)

        private int GetRefMarks01090301(string kbk)
        {
            switch (kbk)
            {
                case "":
                    return 3;
                case "10101000000000100":
                    return 34;
                case "10102000010000100":
                    return 36;
                case "10000000000000000":
                    return 35;
                case "20000000000000000":
                    return 6;
            }
            return 3;            
        }

        private void FactFO3501090301()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 01090301'", TraceMessageKind.Information);
            DataSet reportData = new DataSet();
            try
            {
                GetBudReportData(new string[] { "Шаблон", "(01.09.03.01) Исполнение кассового плана окружного бюджета по доходам.sts", 
                    "ОднаДата", curDate.ToString() }, ref reportData);
                int rowsCount = reportData.Tables[0].Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    DataRow row = reportData.Tables[0].Rows[i];
                    string name = row["НаименованиеПоказателя"].ToString().Trim();
                    if (name.ToUpper().Contains("В ТОМ ЧИСЛЕ"))
                        continue;

                    string kbk = row["КБК"].ToString().Trim();
                    int refMarks = GetRefMarks01090301(kbk);
                    decimal factRep = Convert.ToDecimal(row["КассовоеИсполнение"].ToString().Trim().PadLeft(1, '0'));
                    decimal planRep = Convert.ToDecimal(row["ГодовойКассовыйПлан"].ToString().Trim().PadLeft(1, '0'));
                    if ((factRep == 0) && (planRep == 0))
                        continue;
                    PumpRow(dsFactFO35.Tables[0], new object[] { "RefYearDayUNV", curDate, "RefMarks", refMarks, 
                        "FactRep", factRep, "PlanRep", planRep });
                }
            }
            finally
            {
                reportData.Clear();
            }
            UpdateData();
            WriteToTrace("конец закачки - 'Факт.ФО_Кассовый план исполнения бюджета - 01090301'", TraceMessageKind.Information);
        }

        private void PumpFactFO35Hmao()
        {
            WriteToTrace("начало закачки - 'Факт.ФО_Исполнение кассового плана'", TraceMessageKind.Information);
            FactFO3501090301();
            WriteToTrace("конец закачки - 'Факт.ФО_Исполнение кассового плана'", TraceMessageKind.Information);
        }

        #endregion fctFactFO35 (f_F_ExctCachPl)

        private void DeleteDataHmao()
        {
            string constr = string.Format("RefYearDay = {0}", curDate);
            DirectDeleteFactData(new IFactTable[] { fctFactFO35Outcomes }, -1, this.SourceID, constr);
            constr = string.Format("RefYearDayUNV = {0}", curDate);
            DirectDeleteFactData(new IFactTable[] { fctFactFO35 }, -1, this.SourceID, constr);
        }

        private void PumpHmaoData(DirectoryInfo dir)
        {
            GetDateNovosib();
            DeleteDataHmao();
            budWP = new WorkplaceAutoObjectClass();
            try
            {
                string[] udlData = GetUdlData(dir);
                string dbName = string.Empty;
                string login = string.Empty;
                string password = string.Empty;
                GetUdlParams(udlData, ref dbName, ref login, ref password);
                budWP.Login(dbName, login, password);

                PumpFactFO35OutcomesHmao();
                PumpFactFO35Hmao();
                UpdateData();
            }
            finally
            {
                Marshal.ReleaseComObject(budWP);
            }
        }

        
    }

}
