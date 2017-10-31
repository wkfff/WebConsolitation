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

namespace Krista.FM.Server.DataPumps.FO25Pump
{
    // фо 25 - форма 14
    public class FO25PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // Расходы.ФО_Форма14 (d.R.FOF14)
        private IDbDataAdapter daOutcomesCls;
        private DataSet dsOutcomesCls;
        private IClassifier clsOutcomes;
        private Dictionary<string, int> cacheOutcomes = null;
        // Должности.ФО_Форма14 (d.Post.FOF14)
        private IDbDataAdapter daPostCls;
        private DataSet dsPostCls;
        private IClassifier clsPost;
        private Dictionary<string, int> cachePost = null;
        // Показатели.ФО_Форма14 (d.Marks.FOF14)
        private IDbDataAdapter daMarksCls;
        private DataSet dsMarksCls;
        private IClassifier clsMarks;
        private Dictionary<string, int> cacheMarks = null;
        // Показатели.ФО_Форма14_Автомобили (d.Marks.FOF14auto)
        private IDbDataAdapter daMarksAutoCls;
        private DataSet dsMarksAutoCls;
        private IClassifier clsMarksAuto;
        private Dictionary<string, int> cacheMarksAuto = null;
        // ФКР.ФО_Форма14 (d.FKR.FOF14)
        private IDbDataAdapter daFkr;
        private DataSet dsFkr;
        private IClassifier clsFkr;
        private Dictionary<string, int> cacheFkr = null;
        // Районы.ФО_Форма14 (d.Regions.FOF14)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private Dictionary<string, int> cacheRegions = null;
        // Районы.Служебный для закачки СКИФ
        private IDbDataAdapter daRegionsForPump;
        private DataSet dsRegionsForPump;
        private IClassifier clsRegionsForPump;
        private Dictionary<string, int> regionsForPumpCache = null;

        #endregion Классификаторы

        #region Факты

        // Расходы.ФО_Форма14_Расходы (f.R.FOF14)
        private IDbDataAdapter daOutcomes;
        private DataSet dsOutcomes;
        private IFactTable fctOutcomes;
        // Должности.ФО_Форма14_Должности (f.Post.FOF14)
        private IDbDataAdapter daPost;
        private DataSet dsPost;
        private IFactTable fctPost;
        // Показатели.ФО_Форма14_Справка (f.Marks.FOF14)
        private IDbDataAdapter daMarks;
        private DataSet dsMarks;
        private IFactTable fctMarks;
        // Показатели.ФО_Форма14_Автомобили (f.Marks.FOF14auto)
        private IDbDataAdapter daMarksAuto;
        private DataSet dsMarksAuto;
        private IFactTable fctMarksAuto;

        #endregion Факты

        private int clsSourceId;
        private int regForPumpSourceID;
        // значение 1 - форма 14, значение 2 - форма 14 МО
        private int codeInc;
        private bool isFormat2007;

        #endregion Поля

        #region константы

        private enum Block
        {
            bOutcomes,
            bPost,
            bMarks,
            bMarksAuto
        }

        #endregion константы

        #region Закачка данных

        #region Работа с базой и кэшами

        private void FillCaches()
        {
            FillRowsCache(ref cacheOutcomes, dsOutcomesCls.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cachePost, dsPostCls.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheMarks, dsMarksCls.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheMarksAuto, dsMarksAutoCls.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref cacheFkr, dsFkr.Tables[0], new string[] { "CodeStr", "Name" }, "|", "Id");
            FillRowsCache(ref cacheRegions, dsRegions.Tables[0], new string[] { "Code", "Name" }, "|", "Id");
            FillRowsCache(ref regionsForPumpCache, dsRegionsForPump.Tables[0], new string[] { "Codestr", "Name" }, "|", "RefDocType");
        }

        private void InitRegForPump()
        {
            regForPumpSourceID = AddDataSource("ФО", "0006", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            object isLocked = this.DB.ExecQuery(string.Format("select locked FROM dataSources where Id = {0}", regForPumpSourceID),
                    QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if (Convert.ToInt32(isLocked) != 0)
                throw new Exception(string.Format("Источник ФО 0006, год {0} (Id = {1}) заблокирован от изменений.",
                    this.DataSource.Year, regForPumpSourceID));
            InitClsDataSet(ref daRegionsForPump, ref dsRegionsForPump, clsRegionsForPump, false, string.Empty, regForPumpSourceID);
        }

        protected override void QueryData()
        {
            clsSourceId = AddDataSource("ФО", "0025", ParamKindTypes.Year, string.Empty, this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
            InitClsDataSet(ref daOutcomesCls, ref dsOutcomesCls, clsOutcomes, false, string.Empty, clsSourceId);
            InitClsDataSet(ref daPostCls, ref dsPostCls, clsPost, false, string.Empty, clsSourceId);
            InitClsDataSet(ref daMarksCls, ref dsMarksCls, clsMarks, false, string.Empty, clsSourceId);
            InitClsDataSet(ref daMarksAutoCls, ref dsMarksAutoCls, clsMarksAuto, false, string.Empty, clsSourceId);
            InitClsDataSet(ref daFkr, ref dsFkr, clsFkr, false, string.Empty, clsSourceId);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty, clsSourceId);
            InitRegForPump();

            InitFactDataSet(ref daOutcomes, ref dsOutcomes, fctOutcomes);
            InitFactDataSet(ref daPost, ref dsPost, fctPost);
            InitFactDataSet(ref daMarks, ref dsMarks, fctMarks);
            InitFactDataSet(ref daMarksAuto, ref dsMarksAuto, fctMarksAuto);

            FillCaches();

            isFormat2007 = (this.DataSource.Year * 100 + this.DataSource.Quarter >= 200703);
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daOutcomesCls, dsOutcomesCls, clsOutcomes);
            UpdateDataSet(daPostCls, dsPostCls, clsPost);
            UpdateDataSet(daMarksCls, dsMarksCls, clsMarks);
            UpdateDataSet(daMarksAutoCls, dsMarksAutoCls, clsMarksAuto);
            UpdateDataSet(daFkr, dsFkr, clsFkr);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daRegionsForPump, dsRegionsForPump, clsRegionsForPump);

            UpdateDataSet(daOutcomes, dsOutcomes, fctOutcomes);
            UpdateDataSet(daPost, dsPost, fctPost);
            UpdateDataSet(daMarks, dsMarks, fctMarks);
            UpdateDataSet(daMarksAuto, dsMarksAuto, fctMarksAuto);
        }

        #region guid

        private const string D_R_FOF14_GUID = "0e68d052-406e-46c2-9273-adbac8e4292d";
        private const string D_POST_FOF14_GUID = "e8d918fc-3254-43da-ac69-3766e40d0f4c";
        private const string D_MARKS_FOF14_GUID = "c6cb8bf3-72e4-48ad-b99b-f0179a88113a";
        private const string D_MARKS_FOF14_AUTO_GUID = "caaa0b18-f6db-40b9-b34d-8cb9fac98edf";
        private const string D_FKR_FOF14_GUID = "edf1ce49-e367-4270-bd48-c3cda7bc91ef";
        private const string D_REGIONS_FOF14_GUID = "f722a320-f377-45d3-8219-6e7ef7b9659e";
        private const string D_REGIONS_FOR_PUMP_SKIF_GUID = "e9a95119-21f1-43d8-8dc2-8d4af7c195d0";

        private const string F_R_FOF14_GUID = "7365d822-905a-4218-bc8e-b0c65f724111";
        private const string F_POST_FOF14_GUID = "bd30cefa-8e53-4c76-9684-60e425cc4246";
        private const string F_MARKS_FOF14_GUID = "0deacd56-f043-48a3-95e8-4bc02eb81088";
        private const string F_MARKS_FOF14_AUTO_GUID = "2dbcdf2a-1834-4f0a-b1dd-fb83e497613f";

        #endregion guid
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] { };

            clsMarks = this.Scheme.Classifiers[D_MARKS_FOF14_GUID];
            clsMarksAuto = this.Scheme.Classifiers[D_MARKS_FOF14_AUTO_GUID];
            clsRegionsForPump = this.Scheme.Classifiers[D_REGIONS_FOR_PUMP_SKIF_GUID];

            this.AssociateClassifiers = new IClassifier[] {
                clsOutcomes = this.Scheme.Classifiers[D_R_FOF14_GUID],
                clsPost = this.Scheme.Classifiers[D_POST_FOF14_GUID],
                clsFkr = this.Scheme.Classifiers[D_FKR_FOF14_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_FOF14_GUID] };

            this.UsedFacts = new IFactTable[] { 
                fctOutcomes = this.Scheme.FactTables[F_R_FOF14_GUID],
                fctPost = this.Scheme.FactTables[F_POST_FOF14_GUID],
                fctMarks = this.Scheme.FactTables[F_MARKS_FOF14_GUID],
                fctMarksAuto = this.Scheme.FactTables[F_MARKS_FOF14_AUTO_GUID] };
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsOutcomes);
            ClearDataSet(ref dsPost);
            ClearDataSet(ref dsMarks);
            ClearDataSet(ref dsMarksAuto);

            ClearDataSet(ref dsOutcomesCls);
            ClearDataSet(ref dsPostCls);
            ClearDataSet(ref dsMarksCls);
            ClearDataSet(ref dsMarksAutoCls);
            ClearDataSet(ref dsFkr);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsRegionsForPump);
        }

        #endregion Работа с базой и кэшами

        #region общие методы

        private string GetConfigParamName(Block block)
        {
            switch (block)
            {
                case Block.bOutcomes:
                    return "ucbOutcomes";
                case Block.bPost:
                    return "ucbPost";
                case Block.bMarks:
                    return "ucbMarks";
                case Block.bMarksAuto:
                    return "ucbMarksAuto";
                default:
                    return string.Empty;
            }
        }

        private bool ToPumpBlock(Block block)
        {
            string configParamName = GetConfigParamName(block);
            return (Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, configParamName, "False")));
        }

        #endregion общие методы

        #region работа с xml

        #region классификаторы

        private void PumpCommonCls(XmlNodeList clsNodes, DataTable dt, IClassifier cls, 
            Dictionary<string, int> cache, int[] minMax)
        {
            foreach (XmlNode clsNode in clsNodes)
            {
                string codeStr = clsNode.Attributes["Code"].Value.PadLeft(3, '0');
                int code = Convert.ToInt32(clsNode.Attributes["Code"].Value);
                if ((code < minMax[0]) || (code > minMax[1]))
                    continue;
                code = Convert.ToInt32(string.Format("{0}{1}", codeInc, codeStr));
                string name = clsNode.Attributes["Name"].Value.ToString().Replace("\n", " ");
                string key = string.Format("{0}|{1}", code, name);
                object[] mapping = new object[] { "Code", code, "Name", name, "SourceId", clsSourceId };
                PumpCachedRow(cache, dt, cls, key, mapping);
            }
        }

        private void PumpOutcomesCls(XmlNodeList clsNodes)
        {
            int[] minMax = new int[] { 10, 100 };
            if (codeInc == 2)
                minMax = new int[] { 10, 80 };
            PumpCommonCls(clsNodes, dsOutcomesCls.Tables[0], clsOutcomes, cacheOutcomes, minMax);
        }

        private void PumpPostCls(XmlNodeList clsNodes)
        {
            int[] minMax = new int[] { 200, 310 };
            if (codeInc == 2)
                minMax = new int[] { 200, 290 };
            PumpCommonCls(clsNodes, dsPostCls.Tables[0], clsPost, cachePost, minMax);
        }

        private void PumpMarksCls(XmlNodeList clsNodes)
        {
            int[] minMax = new int[] { 320, 449 };
            if (codeInc == 2)
                minMax = new int[] { 300, 459 };
            PumpCommonCls(clsNodes, dsMarksCls.Tables[0], clsMarks, cacheMarks, minMax);
        }

        private void PumpMarksAutoCls(XmlNodeList clsNodes)
        {
            int[] minMax = new int[] { 450, 460 };
            if (codeInc == 2)
                minMax = new int[] { 460, 470 };
            PumpCommonCls(clsNodes, dsMarksAutoCls.Tables[0], clsMarksAuto, cacheMarksAuto, minMax);
        }

        private void PumpCls(XmlNodeList clsNodes)
        {
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomesCls(clsNodes);
            if (ToPumpBlock(Block.bPost))
                PumpPostCls(clsNodes);
            if (ToPumpBlock(Block.bMarks))
                PumpMarksCls(clsNodes);
            if (ToPumpBlock(Block.bMarksAuto))
                PumpMarksAutoCls(clsNodes);
        }

        private int PumpFkrCode(string codeStr, string name)
        {
            long code = Convert.ToInt64(codeStr);
            string key = string.Format("{0}|{1}", code, name);
            string fkr = codeStr.Substring(0, 4);
            string kcsr = codeStr.Substring(4, 7);
            string kvr = codeStr.Substring(11, 3);
            string rowKey = codeStr.Substring(14, 5);
            object[] mapping = new object[] { "CodeStr", code, "Name", name, "FKR", fkr, "KCSR", kcsr, 
                    "KVR", kvr, "KeyRow", rowKey, "SourceId", clsSourceId };
            return PumpCachedRow(cacheFkr, dsFkr.Tables[0], clsFkr, key, mapping);
        }

        private const string CATALOG_CODE_FKR = "РзПрЦСРВРСтрока";
        private void PumpFkr(XmlDocument doc)
        {
            string xPath = string.Format("RootXml/NSI/Catalogs/Catalog[@Code=\"{0}\"]", CATALOG_CODE_FKR);
            XmlNode catalogNode = doc.SelectSingleNode(xPath);
            if (catalogNode == null)
                return;
            XmlNodeList clsNodes = catalogNode.ChildNodes;
            foreach (XmlNode clsNode in clsNodes)
            {
                string codeStr = clsNode.Attributes["Code"].Value.ToString();
                string name = clsNode.Attributes["Name"].Value.ToString();
                if (name == string.Empty)
                    name = constDefaultClsName;
                PumpFkrCode(codeStr, name);
            }
        }

        private void PumpClsCatalog(XmlDocument doc, string catalogCode)
        {
            string xPath = string.Format("RootXml/NSI/Catalogs/Catalog[@Code=\"{0}\"]", catalogCode);
            XmlNode clsNode = doc.SelectSingleNode(xPath);
            if (clsNode != null)
                PumpCls(clsNode.ChildNodes);
        }

        private const string CATALOG_CODE = "ЧРГО";
        private const string CATALOG_CODE_QUARTER = "ЧРГОкв";
        private void PumpClsDoc(XmlDocument doc)
        {
            codeInc = 1;
            PumpClsCatalog(doc, CATALOG_CODE);
            codeInc = 2;
            PumpClsCatalog(doc, CATALOG_CODE_QUARTER);
            PumpFkr(doc);
            UpdateData();
        }

        #endregion классификаторы

        #region факты

        private bool PumpRegionForPump(string code, string name)
        {
            code = code.PadLeft(10, '0');
            string key = string.Format("{0}|{1}", code, name);
            if (!regionsForPumpCache.ContainsKey(key))
            {
                object[] mapping = new object[] { "CodeStr", code, "Name", name, "RefDocType", 1, "SOURCEID", regForPumpSourceID };
                PumpCachedRow(regionsForPumpCache, dsRegionsForPump.Tables[0], clsRegionsForPump, key, mapping);
                return false;
            }
            return true;
        }

        private int PumpRegion(XmlNode regionNode)
        {
            int code = Convert.ToInt32(regionNode.Attributes["Code"].Value);
            string name = regionNode.Attributes["Name"].Value;
            string key = string.Format("{0}|{1}", code, name);
            if (!PumpRegionForPump(code.ToString(), name))
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "Классификатор Районы.Служебный (SOURCEID {0}) имеет запись (код: {1}; имя: {2}) с неуказанным типом района. " +
                    "Необходимо установить значения поля \"ТипДокумента.СКИФ\" и запустить этап обработки.", regForPumpSourceID, code, name));
            string classCode = regionNode.Attributes["ClassCode"].Value;
            string className = regionNode.Attributes["ClassName"].Value;
            object[] mapping = new object[] { "Code", code, "Name", name, "ClassCode", classCode, 
                "ClassName", className, "SourceId", clsSourceId, "RefDocType", 1 };
            return PumpCachedRow(cacheRegions, dsRegions.Tables[0], clsRegions, key, mapping);
        }

        private int GetClsRef(long clsCode, DataTable dtCls, IClassifier cls)
        {
            DataRow[] row = dtCls.Select(string.Format("Code = {0}", clsCode));
            if (row.GetLength(0) == 0)
                return PumpRow(dtCls, cls, new object[] { "Code", clsCode, "Name", constDefaultClsName, "SourceId", clsSourceId });
            else
                return Convert.ToInt32(row[0]["Id"]);
        }

        private long GetClsCode(XmlNode dataNode)
        {
            XmlNode clsNode = dataNode.Attributes.GetNamedItem("ЧРГО");
            if (clsNode != null)
                return Convert.ToInt64(clsNode.Value);
            else
                return Convert.ToInt64(dataNode.Attributes["ЧРГОкв"].Value);
        }

        private int GetDateRef()
        {
            int dateRef = this.DataSource.Year * 10000 + 9990 + this.DataSource.Quarter;
            return dateRef;
        }

        private object[] GetSums(XmlNode dataNode, object[] sumMapping, decimal sumMultiplier)
        {
            object[] sums = new object[] { };
            for (int i = 0; i < sumMapping.GetLength(0); i += 2)
            {
                string xPath = string.Format("Px[@Num=\"{0}\"]", sumMapping[i + 1].ToString());
                XmlNode sumNode = dataNode.SelectSingleNode(xPath);
                if (sumNode == null)
                    continue;
                decimal value = Convert.ToDecimal(sumNode.Attributes["Value"].Value.Replace(".", ",")) * sumMultiplier;
                sums = (object[])CommonRoutines.ConcatArrays(sums, new object[] { sumMapping[i].ToString(), value });
            }
            return sums;
        }

        private decimal GetSumMultiplier(long clsCode)
        {
            decimal sumMultiplier = 1;
            if (isFormat2007)
            {
                if (codeInc == 1)
                {
                    // форма 14
                    if ((clsCode >= 10) && (clsCode <= 100))
                        sumMultiplier = 1000;
                    if ((clsCode >= 400) && (clsCode <= 444))
                        sumMultiplier = 1000;
                }
                else
                {
                    // форма 14 мо
                    if ((clsCode >= 10) && (clsCode <= 80))
                        sumMultiplier = 1000;
                    if ((clsCode >= 400) && (clsCode <= 450))
                        sumMultiplier = 1000;
                }
            }
            else
            {
                if ((clsCode >= 10) && (clsCode <= 100))
                    sumMultiplier = 1000;
                if ((clsCode >= 400) && (clsCode <= 444))
                    sumMultiplier = 1000;
            }
            return sumMultiplier;
        }

        private void PumpDataNode(XmlNode dataNode, DataTable dtCls, IClassifier cls, 
            string refClsName, object[] sumMapping, int refRegions, DataTable dtFact, int refFkr)
        {
            string clsCodeStr = Convert.ToString(GetClsCode(dataNode)).PadLeft(3, '0');
            long clsCode = Convert.ToInt64(string.Format("{0}{1}", codeInc, clsCodeStr));
            decimal sumMultiplier = GetSumMultiplier(Convert.ToInt32(clsCodeStr));
            int clsRef = GetClsRef(clsCode, dtCls, cls);
            int dateRef = GetDateRef();
            object[] clsMapping = new object[] { "RefRegions", refRegions, "RefFKR", refFkr, 
                refClsName, clsRef, "RefYearDayUNV", dateRef };
            object[] sums = GetSums(dataNode, sumMapping, sumMultiplier);
            object[] mapping = (object[])CommonRoutines.ConcatArrays(clsMapping, sums);
            PumpRow(dtFact, mapping);
        }

        private int GetFkrRef(string fkrCode)
        {
            DataRow[] row = dsFkr.Tables[0].Select(string.Format("CodeStr = '{0}'", fkrCode.TrimStart('0').PadLeft(1, '0')));
            if (row.GetLength(0) == 0)
                return PumpFkrCode(fkrCode, constDefaultClsName);
            else
                return Convert.ToInt32(row[0]["Id"]);
        }

        private void PumpFactNode(XmlNode factNode, DataTable dtCls, IClassifier cls, 
            string refClsName, object[] sumMapping, int refRegions, DataTable dtFact)
        {
            int refFkr = GetFkrRef(factNode.Attributes["РзПрЦСРВРСтрока"].Value);
            XmlNodeList dataNodes = factNode.SelectNodes("Data");
            foreach (XmlNode dataNode in dataNodes)
                PumpDataNode(dataNode, dtCls, cls, refClsName, sumMapping, refRegions, dtFact, refFkr);
        }

        private void PumpFactForm(XmlNode regionNode, string form, DataTable dtCls, IClassifier cls, 
            string refClsName, object[] sumMapping, int refRegions, DataTable dtFact)
        {
            string xPath = string.Format("Form[(@Code=\"{0}\")]", form);
            XmlNode formNode = regionNode.SelectSingleNode(xPath);
            if (formNode == null)
                return;
            XmlNodeList factNodes = formNode.SelectNodes("Document");
            foreach (XmlNode factNode in factNodes)
                PumpFactNode(factNode, dtCls, cls, refClsName, sumMapping, refRegions, dtFact);
        }

        private void PumpOutcomes(XmlNode regionNode, int refRegion)
        {
            codeInc = 1;
            PumpFactForm(regionNode, "01401", dsOutcomesCls.Tables[0], clsOutcomes,
                "RefR", new object[] { "AssignRep", "1", "PerformRep", "2" }, refRegion, dsOutcomes.Tables[0]);
            codeInc = 2;
            PumpFactForm(regionNode, "014mo01", dsOutcomesCls.Tables[0], clsOutcomes,
                "RefR", new object[] { "AssignRep", "1", "PerformRep", "2" }, refRegion, dsOutcomes.Tables[0]);
        }

        private void PumpPost(XmlNode regionNode, int refRegion)
        {
            codeInc = 1;
            PumpFactForm(regionNode, "01402", dsPostCls.Tables[0], clsPost, "RefPost", 
                new object[] { "PlanRep", "1", "FactRep", "2", "MCountRep", "3" }, refRegion, dsPost.Tables[0]);
            codeInc = 2;
            PumpFactForm(regionNode, "014mo02", dsPostCls.Tables[0], clsPost, "RefPost",
                new object[] { "PlanRep", "1", "FactRep", "2", "MCountRep", "3" }, refRegion, dsPost.Tables[0]);
        }

        private void PumpMarks(XmlNode regionNode, int refRegion)
        {
            codeInc = 1;
            PumpFactForm(regionNode, "01403", dsMarksCls.Tables[0], clsMarks, "RefMarks",
                new object[] { "FactRepot", "1" }, refRegion, dsMarks.Tables[0]);
            codeInc = 2;
            PumpFactForm(regionNode, "014mo03", dsMarksCls.Tables[0], clsMarks, "RefMarks",
                new object[] { "FactRepot", "1" }, refRegion, dsMarks.Tables[0]);
        }

        private void PumpMarksAuto(XmlNode regionNode, int refRegion)
        {
            codeInc = 1;
            PumpFactForm(regionNode, "01404", dsMarksAutoCls.Tables[0], clsMarksAuto,
                "RefMarksFOF14auto", new object[] { "InDateRep", "1", "MidPerRep", "2" }, refRegion, dsMarksAuto.Tables[0]);
            codeInc = 2;
            PumpFactForm(regionNode, "014mo04", dsMarksAutoCls.Tables[0], clsMarksAuto,
                "RefMarksFOF14auto", new object[] { "InDateRep", "1", "MidPerRep", "2" }, refRegion, dsMarksAuto.Tables[0]);
        }

        private void PumpRegionFact(XmlNode regionNode, int refRegion)
        {
            if (ToPumpBlock(Block.bOutcomes))
                PumpOutcomes(regionNode, refRegion);
            if (ToPumpBlock(Block.bPost))
                PumpPost(regionNode, refRegion);
            if (ToPumpBlock(Block.bMarks))
                PumpMarks(regionNode, refRegion);
            if (ToPumpBlock(Block.bMarksAuto))
                PumpMarksAuto(regionNode, refRegion);
        }

        private void PumpFactDoc(XmlDocument doc)
        {
            // идем по районам
            string xPath = string.Format("RootXml/Report/Period/Source");
            XmlNodeList regionNodes = doc.SelectNodes(xPath);
            foreach (XmlNode regionNode in regionNodes)
            {
                int refRegion = PumpRegion(regionNode);
                PumpRegionFact(regionNode, refRegion);
            }
            UpdateData();
        }

        #endregion факты

        private void PumpXmlFile(FileInfo file)
        {
            // если не выбран ни один блок - выходим
            if (!ToPumpBlock(Block.bOutcomes) && !ToPumpBlock(Block.bPost) &&
                !ToPumpBlock(Block.bMarks) && !ToPumpBlock(Block.bMarksAuto))
                return;
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FullName);
            try
            {
                PumpClsDoc(doc);
                PumpFactDoc(doc);
            }
            finally
            {
                XmlHelper.ClearDomDocument(ref doc);
            }
        }

        #endregion работа с xml

        #region Перекрытые методы закачки

        protected override void DeleteEarlierPumpedData()
        {
            if (!this.DeleteEarlierData)
                return;
            // удаляем только факты (год, месяц), так как классификаторы формируются на год
            if (ToPumpBlock(Block.bOutcomes))
                DirectDeleteFactData(new IFactTable[] { fctOutcomes }, -1, this.SourceID, string.Empty);
            if (ToPumpBlock(Block.bPost))
                DirectDeleteFactData(new IFactTable[] { fctPost }, -1, this.SourceID, string.Empty);
            if (ToPumpBlock(Block.bMarks))
                DirectDeleteFactData(new IFactTable[] { fctMarks }, -1, this.SourceID, string.Empty);
            if (ToPumpBlock(Block.bMarksAuto))
                DirectDeleteFactData(new IFactTable[] { fctMarksAuto }, -1, this.SourceID, string.Empty);
        }

        private void SetClsHierarchy()
        {
            ClearHierarchy(dsOutcomesCls.Tables[0]);
            ClearHierarchy(dsPostCls.Tables[0]);

            FormStandardHierarchy(ref dsOutcomesCls, clsOutcomes, ClsHierarchyMode.Standard);
            FormStandardHierarchy(ref dsPostCls, clsPost, ClsHierarchyMode.Standard);
            FormStandardHierarchy(ref dsMarksCls, clsMarks, ClsHierarchyMode.Standard);
            FormStandardHierarchy(ref dsMarksAutoCls, clsMarksAuto, ClsHierarchyMode.Standard);
            FormStandardHierarchy(ref dsFkr, clsFkr, ClsHierarchyMode.Standard);

            SetClsHierarchy(clsOutcomes, ref dsOutcomesCls, "CODE",
                const_d_Outcomes_FO25_HierarchyFile2007, ClsHierarchyMode.Special);
            SetClsHierarchy(clsPost, ref dsPostCls, "CODE",
                const_d_Post_FO25_HierarchyFile2007, ClsHierarchyMode.Special);

            // Районы служебный
            SetClsHierarchy(ref dsRegionsForPump, clsRegionsForPump, null, string.Empty, ClsHierarchyMode.Standard);
        }
        
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
            UpdateData();
            // иерархию устанавливаем вручную
            toSetHierarchy = false;
            SetClsHierarchy();
            UpdateData();
        }

        protected override void DirectPumpData()
        {
            PumpDataYQTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

        #region обработка данных

        private string GetSKIFDocType(int docType)
        {
            switch (docType)
            {
                case 1:
                    return "Неуказанный тип отчетности";
                case 20:
                    return "Данные в разрезе муниципальных образований";
                case 21:
                    return "Данные в разрезе муниципальных образований и поселений";
                default:
                    return string.Empty;
            }
        }

        private void SetRegDocType()
        {
            // заполняем тип документа у районов, берем его из районов для закачки скиф
            int count = dsRegions.Tables[0].Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = dsRegions.Tables[0].Rows[i];
                string code = row["CODE"].ToString().PadLeft(10, '0');
                string name = row["NAME"].ToString();
                string key = string.Format("{0}|{1}", code, name);
                if (!regionsForPumpCache.ContainsKey(key))
                    continue;
                int docType = regionsForPumpCache[key];
                switch (docType)
                {
                    case 1:
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                            "Значение признака 'Тип документа.СКИФ' для района '{0}' (код {1}) равно {2} ({3}). ",
                            name, code, docType, GetSKIFDocType(docType)));
                        row["REFDOCTYPE"] = docType.ToString();
                        break;
                    case 20:
                    case 21:
                        WriteEventIntoProcessDataProtocol(ProcessDataEventKind.pdeWarning, string.Format(
                            "Значение признака 'Тип документа.СКИФ' для района '{0}' (код {1}) равно {2} ({3}). " +
                            "Данный тип документа является аналитическим признаком и не подлежит использованию.",
                            name, code, docType, GetSKIFDocType(docType)));
                        break;
                    default:
                        row["REFDOCTYPE"] = docType.ToString();
                        break;
                }
            }
        }

        private void CorrectSums()
        {
            CommonSumCorrectionConfig CommonCorrectionConfig = new CommonSumCorrectionConfig();
            CommonCorrectionConfig.Sum1 = "Assign";
            CommonCorrectionConfig.Sum1Report = "AssignRep";
            CommonCorrectionConfig.Sum2 = "Performed";
            CommonCorrectionConfig.Sum2Report = "PerformRep";
            CommonCorrectionConfig.Sum3 = string.Empty;
            CommonCorrectionConfig.Sum3Report = string.Empty;
            CorrectFactTableSums(fctOutcomes, dsOutcomesCls.Tables[0], clsOutcomes, "RefR",
                CommonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefFKR", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
            CommonCorrectionConfig.Sum1 = "Design";
            CommonCorrectionConfig.Sum1Report = "PlanRep";
            CommonCorrectionConfig.Sum2 = "Fact";
            CommonCorrectionConfig.Sum2Report = "FactRep";
            CommonCorrectionConfig.Sum3 = "MidCount";
            CommonCorrectionConfig.Sum3Report = "MCountRep";
            CorrectFactTableSums(fctPost, dsPostCls.Tables[0], clsPost, "RefPost",
                CommonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefFKR", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
            CommonCorrectionConfig.Sum1 = "Fact";
            CommonCorrectionConfig.Sum1Report = "FactRepot";
            CommonCorrectionConfig.Sum2 = string.Empty;
            CommonCorrectionConfig.Sum2Report = string.Empty;
            CommonCorrectionConfig.Sum3 = string.Empty;
            CommonCorrectionConfig.Sum3Report = string.Empty;
            CorrectFactTableSums(fctMarks, dsMarksCls.Tables[0], clsMarks, "RefMarks",
                CommonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefFKR", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
            CommonCorrectionConfig.Sum1 = "InDate";
            CommonCorrectionConfig.Sum1Report = "InDateRep";
            CommonCorrectionConfig.Sum2 = "MidPeriod";
            CommonCorrectionConfig.Sum2Report = "MidPerRep";
            CommonCorrectionConfig.Sum3 = string.Empty;
            CommonCorrectionConfig.Sum3Report = string.Empty;
            CorrectFactTableSums(fctMarksAuto, dsMarksAutoCls.Tables[0], clsMarksAuto, "RefMarksFOF14auto",
                CommonCorrectionConfig, BlockProcessModifier.MRStandard, new string[] { "RefFKR", "RefYearDayUNV" }, "RefRegions", string.Empty, true);
        }

        protected override void ProcessDataSource()
        {
            SetClsHierarchy();
            UpdateData();
            SetRegDocType();
            UpdateData();
            CorrectSums();
            UpdateData();
       }

        protected override void DirectProcessData()
        {
            int year = -1;
            int month = -1;
            GetPumpParams(ref year, ref month);
            ProcessDataSourcesTemplate(year, month, "Установка иерархии, коррекция сумм фактов по данным источника.");
        }

        #endregion обработка данных

    }
}
