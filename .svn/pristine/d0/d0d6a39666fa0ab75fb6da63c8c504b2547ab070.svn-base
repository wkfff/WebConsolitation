using System;
using System.IO;
using System.IO.Packaging;
using System.Xml;

namespace Krista.FM.Common.TaskDocuments.OfficeCustomPropertiesAdapters
{
    public class OfficeOpenXmlAdapter : OfficeCustomPropertiesAdapter
    {
        const string customPropertiesRelationshipType =
              "http://schemas.openxmlformats.org/officeDocument/2006/relationships/custom-properties";
        const string customPropertiesSchema =
            "http://schemas.openxmlformats.org/officeDocument/2006/custom-properties";
        const string customVTypesSchema = 
            "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes";

        private Package package;
        private PackagePart customPropsPart;
        private XmlDocument cpDom;
        private XmlNode rootNode;
        private XmlNamespaceManager nsManager;

        public OfficeOpenXmlAdapter(string file)
        {
            package = Package.Open(file, FileMode.Open, FileAccess.ReadWrite);
            CreateAndReadCustomPropertiesPart();                       
        }

        private void SetNodeAttribute(XmlNode node, string name, string value)
        {
            node.Attributes.Append(node.OwnerDocument.CreateAttribute(name)).Value = value;
        }

        /// <summary>
        /// Читает секцию пользовательских свойств (custom.xml)
        /// Если её нет - создает.
        /// </summary>
        public void CreateAndReadCustomPropertiesPart()
        {
            //  Получаем секцию пользовательских свойств (custom.xml). Её может и не быть.
            foreach (PackageRelationship relationship in
              package.GetRelationshipsByType(customPropertiesRelationshipType))
            {
                Uri documentUri = PackUriHelper.ResolvePartUri(
                  new Uri("/", UriKind.Relative), relationship.TargetUri);
                customPropsPart = package.GetPart(documentUri);
                //  Если такая секция есть, она одна
                break;
            }

            Uri customPropsUri = new Uri("/docProps/custom.xml", UriKind.Relative);
            cpDom = new XmlDocument();
            if (customPropsPart == null)
            {//Секция не найдена. Создаем.
                XmlProcessingInstruction pi = cpDom.CreateProcessingInstruction(
                    "xml", "version='1.0' encoding='UTF-8' standalone='yes'");
                cpDom.AppendChild(pi);
                customPropsPart = package.CreatePart(customPropsUri,
                    "application/vnd.openxmlformats-officedocument.custom-properties+xml");
                rootNode = cpDom.CreateElement("Properties", customPropertiesSchema);
                SetNodeAttribute(rootNode, "xmlns", customPropertiesSchema);
                SetNodeAttribute(rootNode, "xmlns:vt", customVTypesSchema);
                cpDom.AppendChild(rootNode);
                //  Создаем связь документа с новой секцией
                package.CreateRelationship(customPropsUri,
                  TargetMode.Internal, customPropertiesRelationshipType);
                nsManager = new XmlNamespaceManager(cpDom.NameTable);
                nsManager.AddNamespace("d", customPropertiesSchema);
                nsManager.AddNamespace("vt", customVTypesSchema);
            }
            else
            {
                //  Секция найдена, просто загружаем ее XML
                cpDom.Load(customPropsPart.GetStream());
                rootNode = cpDom.DocumentElement;
                nsManager = new XmlNamespaceManager(cpDom.NameTable);
                nsManager.AddNamespace("d", customPropertiesSchema);
                nsManager.AddNamespace("vt", customVTypesSchema);
            }
            cpDom.Save(customPropsPart.
                GetStream(FileMode.Create, FileAccess.Write));
        }

        private string GetPidValue()
        {
            if (rootNode.HasChildNodes)
            {
                XmlNode lastNode = rootNode.LastChild;
                if (lastNode != null)
                {
                    XmlAttribute pidAttr = lastNode.Attributes["pid"];
                    if (!(pidAttr == null))
                    {
                        string pidValue = pidAttr.Value;
                        //  Increment pidValue, so that the new property
                        //  gets a pid value one higher. This value should be 
                        //  numeric, but it never hurt so to confirm.
                        int value = 0;
                        if (int.TryParse(pidValue, out value))
                        {
                            pidValue = Convert.ToString(value + 1);
                        }
                        return pidValue;
                    }
                }
            }
            return "2";
        }

        public override void Clear()
        {
            for (int i = rootNode.ChildNodes.Count - 1; i >= 0; i--)
            {
                XmlNode propNode = rootNode.ChildNodes[i];
                propNode = rootNode.RemoveChild(propNode);
                propNode = null;
            }            
        }

        public void Close()
        {
            if (package != null)
                package.Close();
        }

        public override void Save()
        {
            cpDom.Save(customPropsPart.GetStream(FileMode.Create, FileAccess.Write));
        }

        public void SetTextProperty(string propertyName, string propertyValue)
        {
            XmlNode propertyNode = cpDom.SelectSingleNode(
                string.Format("d:Properties/d:property[@name='{0}']", propertyName), nsManager);
            XmlNode valueNode;
            const string typeName = "vt:lpwstr";

            if (propertyNode != null)
            {//  Если такое свойство уже есть - проверим совпадение типов и поменяем значение
                if (propertyNode.HasChildNodes)
                {
                    valueNode = propertyNode.ChildNodes[0];
                    if (valueNode != null)
                    {
                        string nodeTypeName = valueNode.Name;
                        if (nodeTypeName == typeName)
                        {
                            //  The types are the same. 
                            //  Replace the value of the node.
                            valueNode.InnerText = propertyValue;
                            return;
                        }
                        else
                        {
                            //  Если типы не совпадают - удаляем нахрен.
                            propertyNode.ParentNode.RemoveChild(propertyNode);
                            propertyNode = null;
                        }
                    }
                }
            }

            string pidValue = GetPidValue();

            propertyNode = cpDom.CreateElement("property", customPropertiesSchema);
            propertyNode.Attributes.Append(cpDom.CreateAttribute("name")).Value = propertyName;
            propertyNode.Attributes.Append(cpDom.CreateAttribute("pid")).Value = pidValue;
            propertyNode.Attributes.Append(cpDom.CreateAttribute("fmtid")).Value = "{D5CDD505-2E9C-101B-9397-08002B2CF9AE}";

            valueNode = cpDom.CreateElement("vt", "lpwstr", customVTypesSchema);
            valueNode.InnerText = propertyValue;
            propertyNode.AppendChild(valueNode);

            rootNode.AppendChild(propertyNode);
        }
               

        public override object GetProperty(string name)
        {
            if (rootNode == null)
                return null;
            XmlNode propertyNode = rootNode.SelectSingleNode(
                string.Format("d:property[@name='{0}']", name), nsManager);
            if (propertyNode == null)
                return null;
            return propertyNode.FirstChild.InnerText;
        }

        public override void SetProperty(string name, object value)
        {
            SetTextProperty(name, Convert.ToString(value));
        }

        
        public override void Dispose()
        {
            if (package != null) 
                package.Close();
        }
    }

    public enum PropertyTypes
    {
        YesNo,
        Text,
        DateTime,
        NumberInteger,
        NumberDouble,
    }
}
