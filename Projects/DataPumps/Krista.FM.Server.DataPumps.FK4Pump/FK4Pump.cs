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

namespace Krista.FM.Server.DataPumps.FK4Pump
{
    public class FK4PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Показатели.Остатки средств (d_Marks_RestMeans)
        private IDbDataAdapter daMarksCls;
        private DataSet dsMarksCls;
        private IClassifier clsMarksCls;
        private Dictionary<string, int> cacheMarksCls = null;

        #endregion Классификаторы

        #region Факты

        // Показатели.ФК_Остатки средств бюджета (f_Marks_FKRestMeans)
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

        private const string MARKS_CLS_GUID = "206833f0-465e-4042-a0ec-5a87917ec76f";
        private const string MARKS_FACT_GUID = "c6ab8781-ec77-44d2-a035-8818478d1baa";
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

        private void CheckDate(XmlNode dateNode, string fileName)
        {
            int fileDate = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileName).Split('-')[1].Trim());
            curDate = Convert.ToInt32(dateNode.InnerText);
            if (fileDate != curDate)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Дата заголовка файла '{0}' не совпадает с датой данных", fileName));
            if (!this.DeleteEarlierData)
                DirectDeleteFactData(new IFactTable[] { fctMarksFact }, -1, this.SourceID, string.Format(" RefYearDayUNV = {0} ", curDate));
        }

        private void PumpNode(XmlNode node)
        {
            marksCode += 1;
            string id = node.SelectSingleNode("ID").InnerText;
            string factor = node.SelectSingleNode("FACTOR").InnerText;
            string name = string.Format("{0}_{1}", id.Replace(".", string.Empty).Replace("E9", string.Empty).PadRight(4, '0'), factor);
            int refMarks = PumpCachedRow(cacheMarksCls, dsMarksCls.Tables[0], clsMarksCls, name,
                new object[] { "Code", marksCode, "Name", name, "ShortName", factor });

            decimal factStart = Convert.ToDecimal(node.SelectSingleNode("ABS_VAL_PDATE_YEAR").InnerText.Replace(".", ",")) * 1000000000;
            decimal factDate = Convert.ToDecimal(node.SelectSingleNode("ABS_VAL_PDATE").InnerText.Replace(".", ",")) * 1000000000;
            decimal factChange = Convert.ToDecimal(node.SelectSingleNode("PRIROST_POK").InnerText.Replace(".", ",")) * 1000000000;

            if (factChange != factDate - factStart)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning,
                    string.Format("Обнаружено неккоректное значение поля 'Изменение с начала года' - {0}", factChange));

            PumpRow(dsMarksFact.Tables[0], new object[] { "FactStart", factStart, "FactDate", factDate, 
                "FactChange", factChange, "RefMarksRest", refMarks, "RefYearDayUNV", curDate });
        }

        private void PumpRows(XmlNodeList xnl)
        {
            foreach (XmlNode node in xnl)
                PumpNode(node);
        }

        private void PumpXmlFile(FileInfo file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                CheckDate(doc.SelectSingleNode("DATA/DS_DATE/DS_DATE_ROW/P_DATE"), file.Name);
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
            marksCode = 0;
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataYTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
