using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.FK8Pump
{
    public class FK8PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.Госпрограммы (d_Marks_GovProgram)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        // Расходы.КПЭ (d_R_KPI)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IClassifier clsOutcomes;
        private Dictionary<string, int> cacheOutcomes = null;
        // Администратор.КПЭ (d_KVSR_KPI)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<string, int> cacheKvsr = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФК_Госпрограммы (f_Marks_FKGovProg)
        private IDbDataAdapter daFKGovProg;
        private DataSet dsFKGovProg;
        private IFactTable fctFKGovProg;

        #endregion Факты

        int curDate = 0;
        int curRefKvsr = -1;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarks, dsMarks.Tables[0], "CodeStr", "Id");
            FillRowsCache(ref cacheOutcomes, dsOutcomes.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daMarks, ref dsMarks, clsMarks, false, string.Empty, string.Empty);
            InitDataSet(ref daOutcomes, ref dsOutcomes, clsOutcomes, false, string.Empty, string.Empty);
            InitDataSet(ref daKvsr, ref dsKvsr, clsKvsr, false, string.Empty, string.Empty);

            InitFactDataSet(ref daFKGovProg, ref dsFKGovProg, fctFKGovProg);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarks, dsMarks, clsMarks);
            UpdateDataSet(daOutcomes, dsOutcomes, clsOutcomes);
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);

            UpdateDataSet(daFKGovProg, dsFKGovProg, fctFKGovProg);
        }

        private const string MARKS_GUID = "11e41767-b69b-49a5-94f9-f92cba8e0ae7";
        private const string OUTCOMES_GUID = "0530d9c5-d2f4-437d-9dca-f601246c7ee0";
        private const string KVSR_GUID = "6aa69879-e100-4103-9db8-09b5663bcfca";
        private const string FACT_GUID = "3537d8f7-1ff0-479a-a8d7-94ba019be168";
        protected override void InitDBObjects()
        {
            clsMarks = this.Scheme.Classifiers[MARKS_GUID];
            clsOutcomes = this.Scheme.Classifiers[OUTCOMES_GUID];
            clsKvsr = this.Scheme.Classifiers[KVSR_GUID];
            this.UsedClassifiers = new IClassifier[] { };
            this.HierarchyClassifiers = new IClassifier[] { clsOutcomes }; 

            fctFKGovProg = this.Scheme.FactTables[FACT_GUID];
            this.UsedFacts = new IFactTable[] { fctFKGovProg };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsFKGovProg);

            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsOutcomes);
            ClearDataSet(ref dsKvsr);
        }

        #endregion Работа с базой и кэшами

        #region работа с xml

        private object[] GetFactValues(object[] sumMapping, XmlNode row, ref bool skipRow)
        {
            skipRow = true;
            for (int i = 0; i <= sumMapping.GetLength(0) - 1; i += 2)
            {
                string strValue = row.SelectSingleNode(sumMapping[i + 1].ToString()).InnerText.Trim();
                if (strValue == "#Missing")
                {
                    sumMapping[i + 1] = DBNull.Value;
                }
                else
                {
                    double factValue = Convert.ToDouble(strValue.Trim().PadLeft(1, '0').Replace(".", ","));
                    if (factValue != 0)
                        skipRow = false;
                    if (i >= 14)
                        factValue /= 100;
                    sumMapping[i + 1] = factValue;
                }
            }
            return sumMapping;
        }

        private void PumpFactRow(XmlNode node, int refOutcomes, int refMarks, int refKvsr)
        {
            bool skipRow = false;
            object[] sumMapping = GetFactValues(new object[] { "LeadLBOReport", "P223", "LeadBAPNOReport", "P10", "AllocateLBOReport", "P11", 
                "AllocateBAPNOReport", "P12", "RefinPlanReport", "P13", "RefinPlanNoPNOReport", "P14", "ExecutionReport", "P15", 
                "PartLeadBAnoPNOReport", "P16", "PartAllocateLBOReport", "P17", "PartExecutionReport", "P18", "PartLBOReport", "P19" }, 
                node, ref skipRow);
            if (skipRow)
                return;
            object[] mapping = (object[])CommonRoutines.ConcatArrays(sumMapping,
                new object[] { "RefYearDayUNV", curDate, "RefGovProgram", refMarks, "RefRKPI", refOutcomes, "RefKVSRKPI", refKvsr });
            PumpRow(dsFKGovProg.Tables[0], mapping);
        }

        private int PumpKvsr(XmlNode node)
        {
            if (node.SelectSingleNode("MGR_ID").InnerText.Trim() == string.Empty)
                return -1;
            if (node.SelectSingleNode("RAZD").InnerText.Trim() != string.Empty)
                return curRefKvsr;

            int code = Convert.ToInt32(node.SelectSingleNode("MGR_ID").InnerText);
            string name = node.SelectSingleNode("NAME").InnerText;
            string key = string.Format("{0}|{1}", code, name);
            curRefKvsr = PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, key,
                new object[] { "Code", code, "Name", name });
            return curRefKvsr;
        }

        private int PumpMarks(XmlNode node)
        {
            string code = node.SelectSingleNode("GP").InnerText; 
            string name = node.SelectSingleNode("NAME").InnerText;
            string shortName = name;
            if (name.Length > 255)
                shortName = name.Substring(0, 255);
            return PumpCachedRow(cacheMarks, dsMarks.Tables[0], clsMarks, code,
                new object[] { "CodeStr", code, "Name", name, "ShortTitl", shortName });
        }

        private int PumpOutcomes(XmlNode node)
        {
            int nullOutcomes = PumpCachedRow(cacheOutcomes, dsOutcomes.Tables[0], clsOutcomes, "0",
                new object[] { "Code", 0, "Name", "Неуказанный код расходной классификации", 
                    "ShortTitl", "Неуказанный код расходной классификации" });
            if (node.SelectSingleNode("RAZD").InnerText.Trim() == string.Empty)
                return nullOutcomes;

            string name = node.SelectSingleNode("NAME").InnerText;
            string razd = node.SelectSingleNode("RAZD").InnerText.PadLeft(2, '0');
            string podrazd = node.SelectSingleNode("PODRAZD").InnerText.PadLeft(2, '0');
            string kbkR2 = node.SelectSingleNode("KBK_R2").InnerText.PadLeft(7, '0');
            string kbkR3 = node.SelectSingleNode("KBK_R3").InnerText.PadLeft(3, '0');
            string code = string.Format("{0}{1}{2}{3}", razd, podrazd, kbkR2, kbkR3).TrimStart('0').PadLeft(1, '0');
            string rzPr = string.Format("{0}{1}", razd, podrazd);
            string shortName = name;
            if (name.Length > 255)
                shortName = name.Substring(0, 255);

            return PumpCachedRow(cacheOutcomes, dsOutcomes.Tables[0], clsOutcomes, code,
                new object[] { "Code", code, "Name", name, "ShortTitl", shortName, "RzPr", rzPr, "KCSR", kbkR2, "KVR", kbkR3 });
        }

        private void PumpNode(XmlNode node)
        {
            int refKvsr = PumpKvsr(node);
            int refMarks = PumpMarks(node);
            int refOutcomes = PumpOutcomes(node);
            PumpFactRow(node, refOutcomes, refMarks, refKvsr);
        }

        private void PumpRows(XmlNodeList xnl)
        {
            foreach (XmlNode node in xnl)
                PumpNode(node);
        }

        private bool GetDate(XmlNode dateNode)
        {
            string value = CommonRoutines.TrimLetters(dateNode.InnerText.Trim());
            curDate = CommonRoutines.ShortDateToNewDate(value);
            DateTime dt = new DateTime(curDate / 10000, curDate / 100 % 100, curDate % 100);
            curDate = CommonRoutines.ShortDateToNewDate(dt.AddDays(-1).ToShortDateString());
            if (curDate / 10000 != this.DataSource.Year)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Дата '{0}' не соответствует источнику '{1}'", curDate, this.DataSource.Year));
                return false;
            }
            if (!this.DeleteEarlierData)
                DirectDeleteFactData(new IFactTable[] { fctFKGovProg }, -1, this.SourceID, string.Format(" RefYearDayUNV = {0} ", curDate));
            return true;
        }

        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                if (!GetDate(doc.SelectSingleNode("DATA/DS_PARAMETRS/DS_PARAMETRS_ROW/P_POST_DATE")))
                    return;
                PumpRows(doc.SelectNodes("DATA/DS_MAIN/DS_MAIN_ROW"));
                UpdateData();
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion работа с xml

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        private void SetHierarchyLevel()
        {
            foreach (DataRow row in dsOutcomes.Tables[0].Rows)
            {
                string code = row["Code"].ToString();
                if (code.EndsWith("000000000000"))
                    row["HierarchyLevel"] = 1;
                else if (code.EndsWith("0000000000"))
                    row["HierarchyLevel"] = 2;
                else if (code.EndsWith("0000000"))
                    row["HierarchyLevel"] = 3;
                else if (code.EndsWith("00000"))
                    row["HierarchyLevel"] = 4;
                else if (code.EndsWith("000"))
                    row["HierarchyLevel"] = 5;
                else 
                    row["HierarchyLevel"] = 6;
            }
            UpdateData();
        }

        private void CorrectSums()
        {
            CommonLiteSumCorrectionConfig scc = new CommonLiteSumCorrectionConfig();
            scc.sumFieldForCorrect = new string[] { "LeadLBOReport", "LeadBAPNOReport", "AllocateLBOReport", "AllocateBAPNOReport", 
                "RefinPlanReport", "RefinPlanNoPNOReport", "ExecutionReport", "PartLeadBAnoPNOReport", "PartAllocateLBOReport", 
                "PartExecutionReport", "PartLBOReport" };
            scc.fields4CorrectedSums = new string[] { "LeadLBO", "LeadBAPNO", "AllocateLBO", "AllocateBAPNO", 
                "RefinPlan", "RefinPlanNoPNO", "Execution", "PartLeadBAnoPNO", "PartAllocateLBO", 
                "PartExecution", "PartLBO" };

            CorrectFactTableSums(fctFKGovProg, dsOutcomes.Tables[0], clsOutcomes, "RefRKPI",
                scc, BlockProcessModifier.MRStandard, new string[] { "RefGovProgram", "RefKVSRKPI", "RefYearDayUNV" }, string.Empty, string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetHierarchyLevel();
            CorrectSums();
            UpdateData();
        }

        protected override void DirectProcessData()
        {
            ProcessDataSourcesTemplate("Установка иерархии классификаторов и коррекция сумм в таблицах фактов");
        }

        #endregion Обработка данных


    }
}
