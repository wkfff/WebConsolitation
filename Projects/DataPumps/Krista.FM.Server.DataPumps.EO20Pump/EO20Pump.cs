using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps.EO20Pump
{

    // ЕО - 0020 - Данные АС ООС
    public class EO20PumpModule : CorrectedPumpModuleBase
    {

        #region Поля

        #region Классификаторы

        // АС ООС.Заказчики (d_OOS_Customers)
        private IDbDataAdapter daCustomers;
        private DataSet dsCustomers;
        private IClassifier clsCustomers;
        private Dictionary<string, int> cacheCustomers;
        // АС ООС.Закупки (d_OOS_Tenders)
        private IDbDataAdapter daTenders;
        private DataSet dsTenders;
        private IClassifier clsTenders;

        #endregion Классификаторы

        private int maxCustomersCode = 0;
        private int maxTendersCode = 0;

        private BlockType blockType;
        private XmlNamespaceManager nsmgr;

        #endregion Поля

        #region Перечисления

        private enum BlockType
        {
            NotificationOK,
            NotificationEF,
            NotificationZK,
            NotificationPO,
            NotificationSZ
        }

        #endregion Перечисления

        #region Закачка данных

        #region Работа с базой и кэшами

        private int GetClsMaxCode(IClassifier cls, string constr)
        {
            string query = string.Format(" select max(CODE) from {0} ", cls.FullDBName);
            if (constr != string.Empty)
                query = string.Format(" {0} where {1} ", query, constr);
            object maxCode = this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
            if ((maxCode == null) || (maxCode == DBNull.Value))
                return 0;
            return Convert.ToInt32(maxCode);
        }

        private void SetMaxCodes()
        {
            maxCustomersCode = GetClsMaxCode(clsCustomers, string.Empty);
            maxTendersCode = GetClsMaxCode(clsTenders, string.Format("SourceId = {0}", this.SourceID));
        }

        private void FillCaches()
        {
            FillRowsCache(ref cacheCustomers, dsCustomers.Tables[0], "RegNumber");
        }

        protected override void QueryData()
        {
            InitDataSet(ref daCustomers, ref dsCustomers, clsCustomers, string.Empty);
            InitClsDataSet(ref daTenders, ref dsTenders, clsTenders);
            FillCaches();
            SetMaxCodes();
        }

        #region GUIDs

        private const string D_OOS_CUSTOMERS_GUID = "a7034cef-415c-4f29-8dee-bf7e801a19d6";
        private const string D_OOS_TENDERS_GUID = "93ffc797-8f1d-45e7-a6e6-bef68dad9a13";

        #endregion GUIDs
        protected override void InitDBObjects()
        {
            clsCustomers = this.Scheme.Classifiers[D_OOS_CUSTOMERS_GUID];
            clsTenders = this.Scheme.Classifiers[D_OOS_TENDERS_GUID];
            this.UsedClassifiers = new IClassifier[] { clsTenders };
            this.UsedFacts = new IFactTable[] { };
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daCustomers, dsCustomers, clsCustomers);
            UpdateDataSet(daTenders,  dsTenders, clsTenders);
        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsCustomers);
            ClearDataSet(ref dsTenders);
        }

        #endregion Работа с базой и кэшами

        #region Работа с Xml

        private string GetXmlTagValue(XmlNode node, string xpath)
        {
            XmlNode tag = node.SelectSingleNode(xpath, nsmgr);
            if (tag == null)
                return string.Empty;
            return tag.InnerText.Trim();
        }

        private int ConvertDate(string value)
        {
            string date = value.Split(new char[] { 'T', 'Т' })[0].Trim().Replace("-", string.Empty);
            if (date == string.Empty)
                return -1;
            return Convert.ToInt32(date);
        }

        private int GetRefPurchTyp()
        {
            switch (blockType)
            {
                case BlockType.NotificationOK:
                    return 1;
                case BlockType.NotificationEF:
                    return 3;
                case BlockType.NotificationZK:
                    return 4;
                case BlockType.NotificationPO:
                    return 6;
                case BlockType.NotificationSZ:
                    return 8;
                default:
                    return -1;
            }
        }

        private int GetGiveDate(int[] refDates)
        {
            switch (blockType)
            {
                case BlockType.NotificationOK:
                case BlockType.NotificationEF:
                case BlockType.NotificationSZ:
                    return refDates[0];
                case BlockType.NotificationZK:
                case BlockType.NotificationPO:
                    return refDates[1];
                default:
                    return -1;
            }
        }

        private int GetConsiderDate(int[] refDates)
        {
            switch (blockType)
            {
                case BlockType.NotificationOK:
                case BlockType.NotificationEF:
                    return refDates[1];
                default:
                    return -1;
            }
        }

        private int GetMatchDate(int[] refDates)
        {
            switch (blockType)
            {
                case BlockType.NotificationEF:
                    return refDates[2];
                default:
                    return -1;
            }
        }

        private int GetResultDate(int[] refDates)
        {
            switch (blockType)
            {
                case BlockType.NotificationOK:
                    return refDates[2];
                default:
                    return -1;
            }
        }

        private int PumpCustomer(XmlNode row)
        {
            if (row == null)
                return -1;

            string regNum = GetXmlTagValue(row, "oos:regNum");
            string name = GetXmlTagValue(row, "oos:fullName");
            if (cacheCustomers.ContainsKey(regNum))
                return cacheCustomers[regNum];

            maxCustomersCode++;
            return PumpCachedRow(cacheCustomers, dsCustomers.Tables[0], clsCustomers, regNum, new object[] {
                "Code", maxCustomersCode, "RegNumber", regNum, "Name", name, "RefTerritory", -1 });
        }

        private void PumpXmlRow(XmlNode row)
        {
            int refPurchTyp = GetRefPurchTyp();
            int refCustomer = PumpCustomer(row.SelectSingleNode("oos:lots/*//oos:customer", nsmgr));

            int[] refDates = new int[] {
                ConvertDate(GetXmlTagValue(row, "*//oos:p1Date")),
                ConvertDate(GetXmlTagValue(row, "*//oos:p2Date")),
                ConvertDate(GetXmlTagValue(row, "*//oos:p3Date")) };
            maxTendersCode++;

            object[] mapping = new object[] {
                "Code", maxTendersCode,
                "NotificationNum", GetXmlTagValue(row, "oos:notificationNumber"),
                "Name", GetXmlTagValue(row, "oos:orderName"),
                "Note", DBNull.Value,
                "PrintForm", GetXmlTagValue(row, "*/oos:url"),
                "Link", GetXmlTagValue(row, "oos:href"),
                "RefCustomer", refCustomer,
                "RefCreateDate", ConvertDate(GetXmlTagValue(row, "oos:createDate")),
                "RefPublicDate", ConvertDate(GetXmlTagValue(row, "oos:publishDate")),
                "RefGiveDate", GetGiveDate(refDates),
                "RefConsiderDate", GetConsiderDate(refDates),
                "RefMatchDate", GetMatchDate(refDates),
                "RefResultDate", GetResultDate(refDates),
                "RefPurchTyp", refPurchTyp
            };

            PumpRow(dsTenders.Tables[0], clsTenders, mapping);
        }

        private void PumpXmlRows(XmlNodeList rows)
        {
            foreach (XmlNode row in rows)
            {
                PumpXmlRow(row);
            }
        }

        private void PumpXmlFile(FileInfo file)
        {
            WriteToTrace("Открытие документа: " + file.Name, TraceMessageKind.Information);
            // далее приходится работать с диспетчером пространства имен,
            // а в документе у многих тегов (не у всех!) есть свой namespace
            // поэтому приходится его выжигать (последовательность 'oos:')
            string innerXml = File.ReadAllText(file.FullName).Replace("oos:", string.Empty);
            // после того как выжгли namespace, создаем поток и загружаем его в XmlDocument
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(innerXml));
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);
            try
            {
                XmlNode root = xmlDoc.DocumentElement;
                nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                // адрес namespace'a хранится в атрибуте xmlns корневого тега export
                nsmgr.AddNamespace("oos", root.Attributes["xmlns"].Value);

                blockType = BlockType.NotificationOK;
                PumpXmlRows(root.SelectNodes("oos:notificationOK", nsmgr));
                blockType = BlockType.NotificationEF;
                PumpXmlRows(root.SelectNodes("oos:notificationEF", nsmgr));
                blockType = BlockType.NotificationZK;
                PumpXmlRows(root.SelectNodes("oos:notificationZK", nsmgr));
                blockType = BlockType.NotificationPO;
                PumpXmlRows(root.SelectNodes("oos:notificationPO", nsmgr));
                blockType = BlockType.NotificationSZ;
                PumpXmlRows(root.SelectNodes("oos:notificationSZ", nsmgr));
            }
            finally
            {
                if (stream != null)
                    ((IDisposable)stream).Dispose();
                XmlHelper.ClearDomDocument(ref xmlDoc);
            }
        }

        #endregion Работа с Xml

        #region Перекрытые методы закачки

        private void ProcessAllFiles(DirectoryInfo dir)
        {
            ProcessFilesTemplate(dir, "*.xml", new ProcessFileDelegate(PumpXmlFile), false);
        }

        private void PumpZipFile(FileInfo file)
        {
            DirectoryInfo tempDir = CommonRoutines.ExtractArchiveFileToTempDir(
                file.FullName, FilesExtractingOption.SingleDirectory, ArchivatorName.Zip);
            try
            {
                ProcessAllFiles(tempDir);
            }
            finally
            {
                CommonRoutines.DeleteDirectory(tempDir);
            }
        }

        protected override void ProcessFiles(DirectoryInfo dir)
        {
            ProcessAllFiles(dir);
            ProcessFilesTemplate(dir, "*.zip", new ProcessFileDelegate(PumpZipFile), false);
        }

        protected override void  DirectPumpData()
        {
            PumpDataVTemplate();
        }

        #endregion Перекрытые методы закачки

        #endregion Закачка данных

    }

}
