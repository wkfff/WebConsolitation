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

namespace Krista.FM.Server.DataPumps.FK5Pump
{
    public class FK5PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.Бюджет СГУ (d_Marks_BudgetSGU)
        private IDbDataAdapter daMarksCls;
        private DataSet dsMarksCls;
        private IClassifier clsMarksCls;
        private Dictionary<string, int> cacheMarksCls = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФК_Бюджет СГУ (f_Marks_FKBudgetSGU)
        private IDbDataAdapter daMarksFact;
        private DataSet dsMarksFact;
        private IFactTable fctMarksFact;

        #endregion Факты

        int curDate = 0;
        int marksCode = 0;

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheMarksCls, dsMarksCls.Tables[0], "Name", "Id");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daMarksCls, ref dsMarksCls, clsMarksCls, false, string.Empty, string.Empty);

            InitFactDataSet(ref daMarksFact, ref dsMarksFact, fctMarksFact);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daMarksCls, dsMarksCls, clsMarksCls);

            UpdateDataSet(daMarksFact, dsMarksFact, fctMarksFact);
        }

        private const string MARKS_CLS_GUID = "20e6783b-d8be-4c6f-ae2f-32584a1b9749";
        private const string MARKS_FACT_GUID = "dd74fa06-efff-4d8c-8c17-3f34f1c8f684";
        protected override void InitDBObjects()
        {
            clsMarksCls = this.Scheme.Classifiers[MARKS_CLS_GUID];
            this.UsedClassifiers = new IClassifier[] { };
            fctMarksFact = this.Scheme.FactTables[MARKS_FACT_GUID];
            this.UsedFacts = new IFactTable[] { fctMarksFact };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsMarksFact);

            ClearDataSet(ref dsMarksCls);
        }

        #endregion Работа с базой и кэшами

        #region работа с xml

        #region 02_rep_21.xml

        private double GetFactValue(string strValue)
        {
            if (strValue == "#Missing")
                return 0;
            // заплата
            if (strValue == "0.0010")
                return 0;
            return Convert.ToDouble(strValue.PadLeft(1, '0').Replace(".", ","));
        }

        private void PumpFactRow02(XmlNode node, int refMarks, int refBudLevel, string xmlFactDateName, string dbFieldName, double multFact)
        {
            double factDate = 0;
            if (node.SelectSingleNode(xmlFactDateName) != null)
                factDate = GetFactValue(node.SelectSingleNode(xmlFactDateName).InnerText.Trim()) * multFact;
            if (factDate != 0)
                PumpRow(dsMarksFact.Tables[0], new object[] { dbFieldName, factDate, "RefMarksBudSGU", refMarks, "RefYearDayUNV", curDate, "RefBudLevels", refBudLevel });
        }

        private void PumpNode02(XmlNode node)
        {
            if (node.SelectSingleNode("FACTOR") == null)
                return;
            marksCode += 1;
            string name = node.SelectSingleNode("FACTOR").InnerText;
            int refMarks = PumpCachedRow(cacheMarksCls, dsMarksCls.Tables[0], clsMarksCls, name,
                new object[] { "Code", marksCode, "Name", name });

            DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, this.SourceID, 
                string.Format(" RefYearDayUNV = {0} and RefMarksBudSGU = {1} ", curDate, refMarks));

            PumpFactRow02(node, refMarks, 21, "D08_MLN", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 18, "ФЕДЕРАЛЬНЫЙ_БЮДЖЕТ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 20, "БЮДЖЕТЫ_ГОСУДАРСТВЕННЫХ_ВНЕБЮДЖЕТНЫХ_ФОНДОВ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 3, "БЮДЖЕТЫ_СУБЪЕКТОВ_РФ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 8, "БЮДЖЕТЫ_ТЕРРИТОРИАЛЬНЫХ_ГОСУДАРСТВЕННЫХ_ВНЕБЮДЖЕТНЫХ_ФОНДОВ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 5, "РАЙОННЫЕ_БЮДЖЕТЫ_МУНИЦИПАЛЬНЫХ_РАЙОНОВ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 4, "БЮДЖЕТЫ_ГОРОДСКИХ_ОКРУГОВ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 11, "БЮДЖЕТЫ_ВНУТРИГОРОДСКИХ_МУНИЦИПАЛЬНЫХ_ОБРАЗОВАНИЙ_ГОРОДОВ_ФЕДЕРАЛЬНОГО_ЗНАЧЕНИЯ_МОСКВЫ_И_СПБ", "FactDate", 1000000);
            PumpFactRow02(node, refMarks, 6, "БЮДЖЕТЫ_ГОРОДСКИХ_И_СЕЛЬСКИХ_ПОСЕЛЕНИЙ", "FactDate", 1000000);
        }

        private void PumpRows02(XmlNodeList xnl)
        {
            foreach (XmlNode node in xnl)
                PumpNode02(node);
        }

        private bool GetDate02(XmlNode dateNode)
        {
            string date = CommonRoutines.TrimLetters(dateNode.InnerText);
            DateTime dt = new DateTime(2000 + Convert.ToInt32(date.Split('.')[2]), Convert.ToInt32(date.Split('.')[1]), Convert.ToInt32(date.Split('.')[0]));
            dt = dt.AddDays(-1);
            curDate = CommonRoutines.ShortDateToNewDate(dt.ToShortDateString());
            if (curDate / 10000 != this.DataSource.Year)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Дата '{0}' не соответствует источнику '{1}'", curDate, this.DataSource.Year));
                return false;
            }
            return true;
        }

        private void PumpXmlFile02(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                if (!GetDate02(doc.SelectSingleNode("DATA/DS_PARAMETERS/ROW/P_DATE")))
                    return;
                PumpRows02(doc.SelectNodes("DATA/*//ROW"));
                UpdateData();
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion 02_rep_21.xml

        #region 03_rep_22.xml  04_rep_23.xml

        private bool GetDate03(XmlNode dateNode)
        {
            curDate = Convert.ToInt32(dateNode.SelectSingleNode("PERIODS").InnerText);
            if (curDate.ToString().Length == 6)
                curDate = curDate * 100 + CommonRoutines.GetDaysInMonth(curDate % 100, curDate / 100);
            if (curDate / 10000 != this.DataSource.Year)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Дата '{0}' не соответствует источнику '{1}'", curDate, this.DataSource.Year));
                return false;
            }
            return true;
        }

        private void PumpNode03(XmlNode node)
        {
            if (!GetDate03(node))
                return;

            string name = string.Empty;
            if (node.SelectSingleNode("KBK_D") != null)
                name = node.SelectSingleNode("KBK_D").InnerText.Trim();
            else
                name = node.SelectSingleNode("KBK_R1.COMMENTS").InnerText.Trim();
            if ((name == string.Empty) || (name == "из них:"))
                return;
            marksCode += 1;
            int refMarks = PumpCachedRow(cacheMarksCls, dsMarksCls.Tables[0], clsMarksCls, name,
                new object[] { "Code", marksCode, "Name", name });

            DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, this.SourceID,
                string.Format(" RefYearDayUNV = {0} and RefMarksBudSGU = {1} ", curDate, refMarks));

            PumpFactRow02(node, refMarks, 18, "_FB", "FactDate", 1000000000);
            PumpFactRow02(node, refMarks, 20, "_BGVF", "FactDate", 1000000000);
            PumpFactRow02(node, refMarks, 3, "_BSRF", "FactDate", 1000000000);
            PumpFactRow02(node, refMarks, 8, "_BTVF", "FactDate", 1000000000);
            PumpFactRow02(node, refMarks, 7, "_BMF", "FactDate", 1000000000);
            PumpFactRow02(node, refMarks, 21, "_KFB", "FactDate", 1000000000);

            PumpFactRow02(node, refMarks, 21, "_TMPPR", "GrowthRate", 1);
        }

        private void PumpRows03(XmlNodeList xnl)
        {
            foreach (XmlNode node in xnl)
                PumpNode03(node);
        }

        private void PumpXmlFile03(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                PumpRows03(doc.SelectNodes("DATA/*//ROW"));
                UpdateData();
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion 03_rep_22.xml  04_rep_23.xml

        #endregion работа с xml

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            marksCode = 0;
            ProcessFilesTemplate(dir, "02_*.xml", new ProcessFileDelegate(PumpXmlFile02), false);
            ProcessFilesTemplate(dir, "03_*.xml", new ProcessFileDelegate(PumpXmlFile03), false);
            ProcessFilesTemplate(dir, "04_*.xml", new ProcessFileDelegate(PumpXmlFile03), false);
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
