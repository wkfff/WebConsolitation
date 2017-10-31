using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.DataPumps
{

    /// <summary>
    /// ������� ����� ��� ������� ������� ����
    /// </summary>
    public abstract partial class SKIFRepPumpModuleBase : CorrectedPumpModuleBase
    {

        public int regForPumpSourceID;
        public int yearSourceID;
        public bool noRegForPump = false;
        public bool forcePumpForm127 = false;
        public double sumMultiplier = 1;

        protected Dictionary<int, string> kvrAuxCache = new Dictionary<int, string>();
        protected Dictionary<int, string> kcsrAuxCache = new Dictionary<int, string>();
        protected Dictionary<int, string> fkrAuxCache = new Dictionary<int, string>();

        #region ���������, ������������

        /// <summary>
        /// ������ ������ ���
        /// </summary>
        protected enum XmlFormat
        {
            /// <summary>
            /// ������ 2004 �. � ������
            /// </summary>
            Format2004,

            /// <summary>
            /// ������ 2005 �. (�� ������� 2005)
            /// </summary>
            Format2005,

            /// <summary>
            /// ������ ������� 2005+
            /// </summary>
            October2005,

            /// <summary>
            /// Skif3 (������ �� 2005 ���)
            /// </summary>
            Skif3
        }

        /// <summary>
        /// ����� ������� ���
        /// </summary>
        protected enum XmlForm
        {
            // ������
            Form649,
            Form650,
            Form651,
            Form414,
            Form428,
            Form428v,
            Form128,
            Form128v,
            Form487,
            Form117,
            Form628r,
            Form127,
            Form127v,
            Form127g,
            Form159,
            Form169,
            Form159V,
            Form169V,
            Form469,
            Form459,
            Form469V,
            Form459V,
            // ������
            Form428g,
            Form428Vg,
            Form623,
            Form625,
            Form624,
            Form630,
            Form12001,
            Form13001,
            Form43001,
            Form12002,
            Form13002,
            Form43002,

            UnknownForm
        }

        #endregion ���������, ������������

        #region ����� ������� XML

        /// <summary>
        /// ���������� ������� � ������� ������ ����� ���
        /// </summary>
        /// <param name="xdReport">���-�������� ������</param>
        /// <returns>������� � ������� ������</returns>
        protected XmlNode GetReportNode(XmlDocument xdReport)
        {
            XmlNode xnReport = xdReport.SelectSingleNode("/Otchet");

            if (xnReport == null)
            {
                xnReport = xdReport.SelectSingleNode("/RootXml/Report");
            }

            return xnReport;
        }

        /// <summary>
        /// ��������� ���-�����
        /// </summary>
        /// <param name="filesList">������ ������</param>
        /// <param name="filesRepList">������ ������ �������</param>
        /// <param name="xdPattern">����������� ������</param>
        private void LoadXMLFiles(FileInfo[] filesList, out FileInfo[] filesRepList, out XmlDocument xdPattern)
        {
            filesRepList = new FileInfo[0];
            xdPattern = null;
            for (int i = 0; i < filesList.GetLength(0); i++)
            {
                if (!File.Exists(filesList[i].FullName))
                    continue;
                // ���������� ���� ��� � ���������������� (����� � ������� ���� ������� ������� ��)
                if (filesList[i].Name.ToUpper().StartsWith("����������_"))
                    continue;
                // ���� ������
                if (filesList[i].Name.ToUpper().StartsWith(MonthPatternName.ToUpper()))
                {
                    if (xdPattern != null)
                        throw new PumpDataFailedException("� �������� ��������� ����� ������ �������.");
                    xmlFilesCount--;
                    // ���� ����� - ���������
                    xdPattern = new XmlDocument();
                    xdPattern.Load(filesList[i].FullName);
                }
                else
                {
                    // ��������� ������ ������ �������
                    filesRepList = (FileInfo[])CommonRoutines.RedimArray(filesRepList, filesRepList.GetLength(0) + 1);
                    filesRepList[filesRepList.GetLength(0) - 1] = filesList[i];
                }
            }
        }

        /// <summary>
        /// ��������� ������� ���������� �� �������� - ���� ������� ����� !, �� ��������� �������������
        /// </summary>
        /// <param name="codeExclusion">������� ����������</param>
        /// <param name="result">���������</param>
        private bool InverseExclusionResult(string codeExclusion, bool result)
        {
            if (codeExclusion.TrimStart('#').StartsWith("!"))
            {
                return !result;
            }

            return result;
        }

        /// <summary>
        /// ���������� ������ �� ������������� (������) ���� ������ ������.
        /// </summary>
        /// <param name="dataNode">���� ������</param>
        /// <param name="attrIndex">������ ��������, ����������� ������ (-1 - ������������ ���� ���������)</param>
        /// <returns>������</returns>
        private string GetClsfCode(XmlNode dataNode, int attrIndex)
        {
            string result = string.Empty;

            if (attrIndex >= 0)
            {
                if (attrIndex < dataNode.Attributes.Count) result = dataNode.Attributes[attrIndex].Value;
            }
            else
            {
                for (int i = 0; i < dataNode.Attributes.Count; i++)
                {
                    result += dataNode.Attributes[i].Value;
                }
            }

            return result;
        }

        /// <summary>
        /// ���������, �������� �� ���� ������ ������
        /// </summary>
        /// <param name="xmlDoc">�����</param>
        /// <returns>�������� ��� ���</returns>
        private XmlNode GetPatternNode(XmlDocument xmlDoc)
        {
            return xmlDoc.SelectSingleNode("//RootXml/Task");
        }

        // ���������� �� ��������� �� ���
        protected int GetYearSourceID()
        {
            return AddDataSource("��", "0002", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        /// <summary>
        /// ���������� �� ��������� ��� ������.���������
        /// </summary>
        /// <returns>�� ��������� ��� ������.���������</returns>
        protected int GetRegions4PumpSourceID()
        {
            return AddDataSource("��", "0006", ParamKindTypes.Year, string.Empty,
                this.DataSource.Year, 0, string.Empty, 0, string.Empty).ID;
        }

        private bool PumpRegion4Pump(string code, string key, string name,
            DataTable regions4PumpTable, IClassifier regions4PumpCls, Dictionary<string, int> regions4PumpCache)
        {
            if (regions4PumpTable == null)
                return true;
            if (!regions4PumpCache.ContainsKey(key))
            {
                PumpCachedRow(regions4PumpCache, regions4PumpTable, regions4PumpCls, code, key, "CODESTR", "REFDOCTYPE",
                    new object[] { "NAME", name, "REFDOCTYPE", 1, "SOURCEID", regForPumpSourceID });
                return false;
            }
            return true;
        }

        protected bool PumpRegionsXML(XmlNode xn, DataTable regionsTable, IClassifier regionsCls,
            Dictionary<string, int> regionsCache, DataTable regions4PumpTable, IClassifier regions4PumpCls,
            Dictionary<string, int> regions4PumpCache)
        {
            XmlNodeList xnlSources = xn.SelectNodes("//Source");
            for (int i = 0; i < xnlSources.Count; i++)
            {
                string regionCode = GetAttrValueByName(xnlSources[i].Attributes, "ObjectNmb", "Code").PadLeft(10, '0');
                string regionName = GetAttrValueByName(xnlSources[i].Attributes, "Name", "ObjectName");
                string regionKey = regionCode + "|" + regionName;
                PumpCachedRow(regionsCache, regionsTable, regionsCls, regionCode, regionKey, "CODESTR", "ID",
                    new object[] { "NAME", ConvertClsName(GetAttrValueByName(xnlSources[i].Attributes, "ObjectName", "Name")),
                        "BUDGETKIND", GetAttrValueByName(xnlSources[i].Attributes, "ClassKey", "ClassCode"),
                        "BUDGETNAME", GetAttrValueByName(xnlSources[i].Attributes, "ClassName") });
                if (!PumpRegion4Pump(regionCode, regionKey, regionName, regions4PumpTable, regions4PumpCls, regions4PumpCache))
                    noRegForPump = true;
            }
            return true;
        }

        /// <summary>
        /// ����������� ����� ��� � ������
        /// </summary>
        /// <param name="xmlForm">�����</param>
        private string XmlFormToString(XmlForm xmlForm)
        {
            switch (xmlForm)
            {
                case XmlForm.Form128: return "128";
                case XmlForm.Form128v: return "128v";
                case XmlForm.Form414: return "414";
                case XmlForm.Form428: return "428";
                case XmlForm.Form428g: return "428g";
                case XmlForm.Form428v: return "428v";
                case XmlForm.Form428Vg: return "428Vg";
                case XmlForm.Form649: return "649";
                case XmlForm.Form650: return "650";
                case XmlForm.Form651: return "651";
                case XmlForm.Form623: return "623";
                case XmlForm.Form625: return "625";
                case XmlForm.Form624: return "624";
                case XmlForm.Form630: return "630";
                case XmlForm.Form487: return "487";
                case XmlForm.Form117: return "117";
                case XmlForm.Form127: return "127";
                case XmlForm.Form127v: return "127v";
                case XmlForm.Form127g: return "127g";
                case XmlForm.Form159: return "159";
                case XmlForm.Form169: return "169";
                case XmlForm.Form159V: return "159V";
                case XmlForm.Form169V: return "169V";
                case XmlForm.Form469: return "469";
                case XmlForm.Form459: return "459";
                case XmlForm.Form469V: return "469V";
                case XmlForm.Form459V: return "459V";
                case XmlForm.Form12001: return "12001";
                case XmlForm.Form13001: return "13001";
                case XmlForm.Form43001: return "43001";
                case XmlForm.Form12002: return "12002";
                case XmlForm.Form13002: return "13002";
                case XmlForm.Form43002: return "43002";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// ����������� ������ ������� ���� ��� � ������ � ����������� ��������
        /// </summary>
        /// <param name="xmlForm">������ ����</param>
        private string XmlFormsToString(XmlForm[] xmlForm)
        {
            string result = string.Empty;

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                result += XmlFormToString(xmlForm[i]) + ", ";
            }

            return result.Trim().Trim(',');
        }

        /// <summary>
        /// ����������� ����� ��� � ������
        /// </summary>
        /// <param name="xmlForm">�����</param>
        private XmlForm StringToXmlForm(string xmlForm)
        {
            switch (xmlForm.ToUpper())
            {
                case "128": return XmlForm.Form128;
                case "128V": return XmlForm.Form128v;
                case "414": return XmlForm.Form414;
                case "428": return XmlForm.Form428;
                case "428G": return XmlForm.Form428g;
                case "428V": return XmlForm.Form428v;
                case "428VG": return XmlForm.Form428Vg;
                case "649": return XmlForm.Form649;
                case "650": return XmlForm.Form650;
                case "651": return XmlForm.Form651;
                case "623": return XmlForm.Form623;
                case "625": return XmlForm.Form625;
                case "624": return XmlForm.Form624;
                case "630": return XmlForm.Form630;
                case "487": return XmlForm.Form487;
                case "117": return XmlForm.Form117;
                case "628R": return XmlForm.Form628r;
                case "127": return XmlForm.Form127;
                case "127V": return XmlForm.Form127v;
                case "127G": return XmlForm.Form127g;
                case "159": return XmlForm.Form159;
                case "169": return XmlForm.Form169;
                case "159V": return XmlForm.Form159V;
                case "169V": return XmlForm.Form169V;
                case "469": return XmlForm.Form469;
                case "459": return XmlForm.Form459;
                case "469V": return XmlForm.Form469V;
                case "459V": return XmlForm.Form459V;
                case "12001": return XmlForm.Form12001;
                case "13001": return XmlForm.Form13001;
                case "43001": return XmlForm.Form43001;
                case "12002": return XmlForm.Form12002;
                case "13002": return XmlForm.Form13002;
                case "43002": return XmlForm.Form43002;
                default: return XmlForm.UnknownForm;
            }
        }

        /// <summary>
        /// ����������� ����� ��� � ������
        /// </summary>
        /// <param name="xmlForm">������ �� ������� ����</param>
        private XmlForm[] StringToXmlForms(string xmlForm)
        {
            string[] forms = xmlForm.Split(';');

            XmlForm[] result = new XmlForm[forms.GetLength(0)];

            for (int i = 0; i < forms.GetLength(0); i++)
            {
                result[i] = StringToXmlForm(forms[i]);
            }

            return result;
        }

        /// <summary>
        /// ���� ������ �������� �� ��� ��������
        /// </summary>
        /// <param name="xac">��������� ���������</param>
        /// <param name="attrName">�������� ��������</param>
        /// <returns>������ (-1 - �� ������)</returns>
        private int GetAttributeIndex(XmlAttributeCollection xac, string attrName)
        {
            for (int i = 0; i < xac.Count; i++)
            {
                if (xac[i].Name.ToUpper() == attrName.ToUpper()) return i;
            }
            return -1;
        }

        /// <summary>
        /// ���� ������� � ��������� �������� � ���������� ��� ������
        /// </summary>
        /// <param name="xd">��������</param>
        /// <param name="nodeName">������������ ��������</param>
        /// <param name="attrName">������������ ��������</param>
        /// <returns>������ �������� (-1 - �� ������)</returns>
        private int GetTableAttributeIndex(XmlDocument xd, string nodeName, string attrName)
        {
            XmlNode xn = xd.SelectSingleNode(string.Format("//Table[@Name = \"{0}\"]/TableFields", nodeName));
            if (xn == null) return -1;
            return GetAttributeIndex(xn.Attributes, attrName);
        }

        /// <summary>
        /// ���������� ����� ������ ���
        /// </summary>
        /// <param name="formNo">����� �����, ��������� � ���</param>
        /// <returns>����� �����</returns>
        private XmlForm GetXmlForm(string formNo)
        {
            string str = formNo.ToUpper();
            switch (this.XmlReportFormat)
            {
                case XmlFormat.October2005:
                    if (CommonRoutines.TrimNumbers(str) == string.Empty)
                    {
                        if (str.StartsWith("414"))
                            return XmlForm.Form414;
                        else if (str.StartsWith("428"))
                            return XmlForm.Form428;
                    }
                    else
                        if (str.StartsWith("428V"))
                            return XmlForm.Form428v;
                    break;
                default:
                    if (CommonRoutines.TrimNumbers(str) == string.Empty)
                    {
                        if (str.StartsWith("649"))
                            return XmlForm.Form649;
                        else if (str.StartsWith("650"))
                            return XmlForm.Form650;
                        else if (str.StartsWith("651"))
                            return XmlForm.Form651;
                        else if (str.StartsWith("623"))
                            return XmlForm.Form623;
                        else if (str.StartsWith("625"))
                            return XmlForm.Form625;
                        else if (str.StartsWith("624"))
                            return XmlForm.Form624;
                        else if (str.StartsWith("630"))
                            return XmlForm.Form630;
                        else if (str.StartsWith("487"))
                            return XmlForm.Form487;
                    }
                    break;
            }
            return XmlForm.UnknownForm;
        }

        /// <summary>
        /// ����������� ������������ �������������� � ������������ ����
        /// </summary>
        /// <param name="name">������������</param>
        /// <returns>������������ � ����������� ����</returns>
        private string ConvertClsName(string name)
        {
            string str = name.Replace("&quot;", "\"");
            if (string.IsNullOrEmpty(str))
                str = constDefaultClsName;

            return str;
        }

        #endregion ����� ������� XML


        #region ������� ������� �������� ������� XML

        /// <summary>
        /// ���������� �������� ���� � ������������ �������������� ��� ������� �������
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xnFormRow">������� � ������� ��������������</param>
        /// <param name="itemNameInd">������ �������� ItemName</param>
        /// <param name="clsfIDInd">������ �������� ClsfID</param>
        /// <param name="clsfCodeInd">������ �������� ClsfCode</param>
        /// <param name="indPagNoInd">������ �������� IndPagNo</param>
        /// <param name="indRowNoInd">������ �������� IndRowNo</param>
        /// <param name="code">���</param>
        /// <param name="name">������������</param>
        /// <param name="kl">��� �����</param>
        /// <param name="kst">��� ������</param>
        /// <returns>���� ���-�� �� �������, �� false</returns>
        private bool GetClsFieldsFromExternalPatternXML(XmlDocument xdPattern, XmlNode xnFormRow, int itemNameInd,
            int clsfIDInd, int clsfCodeInd, int indPagNoInd, int indRowNoInd, out string code, out string name,
            out int kl, out int kst)
        {
            code = string.Empty;
            name = string.Empty;
            kl = 0;
            kst = 0;

            string[] tableFields =
                xnFormRow.Attributes["Values"].Value.Split(new string[] { "," }, StringSplitOptions.None);

            // ������ ��� ������� ������������ �������� ��� � ����������
            XmlNode xnCls = xdPattern.SelectSingleNode(string.Format(
                "//Table[@Name = \"Skif_Classificator\" or @Name = \"MFClassificator\"]/RecordSet/" +
                "Row[starts-with(@Values, \"{0}\")]", tableFields[clsfIDInd]));
            if (xnCls == null)
            {
                return false;
            }

            string[] clsFields = xnCls.Attributes["Values"].Value.Split(',');

            code = clsFields[clsfCodeInd].Trim('\'');
            string[] nameField = xnFormRow.Attributes["Values"].Value.Split(new string[] { "'" }, StringSplitOptions.None);

            if (nameField.GetLength(0) >= 2)
            {
                name = nameField[1].Trim();
            }
            else
            {
                name = constDefaultClsName;
            }

            kl = Convert.ToInt32(tableFields[indPagNoInd]);
            kst = Convert.ToInt32(tableFields[indRowNoInd]);

            return true;
        }

        /// <summary>
        /// ���������� ������� ��������� ��� ������ ������ ��������������� �� ������� �������
        /// </summary>
        /// <param name="formIDInd">������ �������� FormID</param>
        /// <param name="sectNoInd">������ �������� SectNo</param>
        /// <param name="rowsSetIDInd">������ �������� RowsSetID</param>
        /// <param name="itemNameInd">������ �������� ItemName</param>
        /// <param name="clsfIDInd">������ �������� ClsfID</param>
        /// <param name="clsfCodeInd">������ �������� ClsfCode</param>
        /// <param name="indPagNoInd">������ �������� IndPagNo</param>
        /// <param name="indRowNoInd">������ �������� IndRowNo</param>
        private void GetClsAttributesIndexes(XmlDocument xdPattern, ref int formIDInd, ref int sectNoInd,
            ref int rowsSetIDInd, ref int itemNameInd, ref int clsfIDInd, ref int clsfCodeInd, ref int indPagNoInd,
            ref int indRowNoInd)
        {
            // �������� ������� ������ ���������
            formIDInd = GetTableAttributeIndex(xdPattern, "FormSection", "FormID");
            sectNoInd = GetTableAttributeIndex(xdPattern, "FormSection", "SectNo");
            rowsSetIDInd = GetTableAttributeIndex(xdPattern, "FormSection", "RowsSetID");
            itemNameInd = GetTableAttributeIndex(xdPattern, "FormRows", "ItemName");
            clsfIDInd = GetTableAttributeIndex(xdPattern, "FormRows", "ClsfID");
            indPagNoInd = GetTableAttributeIndex(xdPattern, "FormRows", "IndPagNo");
            indRowNoInd = GetTableAttributeIndex(xdPattern, "FormRows", "IndRowNo");
            clsfCodeInd = GetTableAttributeIndex(xdPattern, "MFClassificator", "ClsfCode");
            if (clsfCodeInd < 0)
            {
                clsfCodeInd = GetTableAttributeIndex(xdPattern, "Skif_Classificator", "ClsfCode");
            }
        }

        /// <summary>
        /// ������� ����������� ��� ������� ������ � ���������� ���������� ������ �����
        /// </summary>
        /// <param name="xmlForm">������ ����</param>
        /// <returns>�����������</returns>
        private string GetConstrForExternalPattern(XmlForm[] xmlForm)
        {
            if (xmlForm == null) return string.Empty;

            string result = string.Empty;

            bool disable428v = false;
            Array.Sort(xmlForm);

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                if (xmlForm[i] == XmlForm.Form428)
                {
                    disable428v = true;
                }

                if (xmlForm[i] == XmlForm.Form428v && disable428v) continue;

                result += string.Format("starts-with(@Values, \"{0}\") or ", XmlFormToString(xmlForm[i]));
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(result.Length - 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// ���������� ������������� �� �������, ��������� � ��������� �����
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        protected void PumpClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, Dictionary<string, int> codesMapping)
        {
            int formIDInd = -1;
            int sectNoInd = -1;
            int rowsSetIDInd = -1;
            int itemNameInd = -1;
            int clsfIDInd = -1;
            int indPagNoInd = -1;
            int indRowNoInd = -1;
            int clsfCodeInd = -1;

            GetClsAttributesIndexes(xdPattern, ref formIDInd, ref sectNoInd, ref rowsSetIDInd, ref itemNameInd,
                ref clsfIDInd, ref clsfCodeInd, ref indPagNoInd, ref indRowNoInd);

            XmlNodeList xnlFormSectionRows = xdPattern.SelectNodes(string.Format(
                "//Table[@Name = \"FormSection\"]/RecordSet/Row{0}", GetConstrForExternalPattern(xmlForm)));
            if (xnlFormSectionRows.Count == 0) return;

            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("������� ������ �������������� {0}...", semantic), TraceMessageKind.Information);

            string clsCodeField = GetClsCodeField(cls);

            // ������� ��� ������ FormSection � ���������� ��������������� ��������������
            for (int i = 0; i < xnlFormSectionRows.Count; i++)
            {
                string[] tableFields = xnlFormSectionRows[i].Attributes["Values"].Value.Split(',');
                if (!CommonRoutines.CheckValueEntry(Convert.ToInt32(tableFields[sectNoInd]), sectNo)) continue;

                // �������� �������� � �������������� ��������������
                XmlNodeList xnlFormRows = xdPattern.SelectNodes(string.Format(
                    "//Table[@Name = \"FormRows\"]/RecordSet/Row[starts-with(@Values, \"{0}\")]",
                    tableFields[rowsSetIDInd]));

                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    string clsCode;
                    string clsName;
                    int kl;
                    int kst;

                    if (!GetClsFieldsFromExternalPatternXML(
                        xdPattern, xnlFormRows[j], itemNameInd, clsfIDInd, clsfCodeInd, indPagNoInd, indRowNoInd,
                            out clsCode, out clsName, out kl, out kst))
                    {
                        continue;
                    }

                    PumpCachedRow(codesMapping, dt, cls, clsCode, new object[] {
                        clsCodeField, clsCode, "NAME", ConvertClsName(clsName), "KL", kl, "KST", kst });

                    SetProgress(xnlFormRows.Count, j + 1, string.Format("��������� �������. ������ {0}...", semantic),
                        string.Format("������ {0} �� {1}", j + 1, xnlFormRows.Count));
                }
            }

            WriteToTrace(string.Format("������ �������������� {0} ��������.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ��������� � ��������� �����
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������. ������ ������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� �
        /// �������� 2)</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        protected void PumpClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, string sectNo, DataTable dt,
            IClassifier cls, Dictionary<string, int> codesMapping)
        {
            PumpClsFromExternalPatternXML(xdPattern, xmlForm, CommonRoutines.ParseParamsString(sectNo), dt, cls, codesMapping);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ��������� � ��������� �����.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������, num..-1 - ��� ���������� �������, ������� � num)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ���������
        /// ����� ������������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            if (xdPattern.SelectNodes("//Table[@Name = \"FormSection\"]").Count == 0)
            {
                throw new Exception("������ ��� ������� �������: ����������� ��� ��������� FormSection.");
            }

            int formIDInd = -1;
            int sectNoInd = -1;
            int rowsSetIDInd = -1;
            int itemNameInd = -1;
            int clsfIDInd = -1;
            int indPagNoInd = -1;
            int indRowNoInd = -1;
            int clsfCodeInd = -1;

            GetClsAttributesIndexes(xdPattern, ref formIDInd, ref sectNoInd, ref rowsSetIDInd, ref itemNameInd,
                ref clsfIDInd, ref clsfCodeInd, ref indPagNoInd, ref indRowNoInd);

            XmlNodeList xnlFormSectionRows = xdPattern.SelectNodes(string.Format(
                "//Table[@Name = \"FormSection\"]/RecordSet/Row{0}", GetConstrForExternalPattern(xmlForm)));
            if (xnlFormSectionRows.Count == 0) return;

            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("������� ������ �������������� {0}...", semantic), TraceMessageKind.Information);

            string clsCodeField = GetClsCodeField(cls);
            // ������ ���������� ������� ������ - ��� �������������� �� ���������� �����������
            List<int> pumpedSectNo = new List<int>(10);

            // ������� ��� ������ FormSection � ���������� ��������������� ��������������
            for (int i = 0; i < xnlFormSectionRows.Count; i++)
            {
                string[] tableFields = xnlFormSectionRows[i].Attributes["Values"].Value.Split(',');

                int currSectNo = Convert.ToInt32(tableFields[sectNoInd]);
                if (pumpedSectNo.Contains(currSectNo))
                {
                    continue;
                }
                else
                {
                    pumpedSectNo.Add(currSectNo);
                }

                if (!CommonRoutines.CheckValueEntry(currSectNo, sectNo)) continue;

                // �������� �������� � �������������� ��������������
                XmlNodeList xnlFormRows = xdPattern.SelectNodes(string.Format(
                    "//Table[@Name = \"FormRows\"]/RecordSet/Row[starts-with(@Values, \"{0}\")]",
                    tableFields[rowsSetIDInd]));

                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    string clsCode;
                    string clsName;
                    int kl;
                    int kst;

                    if (!GetClsFieldsFromExternalPatternXML(
                        xdPattern, xnlFormRows[j], itemNameInd, clsfIDInd, clsfCodeInd, indPagNoInd, indRowNoInd,
                            out clsCode, out clsName, out kl, out kst))
                    {
                        continue;
                    }

                    if (CheckCodeExclusion(clsCode, codeExclusions) ||
                        (indPagNo != null && !CheckCodeExclusion(kl, indPagNo)))
                    {
                        continue;
                    }

                    // ��������� ��� �� ��� �� ����� ��������������
                    string[] fieldsMapping = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");

                    if (clsProcessModifier == ClsProcessModifier.FKRBook)
                        fieldsMapping[1] = fieldsMapping[1].TrimStart('0').PadLeft(1, '0');

                    // ������������� ��� ����������� �� ���������, ��� �������� ������������� ��� ����� 0.
                    // ������������� ��� ����������� �� ���������, ��� �������� ������������� ��� �� ����� 0.
                    if ((clsProcessModifier == ClsProcessModifier.EKRBook &&
                        Convert.ToInt32(GetFieldsValuesAsSubstring(new string[] { "CODE", "0..3" }, clsCode, "0")[1]) != 0) ||
                        (clsProcessModifier == ClsProcessModifier.FKRBook && Convert.ToInt32(fieldsMapping[1]) == 0))
                    {
                        // ��� ��� ��� ����� ���������� ���� ��� � ����
                        if (clsProcessModifier == ClsProcessModifier.EKRBook)
                        {
                            if (codesMapping != null)
                            {
                                int id = FindRowID(dt, fieldsMapping, -1);
                                if (id != -1)
                                {
                                    if (!codesMapping.ContainsKey(clsCode))
                                    {
                                        codesMapping.Add(clsCode, id);
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    switch (clsProcessModifier)
                    {
                        case ClsProcessModifier.FKRBook:
                        case ClsProcessModifier.EKRBook:
                            clsCode = fieldsMapping[1];
                            break;
                        case ClsProcessModifier.MarksOutcomes:
                            clsCode = string.Concat(fieldsMapping[1].ToString(), fieldsMapping[3].ToString(), kst);
                            fieldsMapping = (string[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] { "LongCode", clsCode});
                            break;
                    }
                    // ����� ����� � ����� ����
                    if ((fieldsMapping[0] == "SRCINFIN") || (fieldsMapping[0] == "SRCOUTFIN"))
                    {
                        clsCode = string.Concat(fieldsMapping[1].ToString(), fieldsMapping[3].ToString(), kst);
                        fieldsMapping = (string[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] { "LongCode", clsCode });
                    }
                    // ����� �������������
                    if ((fieldsMapping[0] == "FKR") && (clsProcessModifier == ClsProcessModifier.Special))
                    {
                        clsCode = string.Concat(fieldsMapping[1].ToString(), fieldsMapping[3].ToString(), kst);
                        fieldsMapping = (string[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] { "LongCode", clsCode });
                    }

                    // ���������� ������
                    PumpRowFromExternalPattern(dt, cls, useCodeMapping, codesMapping, clsCode, clsName, kl, kst,
                        fieldsMapping, codeMasks, clsCodeField, clsProcessModifier);

                    SetProgress(xnlFormRows.Count, j + 1, string.Format("��������� �������. ������ {0}...", semantic),
                        string.Format("������ {0} �� {1}", j + 1, xnlFormRows.Count));
                }
            }

            WriteToTrace(string.Format("������ �������������� {0} ��������.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ��������� � ��������� �����.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������. ������ ������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� �
        /// �������� 2)</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ���������
        /// ����� ������������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, string sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForm, CommonRoutines.ParseParamsString(sectNo), dt,
                cls, attrValuesMapping, useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier,
                codeMasks);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ��������� � ��������� �����.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������. ������ ������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� �
        /// �������� 2)</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ���������
        /// ����� ������������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, string sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForm, sectNo, dt, cls, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ��������� � ��������� �����.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xdPattern">������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ���������
        /// ����� ������������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        protected void PumpComplexClsFromExternalPatternXML(XmlDocument xdPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, string[] indPagNo, ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromExternalPatternXML(xdPattern, xmlForm, sectNo, dt, cls, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
        }

        /// <summary>
        /// ���������� ������ �������������� �� �������, ������������ � ��������� ����
        /// </summary>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="clsCode">��� �������������� �� �������</param>
        /// <param name="clsName">������������ �������������� �� �������</param>
        /// <param name="kl">��� �����</param>
        /// <param name="kst">��� ������</param>
        /// <param name="fieldsMapping">������ ��� �����������_���� - �������� ��� ������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        private void PumpRowFromExternalPattern(DataTable dt, IClassifier cls, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string clsCode, string clsName, int kl, int kst,
            string[] fieldsMapping, int[] codeMasks, string clsCodeField, ClsProcessModifier clsProcessModifier)
        {
            if (useCodeMapping)
            {
                // ���������� �������� ����� ����
                if (clsProcessModifier == ClsProcessModifier.CacheSubCode)
                {
                    PumpCachedRow(codesMapping, dt, cls, fieldsMapping[1], clsCodeField,
                        new object[] { "NAME", ConvertClsName(clsName), "KL", kl, "KST", kst });
                }
                else
                {
                    PumpCachedRow(codesMapping, dt, cls, clsCode,
                        (object[])CommonRoutines.ConcatArrays(fieldsMapping,
                            new string[] { "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
                }
            }
            else
            {
                if (codeMasks == null)
                {
                    PumpCachedRow(codesMapping, dt, cls, clsCode,
                        (object[])CommonRoutines.ConcatArrays(fieldsMapping,
                            new string[] { clsCodeField, clsCode, "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
                }
                else
                {
                    string newClsCode = BuildCodeBySubCodesMask(fieldsMapping, codeMasks);
                    PumpCachedRow(codesMapping, dt, cls, newClsCode,
                        (object[])CommonRoutines.ConcatArrays(fieldsMapping, new string[] {
                            clsCodeField, newClsCode, "NAME", ConvertClsName(clsName), "KL", kl.ToString(), "KST", kst.ToString() }));
                }
            }
        }

        #endregion ������� ������� �������� ������� XML


        #region ������� ������� ����������� ������� XML

        /// <summary>
        /// ��������� �������� �������� Code
        /// </summary>
        /// <param name="xmlForm">�����</param>
        /// <param name="sectNo">������ (-1 - �� ���������)</param>
        /// <returns>�������� ��������</returns>
        private string GetReportCode(XmlForm xmlForm, int sectNo)
        {
            switch (xmlForm)
            {
                case XmlForm.Form128v:
                    if (sectNo < 0)
                    {
                        return "@Code = \"128v\" or @Code = \"128V\" or ";
                    }
                    else
                    {
                        return string.Format("@Code = \"128{0:00}v\" or @Code = \"128{0:00}V\" or ", sectNo);
                    }

                case XmlForm.Form428g:
                    if (sectNo < 0)
                    {
                        return "@Code = \"428g\" or ";
                    }
                    else
                    {
                        return string.Format("@Code = \"428{0:00}g\" or ", sectNo);
                    }

                case XmlForm.Form428Vg:
                    if (sectNo < 0)
                    {
                        return "@Code = \"428Vg\" or ";
                    }
                    else
                    {
                        return string.Format("@Code = \"428{0:00}Vg\" or ", sectNo);
                    }

                case XmlForm.Form628r:
                    return string.Format("@Code = \"628{0:00}r\" or ", sectNo);

                case XmlForm.Form127g:
                    return string.Format("@Code = \"127g{0:00}\" or @Code = \"127G{0:00}\" or ", sectNo);
                case XmlForm.Form127v:
                    return string.Format("@Code = \"127{0:00}v\" or @Code = \"127{0:00}V\" or ", sectNo);

                default:
                    if (sectNo < 0)
                    {
                        return string.Format("@Code = \"{0}\" or ", XmlFormToString(xmlForm));
                    }
                    else
                    {
                        return string.Format("@Code = \"{0}{1:00}\" or ", XmlFormToString(xmlForm), sectNo);
                    }
            }
        }

        /// <summary>
        /// ������� ����������� ��� ������� ������ � ���������� ���������� ������ ����� � ������
        /// </summary>
        /// <param name="xmlForm">������ ����</param>
        /// <param name="sectNo">������ ������� ������</param>
        /// <returns>�����������</returns>
        private string GetConstrForInternalPattern(XmlForm[] xmlForm, int[] sectNo)
        {
            if (xmlForm == null)
                return string.Empty;
            string result = string.Empty;
            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                if (sectNo != null && sectNo.GetLength(0) > 0)
                    for (int j = 0; j < sectNo.GetLength(0); j++)
                    {
                        result += GetReportCode(xmlForm[i], sectNo[j]);
                    }
                else
                    result += GetReportCode(xmlForm[i], -1);
            }
            if (result != string.Empty)
                result = "[" + result.Remove(result.Length - 4) + "]";
            return result;
        }

        /// <summary>
        /// ���������� ������������� �� �������, ������������ ������ � �������
        /// </summary>
        /// <param name="xnPattern">������� �����</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="formHierarchy">���������� �������� ����� �������</param>
        protected void PumpClsFromInternalPatternXML(XmlNode xnPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, Dictionary<string, int> codesMapping)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("������� ������ �������������� {0}...", semantic), TraceMessageKind.Information);

            int totalNodes = xnPattern.SelectNodes(string.Format(
                "//FormTemplate{0}/FormRows/Rows/Row", GetConstrForInternalPattern(xmlForm, sectNo))).Count;
            if (totalNodes == 0)
            {
                WriteToTrace(string.Format("��� ������ �� �������������� {0}.", semantic), TraceMessageKind.Information);
                return;
            }
            int nodesCount = 0;

            // �������� �������� � ������ � �������������� ��������������
            XmlNodeList xnlFormTemplates = xnPattern.SelectNodes(string.Format(
                "//FormTemplate{0}/FormRows/Rows/Row", GetConstrForInternalPattern(xmlForm, sectNo)));

            string clsCodeField = GetClsCodeField(cls);
            // ������ ���������� ������� ������ - ��� �������������� �� ���������� �����������
            List<int> pumpedSectNo = new List<int>(10);

            for (int i = 0; i < xnlFormTemplates.Count; i++)
            {
                XmlNodeList xnlFormRows = xnlFormTemplates[i].SelectNodes("./FormRows/Rows/Row");

                int currSectNo = GetSectNoFromFormCode(xnlFormTemplates[i].Attributes["Code"].Value);
                if (pumpedSectNo.Contains(currSectNo))
                {
                    nodesCount += xnlFormRows.Count;
                    SetProgress(totalNodes, nodesCount, string.Format("��������� �������. ������ {0}...", semantic),
                        string.Format("������ {0} �� {1}", nodesCount, totalNodes));
                    continue;
                }
                else
                {
                    pumpedSectNo.Add(currSectNo);
                }

                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    nodesCount++;
                    SetProgress(totalNodes, nodesCount, string.Format("��������� �������. ������ {0}...", semantic),
                        string.Format("������ {0} �� {1}", nodesCount, totalNodes));

                    // �������� ���
                    string clsCode = GetClsfCode(xnlFormRows[j], -1);
                    if (clsCode == string.Empty)
                    {
                        continue;
                    }

                    XmlNode tmpNode = xnlFormRows[j].SelectSingleNode("./RowP");
                    PumpCachedRow(codesMapping, dt, cls, clsCode, new object[] {
                        clsCodeField, clsCode,
                        "NAME", ConvertClsName(tmpNode.Attributes["Name"].Value),
                        "KL", tmpNode.Attributes["Page"].Value,
                        "KST", tmpNode.Attributes["Row"].Value });

                }
            }

            WriteToTrace(string.Format("������ �������������� {0} ��������.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// �������� ��� �������������� �� �������� � ������ ����� ��������
        /// </summary>
        /// <param name="codeValues">������ �������� (���_���� - ���)</param>
        /// <param name="codeMasks">������ �����</param>
        /// <returns>���</returns>
        private string BuildCodeBySubCodesMask(string[] codeValues, int[] codeMasks)
        {
            string result = string.Empty;

            int count = codeValues.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                result += codeValues[i + 1].PadLeft(codeMasks[i / 2], '0');
            }

            return result;
        }

        /// <summary>
        /// ���������� � ��� ��� ��������������
        /// </summary>
        /// <param name="dt">������� ��� ������</param>
        /// <param name="codesMapping">���</param>
        /// <param name="clsCode">��� ��������������</param>
        /// <param name="codeValues">�������� ����� ��������������</param>
        private void WriteToCacheClsCode(DataTable dt, Dictionary<string, int> codesMapping, string clsCode,
            object[] codeValues)
        {
            if (codesMapping != null)
            {
                int id = FindRowID(dt, codeValues, -1);
                if (id != -1 && !codesMapping.ContainsKey(clsCode))
                {
                    codesMapping.Add(clsCode, id);
                }
            }
        }

        /// <summary>
        /// ���������� ������ �� ����������� �������
        /// </summary>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">�������������</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ���������
        /// ����� ������������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        /// <param name="clsCodeField">��� ���� ���� ��������������</param>
        /// <param name="clsCode">���</param>
        /// <param name="codeValues">�������� ������������� ����</param>
        /// <param name="xn">������� � ������� ��������������</param>
        /// <param name="kl">��� �����</param>
        private void PumpRowFromInternalPattern(DataTable dt, IClassifier cls, bool useCodeMapping,
            Dictionary<string, int> codesMapping, ClsProcessModifier clsProcessModifier, int[] codeMasks,
            string clsCodeField, string clsCode, string[] codeValues, XmlNode xn, int kl)
        {
            string name = ConvertClsName(xn.Attributes["Name"].Value);
            string kst = xn.Attributes["Row"].Value;
            if (this.DataSource.Year >= 2010)
            {
                switch (clsProcessModifier)
                {
                    case ClsProcessModifier.MarksOutcomes:
                    case ClsProcessModifier.Arrears:
                    case ClsProcessModifier.Excess:
                        kst = clsCode.Substring(0, 5);
                        break;
                }
            }

            // ���� ���������� ����� ���� - ������ ��������
            //if (clsProcessModifier == ClsProcessModifier.EKRBook && (kl == 11 || kl == 12))
            //    name = string.Empty;

            // ������� � ���� �������� � ����������
            clsCode = clsCode.Replace('�', 'A');
            clsCode = clsCode.Replace('�', 'a');
            codeValues[1] = codeValues[1].Replace('�', 'A');
            codeValues[1] = codeValues[1].Replace('�', 'a');

            if (!useCodeMapping)
            {
                if (codeMasks == null)
                {
                    PumpCachedRow(codesMapping, dt, cls, clsCode,
                        (object[])CommonRoutines.ConcatArrays(codeValues, new string[] {
                            clsCodeField, clsCode, "NAME", name, "KL", kl.ToString(), "KST", kst }));
                }
                else
                {
                    string newClsCode = BuildCodeBySubCodesMask(codeValues, codeMasks);
                    PumpCachedRow(codesMapping, dt, cls, newClsCode,
                        (object[])CommonRoutines.ConcatArrays(codeValues, new string[] {
                            clsCodeField, newClsCode, "NAME", name, "KL", kl.ToString(), "KST", kst }));
                }
            }
            else
            {
                // ���������� �������� ����� ����
                switch (clsProcessModifier)
                {
                    case ClsProcessModifier.CacheSubCode:
                    case ClsProcessModifier.EKR:
                    case ClsProcessModifier.EKRBook:
                    case ClsProcessModifier.FKR:
                    case ClsProcessModifier.FKRBook:
                        PumpCachedRow(codesMapping, dt, cls, codeValues[1], clsCodeField, new object[] {
                            "NAME", name, "KL", kl, "KST", kst });
                        break;

                    default:
                        PumpCachedRow(codesMapping, dt, cls, clsCode,
                            (object[])CommonRoutines.ConcatArrays(codeValues, new string[] {
                                "NAME", name, "KL", kl.ToString(), "KST", kst }));
                        break;
                }
            }
        }

        /// <summary>
        /// ���������� ������������� �� �������, ������������ ������ � �������.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xnPattern">������� �����</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attr2FieldMapping">������ ��� ����-������������_��������. ��� ��������, ��� � ��������� ����
        /// ����� �������� �������� �� �������� � ��������������� �������������. ����� ���� ������� ���������
        /// ��������� ����� ; - ����� �������� �������� �� ������� ��������, � ������� ��� ����.
        /// "attrName+attrName" - ����� ����������� ������������ �������� ���������.</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - 2 �������: num1, num2;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� ��
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ���������
        /// ����� ������������� ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        private void PumpComplexClsFromInternalPatternXML(XmlNode xnPattern, XmlForm[] xmlForm, int[] sectNo, DataTable dt,
            IClassifier cls, string[] attr2FieldMapping, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("������� ������ �������������� {0}...", semantic), TraceMessageKind.Information);
            string constr = GetConstrForInternalPattern(xmlForm, sectNo);
            int totalNodes = xnPattern.SelectNodes(string.Format("//FormTemplate{0}/FormRows/Rows/Row", constr)).Count;
            if (totalNodes == 0)
            {
                WriteToTrace(string.Format("��� ������ �� �������������� {0}.", semantic), TraceMessageKind.Information);
                return;
            }
            int nodesCount = 0;
            // �������� �������� � ������ � �������������� ��������������
            XmlNodeList xnlFormTemplates = xnPattern.SelectNodes(string.Format("//FormTemplate{0}", constr));
            string clsCodeField = GetClsCodeField(cls);
            // ������ ���������� ������� ������ - ��� �������������� �� ���������� �����������
            List<int> pumpedSectNo = new List<int>(30);
            for (int i = 0; i < xnlFormTemplates.Count; i++)
            {
                XmlNodeList xnlFormRows = xnlFormTemplates[i].SelectNodes("./FormRows/Rows/Row");
                int currSectNo = GetSectNoFromFormCode(xnlFormTemplates[i].Attributes["Code"].Value);
                if (pumpedSectNo.Contains(currSectNo))
                {
                    nodesCount += xnlFormRows.Count;
                    SetProgress(totalNodes, nodesCount, string.Format("��������� �������. ������ {0}...", semantic),
                        string.Format("������ {0} �� {1}", nodesCount, totalNodes));
                    continue;
                }
                else
                    pumpedSectNo.Add(currSectNo);
                bool ToPumpArrearsRow = false;
                bool ToPumpOutcomesBooksRow = true;
                bool ToPumpInDebtBooksRow = false;
                bool ToPumpOutDebtBooksRow = false;
                bool toPumpExcessBooksRow = false;

                if (this.DataSource.Year >= 2010)
                    ToPumpOutcomesBooksRow = false;

                bool ToCheckKL = true;
                for (int j = 0; j < xnlFormRows.Count; j++)
                {
                    nodesCount++;
                    SetProgress(totalNodes, nodesCount, string.Format("��������� �������. ������ {0}...", semantic),
                        string.Format("������ {0} �� {1}", nodesCount, totalNodes));
                    // �������� ���
                    // ���� ��� ����� ������������ ����� ������������ ��������� �������� ���
                    string clsCode = GetClsfCode(xnlFormRows[j], -1);
                    // ��������� ������ �������� �� ���������: ������ ��� �������� ����-��������_��������
                    string[] codeValues = null;
                    switch (clsProcessModifier)
                    {
                        case ClsProcessModifier.SrcInFin:
                        case ClsProcessModifier.SrcOutFin:
                            // � ������������� ���������� �������� �������������� �����.������_2005 ������ ��������
                            // ������ �� ������������ ���� ��������.
                            // � ������������� ���������� ����������� �������������� ����.������_2005 ������ ��������
                            // ������ �� ������������ ���� �������.
                            if (attr2FieldMapping != null)
                                codeValues = GetFieldsValuesAtPos(attr2FieldMapping, xnlFormRows[j], true);
                            else if (attrValuesMapping != null)
                                codeValues = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");
                            break;
                        default:
                            if (attr2FieldMapping != null)
                                codeValues = GetFieldsValuesAtPos(attr2FieldMapping, xnlFormRows[j], false);
                            else if (attrValuesMapping != null)
                                codeValues = GetFieldsValuesAsSubstring(attrValuesMapping, clsCode, "0");
                            break;
                    }
                    if (codeValues == null)
                        continue;
                    // � �� �������� ��� �� 20
                    if (((clsProcessModifier == ClsProcessModifier.Standard) ||
                         (clsProcessModifier == ClsProcessModifier.SrcInFin) ||
                         (clsProcessModifier == ClsProcessModifier.SrcOutFin)) &&
                        (this.DataSource.Year >= 2005))
                    {
                        codeValues[1] = codeValues[1].ToString().PadLeft(20, '0');
                        clsCode = clsCode.PadLeft(20, '0');
                    }

                    // � ��� �������� ��� �� 14
                    if (clsProcessModifier == ClsProcessModifier.FKR)
                    {
                        codeValues[1] = codeValues[1].ToString() + "0000000000";
                    }

                    int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
                    if (this.DataSource.Year >= 2008)
                    {
                        decimal clsCodeInt = 0;
                        if (this.DataSource.Year <= 2010)
                        {
                            if ((clsProcessModifier == ClsProcessModifier.Arrears) || (clsProcessModifier == ClsProcessModifier.MarksInDebt) ||
                                (clsProcessModifier == ClsProcessModifier.MarksOutDebt) || (clsProcessModifier == ClsProcessModifier.MarksOutcomes) ||
                                (clsProcessModifier == ClsProcessModifier.Excess))
                                clsCodeInt = Convert.ToDecimal(clsCode.TrimStart('0').PadLeft(1, '0'));
                        }
                        switch (clsProcessModifier)
                        {
                            case ClsProcessModifier.Arrears:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 ���� - [1090000000000000000000000, 1210000000000000000000225)
                                    if (clsCode == "1090000000000000000000000")
                                        ToPumpArrearsRow = true;
                                    if (clsCode == "1210000000000000000000225")
                                        ToPumpArrearsRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 ���� - [1000000000000000000000000, 1130000000000000000000225)
                                    if (clsCode == "1000000000000000000000000")
                                        ToPumpArrearsRow = true;
                                    if (clsCode == "1130000000000000000000225")
                                        ToPumpArrearsRow = false;
                                }
                                else
                                {
                                    if (currSectNo == 2)
                                    {
                                        // � 2009 ���� - ����������.������_���������������� ������ ����� 0000000000000000000010100 (�������)
                                        if (this.DataSource.Year >= 2009)
                                        {
                                            if (clsCode == "0000000000000000000010100")
                                                ToPumpArrearsRow = true;
                                            // c ������ 2009 ���� - ����������.������_����������������  [0000000000000000000010100, 0000000000000000022511400)
                                            if (sourceDate >= 200904)
                                            {
                                                if (clsCode == "0000000000000000022511400")
                                                    ToPumpArrearsRow = false;
                                            }
                                        }
                                        // � 2008 ���� - ����������.������_���������������� ������ ����� 000000000000000000008600 (�������)
                                        else if (clsCode == "000000000000000000008600")
                                            ToPumpArrearsRow = true;
                                    }
                                }
                                if (!ToPumpArrearsRow)
                                    continue;
                                break;
                            case ClsProcessModifier.MarksInDebt:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 ���� -  [1050000000000000000000000, 1060000000000000000000000)
                                    // [1070000000000000000000000, 1080000000000000000000000)  + 1240000000000000000000000
                                    if ((clsCode == "1050000000000000000000000") || (clsCode == "1070000000000000000000000"))
                                        ToPumpInDebtBooksRow = true;
                                    if ((clsCode == "1060000000000000000000000") || (clsCode == "1080000000000000000000000") || (clsCode == "1300000000000000000000000"))
                                        ToPumpInDebtBooksRow = false;
                                    if (clsCode == "1240000000000000000000000")
                                        ToPumpInDebtBooksRow = true;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 ���� -  [0960000000000000000000000, 0970000000000000000000000)
                                    // [0980000000000000000000000, 0990000000000000000000000)  + 1160000000000000000000000
                                    if ((clsCode == "0960000000000000000000000") || (clsCode == "0980000000000000000000000"))
                                        ToPumpInDebtBooksRow = true;
                                    if ((clsCode == "0970000000000000000000000") || (clsCode == "0990000000000000000000000"))
                                        ToPumpInDebtBooksRow = false;
                                    if (clsCode == "1160000000000000000000000")
                                        ToPumpInDebtBooksRow = true;
                                }
                                else if (this.DataSource.Year >= 2009)
                                {
                                    // c 2009 ���� ����������.������_������������ ������ [0000000000000000000009700,0000000000000000000009800)
                                    // [0000000000000000000009900, 0000000000000000000010000)
                                    ToPumpInDebtBooksRow = (((clsCodeInt >= 9700) && (clsCodeInt < 9800)) ||
                                                            ((clsCodeInt >= 9900) && (clsCodeInt < 10000)));
                                }
                                else
                                {
                                    // c 2008 ���� ����������.������_������������ ������ [000000000000000000008200,000000000000000000008300)
                                    // [000000000000000000008400, 000000000000000000008500)
                                    ToPumpInDebtBooksRow = (((clsCodeInt >= 8200) && (clsCodeInt < 8300)) ||
                                                            ((clsCodeInt >= 8400) && (clsCodeInt < 8500)));
                                }
                                if (!ToPumpInDebtBooksRow)
                                    continue;
                                break;
                            case ClsProcessModifier.MarksOutDebt:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 ���� -  [1060000000000000000000000, 1070000000000000000000000)
                                    if (clsCode == "1060000000000000000000000")
                                        ToPumpOutDebtBooksRow = true;
                                    if (clsCode == "1070000000000000000000000")
                                        ToPumpOutDebtBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 ���� -  [0970000000000000000000000, 0980000000000000000000000)
                                    if (clsCode == "0970000000000000000000000")
                                        ToPumpOutDebtBooksRow = true;
                                    if (clsCode == "0980000000000000000000000")
                                        ToPumpOutDebtBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 ��� - ����������.������_������������ ������ [0000000000000000000009800, 0000000000000000000009900)
                                    ToPumpOutDebtBooksRow = ((clsCodeInt >= 9800) && (clsCodeInt < 9900));
                                }
                                else
                                {
                                    // 2008 ��� - ����������.������_������������ ������ [000000000000000000008300, 000000000000000000008400)
                                    ToPumpOutDebtBooksRow = ((clsCodeInt >= 8300) && (clsCodeInt < 8400));
                                }
                                if (!ToPumpOutDebtBooksRow)
                                    continue;
                                break;
                            case ClsProcessModifier.MarksOutcomes:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2012)
                                {
                                    // c 2012 ���� - ����������.������_����������
                                    // [0010000000000000000000000, 1050000000000000000000000)
                                    // [1210000000000000000000225, 1240000000000000000000000)
                                    // [1300000000000000000000225, 1499900000000000000000000)
                                    if ((clsCode == "0010000000000000000000000") || (clsCode == "1210000000000000000000225") || (clsCode == "1300000000000000000000000"))
                                        ToPumpOutcomesBooksRow = true;
                                    if ((clsCode == "1050000000000000000000000") || (clsCode == "1240000000000000000000000") || (clsCode == "1499900000000000000000000"))
                                        ToPumpOutcomesBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2011)
                                {
                                    // c 2011 ���� - ����������.������_����������
                                    // [0010000000000000000000000, 1050000000000000000000000)
                                    // [1210000000000000000000225, 1240000000000000000000000)
                                    // [1300000000000000000000225, 1360200000000000000000000)
                                    if ((clsCode == "0010000000000000000000000") || (clsCode == "1210000000000000000000225") || (clsCode == "1300000000000000000000000"))
                                        ToPumpOutcomesBooksRow = true;
                                    if ((clsCode == "1050000000000000000000000") || (clsCode == "1240000000000000000000000") || (clsCode == "1360200000000000000000000"))
                                        ToPumpOutcomesBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // c 2010 ���� - ����������.������_���������� [0010000000000000000000000, 0960000000000000000000000)
                                    // [1130000000000000000000225, 1160000000000000000000000)
                                    if ((clsCode == "0010000000000000000000000") || (clsCode == "1130000000000000000000225"))
                                        ToPumpOutcomesBooksRow = true;
                                    if ((clsCode == "0960000000000000000000000") || (clsCode == "1160000000000000000000000"))
                                        ToPumpOutcomesBooksRow = false;
                                }
                                else if (currSectNo == 2)
                                {
                                    // c 2009 ���� - ����������.������_���������� �� 2 ������ �� ��������� = 0000000000000000000009700 (�� ��������)
                                    if (this.DataSource.Year >= 2009)
                                    {
                                        if (clsCode == "0000000000000000000009700")
                                            ToPumpOutcomesBooksRow = false;
                                        // c ������ 2009 ���� - ����������.������_����������  (<;10100, 0000000000000000022511400;>]
                                        if (sourceDate >= 200904)
                                        {
                                            if (clsCode == "0000000000000000022511400")
                                                ToPumpOutcomesBooksRow = true;
                                        }
                                    }
                                    // c 2008 ���� - ����������.������_���������� �� 2 ������ �� ������������ = 000000000000000000008200 (�� �������)
                                    else
                                    {
                                        if (clsCode == "000000000000000000008200")
                                            ToPumpOutcomesBooksRow = false;
                                    }
                                }
                                if (!ToPumpOutcomesBooksRow)
                                    continue;
                                break;
                            case ClsProcessModifier.Excess:
                                ToCheckKL = false;
                                if (this.DataSource.Year >= 2011)
                                {
                                    // 2011 - [1080000000000000000000000, 1090000000000000000000000)
                                    if (clsCode == "1080000000000000000000000")
                                        toPumpExcessBooksRow = true;
                                    if (clsCode == "1090000000000000000000000")
                                        toPumpExcessBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2010)
                                {
                                    // 2010 - [0990000000000000000000000, 1000000000000000000000000) 
                                    if (clsCode == "0990000000000000000000000")
                                        toPumpExcessBooksRow = true;
                                    if (clsCode == "1000000000000000000000000")
                                        toPumpExcessBooksRow = false;
                                }
                                else if (this.DataSource.Year >= 2009)
                                {
                                    // 2008 - ����������.������_���������� ������ [0000000000000000000010000, 0000000000000000000010100) 
                                    toPumpExcessBooksRow = ((clsCodeInt >= 10000) && (clsCodeInt < 10100));
                                }
                                else
                                {
                                    // 2008 - ����������.������_���������� ������ [000000000000000000008500, 000000000000000000008600) 
                                    toPumpExcessBooksRow = ((clsCodeInt >= 8500) && (clsCodeInt < 8600));
                                }
                                if (!toPumpExcessBooksRow)
                                    continue;
                                break;
                        }
                    }
                    else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                    {
                        switch (clsProcessModifier)
                        {
                            case ClsProcessModifier.Arrears:
                                ToCheckKL = false;
                                // ����������.������_���������������� ������ ����� ������ ���������������� = 000000000000000000007000 (�������)
                                if (currSectNo == 4)
                                {
                                    if (clsCode == "000000000000000000007000")
                                        ToPumpArrearsRow = true;
                                    if (!ToPumpArrearsRow)
                                        continue;
                                }
                                break;
                            case ClsProcessModifier.MarksOutcomes:
                                ToCheckKL = false;
                                // ����������.������_���������� ������ �� 1 ������ ��� � �� 4 - �� ������ ���������������� = 000000000000000000007000 (�� �������)
                                if (currSectNo == 4)
                                {
                                    if (clsCode == "000000000000000000007000")
                                        ToPumpOutcomesBooksRow = false;
                                    if (!ToPumpOutcomesBooksRow)
                                        continue;
                                }
                                break;
                        }
                    }
                    XmlNode tmpNode = xnlFormRows[j].SelectSingleNode("./RowP");
                    int kl = Convert.ToInt32(tmpNode.Attributes["Page"].Value);
                    string kst = tmpNode.Attributes["Row"].Value.PadLeft(4, '0');
                    // � 2009 ���� kst ����� �������� �� 5 ��������
                    if (this.DataSource.Year >= 2009)
                        kst = kst.PadLeft(5, '0');
                    switch (clsProcessModifier)
                    {
                        case ClsProcessModifier.MarksOutDebt:
                            if (this.DataSource.Year >= 2010)
                            {
                                kst = clsCode.Substring(0, 5);
                                codeValues[1] = string.Empty.PadLeft(11, '0');
                                codeValues[3] = "000000";
                                // ��� =  ������ (KST) + �������� ����������� �������������� + ������������
                                clsCode = kst + codeValues[1] + codeValues[3];
                            } 
                            else if (this.DataSource.Year == 2004)
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                            {
                                codeValues[1] = string.Empty.PadLeft(20, '0');
                                codeValues[3] = "000000";
                                // ��� = �������� �������� ��������������+�����������+������ (KST)
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            }
                            break;
                        case ClsProcessModifier.MarksInDebt:
                            if (this.DataSource.Year >= 2010)
                            {
                                kst = clsCode.Substring(0, 5);
                                codeValues[1] = string.Empty.PadLeft(14, '0');
                                codeValues[3] = "000";
                                // ��� =  ������ (KST) + �������� ����������� �������������� + ������������
                                clsCode = kst + codeValues[1] + codeValues[3];
                            }
                            else if (this.DataSource.Year == 2004)
                            {
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            }
                            else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                            {
                                codeValues[1] = string.Empty.PadLeft(20, '0');
                                codeValues[3] = "000";
                                // ��� = �������� ����������� �������������� + ������������ + ������ (KST)
                                clsCode = codeValues[1] + codeValues[3] + kst;
                            }
                            break;
                        case ClsProcessModifier.MarksOutcomes:
                        case ClsProcessModifier.Arrears:
                        case ClsProcessModifier.Excess:
                            if (this.DataSource.Year >= 2010)
                            {
                                kst = clsCode.Substring(0, 5);
                                // ��� = KST+���+����+���+���
                                clsCode = kst + codeValues[1] + codeValues[5] + codeValues[7] + codeValues[3];
                            }
                            else
                            {
                                if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)

                                    // ��� = ���+����+���+���+KST
                                    clsCode = codeValues[1] + codeValues[5] + codeValues[7] + codeValues[3] + kst;
                                else
                                    // ��� = ���+���+Kl
                                    clsCode = codeValues[1] + codeValues[3] + kl.ToString().PadLeft(3, '0');
                            }
                            break;
                    }

                    // ��������� ��������� ���� � ������ ����������
                    if (clsCode == string.Empty || CheckCodeExclusion(clsCode, codeExclusions))
                        continue;
                    // ��������� ��������� kl � ������ ����������
                    // �� ��������� �� KL ��� ����������.������_���������� ������� � ������� 2007 
                    if ((ToCheckKL) && (indPagNo != null))
                        if (!CheckCodeExclusion(kl, indPagNo))
                            continue;
                    PumpRowFromInternalPattern(dt, cls, useCodeMapping, codesMapping, clsProcessModifier, 
                        codeMasks, clsCodeField, clsCode, codeValues, tmpNode, kl);
                }
            }
            WriteToTrace(string.Format("������ �������������� {0} ��������.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ������������ ������ � �������.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xnPattern">������� �����</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attr2FieldMapping">������ ��� ����-������������_��������. ��� ��������, ��� � ��������� ����
        /// ����� �������� �������� �� �������� � ��������������� �������������. ����� ���� ������� ��������� 
        /// ��������� ����� ; - ����� �������� �������� �� ������� ��������, � ������� ��� ����.
        /// "attrName+attrName" - ����� ����������� ������������ �������� ���������.</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� �� 
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� ���� �������������� (��������� ���� �� ������������).
        /// ������ ��������� ������: "code" - ����������� ����, ������ ����������;
        /// "code*" - ����������� ����, ������������ � ����������;
        /// "code1;code2" - ����������� ����, �������� � �������� code1..code2;
        /// "code;" - ����������� ���� >= code;
        /// ";code" - ����������� ����, ������� ��� ������ code;
        /// "*code* - ����������� ����, ���������� code"</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ��������� ����� ������������� 
        /// ��������������</param>
        protected void PumpComplexClsFromInternalPatternXML(XmlNode xnPattern, XmlForm[] xmlForm, int[] sectNo,
            DataTable dt, IClassifier cls, string[] attr2FieldMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sectNo, dt, cls, attr2FieldMapping, null,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ������������ ������ � �������.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xnPattern">������� �����</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - 2 �������: num1, num2;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� �� 
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ��������� ����� ������������� 
        /// ��������������</param>
        protected void PumpComplexClsFromInternalPatternXML(DataTable dt, XmlNode xnPattern, XmlForm[] xmlForm,
            int[] sectNo, IClassifier cls, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sectNo, dt, cls, null, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, null);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ������������ ������ � �������.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xnPattern">������� �����</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ���� - ��� ����. ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - 2 �������: num1, num2;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� �� 
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ��������� ����� ������������� 
        /// ��������������</param>
        protected void PumpComplexClsFromInternalPatternXML(DataTable dt, XmlNode xnPattern, XmlForm[] xmlForm,
            string sectNo, IClassifier cls, string[] attrValuesMapping, bool useCodeMapping,
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo,
            ClsProcessModifier clsProcessModifier)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, CommonRoutines.ParseParamsString(sectNo), dt,
                cls, null, attrValuesMapping, useCodeMapping, codesMapping, codeExclusions, indPagNo,
                clsProcessModifier, null);
        }

        /// <summary>
        /// ���������� ������������� �� �������, ������������ ������ � �������.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xnPattern">������� �����</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="sectNo">����� ������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrValuesMapping">������ ��� ����-����������_��������.
        /// ������: "field1", 3, "field2", 2. ��� ������, ��� ������ 3 ������� ����� �������� � field1, 
        /// ��������� 2 - � field2. ���� ���� = null, �� ������� ������������, ����
        /// ����������_�������� = -1, �� ������� ��� ������� �� ����� ������ ���� ��� ��������� ����,
        /// ����� �������� ����������� ��� ����� ������� � �����</param>
        /// <param name="useCodeMapping">���� � ������ ����� ������� ���� ����, �� ��� �������� �������������� �� 
        /// ���������� �������, ����� ������� ��������� � �������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        /// <param name="indPagNo">������ �������� IndPagNo - ����������� ��� �������. �������� ��. � codeExclusions</param>
        /// <param name="clsProcessModifier">����������� ��������� ��������������. ����� ��� ��������� ������� ��������� ����� ������������� 
        /// ��������������</param>
        /// <param name="codeMasks">������ ����� �������� ���� ��������������. ���� ������, �� ��� ��������������
        /// ����� ������ �� ��������, ���������� �� attr2FieldMapping ��� attrValuesMapping, ����������� ��
        /// ��������� ����� (useCodeMapping false)</param>
        protected void PumpComplexClsFromInternalPatternXML(DataTable dt, XmlNode xnPattern, XmlForm[] xmlForm, 
            int[] sectNo, IClassifier cls, string[] attrValuesMapping, bool useCodeMapping, 
            Dictionary<string, int> codesMapping, string[] codeExclusions, string[] indPagNo, 
            ClsProcessModifier clsProcessModifier, int[] codeMasks)
        {
            PumpComplexClsFromInternalPatternXML(xnPattern, xmlForm, sectNo, dt, cls, null, attrValuesMapping,
                useCodeMapping, codesMapping, codeExclusions, indPagNo, clsProcessModifier, codeMasks);
        }

        /// <summary>
        /// ���������� ������� ������������ ���� ��� �������� ������ ���
        /// </summary>
        /// <param name="xmlNode">������� ���</param>
        /// <param name="fieldMapping">������ ������ ������������ �����</param>
        /// <returns>������� ��� �������� ���</returns>
        private string[] GetFieldMappingForNSICatalog(XmlNode xmlNode, string[] fieldMapping)
        {
            string[] result = null;

            int count = fieldMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                if (xmlNode.Attributes["Code"].Value == fieldMapping[i])
                {
                    if (result == null)
                    {
                        result = new string[2];
                    }
                    else
                    {
                        Array.Resize(ref result, result.GetLength(0) + 2);
                    }

                    Array.Copy(fieldMapping[i + 1].Split(new char[] { ';' }, 2), 0, result, result.GetLength(0) - 2, 2);
                }
            }

            return result;
        }

        /// <summary>
        /// ���������� ������������� �� NSI �������, ������������ ������ � �������.
        /// ��������� ������ ������� ������������ ��������� �����.
        /// </summary>
        /// <param name="xnPattern">������� �������</param>
        /// <param name="dt">������� ��������������</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="attrNames">������ ������������ ��������� ��� �������</param>
        /// <param name="attr2FieldMapping">������ ��� ������������_��������-����. ��� ��������, ��� � ��������� ����
        /// ����� �������� �������� �� �������� � ��������������� �������������. ������ ����:
        /// "fieldName;valueSubStr" - fieldName - ������������ ����, ������ valueSubStr:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - 2 �������: num1, num2;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">������ ���������� (�������� ��. � PumpXMLReportBlock)</param>
        protected void PumpClsFromInternalNSIPatternXML(XmlNode xnPattern, DataTable dt, IClassifier cls,
            string[] attrNames, string[] attr2FieldMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions, bool skipCodeWithLetters)
        {
            string semantic = cls.FullCaption;
            WriteToTrace(string.Format("������� ������ �������������� {0}...", semantic), TraceMessageKind.Information);

            // �������� �������� � ������ � �������������� ��������������
            XmlNodeList xnlNSIData = xnPattern.SelectNodes(string.Format(
                "//NSI/Catalogs/Catalog{0}", GetXPathConstrByAttr("Code", attrNames)));

            for (int i = 0; i < xnlNSIData.Count; i++)
            {
                string[] fieldMapping = GetFieldMappingForNSICatalog(xnlNSIData[i], attr2FieldMapping);
                if (fieldMapping == null) continue;

                string[] exclusions = GetFieldMappingForNSICatalog(xnlNSIData[i], codeExclusions);

                XmlNodeList xnlCatalogItems = xnlNSIData[i].SelectNodes("./CatalogItem");
                for (int j = 0; j < xnlCatalogItems.Count; j++)
                {
                    string code = xnlCatalogItems[j].Attributes["Code"].Value;
                    if ((skipCodeWithLetters) && (CommonRoutines.TrimNumbers(code) != string.Empty))
                        continue;
                    if (this.SkifReportFormat == SKIFFormat.MonthReports)
                        if (CheckCodeExclusion(code, codeExclusions))
                            continue;
                    if (exclusions != null)
                    {
                        if (CheckCodeExclusion(code, exclusions))
                        {
                            continue;
                        }
                    }

                    string[] fieldValues = GetFieldsValuesAsSubstring(fieldMapping, code, "0");
                    if (fieldValues == null) continue;

                    // ��� ��� � ���� ���������� 10 �����
                    if (cls.ObjectKey == "0299a09f-9d23-4e6c-b39a-930cbe219c3a")
                        fieldValues[1] += "0000000000";

                    PumpCachedRow(codesMapping, dt, cls, fieldValues[1], fieldValues[0], new object[] { 
                        "NAME", ConvertClsName(xnlCatalogItems[j].Attributes["Name"].Value), "KL", "-1", "KST", "-1" });
                }
            }

            WriteToTrace(string.Format("������ �������������� {0} ��������.", semantic), TraceMessageKind.Information);
        }

        protected void PumpClsFromInternalNSIPatternXML(XmlNode xnPattern, DataTable dt, IClassifier cls,
            string[] attrNames, string[] attr2FieldMapping, Dictionary<string, int> codesMapping,
            string[] codeExclusions)
        {
            PumpClsFromInternalNSIPatternXML(xnPattern, dt, cls, attrNames, attr2FieldMapping, codesMapping, codeExclusions, true);
        }

        /// <summary>
        /// ���������� �������� �������� �� ����� �� ������
        /// </summary>
        /// <param name="xac">��������� ���������</param>
        /// <param name="attrName">������ ���� ��������</param>
        /// <returns>�������� ��������</returns>
        protected string GetAttrValueByName(XmlAttributeCollection xac, params string[] attrName)
        {
            int count = attrName.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                XmlNode xn = xac.GetNamedItem(attrName[i]);
                if (xn != null)
                {
                    return xn.Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// ���������� ������ �������� ����� ��������������, ���������� ���������� ��������� ��������
        /// </summary>
        /// <param name="fieldsMapping">������ ��� ����-������������_��������. ��� ��������, ��� � ��������� ����
        /// ����� �������� �������� �� �������� � ��������������� �������������. ����� ���� ������� ��������� 
        /// ��������� ����� ; - ����� �������� �������� �� ������� ��������, � ������� ��� ����.
        /// "attrName+attrName" - ����� ����������� ������������ �������� ���������.</param>
        /// <param name="xnlFormRow">������� ���, ���������� �������� �� ���������� �����</param>
        /// <param name="skipEmptyAttr">���� ���������� ������ �������, �� �������� null</param>
        /// <returns>������ �������� �����</returns>
        private string[] GetFieldsValuesAtPos(string[] fieldsMapping, XmlNode xnlFormRow, bool skipEmptyAttr)
        {
            string[] codeValues = new string[fieldsMapping.GetLength(0)];

            int count = fieldsMapping.GetLength(0);
            for (int j = 0; j < count; j += 2)
            {
                codeValues[j] = fieldsMapping[j];
                string attrValue = string.Empty;

                attrValue = GetFieldValueAtPos(fieldsMapping[j + 1], xnlFormRow, false);

                if (attrValue == string.Empty)
                {
                    if (skipEmptyAttr)
                    {
                        return null;
                    }
                    else
                    {
                        codeValues[j + 1] = "0";
                    }
                }
                else
                {
                    codeValues[j + 1] = attrValue;
                }
            }

            return codeValues;
        }

        /// <summary>
        /// ���������� �������� ��������� ��������
        /// </summary>
        /// <param name="attr">������� �� ��������� ��������������. ����� ���� ������� ��������� 
        /// ��������� ����� ; - ����� �������� �������� �� ������� ��������, � ������� ��� ����.
        /// "attrName+attrName" - ����� ����������� ������������ �������� ���������.</param>
        /// <param name="xn">������� ��� � ����������</param>
        /// <param name="allowParent">true - �������� ����� ��������� ������� � ������������ ��������</param>
        /// <returns>��������</returns>
        private string GetFieldValueAtPos(string attr, XmlNode xn, bool allowParent)
        {
            string attrValue = string.Empty;

            XmlNode parent = xn.ParentNode;

            // ����� ���� ������� ��������� 
            // ��������� ����� ; - ����� �������� �������� �� ������� ��������, � ������� ��� ����.
            string[] attrNames = attr.Split(';');

            int count = attrNames.GetLength(0);
            for (int k = 0; k < count; k++)
            {
                // "attrName+attrName" - ����� ����������� ������������ �������� ���������
                string[] attrSummands = attrNames[k].Split('+');

                int summandsCount;
                if (allowParent && parent != null)
                {
                    summandsCount = attrSummands.GetLength(0);
                    for (int m = 0; m < summandsCount; m++)
                    {
                        attrValue += GetAttrValueByName(parent.Attributes, attrSummands[m]);
                    }
                }

                summandsCount = attrSummands.GetLength(0);
                for (int m = 0; m < summandsCount; m++)
                {
                    attrValue += GetAttrValueByName(xn.Attributes, attrSummands[m]);
                }

                if (attrValue != string.Empty)
                {
                    break;
                }
            }

            return attrValue;
        }

        /// <summary>
        /// ���������� ����� ������ �� ���� �����
        /// </summary>
        /// <param name="formCode">��� �����</param>
        /// <returns>����� ������</returns>
        private int GetSectNoFromFormCode(string formCode)
        {
            string str = CommonRoutines.RemoveLetters(formCode);

            if (str.Length >= 5)
            {
                return Convert.ToInt32(str.Substring(str.Length - 2, 2));
            }

            return -1;
        }

        #endregion ������� ������� ����������� ������� XML


        #region ������� ������� ������ ����� XML

        /// <summary>
        /// ���������� ������ ������
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">������ ������� ������</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ �������� ������ �� ��������������</param>
        /// <param name="regionID">�� ������</param>
        /// <param name="columnValues">�������� �������� ����</param>
        private void PumpReportRow(DataTable factTable, IFactTable fct, string date, object[] clsValuesMapping,
            int regionID, object[] columnValues, int budgetLevel, BlockProcessModifier blockProcessModifier)
        {
            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    if (blockProcessModifier == BlockProcessModifier.MRArrears)
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(columnValues, clsValuesMapping,
                            new object[] { "RefYearDayUNV", date, "RefRegion", regionID }));
                    }
                    else
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(columnValues, clsValuesMapping,
                            new object[] { "RefYearDayUNV", date, "REFREGIONS", regionID, "REFBDGTLEVELS", budgetLevel }));
                    }
                    break;

                case SKIFFormat.YearReports:
                    date = string.Format("{0}0001", this.DataSource.Year);
                    if (blockProcessModifier == BlockProcessModifier.YRBalanc)
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays(columnValues, clsValuesMapping,
                            new object[] { "REFYEARDAYUNV", date, "REFREGION", regionID }));
                    }
                    else
                    {
                        PumpRow(factTable, (object[])CommonRoutines.ConcatArrays( columnValues, clsValuesMapping,
                            new object[] { "REFYEARDAYUNV", date, "REFREGIONS", regionID, "REFBDGTLEVELS", budgetLevel }));
                    }
                    break;
            }
        }

        // ���������� ��� �������� ��
        private void PumpPxNodeAll(XmlNode dataNode, DataTable factTable, IFactTable fct,
            string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID,
            BlockProcessModifier blockProcessModifier, int budgetLevel)
        {
            XmlNodeList px = dataNode.SelectNodes("./Px");
            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0) * 2];
            string clsCode = GetClsfCode(dataNode, -1);
            for (int i = 0; i < px.Count; i++)
            {
                double sum = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;

                if (sum != 0)
                {
                    int count = colNo2ColumnMapping.GetLength(0);
                    for (int j = 0; j < count; j++)
                    {
                        columnValues[j] = colNo2ColumnMapping[j];
                        columnValues[j + 1] = sum;
                    }

                    switch (blockProcessModifier)
                    {
                        case BlockProcessModifier.YREmbezzles:
                            if (i >= 9) return;
                            clsValuesMapping[1] = clsCode;
                            clsValuesMapping[3] = GetAttrValueByName(px[i].Attributes, "Num", "ColNo");
                            break;
                    }

                    PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
                }
            }
        }

        // ���������� ��� �������� �� ��� ����� "�������������"
        private void PumpPxNodeArrears(XmlNode dataNode, DataTable factTable, IFactTable fct,
            string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID,
            BlockProcessModifier blockProcessModifier, int budgetLevel)
        {
            XmlNodeList px = dataNode.SelectNodes("./Px");
            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0) * 2];
            for (int i = 0; i < px.Count; i++)
            {
                int num = Convert.ToInt32(GetAttrValueByName(px[i].Attributes, "Num"));
                columnValues[(num - 1) * 2] = colNo2ColumnMapping[num - 1];
                if (num <= 2)
                {
                    columnValues[(num - 1) * 2 + 1] = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;
                }
                else
                {
                    string strValue = px[i].Attributes["Value"].Value;
                    if (strValue.Length > 4000)
                        strValue = strValue.Substring(0, 4000);
                    columnValues[(num - 1) * 2 + 1] = strValue;
                }
            }
            PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
        }

        #region ������ (����� 120, 130, 430)

        private int GetMeansTypeBalanc(XmlForm xmlForm, int num)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form13001))
            {
                if (new int[] { 4, 8 }.Contains(num))
                    return 0;
                if (new int[] { 1, 5 }.Contains(num))
                    return 1;
                if (new int[] { 2, 6 }.Contains(num))
                    return 2;
                if (new int[] { 3, 7 }.Contains(num))
                    return 3;
            }
            else if ((xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002))
            {
                if (new int[] { 3, 6 }.Contains(num))
                    return 0;
                if (new int[] { 1, 4 }.Contains(num))
                    return 1;
                if (new int[] { 2, 5 }.Contains(num))
                    return 2;
            }
            else if ((xmlForm == XmlForm.Form43001) && (this.DataSource.Year >= 2011))
            {
                if (new int[] { 1, 4, 5, 8, 21, 24, 25, 28 }.Contains(num))
                    return 0;
                if (new int[] { 2, 6, 9, 11, 13, 15, 17, 19, 22, 26, 29, 31, 33, 35, 37, 39 }.Contains(num))
                    return 1;
                if (new int[] { 3, 7, 10, 12, 14, 16, 18, 20, 23, 27, 30, 32, 34, 36, 38, 40 }.Contains(num))
                    return 2;
            }
            else if (((xmlForm == XmlForm.Form43001) && (this.DataSource.Year < 2011)) ||
                (xmlForm == XmlForm.Form43002))
            {
                if (new int[] { 1, 4, 19, 22 }.Contains(num))
                    return 0;
                if (new int[] { 2, 5, 7, 9, 11, 13, 15, 17, 20, 23, 25, 27, 29, 31, 33, 35 }.Contains(num))
                    return 1;
                if (new int[] { 3, 6, 8, 10, 12, 14, 16, 18, 21, 24, 26, 28, 30, 32, 34, 36 }.Contains(num))
                    return 2;
            }
            return -1;
        }

        private int GetBudgetLevelBalanc(XmlForm xmlForm, int num, int vbAttrValue)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form12002) ||
                (xmlForm == XmlForm.Form13001) || (xmlForm == XmlForm.Form13002))
            {
                switch (vbAttrValue)
                {
                    case 0: return 1;
                    case 2: return 3;
                    case 3: return 11;
                    case 4: return 4;
                    case 5: return 5;
                    case 10: return 6;
                    case 9: return 8;
                }
            }
            else if ((xmlForm == XmlForm.Form43001) && (this.DataSource.Year >= 2011))
            {
                    if (new int[] { 1, 2, 3, 4, 21, 22, 23, 24 }.Contains(num))
                        return 1;
                    if (new int[] { 5, 6, 7, 8, 25, 26, 27, 28 }.Contains(num))
                        return 2;
                    if (new int[] { 9, 10, 29, 30 }.Contains(num))
                        return 3;
                    if (new int[] { 11, 12, 31, 32 }.Contains(num))
                        return 11;
                    if (new int[] { 13, 14, 33, 34 }.Contains(num))
                        return 4;
                    if (new int[] { 15, 16, 35, 36 }.Contains(num))
                        return 5;
                    if (new int[] { 17, 18, 37, 38 }.Contains(num))
                        return 6;
                    if (new int[] { 19, 20, 39, 40 }.Contains(num))
                        return 8;
            }
            else if (((xmlForm == XmlForm.Form43001) && (this.DataSource.Year < 2011)) ||
                (xmlForm == XmlForm.Form43002))
            {
                if (new int[] { 1, 2, 3, 19, 20, 21 }.Contains(num))
                    return 1;
                if (new int[] { 4, 5, 6, 22, 23, 24 }.Contains(num))
                    return 2;
                if (new int[] { 7, 8, 25, 26 }.Contains(num))
                    return 3;
                if (new int[] { 9, 10, 27, 28 }.Contains(num))
                    return 11;
                if (new int[] { 11, 12, 29, 30 }.Contains(num))
                    return 4;
                if (new int[] { 13, 14, 31, 32 }.Contains(num))
                    return 5;
                if (new int[] { 15, 16, 33, 34 }.Contains(num))
                    return 6;
                if (new int[] { 17, 18, 35, 36 }.Contains(num))
                    return 8;
            }
            return -1;
        }

        private int GetColumnIndexBalanc(XmlForm xmlForm, int num)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form13001))
            {
                if (num >= 5)
                    return 1;
                return 0;
            }
            else if ((xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002))
            {
                if (num >= 4)
                    return 1;
                return 0;
            }
            else if ((xmlForm == XmlForm.Form43001) && (this.DataSource.Year >= 2011))
            {
                if ((num == 4) || (num == 8))
                    return 2;
                if ((num == 24) || (num == 28))
                    return 3;
                if (num < 21)
                    return 0;
                return 1;
            }
            else if (((xmlForm == XmlForm.Form43001) && (this.DataSource.Year < 2011)) ||
                (xmlForm == XmlForm.Form43002))
            {
                if (num >= 19)
                    return 1;
                return 0;
            }
            return 0;
        }

        // ���������� ��� �������� Px ��� ����� "������"
        private void PumpPxNodeBalanc(XmlForm xmlForm, XmlNode dataNode, DataTable factTable, IFactTable fct,
            string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID,
            BlockProcessModifier blockProcessModifier, int budgetLevel, int vbAttrValue)
        {
            XmlNodeList px = dataNode.SelectNodes("./Px");
            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0) * 2];
            for (int i = 0; i < px.Count; i++)
            {
                int num = Convert.ToInt32(GetAttrValueByName(px[i].Attributes, "Num"));
                clsValuesMapping = (object[])CommonRoutines.ConcatArrays(clsValuesMapping, new object[] {
                    "RefBdgtLev", GetBudgetLevelBalanc(xmlForm, num, vbAttrValue),
                    "RefMeansType", GetMeansTypeBalanc(xmlForm, num) });

                columnValues[1] = DBNull.Value;
                columnValues[3] = DBNull.Value;
                columnValues[5] = DBNull.Value;
                columnValues[7] = DBNull.Value;

                double factValue = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;
                int columnIndex = GetColumnIndexBalanc(xmlForm, num);
                columnValues[columnIndex * 2] = colNo2ColumnMapping[columnIndex];
                columnValues[columnIndex * 2 + 1] = factValue;

                PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
            }
        }

        #endregion

        /// <summary>
        /// ���������� �������� �������� �������� �� ����� �� ������
        /// </summary>
        /// <param name="xn">������ ���������</param>
        /// <param name="defaultValue">�������� ��������, ���� �� �� ������ ��� ����� 0</param>
        /// <param name="colNo">������ �������� ColNo.
        /// ColNo ����� ��������� �������� ����� ; - ����� ����� ������� ��� �������, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ��������, -1 - ��� ��������</param>
        /// <param name="usedColNo">ColNo, �� �������� ���� ����� ��������</param>
        /// <returns>�������� ��������</returns>
        private double GetPxNodeValue(XmlNode xn, double defaultValue, string colNo, out int usedColNo)
        {
            usedColNo = -1;
            if (colNo == string.Empty)
            {
                return defaultValue;
            }

            string[] colNoArray = colNo.Split(';');
            XmlNodeList px = xn.SelectNodes(string.Format("./Px{0}", GetXPathConstrByAttr("ColNo", colNoArray)));
            if (px.Count == 0)
            {
                px = xn.SelectNodes(string.Format("./Px{0}", GetXPathConstrByAttr("Num", colNoArray)));
            }

            for (int i = 0; i < px.Count; i++)
            {
                double d = CommonRoutines.ReduceDouble(px[i].Attributes["Value"].Value) * this.SumFactor;
                if (d != 0)
                {
                    usedColNo = Convert.ToInt32(colNoArray[i]);
                    return d;
                }
            }

            return defaultValue;
        }

        // ���������� ������ ��������� ��������
        private void PumpPxNodeStated(List<XmlForm> xmlForm, XmlNode dataNode, DataTable factTable, IFactTable fct, string date, object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier, int budgetLevel, int vbAttrValue)
        {
            if (xmlForm != null && (xmlForm.Contains(XmlForm.Form128) || xmlForm.Contains(XmlForm.Form128v)))
            {
                // ��� ���� ���� ����� �������� �������� �� ������������� ��������
                if (XmlHelper.GetIntAttrValue(dataNode.ParentNode, "��", -1) != vbAttrValue)
                    return;
            }

            object[] columnValues = new object[colNo2ColumnMapping.GetLength(0)];

            // ���������� ��������� �������� ��
            bool zeroSums = true;
            int usedColNo = -1;
            int count = colNo2ColumnMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                double sum = GetPxNodeValue(dataNode, 0, colNo2ColumnMapping[i + 1], out usedColNo) * sumMultiplier;

                if (sum != 0)
                {
                    zeroSums = false;
                }

                columnValues[i] = colNo2ColumnMapping[i];
                if (sum == 0)
                    columnValues[i + 1] = DBNull.Value;
                else
                    columnValues[i + 1] = sum;
            }

            if (!zeroSums)
            {
                PumpReportRow(factTable, fct, date, clsValuesMapping, regionID, columnValues, budgetLevel, blockProcessModifier);
            }
        }

        /// <summary>
        /// ���������� ���� ������� ������
        /// </summary>
        /// <param name="factTable">������� ������, ���� ������</param>
        /// <param name="fct">IFactTable ������� ������</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ �������� ��������������</param>
        /// <param name="colNo2ColumnMapping">������ ��� ���_����-ColNo.
        /// ColNo ����� ��������� �������� ����� ; - ����� ����� ������� ��� �������, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ��������.
        /// ���� � ��������� nodeProcessOption ������� ��������� ���� ���������, �� ������ ������ ���������
        /// ������ ����� �����.</param>
        /// <param name="regionID">�� ������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="nodeProcessOption">��� ��������� �������� � ������� ������.</param>
        /// <param name="budgetLevel">������� �������</param>
        /// <param name="xmlForm">�����</param>
        /// <param name="vbAttrValue">�������� �������� �� �������� Documents, �� �������� ������� ������ ��
        /// ���������� ������ �������</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel, List<XmlForm> xmlForm, int vbAttrValue)
        {
            if (colNo2ColumnMapping == null) return;

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRSrcOutFin:
                    // ��� ����� "��������� �������� ��������������" ������������ ������ �� ��������,
                    // ��� ���� �������� � ���� ��������
                    if (GetAttrValueByName(dataNode.Attributes, "��������", "�����", "ClsfCode") == string.Empty)
                        return;
                    break;

                case BlockProcessModifier.MRSrcInFin:
                    // ��� ����� "��������� ����������� ��������������" ������������ ������ �� ��������,
                    // ��� ���� �������� � ���� �������
                    if (GetAttrValueByName(dataNode.Attributes, "�������", "����", "ClsfCode") == string.Empty)
                        return;
                    break;
            }

            switch (nodeProcessOption)
            {
                case NodeProcessOption.All:
                    PumpPxNodeAll(dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel);
                    break;

                case NodeProcessOption.Arrears:
                    PumpPxNodeArrears(dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel);
                    break;

                case NodeProcessOption.Balanc:
                    PumpPxNodeBalanc(xmlForm[0], dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel, vbAttrValue);
                    break;

                case NodeProcessOption.Stated:
                    PumpPxNodeStated(xmlForm, dataNode, factTable, fct, date, clsValuesMapping,
                        colNo2ColumnMapping, regionID, blockProcessModifier, budgetLevel, vbAttrValue);
                    break;
            }
        }

        /// <summary>
        /// ���������� ���� ������� ������
        /// </summary>
        /// <param name="factTable">������� ������, ���� ������</param>
        /// <param name="fct">IFactTable ������� ������</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ �������� ��������������</param>
        /// <param name="colNo2ColumnMapping">������ ��� ���_����-ColNo.
        /// ColNo ����� ��������� �������� ����� ; - ����� ����� ������� ��� �������, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ��������.
        /// ���� � ��������� nodeProcessOption ������� ��������� ���� ���������, �� ������ ������ ���������
        /// ������ ����� �����.</param>
        /// <param name="regionID">�� ������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="nodeProcessOption">��� ��������� �������� � ������� ������.</param>
        /// <param name="budgetLevel">������� �������</param>
        /// <param name="vbAttrValue">�������� �������� �� �������� Documents, �� �������� ������� ������ ��
        /// ���������� ������ �������</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel, int vbAttrValue)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, colNo2ColumnMapping, regionID,
                blockProcessModifier, nodeProcessOption, budgetLevel, null, vbAttrValue);
        }

        /// <summary>
        /// ���������� ���� ������� ������
        /// </summary>
        /// <param name="factTable">������� ������, ���� ������</param>
        /// <param name="fct">IFactTable ������� ������</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ �������� ��������������</param>
        /// <param name="colNo2ColumnMapping">������ ��� ���_����-ColNo.
        /// ColNo ����� ��������� �������� ����� ; - ����� ����� ������� ��� �������, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ��������.
        /// ���� � ��������� nodeProcessOption ������� ��������� ���� ���������, �� ������ ������ ���������
        /// ������ ����� �����.</param>
        /// <param name="regionID">�� ������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="nodeProcessOption">��� ��������� �������� � ������� ������.</param>
        /// <param name="budgetLevel">������� �������</param>
        /// <param name="xmlForm">�����</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel, List<XmlForm> xmlForm)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, colNo2ColumnMapping, regionID,
                blockProcessModifier, nodeProcessOption, budgetLevel, null, -1);
        }

        /// <summary>
        /// ���������� ���� ������� ������
        /// </summary>
        /// <param name="factTable">������� ������, ���� ������</param>
        /// <param name="fct">IFactTable ������� ������</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ �������� ��������������</param>
        /// <param name="colNo2ColumnMapping">������ ��� ���_����-ColNo.
        /// ColNo ����� ��������� �������� ����� ; - ����� ����� ������� ��� �������, ��� ���� ��������;
        /// ������ ������ ������������ �������� �������� ��������.
        /// ���� � ��������� nodeProcessOption ������� ��������� ���� ���������, �� ������ ������ ���������
        /// ������ ����� �����.</param>
        /// <param name="regionID">�� ������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="nodeProcessOption">��� ��������� �������� � ������� ������.</param>
        /// <param name="budgetLevel">������� �������</param>
        private void ProcessPxNode(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, string[] colNo2ColumnMapping, int regionID, BlockProcessModifier blockProcessModifier,
            NodeProcessOption nodeProcessOption, int budgetLevel)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, colNo2ColumnMapping, regionID, 
                blockProcessModifier, nodeProcessOption, budgetLevel, null);
        }

        /// <summary>
        /// ���������� ������ ����� 414
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm414(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "1",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "2",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "3",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 7);
        }

        private void ProcessForm487Refs2009(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "1", "FACTREPORT", "13", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "2", "FACTREPORT", "14", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 12);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "3", "FACTREPORT", "15", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "4", "FACTREPORT", "16", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 13);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "5", "FACTREPORT", "17", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "6", "FACTREPORT", "18", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 17);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "7", "FACTREPORT", "19", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 4);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "8", "FACTREPORT", "20", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 14);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "9", "FACTREPORT", "21", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "10", "FACTREPORT", "22", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 15);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "11", "FACTREPORT", "23", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", "12", "FACTREPORT", "24", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 16);
        }

        /// <summary>
        /// ���������� ������ ����� 487
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm487(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            bool isRefsData = ((blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx) ||
                               (blockProcessModifier == BlockProcessModifier.MRCommonBooks) ||
                               (blockProcessModifier == BlockProcessModifier.MRExcessBooks));
            // c 2009 ���� - ��������� � ������� ����� ����
            if ((this.DataSource.Year >= 2009) && (isRefsData))
            {
                ProcessForm487Refs2009(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                return;
            }

            string assignedSumColumn = string.Empty;
            if (isRefsData)
                assignedSumColumn = "1";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "7", 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);

            if (isRefsData)
                assignedSumColumn = "2";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "8",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            if (isRefsData)
                assignedSumColumn = "3";
            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "9",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            else
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "9",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);

            if (isRefsData)
                assignedSumColumn = "4";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "10",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);

            if (isRefsData)
                assignedSumColumn = "5";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "11",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);

            if (isRefsData)
                assignedSumColumn = "6";
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                    "MONTHPLANREPORT", string.Empty, "ASSIGNEDREPORT", assignedSumColumn, "FACTREPORT", "12",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
        }

        /// <summary>
        /// ���������� ������ ����� 623
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm623(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "2" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "3" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "ASSIGNEDREPORT", string.Empty, "PERFORMEDREPORT", "4" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 7);

            switch (regionCode.Length)
            {
                case 2:
                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                        new string[] { "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", string.Empty },
                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                    break;

                case 5:
                    if (regionCode[2] == '0')
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", string.Empty },
                            regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                    }
                    if (regionCode.EndsWith("900") || regionCode.EndsWith("100"))
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", string.Empty },
                            regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                    }
                    break;
            }
        }

        /// <summary>
        /// ���������� ������ ����� 625
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm625(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            date = string.Format("{0}0000", this.DataSource.Year);
            if (this.DataSource.Year >= 2010)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "1", "EndYearRep", "4", "BudMidYRep", "7", "FMidYRep", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "2", "EndYearRep", "5", "BudMidYRep", "8", "FMidYRep", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "3", "EndYearRep", "6", "BudMidYRep", "9", "FMidYRep", "12" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 9);
            }
            else
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                    new string[] { "BegYearRep", "1", "EndYearRep", "2", "BudMidYRep", "3", "FMidYRep", "4" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            }
        }

        private void ProcessForm624(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            date = string.Format("{0}0000", this.DataSource.Year);
            int budLevel = 2;
            if (this.DataSource.Year >= 2010)
                budLevel = 4;
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "BegYearRep", "2", "EndYearRep", "6", "BudMidYRep", "10", "FMidYRep", "14" },
                regID, blockProcessModifier, NodeProcessOption.Stated, budLevel);
            if (this.DataSource.Year >= 2010)
                budLevel = 5;
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "BegYearRep", "3", "EndYearRep", "7", "BudMidYRep", "11", "FMidYRep", "15" },
                regID, blockProcessModifier, NodeProcessOption.Stated, budLevel);
            if (this.DataSource.Year >= 2010)
                budLevel = 6;
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "BegYearRep", "4", "EndYearRep", "8", "BudMidYRep", "12", "FMidYRep", "16" },
                regID, blockProcessModifier, NodeProcessOption.Stated, budLevel);
        }

        /// <summary>
        /// ���������� ������ ����� 630
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm630(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            string regionCode, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            switch (regionCode.Length)
            {
                case 2:
                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                        new string[] { "FACT" }, regID, blockProcessModifier, NodeProcessOption.All, 2);
                    break;

                case 5:
                    if (regionCode[2] == '0')
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "FACT" }, regID, blockProcessModifier, NodeProcessOption.All, 7);
                    }
                    if (regionCode.EndsWith("900") || regionCode.EndsWith("100"))
                    {
                        ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                            new string[] { "FACT" }, regID, blockProcessModifier, NodeProcessOption.All, 3);
                    }
                    break;

                default: return;
            }
        }

        /// <summary>
        /// ���������� ������ ����� 128
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm128(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            string monthNum = string.Empty;
            string factNum = string.Empty;
            string spreadYearNum = string.Empty;
            string spreadMonthNum = string.Empty;

            if (blockProcessModifier == BlockProcessModifier.MRDefProf ||
                blockProcessModifier == BlockProcessModifier.MROutcomes)
            {
                monthNum = "2";
                factNum = "3";
                spreadYearNum = "4";
                spreadMonthNum = "5";
            }
            else if (blockProcessModifier == BlockProcessModifier.MRIncomes || 
                blockProcessModifier == BlockProcessModifier.MRSrcInFin ||
                blockProcessModifier == BlockProcessModifier.MRSrcOutFin)
            {
                factNum = "2";
                spreadYearNum = "3";
            }

            List<XmlForm> xmlForm = new List<XmlForm>(2);
            xmlForm.Add(XmlForm.Form128);
            xmlForm.Add(XmlForm.Form128v);

            if (this.skifFormat == SKIFFormat.MonthReports)
            {

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3, xmlForm, 2);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4, xmlForm, 4);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5, xmlForm, 5);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6, xmlForm, 10);

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8, xmlForm, 9);

                if (this.DataSource.Year * 100 + this.DataSource.Month >= 200704)
                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", monthNum, "FACTREPORT", factNum,
                    "SPREADFACTYEARPLANREPORT", spreadYearNum, "SPREADFACTMONTHPLANREPORT", spreadMonthNum },
                        regID, blockProcessModifier, NodeProcessOption.Stated, 11, xmlForm, 3);
            }
            else
            {
                string performedNum = "2";
                if ((blockProcessModifier == BlockProcessModifier.YROutcomes) ||
                    (blockProcessModifier == BlockProcessModifier.YRDefProf))
                    performedNum = "3";

                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3, xmlForm, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7, xmlForm, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4, xmlForm, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5, xmlForm, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6, xmlForm, 10);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8, xmlForm, 9);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "AssignedReport", "1", "PerformedReport", performedNum },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2, xmlForm, 0);
            }
        }

        #region ����� 117

        private string[] GetSumsMappingForm117()
        {
            if (this.SkifReportFormat == SKIFFormat.MonthReports)
                return new string[] { "YEARPLANREPORT", "1", "FACTREPORT", "2", "SPREADFACTYEARPLANREPORT", "3" };
            else
                return new string[] { "AssignedReport", "1", "PerformedReport", "2" };
        }

        private int GetBudgetLevelForm117(int vb)
        {
            switch (vb)
            {
                case 0:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 11;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 9:
                    return 8;
                case 10:
                    return 6;
                default:
                    return 1;
            }
        }

        private void ProcessForm117(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            int vb = XmlHelper.GetIntAttrValue(dataNode.ParentNode, "��", -1);
            int budgetLevel = GetBudgetLevelForm117(vb);
            string[] sumsMapping = GetSumsMappingForm117();
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, sumsMapping,
                regID, blockProcessModifier, NodeProcessOption.Stated, budgetLevel);
        }

        #endregion ����� 117

        private void ProcessForm628(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            int budgetLevel = 1;
            for (int i = 1; i <= 8; i++)
            {
                int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
                if ((sourceDate >= 200704) && (i == 4))
                    continue;
                string quarterPlanReport = Convert.ToString(i + 8);
                string monthPlanReport = Convert.ToString(i + 16);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                    "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", quarterPlanReport, 
                    "MONTHPLANREPORT", monthPlanReport, "ASSIGNEDREPORT", string.Empty, "FACTREPORT", string.Empty, 
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, budgetLevel);
                budgetLevel++;
            }
        }

        #region ����� 428

        private void ProcessForm428Account(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "1" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "2" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "3" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 4);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "4" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "5" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { "ArrivalRep", "6" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 8);
        }

        private void ProcessForm428_2011(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "1", "FACTREPORT", "11", "ExcSumPRep", "2", "ExcSumFRep", "12" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 1);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "3", "FACTREPORT", "13", "ExcSumPRep", "4", "ExcSumFRep", "14" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "5", "FACTREPORT", "15" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "6", "FACTREPORT", "16" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 11);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "7", "FACTREPORT", "17" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 4);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "8", "FACTREPORT", "18" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "9", "FACTREPORT", "19" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                new string[] { "YEARPLANREPORT", "10", "FACTREPORT", "20" },
                regID, blockProcessModifier, NodeProcessOption.Stated, 8);
        }

        /// <summary>
        /// ���������� ������ ����� 428 (������)
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="xmlForm">��� �����</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm428MR(DataTable factTable, IFactTable fct, XmlNode dataNode, XmlForm xmlForm,
            string date, object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            if (blockProcessModifier == BlockProcessModifier.MRAccount)
            {
                ProcessForm428Account(factTable, fct, dataNode, date,
                    clsValuesMapping, blockProcessModifier, regID);
                return;
            }

            if (this.DataSource.Year >= 2011)
            {
                ProcessForm428_2011(factTable, fct, dataNode, date,
                    clsValuesMapping, blockProcessModifier, regID);
                return;
            }

            if ((xmlForm == XmlForm.Form428v) && (this.DataSource.Year >= 2008))
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "1", "YEARPLAN", "1", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "4",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "2", "YEARPLAN", "2", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "5",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "3", "YEARPLAN", "3", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "6",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                return;
            }

            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "1", "YEARPLAN", "1", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "9",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 1);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "2", "YEARPLAN", "2", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "10",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "3", "YEARPLAN", "3", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "11",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 3);

            if (this.DataSource.Year * 100 + this.DataSource.Month >= 200704)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "4", "YEARPLAN", "4", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "12",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 11);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "5", "YEARPLAN", "5", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "13",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "6", "YEARPLAN", "6", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "14",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "7", "YEARPLAN", "7", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "15",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
            }
            else
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "4", "YEARPLAN", "4", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "12",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "5", "YEARPLAN", "5", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "13",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "6", "YEARPLAN", "6", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "14",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "YEARPLANREPORT", "7", "YEARPLAN", "7", "QUARTERPLANREPORT", string.Empty,
                    "MONTHPLANREPORT", string.Empty, "FACTREPORT", "15",
                    "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7);
            }
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "YEARPLANREPORT", "8", "YEARPLAN", "8", "QUARTERPLANREPORT", string.Empty,
                "MONTHPLANREPORT", string.Empty, "FACTREPORT", "16",
                "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                regID, blockProcessModifier, NodeProcessOption.Stated, 8);
        }

        /// <summary>
        /// ���������� ������ ����� 428 (������)
        /// </summary>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="xmlForm">��� �����</param>
        /// <param name="date">����</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������</param>
        /// <param name="regID">�� ������</param>
        private void ProcessForm428YR(DataTable factTable, IFactTable fct, XmlNode dataNode, XmlForm xmlForm, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            if ((this.DataSource.Year >= 2011) && (xmlForm == XmlForm.Form428g))
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "11", "ASSIGNED", "1", "PERFORMED", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "13", "ASSIGNED", "3", "PERFORMED", "13" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "15", "ASSIGNED", "5", "PERFORMED", "15" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "6", "PERFORMEDREPORT", "16", "ASSIGNED", "6", "PERFORMED", "16" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 11);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "7", "PERFORMEDREPORT", "17", "ASSIGNED", "7", "PERFORMED", "17" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "8", "PERFORMEDREPORT", "18", "ASSIGNED", "8", "PERFORMED", "18" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "9", "PERFORMEDREPORT", "19", "ASSIGNED", "9", "PERFORMED", "19" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "10", "PERFORMEDREPORT", "20", "ASSIGNED", "10", "PERFORMED", "20" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "EXCSUMPREP", "2", "EXCSUMFREP", "12", "EXCSUMP", "2", "EXCSUMF", "12" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "EXCSUMPREP", "4", "EXCSUMFREP", "14", "EXCSUMP", "4", "EXCSUMF", "14" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
            }
            else if ((this.DataSource.Year >= 2007) && (xmlForm == XmlForm.Form428g))
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "9", "ASSIGNED", "1", "PERFORMED", "9" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "10", "ASSIGNED", "2", "PERFORMED", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "11", "ASSIGNED", "3", "PERFORMED", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "13", "ASSIGNED", "5", "PERFORMED", "13" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "6", "PERFORMEDREPORT", "14", "ASSIGNED", "6", "PERFORMED", "14" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "7", "PERFORMEDREPORT", "15", "ASSIGNED", "7", "PERFORMED", "15" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "8", "PERFORMEDREPORT", "16", "ASSIGNED", "8", "PERFORMED", "16" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
            }
            else if (this.DataSource.Year >= 2007)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "4", "ASSIGNED", "1", "PERFORMED", "4" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "5", "ASSIGNED", "2", "PERFORMED", "5" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "6", "ASSIGNED", "3", "PERFORMED", "6" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
            }
            else if (this.DataSource.Year >= 2006)
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "10", "ASSIGNED", "1", "PERFORMED", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "11", "ASSIGNED", "2", "PERFORMED", "11" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "12", "ASSIGNED", "3", "PERFORMED", "12" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "14", "ASSIGNED", "5", "PERFORMED", "14" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 4);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "6", "PERFORMEDREPORT", "15", "ASSIGNED", "6", "PERFORMED", "15" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 5);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "7", "PERFORMEDREPORT", "16", "ASSIGNED", "7", "PERFORMED", "16" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 6);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "8", "PERFORMEDREPORT", "17", "ASSIGNED", "8", "PERFORMED", "17" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                    "ASSIGNEDREPORT", "9", "PERFORMEDREPORT", "18", "ASSIGNED", "9", "PERFORMED", "18" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
            }
            else
            {
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "1", "PERFORMEDREPORT", "6", "ASSIGNED", "1", "PERFORMED", "6" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 1);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "2", "PERFORMEDREPORT", "7", "ASSIGNED", "2", "PERFORMED", "7" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 2);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "3", "PERFORMEDREPORT", "8", "ASSIGNED", "3", "PERFORMED", "8" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 3);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "4", "PERFORMEDREPORT", "9", "ASSIGNED", "4", "PERFORMED", "9" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                "ASSIGNEDREPORT", "5", "PERFORMEDREPORT", "10", "ASSIGNED", "5", "PERFORMED", "10" },
                    regID, blockProcessModifier, NodeProcessOption.Stated, 8);
            }
        }

        #endregion ����� 428

        #region ����� 127

        private string[] GetSumsMappingForm127(BlockProcessModifier blockProcessModifier)
        {
            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRIncomes:
                case BlockProcessModifier.MRSrcInFin:
                case BlockProcessModifier.MRSrcOutFin:
                    return new string[] { "YearPlanReport", "1", "FactReport", "5", "SpreadFactYearPlanReport", "6" };
                case BlockProcessModifier.MROutcomes:
                case BlockProcessModifier.MRDefProf:
                    return new string[] { "YearPlanReport", "1", "MonthPlanReport", "2", "FactReport", "6", 
                        "SpreadFactYearPlanReport", "7", "SpreadFactMonthPlanReport", "8" };

                case BlockProcessModifier.YRIncomes:
                case BlockProcessModifier.YRSrcFin:
                    return new string[] { "AssignedReport", "1", "PerformedReport", "5" };
                case BlockProcessModifier.YROutcomes:
                case BlockProcessModifier.YRDefProf:
                    return new string[] { "AssignedReport", "1", "PerformedReport", "6" };
                default:
                    return new string[] { };
            }
        }

        private int GetBudgetLevelForm127(int vb)
        {
            switch (vb)
            {
                case 0:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 7;
                case 4:
                    return 4;
                case 5:
                    return 5;
                case 9:
                    return 8;
                case 10:
                    return 6;
                case 33:
                    return 11;
                default:
                    return 1;
            }
        }

        private void ProcessForm127(DataTable factTable, IFactTable fct, XmlNode dataNode, string date,
            object[] clsValuesMapping, BlockProcessModifier blockProcessModifier, int regID)
        {
            int vb = XmlHelper.GetIntAttrValue(dataNode.ParentNode, "��", -1);
            int budgetLevel = GetBudgetLevelForm127(vb);
            string[] sumsMapping = GetSumsMappingForm127(blockProcessModifier);
            ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, sumsMapping,
                regID, blockProcessModifier, NodeProcessOption.Stated, budgetLevel);
        }

        #endregion ����� 127

        /// <summary>
        /// ���������� �������� � ������� �� �������
        /// </summary>
        /// <param name="da">����������� ������� ������</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="dataNode">������� � ������� �� ��������������</param>
        /// <param name="xmlForm">����� �����</param>
        /// <param name="regionCode">��� ������</param>
        /// <param name="date">����</param>
        /// <param name="regionCode">�������� ��� ������</param>
        /// <param name="clsValuesMapping">������ ��� ����-�������� ��������������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="regID">�� ������</param>
        private void ProcessPxNodes(IDbDataAdapter da, DataTable factTable, IFactTable fct, XmlNode dataNode,
            XmlForm xmlForm, int regionID, string date, string regionCode, object[] clsValuesMapping,
            BlockProcessModifier blockProcessModifier, int nullRegions, int vbAttrValue)
        {
            int regID = regionID;
            if (regID < 0)
            {
                regID = nullRegions;
            }

            switch (this.SkifReportFormat)
            {
                #region SKIFFormat.MonthReports

                case SKIFFormat.MonthReports:
                    switch (this.XmlReportFormat)
                    {
                        #region XmlFormat.Format2004
                        case XmlFormat.Format2004:
                            switch (xmlForm)
                            {
                                case XmlForm.Form649:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "1", "QUARTERPLANREPORT", "5", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "2", "QUARTERPLANREPORT", "6", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "3", "QUARTERPLANREPORT", "7", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;

                                case XmlForm.Form650:
                                case XmlForm.Form651:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "1",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "2",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "3",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;
                            }
                            break;
                        #endregion XmlFormat.Format2004

                        #region XmlFormat.Format2005
                        case XmlFormat.Format2005:
                            switch (xmlForm)
                            {
                                case XmlForm.Form649:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "1", "QUARTERPLANREPORT", "4", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "2", "QUARTERPLANREPORT", "5", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "3", "QUARTERPLANREPORT", "6", 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", string.Empty,
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;

                                case XmlForm.Form650:
                                case XmlForm.Form651:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "1",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "2",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", string.Empty, "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "3",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.October2005
                        case XmlFormat.October2005:
                            switch (xmlForm)
                            {
                                case XmlForm.Form414:
                                    ProcessForm414(factTable, fct, dataNode, date, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form428:
                                case XmlForm.Form428v:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "1", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "5",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 2);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "2", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "6",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 3);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "3", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "7",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 8);

                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] { 
                                            "YEARPLANREPORT", "4", "QUARTERPLANREPORT", string.Empty, 
                                            "MONTHPLANREPORT", string.Empty, "FACTREPORT", "8",
                                            "SPREADFACTYEARPLANREPORT", string.Empty, "SPREADFACTMONTHPLANREPORT", string.Empty },
                                        regID, blockProcessModifier, NodeProcessOption.Stated, 7);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.Skif3
                        case XmlFormat.Skif3:
                            switch (xmlForm)
                            {
                                case XmlForm.Form128:
                                case XmlForm.Form128v:
                                    ProcessForm128(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form414:
                                    ProcessForm414(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form487:
                                    ProcessForm487(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form428:
                                case XmlForm.Form428v:
                                    ProcessForm428MR(factTable, fct, dataNode, xmlForm, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form117:
                                    ProcessForm117(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form628r:
                                    ProcessForm628(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form127:
                                case XmlForm.Form127g:
                                case XmlForm.Form127v:
                                    ProcessForm127(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;
                                case XmlForm.Form159:
                                case XmlForm.Form169:
                                case XmlForm.Form159V:
                                case XmlForm.Form169V:
                                case XmlForm.Form469:
                                case XmlForm.Form459:
                                case XmlForm.Form469V:
                                case XmlForm.Form459V:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping, new string[] {
                                        "Arrears", "OutArrears", "Contractor", "Occasion" }, regID, blockProcessModifier,
                                        NodeProcessOption.Arrears, -1);
                                    break;
                            }
                            break;
                        #endregion
                    }
                    break;

                #endregion SKIFFormat.MonthReports

                #region SKIFFormat.YearReports

                case SKIFFormat.YearReports:
                    switch (this.XmlReportFormat)
                    {
                        #region XmlFormat.Format2004
                        case XmlFormat.Format2004:
                            switch (xmlForm)
                            {
                                case XmlForm.Form623:
                                    ProcessForm623(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form625:
                                    ProcessForm625(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form630:
                                    ProcessForm630(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.Format2005
                        case XmlFormat.Format2005:
                            switch (xmlForm)
                            {
                                case XmlForm.Form623:
                                    ProcessForm623(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form625:
                                    ProcessForm625(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form630:
                                    ProcessForm630(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;
                            }
                            break;
                        #endregion

                        #region XmlFormat.October2005
                        case XmlFormat.October2005:
                            break;
                        #endregion

                        #region XmlFormat.Skif3
                        case XmlFormat.Skif3:
                            switch (xmlForm)
                            {
                                case XmlForm.Form128:
                                case XmlForm.Form128v:
                                    ProcessForm128(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form428g:
                                case XmlForm.Form428Vg:
                                    ProcessForm428YR(factTable, fct, dataNode, xmlForm, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form625:
                                    ProcessForm625(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form624:
                                    ProcessForm624(factTable, fct, dataNode, date, regionCode, clsValuesMapping,
                                        blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form127:
                                case XmlForm.Form127g:
                                case XmlForm.Form127v:
                                    ProcessForm127(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form117:
                                    ProcessForm117(factTable, fct, dataNode, date, clsValuesMapping, blockProcessModifier, regID);
                                    break;

                                case XmlForm.Form12001:
                                case XmlForm.Form13001:
                                case XmlForm.Form43001:
                                case XmlForm.Form12002:
                                case XmlForm.Form13002:
                                case XmlForm.Form43002:
                                    ProcessPxNode(factTable, fct, dataNode, date, clsValuesMapping,
                                        new string[] { "BegYear", "EndYear", "ExceptBegYear", "ExceptEndYear" },
                                        regID, blockProcessModifier, NodeProcessOption.Balanc, -1,
                                        new List<XmlForm>(new XmlForm[] { xmlForm }), vbAttrValue);
                                    break;
                            }
                            break;
                        #endregion
                    }
                    break;

                #endregion SKIFFormat.YearReports
            }
        }

        /// <summary>
        /// ���������� ����������� ��� ������� �� ��� �� ��������
        /// </summary>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="attrValues">������ � ����������� ��������</param>
        /// <returns>�����������</returns>
        protected string GetXPathConstrByAttr(string attrName, string[] attrValues)
        {
            string result = string.Empty;

            int count = attrValues.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result += string.Format(" or @{0} = \"{1}\"", attrName, attrValues[i]);
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(0, 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// ���������� ����������� ��� ������� �� ��� �� ��������
        /// </summary>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="attrValues">������ � ����������� ��������</param>
        /// <returns>�����������</returns>
        protected string GetXPathConstrByAttr(string attrName, int[] attrValues)
        {
            string result = string.Empty;

            int count = attrValues.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                result += string.Format(" or @{0} = \"{1}\"", attrName, attrValues[i]);
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(0, 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// ���������� ����������� ��� ������� �� ��� �� ��������
        /// </summary>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="xmlForm">������ ������� ����</param>
        /// <returns>�����������</returns>
        protected string GetXPathConstrByAttr(string attrName, XmlForm[] xmlForm)
        {
            string result = string.Empty;

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                result += string.Format(" or @{0} = \"{1}\"", attrName, XmlFormToString(xmlForm[i]));
            }

            if (result != string.Empty)
            {
                result = "[" + result.Remove(0, 4) + "]";
            }

            return result;
        }

        /// <summary>
        /// ���������� ����������� ��� ������� �� ��� �� FormNo.
        /// </summary>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="attrValues">������ � ����������� ��������</param>
        /// <returns>�����������</returns>
        private string GetXPathConstrByFormNo(int sectNo, XmlForm[] xmlForm, XmlFormat xmlFormat)
        {
            string result = string.Empty;

            for (int i = 0; i < xmlForm.GetLength(0); i++)
            {
                switch (xmlFormat)
                {
                    case XmlFormat.Format2004:
                        result += string.Format(" or @FormNo = \"{0}\"", XmlFormToString(xmlForm[i]));
                        break;

                    case XmlFormat.Format2005:
                    case XmlFormat.October2005:
                        result += string.Format(
                            " or @Code = \"{0}{1}\"",
                            XmlFormToString(xmlForm[i]), sectNo.ToString().PadLeft(2, '0'));
                        break;
                }
            }

            if (result != string.Empty)
            {
                // ������� ������ or
                result = "[" + result.Remove(0, 3).Trim() + "]";

                switch (xmlFormat)
                {
                    case XmlFormat.Format2004:
                        result = string.Format("//Forma{0}/Section[@SectNo = \"{1}\"]", result, sectNo);
                        break;

                    case XmlFormat.Format2005:
                    case XmlFormat.October2005:
                        result = string.Format("//Form{0}", result);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// ������� ������ �������� ������ �� �������������� � ����������� �� �����
        /// </summary>
        /// <param name="formNo">����� �����</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <returns>������ ��������</returns>
        private object[] PrepareIndividualCodesMappingXML(XmlForm xmlForm, BlockProcessModifier blockProcessModifier)
        {
            object[] result = new object[0];

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRStandard:
                case BlockProcessModifier.MRDefProf:
                case BlockProcessModifier.MRIncomes:
                case BlockProcessModifier.MROutcomes:
                case BlockProcessModifier.MRSrcInFin:
                case BlockProcessModifier.MRSrcOutFin:
                case BlockProcessModifier.MRAccount:
                case BlockProcessModifier.YRDefProf:
                case BlockProcessModifier.YROutcomes:
                case BlockProcessModifier.YRIncomes:
                case BlockProcessModifier.YRSrcFin:
                    Array.Resize(ref result, 2);
                    result[0] = "REFMEANSTYPE";

                    switch (xmlForm)
                    {
                        case XmlForm.Form414:
                        case XmlForm.Form487:
                            break;
                        case XmlForm.Form128:
                        case XmlForm.Form428:
                        case XmlForm.Form649:
                        case XmlForm.Form650:
                        case XmlForm.Form428g:
                        case XmlForm.Form623:
                        case XmlForm.Form117:
                        case XmlForm.Form628r:
                        case XmlForm.Form127:
                        case XmlForm.Form127g:
                            result[1] = 1;
                            break;
                        case XmlForm.Form128v:
                        case XmlForm.Form428v:
                        case XmlForm.Form428Vg:
                        case XmlForm.Form127v:
                            result[1] = 2;
                            break;
                        case XmlForm.Form651:
                            break;
                    }

                    break;
                case BlockProcessModifier.MRArrears:
                    Array.Resize(ref result, 4);
                    result[0] = "RefMeansType";
                    result[2] = "RefType";

                    switch (xmlForm)
                    {
                        case XmlForm.Form159:
                        case XmlForm.Form459:
                            result[1] = 1;
                            result[3] = 1;
                            break;
                        case XmlForm.Form159V:
                        case XmlForm.Form459V:
                            result[1] = 2;
                            result[3] = 1;
                            break;
                        case XmlForm.Form169:
                        case XmlForm.Form469:
                            result[1] = 1;
                            result[3] = 0;
                            break;
                        case XmlForm.Form169V:
                        case XmlForm.Form469V:
                            result[1] = 2;
                            result[3] = 0;
                            break;
                    }

                    break;
            }

            return result;
        }

        /// <summary>
        /// ��������� ��� �������������� ��� ������ ������ �� �������������� ������ ������
        /// </summary>
        /// <param name="clsCodeAttr">�������� ����</param>
        /// <param name="xn">������� �� ������� ������</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� �������������� 
        /// �������� ��� ������-���� �� ������.</param>
        /// <returns>���</returns>
        private string GetClsCodeFromFactNode(string clsCodeAttr, XmlNode xn, BlockProcessModifier blockProcessModifier)
        {
            string result = string.Empty;

            if (clsCodeAttr == string.Empty)
            {
                result = GetClsfCode(xn, -1);
            }
            else
            {
                result = GetFieldValueAtPos(clsCodeAttr, xn, true);
            }

            switch (blockProcessModifier)
            {
                case BlockProcessModifier.YREmbezzles: return result.TrimStart('0');
            }

            return result;
        }

        /// <summary>
        /// �������� ������ ������ �� ��������� ����������
        /// </summary>
        /// <param name="xn">������� � ������� ������</param>
        /// <param name="sectNo">������ ������</param>
        /// <param name="xmlForm">������ ���� ������</param>
        /// <returns>�������� � ������� ������</returns>
        private XmlNodeList GetFactDocumentNodes(XmlNode xn, int[] sectNo, XmlForm[] xmlForm)
        {
            switch (this.XmlReportFormat)
            {
                case XmlFormat.Format2004:
                    return xn.SelectNodes(string.Format(
                        ".//Forma{0}/Section{1}",
                        GetXPathConstrByAttr("FormNo", xmlForm), GetXPathConstrByAttr("SectNo", sectNo)));

                default:
                    return xn.SelectNodes(string.Format(
                        ".//Form{0}/Document", GetConstrForInternalPattern(xmlForm, sectNo)));
            }
        }

        /// <summary>
        /// ���������� ���������� ��������� � �������
        /// </summary>
        /// <param name="xn">�������� ������� (��� ������)</param>
        /// <param name="formSection">������ ��� �����_�����-�����_������. ������ ���� ����������, ������ �� ����� 
        /// ������ � ���� ����� �������� (������������ ������ ������). ������ ���� ������ ���� ����������� ����� ";".
        /// ������ �����_������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� � 
        /// �������� 2)</param>
        /// <returns>���������� ��������� � �������</returns>
        private int GetTotalDataNodes(XmlNode xn, string[] formSection)
        {
            int result = 0;

            for (int i = 0; i < formSection.GetLength(0); i += 2)
            {
                XmlNodeList documents = GetFactDocumentNodes(xn,
                    CommonRoutines.ParseParamsString(formSection[i + 1]), StringToXmlForms(formSection[i]));
                foreach (XmlNode document in documents)
                {
                    result += document.SelectNodes("Data").Count;
                }
            }

            return result;
        }

        /// <summary>
        /// ���������, ����� ��� ������������� �� ���� ���������������
        /// </summary>
        /// <param name="fieldsMapping">������ ��� ����_����_��������������-��������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="clsCode">��� �������������� �� ���</param>
        /// <returns>true - ��� ������ �� ���� codesMapping, ����� � �����-�� ���.</returns>
        private void FindClsIDByCode(object[] fieldsMapping, Dictionary<string, int>[] codesMapping,
            string clsCode)
        {
            int count = fieldsMapping.GetLength(0);
            for (int i = 0; i < count; i += 2)
            {
                if (codesMapping[i / 2].ContainsKey(clsCode))
                {
                    fieldsMapping[i + 1] = codesMapping[i / 2][clsCode];
                }
                else
                {
                    throw new Exception(string.Format("��� {0} �� ��������� � ����������� ��������������", clsCode));
                }
            }
        }

        /// <summary>
        /// ���������, ����� ��� ������������� �� ���� ���������������. ���� ��� ���-�� ���, �� �������� ������
        /// �� "����������� �������������"
        /// </summary>
        /// <param name="clsTable">������� ���������������</param>
        /// <param name="cls">������� ���������������</param>
        /// <param name="factRefsToCls">������ ������ �� �������������� �� ������</param>
        /// <param name="codesMapping">���� ���������������</param>
        /// <param name="clsValuesMapping">������ ����-�������� ������ �� �������������</param>
        /// <param name="clsCode">��� ��������������</param>
        private void CheckClsIDByCodeWithUnknown(DataTable[] clsTable, IClassifier[] cls, string[] factRefsToCls,
            Dictionary<string, int>[] codesMapping, object[] clsValuesMapping, string clsCode)
        {
            int count = factRefsToCls.GetLength(0);
            for (int i = 0; i < count; i++)
            {
                if (!codesMapping[i].ContainsKey(clsCode))
                {
                    if (codesMapping[i].ContainsKey("0"))
                    {
                        clsValuesMapping[i * 2 + 1] = codesMapping[i]["0"];
                    }
                    else
                    {
                        clsValuesMapping[i * 2 + 1] = PumpCachedRow(
                            codesMapping[i], clsTable[i], cls[i], "0", new object[] { 
                                GetClsCodeField(cls[i]), "0", "NAME", "����������� �������������",
                                "KL", -1, "KST", -1 });
                    }
                }
                else
                {
                    clsValuesMapping[i * 2 + 1] = codesMapping[i][clsCode];
                }
            }
        }

        /// <summary>
        /// ��������� ������ �������� �� ���������������. �������� ���������� ��. � PumpXMLReportBlock.
        /// </summary>
        /// <returns>������ �������� ������� ��� ���</returns>
        private bool GetClsIDForFact(DataTable[] clsTable, IClassifier[] cls, string[] factRefsToCls,
            int[] nullRefsToCls, Dictionary<string, int>[] codesMapping, BlockProcessModifier blockProcessModifier,
            int yearIndex, object[] clsValuesMapping, string clsCodeField, string clsCode, string[] attr2ClsMapping)
        {
            if (clsTable == null || cls == null || factRefsToCls == null || nullRefsToCls == null)
                return true;
            int clsID;
            try
            {
                if ((blockProcessModifier == BlockProcessModifier.MROutcomes) && (this.DataSource.Year >= 2005))
                    clsCode = clsCode.PadLeft(20, '0');
                clsCode = clsCode.ToUpper().Replace('X', '0').Replace('�', '0');
                switch (blockProcessModifier)
                {
                    case BlockProcessModifier.MROutcomes:
                    case BlockProcessModifier.MRIncomesBooks:
                    case BlockProcessModifier.MROutcomesBooks:
                    case BlockProcessModifier.YROutcomes:
                        // �������� �������� ����� ���������������
                        string[] codeValues = GetCodeValuesAsSubstring(attr2ClsMapping, clsCode, "0");
                        if (codeValues == null)
                        {
                            codeValues = new string[clsTable.GetLength(0)];
                            CommonRoutines.InitArray(codeValues, clsCode);
                        }

                        // ��� ��� � ���� ����������� 10 �����
                        if (cls[0].ObjectKey == "0299a09f-9d23-4e6c-b39a-930cbe219c3a")
                            codeValues[0] += "0000000000";

                        // ��������� ������ �� �������������� �� ���������� �����
                        int count = codeValues.GetLength(0);
                        for (int i = 0; i < count; i++)
                        {
                            if (clsTable[i] == null)
                                continue;

                            string code = codeValues[i];
                            if (this.DataSource.Year >= 2005)
                                code = code.TrimStart('0').PadLeft(1, '0');
                            // ������� � ���� �������� � ����������
                            code = code.Replace('�', 'A');
                            code = code.Replace('�', 'a');
                            clsValuesMapping[i * 2 + 1] = PumpCachedRow(codesMapping[i], clsTable[i], cls[i], code,
                                new object[] { GetClsCodeField(cls[i]), code, "NAME", constDefaultClsName, "KL", -1, "KST", -1 });
                        }
                        break;
                    case BlockProcessModifier.MROutcomesBooksEx:
                    case BlockProcessModifier.MRCommonBooks:
                    case BlockProcessModifier.MRExcessBooks:
                        clsID = nullRefsToCls[yearIndex];
                        if (codesMapping != null)
                        {
                            foreach (KeyValuePair<string, int> item in codesMapping[0])
                            {
                                if (item.Key.TrimStart('0') == clsCode.TrimStart('0'))
                                {
                                    clsID = item.Value;
                                    break;
                                }
                            }
                        }
                        else
                            clsID = FindRowID(clsTable[yearIndex],
                                new object[] { clsCodeField, clsCode }, nullRefsToCls[yearIndex]);
                        clsValuesMapping[yearIndex * 2 + 1] = clsID;
                        break;
                    case BlockProcessModifier.YREmbezzles:
                        break;

                    default:
                        if (yearIndex >= 0)
                        {
                            if (codesMapping != null)
                                clsID = FindCachedRow(codesMapping[0], clsCode, nullRefsToCls[yearIndex]);
                            else
                                clsID = FindRowID(clsTable[yearIndex],
                                    new object[] { clsCodeField, clsCode }, nullRefsToCls[yearIndex]);
                            clsValuesMapping[yearIndex * 2 + 1] = clsID;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("������ ��� ������������ ����� ������������� �� {0}", clsCode), ex);
            }
            return true;
        }

        /// <summary>
        /// ���������� ������ ���������� ����� ��� ��������� �����
        /// </summary>
        /// <param name="codeExclusions">������ ����������</param>
        /// <param name="xmlForm">�����</param>
        /// <returns>������ ���������� ��� �����</returns>
        private string[] GetCodesExclusions4XmlForm(Dictionary<XmlForm, string[]> codeExclusions, XmlForm xmlForm)
        {
            if (codeExclusions.ContainsKey(xmlForm))
            {
                return codeExclusions[xmlForm];
            }
            else if (codeExclusions.ContainsKey(XmlForm.UnknownForm))
            {
                return codeExclusions[XmlForm.UnknownForm];
            }

            return new string[0];
        }

        /// <summary>
        /// ���� ��������� ����� � ������� ����
        /// </summary>
        /// <param name="xmlForm">�����</param>
        /// <param name="xmlForms">������ ����</param>
        /// <returns>���� ��� ���</returns>
        private bool FindXmlForm(XmlForm xmlForm, XmlForm[] xmlForms)
        {
            foreach (XmlForm form in xmlForms)
            {
                if (form == xmlForm)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// ��� ������ ���������, ���� �� ������ �� ��������� ������ � ������� � ����������, ��� ������, � ��� - ���
        /// </summary>
        /// <param name="xn">���� � ������� ������ �� ������</param>
        /// <param name="formSection">������ ��� �����_�����-�����_������. ������ ���� ����������, ������ �� ����� 
        /// ������ � ���� ����� �������� (������������ ������ ������). ������ ���� ������ ���� ����������� ����� ";".
        /// ������ �����_������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� � 
        /// �������� 2)</param>
        /// <returns>����������������� �������� formSection</returns>
        private string[] CheckFormForPump(XmlNode xn, string[] formSection)
        {
            string[] result = new string[formSection.GetLength(0)];
            Array.Copy(formSection, result, formSection.GetLength(0));

            int count128 = 0;
            int count428 = 0;

            // ���� �������, ������� ��������� ����������� ����� ��, �������� � ������������ ������ �� 128 �����, 
            // � ����������������� ����� �� 428.
            // � xml-����� ����������������� ����� ����� ���� ����������� ������������ � 128 � 428 ������, 
            // ������ � 128 ����� ��� ������ � 428 �����.

            for (int i = 0; i < result.GetLength(0); i += 2)
            {
                XmlForm[] xmlForms = StringToXmlForms(result[i]);
                for (int j = 0; j < xmlForms.GetLength(0); j++)
                {
                    int count = GetTotalDataNodes(xn, new string[] { result[i], result[i + 1] });

                    switch (xmlForms[j])
                    {
                        case XmlForm.Form128:
                        case XmlForm.Form128v:
                            count128 += count;
                            break;

                        case XmlForm.Form428:
                        case XmlForm.Form428g:
                        case XmlForm.Form428Vg:
                        case XmlForm.Form428v:
                            count428 += count;
                            break;
                    }
                }
            }

            for (int i = 0; i < result.GetLength(0); i += 2)
            {
                // ��� ������ �� ����� 128
                if (count428 == 0)
                {
                    result[i] = result[i].Replace("428v", string.Empty);
                    result[i] = result[i].Replace("428", string.Empty);
                }
                else
                    // � 428 ������ ����, �� 128 ������ �� �����
                    count128 = 0;
                // ��� ������ �� ����� 128
                if (count128 == 0)
                {
                    result[i] = result[i].Replace("128v", string.Empty);
                    result[i] = result[i].Replace("128", string.Empty);
                }

                result[i] = result[i].Replace(";;", ";").Trim(';');
            }

            return result;
        }

        /// <summary>
        /// �������� ��������� ��� ���������
        /// </summary>
        /// <param name="docType"> ��� ��������� </param>
        /// <returns></returns>
        public string GetSKIFDocType(int docType)
        {
            switch (docType)
            {
                case 0:
                    return "�������� �� �������";
                case 1:
                    return "����������� ��� ����������";
                case 2:
                    return "����������� ���������� ������������� �����������";
                case 3:
                    return "����������������� ���������� � ���������� ������������ ��������������� ������";
                case 4:
                    return "���������� ������� �������������� ������� �������";
                case 5:
                    return "����������� ����� �� ������� ��������";
                case 9:
                    return "����������������� ����� ������������� �����������";
                case 10:
                    return "����������������� ����� �������������� ������";
                case 12:
                    return "����� ������������� �����";
                case 20:
                    return "������ � ������� ������������� �����������";
                case 21:
                    return "������ � ������� ������������� ����������� � ���������";
                default: 
                    return string.Empty;
            }
        }

        private void FormCLSFromReport(string code, BlockProcessModifier blockProcessModifier, Dictionary<string, int> cache,
            DataTable[] clsTable, IClassifier[] cls)
        {
            switch (blockProcessModifier)
            {
                case BlockProcessModifier.MRIncomes:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(cache, clsTable[0], cls[0], code, 
                            new object[] { "CODESTR", code, "NAME", "����������� ������������", "KL", 0, "KST", 0 });
                    break;
                case BlockProcessModifier.MRSrcInFin:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(cache, clsTable[0], cls[0], code, 
                            new object[] { "CODESTR", code, "NAME", "����������� ������������", "KL", 0, "KST", 0 });
                    break;
                case BlockProcessModifier.MRSrcOutFin:
                    if (this.DataSource.Year >= 2005)
                        PumpCachedRow(cache, clsTable[0], cls[0], code, 
                            new object[] { "CODESTR", code, "NAME", "����������� ������������", "KL", 0, "KST", 0 });
                    break;
            }
        }

        // ���� ������ �� ����� ������ ��� ��������, ������ �� ����� ������������ ����� - ������ �� ���� ����� ���������� (return false)
        private bool CheckFormPriority(string regionKey, XmlForm form)
        {
            switch (form.ToString().ToUpper())
            {
                case "FORM127":
                case "FORM127G":
                case "FORM127V":
                    // 127 ����� - ����� ������������, ������ ������
                    if (!forcePumpForm127)
                        return true;
                    else
                        // ���� �������������� ������� 127 �����, �� ������, ���� �� ������ �� �������� ������� ������
                        return (!pumpedRegions.ContainsKey(regionKey) || (pumpedRegions[regionKey].Contains(XmlForm.Form127) ||
                                                                          pumpedRegions[regionKey].Contains(XmlForm.Form127v) ||
                                                                          pumpedRegions[regionKey].Contains(XmlForm.Form127g)));
                case "FORM628R":
                case "FORM428":
                case "FORM428V":
                case "FORM428G":
                case "FORM428VG":
                case "FORM128":
                case "FORM128V":
                    // �� ���� ���� ������, ���� �� ���� � 127
                    if (!pumpedRegions.ContainsKey(regionKey))
                        return true;
                    return (!pumpedRegions[regionKey].Contains(XmlForm.Form127) && !pumpedRegions[regionKey].Contains(XmlForm.Form127v) &&
                            !pumpedRegions[regionKey].Contains(XmlForm.Form127g));
                case "FORM117":
                    // �� 117 ������, ���� ������ �� ���� � ���� ������ (127, 128, 428, 628)
                    return (!pumpedRegions.ContainsKey(regionKey) || (pumpedRegions[regionKey].Contains(XmlForm.Form117)));
            }
            // ��������� ����� � �������� �� �������������� �� ���������, ������
            return true;
        }

        private int CleanIntValue(string value)
        {
            value = CommonRoutines.TrimLetters(value.Trim()).Trim();
            return Convert.ToInt32(value.PadLeft(1, '0'));
        }

        // ������� ������ �� ������ ���������
        private void PumpXMLData(XmlNodeList xnlData, IDbDataAdapter da, DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, string[] factRefsToCls, int[] nullRefsToCls,
            Dictionary<string, int>[] codesMapping, string[] codeExcl, BlockProcessModifier blockProcessModifier,
            string clsCodeAttr, int nullRegions, string[] attr2ClsMapping, string date, int yearIndex,
            object[] clsValuesMapping, string clsCodeField, string regionCode, string regionKey, int regionID,
            XmlForm xmlForm, object[] individualCodesMapping, int vbAttrValue)
        {
            List<XmlForm> formsList = null;
            if (!pumpedRegions.ContainsKey(regionKey))
            {
                formsList = new List<XmlForm>();
                pumpedRegions.Add(regionKey, formsList);
            }
            else
                formsList = pumpedRegions[regionKey];
            if (!formsList.Contains(xmlForm))
                formsList.Add(xmlForm);

            bool toPumpOutcomesRefs = true;
            bool toPumpInDebtRefs = false;
            bool toPumpOutDebtRefs = false;
            bool toPumpArrearsRefs = false;
            bool toPumpExcessRefs = false;

            if (this.DataSource.Year >= 2010)
                toPumpOutcomesRefs = false;

            int sourceDate = this.DataSource.Year * 100 + this.DataSource.Month;
            for (int k = 0; k < xnlData.Count; k++)
            {
                string clsCode = GetClsCodeFromFactNode(clsCodeAttr, xnlData[k], blockProcessModifier);

                #region
                if (this.DataSource.Year >= 2008)
                {
                    decimal clsCodeInt = 0;
                    if (this.DataSource.Year < 2010)
                    {
                        if ((blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx) ||
                            (blockProcessModifier == BlockProcessModifier.MRCommonBooks) ||
                            (blockProcessModifier == BlockProcessModifier.MRExcessBooks))
                            clsCodeInt = Convert.ToDecimal(clsCode.TrimStart('0').PadLeft(1, '0'));
                    }
                    switch (blockProcessModifier)
                    {
                        case BlockProcessModifier.MROutcomesBooksEx:
                            //if ((this.DataSource.Year < 2010) && (sectNoList[0] != 2))
                            //   break;
                            if (this.DataSource.Year >= 2009)
                            {
                                // 2009 - ���������� �� ���� 0000000000000000000009700
                                if (clsCode == "0000000000000000000009700")
                                    toPumpOutcomesRefs = false;
                                // c ������ 2009 ���� - ����������.������_����������  (<;10100, 0000000000000000022511400;>]
                                if (sourceDate >= 200904)
                                {
                                    if (clsCode == "0000000000000000022511400")
                                        toPumpOutcomesRefs = true;
                                }
                            }
                            // 2008 - ���������� �� ���� 000000000000000000008200
                            else
                            {
                                if (clsCode == "000000000000000000008200")
                                    toPumpOutcomesRefs = false;
                            }
                            //   if (!toPumpOutcomesRefs)
                            //      continue;
                            break;
                        case BlockProcessModifier.MRCommonBooks:
                        case BlockProcessModifier.MRExcessBooks:
                            if (factRefsToCls[0] == "REFMARKSINDEBT")
                            {
                                if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 - ���������� ���� [0000000000000000000009700,0000000000000000000009800)
                                    // [0000000000000000000009900, 0000000000000000000010000)
                                    toPumpInDebtRefs = (((clsCodeInt >= 9700) && (clsCodeInt < 9800)) ||
                                                        ((clsCodeInt >= 9900) && (clsCodeInt < 10000)));
                                }
                                else
                                {
                                    // 2008 - ���������� ���� [000000000000000000008200,000000000000000000008300)
                                    // [000000000000000000008400, 000000000000000000008500)
                                    toPumpInDebtRefs = (((clsCodeInt >= 8200) && (clsCodeInt < 8300)) ||
                                                        ((clsCodeInt >= 8400) && (clsCodeInt < 8500)));
                                }
                                //        if (!toPumpInDebtRefs)
                                //           continue;
                            }
                            else if (factRefsToCls[0] == "REFMARKSOUTDEBT")
                            {
                                if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 - ������ ���� [0000000000000000000009800, 0000000000000000000009900)
                                    toPumpOutDebtRefs = ((clsCodeInt >= 9800) && (clsCodeInt < 9900));
                                }
                                else
                                {
                                    // 2008 - ������ ���� [000000000000000000008300, 000000000000000000008400)
                                    toPumpOutDebtRefs = ((clsCodeInt >= 8300) && (clsCodeInt < 8400));
                                }
                                //        if (!toPumpOutDebtRefs)
                                //           continue;
                            }
                            else if (factRefsToCls[0] == "REFMARKSARREARS")
                            {
                                // 2009 - ������ ����� 0000000000000000000010100 (�������)
                                if (this.DataSource.Year >= 2009)
                                {
                                    if (clsCode == "0000000000000000000010100")
                                        toPumpArrearsRefs = true;
                                    // c ������ 2009 ���� - ����������.������_����������������  [0000000000000000000010100, 22511400)
                                    if (sourceDate >= 200904)
                                    {
                                        if (clsCode == "0000000000000000022511400")
                                            toPumpArrearsRefs = false;
                                    }
                                }
                                else
                                    // 2008 - ������ ����� 000000000000000000008600 (�������)
                                    if (clsCode == "000000000000000000008600")
                                        toPumpArrearsRefs = true;
                                //  if (!toPumpArrearsRefs)
                                //     continue;
                            }
                            else if (factRefsToCls[0] == "RefMarks")
                            {
                                if (this.DataSource.Year >= 2009)
                                {
                                    // 2009 - ������ ���� [0000000000000000000010000, 0000000000000000000010100)
                                    toPumpExcessRefs = ((clsCodeInt >= 10000) && (clsCodeInt < 10100));
                                }
                                else
                                {
                                    // 2008 - ������ ���� [000000000000000000008500, 000000000000000000008600)
                                    toPumpExcessRefs = ((clsCodeInt >= 8500) && (clsCodeInt < 8600));
                                }
                                //     if (!toPumpExcessRefs)
                                //        continue;
                            }
                            break;
                    }
                }
                else if (this.DataSource.Year * 100 + this.DataSource.Month >= 200702)
                {
                    if (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx)
                    {
                        // ���������� �� ���� 000000000000000000007000
                        if (clsCode == "000000000000000000007000")
                            toPumpOutcomesRefs = false;
                        if (!toPumpOutcomesRefs)
                            continue;
                    }
                    else if (factRefsToCls != null)
                    {
                        if (factRefsToCls[0] == "REFMARKSARREARS")
                        {
                            // ������ ����� 000000000000000000007000 (�������)
                            if (clsCode == "000000000000000000007000")
                                toPumpArrearsRefs = true;
                            if (!toPumpArrearsRefs)
                                continue;
                        }
                    }
                }
                #endregion

                // ��� 117 ����� - ��� ������ ��������������, ���� ��� ���������� �������� - ���������� ������
                switch (blockProcessModifier)
                {
                    case BlockProcessModifier.MRSrcOutFin:
                    case BlockProcessModifier.MRSrcInFin:
                        if ((xmlForm == XmlForm.Form117) && (clsCode == string.Empty))
                            continue;
                        break;
                }

                switch (xmlForm)
                {
                    case XmlForm.Form128:
                    case XmlForm.Form128v:
                        if (this.skifFormat == SKIFFormat.MonthReports)
                        {
                            // ����� ���� ��������������� ��������� � ������������ ��������
                            clsCode = GetClsCodeFromFactNode(
                                clsCodeAttr, xnlData[k].ParentNode, blockProcessModifier).Remove(0, 2) + clsCode;
                        }
                        break;
                }

                if (this.DataSource.Year < 2005)
                    if ((blockProcessModifier == BlockProcessModifier.MROutcomesBooks) ||
                        (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx))
                    {
                        string psr = clsCode.Substring(clsCode.Length - 6);
                        string psrCodes = "110100,110200,110310,110330,110700,130100,130200,130300,240110";
                        if (blockProcessModifier == BlockProcessModifier.MROutcomesBooks)
                            if (!psrCodes.Contains(psr))
                                continue;
                        if (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx)
                            if (psrCodes.Contains(psr))
                                continue;
                    }

                if (CheckCodeExclusion(clsCode, codeExcl))
                    continue;

                sumMultiplier = 1;
                #region ��������� ��� �������������� �� ������
                if (clsCode != string.Empty)
                    switch (blockProcessModifier)
                    {
                        case BlockProcessModifier.MROutcomesBooksEx:
                        case BlockProcessModifier.MRExcessBooks:
                            if (this.DataSource.Year >= 2010)
                                clsCode = clsCode.Remove(5, 3);
                            break;
                        case BlockProcessModifier.MRCommonBooks:
                            if (this.DataSource.Year >= 2010)
                            {
                                if ((factRefsToCls[0] == "REFMARKSARREARS") ||
                                (factRefsToCls[0] == "REFMARKSINDEBT") ||
                                (factRefsToCls[0] == "REFMARKSOUTDEBT"))
                                    clsCode = clsCode.Remove(5, 3);
                            }
                            break;
                        case BlockProcessModifier.MRIncomes:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            // ������� � ���� �������� � ����������
                            clsCode = clsCode.Replace('�', 'A');
                            clsCode = clsCode.Replace('�', 'a');
                            FormCLSFromReport(clsCode, blockProcessModifier, codesMapping[0], clsTable, cls);
                            break;
                        case BlockProcessModifier.MRSrcInFin:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            // ������� � ���� �������� � ����������
                            clsCode = clsCode.Replace('�', 'A');
                            clsCode = clsCode.Replace('�', 'a');
                            if (GetAttrValueByName(xnlData[k].Attributes, "�������", "����", "ClsfCode") != string.Empty)
                                FormCLSFromReport(clsCode, blockProcessModifier, codesMapping[0], clsTable, cls);
                            break;
                        case BlockProcessModifier.MRSrcOutFin:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            // ������� � ���� �������� � ����������
                            clsCode = clsCode.Replace('�', 'A');
                            clsCode = clsCode.Replace('�', 'a');
                            if (GetAttrValueByName(xnlData[k].Attributes, "��������", "�����", "ClsfCode") != string.Empty)
                                FormCLSFromReport(clsCode, blockProcessModifier, codesMapping[0], clsTable, cls);
                            break;
                        case BlockProcessModifier.YRSrcFin:
                        case BlockProcessModifier.YRIncomes:
                            if (this.DataSource.Year >= 2005)
                                clsCode = clsCode.PadLeft(20, '0');
                            break;
                        case BlockProcessModifier.YRNet:
                            // ���
                            int auxCode = Convert.ToInt32(clsCode.Substring(0, 4));
                            clsValuesMapping[1] = PumpCachedRow(codesMapping[0], clsTable[0], cls[0], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // ����
                            auxCode = Convert.ToInt32(clsCode.Substring(4, 7));
                            clsValuesMapping[3] = PumpCachedRow(codesMapping[1], clsTable[1], cls[1], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // ���
                            auxCode = Convert.ToInt32(clsCode.Substring(11, 3));
                            clsValuesMapping[5] = PumpCachedRow(codesMapping[2], clsTable[2], cls[2], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // ����
                            auxCode = Convert.ToInt32(clsCode.Substring(16, 3));
                            if ((auxCode == 850) || ((auxCode >= 500) && (auxCode < 700)))
                                sumMultiplier = 1000;
                            else
                                sumMultiplier = 1;

                            clsValuesMapping[7] = PumpCachedRow(codesMapping[3], clsTable[3], cls[3], auxCode.ToString(),
                                new object[] { "Code", auxCode, "Name", constDefaultClsName });
                            // ������ - ��� = 16 ������ �������� ������������ ����
                            string subKvrCode = clsCode.Substring(0, 16);
                            // ������ ���������� � �� �������
                            subKvrCode = subKvrCode.ToUpper().Replace('X', '�');
                            clsValuesMapping[9] = FindCachedRow(codesMapping[4], subKvrCode, nullRefsToCls[4]);
                            if (Convert.ToInt32(clsValuesMapping[9]) == nullRefsToCls[4])
                            {
                                // ������ ������� � �� ����������
                                subKvrCode = subKvrCode.ToUpper().Replace('�', 'X');
                                clsValuesMapping[9] = FindCachedRow(codesMapping[4], subKvrCode, nullRefsToCls[4]);
                            }
                            break;
                        case BlockProcessModifier.MRArrears:
                            clsValuesMapping[1] = FindCachedRow(codesMapping[0], clsCode, nullRefsToCls[0]);
                            break;
                        case BlockProcessModifier.YRBalanc:
                            if ((xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002) || (xmlForm == XmlForm.Form43002))
                            {
                                // ��� ����� "������_�������" ���� � �������������� ��� ������ � ��������������� �����,
                                // �� ��������� ��� � ������� �������������� � ����������� �������������
                                clsCode = Convert.ToInt32(clsCode.Trim()).ToString();
                                if (!codesMapping[0].ContainsKey(clsCode))
                                {
                                    clsValuesMapping[1] = PumpCachedRow(codesMapping[0], clsTable[0], cls[0],
                                        clsCode, new object[] { "Code", clsCode, "Name", constDefaultClsName });
                                }
                            }
                            else
                            {
                                clsValuesMapping[1] = FindCachedRow(codesMapping[0], clsCode, nullRefsToCls[0]);
                            }
                            break;
                    }
                #endregion

                #region �������� ������ �� ��� � ���
                if (((xmlForm == XmlForm.Form117) || ((xmlForm == XmlForm.Form127) ||
                     (xmlForm == XmlForm.Form127g) || (xmlForm == XmlForm.Form127v))) &&
                    (blockProcessModifier == BlockProcessModifier.MROutcomes))
                {
                    int fkr = CleanIntValue(xnlData[k].ParentNode.Attributes["����"].Value);
                    int kcsr = CleanIntValue(xnlData[k].ParentNode.Attributes["���"].Value);
                    int kvr = CleanIntValue(xnlData[k].ParentNode.Attributes["��"].Value);
                    string name = constDefaultClsName;
                    if (kvr != 0)
                    {
                        if (kvrAuxCache.ContainsKey(kvr))
                            name = kvrAuxCache[kvr];
                    }
                    else if (kcsr != 0)
                    {
                        if (kcsrAuxCache.ContainsKey(kcsr))
                            name = kcsrAuxCache[kcsr];
                    }
                    else if (fkr != 0)
                    {
                        if (fkrAuxCache.ContainsKey(fkr))
                            name = fkrAuxCache[fkr];
                    }
                    if (name.Trim() == string.Empty)
                        name = constDefaultClsName;

                    string fkrCode = string.Format("{0}{1}{2}", fkr, kcsr.ToString().PadLeft(7, '0'),
                        kvr.ToString().PadLeft(3, '0'));
                    clsValuesMapping[1] = PumpCachedRow(codesMapping[0], clsTable[0], cls[0], fkrCode,
                        new object[] { "Code", fkrCode, "Name", name });

                    string ekr = clsCode;
                    clsValuesMapping[3] = PumpCachedRow(codesMapping[1], clsTable[1], cls[1], ekr,
                        new object[] { "Code", ekr, "Name", constDefaultClsName });
                    if (xmlForm != XmlForm.Form117)
                    {
                        string kvsr = xnlData[k].ParentNode.Attributes["���"].Value.TrimStart('0').PadLeft(1, '0');
                        clsValuesMapping[5] = FindCachedRow(codesMapping[2], kvsr, nullRefsToCls[2]);
                    }
                }
                else
                {
                    if (blockProcessModifier != BlockProcessModifier.YRNet)
                        if (!GetClsIDForFact(clsTable, cls, factRefsToCls, nullRefsToCls, codesMapping,
                            blockProcessModifier, yearIndex, clsValuesMapping, clsCodeField, clsCode, attr2ClsMapping))
                            continue;
                }
                #endregion

                // ��� �������������� � ����� ������� ���, �������������� ������ �� ������ ����
                if ((blockProcessModifier == BlockProcessModifier.MRCommonBooks) ||
                    (blockProcessModifier == BlockProcessModifier.MRExcessBooks) ||
                    (blockProcessModifier == BlockProcessModifier.MROutcomesBooksEx))
                    if (Convert.ToInt32(clsValuesMapping[1]) == nullRefsToCls[0])
                        continue;

                // ���������� ������ �� ����� ��������������
                ProcessPxNodes(da, factTable, fct, xnlData[k], xmlForm, regionID, date, regionCode,
                    (object[])CommonRoutines.ConcatArrays(clsValuesMapping, individualCodesMapping),
                    blockProcessModifier, nullRegions, vbAttrValue);

                // ��������� ������, ���� �����
                if (factTable.Rows.Count >= constMaxQueryRecords)
                {
                    UpdateData();
                    ClearDataSet(da, factTable);
                }
            }
        }

        private string GetReportDate()
        {
            switch (this.SkifReportFormat)
            {
                case SKIFFormat.MonthReports:
                    return string.Format("{0}{1:00}00", this.DataSource.Year, this.DataSource.Month);

                case SKIFFormat.YearReports:
                    return string.Format("{0}", this.DataSource.Year);
            }
            return string.Empty;
        }

        private int GetArrearsYear(XmlNode document)
        {
            string year = GetAttrValueByName(document.Attributes, "���");
            return Convert.ToInt32(year.Trim().PadLeft(1, '0'));
        }

        private int GetBalancVB(XmlNode document, XmlForm xmlForm)
        {
            if ((xmlForm == XmlForm.Form12001) || (xmlForm == XmlForm.Form13001) ||
                (xmlForm == XmlForm.Form12002) || (xmlForm == XmlForm.Form13002))
            {
                return Convert.ToInt32(GetAttrValueByName(document.Attributes, "��").Trim().PadLeft(1, '0'));
            }
            return -1;
        }

        /// <summary>
        /// ���������� ������ ����� ������
        /// </summary>
        /// <param name="blockName">�������� ����� (��� ���������)</param>
        /// <param name="xn">���� ������</param>
        /// <param name="formSection">������ ��� �����_�����-�����_������. ������ ���� ����������, ������ �� �����
        /// ������ � ���� ����� �������� (������������ ������ ������). ������ ���� ������ ���� ����������� ����� ";".
        /// ������ �����_������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� �
        /// �������� 2)</param>
        /// <param name="da">����������� ������� ������</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="consDa">����������� ������� ������ ����. �������</param>
        /// <param name="consFactTable">������� ������ ����. �������</param>
        /// <param name="consFct">IFactTable ����. �������</param>
        /// <param name="clsTable">������� ���������������</param>
        /// <param name="cls">������� ���������������, ��������������� clsTable</param>
        /// <param name="clsYears">����, ������������ � ������ ������ ���� � ����� ������������� �� clsTable ������</param>
        /// <param name="factRefsToCls">������ �� ������� ������ �� �������������� clsTable</param>
        /// <param name="nullRefsToCls">������ �� ������� ������ �� �������������� clsTable, ������ null</param>
        /// <param name="progressMsg">��������� ���������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">���������� ���� �������������� (��������� ���� �� ������������).
        /// ������ ��� Rule - CodePart, ��� CodePart - ����� �������� ����, � �������� ����� ��������� �������
        /// ���������� Rule. ���� ������������ ������ Rule, �.�. ������ �������� 1 �������, �� ��� ������� �����
        /// ��������� �� ����� ����. ���� ��� ������������� ���� �� ������ �� ������, �� �� ����� ��������.
        /// ������ ��������� �������:
        /// "rule1;rule2" - ��������� ������, ����������� ����������, ����� ���� ����������� ����� ����� � �������, ������������ �� �;
        /// ������ ����� ��. � PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// ������ ������:
        /// "code" - ����������� ����, ������ ����������;
        /// "code*" - ����������� ����, ������������ � ����������;
        /// "*code" - ����������� ����, ��������������� �� ���������;
        /// "code1..code2" - ����������� ����, �������� � �������� code1..code2;
        /// ">=code" - ����������� ���� >= code;
        /// "code" - ����������� ����, ������� ��� ������ code (����� code ���� ������-����� ����� �������, �
        /// ����� ����������� �� ��������� :).
        /// �������� ������:
        /// "!" - ���������; "#" - ������������� ������� (���� ��� ��� �� �������������, �� �� �� ����� ��������
        /// ��� ����������� �� ������ ������; ��� ������� ������ ���� ������)</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="clsCodeAttr">������������ �������� � ����� ��������������. ������ ������ - ������������
        /// ���� ���������, "attrName+attrName" - ����� ����������� ������������ �������� ��������� ���������.</param>
        /// <param name="regionsCache">��� �������������� ������</param>
        /// <param name="nullRegions">�� ������ �� ����������� ������ �������������� ������</param>
        /// <param name="attr2ClsMapping"> ������ ����������_�������� ������������� ���� ��������������, ������ ����� �������
        /// �������� ��� ��������� ���� ���������������.
        /// ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="regions4PumpSkifCache">��� �������������� ������.���������</param>
        /// <param name="regionsUnitCache">��� �������������� ������ ��� ����. �������</param>
        /// <param name="nullRegionsUnit">�� ������ �� ����������� ������ �������������� ������ ��� ����. �������</param>
        protected void PumpXMLReportBlock(string blockName, XmlNode xn, string[] formSection, IDbDataAdapter da,
            DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls,
            string progressMsg, Dictionary<string, int>[] codesMapping, Dictionary<XmlForm, string[]> codeExclusions,
            BlockProcessModifier blockProcessModifier, string clsCodeAttr, Dictionary<string, int> regionsCache,
            int nullRegions, string[] attr2ClsMapping, Dictionary<string, int> regions4PumpSkifCache)
        {
            #region ������ ����� ���������� ��������� ������
            int totalDataNodes = GetTotalDataNodes(xn, formSection);
            WriteToTrace(string.Format("{0} ��������� Data.", totalDataNodes), TraceMessageKind.Information);
            if (totalDataNodes == 0)
                return;
            #endregion

            // ��������� ����
            string date = GetReportDate();

            // ���������� ������ ���� �������� ��������� � ������� ���� ���.
            // ����� ��� ����, ����� ����������, ��� ����� �� ������ ��������.
            int yearIndex = GetYearIndexByYear(clsYears);

            #region ��������� ������ �������� ������ �� �������������� � �������������� ��� �������� �� ����������� ������
            object[] clsValuesMapping = null;
            if (factRefsToCls != null)
            {
                clsValuesMapping = new object[factRefsToCls.GetLength(0) * 2];
                for (int i = 0; i < factRefsToCls.GetLength(0); i++)
                {
                    clsValuesMapping[i * 2] = factRefsToCls[i];
                    clsValuesMapping[i * 2 + 1] = nullRefsToCls[i];
                }
            }
            #endregion

            // ���������� �������� ���� ���� ��������������
            string clsCodeField = string.Empty;
            if (yearIndex >= 0 && cls != null)
            {
                clsCodeField = GetClsCodeField(cls[yearIndex]);
            }

            XmlNodeList xnlSources = xn.SelectNodes("//Source");

            for (int i = 0; i < xnlSources.Count; i++)
            {
                string regionCode = GetAttrValueByName(xnlSources[i].Attributes, "ObjectNmb", "Code").PadLeft(10, '0');
                string regionName = GetAttrValueByName(xnlSources[i].Attributes, new string[] { "Name", "ObjectName" });
                string regionKey = regionCode + "|" + regionName;
                string classCode = GetAttrValueByName(xnlSources[i].Attributes, "ClassKey", "ClassCode").ToUpper().Trim();
                int regionID = FindCachedRow(regionsCache, regionKey, nullRegions);

                string[] correctedFormSection = (string[])formSection.Clone();
                //if (this.SkifReportFormat == SKIFFormat.MonthReports)
                correctedFormSection = CheckFormForPump(xnlSources[i], formSection);

                for (int f = 0; f < correctedFormSection.GetLength(0); f += 2)
                {
                    // ������ ���� ������
                    XmlForm[] xmlForm = StringToXmlForms(correctedFormSection[f]);
                    // ������ ������ ����
                    int[] sectNoList = CommonRoutines.ParseParamsString(correctedFormSection[f + 1]);

                    for (int j = 0; j < xmlForm.GetLength(0); j++)
                    {
                        if (xmlForm[j] == XmlForm.UnknownForm)
                            continue;

                        #region ������� - ������ ������
                        // ��� ��� ���������� ����������� �� ������ - ��� ��� ��� ���� ������ ������� ������������
                        // ������� � ������������ � ����������� �� ����� �����...
                        if (xmlForm[j].ToString().ToUpper().StartsWith("FORM127"))
                        {
                            bool isFirstGroupClassCodes =
                                ((classCode == "��") || (classCode == "��1") || (classCode == "����") ||
                                (classCode == "���") || (classCode == "����") || (classCode == "������") ||
                                (classCode == "��� (���)") || (classCode == "���� �����") || (classCode == "����������� ��������") ||
                                (classCode == "�������") || (classCode == "���������"));
                            if (!forcePumpForm127)
                            {
                                // ������ ������� 127 ����� - ������ ������ � ����� ����� "������ ������"
                                if (!isFirstGroupClassCodes)
                                    continue;
                            }
                            else
                            {
                                // ������ ������� 127 ����� - ������ ������ � ����� ����� "������ ������"
                                if (isFirstGroupClassCodes)
                                    continue;
                            }
                        }
                        #endregion

                        if (!CheckFormPriority(regionKey, xmlForm[j]))
                            continue;

                        object[] individualCodesMapping = PrepareIndividualCodesMappingXML(xmlForm[j], blockProcessModifier);

                        string[] codeExcl = GetCodesExclusions4XmlForm(codeExclusions, xmlForm[j]);

                        // �������� ������ XML
                        XmlNodeList documents = GetFactDocumentNodes(xnlSources[i], sectNoList, new XmlForm[] { xmlForm[j] });
                        foreach (XmlNode document in documents)
                        {
                            XmlNodeList xnlData = document.SelectNodes("Data");
                            if (xnlData.Count == 0)
                                continue;

                            #region �������� ��� ������������� (��� MRArrears)
                            if (blockProcessModifier == BlockProcessModifier.MRArrears)
                            {
                                individualCodesMapping = (object[])CommonRoutines.ConcatArrays(individualCodesMapping,
                                    new object[] { "Year", GetArrearsYear(document) });
                            }
                            #endregion
                            #region �������� �������� �������� �� (��� YRBalanc)
                            int vbAttrValue = -1;
                            if (blockProcessModifier == BlockProcessModifier.YRBalanc)
                            {
                                vbAttrValue = GetBalancVB(document, xmlForm[j]);
                            }
                            #endregion

                            PumpXMLData(xnlData, da, factTable, fct, clsTable, cls, factRefsToCls, nullRefsToCls,
                                codesMapping, codeExcl, blockProcessModifier, clsCodeAttr, nullRegions, attr2ClsMapping,
                                date, yearIndex, clsValuesMapping, clsCodeField, regionCode, regionKey,
                                regionID, xmlForm[j], individualCodesMapping, vbAttrValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ���������� ������ ����� ������
        /// </summary>
        /// <param name="blockName">�������� ����� (��� ���������)</param>
        /// <param name="xn">���� ������</param>
        /// <param name="formSection">������ ��� �����_�����-�����_������. ������ ���� ����������, ������ �� �����
        /// ������ � ���� ����� �������� (������������ ������ ������). ������ ���� ������ ���� ����������� ����� ";".
        /// ������ �����_������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� �
        /// �������� 2)</param>
        /// <param name="da">����������� ������� ������</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="consDa">����������� ������� ������ ����. �������</param>
        /// <param name="consFactTable">������� ������ ����. �������</param>
        /// <param name="consFct">IFactTable ����. �������</param>
        /// <param name="clsTable">������� ���������������</param>
        /// <param name="cls">������� ���������������, ��������������� clsTable</param>
        /// <param name="clsYears">����, ������������ � ������ ������ ���� � ����� ������������� �� clsTable ������</param>
        /// <param name="factRefsToCls">������ �� ������� ������ �� �������������� clsTable</param>
        /// <param name="nullRefsToCls">������ �� ������� ������ �� �������������� clsTable, ������ null</param>
        /// <param name="progressMsg">��������� ���������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">���������� ���� �������������� (��������� ���� �� ������������).
        /// ������ ��� Rule - CodePart, ��� CodePart - ����� �������� ����, � �������� ����� ��������� �������
        /// ���������� Rule. ���� ������������ ������ Rule, �.�. ������ �������� 1 �������, �� ��� ������� �����
        /// ��������� �� ����� ����. ���� ��� ������������� ���� �� ������ �� ������, �� �� ����� ��������.
        /// ������ ��������� �������:
        /// "rule1;rule2" - ��������� ������, ����������� ����������, ����� ���� ����������� ����� ����� � �������, ������������ �� �;
        /// ������ ����� ��. � PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// ������ ������:
        /// "code" - ����������� ����, ������ ����������;
        /// "code*" - ����������� ����, ������������ � ����������;
        /// "*code" - ����������� ����, ��������������� �� ���������;
        /// "code1..code2" - ����������� ����, �������� � �������� code1..code2;
        /// ">=code" - ����������� ���� >= code;
        /// "code" - ����������� ����, ������� ��� ������ code (����� code ���� ������-����� ����� �������, �
        /// ����� ����������� �� ��������� :).
        /// �������� ������:
        /// "!" - ���������; "#" - ������������� ������� (���� ��� ��� �� �������������, �� �� �� ����� ��������
        /// ��� ����������� �� ������ ������; ��� ������� ������ ���� ������)</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="clsCodeAttr">������������ �������� � ����� ��������������. ������ ������ - ������������
        /// ���� ���������, "attrName+attrName" - ����� ����������� ������������ �������� ��������� ���������.</param>
        /// <param name="regionsCache">��� �������������� ������</param>
        /// <param name="nullRegions">�� ������ �� ����������� ������ �������������� ������</param>
        /// <param name="attr2ClsMapping"> ������ ����������_�������� ������������� ���� ��������������, ������ ����� �������
        /// �������� ��� ��������� ���� ���������������.
        /// ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="regions4PumpSkifCache">��� �������������� ������.���������</param>
        /// <param name="regionsUnitCache">��� �������������� ������ ��� ����. �������</param>
        /// <param name="nullRegionsUnit">�� ������ �� ����������� ������ �������������� ������ ��� ����. �������</param>
        protected void PumpXMLReportBlock(string blockName, XmlNode xn, string[] formSection, IDbDataAdapter da,
            DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls,
            string progressMsg, Dictionary<string, int>[] codesMapping, string[] codeExclusions,
            BlockProcessModifier blockProcessModifier, string clsCodeAttr, Dictionary<string, int> regionsCache,
            int nullRegions, string[] attr2ClsMapping, Dictionary<string, int> regions4PumpSkifCache)
        {
            Dictionary<XmlForm, string[]> codesExcl = new Dictionary<XmlForm,string[]>(1);
            codesExcl.Add(XmlForm.UnknownForm, codeExclusions);

            PumpXMLReportBlock(blockName, xn, formSection, da, factTable, fct,
                clsTable, cls, clsYears, factRefsToCls, nullRefsToCls, progressMsg, codesMapping, codesExcl,
                blockProcessModifier, clsCodeAttr, regionsCache, nullRegions, attr2ClsMapping, regions4PumpSkifCache);
        }

        /// <summary>
        /// ���������� ������ ����� ������
        /// </summary>
        /// <param name="blockName">�������� ����� (��� ���������)</param>
        /// <param name="xn">���� ������</param>
        /// <param name="formSection">������ ��� �����_�����-�����_������. ������ ���� ����������, ������ �� �����
        /// ������ � ���� ����� �������� (������������ ������ ������). ������ ���� ������ ���� ����������� ����� ";".
        /// ������ �����_������:
        /// "sectNo" - ������������ ������ ��������� ������;
        /// "sectNo1;sectNo2;..." - ������������ ������ sectNo1 � sectNo2;
        /// "sectNo1..sectNo2" - ������������ ������ ������ � sectNo1 �� sectNo2 (�������� ������������� �
        /// �������� 2)</param>
        /// <param name="da">����������� ������� ������</param>
        /// <param name="factTable">������� ������</param>
        /// <param name="fct">IFactTable</param>
        /// <param name="consDa">����������� ������� ������ ����. �������</param>
        /// <param name="consFactTable">������� ������ ����. �������</param>
        /// <param name="consFct">IFactTable ����. �������</param>
        /// <param name="clsTable">������� ���������������</param>
        /// <param name="cls">������� ���������������, ��������������� clsTable</param>
        /// <param name="clsYears">����, ������������ � ������ ������ ���� � ����� ������������� �� clsTable ������</param>
        /// <param name="factRefsToCls">������ �� ������� ������ �� �������������� clsTable</param>
        /// <param name="nullRefsToCls">������ �� ������� ������ �� �������������� clsTable, ������ null</param>
        /// <param name="progressMsg">��������� ���������</param>
        /// <param name="codesMapping">������ ��� ���_�_��� - ��_������_�_�������� ��� ������� ��������������</param>
        /// <param name="codeExclusions">���������� ���� �������������� (��������� ���� �� ������������).
        /// ������ ��� Rule - CodePart, ��� CodePart - ����� �������� ����, � �������� ����� ��������� �������
        /// ���������� Rule. ���� ������������ ������ Rule, �.�. ������ �������� 1 �������, �� ��� ������� �����
        /// ��������� �� ����� ����. ���� ��� ������������� ���� �� ������ �� ������, �� �� ����� ��������.
        /// ������ ��������� �������:
        /// "rule1;rule2" - ��������� ������, ����������� ����������, ����� ���� ����������� ����� ����� � �������, ������������ �� �;
        /// ������ ����� ��. � PumpComplexClsFromInternalPatternXML.AttrValuesMapping.
        /// ������ ������:
        /// "code" - ����������� ����, ������ ����������;
        /// "code*" - ����������� ����, ������������ � ����������;
        /// "*code" - ����������� ����, ��������������� �� ���������;
        /// "code1..code2" - ����������� ����, �������� � �������� code1..code2;
        /// ">=code" - ����������� ���� >= code;
        /// "code" - ����������� ����, ������� ��� ������ code (����� code ���� ������-����� ����� �������, �
        /// ����� ����������� �� ��������� :).
        /// �������� ������:
        /// "!" - ���������; "#" - ������������� ������� (���� ��� ��� �� �������������, �� �� �� ����� ��������
        /// ��� ����������� �� ������ ������; ��� ������� ������ ���� ������)</param>
        /// <param name="blockProcessModifier">����������� ��������� �����. ����� ��� ���������� ��������������
        /// �������� ��� ������-���� �� ������.</param>
        /// <param name="clsCodeAttr">������������ �������� � ����� ��������������. ������ ������ - ������������
        /// ���� ���������, "attrName+attrName" - ����� ����������� ������������ �������� ��������� ���������.</param>
        /// <param name="regionsCache">��� �������������� ������</param>
        /// <param name="nullRegions">�� ������ �� ����������� ������ �������������� ������</param>
        /// <param name="attr2ClsMapping"> ������ ����������_�������� ������������� ���� ��������������, ������ ����� �������
        /// �������� ��� ��������� ���� ���������������.
        /// ������ ����������_��������:
        /// "num" - ����� ������� (-1 - ��� �������);
        /// "num1;num2;..." - ������� num1, num2...;
        /// "num1..num2" - �������� num1..num2 (�������� ������������� � �.2; 0..num - ������ num ��������,
        /// -1..num - ��������� num ��������)</param>
        /// <param name="regions4PumpSkifCache">��� �������������� ������.���������</param>
        /// <param name="regionsUnitCache">��� �������������� ������ ��� ����. �������</param>
        /// <param name="nullRegionsUnit">�� ������ �� ����������� ������ �������������� ������ ��� ����. �������</param>
        protected void PumpXMLReportBlock(string blockName, XmlNode xn, string[] formSection, IDbDataAdapter da,
            DataTable factTable, IFactTable fct,
            DataTable[] clsTable, IClassifier[] cls, int[] clsYears, string[] factRefsToCls, int[] nullRefsToCls,
            string progressMsg, Dictionary<string, int>[] codesMapping, BlockProcessModifier blockProcessModifier,
            string clsCodeAttr, Dictionary<string, int> regionsCache, int nullRegions, string[] attr2ClsMapping,
            Dictionary<string, int> regions4PumpSkifCache)
        {
            PumpXMLReportBlock(blockName, xn, formSection, da, factTable, fct,
                clsTable, cls, clsYears, factRefsToCls, nullRefsToCls, progressMsg, codesMapping, new string[0],
                blockProcessModifier, clsCodeAttr, regionsCache, nullRegions, attr2ClsMapping, regions4PumpSkifCache);
        }

        #endregion ������� ������� ������ ����� XML


        #region ����� ����������� ������� ������� XML

        /// <summary>
        /// ���������� ������ ������� �������
        /// </summary>
        /// <param name="xdReport">������</param>
        protected virtual void PumpExternalXMLPattern(XmlDocument xdPattern)
        {

        }

        /// <summary>
        /// ���������� ������ ������ �������
        /// </summary>
        /// <param name="xdReport">������</param>
        protected virtual void PumpInternalXMLPattern(XmlNode xnPattern)
        {

        }

        protected virtual void PumpKITPattern(DirectoryInfo dir)
        {

        }

        /// <summary>
        /// ���������� ������.
        /// </summary>
        /// <param name="xdPattern">�������� � ����������� �������� (��� ������� �������)</param>
        /// <param name="filesRepList">������ ������ ������ (��� ������ ������� ������ �������)</param>
        private void PumpXMLPattern(XmlDocument xdPattern, FileInfo[] filesRepList, DirectoryInfo dir)
        {
            // ��������� ������ �������
            XmlNode xn = null;
            bool patternIsPumped = false;

            // ������ ����� ���� ������ � ����� �� ������ �������. ������ ���� �� ����
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                xn = GetPatternNode(ConfigureXmlParams(filesRepList[i]));

                if (xn != null)
                {
                    WriteToTrace(string.Format("����� ��������� ������� {0}.", filesRepList[i].Name), TraceMessageKind.Information);
                    PumpInternalXMLPattern(xn);
                    WriteToTrace(string.Format("������ {0} ���������.", filesRepList[i].Name), TraceMessageKind.Information);

                    patternIsPumped = true;
                }
            }

            // ��������� ������� �������
            if (!patternIsPumped && xdPattern != null)
            {
                WriteToTrace(string.Format("����� ��������� �������� �������."), TraceMessageKind.Information);
                PumpExternalXMLPattern(xdPattern);
                patternIsPumped = true;
                WriteToTrace(string.Format("������ ���������."), TraceMessageKind.Information);
            }
            else if (!patternIsPumped)
            {
                if (dir.GetFiles("*.kit", SearchOption.AllDirectories).GetLength(0) != 0)
                {
                    WriteToTrace(string.Format("����� ��������� kit ��������."), TraceMessageKind.Information);
                    PumpKITPattern(dir);
                    WriteToTrace(string.Format("������� kit ����������."), TraceMessageKind.Information);
                }
            }
            else if (!patternIsPumped && xdPattern == null)
            {
                throw new PumpDataFailedException("�� � ����� �� ������ ������ �� ������.");
            }

            UpdateData();
        }

        /// <summary>
        /// ���������� ������ �� ������ ������� ���
        /// </summary>
        /// <param name="xnReport">������� � ������� ������</param>
        protected virtual bool PumpRegionsFromXMLReport(XmlNode xnReport)
        {
            return false;
        }

        /// <summary>
        /// ���������� ����� ������� ���
        /// </summary>
        /// <param name="xnReport">������� � ������� ������</param>
        protected virtual void PumpXMLReport(XmlNode xnReport, string progressMsg)
        {

        }

        /// <summary>
        /// ������������� �������� ��������� ��� ���� � ������ ���� � ����������� �� ����
        /// </summary>
        private XmlDocument ConfigureXmlParams(FileInfo reportFile)
        {
            XmlDocument xdReport = new XmlDocument();
            try
            {
                xdReport.Load(reportFile.FullName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("��� �������� ������ ��������� ������, xml ����� ������������ ������: {0}", ex.Message), ex);
            }
            sumFactor = 1;
            xmlFormat = XmlFormat.Skif3;

            if (((this.DataSource.Year < 2005) || (this.DataSource.Year == 2005 && this.DataSource.Month <= 9)) &&
                reportFile.Name.ToUpper().StartsWith("���"))
            {
                // ��� ������� �� ������� �� ������� 2005 ���������� � �����.
                sumFactor = 1000;

                if (xdReport.SelectSingleNode("/Otchet") != null)
                {
                    xmlFormat = XmlFormat.Format2004;
                }
                else if (xdReport.SelectSingleNode("/RootXml//Report") != null)
                {
                    xmlFormat = XmlFormat.Format2005;
                }
                else
                {
                    throw new Exception("�� ������ ������� � ������� ������");
                }
            }
            else if (this.DataSource.Year < 2006 && reportFile.Name.ToUpper().StartsWith("���"))
            {
                xmlFormat = XmlFormat.October2005;
            }

            return xdReport;
        }

        /// <summary>
        /// ���������� ������ ������� XML
        /// </summary>
        /// <param name="dir">������� �������</param>
        protected void PumpXMLReports(DirectoryInfo dir)
        {
            FileInfo[] filesList = dir.GetFiles("*.xml", SearchOption.AllDirectories);
            if (filesList.GetLength(0) == 0)
                return;
            XmlDocument xdPattern = null;
            // �������� ������ � XML
            // ������ ������ �������
            FileInfo[] filesRepList;
            LoadXMLFiles(filesList, out filesRepList, out xdPattern);

            // ���������� ������
            PumpXMLPattern(xdPattern, filesRepList, dir);

            bool pumpReports = true;

            // ������� ������
            for (int i = 0; i < filesRepList.GetLength(0); i++)
            {
                filesCount++;
                string progressMsg = string.Format("��������� ����� {0} ({1} �� {2})...",
                    filesRepList[i].Name, filesCount, xmlFilesCount);

                if (!File.Exists(filesRepList[i].FullName))
                    continue;

                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeStartFilePumping,
                    string.Format("����� ������� ����� {0}.", filesRepList[i].FullName));

                try
                {
                    // �������� ������� � ��������
                    XmlNode xnReport = GetReportNode(ConfigureXmlParams(filesRepList[i]));
                    if (xnReport == null)
                    {
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeError,
                            "� ����� ����������� ������ ��� �������.");
                        continue;
                    }

                    // ���������� ������
                    if (!PumpRegionsFromXMLReport(xnReport))
                        pumpReports = false;

                    // ���������� ������ ������
                    if (pumpReports)
                    {
                        PumpXMLReport(xnReport, progressMsg);

                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeSuccessfullFinishFilePump,
                            string.Format("���� {0} ������� �������.", filesRepList[i].FullName));
                    }

                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeFinishFilePumpWithError,
                        string.Format("������� �� ����� {0} ��������� � ��������", filesRepList[i].FullName), ex);
                    continue;
                }
                finally
                {
                    xdPattern = null;

                    CollectGarbage();
                }
            }

            if (noRegForPump)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                    "������������� ������.��������� (SOURCEID {0}) ����� ������ � ����������� ����� ������. " +
                    "���������� ���������� �������� ���� \"������������.����\" � ��������� ���� ���������.", regForPumpSourceID));

            // ���������� ������
            UpdateData();
        }

        #endregion ����� ����������� ������� ������� XML
    }
}