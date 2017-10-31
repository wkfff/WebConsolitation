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
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.Server.DataPumps.DataAccess;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.FST1Pump
{

    // ФСТ - 0001 - Тарифы ЖКХ по субъектам РФ
    public class FST1PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // ФСТ.Показатели (d_FTS_Indicators)
        private IDbDataAdapter daIndicators;
        private DataSet dsIndicators;
        private IClassifier clsIndicators;
        private Dictionary<string, int> cacheIndicators;
        private Dictionary<int, string> cacheIndicatorsIds;
        // ФСТ.Территории (d_FTS_Territory)
        private IDbDataAdapter daTerritory;
        private DataSet dsTerritory;
        private IClassifier clsTerritory;
        private Dictionary<string, int> cacheTerritory;
        private Dictionary<int, string> cacheTerritoryIds;
        // ФСТ.Сферы (d_FTS_Sphere)
        private IDbDataAdapter daSphere;
        private DataSet dsSphere;
        private IClassifier clsSphere;
        private Dictionary<string, int> cacheSphere;
        // ФСТ.Источники финансирования (d_FTS_SourceOfFinance)
        private IDbDataAdapter daSource;
        private DataSet dsSource;
        private IClassifier clsSource;
        private Dictionary<string, int> cacheSource;

        #endregion Классификаторы

        #region Факты

        // ФСТ.ФСТ_ЖКХ_Тарифы (f_FTS_Tariff)
        private IDbDataAdapter daTariff;
        private DataSet dsTariff;
        private IFactTable fctTariff;
        // ФСТ.ФСТ_ЖКХ_Показатели качества (f_FTS_QualityScore)
        private IDbDataAdapter daQualityScore;
        private DataSet dsQualityScore;
        private IFactTable fctQualityScore;
        // ФСТ.ФСТ_ЖКХ_Инвестиции (f_FTS_InvestResources)
        private IDbDataAdapter daInvestResources;
        private DataSet dsInvestResources;
        private IFactTable fctInvestResources;

        #endregion Факты

        private ReportType reportType;
        private List<int> deletedDateList = null;

        #endregion Поля

        #region Константы

        private enum ReportType
        {
            P1,
            P2,
            P3,
            P4,
            Unknown
        }

        #endregion Константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheIndicators, dsIndicators.Tables[0], "Code", "Id");
            FillRowsCache(ref cacheTerritory, dsTerritory.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheSphere, dsSphere.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheSource, dsSource.Tables[0], "Name", "Id");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daIndicators, ref dsIndicators, clsIndicators, string.Empty);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, string.Empty);
            InitDataSet(ref daSphere, ref dsSphere, clsSphere, string.Empty);
            InitDataSet(ref daSource, ref dsSource, clsSource, string.Empty);
            InitFactDataSet(ref daTariff, ref dsTariff, fctTariff);
            InitFactDataSet(ref daQualityScore, ref dsQualityScore, fctQualityScore);
            InitFactDataSet(ref daInvestResources, ref dsInvestResources, fctInvestResources);
            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daTerritory, dsTerritory, clsTerritory);
            UpdateDataSet(daSphere, dsSphere, clsSphere);
            UpdateDataSet(daSource, dsSource, clsSource);
            UpdateDataSet(daTariff, dsTariff, fctTariff);
            UpdateDataSet(daQualityScore, dsQualityScore, fctQualityScore);
            UpdateDataSet(daInvestResources, dsInvestResources, fctInvestResources);
        }

        #region Guids

        private const string D_FTS_INDICATORS_GUID = "c0680618-41e5-4e4d-87f8-c7e821d07799";
        private const string D_FTS_TERRITORY_GUID = "ad3e6adc-ca91-4dec-b381-acb3187c2677";
        private const string D_FTS_SPHERE_GUID = "10d54bed-2fc0-4c28-8ec6-8f03ce8a747f";
        private const string D_FTS_SOURCEOFFINANCE_GUID = "515f5043-0dad-40b5-953c-a35abb5834f4";
        private const string F_FTS_TARIFF_GUID = "91262cfe-37f7-408a-8055-e4e0708f3948";
        private const string F_FTS_QUALITYSCORE_GUID = "f90e46d3-6e76-4e9a-9300-fc0897b9cfb1";
        private const string F_FTS_INVESTRESOURCES_GUID = "dc8dccc4-9743-42f1-896c-c95c63c3dd7a";

        #endregion Guids
        protected override void InitDBObjects()
        {
            clsIndicators = this.Scheme.Classifiers[D_FTS_INDICATORS_GUID];
            clsTerritory = this.Scheme.Classifiers[D_FTS_TERRITORY_GUID];
            clsSphere = this.Scheme.Classifiers[D_FTS_SPHERE_GUID];
            clsSource = this.Scheme.Classifiers[D_FTS_SOURCEOFFINANCE_GUID];

            fctTariff = this.Scheme.FactTables[F_FTS_TARIFF_GUID];
            fctQualityScore = this.Scheme.FactTables[F_FTS_QUALITYSCORE_GUID];
            fctInvestResources = this.Scheme.FactTables[F_FTS_INVESTRESOURCES_GUID];

            this.UsedClassifiers = new IClassifier[] { };
            this.AssociateClassifiersEx = new IClassifier[] { clsTerritory };
            this.UsedFacts = new IFactTable[] { };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsIndicators);
            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsSphere);
            ClearDataSet(ref dsSource);
            ClearDataSet(ref dsTariff);
            ClearDataSet(ref dsQualityScore);
            ClearDataSet(ref dsInvestResources);
        }

        #endregion Работа с базой и кэшами

        #region Общие методы

        private ReportType GetReportType(string filename)
        {
            filename = filename.ToUpper();
            if (filename.StartsWith("P1"))
                return ReportType.P1;
            else if (filename.StartsWith("P2"))
                return ReportType.P2;
            else if (filename.StartsWith("P3"))
                return ReportType.P3;
            else if (filename.StartsWith("P4"))
                return ReportType.P4;
            return ReportType.Unknown;
        }

        private decimal ConvertFactValue(string factValue)
        {
            factValue = factValue.Trim().PadLeft(1, '0').Replace('.', ',');
            return Convert.ToDecimal(factValue);
        }

        #endregion Общие методы

        #region Работа с Xml

        #region работа с классификаторами

        // собираем информацию о классификаторах
        private Dictionary<string, string> GetXmlClsInfo(XmlNodeList nodes)
        {
            Dictionary<string, string> clsInfo = new Dictionary<string, string>();
            for (int i = 0; i < nodes.Count; i++)
            {
                string nodeName = nodes[i].Name;
                if (nodeName.StartsWith("L"))
                    break;

                string nodeValue = nodes[i].FirstChild.Value.Trim();
                switch (nodeName)
                {
                    case "NSRF_ID":
                        if (!clsInfo.ContainsKey("terrCode"))
                            clsInfo.Add("terrCode", nodeValue);
                        break;
                    case "NSRF_NAME":
                        if (!clsInfo.ContainsKey("terrName"))
                            clsInfo.Add("terrName", nodeValue);
                        break;
                    case "SFERA_ID":
                        if (!clsInfo.ContainsKey("sphereCode"))
                            clsInfo.Add("sphereCode", nodeValue);
                        break;
                    case "SFERA_NAME":
                        if (!clsInfo.ContainsKey("sphereName"))
                            clsInfo.Add("sphereName", nodeValue);
                        break;
                    case "IFIN_ID":
                        if (!clsInfo.ContainsKey("sourceCode"))
                            clsInfo.Add("sourceCode", nodeValue);
                        break;
                    case "IFIN_NAME":
                        if (!clsInfo.ContainsKey("sourceName"))
                            clsInfo.Add("sourceName", nodeValue);
                        break;
                    case "YEAR":
                        if (!clsInfo.ContainsKey("year"))
                            clsInfo.Add("year", nodeValue);
                        break;
                    case "PRD2_NAME":
                        if (!clsInfo.ContainsKey("quarter"))
                            clsInfo.Add("quarter", nodeValue);
                        break;
                }
            }
            return clsInfo;
        }

        private int GetQuarterNum(string quarter)
        {
            quarter = quarter.ToUpper();
            if (quarter == "I КВАРТАЛ")
                return 1;
            if (quarter == "II КВАРТАЛ")
                return 2;
            if (quarter == "III КВАРТАЛ")
                return 3;
            if (quarter == "IV КВАРТАЛ")
                return 4;
            return -1;
        }

        private int GetRefPeriod(Dictionary<string, string> clsInfo)
        {
            int refPeriod = Convert.ToInt32(clsInfo["year"].PadLeft(1, '0')) * 10000;
            if (clsInfo.ContainsKey("quarter"))
                refPeriod += GetQuarterNum(clsInfo["quarter"]) + 9990;
            else
                refPeriod += 1;
            if (!deletedDateList.Contains(refPeriod))
            {
                this.UsedFacts = new IFactTable[] { fctTariff };
                DeleteData(string.Format("RefYear = {0}", refPeriod), string.Format("За период: {0}", refPeriod));
                this.UsedFacts = new IFactTable[] { fctInvestResources, fctQualityScore };
                DeleteData(string.Format("RefYearQuarter = {0}", refPeriod), string.Format("За период: {0}", refPeriod));
                this.UsedFacts = new IFactTable[] { };
                deletedDateList.Add(refPeriod);
            }
            return refPeriod;
        }

        private int GetRefIndicator(string code)
        {
            return FindCachedRow(cacheIndicators, code, -1);
        }

        private int PumpTerritoryCls(Dictionary<string, string> clsInfo)
        {
            string code = clsInfo["terrCode"];
            string name = clsInfo["terrName"];
            object[] mapping = new object[] { "Code", code, "Name", name };
            return PumpCachedRow(cacheTerritory, dsTerritory.Tables[0], clsTerritory, name, mapping);
        }

        private int PumpSphereCls(Dictionary<string, string> clsInfo)
        {
            string code = clsInfo["sphereCode"];
            string name = clsInfo["sphereName"];
            object[] mapping = new object[] { "Code", code, "Name", name };
            return PumpCachedRow(cacheSphere, dsSphere.Tables[0], clsSphere, name, mapping);
        }

        private int PumpSourceCls(Dictionary<string, string> clsInfo)
        {
            string code = clsInfo["sourceCode"];
            string name = clsInfo["sourceName"];
            object[] mapping = new object[] { "Code", code, "Name", name };
            return PumpCachedRow(cacheSource, dsSource.Tables[0], clsSource, name, mapping);
        }

        #endregion работа с классификаторами

        #region работа с фактами

        private void PumpFactRow(IDbDataAdapter daFact, DataSet dsFact, object[] mapping)
        {
            PumpRow(dsFact.Tables[0], mapping);
            if (dsFact.Tables[0].Rows.Count >= MAX_DS_RECORDS_AMOUNT)
            {
                UpdateData();
                ClearDataSet(daFact, ref dsFact);
            }
        }

        private void PumpFactRows(XmlNodeList nodes, int refPeriod, int refTerritory)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                string nodeName = nodes[i].Name;
                if (!nodeName.StartsWith("L"))
                    continue;

                int refIndicator = GetRefIndicator(nodeName);
                if (refIndicator == -1)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                        "В классификаторе \"ФСТ.Показатели\" нет записи с кодом {0}.", nodeName));
                    continue;
                }

                decimal fact = ConvertFactValue(nodes[i].FirstChild.Value);
                if (reportType == ReportType.P1)
                {
                    object[] mapping = new object[] { "Fact", fact, "RefYear", refPeriod,
                        "RefTerritory", refTerritory, "RefIndicators", refIndicator };
                    PumpFactRow(daTariff, dsTariff, mapping);
                }
                else if (reportType == ReportType.P2)
                {
                    object[] mapping = new object[] { "Planing", fact, "RefYearQuarter", refPeriod,
                        "RefTerritory", refTerritory, "RefIndicator", refIndicator };
                    PumpFactRow(daQualityScore, dsQualityScore, mapping);
                }
                else if (reportType == ReportType.P4)
                {
                    object[] mapping = new object[] { "Fact", fact, "RefYearQuarter", refPeriod,
                        "RefTerritory", refTerritory, "RefIndicator", refIndicator };
                    PumpFactRow(daQualityScore, dsQualityScore, mapping);
                }
            }
        }

        private void PumpFactRowsInvest(XmlNodeList nodes, int refPeriod, int refTerritory, int refSphere, int refSource)
        {
            int refIndicator = -1;
            decimal planing = 0;
            decimal sponsorship = 0;
            decimal disbursed = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                string nodeName = nodes[i].Name;
                if (nodeName == "L46")
                {
                    refIndicator = GetRefIndicator(nodeName);
                    if (refIndicator == -1)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                            "В классификаторе \"ФСТ.Показатели\" нет записи с кодом {0}.", nodeName));
                        return;
                    }
                    planing = ConvertFactValue(nodes[i].FirstChild.Value);
                }
                else if (nodeName == "L47")
                {
                    sponsorship = ConvertFactValue(nodes[i].FirstChild.Value);
                }
                else if (nodeName == "L48")
                {
                    disbursed = ConvertFactValue(nodes[i].FirstChild.Value);
                }
            }

            object[] mapping = new object[] { "Planing", planing, "Sponsorship", sponsorship,
                "Disbursed", disbursed, "RefYearQuarter", refPeriod, "RefTerritory", refTerritory,
                "RefIndicator", refIndicator, "RefSphere", refSphere, "RefFinSources", refSource };
            PumpFactRow(daInvestResources, dsInvestResources, mapping);
        }

        #endregion работа с фактами

        private void PumpXmlRecord(XmlNodeList record)
        {
            Dictionary<string, string> clsInfo = GetXmlClsInfo(record);
            try
            {
                int refPeriod = GetRefPeriod(clsInfo);
                int refTerritory = PumpTerritoryCls(clsInfo);
                if (reportType != ReportType.P3)
                {
                    PumpFactRows(record, refPeriod, refTerritory);
                }
                else
                {
                    int refSphere = PumpSphereCls(clsInfo);
                    int refSource = PumpSourceCls(clsInfo);
                    PumpFactRowsInvest(record, refPeriod, refTerritory, refSphere, refSource);
                }
            }
            finally
            {
                clsInfo.Clear();
            }
        }

        private void PumpXmlFile(FileInfo file)
        {
            reportType = GetReportType(file.Name);
            if (reportType == ReportType.Unknown)
                return;

            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                XmlNodeList records = doc.GetElementsByTagName("record");
                int totalCount = records.Count;
                for (int i = 0; i < totalCount; i++)
                    try
                    {
                        SetProgress(totalCount, i,
                            string.Format("Обработка файла {0}...", file.FullName),
                            string.Format("Запись {0} из {1}", i + 1, totalCount));
                        PumpXmlRecord(records.Item(i).ChildNodes);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("При обработке записи {0} возникла ошибка ({1})",
                            i + 1, ex.Message), ex);
                    }
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion Работа с Xml

        #region Перекрытые методы закачки

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            if (cacheIndicators.Count <= 1)
                throw new Exception("Не заполнен классификатор \"ФСТ.Показатели\". Данные по этому источнику закачаны не будут.");
            deletedDateList = new List<int>();
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
            deletedDateList.Clear();
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region Обработка данных

        #region Работа с базой и кэшами

        private void FillProcessCaches()
        {
            FillRowsCache(ref cacheIndicatorsIds, dsIndicators.Tables[0], "Id", "Name");
            FillRowsCache(ref cacheTerritoryIds, dsTerritory.Tables[0], "Id", "Name");
        }

        protected override void QueryDataForProcess()
        {
            InitDataSet(ref daIndicators, ref dsIndicators, clsIndicators, string.Empty);
            InitDataSet(ref daTerritory, ref dsTerritory, clsTerritory, string.Empty);
            InitDataSet(ref daTariff, ref dsTariff, fctTariff, string.Format("SourceID = {0}", this.SourceID));
            InitDataSet(ref daQualityScore, ref dsQualityScore, fctQualityScore, string.Format("SourceID = {0}", this.SourceID));
            FillProcessCaches();
        }

        protected override void ProcessFinalizing()
        {
            ClearDataSet(ref dsIndicators);
            ClearDataSet(ref dsTerritory);
            ClearDataSet(ref dsTariff);
            ClearDataSet(ref dsQualityScore);
        }

        protected override void UpdateProcessedData()
        {
            // заглушка
        }

        #endregion Работа с базой и кэшами

        private bool IsNullFactValue(object factValue)
        {
            if ((factValue == null) || (factValue == DBNull.Value))
                return true;
            decimal fact = 0;
            Decimal.TryParse(factValue.ToString(), out fact);
            return (fact == 0);
        }

        private void FindFactNullValuesForTariff()
        {
            int rowsCount = dsTariff.Tables[0].Rows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                DataRow row = dsTariff.Tables[0].Rows[curRow];

                if (!IsNullFactValue(row["Fact"]))
                        continue;

                int refTerritory = Convert.ToInt32(row["RefTerritory"]);
                string territoryName = string.Empty;
                if (cacheTerritoryIds.ContainsKey(refTerritory))
                    territoryName = cacheTerritoryIds[refTerritory];
                
                int refIndicators = Convert.ToInt32(row["RefIndicators"]);
                string indicatorsName = string.Empty;
                if (cacheIndicatorsIds.ContainsKey(refIndicators))
                    indicatorsName = cacheIndicatorsIds[refIndicators];
                
                string message = string.Format("В {0} (ID={1}) за {2} значение '{3} (ID={4})' равно нулю (0).",
                    territoryName, refTerritory, row["RefYear"], indicatorsName, refIndicators);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, message);
            }
        }

        private void FindFactNullValuesForQualityScore()
        {
            int rowsCount = dsQualityScore.Tables[0].Rows.Count;
            for (int curRow = 0; curRow < rowsCount; curRow++)
            {
                DataRow row = dsQualityScore.Tables[0].Rows[curRow];

                if (!IsNullFactValue(row["Fact"]) || !IsNullFactValue(row["Planing"]))
                    continue;

                int refTerritory = Convert.ToInt32(row["RefTerritory"]);
                string territoryName = string.Empty;
                if (cacheTerritoryIds.ContainsKey(refTerritory))
                    territoryName = cacheTerritoryIds[refTerritory];

                int refIndicators = Convert.ToInt32(row["RefIndicator"]);
                string indicatorsName = string.Empty;
                if (cacheIndicatorsIds.ContainsKey(refIndicators))
                    indicatorsName = cacheIndicatorsIds[refIndicators];

                string message = string.Format("В {0} (ID={1}) за {2} значение '{3} (ID={4})' равно нулю (0).",
                    territoryName, refTerritory, row["RefYearQuarter"], indicatorsName, refIndicators);
                WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeInformation, message);
            }
        }

        protected override void ProcessDataSource()
        {
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                "Старт проверки нулевых значений в таблице {0}", fctTariff.OlapName));
            FindFactNullValuesForTariff();
            WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                "Старт проверки нулевых значений в таблице {0}", fctQualityScore.OlapName));
            FindFactNullValuesForQualityScore();
        }

        protected override void DirectProcessData()
        {
            ProcessDataSourcesTemplate("Выполняется проверка таблиц фактов на нулевые значения.");
        }

        #endregion Обработка данных

    }
}
