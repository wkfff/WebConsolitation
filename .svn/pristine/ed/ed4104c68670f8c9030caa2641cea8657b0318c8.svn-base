using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Globalization;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.RKC1Pump
{
    public class RKC1PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // РКЦ.Виды платежей (d_RKC_Types)
        private IDbDataAdapter daRkcTypes;
        private DataSet dsRkcTypes;
        private IClassifier clsRkcTypes;
        private Dictionary<string, int> cacheRkcTypes = null;
        // РКЦ.Управляющие компании (d_RKC_Company)
        private IDbDataAdapter daRkcCompany;
        private DataSet dsRkcCompany;
        private IClassifier clsRkcCompany;
        private Dictionary<string, int> cacheRkcCompany = null;
        // РКЦ.Квартиры (d_RKC_Apartments)
        private IDbDataAdapter daRkcApartments;
        private DataSet dsRkcApartments;
        private IClassifier clsRkcApartments;
        private Dictionary<string, int> cacheRkcApartments = null;
        // Территории.РФ (d_Territory_RF)
        private IDbDataAdapter daTerr;
        private DataSet dsTerr;
        private IClassifier clsTerr;
        private Dictionary<string, int> cacheTerr = null;

        #endregion Классификаторы

        #region Факты

        // РКЦ.Тарифы ЖКХ (f_RKC_TariffGKH)
        private IDbDataAdapter daRKCFact;
        private DataSet dsRKCFact;
        private IFactTable fctRKCFact;

        #endregion Факты

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheRkcTypes, dsRkcTypes.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheRkcCompany, dsRkcCompany.Tables[0], "Name", "Id");
            FillRowsCache(ref cacheRkcApartments, dsRkcApartments.Tables[0], "Guid", "Id");
            FillRowsCache(ref cacheTerr, dsTerr.Tables[0], "Name", "Id");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daRkcTypes, ref dsRkcTypes, clsRkcTypes, false, string.Empty, string.Empty);
            InitDataSet(ref daRkcCompany, ref dsRkcCompany, clsRkcCompany, false, string.Empty, string.Empty);
            InitDataSet(ref daRkcApartments, ref dsRkcApartments, clsRkcApartments, false, string.Empty, string.Empty);
            InitDataSet(ref daTerr, ref dsTerr, clsTerr, false, string.Empty, string.Empty);

            InitFactDataSet(ref daRKCFact, ref dsRKCFact, fctRKCFact);

            FillCaches();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daRkcTypes, dsRkcTypes, clsRkcTypes);
            UpdateDataSet(daRkcCompany, dsRkcCompany, clsRkcCompany);
            UpdateDataSet(daRkcApartments, dsRkcApartments, clsRkcApartments);

            UpdateDataSet(daRKCFact, dsRKCFact, fctRKCFact);
        }

        private const string TYPES_GUID = "df0527da-ec25-4ef2-88b4-99a229a85604";
        private const string COMPANY_GUID = "81d9f097-75b0-436f-bc04-fcdfce875d0c";
        private const string APARTMENTS_GUID = "e43b206b-e4e8-4a00-b024-b80a6ce84645";
        private const string TERR_GUID = "66b9a66d-85ca-41de-910e-f9e6cb483960";
        private const string FACT_GUID = "08c57958-1271-4256-84eb-42d74a60224f";
        protected override void InitDBObjects()
        {
            clsRkcTypes = this.Scheme.Classifiers[TYPES_GUID];
            clsRkcCompany = this.Scheme.Classifiers[COMPANY_GUID];
            clsRkcApartments = this.Scheme.Classifiers[APARTMENTS_GUID];
            clsTerr = this.Scheme.Classifiers[TERR_GUID];
            this.UsedClassifiers = new IClassifier[] { };

            fctRKCFact = this.Scheme.FactTables[FACT_GUID];
            this.UsedFacts = new IFactTable[] { fctRKCFact };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsRKCFact);

            ClearDataSet(ref dsRkcTypes);
            ClearDataSet(ref dsRkcCompany);
            ClearDataSet(ref dsRkcApartments);
            ClearDataSet(ref dsTerr);
        }

        #endregion Работа с базой и кэшами

        #region работа с xml

        private string GetNodeValue(XmlNode node, string nodeName)
        {
            if (node.SelectSingleNode(nodeName) != null)
                return node.SelectSingleNode(nodeName).InnerText.Trim();
            return string.Empty;
        }

        private void PumpFactRow(XmlNode node, int refOrg, int refApartment, int curDate, int refTerritory)
        {
            string code = XmlHelper.GetStringAttrValue(node, "code", string.Empty);
            string name = XmlHelper.GetStringAttrValue(node, "name", string.Empty);
            string key = string.Format("{0}|{1}", code, name);
            int refTypes = PumpCachedRow(cacheRkcTypes, dsRkcTypes.Tables[0], clsRkcTypes, key,
                new object[] { "Code", code, "Name", name });

            string serviceType = GetNodeValue(node, "type_service");
            string normUnit = GetNodeValue(node, "norm_unit");
            decimal normValue = Convert.ToDecimal(GetNodeValue(node, "norm_value").PadLeft(1, '0'));
            string tariffUnit = GetNodeValue(node, "tariff_unit");
            decimal tariffValue = Convert.ToDecimal(GetNodeValue(node, "tariff_value").PadLeft(1, '0'));
            decimal amount = Convert.ToDecimal(GetNodeValue(node, "amount").PadLeft(1, '0'));
            object[] mapping = new object[] { "serviceType", serviceType, "normUnit", normUnit, "normValue", normValue, 
                "tariffUnit", tariffUnit, "tariffValue", tariffValue, "amount", amount, "RefYearUNV", curDate, 
                "RefTypes", refTypes, "RefCompany", refOrg, "RefApartments", refApartment, "RefTerritory", refTerritory};

            PumpRow(dsRKCFact.Tables[0], mapping);
        }

        private void PumpFactRows(XmlNodeList xnl, int refOrg, int refApartment, int curDate, int refTerritory)
        {
            foreach (XmlNode node in xnl)
                PumpFactRow(node, refOrg, refApartment, curDate, refTerritory);
        }

        private int PumpOrg(XmlNode node)
        {
            string org = XmlHelper.GetStringAttrValue(node, "org", string.Empty);
            return PumpCachedRow(cacheRkcCompany, dsRkcCompany.Tables[0], clsRkcCompany, org,
                new object[] { "Code", 0, "Name", org });
        }

        private int PumpApartment(XmlNode node)
        {
            string guid = XmlHelper.GetStringAttrValue(node, "inr", string.Empty);
            string area = XmlHelper.GetStringAttrValue(node, "area", string.Empty);
            string qPers = XmlHelper.GetStringAttrValue(node, "q_pers", string.Empty);
            string gkh_type = XmlHelper.GetStringAttrValue(node, "gkh_type", string.Empty);
            return PumpCachedRow(cacheRkcApartments, dsRkcApartments.Tables[0], clsRkcApartments, guid,
                new object[] { "Guid", guid, "area", area, "NumberResidents", qPers, "TypesOfLandscaping", gkh_type });
        }

        private int GetTerritory(XmlNode node)
        {
            string name = XmlHelper.GetStringAttrValue(node, "mo", string.Empty);
            return cacheTerr[name];
        }

        private int GetDate(XmlNode node)
        {
            int year = XmlHelper.GetIntAttrValue(node, "year", 0);
            string monthStr = XmlHelper.GetStringAttrValue(node, "month", "0");
            int monthInt = DateTime.ParseExact(monthStr.ToLower(), "MMMM", new CultureInfo("ru-RU", false)).Month;
            return year * 10000 + monthInt * 100;
        }

        private void PumpNode(XmlNode node)
        {
            int curDate = GetDate(node);
            int refOrg = PumpOrg(node);
            int refApartment = PumpApartment(node);
            int refTerritory = GetTerritory(node);
            PumpFactRows(node.SelectNodes("payment"), refOrg, refApartment, curDate, refTerritory);
            PumpFactRows(node.SelectNodes("payment_other"), refOrg, refApartment, curDate, refTerritory);
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
                PumpRows(doc.SelectNodes("records/record"));
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
