using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using Krista.FM.Common.Exceptions;
using System.Xml.XPath;

namespace Krista.FM.Client.Help
{
    class DocumentSave : CommonSave
    {
        public DocumentSave(HelpManager manager)
            : base(manager)
        {
            CreateTransform();
        }

        protected override void CreateTransform()
        {
            base.CreateTransform();

            xslt.Load(String.Format(@"{0}\Document.xsl", HelpManager.profile));
        }

        /// <summary>
        /// Трансформация документа
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public override void SaveToHtml(System.Xml.XmlNode nodeBase)
        {
            if (nodeBase != null)
            {
                try
                {
                    XmlDocument document = GetXMLDocument(nodeBase);

                    XmlReader reader = XmlReader.Create(new StringReader(document.InnerXml));

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    XmlWriter writer =
                        XmlWriter.Create(
                            String.Format(@"{0}\{1}\diagrams\{2}.html",
                            AppDomain.CurrentDomain.BaseDirectory, HelpManager.result,
                            HelpManager.CheckDocumentName(nodeBase.Attributes["name"].InnerText)), settings);


                    Transform(reader, writer);

                    writer.Close();
                    reader.Close();
                }
                catch (InvalidOperationException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch (ArgumentNullException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch (ArgumentException ex)
                {
                    throw new HelpException(ex.ToString());
                }
            }
        }

        private XmlDocument GetXMLDocument(XmlNode nodeBase)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.InnerXml =
                    String.Format("<?xml version = \"1.0\" encoding = \"windows-1251\"?><ServerConfiguration/>");

                XmlNode document = xmlDocument.CreateElement("Document");
                Krista.FM.Common.Xml.XmlHelper.SetAttribute(document, "type", nodeBase.Attributes["type"].InnerText);
                Krista.FM.Common.Xml.XmlHelper.SetAttribute(document, "name", nodeBase.Attributes["name"].InnerText);
                if (nodeBase.Attributes["description"] != null)
                    Krista.FM.Common.Xml.XmlHelper.SetAttribute(document, "description", nodeBase.Attributes["description"].InnerText);

                if (nodeBase.SelectSingleNode("DeveloperDescription") != null)
                    XmlHelper.AppendCDataSection(document, nodeBase.SelectSingleNode("DeveloperDescription").InnerText);

                // Объекты на диаграмме
                XmlNode diagram = xmlDocument.CreateElement("Diagram");
                XmlNodeList entities = nodeBase.SelectNodes("./Diagram/Symbols/EntitySymbol");

                foreach (XmlNode entity in entities)
                {
                    XmlNode entitySymbol = xmlDocument.CreateElement("EntitySymbol");
                    string s = entity.SelectSingleNode("Object").InnerText;
                    ICommonObject en = SchemeEditor.SchemeEditor.Instance.Scheme.GetObjectByKey(s);
                    if (en != null)
                    {
                        XmlHelper.SetAttribute(entitySymbol, "caption", String.Format("{0}.{1}", ((IEntity)en).SemanticCaption, en.Caption));
                        XmlHelper.SetAttribute(entitySymbol, "objectKey", en.ObjectKey);
                        XmlHelper.SetAttribute(entitySymbol, "typeCls", ((int)((IEntity)en).ClassType).ToString());
                        SetClassType(en, entitySymbol);

                        diagram.AppendChild(entitySymbol);
                    }
                }

                document.AppendChild(diagram);

                xmlDocument.DocumentElement.AppendChild(document);

                ConvertCDATASection(xmlDocument);

                return xmlDocument;
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XPathException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        protected override void ConvertCDATASection(XmlDocument doc)
        {
            ConvertCDATASection converter = new ConvertCDATASection();

            XmlNode node = doc.SelectSingleNode("/ServerConfiguration/Document/DeveloperDescription");

            if (node != null)
            {
                ConvertCDATANode(node, converter);
            }
        }

        /// <summary>
        /// Используется только для сортировки, поскольку индексация на сервере не соответствует требованиям
        /// </summary>
        /// <param name="en"></param>
        /// <param name="entitySymbol"></param>
        private void SetClassType(ICommonObject en, XmlNode entitySymbol)
        {
            switch(((IEntity)en).ClassType)
            {
                case ClassTypes.clsBridgeClassifier:
                    {
                        XmlHelper.SetAttribute(entitySymbol, "typeIndex", "2");
                        break;
                    }
                case ClassTypes.clsDataClassifier:
                    {
                        XmlHelper.SetAttribute(entitySymbol, "typeIndex", "3");
                        break;
                    }
                case ClassTypes.clsFactData:
                    {
                        XmlHelper.SetAttribute(entitySymbol, "typeIndex", "4");
                        break;
                    }
                case ClassTypes.clsFixedClassifier:
                    {
                        XmlHelper.SetAttribute(entitySymbol, "typeIndex", "1");
                        break;
                    }
                case ClassTypes.Table:
                    {
                        XmlHelper.SetAttribute(entitySymbol, "typeIndex", "5");
                        break;
                    }
                case ClassTypes.DocumentEntity:
                    {
                        XmlHelper.SetAttribute(entitySymbol, "typeIndex", "6");
                        break;
                    }
                default:
                    throw new Exception(String.Format("Тип класса {0} не обработан.", ((IEntity)en).ClassType));
            }
        }
    }
}
