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

namespace Krista.FM.Server.DataPumps.FO34Pump
{
    public class FO34PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        // кэши классификаторов: ай ди - код
        private DataTable dtFactCls = null;
        private Dictionary<int, string> cacheFactCls = null;
        private DataTable dtKcsr = null;
        private Dictionary<int, string> cacheKcsr = null;
        private DataTable dtKvsr = null;
        private Dictionary<int, string> cacheKvsr = null;
        private DataTable dtFkr = null;
        private Dictionary<int, string> cacheFkr = null;
        private DataTable dtKvr = null;
        private Dictionary<int, string> cacheKvr = null;
        private DataTable dtEkr = null;
        private Dictionary<int, string> cacheEkr = null;


        #region Факты

        // Расходы.ФО_АС Бюджет_План расходов (f_R_BudgetData)
        private IFactTable fctOutcomesPlan;
        // Расходы.Расходы казначейство (f_R_FacialFinDetail)
        private IFactTable fctTreasury;

        #endregion Факты

        #endregion Поля

        #region Закачка данных

        #region Работа с базой и кэшами

        private const string D_KCSR_GUID = "baa5a0db-a283-4a31-b063-b0275fbb36d3";
        private const string F_OUTCOMES_PLAN_GUID = "9ab30b73-a50a-434c-b002-06918a82376b";
        private const string F_TREASURY_GUID = "1ed1e2cb-814e-48cf-9c89-72dcbff09f04";
        protected override void InitDBObjects()
        {
            this.UsedClassifiers = new IClassifier[] {};
            this.UsedFacts = new IFactTable[] { };
            fctOutcomesPlan = this.Scheme.FactTables[F_OUTCOMES_PLAN_GUID];
            fctTreasury = this.Scheme.FactTables[F_TREASURY_GUID];
        }

        private void GetCache(int sourceId, ref DataTable dt, ref Dictionary<int, string> cache, string tableName)
        {
            string query = string.Format("select id, code from {0} where SourceId = {1}", tableName, sourceId);
            dt = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            FillRowsCache(ref cache, dt, "Id", "Code");
        }
        
        private void GetCaches(int sourceId)
        {
            GetCache(sourceId, ref dtKcsr, ref cacheKcsr, "d_KCSR_Budget");
            GetCache(sourceId, ref dtFactCls, ref cacheFactCls, "d_Fact_Budget");
            GetCache(sourceId, ref dtKvsr, ref cacheKvsr, "d_KVSR_Budget");
            GetCache(sourceId, ref dtFkr, ref cacheFkr, "d_FKR_Budget");
            GetCache(sourceId, ref dtKvr, ref cacheKvr, "d_KVR_Budget");
            GetCache(sourceId, ref dtEkr, ref cacheEkr, "d_EKR_Budget");
        }

        #endregion Работа с базой и кэшами

        #region экспорт отчета

        #region получение данных факта

        private string GetCacheKey(DataRow row, string[] refsCls)
        {
            string key = string.Empty;
            foreach (string clsRef in refsCls)
            {
                string refValue = row[clsRef].ToString();
                key += string.Format("{0}|", refValue);
            }
            key = key.Remove(key.Length - 1);
            return key;
        }

        // возвращает данные факта: ключ синтетический (ссылки на классификаторы), значение - сумма
        private Dictionary<string, decimal> GetFactData(IFactTable fct, int sourceId, List<int> kcsrList, bool isTreasury)
        {
            Dictionary<string, decimal> factData = new Dictionary<string, decimal>();
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1}", fct.FullDBName, sourceId), QueryResultTypes.Scalar));
            if (totalRecs == 0)
                return factData;
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where SOURCEID = {1}", fct.FullDBName, sourceId), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            string blockName = string.Empty;
            if (isTreasury)
                blockName = "Казначейство";
            else
                blockName = "План расходов";
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2}", firstID, lastID, sourceId);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fct, idConstr);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    SetProgress(totalRecs, processedRecCount, string.Format("Обработка данных блока '{0}'...", blockName),
                        string.Format("Запись {0} из {1}", processedRecCount, totalRecs));
                    int refKcsr = Convert.ToInt32(row["RefKCSR2005"]);
                    if (!kcsrList.Contains(refKcsr))
                        continue;
                    string[] refCls = new string[] { "RefKCSR2005", "RefKVSR", "RefFKR2005", "RefKVR", "RefEKR2005", "RefFact" };
                    if (isTreasury)
                        refCls = (string[])CommonRoutines.ConcatArrays(refCls, new string[] { "RefYearMonthDay" });
                    else
                        refCls = (string[])CommonRoutines.ConcatArrays(refCls, new string[] { "RefYearMonth" });
                    string cacheKey = GetCacheKey(row, refCls);
                    decimal sum = 0;
                    if (isTreasury)
                        sum = Convert.ToDecimal(row["Debit"]) - Convert.ToDecimal(row["ReturnDebit"]);
                    else
                        sum = Convert.ToDecimal(row["Summe"]);
                    if (!factData.ContainsKey(cacheKey))
                        factData.Add(cacheKey, sum);
                    else
                        factData[cacheKey] += sum;
                }
                ClearDataSet(ref ds);
            }
            while (processedRecCount < totalRecs);
            return factData;
        }

        // список кцср, по которым нужны записи факта (указанный в параметрах кцср и его подчиненные)
        private List<int> GetKcsrList(int sourceId)
        {
            List<int> kcsrList = new List<int>();
            int kcsr = Convert.ToInt32(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "uneKcsr", string.Empty));
            string query = string.Format("select id, parentId from d_KCSR_Budget where SourceId = {0}", sourceId);
            DataTable kcsrTable = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            foreach (DataRow row in kcsrTable.Rows)
            {
                int id = Convert.ToInt32(row["Id"]);
                int parentId = Convert.ToInt32(row["parentId"].ToString().PadRight(1, '0'));
                if ((id == kcsr) || (parentId == kcsr))
                    kcsrList.Add(id);
            }
            return kcsrList;
        }

        #endregion получение данных факта

        #region экспорт в xml

        private string CorrectDate(string date)
        {
            string correctedDate = date.Substring(4);
            switch (correctedDate)
            {
                case "9991":
                    correctedDate = "0331";
                    break;
                case "9992":
                    correctedDate = "0630";
                    break;
                case "9993":
                    correctedDate = "0930";
                    break;
                case "9994":
                case "0000":
                    correctedDate = "1231";
                    break;
            }
            return string.Concat(date.Substring(0, 4), correctedDate);
        }

        private string GetDatePrefix(string planFact)
        {
            if (planFact == "1")
                return "П";
            else
                return "Ф";
        }

        public XmlNode AddChildNode(XmlNode xmlRootNode, string tagName, string value, string nameSpaceUri)
        {
            XmlNode xmlNode = xmlRootNode.OwnerDocument.CreateElement(tagName, nameSpaceUri);
            xmlNode.InnerText = value;
            xmlRootNode.AppendChild(xmlNode);
            return xmlNode;
        }

        private void ExportDataToXmlNode(XmlNode rootNode, Dictionary<string, decimal> factData, 
            string desc, string planFact)
        {
            string datePrefix = GetDatePrefix(planFact);
            foreach (KeyValuePair<string, decimal> cacheItem in factData)
            {
                XmlNode rowNode = AddChildNode(rootNode, "row", string.Empty, BDO_NAMESPACE);
                string[] refCls = cacheItem.Key.Split('|');
                string txDate = refCls[6];
                if (planFact == "1")
                    txDate = CorrectDate(txDate);
                AddChildNode(rowNode, "tx_number", string.Format("{0}{1}", datePrefix, txDate.Remove(6)), BDO_NAMESPACE);
                txDate = txDate.Insert(4, "-").Insert(7, "-");
                AddChildNode(rowNode, "tx_date", txDate, BDO_NAMESPACE);
                AddChildNode(rowNode, "program_code", "00", BDO_NAMESPACE);
                AddChildNode(rowNode, "subprogram_code", "00", BDO_NAMESPACE);
                AddChildNode(rowNode, "activity_code", string.Format("00{0}",
                    cacheFactCls[Convert.ToInt32(refCls[5])].PadLeft(3, '0')), BDO_NAMESPACE);
                AddChildNode(rowNode, "plan_fact", planFact, BDO_NAMESPACE);
                AddChildNode(rowNode, "kbk_kvsr", cacheKvsr[Convert.ToInt32(refCls[1])].PadLeft(3, '0'), BDO_NAMESPACE);
                AddChildNode(rowNode, "kbk_fkr", cacheFkr[Convert.ToInt32(refCls[2])].PadLeft(4, '0'), BDO_NAMESPACE);
                AddChildNode(rowNode, "kbk_kcsr", cacheKcsr[Convert.ToInt32(refCls[0])].PadLeft(7, '0'), BDO_NAMESPACE);
                AddChildNode(rowNode, "kbk_kvr", cacheKvr[Convert.ToInt32(refCls[3])].PadLeft(3, '0'), BDO_NAMESPACE);
                AddChildNode(rowNode, "kbk_ekr", cacheEkr[Convert.ToInt32(refCls[4])].PadLeft(3, '0'), BDO_NAMESPACE);
                AddChildNode(rowNode, "amount", cacheItem.Value.ToString().Replace(',', '.'), BDO_NAMESPACE);
                AddChildNode(rowNode, "oper_description", desc, BDO_NAMESPACE);
            }
        }

        #endregion экспорт в xml

        private string GetFileName(string dirFullName)
        {
            return string.Format("{0}\\pm{1}{2}{3}{4}{5}{6}.xml", dirFullName, 
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second );
        }

        private const string BDO_NAMESPACE = "http://www.bdo.ru/xmao/budget";
        private void ExportData(DirectoryInfo destDir, int sourceId)
        {
            List<int> kcsrList = GetKcsrList(sourceId);
            if (kcsrList.Count == 0)
                return;
            GetCaches(sourceId);
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootNode = xmlDocument.CreateElement("payments", BDO_NAMESPACE);
            rootNode.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlDocument.AppendChild(rootNode);
            Dictionary<string, decimal> treasuryData = GetFactData(fctTreasury, sourceId, kcsrList, true);
            Dictionary<string, decimal> outcomesPlanData = GetFactData(fctOutcomesPlan, sourceId, kcsrList, false);
            try
            {
                ExportDataToXmlNode(rootNode, outcomesPlanData, "Финансирование-план", "1");
                ExportDataToXmlNode(rootNode, treasuryData, "Финансирование-факт", "2");
                string fileName = GetFileName(destDir.FullName);
                XmlHelper.Save(xmlDocument, fileName);
            }
            finally
            {
                treasuryData.Clear();
                outcomesPlanData.Clear();
                XmlHelper.ClearDomDocument(ref xmlDocument);
            }
        }

        #endregion экспорт отчета

        #region Перекрытые методы закачки

        private string GetParamsYear()
        {
            string dateMin = GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "mePeriodMin", string.Empty);
            return dateMin.Replace(".", string.Empty).Substring(4);
        }

        private DirectoryInfo CheckRootSubDir(DirectoryInfo rootDir, string subDirName)
        {
            DirectoryInfo[] subDir = rootDir.GetDirectories(subDirName, SearchOption.TopDirectoryOnly);
            if (subDir.GetLength(0) == 0)
                return rootDir.CreateSubdirectory(subDirName);
            else
                return subDir[0];
        }

        protected override void DirectPumpData()
        {
            string year = GetParamsYear();
            DirectoryInfo yearDir = CheckRootSubDir(this.RootDir, year);
            // получаем источники по заданному в параметрах году, для каждого создаем папку - ФинОрган и экспортируем в нее данные
            string query = string.Format("select id, name from DataSources where SupplierCode = 'ФО' and DataCode = 1 and Year = {0}", year);
            DataTable dataSources = (DataTable)this.DB.ExecQuery(query, QueryResultTypes.DataTable, new IDbDataParameter[] { });
            foreach (DataRow row in dataSources.Rows)
            {
                int sourceId = Convert.ToInt32(row["Id"]);
                string finOrgan = row["Name"].ToString();
                DirectoryInfo finOrganDir = CheckRootSubDir(yearDir, finOrgan);
                ExportData(finOrganDir, sourceId);
            }
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }
}
