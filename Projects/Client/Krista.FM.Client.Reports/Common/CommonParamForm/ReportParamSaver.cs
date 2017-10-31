using System;
using System.IO;
using System.Linq;
using System.Xml;
using Krista.FM.Common.Templates;

namespace Krista.FM.Client.Reports.Common.CommonParamForm
{
    class ReportParamSaver
    {
        private const string ReportsSection = "Reports";
        private const string AttrParamValue = "Value";
        private const string ParamFileName = "ReportParams.xml";
        private readonly ParamContainer paramObject;
        private readonly string commandName;

        public ReportParamSaver(string commandName, ParamContainer paramBuilder)
        {
            paramObject = paramBuilder;
            this.commandName = commandName;
        }

        private bool UseSaveMethods()
        {
            var upperCommanName = commandName.ToUpper();
            return upperCommanName.StartsWith("REPORTUFNS") ||
                upperCommanName.StartsWith("REPORTUFK") ||
                upperCommanName.StartsWith("REPORTMONTH") ||
                upperCommanName.StartsWith("REPORTMOFO");
        }

        private string GetFullFileName()
        {
            return Path.Combine(TemplatesDocumentsHelper.GetDocsFolder(), ParamFileName);
        }

        private XmlDocument GetDocument()
        {
            if (!UseSaveMethods())
            {
                return null;
            }

            var fName = GetFullFileName();
            var xmlDoc = new XmlDocument();

            if (File.Exists(fName))
            {
                xmlDoc.Load(fName);
            }
            else
            {
                xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            }

            return xmlDoc;
        }

        private XmlNode GetParamNode(XmlDocument xmlDoc, bool createStructure)
        {
            if (xmlDoc == null)
            {
                return null;
            }

            var section = xmlDoc.SelectSingleNode(ReportsSection);

            if (section == null)
            {
                if (createStructure)
                {
                    section = xmlDoc.CreateElement(ReportsSection);
                    xmlDoc.AppendChild(section);
                }
                else
                {
                    return null;
                }
            }

            var paramNode = section.SelectSingleNode(commandName);

            if (paramNode == null)
            {
                if (createStructure)
                {
                    paramNode = xmlDoc.CreateElement(commandName);
                    section.AppendChild(paramNode);
                }
                else
                {
                    return null;
                }
            }

            return paramNode;
        }

        public void SaveParams()
        {
            var xmlDoc = GetDocument();
            var paramNode = GetParamNode(xmlDoc, true);

            if (paramNode == null)
            {
                return;
            }

            paramNode.RemoveAll();

            // сохраняем все параметры
            foreach (var paramInfo in paramObject.GetParams())
            {
                var attrNode = xmlDoc.CreateElement(paramInfo.Key);
                var attrValue = xmlDoc.CreateAttribute(AttrParamValue);
                attrValue.Value = Convert.ToString(paramInfo.Value);
                attrNode.Attributes.Append(attrValue);
                paramNode.AppendChild(attrNode);
            }

            xmlDoc.Save(GetFullFileName());
        }

        public void LoadParams()
        {
            var xmlDoc = GetDocument();
            var reportNode = GetParamNode(xmlDoc, false);

            if (reportNode == null)
            {
                return;
            }

            foreach (XmlNode paramNode in reportNode.ChildNodes)
            {
                if (paramNode.Attributes == null)
                {
                    continue;
                }

                var attrNode = paramNode.Attributes.GetNamedItem(AttrParamValue);
                var paramInfo = paramObject.lstParams.Where(f => f.Name == paramNode.Name).FirstOrDefault();

                if (paramInfo != null)
                {
                    paramInfo.DefaultValue = attrNode.Value;
                }
            }
        }
    }
}
