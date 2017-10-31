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

namespace Krista.FM.Server.DataPumps.FK6Pump
{
    public class FK6PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.ФК_ФЦП (d_Marks_FKFZP)
        private IDbDataAdapter daMarksFKFZP;
        private DataSet dsMarksFKFZP;
        private IClassifier clsMarksFKFZP;
        private Dictionary<string, int> cacheMarksFKFZP = null;
        // Показатели.ФК_Направление расходов (d_Marks_FKDirection)
        private IDbDataAdapter daMarksFKDirection;
        private DataSet dsMarksFKDirection;
        private IClassifier clsMarksFKDirection;
        private Dictionary<string, int> cacheMarksFKDirection = null;
        // Администратор.КПЭ (d_KVSR_KPI)
        private IDbDataAdapter daKvsr;
        private DataSet dsKvsr;
        private IClassifier clsKvsr;
        private Dictionary<string, int> cacheKvsr = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФК_ФЦП (f_Marks_FKFZP)
        private IDbDataAdapter daMarksFact;
        private DataSet dsMarksFact;
        private IFactTable fctMarksFact;

        #endregion Факты

        int curDate = 0;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarksFKFZP, dsMarksFKFZP.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheMarksFKDirection, dsMarksFKDirection.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheKvsr, dsKvsr.Tables[0], "Name", "Id");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daMarksFKFZP, ref dsMarksFKFZP, clsMarksFKFZP, false, string.Empty, string.Empty);
            InitDataSet(ref daMarksFKDirection, ref dsMarksFKDirection, clsMarksFKDirection, false, string.Empty, string.Empty);
            InitDataSet(ref daKvsr, ref dsKvsr, clsKvsr, false, string.Empty, string.Empty);

            InitFactDataSet(ref daMarksFact, ref dsMarksFact, fctMarksFact);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarksFKFZP, dsMarksFKFZP, clsMarksFKFZP);
            UpdateDataSet(daMarksFKDirection, dsMarksFKDirection, clsMarksFKDirection);
            UpdateDataSet(daKvsr, dsKvsr, clsKvsr);

            UpdateDataSet(daMarksFact, dsMarksFact, fctMarksFact);
        }

        private const string MARKS_FKFZP_GUID = "e2135c70-2ca0-4999-9a29-5ace97672a42";
        private const string MARKS_FKDIRECTION_GUID = "b054525f-a6cb-4a68-bc16-42de48095188";
        private const string KVSR_GUID = "6aa69879-e100-4103-9db8-09b5663bcfca";
        private const string MARKS_FACT_GUID = "9e406422-7f0b-4813-b18e-8f3309fd59a9";
        protected override void InitDBObjects()
        {
            clsMarksFKFZP = this.Scheme.Classifiers[MARKS_FKFZP_GUID];
            clsMarksFKDirection = this.Scheme.Classifiers[MARKS_FKDIRECTION_GUID];
            clsKvsr = this.Scheme.Classifiers[KVSR_GUID];
            this.UsedClassifiers = new IClassifier[] { };

            fctMarksFact = this.Scheme.FactTables[MARKS_FACT_GUID];
            this.UsedFacts = new IFactTable[] { fctMarksFact };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMarksFact);

            ClearDataSet(ref dsMarksFKFZP);
            ClearDataSet(ref dsMarksFKDirection);
            ClearDataSet(ref dsKvsr);
        }

        #endregion Работа с базой и кэшами

        #region работа с xml

        private object[] GetFactValues(XmlNode row, ref bool skipRow)
        {
            skipRow = true;
            object[] factValues = new object[8];
            foreach (XmlNode fact in row.ChildNodes)
                try
                {
                    if (!fact.Name.StartsWith("POK_"))
                        continue;

                    int index = Convert.ToInt32(CommonRoutines.TrimLetters(fact.Name)) - 1;
                    string strValue = fact.InnerText.Trim();
                    if (strValue == "#Missing")
                    {
                        factValues[index] = DBNull.Value;
                    }
                    else
                    {
                        double factValue = Convert.ToDouble(strValue.Trim().PadLeft(1, '0').Replace(".", ","));
                        if (factValue != 0)
                            skipRow = false;
                        if (index < 4)
                            factValue *= 1000000;
                        factValues[index] = factValue;
                    }
                }
                catch
                {
                }
            return factValues;
        }

        private void PumpFactRow(XmlNode node, int refFKDirection, int refFKFZP, int refKVSRKPI)
        {
            bool skipRow = false;
            object[] factValues = GetFactValues(node, ref skipRow);
            if (skipRow)
                return;

            PumpRow(dsMarksFact.Tables[0], new object[] {
                "RefYearDayUNV", curDate, "RefFKDirection", refFKDirection, "RefFKFZP", refFKFZP, "RefKVSRKPI", refKVSRKPI,
                "RefinPlan", factValues[0], "LeadBA", factValues[1], "AllocateBA", factValues[2], "Execution", factValues[3],
                "PartLeadBA", factValues[4], "PartAllocateBA", factValues[5], "PartExecution", factValues[6], "PartLBO", factValues[7] });
        }

        private int PumpKvsr(XmlNode node)
        {
            string name = string.Empty;
            int code = 0;
            if (node.SelectSingleNode("D06.COMMENTS") != null)
            {
                name = node.SelectSingleNode("D06.COMMENTS").InnerText.Trim();
                code = Convert.ToInt32(name.Split('-')[2].Trim().Replace("D06_", string.Empty).TrimStart('0').PadLeft(1, '0'));
            }
            else if (node.SelectSingleNode("D06") != null)
            {
                name = node.SelectSingleNode("D06").InnerText.Trim();
                if (name != "ВСЕГО")
                    code = Convert.ToInt32(name.Split('-')[2].Trim().Replace("D06_", string.Empty).TrimStart('0').PadLeft(1, '0'));
            }
            else
                return -1;
            string shortName = name.Split(new string[] { "--" }, StringSplitOptions.None)[0].Trim();
            return PumpCachedRow(cacheKvsr, dsKvsr.Tables[0], clsKvsr, name,
                new object[] { "Code", code, "Name", name, "ShortName", shortName });
        }

        private int PumpMarksFKFZP(XmlNode node)
        {
            string name = string.Empty;
            int code = 0;
            if (node.SelectSingleNode("KBK_R2.COMMENTS") != null)
            {
                name = node.SelectSingleNode("KBK_R2.COMMENTS").InnerText;
                string[] splitStr = name.Split('-');
                code = Convert.ToInt32(splitStr[splitStr.GetLength(0) - 1].Trim().Replace("KBK_R2_", string.Empty).TrimStart('0').PadLeft(1, '0'));
            }
            else if (node.SelectSingleNode("KBK_R2") != null)
            {
                name = node.SelectSingleNode("KBK_R2").InnerText;
                code = 0;
            }
            else
                return -1;
            string shortName = name.Split(new string[] { "--" }, StringSplitOptions.None)[0].Trim();
            return PumpCachedRow(cacheMarksFKFZP, dsMarksFKFZP.Tables[0], clsMarksFKFZP, name,
                new object[] { "Code", code, "Name", name, "ShortName", shortName });
        }

        private int PumpMarksFKDirection(XmlNode node)
        {
            string name = string.Empty;
            if (node.SelectSingleNode("KBK_R3") != null)
                name = node.SelectSingleNode("KBK_R3").InnerText;
            else
                return -1;
            return PumpCachedRow(cacheMarksFKDirection, dsMarksFKDirection.Tables[0], clsMarksFKDirection, name,
                new object[] { "Code", 0, "Name", name, "ShortName", name });
        }

        private void PumpNode(XmlNode node)
        {
            int refKvsr = PumpKvsr(node);
            int refMarksFKFZP = PumpMarksFKFZP(node);
            int refMarksFKDirection = PumpMarksFKDirection(node);
            PumpFactRow(node, refMarksFKDirection, refMarksFKFZP, refKvsr);
        }

        private void PumpRows(XmlNodeList xnl)
        {
            foreach (XmlNode node in xnl)
                PumpNode(node);
        }

        private bool GetDate(XmlNode dateNode)
        {
            curDate = Convert.ToInt32(dateNode.InnerText.Trim());
            if (curDate / 10000 != this.DataSource.Year)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Дата '{0}' не соответствует источнику '{1}'", curDate, this.DataSource.Year));
                return false;
            }
            if (!this.DeleteEarlierData)
                DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, this.SourceID, string.Format(" RefYearDayUNV = {0} ", curDate));
            return true;
        }

        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                if (!GetDate(doc.SelectSingleNode("DATA/DS_DATE/DS_DATE_ROW/P_DATE")))
                    return;
                PumpRows(doc.SelectNodes("DATA/*//ROW"));
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

    }
}
